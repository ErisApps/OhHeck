using System;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Analyzer.Lints.SmellyJson;

[BeatmapWarning("string-bool")]
public class StringBoolAnalyzer : IFieldAnalyzer, IFieldOptimizer {

	// all my homies hate "true" in json values
	public void Validate(Type fieldType, in object? value, IWarningOutput outerWarningOutput)
	{
		if (value is FakeTruthy fakeTruthy && fakeTruthy.IsString())
		{
			outerWarningOutput.WriteWarning("Boolean is string: \"{FakeTruthy}\"", GetType(), fakeTruthy);
		}
	}

	public void Optimize(ref object? value)
	{
		if (value is FakeTruthy fakeTruthy)
		{
			value = fakeTruthy.IsTrue();
		}
	}
}