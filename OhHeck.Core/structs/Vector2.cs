using System;

namespace OhHeck.Core.structs;

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
		get
		{
			float result;
			if (index != 0)
			{
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid Vector2 index!");
				}
				result = this.y;
			}
			else
			{
				result = this.x;
			}
			return result;
		}
		set
		{
			if (index != 0)
			{
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid Vector2 index!");
				}
				this.y = value;
			}
			else
			{
				this.x = value;
			}
		}
	}
}