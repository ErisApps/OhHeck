using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Helpers.Converters;
using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Models.ModData.Tracks;


[JsonConverter(typeof(PointDefinitionProxyConverter))]
public class PointDefinitionDataProxy : IAnalyzable
{
	public BeatmapCustomData? BeatmapCustomData { get; internal set; }

	public string? Name { get; }

	private readonly Lazy<PointDefinitionData> _pointDefinitionDataLazy;

	public PointDefinitionData PointDefinitionData => _pointDefinitionDataLazy.Value;

	public PointDefinitionDataProxy(string name)
	{
		Name = name;
		_pointDefinitionDataLazy = new Lazy<PointDefinitionData>(() =>
		{
			if (BeatmapCustomData is null)
			{
				throw new InvalidOperationException($"{nameof(BeatmapCustomData)} is null");
			}

			return BeatmapCustomData.PointDefinitions!.FirstOrDefault(e => e.Name == Name) ?? throw new InvalidOperationException($"Unable to find point {name}");
		});
	}

	public PointDefinitionDataProxy(PointDefinitionData pointData) => _pointDefinitionDataLazy = new Lazy<PointDefinitionData>(pointData);

	public static implicit operator PointDefinitionData(PointDefinitionDataProxy self) => self.PointDefinitionData;
	public string GetFriendlyName() => PointDefinitionData.GetFriendlyName();
}

public class PointDefinitionData : IAnalyzable
{
	[JsonConstructor]
	public PointDefinitionData(string? name, List<PointData> points)
	{
		Name = name;
		Points = points;
	}

	public PointDefinitionData(List<PointData> points) => Points = points;

	[JsonPropertyName("_name")]
	public string? Name { get; }

	// We might need a custom parser for this to make this actually not stupid
	// We also need to parse [...] -> PointData
	// Most of the time it will be [[...], [...]] -> List<PointData> though
	[JsonPropertyName("_points")]
	[JsonConverter(typeof(PointDataListConverter))]
	public List<PointData> Points { get; }

	public string GetFriendlyName() => $"PointDefinition {Name}";
}