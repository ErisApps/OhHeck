using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.ModData.Chroma;
using OhHeck.Core.Models.ModData.Tracks;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Models.Beatmap;

public class BeatmapCustomData : IAnalyzable
{
	[JsonPropertyName("_environment")]
	public List<EnvironmentEnhancement> EnvironmentEnhancements { get; } = new();

	[JsonPropertyName("_customEvents")]
	public List<BeatmapCustomEvent> CustomEvents { get; } = new();

	[JsonPropertyName("_pointDefinitions")]
	public List<PointDefinitionData> PointDefinitions { get; } = new();

	[JsonExtensionData]
	public Dictionary<string, JsonElement> DontCareAboutThisData { get; set; } = new();

	public BeatmapCustomData(List<EnvironmentEnhancement> environmentEnhancements, List<BeatmapCustomEvent> customEvents, List<PointDefinitionData> pointDefinitions)
	{
		EnvironmentEnhancements = environmentEnhancements;
		CustomEvents = customEvents;
		PointDefinitions = pointDefinitions;
	}

	public string GetFriendlyName() => nameof(BeatmapCustomData);
}

public class BeatmapCustomEvent : IAnalyzable
{
	[JsonPropertyName("_time")]
	public float Time { get; }

	[JsonPropertyName("_type")]
	public string Type { get; }

	[JsonPropertyName("_data")]
	public Dictionary<string, object> Data { get; }

	public BeatmapCustomEvent(float time, string type, Dictionary<string, object> data)
	{
		Time = time;
		Type = type;
		Data = data;
	}

	public string GetFriendlyName() => nameof(BeatmapCustomEvent);
}


public class ObjectCustomData
{
	// Unsure if the array works like PointData or always float[]
	// Can also be a string which points to a Point Def defined in beatmap customData
	[JsonPropertyName("_animation")]
	public Dictionary<string, object>? animation;

	[JsonPropertyName("_position")]
	public List<float> Position { get; } = new();

	[JsonPropertyName("_rotation")]
	public Vector3? Rotation { get; set; }

	[JsonPropertyName("_localRotation")]
	public Vector3? LocalRotation { get; set; }

	[JsonPropertyName("_noteJumpMovementSpeed")]
	public float? NoteJumpMovementSpeed { get; set; }

	[JsonPropertyName("_noteJumpStartBeatOffset")]
	public float? NoteJumpStartBeatOffset { get; set; }

	// mappers worst enemy
	[JsonPropertyName("_fake")]
	public FakeTruthy? Fake { get; set; }

	[JsonPropertyName("_interactable")]
	public FakeTruthy? Cuttable { get; set; }


	[JsonExtensionData]
	public Dictionary<string, JsonElement> DontCareAboutThisData { get; } = new();
}

public class ObstacleCustomData : ObjectCustomData, IAnalyzable
{
	public string GetFriendlyName() => nameof(ObstacleCustomData);
}

public class NoteCustomData : ObjectCustomData, IAnalyzable
{
	public string GetFriendlyName() => nameof(NoteCustomData);
}

public class EventCustomData : IAnalyzable
{
	[JsonExtensionData]
	public Dictionary<string, JsonElement> DontCareAboutThisData { get; } = new();

	public string GetFriendlyName() => nameof(EventCustomData);
}