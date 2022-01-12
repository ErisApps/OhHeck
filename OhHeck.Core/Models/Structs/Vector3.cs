using System;
using System.Text.Json.Serialization;
using OhHeck.Core.Helpers.Converters;

namespace OhHeck.Core.Models.Structs;

[JsonConverter(typeof(Vector3Converter))]
public struct Vector3
{
	public float x;
	public float y;
	public float z;

	public Vector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3(float x, float y)
	{
		this.x = x;
		this.y = y;
		z = 0f;
	}

	public float this[int index]
	{
		get => index switch
		{
			0 => x,
			1 => y,
			2 => z,
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
				case 2:
					z = value;
					break;
				default:
					throw new IndexOutOfRangeException(nameof(index));
			}
		}
	}
}