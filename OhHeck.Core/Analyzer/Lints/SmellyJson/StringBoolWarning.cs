using System;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Analyzer.Lints.SmellyJson;

[BeatmapWarning("string-bool")]
public class StringBoolAnalyzer : IFieldAnalyzer {

	// all my homies hate "true" in json values
	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput)
	{
		if (value is FakeTruthy fakeTruthy && fakeTruthy.IsString())
		{
			outerWarningOutput.WriteWarning($"Boolean is string: \"{fakeTruthy}\"", GetType());
		}
	}
}