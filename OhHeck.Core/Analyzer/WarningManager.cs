using System;
using System.Collections.Generic;
using System.Reflection;

namespace OhHeck.Core.Analyzer;

public class WarningManager
{
	private List<IBeatmapWarning> _beatmapWarnings = new();
	private static readonly Type IBeatmapWarningType = typeof(IBeatmapWarning);

	public void Init()
	{
		foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
		{
			var warningAttribute = type.GetCustomAttribute<BeatmapWarningAttribute>();

			if (warningAttribute is null)
			{
				continue;
			}

			// TODO: Use logger framework?
			Console.WriteLine($"Class {type} has warning attribute");

			// TODO: Use dependency injection to create an instance
			var instance = (IBeatmapWarning?) Activator.CreateInstance(type);
			_beatmapWarnings.Add(instance!);
		}
	}
}