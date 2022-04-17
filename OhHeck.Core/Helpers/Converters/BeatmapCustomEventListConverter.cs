using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

public class BeatmapCustomEventListConverter: JsonConverter<List<BeatmapCustomEvent>>
{
	public override List<BeatmapCustomEvent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException("Not an array");
		}

		List<BeatmapCustomEvent> list = new();

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				break;
			}


			list.Add(ReadEvent<BeatmapCustomEvent>(ref reader, options));
		}


		return list;
	}

	internal static BeatmapCustomEvent ReadEvent<T>(ref Utf8JsonReader reader, JsonSerializerOptions options) where T: BeatmapCustomEvent
	{
		var restore = reader;
		// var tempReader = new Utf8JsonReader(reader., reader.IsFinalBlock, reader.CurrentState, options);
		BeatmapCustomEvent basicBeatmapCustomEvent = JsonSerializer.Deserialize<T>(ref reader, options)!;


		switch (basicBeatmapCustomEvent.Type)
		{
			case EventTypes.ANIMATE_TRACK:
				reader = restore;
				basicBeatmapCustomEvent = JsonSerializer.Deserialize<AnimateTrackEvent>(ref reader, options)!;
				break;
			case EventTypes.ASSIGN_PATH_ANIMATION:
				reader = restore;
				basicBeatmapCustomEvent = JsonSerializer.Deserialize<AssignPathAnimationEvent>(ref reader, options)!;
				break;
		}

		if (basicBeatmapCustomEvent is not AnimateEvent animateEvent)
		{
			return basicBeatmapCustomEvent;
		}

		var propDictionary = animateEvent.Data
				// skip keys that are not PointData types
			.Where(s => s.Key is not ("_track" or "_duration" or "_easing"))
			.ToDictionary(s => s.Key, s => s.Value);
		animateEvent.PointProperties = PointHelper.PointDefinitionDatasFromDictionary(propDictionary);

		return basicBeatmapCustomEvent;
	}



	public override void Write(Utf8JsonWriter writer, List<BeatmapCustomEvent> value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		foreach (var beatmapCustomEvent in value)
		{
			switch (beatmapCustomEvent)
			{
				case null:
					continue;
				case AnimateEvent animateEvent:
					JsonSerializer.Serialize(writer, animateEvent, options);
					break;
				default:
					JsonSerializer.Serialize(writer, beatmapCustomEvent, options);
					break;
			}
		}
		writer.WriteEndArray();
	}
}