using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIoc;
using OhHeck.Core.Models.Beatmap;
using Serilog;

namespace OhHeck.Core.Analyzer;

public class WarningManager
{
	private readonly Dictionary<string, IBeatmapWarning> _beatmapWarnings = new();
	private static readonly Type IBeatmapWarningType = typeof(IBeatmapWarning);
	private static readonly Type IAnalyzableType = typeof(IAnalyzable);

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

			if (_beatmapWarnings.TryGetValue(warningAttribute.Name, out var existingWarning))
			{
				throw new InvalidOperationException($"Beatmap warning {warningAttribute} already exists tied to {existingWarning.GetType()}");
			}

			if (!IBeatmapWarningType.IsAssignableFrom(type))
			{
				throw new InvalidOperationException($"{type} must inherit {nameof(IBeatmapWarning)}");
			}

			// TODO: Use logger framework?
			_logger.Debug($"Class {type} has warning attribute");

			var instance = (IBeatmapWarning) _container.New(type);
			_beatmapWarnings[warningAttribute.Name] = instance;
		}
	}


	// Nonnull
	public void Analyze(IAnalyzable analyzable) => Analyze(analyzable, null, analyzable.GetType());
	public void Analyze(IAnalyzable analyzable, IAnalyzable parent) => Analyze(analyzable, parent, analyzable.GetType());

	// Nullable
	public void Analyze(IAnalyzable? analyzable, IAnalyzable? parent, Type type)
	{
		// Early return
		if (_beatmapWarnings.Count == 0)
		{
			return;
		}

		var friendlyName = analyzable?.GetFriendlyName() ?? type.Name;

		var memberInfos = AnalyzeMemberInfo(analyzable, parent, type, friendlyName);

		if (analyzable is null)
		{
			return;
		}

		// Now to recursively analyze
		foreach (var (_, (memberValue, memberType)) in memberInfos)
		{
			if (!IAnalyzableType.IsAssignableFrom(memberType))
			{
				continue;
			}

			var fieldValue = (IAnalyzable?) memberValue;
			Analyze(fieldValue, analyzable, memberType);
		}
	}

	private Dictionary<MemberInfo, (object?, Type)> AnalyzeMemberInfo(IAnalyzable? analyzable, IAnalyzable? parent, IReflect type, string friendlyName)
	{
		var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
		var propInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

		var memberInfos = fieldInfos.ToList<MemberInfo>();
		memberInfos.AddRange(propInfos.ToList<MemberInfo>());

		Dictionary<MemberInfo, (object?, Type)> memberValues = new();


		// Analyze each field
		foreach (var memberInfo in memberInfos)
		{
			var analyzeMemberAttribute = memberInfo.GetCustomAttribute<AnalyzeMemberAttribute>();
			var friendlyMemberName = analyzeMemberAttribute?.Name ?? memberInfo.Name;

			object? memberValue = null;
			var memberType = memberInfo switch
			{
				PropertyInfo propertyInfo => propertyInfo.PropertyType,
				FieldInfo fieldInfo => fieldInfo.FieldType,
				_ => null
			};

			if (analyzable is not null)
			{
				memberValue = memberInfo switch
				{
					PropertyInfo propertyInfo => propertyInfo.GetValue(analyzable),
					FieldInfo fieldInfo => fieldInfo.GetValue(analyzable),
					_ => memberValue
				};
			}

			memberValues[memberInfo] = (memberValue, memberType!);

			var warnings = _beatmapWarnings
				.Select(warning => warning.Value.Validate(memberType!, memberValue))
				.Where(s => s is not null).ToList();

			if (warnings.Count == 0)
			{
				continue;
			}

			foreach (var warning in warnings)
			{
				_logger.Warning($"Warning: {friendlyName}:{{{friendlyMemberName}}} {warning}");
				if (parent is not null)
				{
					_logger.Warning($"Parent {parent.GetFriendlyName()} {parent.ExtraData()}");
				}

				_logger.Warning("");
			}

			break;
		}
		return memberValues;
	}
}