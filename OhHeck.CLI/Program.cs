using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using DryIoc;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap;
using Serilog;

using var log = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateLogger();

Log.Logger = log;

using var container = new Container();

// default logger
container.Register(Made.Of(() => Log.Logger), setup: Setup.With(condition: r => r.Parent.ImplementationType == null));

// type dependent logger
container.Register(
	Made.Of(() => Log.ForContext(Arg.Index<Type>(0)), r => r.Parent.ImplementationType),
	setup: Setup.With(condition: r => r.Parent.ImplementationType != null));

var defaultLogger = container.Resolve<ILogger>();

container.Register<WarningManager, WarningManager>(Reuse.Singleton);
var warningManager = container.Resolve<WarningManager>();
warningManager.Init(GetSuppresedWarnings(args));

void TestMap(string name)
{
	defaultLogger.Information($"Testing map {name}");
	var fileStream = File.OpenRead($"./test_maps/{name}.dat");

	var options = new JsonSerializerOptions()
	{
		IgnoreReadOnlyProperties = false, IgnoreReadOnlyFields = false, IncludeFields = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

	};

	var stopwatch = Stopwatch.StartNew();
	var beatmapSaveData = JsonSerializer.Deserialize<BeatmapSaveData>(fileStream, options);
	stopwatch.Stop();

	defaultLogger.Information($"Parsed beatmap in {stopwatch.ElapsedMilliseconds}ms");

	if (beatmapSaveData is null)
	{
		defaultLogger.Information("Beatmap is null. Like my love and sanity");
		return;
	}

	defaultLogger.Information($"Version {beatmapSaveData.Version}");
	defaultLogger.Information($"Events {beatmapSaveData.Events.Count}");
	defaultLogger.Information($"Notes {beatmapSaveData.Notes.Count}");
	defaultLogger.Information($"Obstacles {beatmapSaveData.Obstacles.Count}");

	if (beatmapSaveData.BeatmapCustomData is null)
	{
		return;
	}

	var beatmapCustomData = beatmapSaveData.BeatmapCustomData;

	defaultLogger.Information($"Beatmap custom data is not null, noozle!");

	defaultLogger.Information($"Point defs: {beatmapCustomData.PointDefinitions?.Count ?? -1}");
	defaultLogger.Information($"Environment enhancements: {beatmapCustomData.EnvironmentEnhancements?.Count ?? -1}");
	defaultLogger.Information($"Custom Events: {beatmapCustomData.CustomEvents?.Count ?? -1}");

	warningManager.AnalyzeBeatmap(beatmapSaveData);
}

TestMap("CentipedeEPlus");
defaultLogger.Information(string.Empty);
TestMap("SomewhereOutThereEPlus");


return 0;


HashSet<string> GetSuppresedWarnings(IEnumerable<string> args)
{
	return args.Where(s => s.StartsWith("-w")).Select(s => s["-w".Length..]).ToHashSet();
}