using System;
using System.Linq;
using System.Text.Json;

namespace OhHeck.Core.Helpers;

public static class JSONUtils
{
	public static object? ToNativeType(this JsonElement jsonElement) =>
		jsonElement.ValueKind switch
		{
			JsonValueKind.Array => jsonElement.EnumerateArray().Select(o => o.ToNativeType()).ToList(),
			JsonValueKind.String => jsonElement.GetString(),
			JsonValueKind.Undefined => null,
			JsonValueKind.Object => jsonElement.EnumerateObject().ToDictionary(k => k.Name, v => v.Value.ToNativeType()),
			JsonValueKind.Number => jsonElement.GetSingle(),
			JsonValueKind.True => jsonElement.GetBoolean(),
			JsonValueKind.False => jsonElement.GetBoolean(),
			JsonValueKind.Null => null,
			_ => throw new ArgumentOutOfRangeException(nameof(jsonElement))
		};
}