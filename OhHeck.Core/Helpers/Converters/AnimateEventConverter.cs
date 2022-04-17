using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

public class AnimateEventConverter : JsonConverter<AnimateEvent>
{
	public override AnimateEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> (AnimateEvent) BeatmapCustomEventListConverter.ReadEvent<AnimateEvent>(ref reader, options);

	public override void Write(Utf8JsonWriter writer, AnimateEvent value, JsonSerializerOptions options)
	{
		// TODO: Do this at serialization
		foreach (var (key, pointDefinitionDataProxy) in value.PointProperties)
		{
			value.Data[key] = JsonSerializer.SerializeToElement(pointDefinitionDataProxy, options);
		}

		JsonSerializer.Serialize(writer, value, typeof(BeatmapCustomEvent), options);
	}
}