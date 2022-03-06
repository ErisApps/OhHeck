using System;
using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Models.ModData.Chroma;

namespace OhHeck.Core.Analyzer.Lints;

[BeatmapWarning("ends-with-regex-lookup")]
public class EndsWithRegexLookup : IFieldAnalyzer
{
	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput)
	{
		if (value is not List<EnvironmentEnhancement> environmentEnhancements)
		{
			return;
		}

		foreach (var regex in
		         from environmentEnhancement in environmentEnhancements
		         where environmentEnhancement.LookupMethod is LookupMethod.Regex
		         select environmentEnhancement.Id into regex
		         where regex.EndsWith("$") select regex)
		{
			outerWarningOutput.WriteWarning($"Using $ at the end of a regex lookup. Consider using EndsWith lookup instead. Regex: {regex}", GetType());
		}
	}
}