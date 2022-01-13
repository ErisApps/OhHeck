using System;
using System.Reflection;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Analyzer.SmellyJson;

[BeatmapWarning("string-bool")]
public class StringBoolWarning : IBeatmapWarning {

	public string? Validate(Type fieldType, object? value)
	{
		return value is FakeTruthy fakeTruthy && fakeTruthy.IsString() ? $"Boolean is string: \"{fakeTruthy}\"" : null;
	}
}