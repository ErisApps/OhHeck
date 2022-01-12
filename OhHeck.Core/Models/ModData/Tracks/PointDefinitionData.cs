using System.Collections.Generic;
using System.Text.Json.Serialization;
using OhHeck.Core.Helpers.Converters;

namespace OhHeck.Core.Models.ModData.Tracks;

public class PointDefinitionData
{
	public PointDefinitionData(string name, List<PointData> points)
	{
		Name = name;
		Points = points;
	}

	[JsonPropertyName("_name")]
	public string Name { get; }

	// We might need a custom parser for this to make this actually not stupid
	// We also need to parse [...] -> PointData
	// Most of the time it will be [[...], [...]] -> List<PointData> though
	[JsonPropertyName("_points")]
	[JsonConverter(typeof(PointDataListConverter))]
	public List<PointData> Points { get; }
}