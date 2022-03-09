using System;
using JetBrains.Annotations;

namespace OhHeck.Core.Analyzer.Attributes;

[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public class BeatmapWarningAttribute : Attribute
{
	public string Name { get; }

	public BeatmapWarningAttribute(string name) => Name = name;
}