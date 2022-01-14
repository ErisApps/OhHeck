using System;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Analyzer.SmellyJson;

[BeatmapWarning("string-bool")]
public class StringBoolAnalyzer : IBeatmapAnalyzer {

	public void Validate(Type fieldType, object? value, IWarningOutput warningOutput)
	{
		if (value is FakeTruthy fakeTruthy && fakeTruthy.IsString())
		{
			warningOutput.WriteWarning($"Boolean is string: \"{fakeTruthy}\"");
		}
	}
}