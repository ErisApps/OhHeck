using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.Beatmap.Enums;
using OhHeck.Core.Models.Beatmap.v3.Enums;

// we don't care anymore!
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBeProtected.Global

namespace OhHeck.Core.Models.beatmap.v3;

// TODO: Binary bool converter

public class BeatmapSaveData : IAnalyzable
{
	[JsonPropertyName("version")] public string? Version { get; }

	[JsonPropertyName("bpmEvents")] public List<BpmChangeEventData> BpmEvents { get; }

	[JsonPropertyName("rotationEvents")] public List<RotationEventData> RotationEvents { get; }

	[JsonPropertyName("basicBeatmapEvents")]
	public List<BasicEventData> BasicBeatmapEvents { get; }

	[JsonPropertyName("colorBoostBeatmapEvents")]
	public List<ColorBoostEventData> ColorBoostBeatmapEvents { get; }

	[JsonPropertyName("lightColorEventBoxGroups")]
	public List<LightColorEventBoxGroup> LightColorEventBoxGroups { get; }

	[JsonPropertyName("lightRotationEventBoxGroups")]
	public List<LightRotationEventBoxGroup> LightRotationEventBoxGroups { get; }

	[JsonPropertyName("obstacles")]
	public List<ObstacleData> Obstacles { get; }

	[JsonPropertyName("colorNotes")]
	public List<ColorNoteData> Notes { get; }

	[JsonPropertyName("bombNotes")]
	public List<BombNoteData> BombNotes { get; }

	[JsonPropertyName("sliders")]
	public List<SliderData> Sliders { get; }
	[JsonPropertyName("burstSliders")]
	public List<BurstSliderData> BurstSliders { get; }

	[JsonPropertyName("waypoints")]
	public List<WaypointData> Waypoints { get; }

	[JsonPropertyName("basicEventTypesWithKeywords")]
	public BasicEventTypesWithKeywords BasicEventTypesWithKeywords { get; }

	[JsonPropertyName("useNormalEventsAsCompatibleEvents")]
	public bool UseNormalEventsAsCompatibleEvents { get; }

	[JsonPropertyName(JsonKeys.CUSTOM_DATA_KEY_V3)]
	public BeatmapCustomData? BeatmapCustomData { get; }

	public BeatmapSaveData(string? version, List<BpmChangeEventData> bpmEvents, List<RotationEventData> rotationEvents, List<BasicEventData> basicBeatmapEvents, List<ColorBoostEventData> colorBoostBeatmapEvents, List<LightColorEventBoxGroup> lightColorEventBoxGroups, List<LightRotationEventBoxGroup> lightRotationEventBoxGroups, List<ObstacleData> obstacles, List<ColorNoteData> notes, List<BombNoteData> bombNotes, List<SliderData> sliders, List<BurstSliderData> burstSliders, List<WaypointData> waypoints, BasicEventTypesWithKeywords basicEventTypesWithKeywords, bool useNormalEventsAsCompatibleEvents, BeatmapCustomData? beatmapCustomData)
	{
		Version = version;
		BpmEvents = bpmEvents;
		RotationEvents = rotationEvents;
		BasicBeatmapEvents = basicBeatmapEvents;
		ColorBoostBeatmapEvents = colorBoostBeatmapEvents;
		LightColorEventBoxGroups = lightColorEventBoxGroups;
		LightRotationEventBoxGroups = lightRotationEventBoxGroups;
		Obstacles = obstacles;
		Notes = notes;
		BombNotes = bombNotes;
		Sliders = sliders;
		BurstSliders = burstSliders;
		Waypoints = waypoints;
		BasicEventTypesWithKeywords = basicEventTypesWithKeywords;
		UseNormalEventsAsCompatibleEvents = useNormalEventsAsCompatibleEvents;
		BeatmapCustomData = beatmapCustomData;
	}

	[JsonExtensionData] public Dictionary<string, JsonElement> DontCareAboutThisData { get; set; } = new();

	public string GetFriendlyName() => "Beatmap v3";
}

public abstract class BeatmapSaveDataItem : IAnalyzable, IComparable<BeatmapSaveDataItem> {
	protected BeatmapSaveDataItem(float beat) => Beat = beat;

	[JsonPropertyName("b")]
	public float Beat { get; set; }

	public abstract string GetFriendlyName();

	public virtual string ExtraData() => $"Beat: {Beat}";

