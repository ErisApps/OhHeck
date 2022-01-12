using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OhHeck.Core.Beatmap;
using OhHeck.Core.structs;

namespace OhHeck.Core;

public class BeatmapCustomEvent
{
	[JsonPropertyName("_time")]
	public float Time;

	[JsonPropertyName("_type")]
	public string Type;

	[JsonPropertyName("_data")]
	public Dictionary<string, object> Data = new();

	public BeatmapCustomEvent(float time, string type, Dictionary<string, object> data)
	{
		Time = time;
		Type = type;
		Data = data;
	}
}

public class PointData
{
	public float[] Data { get; }

	public float Time { get; }

	// if null, it SHOULD mean the first point
	public Functions? Easing { get; }

	public bool Smooth { get; } = false;

	public PointData(List<object> data)
	{
		List<float> preData = new();

		foreach (var o in data)
		{
			switch (o)
			{
				case int:
				case float:
					preData.Add((float) o);
					break;
				case string str:
				{
					if (str == "splineCatmullRom")
					{
						Smooth = true;
					}
					else
					{
						Easing = Enum.Parse(typeof(Functions), str) as Functions?;
					}
					break;
				}
			}
		}

		Time = preData.Last();
		Data = preData.GetRange(0, preData.Count - 1).ToArray();
	}
}

public class PointDefinitionData
{
	public PointDefinitionData(string name, List<object> points)
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
	public List<object> Points { get; } = new();
}

public class BeatmapCustomData
{
	public BeatmapCustomData(List<BeatmapCustomEvent> customEvents, List<PointDefinitionData> pointDefinitions, Dictionary<string, object> dontCareAboutThisData)
	{
		CustomEvents = customEvents;
		PointDefinitions = pointDefinitions;
		DontCareAboutThisData = dontCareAboutThisData;
	}

	[JsonPropertyName("_customEvents")]
	public List<BeatmapCustomEvent> CustomEvents { get; } = new();

	[JsonPropertyName("_pointDefinitions")]
	public List<PointDefinitionData> PointDefinitions { get; } = new();

	[JsonExtensionData]
	public Dictionary<string, object> DontCareAboutThisData { get; } = new();
}

public class ObjectCustomData
{
	// Unsure if the array works like PointData or always float[]
	[JsonPropertyName("_animation")]
	public Dictionary<string, object[]> animation = new();

	[JsonPropertyName("_position")]
	public Vector2 Position;

	[JsonPropertyName("_rotation")]
	public Vector3 Rotation;

	[JsonPropertyName("_localRotation")]
	public Vector3 LocalRotation;

	[JsonPropertyName("_noteJumpMovementSpeed")]
	public float NoteJumpMovementSpeed;

	[JsonPropertyName("_noteJumpStartBeatOffset")]
	public float NoteJumpStartBeatOffset;

	// mappers worst enemy
	[JsonPropertyName("_fake")]
	public bool Fake;

	[JsonPropertyName("_interactable")]
	public bool Cuttable;


	[JsonExtensionData]
	public Dictionary<string, object> DontCareAboutThisData { get; } = new();
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
	public Dictionary<string, object> DontCareAboutThisData { get; } = new();
}