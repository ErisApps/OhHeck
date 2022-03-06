using System;
using System.Collections.Generic;
using System.Reflection;
using DryIoc;
using OhHeck.Core.Helpers;
using Serilog;

namespace OhHeck.Core.Analyzer;

public class WarningManager
{
	private readonly Dictionary<string, IFieldAnalyzer> _beatmapAnalyzers = new();
	private static readonly Type BeatmapAnalyzerType = typeof(IFieldAnalyzer);
	private static readonly Type AnalyzableType = typeof(IAnalyzable);

	private readonly IContainer _container;
	private readonly ILogger _logger;

	private HashSet<string> _suppressedWarnings = new();

	public WarningManager(IContainer container, ILogger logger)
	{
		_container = container;
		_logger = logger;
	}

	public void Init(HashSet<string> suppressedWarnings)
	{
		_suppressedWarnings = suppressedWarnings;
		foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
		{
			var warningAttribute = type.GetCustomAttribute<BeatmapWarningAttribute>();

			if (warningAttribute is null || suppressedWarnings.Contains(warningAttribute.Name))
			{
				continue;
			}

			if (_beatmapAnalyzers.TryGetValue(warningAttribute.Name, out var existingWarning))
			{
				throw new InvalidOperationException($"Beatmap analyzer {warningAttribute} already exists tied to {existingWarning.GetType()}");
			}

			if (!BeatmapAnalyzerType.IsAssignableFrom(type))
			{
				throw new InvalidOperationException($"{type} must inherit {nameof(IFieldAnalyzer)}");
			}

			_logger.Debug("Class {Type} has warning attribute", type);

			var instance = (IFieldAnalyzer) _container.New(type);
			_beatmapAnalyzers[warningAttribute.Name] = instance;
		}
	}

	// Nullable
	public void Analyze(IAnalyzable? analyzable, IAnalyzable? parent, Type type, IWarningOutput warningOutput)
	{
		// Early return
		if (_beatmapAnalyzers.Count == 0)
		{
			return;
		}

		var friendlyName = analyzable?.GetFriendlyName() ?? type.Name;

		var memberInfos = AnalyzeMemberInfo(analyzable, parent, type, friendlyName, warningOutput);

		if (analyzable is null)
		{
			return;
		}

		// Now to recursively analyze
		foreach (var (_, (memberValue, memberType, friendlyMemberName)) in memberInfos)
		{
			if (!AnalyzableType.IsAssignableFrom(memberType))
			{
				continue;
			}

			warningOutput.PushWarningInfo(new WarningContext(Type: friendlyName, MemberLocation: friendlyMemberName, Parent: parent));
			var fieldValue = (IAnalyzable?) memberValue;
			Analyze(fieldValue, analyzable, memberType, warningOutput);
			warningOutput.PopWarningInfo();
		}
	}

	private Dictionary<MemberInfo, (object?, Type, string)> AnalyzeMemberInfo(IAnalyzable? analyzable, IAnalyzable? parent, IReflect type, string friendlyName, IWarningOutput warningOutput)
	{
		var memberInfos = ReflectionUtils.GetTypeFieldsSuperRecursive(type);

		// Member object value, MemberType, friendlyMemberName
		Dictionary<MemberInfo, (object?, Type, string)> memberValues = new();


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

			memberValues[memberInfo] = (memberValue, memberType!, friendlyMemberName);

			warningOutput.PushWarningInfo(new WarningContext(Type: friendlyName, MemberLocation: friendlyMemberName, Parent: parent));
			foreach (var (_, wAnalyzer) in _beatmapAnalyzers)
			{
				wAnalyzer.Validate(memberType!, memberValue, warningOutput);
			}
			warningOutput.PopWarningInfo();
		}
		return memberValues;
	}
}