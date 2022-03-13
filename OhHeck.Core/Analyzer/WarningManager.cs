using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIoc;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Analyzer.Implementation;
using OhHeck.Core.Helpers;
using Serilog;

namespace OhHeck.Core.Analyzer;

public class WarningManager
{
	private readonly Dictionary<string, IFieldAnalyzer> _beatmapAnalyzers = new();
	private static readonly Type BeatmapAnalyzerType = typeof(IFieldAnalyzer);
	private static readonly Type AnalyzableType = typeof(IAnalyzable);

	[ThreadStatic]
	private static Dictionary<IReflect, IReadOnlyDictionary<MemberInfo, MemberData>> _cachedAnalyzedFields;

	private readonly IContainer _container;
	private readonly ILogger _logger;

	// ReSharper disable once NotAccessedField.Local
	private HashSet<string> _suppressedWarnings = new();

	public WarningManager(IContainer container, ILogger logger)
	{
		_container = container;
		_logger = logger;
	}

	static WarningManager() => _cachedAnalyzedFields = new Dictionary<IReflect, IReadOnlyDictionary<MemberInfo, MemberData>>();

	public void Init(IEnumerable<string> suppressedWarnings, IEnumerable<ConfigureWarningValue> configureWarningValues)
	{
		var registeredCache = new Dictionary<Type, BeatmapWarningAttribute>();

		// Register to DI
		_suppressedWarnings = new HashSet<string>(suppressedWarnings);
		foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
		{
			var warningAttribute = type.GetCustomAttribute<BeatmapWarningAttribute>();

			if (warningAttribute is null || _suppressedWarnings.Contains(warningAttribute.Name))
			{
				continue;
			}

			var existingWarning = _container.Resolve<IFieldAnalyzer>(serviceKey: warningAttribute.Name, IfUnresolved.ReturnDefaultIfNotRegistered);
			if (existingWarning is not null)
			{
				throw new InvalidOperationException($"Beatmap analyzer {warningAttribute} already exists tied to {existingWarning.GetType()}");
			}

			if (!BeatmapAnalyzerType.IsAssignableFrom(type))
			{
				throw new InvalidOperationException($"{type} must inherit {nameof(IFieldAnalyzer)}");
			}

			_logger.Debug("Class {Type} has warning attribute", type);

			_container.Register(typeof(IFieldAnalyzer), type, serviceKey: warningAttribute.Name, reuse: Reuse.Singleton);
			registeredCache[type] = warningAttribute;
		}

		// Now resolve finally dependencies
		var registeredFieldAnalyzers = _container.ResolveMany<IFieldAnalyzer>();

		if (registeredFieldAnalyzers is null)
		{
			return;
		}

		Dictionary<string, List<ConfigureWarningValue>> warningValuesDictionary = new();
		foreach (var configureWarningValue in configureWarningValues)
		{
			if (!warningValuesDictionary.TryGetValue(configureWarningValue.WarningName, out var list))
			{
				warningValuesDictionary[configureWarningValue.WarningName] = list = new List<ConfigureWarningValue>();
			}

			list.Add(configureWarningValue);
		}

		foreach (var registeredFieldAnalyzer in registeredFieldAnalyzers)
		{
			var type = registeredFieldAnalyzer.GetType();
			var warningAttribute = registeredCache[type];
			if (warningValuesDictionary.TryGetValue(warningAttribute.Name, out var scopedWarningValues))
			{
				InjectWarningConfigValues(registeredFieldAnalyzer, type, scopedWarningValues);
			}

			_beatmapAnalyzers[warningAttribute.Name] = registeredFieldAnalyzer;
		}
	}

	private static void InjectWarningConfigValues(IFieldAnalyzer fieldAnalyzer, IReflect type, IEnumerable<ConfigureWarningValue> configureWarningValues)
	{
		// I hate this
		// TODO: Support properties
		// TODO: Support constructor
		var fieldTypes = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).ToDictionary(f => f.GetCustomAttribute<WarningConfigPropertyAttribute>()?.ContractName ?? string.Empty, f => f);

