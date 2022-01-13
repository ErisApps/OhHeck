using System.Reflection;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Warnings.SmellyJson;

[BeatmapWarning("string-bool")]
public class StringBoolWarning : IBeatmapWarning {

	public string? Validate(FieldInfo fieldInfo, object? value)
	{
		return value is FakeTruthy fakeTruthy && fakeTruthy.IsString() ? $"Boolean is string: \"{fakeTruthy}\"" : null;
	}
}