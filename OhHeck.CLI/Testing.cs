﻿using System;
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
	private const int MAXIMUM_THREAD = 8;

	private static void Validate(Span<AnalyzeProcessedData> analyzeProcessedDatas, WarningManager warningManager, IWarningOutput warningOutput)
	{
		foreach (var analyzeProcessedData in analyzeProcessedDatas)
		{
			warningManager.Validate(analyzeProcessedData, warningOutput);
		}
	}


	private static async Task ParallelValidate(IEnumerable<AnalyzeProcessedData> analyzeProcessedDatas, WarningManager warningManager, IWarningOutput warningOutput)
	{
		var processedDatas = analyzeProcessedDatas.ToArray();
		if (processedDatas.Length == 0)
		{
			return;
		}

		var elementLimitPerThread = Math.Min(processedDatas.Length, processedDatas.Length / MAXIMUM_THREAD);

		// single threaded
		if (elementLimitPerThread == processedDatas.Length)
		{
			Validate(processedDatas, warningManager, warningOutput);
		}
		else
		{
			var tasks = new List<Task>(MAXIMUM_THREAD);

			for (var i = 0; i < MAXIMUM_THREAD; i++)
			{
				var start = elementLimitPerThread * (i);
				var end = Math.Max(elementLimitPerThread * (i + 1), processedDatas.Length - 1);

				tasks.Add(Task.Run(() => Validate(new Span<AnalyzeProcessedData>(processedDatas, start, end), warningManager, warningOutput)));

				// no more threads can be assigned work
				if (end == processedDatas.Length - 1)
				{
					break;
				}
			}

			await Task.WhenAll(tasks);
		}
	}

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

		WarningOutput warningOutput = new();
		stopwatch = Stopwatch.StartNew();

		var analyzeProcessedDatas = warningManager.AnalyzeBeatmap(beatmapSaveData);
		log.Information("Took {Time}ms to analyze beatmap", stopwatch.ElapsedMilliseconds);
		stopwatch.Restart();

		await ParallelValidate(analyzeProcessedDatas, warningManager, warningOutput);
		stopwatch.Stop();
		log.Information("Took {Time}ms to validate beatmap", stopwatch.ElapsedMilliseconds);


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