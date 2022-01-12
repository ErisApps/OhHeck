using System;
using System.Text.Json.Serialization;
using OhHeck.Core.Helpers.Converters;

namespace OhHeck.Core.Models.Structs;

[JsonConverter(typeof(Vector2Converter))]
public struct Vector2
{
	public float x;
	public float y;

	public Vector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public float this[int index]
	{
		get => index switch
		{
			0 => x,
			1 => y,
			_ => throw new IndexOutOfRangeException(nameof(index))
		};
		set
		{
			switch (index)
			{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				default:
					throw new IndexOutOfRangeException(nameof(index));
			}
		}
	}
}