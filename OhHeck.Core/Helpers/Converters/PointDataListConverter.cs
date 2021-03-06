using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

// Parses a List of PointData such as is in PointDefinitionData
public class PointDataListConverter : JsonConverter<List<PointData>>
{
	private static PointData ParsePointData(ref Utf8JsonReader reader, bool single)
	{
		var smooth = false;
		Functions? easing = null;

		List<float> preData = new();

		void HandleToken(ref Utf8JsonReader reader)
		{
			switch (reader.TokenType)
			{
				// Point data is [...]
				case JsonTokenType.Number:
					preData.Add((float) reader.GetDouble());
					break;
				case JsonTokenType.String:
					try
					{
						var str = reader.GetString()!;
						if (str == PointData.SMOOTHIDENTIFIER)
						{
							smooth = true;
						}
						else
						{
							easing = Enum.Parse(typeof(Functions), str) as Functions?;
						}
					}
					catch (Exception e)
					{
						throw new JsonException(null, e);
					}

					break;
				default:
					throw new JsonException();
			}
		}

		// handle the first token
		if (single)
		{
			HandleToken(ref reader);
		}

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				break;
			}

			HandleToken(ref reader);
		}


		float time;
		float[] animationData;

		if (single)
		{
			// single point defs are 0 time
			time = 0;
			animationData = preData.ToArray();
		}
		else
		{
			time = preData.Last();
			animationData = preData.GetRange(0, preData.Count - 1).ToArray(); // exclude time from array
		}

		return new PointData(time: time, data: animationData, smooth: smooth, easing: easing);
	}

	private static void WritePointData(ref Utf8JsonWriter writer, PointData point, bool single)
	{
		// single point def
		writer.WriteStartArray();
		foreach (var x in point.Data)
		{
			writer.WriteNumberValue(x);
		}

		if (!single)
		{
			writer.WriteNumberValue(point.Time);

			if (point.Easing is not null)
			{
				writer.WriteStringValue(point.Easing.ToString());
			}

			if (point.Smooth)
			{
				writer.WriteStringValue(PointData.SMOOTHIDENTIFIER);
			}
		}

		writer.WriteEndArray();
	}

	public override List<PointData>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		List<PointData> pointDatas = new();


		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException("Not an array");
		}

		// Parses an array of floats/strings and inner arrays of floats/strings
		// [0, 0, 0, "ease"] and [[0, 0, 0], [1, 1, 1, "easeLinear"]]
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				break;
			}

			switch (reader.TokenType)
			{
				// point data is [[...], [...]]
				case JsonTokenType.StartArray:
					while (reader.TokenType != JsonTokenType.EndArray)
					{
						pointDatas.Add(ParsePointData(ref reader, false));
					}
					break;
				// point data is [...]
				case JsonTokenType.Number:
				case JsonTokenType.String:
					pointDatas.Add(ParsePointData(ref reader, true));
					goto finish;

				default:
					throw new JsonException($"Not a array or number. Received {reader.TokenType}");
			}
		}

		finish:
		PointHelper.SortPoints(pointDatas);
		return pointDatas;
	}

	public override void Write(Utf8JsonWriter writer, List<PointData> value, JsonSerializerOptions options)
	{
		if (value.Count == 1)
		{
			var point = value.First();
			WritePointData(ref writer, point, true);
		}
		else
		{
			writer.WriteStartArray();
			foreach (var point in value)
			{
				WritePointData(ref writer, point, false);
			}
			writer.WriteEndArray();
		}
	}
}