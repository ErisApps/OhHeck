using System.Collections.Generic;
using System.Text.Json.Serialization;
using OhHeck.Core.Beatmap.ModCustomData;
using OhHeck.Core.structs;

namespace OhHeck.Core.Beatmap;

public class BeatmapCustomEvent
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
}

public class BeatmapCustomData
{
	[JsonPropertyName("_environment")]
	public List<EnvironmentEnhancement>? EnvironmentEnhancements { get; }

	[JsonPropertyName("_customEvents")]
	public List<BeatmapCustomEvent>? CustomEvents { get; }

	[JsonPropertyName("_pointDefinitions")]
	public List<PointDefinitionData>? PointDefinitions { get; }

	[JsonExtensionData]
	public Dictionary<string, object> DontCareAboutThisData { get; set; } = new();

	public BeatmapCustomData(List<EnvironmentEnhancement>? environmentEnhancements, List<BeatmapCustomEvent>? customEvents, List<PointDefinitionData>? pointDefinitions)
	{
		EnvironmentEnhancements = environmentEnhancements;
		CustomEvents = customEvents;
		PointDefinitions = pointDefinitions;
	}
}

public class ObjectCustomData
{
	// Unsure if the array works like PointData or always float[]
	[JsonPropertyName("_animation")]
	public Dictionary<string, object[]>? animation;

	[JsonPropertyName("_position")]
	public Vector2? Position;

	[JsonPropertyName("_rotation")]
	public Vector3? Rotation;

	[JsonPropertyName("_localRotation")]
	public Vector3? LocalRotation;

	[JsonPropertyName("_noteJumpMovementSpeed")]
	public float? NoteJumpMovementSpeed;

	[JsonPropertyName("_noteJumpStartBeatOffset")]
	public float? NoteJumpStartBeatOffset;

	// mappers worst enemy
	[JsonPropertyName("_fake")]
	public FakeTruthy? Fake;

	[JsonPropertyName("_interactable")]
	public FakeTruthy? Cuttable;


	[JsonExtensionData]
	public Dictionary<string, object> DontCareAboutThisData { get; set; } = new();
}

public class ObstacleCustomData : ObjectCustomData
{

}

public class NoteCustomData : ObjectCustomData
{

}

public class EventCustomData
{
	[JsonExtensionData]
	public Dictionary<string, object> DontCareAboutThisData { get; set; } = new();
}