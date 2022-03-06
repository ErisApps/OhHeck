using System;
using System.Collections.Generic;
using System.Linq;
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

#region Startup
// if -1, infinite warnings
var maxWarningCount = GetWarningCount(args) ?? 20;


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

if (args.Any(e => e == "-test"))
{
	TestMapDefault("CentipedeEPlus");
	log.Information("");
	TestMapDefault("SomewhereOutThereEPlus");
}
else
{
	Testing.TestMap(log, args.First(e => !e.StartsWith("-") && !e.StartsWith("-wc ")), warningManager, maxWarningCount);
}

return 0;

void TestMapDefault(string name)
{
	Testing.TestMap(log, $"./test_maps/{name}.dat", warningManager, maxWarningCount);
}

HashSet<string> GetSuppressedWarnings(IEnumerable<string> args)
{
	return args.Where(s => s.StartsWith("-w")).Select(s => s["-w".Length..]).ToHashSet();
}

int? GetWarningCount(IReadOnlyList<string> args)
{
	for (var i = 0; i < args.Count; i++)
	{
		var s = args[i];

		if (s == "-wc" && int.TryParse(args[i + 1], out var l))
		{
			return l;
		}
	}

	return null;
}