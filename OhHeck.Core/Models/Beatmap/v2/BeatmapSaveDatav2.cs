using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap.Enums;
using OhHeck.Core.Models.Beatmap.v2.Enums;

// we don't care anymore!
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace OhHeck.Core.Models.Beatmap.v2;

public class BeatmapSaveData : IAnalyzable
{
	[JsonPropertyName("_version")]
	public string? Version { get; }

	[JsonPropertyName("_events")]
	public List<EventData> Events { get; }

	[JsonPropertyName("_obstacles")]
	public List<ObstacleData> Obstacles { get; }

	[JsonPropertyName("_notes")]
	public List<NoteData> Notes { get; }

	[JsonPropertyName("_sliders")]
	public List<SliderData>? Sliders { get; }

	[JsonPropertyName("_waypoints")]
	public List<WaypointData> Waypoints { get; }

	[JsonPropertyName("_specialEventsKeywordFilters")]
	public SpecialEventKeywordsFiltersData? SpecialEventsKeywordFilters { get; }

	[JsonPropertyName("_customData")]
	public BeatmapCustomData? BeatmapCustomData { get; }

	[JsonExtensionData]
	public Dictionary<string, JsonElement> DontCareAboutThisData { get; set; } = new();

	public BeatmapSaveData(string? version, List<EventData> events, List<ObstacleData> obstacles, List<NoteData> notes, List<SliderData>? sliders, List<WaypointData> waypoints, SpecialEventKeywordsFiltersData? specialEventsKeywordFilters, BeatmapCustomData? beatmapCustomData)
	{
		Version = version;
		Events = events;
		Obstacles = obstacles;
		Notes = notes;
		Sliders = sliders;
		Waypoints = waypoints;
		SpecialEventsKeywordFilters = specialEventsKeywordFilters;
		BeatmapCustomData = beatmapCustomData;
	}

	public string GetFriendlyName() => "Beatmap";
}

public abstract class BeatmapSaveDataItem : IComparable<BeatmapSaveDataItem>
{
	protected BeatmapSaveDataItem(float time) => Time = time;

	[JsonPropertyName("_time")]
	public float Time { get; set; }

	public int CompareTo(BeatmapSaveDataItem? other)
	{
		if (ReferenceEquals(this, other))
		{
			return 0;
		}

		return ReferenceEquals(null, other) ? 1 : Time.CompareTo(other.Time);
	}
}

public class EventData : BeatmapSaveDataItem, IAnalyzable
{
	[JsonPropertyName("_type")]
	public BeatmapEventType Type { get; }

	[JsonPropertyName("_value")]
	public int Value { get; }

	[JsonPropertyName("_floatValue")]
	public float? FloatValue { get; set; }

	[JsonPropertyName("_customData")]
	public EventCustomData? EventCustomData { get; set; }

	[JsonExtensionData]
	public Dictionary<string, JsonElement> DontCareAboutThisData { get; set; } = new();

	public EventData(float time, BeatmapEventType type, int value, float? floatValue, EventCustomData? eventCustomData) : base(time)
	{
		Type = type;
		Value = value;
		FloatValue = floatValue;
		EventCustomData = eventCustomData;
	}

	public string GetFriendlyName() => "EventData";
	public string ExtraData() => $"Time: {Time} Type {Type}";
}



public class NoteData : BeatmapSaveDataItem, IAnalyzable
{
	[JsonPropertyName("_lineIndex")]
	public int LineIndex { get; set; }

	[JsonPropertyName("_lineLayer")]
	public int LineLayer { get; set; }

	[JsonPropertyName("_type")]
	public NoteType Type { get; }

	[JsonPropertyName("_cutDirection")]
	public NoteCutDirection CutDirection { get; }

	[JsonPropertyName("_customData")]
	public NoteCustomData? NoteCustomData { get; set; }

	public NoteData(float time, int lineIndex, int lineLayer, NoteType type, NoteCutDirection cutDirection, NoteCustomData? noteCustomData) : base(time)
	{
		LineIndex = lineIndex;
		LineLayer = lineLayer;
		Type = type;
		CutDirection = cutDirection;
		NoteCustomData = noteCustomData;
	}

	public string GetFriendlyName() => "NoteData";
	public string ExtraData() => $"Time: {Time} Type {Type}";
}

public class SliderData : BeatmapSaveDataItem
{
	[JsonPropertyName("_colorType")]
	public ColorType ColorType { get; set; }


	[JsonPropertyName("_headTime")]
	public float HeadTime { get; set; }


	[JsonPropertyName("_headLineIndex")]
	public int HeadLineIndex { get; set; }


	[JsonPropertyName("_headLineLayer")]
	public NoteLineLayer HeadLineLayer { get; set; }