	public int CompareTo(BeatmapSaveDataItem? other)
	{
		if (ReferenceEquals(this, other))
		{
			return 0;
		}

		return ReferenceEquals(null, other) ? 1 : Beat.CompareTo(other.Beat);
	}
}

#region Events
public class BasicEventData : BeatmapSaveDataItem
{
	[JsonPropertyName("et")]
	public BeatmapEventType Type { get; set; }

	[JsonPropertyName("i")]
	public int Value { get; set; }

	[JsonPropertyName("f")]
	public float? FloatValue { get; set; }

	[JsonPropertyName("_customData")]
	public EventCustomData? EventCustomData { get; set; }

	public BasicEventData(float beat, BeatmapEventType type, int value, float? floatValue, EventCustomData? eventCustomData) : base(beat)
	{
		Type = type;
		Value = value;
		FloatValue = floatValue;
		EventCustomData = eventCustomData;
	}

	public override string GetFriendlyName() => "EventData";
	public override string ExtraData() => $"Beat: {Beat} Type {Type}";
}

public class ColorBoostEventData : BeatmapSaveDataItem
{
	[JsonPropertyName("o")]
	public bool Boost { get; set; }

	public ColorBoostEventData(float beat, bool boost) : base(beat) => Boost = boost;
	public override string GetFriendlyName() => $"{nameof(ColorBoostEventData)}";

	public override string ExtraData() => $"Beat: {Beat} Boost: {Boost}";
}

public class BpmChangeEventData : BeatmapSaveDataItem
{
	[JsonPropertyName("m")]
	public float Bpm { get; set; }

	public BpmChangeEventData(float beat, float bpm) : base(beat) => Bpm = bpm;
	public override string GetFriendlyName() => nameof(BpmChangeEventData);
}

public class RotationEventData : BeatmapSaveDataItem
{
	[JsonPropertyName("e")]
	public ExecutionTime ExecutionTime { get; set; }

	[JsonPropertyName("r")]
	public float Rotation { get; set; }

	public RotationEventData(float beat, ExecutionTime executionTime, float rotation) : base(beat)
	{
		ExecutionTime = executionTime;
		Rotation = rotation;
	}

	public override string GetFriendlyName() => nameof(RotationEventData);
}

public class BasicEventTypesWithKeywords : IAnalyzable
{
	[JsonPropertyName("d")]
	public List<BasicEventTypesForKeyword> Data { get; set; }


	public BasicEventTypesWithKeywords(List<BasicEventTypesForKeyword> data) => Data = data;

	public class BasicEventTypesForKeyword : IAnalyzable
	{
		[JsonPropertyName("k")]
		public string Keyword { get; set; }

		[JsonPropertyName("e")]
		public List<BeatmapEventType> EventType { get; set; }

		public BasicEventTypesForKeyword(string keyword, List<BeatmapEventType> eventType)
		{
			Keyword = keyword;
			EventType = eventType;
		}

		public string GetFriendlyName() => nameof(BasicEventTypesForKeyword);
	}

	public string GetFriendlyName() => nameof(BasicEventTypesWithKeywords);
}

public abstract class EventBox : IAnalyzable
{
	[JsonPropertyName("f")]
	public IndexFilter IndexFilter { get; set; }

	[JsonPropertyName("w")]
	public float BeatDistributionParam { get; set; }

	[JsonPropertyName("d")]
	public DistributionParamType BeatDistributionParamType { get; set; }

	protected EventBox(IndexFilter indexFilter, float beatDistributionParam, DistributionParamType beatDistributionParamType)
	{
		IndexFilter = indexFilter;
		BeatDistributionParam = beatDistributionParam;
		BeatDistributionParamType = beatDistributionParamType;
	}

	public enum DistributionParamType
	{
		Wave = 1,
		Step
	}

	public abstract string GetFriendlyName();
}

public class IndexFilter
{
	[JsonPropertyName("f")]
	public IndexFilterType Type { get; set; }

	[JsonPropertyName("p")]
	public int Param0 { get; set; }

	[JsonPropertyName("t")]
	public int Param1 { get; set; }

	[JsonPropertyName("r")]
	public int ReversedInt { get; set; }

	[JsonIgnore]
	public bool Reversed
	{
		get => ReversedInt != 0;
		set => ReversedInt = value ? 1 : 0;
	}

	public enum IndexFilterType
	{
		Division = 1,
		StepAndOffset
	}
}

public class LightColorEventBox : EventBox
{
	[JsonPropertyName("r")]
	public float BrightnessDistributionParam { get; set; }