		foreach (var warningValue in configureWarningValues)
		{
			if (!fieldTypes.TryGetValue(warningValue.Property, out var fieldT))
			{
				continue;
			}

			var val = ReflectionUtils.GetAsT(warningValue.Value, fieldT.FieldType);

			fieldT.SetValue(fieldAnalyzer, val);
		}
	}

	// TODO: Rewrite this is confusing even for me
	public void Validate(AnalyzeProcessedData analyzeProcessedData, IWarningOutput warningOutput)
	{
		var (memberType, memberValue, warningContext) = analyzeProcessedData;

		warningOutput.PushWarningInfo(warningContext);

		foreach (var (_, analyzer) in _beatmapAnalyzers)
		{
			analyzer.Validate(memberType, memberValue, warningOutput);
		}

		warningOutput.PopWarningInfo();
	}

	// Nullable
	public ICollection<AnalyzeProcessedData> Analyze(IAnalyzable? analyzable, IAnalyzable? parentOfAnalyzable, Type typeOfAnalyzable, ICollection<AnalyzeProcessedData>? analyzeDatas = null)
	{
		analyzeDatas ??= new List<AnalyzeProcessedData>();

		// Early return
		if (_beatmapAnalyzers.Count == 0)
		{
			return analyzeDatas;
		}

		// null means no fields to analyze
		if (analyzable is null)
		{
			return analyzeDatas;
		}

		var friendlyName = analyzable.GetFriendlyName();
		var memberInfos = GetPublicMembersData(typeOfAnalyzable);


		foreach (var (_, (memberInfo, memberType, friendlyMemberName)) in memberInfos)
		{
			// TODO: Field accessor?
			//get member value
			object? memberValue = null;
			if (analyzable is not null)
			{
				memberValue = memberInfo switch
				{
					PropertyInfo propertyInfo => propertyInfo.GetValue(analyzable),
					FieldInfo fieldInfo => fieldInfo.GetValue(analyzable),
					_ => memberValue
				};
			}

			var analyzeData = new AnalyzeProcessedData(memberType, memberValue, new WarningContext(friendlyName, friendlyMemberName, parentOfAnalyzable));
			analyzeDatas.Add(analyzeData);

			// Analyze lists
			if (memberValue is IEnumerable enumerable)
			{
				AnalyzeEnumerable(enumerable, analyzable, analyzeDatas);
			}

			// If not Analyzable, don't process
			if (!AnalyzableType.IsAssignableFrom(memberType))
			{
				continue;
			}

			// Get
			var fieldValue = (IAnalyzable?) memberValue;

			Analyze(fieldValue, analyzable, memberType, analyzeDatas);
		}

		return analyzeDatas;
	}

	private void AnalyzeEnumerable(IEnumerable enumerable, IAnalyzable? parent, ICollection<AnalyzeProcessedData> analyzeDatas)
	{
		foreach (var o in enumerable)
		{
			if (o is not IAnalyzable analyzable)
			{
				continue;
			}

			Analyze(analyzable, parent, o.GetType(), analyzeDatas);
		}
	}

	private static IReadOnlyDictionary<MemberInfo, MemberData> GetPublicMembersData(IReflect type)
	{
		// Double cache lets go!
		if (_cachedAnalyzedFields.TryGetValue(type, out var cached))
		{
			return cached;
		}

		var memberInfos = ReflectionUtils.GetTypeFieldsSuperRecursive(type);

		// Member object value, MemberType, friendlyMemberName
		Dictionary<MemberInfo, MemberData> memberValues = new();


		// Analyze each field
		foreach (var memberInfo in memberInfos)
		{
			var analyzeMemberAttribute = memberInfo.GetCustomAttribute<AnalyzeMemberAttribute>();
			var friendlyMemberName = analyzeMemberAttribute?.Name ?? memberInfo.Name;

			// get member type
			var memberType = memberInfo switch
			{
				PropertyInfo propertyInfo => propertyInfo.PropertyType,
				FieldInfo fieldInfo => fieldInfo.FieldType,
				_ => null
			};

			memberValues[memberInfo] = new MemberData(memberInfo, memberType!, friendlyMemberName);
		}

		_cachedAnalyzedFields[type] = memberValues;
		return memberValues;
	}

	private record MemberData(MemberInfo MemberInfo, Type MemberType, string FriendlyMemberName);
	// private readonly struct MemberData
	// {
	// 	public readonly object? MemberValue;
	// 	public readonly Type MemberType;
	// 	public readonly string FriendlyMemberName;
	//
	// 	public MemberData(object? memberValue, Type memberType, string friendlyMemberName)
	// 	{
	// 		MemberValue = memberValue;
	// 		MemberType = memberType;
	// 		FriendlyMemberName = friendlyMemberName;
	// 	}
	//
	// 	public void Deconstruct(out object? memberValue, out Type memberType, out string friendlyMemberName)
	// 	{
	// 		memberValue = MemberValue;
	// 		memberType = MemberType;
	// 		friendlyMemberName = FriendlyMemberName;
	// 	}
	// }


}