using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OhHeck.Core.Models.Beatmap.Enums;
using OhHeck.Core.Models.Beatmap.v2.Enums;
using OhHeck.Core.Models.beatmap.v3;
using OhHeck.Core.Models.Beatmap.v3.Enums;
using BeatmapSaveData = OhHeck.Core.Models.beatmap.v3.BeatmapSaveData;
using Beatmapv2 = OhHeck.Core.Models.Beatmap.v2;
using Beatmapv3 = OhHeck.Core.Models.Beatmap.v3;
using ObstacleData = OhHeck.Core.Models.beatmap.v3.ObstacleData;
using SliderData = OhHeck.Core.Models.beatmap.v3.SliderData;
using SliderMidAnchorMode = OhHeck.Core.Models.Beatmap.v3.Enums.SliderMidAnchorMode;
using WaypointData = OhHeck.Core.Models.beatmap.v3.WaypointData;

namespace OhHeck.Core.Models.Beatmap;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
public class BeatmapJsonHelper
{
	// Stolen from SongCore
	private static readonly Regex VersionRegex = new(
		@"""_?version""\s*:\s*""(?<version>[0-9]+\.[0-9]+\.?[0-9]?)""",
		RegexOptions.Compiled | RegexOptions.CultureInvariant
	);

	public static Version GetVersionRegex(string data)
	{
		var truncatedText = data.Length > 50 ? data[..50] : data;
		var match = VersionRegex.Match(truncatedText);
		return !match.Success ? FallbackVersion : new Version(match.Groups["version"].Value);
	}

	public static Version GetVersion(string data)
	{
		var text = data[..50];
		var num = text.IndexOf("\"_version\":\"", StringComparison.Ordinal);
		int num2;
		if (num == -1)
		{
			num2 = text.IndexOf("\"version\":\"", StringComparison.Ordinal);
			num2 += "\"version\":\"".Length;
		}
		else
		{
			num2 = num + "\"_version\":\"".Length;
		}

		var num3 = text.IndexOf("\"", num2, StringComparison.Ordinal);
		return new Version(text.Substring(num2, num3 - num2));
	}

	public static Version GetVersion(ref Stream data)
	{
		if (!data.CanSeek)
		{
			var newStream = new MemoryStream();
			data.CopyTo(newStream);
			data.Dispose();
			data = newStream;
		}

		var bytes = new byte[50];
		data.Read(bytes, 0, 50);
		data.Seek(0, SeekOrigin.Begin);

		var str = Encoding.UTF8.GetString(bytes);

		return GetVersionRegex(str);
	}