	[JsonPropertyName("t")]
	public DistributionParamType BrightnessDistributionParamType { get; set; }

	[JsonPropertyName("b")]
	public int BrightnessDistributionShouldAffectFirstBaseEventInt { get; set; }

	[JsonIgnore]
	public bool BrightnessDistributionShouldAffectFirstBaseEvent {
		get => BrightnessDistributionShouldAffectFirstBaseEventInt == 1;
		set => BrightnessDistributionShouldAffectFirstBaseEventInt = value ? 1 : 0;
	}


	[JsonPropertyName("e")]
	public List<LightColorBaseData> LightColorBaseDataList { get; set; }

	public LightColorEventBox(IndexFilter indexFilter, float beatDistributionParam, DistributionParamType beatDistributionParamType, float brightnessDistributionParam, DistributionParamType brightnessDistributionParamType, bool brightnessDistributionShouldAffectFirstBaseEvent, List<LightColorBaseData> lightColorBaseDataList) : base(indexFilter, beatDistributionParam, beatDistributionParamType)
	{
		BrightnessDistributionParam = brightnessDistributionParam;
		BrightnessDistributionParamType = brightnessDistributionParamType;
		BrightnessDistributionShouldAffectFirstBaseEvent = brightnessDistributionShouldAffectFirstBaseEvent;
		LightColorBaseDataList = lightColorBaseDataList;
	}

	public override string GetFriendlyName() => nameof(LightColorEventBox);
}

public class LightColorBaseData
{
	[JsonPropertyName("b")]
	public float Beat { get; set; }

	[JsonPropertyName("i")]
	public TransitionType TransitionType { get; set; }

	[JsonPropertyName("c")]
	public EnvironmentColorType ColorType { get; set; }

	[JsonPropertyName("s")]
	public float Brightness { get; set; }

	[JsonPropertyName("f")]
	public int StrobeBeatFrequency { get; set; }

	public LightColorBaseData(float beat, TransitionType transitionType, EnvironmentColorType colorType, float brightness, int strobeBeatFrequency)
	{
		Beat = beat;
		TransitionType = transitionType;
		ColorType = colorType;
		Brightness = brightness;
		StrobeBeatFrequency = strobeBeatFrequency;
	}
}

public class LightRotationEventBox : EventBox
{
	[JsonPropertyName("s")]
	public float RotationDistributionParam { get; set; }

	[JsonPropertyName("t")]
	public DistributionParamType RotationDistributionParamType { get; set; }

	[JsonPropertyName("a")]
	public Axis Axis { get; set; }

	[JsonPropertyName("r")]
	public int FlipRotationInt { get; set; }

	[JsonIgnore]
	public bool FlipRotation {
		get => FlipRotationInt == 1;
		set => FlipRotationInt = value ? 1 : 0;
	}

	[JsonPropertyName("b")]
	public int RotationDistributionShouldAffectFirstBaseEventInt { get; set; }

	[JsonIgnore]
	public bool RotationDistributionShouldAffectFirstBaseEvent {
		get => RotationDistributionShouldAffectFirstBaseEventInt == 1;
		set => RotationDistributionShouldAffectFirstBaseEventInt = value ? 1 : 0;
	}


	[JsonPropertyName("l")]
	public List<LightRotationBaseData> LightRotationBaseDataList { get; set; }

	public LightRotationEventBox(IndexFilter indexFilter, float beatDistributionParam, DistributionParamType beatDistributionParamType, float rotationDistributionParam, DistributionParamType rotationDistributionParamType, Axis axis, bool flipRotation, bool rotationDistributionShouldAffectFirstBaseEvent, List<LightRotationBaseData> lightRotationBaseDataList) : base(indexFilter, beatDistributionParam, beatDistributionParamType)
	{
		RotationDistributionParam = rotationDistributionParam;
		RotationDistributionParamType = rotationDistributionParamType;
		Axis = axis;
		FlipRotation = flipRotation;
		RotationDistributionShouldAffectFirstBaseEvent = rotationDistributionShouldAffectFirstBaseEvent;
		LightRotationBaseDataList = lightRotationBaseDataList;
	}

	public override string GetFriendlyName() => nameof(LightRotationEventBox);
}

public class LightRotationBaseData
{
	[JsonPropertyName("b")]
	public float Beat { get; set; }

	[JsonPropertyName("e")]
	public EaseType EaseType { get; set; }

