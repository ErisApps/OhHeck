using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Helpers.Converters;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.beatmap.v3;

namespace OhHeck.Core.Helpers;

public static class BeatmapJsonParser
{

	public static BeatmapSaveData? ParseBeatmap(ref Stream stream, JsonSerializerOptions? options, out Stopwatch stopwatch)
	{
		var version = BeatmapJsonHelper.GetVersion(ref stream);

		options ??= new JsonSerializerOptions
		{
			IgnoreReadOnlyProperties = false,
			IgnoreReadOnlyFields = false,
			IncludeFields = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			// mappers grr, to make configurable somehow
			NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals
		};

		PointDefinitionReferenceHandler pointDefinitionReferenceHandler = new();
		options.ReferenceHandler = pointDefinitionReferenceHandler;

		stopwatch = Stopwatch.StartNew();

		if (version.CompareTo(BeatmapJsonHelper.version2_6_0) <= 0)
		{
			return BeatmapJsonHelper.ConvertBeatmapSaveData(JsonSerializer.Deserialize<Models.Beatmap.v2.BeatmapSaveData>(stream, options)!);
		}
		var result = JsonSerializer.Deserialize<BeatmapSaveData>(stream, options);
		stopwatch.Stop();

		return result;
	}

}