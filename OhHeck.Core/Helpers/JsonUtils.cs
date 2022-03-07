using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Helpers.Converters;
using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Helpers;

public static class JsonUtils
{

	public static BeatmapSaveData? ParseBeatmap(Stream stream, JsonSerializerOptions? options, out Stopwatch stopwatch)
	{
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
		var beatmapSaveData = JsonSerializer.Deserialize<BeatmapSaveData>(stream, options);
		stopwatch.Stop();

		return beatmapSaveData;
	}

}