	[JsonPropertyName("l")]
	public int LoopsCount { get; set; }

	[JsonPropertyName("r")]
	public float Rotation { get; set; }

	[JsonPropertyName("o")]
	public RotationDirection Direction { get; set; }

	[JsonPropertyName("p")]
	public int UsePreviousEventRotationValueInt { get; set; }

	[JsonIgnore]
	public bool UsePreviousEventRotationValue
	{
		get => UsePreviousEventRotationValueInt == 1;
		set => UsePreviousEventRotationValueInt = value ? 1 : 0;
	}

	public enum RotationDirection
	{
		Automatic,
		Clockwise,
		Counterclockwise
	}

	public LightRotationBaseData(float beat, EaseType easeType, int loopsCount, float rotation, RotationDirection direction)
	{
		Beat = beat;
		EaseType = easeType;
		LoopsCount = loopsCount;
		Rotation = rotation;
		Direction = direction;
	}
}

public abstract class EventBoxGroup : BeatmapSaveDataItem
{
	protected EventBoxGroup(float beat, int groupId) : base(beat) => GroupId = groupId;

	[JsonPropertyName("g")]
	public int GroupId { get; set; }

	[JsonIgnore]
	public abstract IReadOnlyList<EventBox> BaseEventBoxes { get; }

	public override string GetFriendlyName() => nameof(EventBoxGroup);
}

public class EventBoxGroup<T> : EventBoxGroup where T : EventBox
{
	public EventBoxGroup(float beat, int groupId, List<T> eventBoxes) : base(beat, groupId) => EventBoxes = eventBoxes;

	[JsonIgnore]
	public override IReadOnlyList<EventBox> BaseEventBoxes => EventBoxes;

	[JsonPropertyName("e")]
	public List<T> EventBoxes { get; set; }

	public override string GetFriendlyName() => nameof(EventBoxGroup<T>);
}

public class LightColorEventBoxGroup : EventBoxGroup<LightColorEventBox>
{
	public LightColorEventBoxGroup(float beat, int groupId, List<LightColorEventBox> eventBoxes) : base(beat, groupId, eventBoxes)
	{
	}
}

public class LightRotationEventBoxGroup : EventBoxGroup<LightRotationEventBox>
{
	public LightRotationEventBoxGroup(float beat, int groupId, List<LightRotationEventBox> eventBoxes) : base(beat, groupId, eventBoxes)
	{
	}
}

#endregion

public class ColorNoteData : BeatmapSaveDataItem
{
	[JsonPropertyName("x")]
	public int LineIndex { get; set; }

	[JsonPropertyName("y")]
	public int LineLayer { get; set; }

	[JsonPropertyName("a")]
	public int AngleOffset { get; set; }

	[JsonPropertyName("c")]
	public NoteColorType Color { get; }

	[JsonPropertyName("d")]
	public NoteCutDirection CutDirection { get; }

	[JsonPropertyName("_customData")]
	public NoteCustomData? NoteCustomData { get; set; }

	public ColorNoteData(float beat, int lineIndex, int lineLayer, NoteColorType color, NoteCutDirection cutDirection, int angleOffset, NoteCustomData? noteCustomData) : base(beat)
	{
		LineIndex = lineIndex;
		LineLayer = lineLayer;
		AngleOffset = angleOffset;
		Color = color;
		CutDirection = cutDirection;
		NoteCustomData = noteCustomData;
	}

	public override string GetFriendlyName() => "NoteData";
	public override string ExtraData() => $"Beat: {Beat} Color {Color}";
}

public class BombNoteData : BeatmapSaveDataItem
{
	[JsonPropertyName("x")]
	public int LineIndex { get; set; }

	[JsonPropertyName("y")]
	public int LineLayer { get; set; }

	[JsonPropertyName("_customData")]
	public NoteCustomData? NoteCustomData { get; set; }

	public BombNoteData(float beat, int lineIndex, int lineLayer, NoteCustomData? noteCustomData) : base(beat)
	{
		LineIndex = lineIndex;
		LineLayer = lineLayer;
		NoteCustomData = noteCustomData;
	}

	public override string GetFriendlyName() => nameof(BombNoteData);
	public override string ExtraData() => $"Beat: {Beat}";
}

public class WaypointData : BeatmapSaveDataItem
{
	[JsonPropertyName("x")]
	public int LineIndex { get; set; }

	[JsonPropertyName("y")]
	public int LineLayer { get; set; }

	[JsonPropertyName("d")]
	public OffsetDirection OffsetDirection;

