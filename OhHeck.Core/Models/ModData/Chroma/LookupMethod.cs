using System.Text.Json.Serialization;

namespace OhHeck.Core.Models.ModData.Chroma;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LookupMethod
{
	Regex,
	Exact,
	Contains
}