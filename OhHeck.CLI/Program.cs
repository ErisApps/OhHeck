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

#region Startup
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

container.Register<WarningManager, WarningManager>(Reuse.Singleton);
var warningManager = container.Resolve<WarningManager>();
warningManager.Init(GetSuppressedWarnings(args));
#endregion

void TestMap(string name)
{
	log.Information($"Testing map {name}");
	var fileStream = File.OpenRead($"./test_maps/{name}.dat");

	var options = new JsonSerializerOptions()
	{
		IgnoreReadOnlyProperties = false, IgnoreReadOnlyFields = false, IncludeFields = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

	};

	var stopwatch = Stopwatch.StartNew();
	var beatmapSaveData = JsonSerializer.Deserialize<BeatmapSaveData>(fileStream, options);
	stopwatch.Stop();

	log.Information($"Parsed beatmap in {stopwatch.ElapsedMilliseconds}ms");

	if (beatmapSaveData is null)
	{
		log.Information("Beatmap is null. Like my love and sanity");
		return;
	}

	log.Information($"Version {beatmapSaveData.Version}");
	log.Information($"Events {beatmapSaveData.Events.Count}");
	log.Information($"Notes {beatmapSaveData.Notes.Count}");
	log.Information($"Obstacles {beatmapSaveData.Obstacles.Count}");

	if (beatmapSaveData.BeatmapCustomData is null)
	{
		return;
	}

	var beatmapCustomData = beatmapSaveData.BeatmapCustomData;

	log.Information("Beatmap custom data is not null, noozle!");

	log.Information($"Point defs: {beatmapCustomData.PointDefinitions?.Count ?? -1}");
	log.Information($"Environment enhancements: {beatmapCustomData.EnvironmentEnhancements?.Count ?? -1}");
	log.Information($"Custom Events: {beatmapCustomData.CustomEvents?.Count ?? -1}");

	warningManager.AnalyzeBeatmap(beatmapSaveData);
}

TestMap("CentipedeEPlus");
log.Information(string.Empty);
TestMap("SomewhereOutThereEPlus");


return 0;


HashSet<string> GetSuppressedWarnings(IEnumerable<string> args)
{
	return args.Where(s => s.StartsWith("-w")).Select(s => s["-w".Length..]).ToHashSet();
}