	public WaypointData(float beat, int lineIndex, int lineLayer, OffsetDirection offsetDirection) : base(beat)
	{
		OffsetDirection = offsetDirection;
		LineIndex = lineIndex;
		LineLayer = lineLayer;
	}


	public override string GetFriendlyName() => nameof(WaypointData);
}

public abstract class BaseSliderData : BeatmapSaveDataItem
{
	[JsonPropertyName("c")]
	public NoteColorType ColorType { get; set; }

	[JsonPropertyName("x")]
	public int HeadLine { get; set; }

	[JsonPropertyName("y")]
	public int HeadLayer { get; set; }

	[JsonPropertyName("d")]
	public NoteCutDirection HeadCutDirection { get; set; }

	[JsonPropertyName("tb")]
	public float TailBeat { get; set; }

	[JsonPropertyName("tx")]
	public int TailLine { get; set; }

	[JsonPropertyName("ty")]
	public int TailLayer { get; set; }

	protected BaseSliderData(float beat, NoteColorType colorType, int headLine, int headLayer, NoteCutDirection headCutDirection, float tailBeat, int tailLine, int tailLayer) : base(beat)
	{
		ColorType = colorType;
		HeadLine = headLine;
		HeadLayer = headLayer;
		HeadCutDirection = headCutDirection;
		TailBeat = tailBeat;
		TailLine = tailLine;
		TailLayer = tailLayer;
	}
}

public class SliderData : BaseSliderData
{
	[JsonPropertyName("mu")]
	public float HeadControlPointLengthMultiplier { get; set; }

	[JsonPropertyName("tmu")]
	public float TailControlPointLengthMultiplier { get; set; }

	[JsonPropertyName("tc")]
	public NoteCutDirection TailCutDirection { get; set; }

	[JsonPropertyName("m")]
	public SliderMidAnchorMode SliderMidAnchorMode { get; set; }


	public SliderData(float beat, NoteColorType colorType, int headLine, int headLayer, float headControlPointLengthMultiplier, NoteCutDirection headCutDirection, float tailBeat, int tailLine, int tailLayer, float tailControlPointLengthMultiplier, NoteCutDirection tailCutDirection, SliderMidAnchorMode sliderMidAnchorMode) : base(beat, colorType, headLine, headLayer, headCutDirection, tailBeat, tailLine, tailLayer)
	{
		HeadControlPointLengthMultiplier = headControlPointLengthMultiplier;
		TailControlPointLengthMultiplier = tailControlPointLengthMultiplier;
		TailCutDirection = tailCutDirection;
		SliderMidAnchorMode = sliderMidAnchorMode;
	}

	public override string GetFriendlyName() => nameof(SliderData);
}

public class BurstSliderData : BaseSliderData
{
	[JsonPropertyName("sc")]
	public int SliceCount { get; set; }

	[JsonPropertyName("s")]
	public float SquishAmount { get; set; }

	public BurstSliderData(float beat, NoteColorType colorType, int headLine, int headLayer, NoteCutDirection headCutDirection, float tailBeat, int tailLine, int tailLayer, int sliceCount, float squishAmount) : base(beat, colorType, headLine, headLayer, headCutDirection, tailBeat, tailLine, tailLayer)
	{
		SliceCount = sliceCount;
		SquishAmount = squishAmount;
	}

	public override string GetFriendlyName() => nameof(BurstSliderData);
}

public class ObstacleData : BeatmapSaveDataItem
{
	[JsonPropertyName("x")]
	public int LineIndex { get; set; }

	[JsonPropertyName("y")]
	public int LineLayer { get; set; }

	[JsonPropertyName("d")]
	public float Duration { get; }

	[JsonPropertyName("w")]
	public int Width { get; set; }

	[JsonPropertyName("h")]
	public int Height { get; set; }


	[JsonPropertyName("_customData")]
	public ObstacleCustomData? ObstacleCustomData { get; set; }

	public ObstacleData(float beat, int lineIndex, int lineLayer, float duration, int width, int height, ObstacleCustomData? obstacleCustomData) : base(beat)
	{
		LineIndex = lineIndex;
		LineLayer = lineLayer;
		Duration = duration;
		Width = width;
		Height = height;
		ObstacleCustomData = obstacleCustomData;
	}

	public override string GetFriendlyName() => nameof(ObstacleData);
	public override string ExtraData() => $"Time: {Beat} Type {Width}:{Height}";
}
