using System;

namespace OhHeck.Core.Analyzer;

[System.AttributeUsage(AttributeTargets.Class)]
public class BeatmapWarningAttribute : Attribute
{
	public string Name { get; }

	public BeatmapWarningAttribute(string name) => Name = name;
}