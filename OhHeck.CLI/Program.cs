﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Beatmap;

Console.WriteLine("Hello, World!");

var fileStream = File.OpenRead("./ExpertPlusLawless.dat");

var options = new JsonSerializerOptions()
{
	IgnoreReadOnlyProperties = false, IgnoreReadOnlyFields = false, IncludeFields = true,
	DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

};

var stopwatch = Stopwatch.StartNew();
var beatmapSaveData = JsonSerializer.Deserialize<BeatmapSaveData>(fileStream, options);
stopwatch.Stop();

Console.WriteLine($"Parsed beatmap in {stopwatch.ElapsedMilliseconds}ms");

if (beatmapSaveData is null)
{
	Console.WriteLine("Beatmap is null. Like my love and sanity");
	return 0;
}

Console.WriteLine($"Version {beatmapSaveData.Version}");
Console.WriteLine($"Events {beatmapSaveData.Events.Count}");
Console.WriteLine($"Notes {beatmapSaveData.Notes.Count}");
Console.WriteLine($"Obstacles {beatmapSaveData.Obstacles.Count}");

if (beatmapSaveData.BeatmapCustomData is not null)
{
	var beatmapCustomData = beatmapSaveData.BeatmapCustomData;

	Console.WriteLine($"Beatmap custom data is not null, noozle!");

	Console.WriteLine($"Point defs: {beatmapCustomData.PointDefinitions?.Count ?? -1}");
	Console.WriteLine($"Environment enhancements: {beatmapCustomData.EnvironmentEnhancements?.Count ?? -1}");
	Console.WriteLine($"Custom Events: {beatmapCustomData.CustomEvents?.Count ?? -1}");
}

return 0;