	[JsonPropertyName("_headControlPointLengthMultiplier")]
	public float HeadControlPointLengthMultiplier { get; set; }

	[JsonPropertyName("_headCutDirection")]
	public NoteCutDirection HeadCutDirection { get; set; }


	[JsonPropertyName("_tailTime")]
	public float TailTime { get; set; }


	[JsonPropertyName("_tailLineIndex")]
	public int TailLineIndex { get; set; }


	[JsonPropertyName("_tailLineLayer")]
	public NoteLineLayer TailLineLayer { get; set; }


	[JsonPropertyName("_tailControlPointLengthMultiplier")]
	public float TailControlPointLengthMultiplier { get; set; }


	[JsonPropertyName("_tailCutDirection")]
	public NoteCutDirection TailCutDirection { get; set; }

	[JsonPropertyName("_sliderMidAnchorMode")]
	public SliderMidAnchorMode SliderMidAnchorMode { get; set; }

	public SliderData(float time, ColorType colorType, float headTime, int headLineIndex, NoteLineLayer headLineLayer, float headControlPointLengthMultiplier, NoteCutDirection headCutDirection, float tailTime, int tailLineIndex, NoteLineLayer tailLineLayer, float tailControlPointLengthMultiplier, NoteCutDirection tailCutDirection, SliderMidAnchorMode sliderMidAnchorMode) : base(time)
	{
		ColorType = colorType;
		HeadTime = headTime;
		HeadLineIndex = headLineIndex;
		HeadLineLayer = headLineLayer;
		HeadControlPointLengthMultiplier = headControlPointLengthMultiplier;
		HeadCutDirection = headCutDirection;
		TailTime = tailTime;
		TailLineIndex = tailLineIndex;
		TailLineLayer = tailLineLayer;
		TailControlPointLengthMultiplier = tailControlPointLengthMultiplier;
		TailCutDirection = tailCutDirection;
		SliderMidAnchorMode = sliderMidAnchorMode;
	}
}

public class ObstacleData : BeatmapSaveDataItem, IAnalyzable
{
	[JsonPropertyName("_lineIndex")]
	public int LineIndex { get; set; }

	[JsonPropertyName("_type")]
	public ObstacleType Type { get; }

	[JsonPropertyName("_duration")]
	public float Duration { get; }

	[JsonPropertyName("_width")]
	public int Width { get; }

	[JsonPropertyName("_customData")]
	public ObstacleCustomData? ObstacleCustomData { get; set; }

	public ObstacleData(float time, int lineIndex, ObstacleType type, float duration, int width, ObstacleCustomData? obstacleCustomData) : base(time)
	{
		LineIndex = lineIndex;
		Type = type;
		Duration = duration;
		Width = width;
		ObstacleCustomData = obstacleCustomData;
	}

	public string GetFriendlyName() => "ObstacleData";
	public string ExtraData() => $"Time: {Time} Type {Type}";
}

public class WaypointData : BeatmapSaveDataItem
{
	[JsonPropertyName("_lineIndex")]
	public int LineIndex { get; set; }

	[JsonPropertyName("_lineLayer")]
	public NoteLineLayer LineLayer;

	[JsonPropertyName("offsetDirection")]
	public OffsetDirection OffsetDirection;

	[JsonExtensionData]
	public Dictionary<string, JsonElement> DontCareAboutThisData { get; set; } = new();

	public WaypointData(float time, NoteLineLayer lineLayer, OffsetDirection offsetDirection, int lineIndex) : base(time)
	{
		LineLayer = lineLayer;
		OffsetDirection = offsetDirection;
		LineIndex = lineIndex;
	}
}

public class SpecialEventKeywordsFiltersData : IAnalyzable
{
	[JsonPropertyName("_keywords")]
	public List<SpecialEventsForKeyword>? Keywords { get; set; }

	[JsonExtensionData]
	public Dictionary<string, JsonElement> DontCareAboutThisData { get; set; } = new();

	public SpecialEventKeywordsFiltersData(List<SpecialEventsForKeyword> keywords) => Keywords = keywords;
	public string GetFriendlyName() => nameof(SpecialEventsForKeyword);
}

public class SpecialEventsForKeyword : IAnalyzable
{
	[JsonPropertyName("_keyword")]
	public string Keyword { get; set; }

	[JsonPropertyName("_specialEvents")]
	public List<BeatmapEventType> SpecialEvents;

	[JsonExtensionData]
	public Dictionary<string, JsonElement> DontCareAboutThisData { get; set; } = new();

	public SpecialEventsForKeyword(List<BeatmapEventType> specialEvents, string keyword)
	{
		SpecialEvents = specialEvents;
		Keyword = keyword;
	}

	public string GetFriendlyName() => nameof(SpecialEventsForKeyword);
}