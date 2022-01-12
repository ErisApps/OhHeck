using System;
using System.Text.Json.Serialization;
using OhHeck.Core.Json;

namespace OhHeck.Core.Structs;

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

	// Token: 0x06000FE8 RID: 4072 RVA: 0x00017EE6 File Offset: 0x000160E6
	public Vector3(float x, float y)
	{
		this.x = x;
		this.y = y;
		this.z = 0f;
	}

	public float this[int index]
	{
		get
		{
			var result = index switch
			{
				0 => this.x,
				1 => this.y,
				2 => this.z,
				_ => throw new IndexOutOfRangeException("Invalid Vector3 index!")
			};
			return result;
		}
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
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
			}
		}
	}
}