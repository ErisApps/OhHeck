﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIoc;
using OhHeck.Core.Models.Beatmap;
using Serilog;

namespace OhHeck.Core.Analyzer;

public class WarningManager
{
	private List<IBeatmapWarning> _beatmapWarnings = new();
	private static readonly Type IBeatmapWarningType = typeof(IBeatmapWarning);
	private static readonly Type IAnalyzableType = typeof(IAnalyzable);

	private readonly IContainer _container;
	private readonly ILogger _logger;

	public WarningManager(IContainer container, ILogger logger)
	{
		_container = container;
		_logger = logger;
	}

	public void Init()
	{
		foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
		{
			var warningAttribute = type.GetCustomAttribute<BeatmapWarningAttribute>();

			if (warningAttribute is null)
			{
				continue;
			}

			if (!IBeatmapWarningType.IsAssignableFrom(type))
			{
				throw new InvalidOperationException($"{type} must inherit {nameof(IBeatmapWarning)}");
			}

			// TODO: Use logger framework?
			_logger.Debug($"Class {type} has warning attribute");

			var instance = (IBeatmapWarning) _container.New(type);
			_beatmapWarnings.Add(instance);
		}
	}


	// Nonnull
	public void Analyze(IAnalyzable analyzable) => Analyze(analyzable, null, analyzable.GetType());
	public void Analyze(IAnalyzable analyzable, IAnalyzable parent) => Analyze(analyzable, parent, analyzable.GetType());

	// Nullable
	public void Analyze(IAnalyzable? analyzable, IAnalyzable? parent, Type type)
	{
		var friendlyName = analyzable?.GetFriendlyName() ?? type.Name;
		var fieldInfos = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

		Dictionary<FieldInfo, object?> fieldValues = new();

		// Analyze each field
		foreach (var fieldInfo in fieldInfos)
		{
			object? fieldValue = null;
			if (analyzable is not null)
			{
				fieldValues[fieldInfo] = fieldValue = fieldInfo.GetValue(analyzable);
			}

			var warnings = _beatmapWarnings
				.Select(warning => warning.Validate(fieldInfo, fieldValue))
				.Where(s => s is not null).ToList();

			if (warnings.Count == 0)
			{
				continue;
			}

			foreach (var warning in warnings)
			{
				_logger.Warning($"Warning: {friendlyName}:{fieldInfo.Name} {warning}");
				if (parent is not null)
				{
					_logger.Warning($"Parent {parent.GetFriendlyName()} {parent.ExtraData()}");
				}

				_logger.Warning("");
			}

			break;
		}

		if (analyzable is null)
		{
			return;
		}


		// Now to recursively analyze
		foreach (var fieldInfo in fieldInfos)
		{
			if (!IAnalyzableType.IsAssignableFrom(fieldInfo.FieldType))
			{
				continue;
			}

			var fieldValue = (IAnalyzable?) fieldValues[fieldInfo];
			Analyze(fieldValue, analyzable, fieldInfo.FieldType);
		}
	}
}