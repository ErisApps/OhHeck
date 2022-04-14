using System.Collections.Generic;
using System.Linq;
using CommandLine;
using OhHeck.Core.Analyzer.Implementation;

namespace OhHeck.CLI;

public class CLIOptions
{
	public CLIOptions(string map, bool listWarnings, bool test, int warningCount, IEnumerable<string> suppressedWarnings, IEnumerable<string> configureWarningValues)
	{
		Map = map;
		ListWarnings = listWarnings;
		Test = test;
		WarningCount = warningCount;
		SuppressedWarnings = suppressedWarnings;
		ConfigureWarningValues = configureWarningValues;
	}

	[Option(SetName = "mapLoad")]
	public string Map { get; }

	[Option('l',"list", Default = false, SetName = "mapLoad")]
	public bool ListWarnings { get; }

	[Option(Default = false, SetName = "mapLoad")]
	public bool Test { get; }

	[Option("warning-count", Default = 20)]
	public int WarningCount { get; }

	[Option("suppressed-warnings", Separator = ';')]
	public IEnumerable<string> SuppressedWarnings { get; }

	[Option("configure")]
	public IEnumerable<string> ConfigureWarningValues { get; }

	public IEnumerable<ConfigureWarningValue> ConfigureWarningValuesProcessed => ConfigureWarningValues.Select(ConfigureWarningValue.FromString);
}