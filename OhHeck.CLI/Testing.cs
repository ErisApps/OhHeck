using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Analyzer.Implementation;
using OhHeck.Core.Helpers;
using Serilog.Core;

namespace OhHeck.CLI;

public static class Testing
{

	public static async Task TestMap(Logger log, string name, WarningManager warningManager, int maxWarningCount)
	{
		log.Information("Testing map {Name}", name);
		var fileStream = File.OpenRead(name);

		var beatmapSaveData = JsonUtils.ParseBeatmap(fileStream, null, out var stopwatch);

		log.Information("Parsed beatmap in {Count}ms", stopwatch.ElapsedMilliseconds);

		if (beatmapSaveData is null)
		{
			log.Information("Beatmap is null. Like my love and sanity");
			return;
		}

		log.Information("Version {Version}", beatmapSaveData.Version);
		log.Information("Events {Count}", beatmapSaveData.Events.Count);
		log.Information("Notes {Count}", beatmapSaveData.Notes.Count);
		log.Information("Obstacles {Count}", beatmapSaveData.Obstacles.Count);

		if (beatmapSaveData.BeatmapCustomData is null)
		{
			return;
		}

		var beatmapCustomData = beatmapSaveData.BeatmapCustomData;

		log.Information("Beatmap custom data is not null, noozle!");

		log.Information("Point defs: {Count}", beatmapCustomData.PointDefinitions?.Count ?? -1);
		log.Information("Environment enhancements: {Count}", beatmapCustomData.EnvironmentEnhancements?.Count ?? -1);
		log.Information("Custom Events: {Count}", beatmapCustomData.CustomEvents?.Count ?? -1);

		stopwatch = Stopwatch.StartNew();
		WarningOutput warningOutput = new();
		await warningManager.AnalyzeBeatmap(beatmapSaveData, warningOutput).ConfigureAwait(true);
		stopwatch.Stop();
		log.Information("Took {Time}ms to analyze beatmap", stopwatch.ElapsedMilliseconds);

		var warningCount = 0;
		var analyzerNameDictionary = new Dictionary<Type, string>();
		foreach (var (message, warningInfo, analyzerType) in warningOutput.GetWarnings())
		{
			if (!analyzerNameDictionary.TryGetValue(analyzerType, out var analyzerName))
			{
				analyzerNameDictionary[analyzerType] = analyzerName = analyzerType.GetCustomAttribute<BeatmapWarningAttribute>()?.Name ?? analyzerType.Name;
			}

			warningCount++;
			if (maxWarningCount != -1 && warningCount > maxWarningCount)
			{
				break;
			}

			var (type, memberLocation, parent) = warningInfo;
			log.Warning("Warning [{AnalyzerName}]: {Type}:{{{MemberLocation}}} {Message}", analyzerName, type, memberLocation, message);
			if (parent is not null)
			{
				log.Warning("Parent {FriendlyName} {ExtraData}", parent.GetFriendlyName(), parent.ExtraData());
			}

			log.Warning("");
		}

		if (warningCount <= maxWarningCount || maxWarningCount == -1)
		{
			return;
		}

		log.Warning("Warning count exceeded max warning count {MaxWarningCount}", maxWarningCount);
		log.Warning("Remaining {WarningCount}", warningOutput.GetWarnings().Count() - warningCount);
	}
}