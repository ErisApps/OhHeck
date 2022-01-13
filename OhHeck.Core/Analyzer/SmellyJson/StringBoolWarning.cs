using System.Reflection;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Analyzer.SmellyJson;

[BeatmapWarning("string-bool")]
public class StringBoolWarning : IBeatmapWarning {

	public string? Validate(FieldInfo fieldInfo, object? value)
	{
		if (value is FakeTruthy fakeTruthy && fakeTruthy.IsString())
		{
			return $"Boolean is string: \"{fakeTruthy}\"";
		}

		return null;
	}
}