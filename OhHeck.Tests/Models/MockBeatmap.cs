using System.Collections.Generic;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.Beatmap.Enums;
using OhHeck.Core.Models.Beatmap.v2;
using OhHeck.Core.Models.Beatmap.v2.Enums;
using OhHeck.Core.Models.ModData.Chroma;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Tests.Models;

public class MockBeatmap : BeatmapSaveData
{
	private static List<BeatmapCustomEvent> CustomEvents => new()
	{
		new BeatmapCustomEvent(0.0f, EventTypes.ANIMATE_TRACK, new Dictionary<string, object>()),
		new BeatmapCustomEvent(0.0f, EventTypes.ASSIGN_TRACK_PARENT, new Dictionary<string, object>()),
		new BeatmapCustomEvent(0.0f, EventTypes.ASSIGN_PLAYER_TO_TRACK, new Dictionary<string, object>()),
	};

	private static List<EnvironmentEnhancement> EnvironmentEnhancements => new()
	{
		new EnvironmentEnhancement(LookupMethod.Exact, "Thing"),
		new EnvironmentEnhancement(LookupMethod.Regex, "Thing$"),
		new EnvironmentEnhancement(LookupMethod.EndsWith, "Something")
	};

	private static List<PointDefinitionData> PointDefinitionDatas = new() { new PointDefinitionData("p1", new List<PointData>()) };

	private static BeatmapCustomData BeatmapCustomData => new BeatmapCustomData(EnvironmentEnhancements, CustomEvents, PointDefinitionDatas);


	private static List<EventData> Events => new() { new EventData(0.0f, BeatmapEventType.Event0, 0, 0.0f, new EventCustomData()) };
	private static List<ObstacleData> Obstacles => new() { new ObstacleData(0.0f, 0, ObstacleType.FullHeight, 0.0f, 1, new ObstacleCustomData()) };
	private static List<NoteData> Notes => new() { new NoteData(0.0f, 1, 2, NoteType.NoteA, NoteCutDirection.Any, new NoteCustomData()) };


	public MockBeatmap() : base("2.5.0",
		Events,
		Obstacles,
		Notes,
		new List<SliderData>(),
		new List<WaypointData>(),
		null,
		BeatmapCustomData)
	{

	}
}