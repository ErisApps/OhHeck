using System;
using CommandLine;
using DryIoc;
using OhHeck.CLI;
using OhHeck.Core.Analyzer;
using Serilog;

// TODO: Optimizations
// for example:
// omit "_time": 0 // time is 0 by default, we can save a few bytes
// [[..., 0]] -> [...] // save a few bytes
// round point datas and yeet unnecessary
// make fern's life less painful


// I wish I wrote this in rust, but hey C# means I get cool dependency injection and runtime magic!
// Centipede will not load in less than 5 minutes with similar-point-data-slope wtf
#region Startup
// if -1, infinite warnings
CLIOptions? opt = null;
var result = Parser.Default.ParseArguments<CLIOptions>(args).WithParsed(option =>
{
	opt = option;
});

if (opt is null)
{
	return (int) result.Tag;
}


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
warningManager.Init(opt.SuppressedWarnings, opt.ConfigureWarningValuesProcessed);
#endregion

var maxWarningCount = opt.WarningCount;

if (opt.Test) {
	TestMapDefault("CentipedeEPlus");
	log.Information("");
	TestMapDefault("SomewhereOutThereEPlus");
}
else if (opt.ListWarnings)
{
	log.Information("Registered warning lints:");
	foreach (var (warningName, _) in warningManager.BeatmapAnalyzers)
	{
		// TODO: List configuration proerties
		log.Information("\t{WarningName}", warningName);
	}
} else {
	Testing.TestMap(log, opt.Map, warningManager, maxWarningCount);
}

return (int) result.Tag;

void TestMapDefault(string name)
{
	Testing.TestMap(log, $"./test_maps/{name}.dat", warningManager, maxWarningCount);
}