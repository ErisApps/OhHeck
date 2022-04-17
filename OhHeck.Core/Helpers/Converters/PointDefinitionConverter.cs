using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

public class PointDefinitionConverter : JsonConverter<PointDefinitionData>
{
	public override PointDefinitionData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (PointDefinitionData?) JsonSerializer.Deserialize(ref reader, typeToConvert, options);

	public override void Write(Utf8JsonWriter writer, PointDefinitionData value, JsonSerializerOptions options)
	{
		if (value.Name is not null)
		{
			writer.WriteStringValue(value.Name);
		}
		else
		{
			JsonSerializer.Serialize(writer, value.Points, options);
		}
	}
}