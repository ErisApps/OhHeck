using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OhHeck.Core.Models.ModData.Chroma;

public class EnvironmentEnhancement
{
	// {
	// 	"_id": "^.*\\[\\d*[13579]\\]BigTrackLaneRing\\(Clone\\)$",
	// 	"_lookupMethod": "Regex",
	// 	"_scale": [0.1, 0.1, 0.1]
	// }

	public EnvironmentEnhancement(LookupMethod lookupMethod, string id)
	{
		LookupMethod = lookupMethod;
		Id = id;
	}

	[JsonPropertyName("_id")]
	public string Id { get; }

	[JsonPropertyName("_lookupMethod")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public LookupMethod LookupMethod { get; }

	[JsonExtensionData]
	public Dictionary<string, object> DontCareAboutThisData { get; set; } = new();
}