	public static BeatmapSaveData ConvertBeatmapSaveData(Beatmapv2.BeatmapSaveData beatmapSaveData)
	{
		var events = beatmapSaveData.Events;
		var notes = beatmapSaveData.Notes;
		var obstacles = beatmapSaveData.Obstacles;
		var sliders = beatmapSaveData.Sliders;
		var waypoints = beatmapSaveData.Waypoints;

		events.Sort();
		notes.Sort();
		sliders?.Sort();
		waypoints.Sort();
		obstacles.Sort();

		var convertedNotes = new List<ColorNoteData>(notes.Count);
		var convertedBombs = new List<BombNoteData>(200);
		foreach (var noteData in notes)
		{
			if (noteData.Type == NoteType.Bomb)
			{
				convertedBombs.Add(new BombNoteData(noteData.Time, noteData.LineIndex, noteData.LineLayer, noteData.NoteCustomData));
			}
			else
			{
				convertedNotes.Add(new ColorNoteData(noteData.Time, noteData.LineIndex, noteData.LineLayer, GetNoteColorType(noteData.Type), noteData.CutDirection, 0, noteData.NoteCustomData));
			}
		}

		var convertedObstacles = obstacles.Select(obstacleData => new ObstacleData(obstacleData.Time, obstacleData.LineIndex, GetLayerForObstacleType(obstacleData.Type), obstacleData.Duration, obstacleData.Width, GetHeightForObstacleType(obstacleData.Type), obstacleData.ObstacleCustomData)).ToList();

		var convertedSliders = sliders?.Select(sliderData => new SliderData(sliderData.Time, GetNoteColorType(sliderData.ColorType), sliderData.HeadLineIndex, (int) sliderData.HeadLineLayer,
			sliderData.HeadControlPointLengthMultiplier, sliderData.HeadCutDirection, sliderData.TailTime, sliderData.TailLineIndex, (int) sliderData.TailLineLayer,
			sliderData.TailControlPointLengthMultiplier, sliderData.TailCutDirection, (SliderMidAnchorMode) sliderData.SliderMidAnchorMode)).ToList() ?? new List<SliderData>();

		var convertedWaypoints = waypoints.Select(waypointData => new WaypointData(waypointData.Time, waypointData.LineIndex, (int) waypointData.LineLayer, waypointData.OffsetDirection)).ToList();

		var bpmChangeEventDatas = new List<BpmChangeEventData>(100);
		var convertedEvents = new List<BasicEventData>(events.Count);
		var convertedRotations = new List<RotationEventData>(100);
		var convertedBoostEvents = new List<ColorBoostEventData>(100);
		foreach (var eventData in events)
		{
			var type = eventData.Type;
			if (type <= BeatmapEventType.Event14)
			{
				switch (type)
				{
					case BeatmapEventType.Event5:
						convertedBoostEvents.Add(new ColorBoostEventData(eventData.Time, eventData.Value == 1));
						continue;
					case BeatmapEventType.Event14:
						convertedRotations.Add(new RotationEventData(eventData.Time, ExecutionTime.Early, SpawnRotationForEventValue(eventData.Value)));
						continue;
				}
			}
			else
			{
				switch (type)
				{
					case BeatmapEventType.Event15:
						convertedRotations.Add(new RotationEventData(eventData.Time, ExecutionTime.Late, SpawnRotationForEventValue(eventData.Value)));
						continue;
					case BeatmapEventType.BpmChange:
						bpmChangeEventDatas.Add(new BpmChangeEventData(eventData.Time, eventData.FloatValue ?? 0));
						continue;
				}
			}

			convertedEvents.Add(new BasicEventData(eventData.Time, eventData.Type, eventData.Value, eventData.FloatValue, eventData.EventCustomData));
		}

		var lightColorEventBoxGroups = new List<LightColorEventBoxGroup>();
		var lightRotationEventBoxGroups = new List<LightRotationEventBoxGroup>();
		var specialEventsKeywordFilters = beatmapSaveData.SpecialEventsKeywordFilters;
		int? num;
		if (specialEventsKeywordFilters == null)
		{
			num = null;
		}
		else
		{
			var keywords = specialEventsKeywordFilters.Keywords;
			num = ((keywords != null) ? new int?(keywords.Count) : null);
		}

		var basicEventTypesForKeywords = new List<BasicEventTypesWithKeywords.BasicEventTypesForKeyword>(num ?? 0);
		if (specialEventsKeywordFilters is { Keywords: { } })
		{
			basicEventTypesForKeywords.AddRange(specialEventsKeywordFilters.Keywords.Select(specialEventsForKeyword => new BasicEventTypesWithKeywords.BasicEventTypesForKeyword(specialEventsForKeyword.Keyword, specialEventsForKeyword.SpecialEvents)));
		}

		var basicEventTypesWithKeywords = new BasicEventTypesWithKeywords(basicEventTypesForKeywords);
		return new BeatmapSaveData("3.0.0", bpmChangeEventDatas, convertedRotations,convertedEvents, convertedBoostEvents, lightColorEventBoxGroups, lightRotationEventBoxGroups, convertedObstacles, convertedNotes, convertedBombs, convertedSliders, new List<BurstSliderData>(), convertedWaypoints, basicEventTypesWithKeywords, true, beatmapSaveData.BeatmapCustomData);
	}

	private static NoteColorType GetNoteColorType(NoteType noteType) => noteType == NoteType.NoteB ? NoteColorType.ColorB : NoteColorType.ColorA;
	private static NoteColorType GetNoteColorType(ColorType colorType) => colorType == ColorType.ColorB ? NoteColorType.ColorB : NoteColorType.ColorA;
	private static int GetHeightForObstacleType(ObstacleType obstacleType) => obstacleType != ObstacleType.Top ? 5 : 3;
	private static int GetLayerForObstacleType(ObstacleType obstacleType) => obstacleType != ObstacleType.Top ? 0 : 2;
	private static Beatmapv3.Enums.SliderType GetSliderType(Beatmapv2.Enums.SliderType sliderType) => sliderType == Beatmapv2.Enums.SliderType.Burst ? Beatmapv3.Enums.SliderType.Burst : Beatmapv3.Enums.SliderType.Normal;

	// Token: 0x06000115 RID: 277 RVA: 0x00004D9B File Offset: 0x00002F9B
	private static float SpawnRotationForEventValue(int index)
	{
		if (index >= 0 && index < SpawnRotations.Length)
		{
			return SpawnRotations[index];
		}
		return 0f;
	}

	private static readonly float[] SpawnRotations = {
		-60f,
		-45f,
		-30f,
		-15f,
		15f,
		30f,
		45f,
		60f
	};

	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public static readonly Version version2_6_0 = new("2.6.0");
	private static readonly Version FallbackVersion = new("2.0.0");
	private const string K_LEGACY_VERSION_SEARCH_STRING = "\"_version\":\"";
	private const string K_VERSION_SEARCH_STRING = "\"version\":\"";
}