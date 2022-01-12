using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OhHeck.Core.Beatmap;

public class BeatmapSaveData
{
	[JsonPropertyName("_version")]
	public string? Version { get; }

	[JsonPropertyName("_events")]
	public List<EventData> Events { get; } = new();

	[JsonPropertyName("_obstacles")]
	public List<ObstacleData> Obstacles { get; } = new();

	[JsonPropertyName("_notes")]
	public List<ObstacleData> Notes { get; } = new();

	[JsonPropertyName("_customData")]
	public BeatmapCustomData BeatmapCustomData { get; set; }

	[JsonExtensionData]
	public Dictionary<string, object> DontCareAboutThisData { get; } = new();

	public BeatmapSaveData(BeatmapCustomData beatmapCustomData, string? version, List<EventData> events, List<ObstacleData> obstacles, List<ObstacleData> notes, Dictionary<string, object> dontCareAboutThisData)
	{
		BeatmapCustomData = beatmapCustomData;
		Version = version;
		Events = events;
		Obstacles = obstacles;
		Notes = notes;
		DontCareAboutThisData = dontCareAboutThisData;
	}
}

public class EventData
{
	[JsonPropertyName("_time")]
	public float Time { get; }

	[JsonPropertyName("_type")]
	public BeatmapEventType Type { get; }

	[JsonPropertyName("_value")]
	public int Value { get; }

	[JsonPropertyName("_floatValue")]
	public float FloatValue { get; set; }

	[JsonPropertyName("_customData")]
	public EventCustomData EventCustomData { get; set; }


	public EventData(float time, BeatmapEventType type, int value, float floatValue, EventCustomData eventCustomData)
	{
		Time = time;
		Type = type;
		Value = value;
		FloatValue = floatValue;
		EventCustomData = eventCustomData;
	}
}

public class NoteData
{
	[JsonPropertyName("_time")]
	public float Time { get; }

	[JsonPropertyName("_lineIndex")]
	public int LineIndex { get; set; }

	[JsonPropertyName("_lineLayer")]
	public int LineLayer { get; set; }

	[JsonPropertyName("_type")]
	public NoteType Type { get; }

	[JsonPropertyName("cutDirection")]
	public NoteCutDirection CutDirection { get; }

	[JsonPropertyName("_customData")]
	public NoteCustomData NoteCustomData { get; set; }

	public NoteData(float time, int lineIndex, int lineLayer, NoteType type, NoteCutDirection cutDirection, NoteCustomData noteCustomData)
	{
		Time = time;
		LineIndex = lineIndex;
		LineLayer = lineLayer;
		Type = type;
		CutDirection = cutDirection;
		NoteCustomData = noteCustomData;
	}
}

public class ObstacleData
{
	[JsonPropertyName("_time")]
	public float Time { get; }

	[JsonPropertyName("_lineIndex")]
	public int LineIndex { get; set; }

	[JsonPropertyName("_type")]
	public ObstacleType Type { get; }

	[JsonPropertyName("_duration")]
	public float Duration { get; }

	[JsonPropertyName("_width")]
	public int Width { get; }

	[JsonPropertyName("_customData")]
	public ObstacleCustomData ObstacleCustomData { get; set; }

	public ObstacleData(float time, int lineIndex, ObstacleType type, float duration, int width, ObstacleCustomData obstacleCustomData)
	{
		Time = time;
		LineIndex = lineIndex;
		Type = type;
		Duration = duration;
		Width = width;
		ObstacleCustomData = obstacleCustomData;
	}
}
