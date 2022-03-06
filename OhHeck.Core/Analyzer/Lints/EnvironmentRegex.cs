using System;
using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Helpers;
using OhHeck.Core.Models.ModData.Chroma;

namespace OhHeck.Core.Analyzer.Lints;

[BeatmapWarning("environment-regex")]
public class EnvironmentRegex : IFieldAnalyzer {

	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput)
	{
		if (value is not List<EnvironmentEnhancement> environmentEnhancements)
		{
			return;
		}

		foreach (var regex in
		         from environmentEnhancement in environmentEnhancements
		         where environmentEnhancement.LookupMethod is LookupMethod.Regex
		         select environmentEnhancement.Id)
		{
			if (!RegexBindings.IsRegexValid(regex, out var message))
			{
				outerWarningOutput.WriteWarning($"{regex} is invalid: {message}");
			}
		}
	}
}