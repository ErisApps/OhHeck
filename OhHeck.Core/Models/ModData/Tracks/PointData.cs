using System;
using System.Collections.Generic;
using System.Linq;

namespace OhHeck.Core.Models.ModData.Tracks;

public class PointData
{
	public float[] Data { get; }

	public float Time { get; }

	// if null, it SHOULD mean the first point
	public Functions? Easing { get; }

	public bool Smooth { get; } = false;

	public PointData(float[] data, float time, Functions? easing, bool smooth)
	{
		Data = data;
		Time = time;
		Easing = easing;
		Smooth = smooth;
	}

	public PointData(List<object> data)
	{
		List<float> preData = new();

		foreach (var o in data)
		{
			switch (o)
			{
				case int:
				case float:
					preData.Add((float) o);
					break;
				case string str:
				{
					if (str == "splineCatmullRom")
					{
						Smooth = true;
					}
					else
					{
						Easing = Enum.Parse(typeof(Functions), str) as Functions?;
					}

					break;
				}
			}
		}

		Time = preData.Last();
		Data = preData.GetRange(0, preData.Count - 1).ToArray();
	}
}