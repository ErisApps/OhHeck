using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Analyzer.Implementation;
using OhHeck.Core.Helpers;
using OhHeck.Core.Helpers.Enumerable;
using Serilog.Core;
using Serilog.Parsing;

namespace OhHeck.CLI;

public static class Testing
{

	private static void Validate(IEnumerable<AnalyzeProcessedData> analyzeProcessedDatas, WarningManager warningManager, IWarningOutput warningOutput)
	{
		foreach (var analyzeProcessedData in analyzeProcessedDatas)
		{
			try
			{
				warningManager.Validate(analyzeProcessedData, warningOutput);
			}
			catch (Exception e)
			{
				// Throw with information
				throw new AggregateException(
					$"Caught an exception on field {analyzeProcessedData.WarningContext.Parent} {analyzeProcessedData.WarningContext.Type}:{analyzeProcessedData.WarningContext.MemberLocation}", e);
			}
		}
	}

	private static string FormatString(string message, IReadOnlyList<object?>? formatParams)
	{
		string messageFormatted;
		if (formatParams is not null && formatParams.Count != 0)
		{
			var stringBuilder = new StringBuilder(message.Length);
			var parser = new MessageTemplateParser();

			var template = parser.Parse(message);

			var index = 0;
			foreach (var tok in template.Tokens)
			{
				object s = tok;
				if (tok is not TextToken)
				{
					s = formatParams[index++] ?? "null";

					if (s is IEnumerable enumerable and not string)
					{
						s = enumerable.ArrayToString();
					}
				}

				stringBuilder.Append(s);
			}

			messageFormatted = stringBuilder.ToString();
		}
		else
		{
			messageFormatted = message;
		}

		return messageFormatted;
	}

	public static void TestMap(Logger log, string name, WarningManager warningManager, int maxWarningCount)
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

		Validate(analyzeProcessedDatas, warningManager, warningOutput);
		stopwatch.Stop();
		log.Information("Took {Time}ms to validate beatmap", stopwatch.ElapsedMilliseconds);


		var warningCount = 0;
		var analyzerNameDictionary = new Dictionary<Type, string>();
		foreach (var (message, warningInfo, analyzerType, formatParams) in warningOutput.GetWarnings())
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



			var messageFormatted = FormatString(message, formatParams);
			log.Warning("Warning [{AnalyzerName}]: {Type}:{{{MemberLocation}}} {MessageFormatted}", analyzerName, type, memberLocation, messageFormatted);
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