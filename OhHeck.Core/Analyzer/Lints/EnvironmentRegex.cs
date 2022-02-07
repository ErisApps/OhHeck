using System;
using OhHeck.Core.Helpers;
using OhHeck.Core.Models.ModData.Chroma;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Analyzer.Lints;

[BeatmapWarning("environment-regex")]
public class EnvironmentRegex : IBeatmapAnalyzer {

	public void Validate(Type fieldType, object? value, IWarningOutput warningOutput)
	{
		if (value is not EnvironmentEnhancement { LookupMethod: LookupMethod.Regex } environmentEnhancement)
		{
			return;
		}

		var regex = environmentEnhancement.Id;

		if (!RegexBindings.IsRegexValid(regex, out var message))
		{
			warningOutput.WriteWarning(message);
		}
	}
}