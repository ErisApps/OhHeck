using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OhHeck.Core.Helpers.Enumerable;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("similar-point-data-slope")]
public class SimilarPointDataSlope : IBeatmapAnalyzer
{

	// These numbers at quick glance seem to be fairly reliable, nice
	// however they should be configurable or looked at later
	private const float DIFFERENCE_THRESHOLD = 0.003f; // TODO: Configurable
	private const float Y_INTERCEPT_DIFFERENCE_THRESHOLD = 0.5f; // TODO: Configurable
	private const bool COMPARE_ALL_PREVIOUS_POINTS = true; // TODO: Configurable

	// Compares points a, b and c where b is between a and c.
	// If a-b's slope is similar to a-c, it deems it unnecessary
	public void Validate(Type fieldType, object? value, IWarningOutput warningOutput)
	{
		if (value is not List<PointData> { Count: > 1 } pointDatas)
		{
			return;
		}


		// compare 3 points consecutively
		var prevPoint = pointDatas.First(); // if empty, we have a problem
		for (var i = 1; i < pointDatas.Count - 1; i++)
		{
			var endPoint = pointDatas[i + 1];
			var middlePoint = pointDatas[i];


			if (ComparePoints(prevPoint, middlePoint, endPoint, out var middleSlope, out var endSlope))
			{
				WriteWarning(warningOutput, prevPoint, middlePoint, middleSlope, endPoint, endSlope);
				continue;
			}


			prevPoint = middlePoint;

			// compare every point from the start
			for (var j = 0; j < i - 2; j++)
			{
				// compare every point between start and end
				for (var k = j + 1; k < i - 1; k++)
				{
					var startPoint = pointDatas[k];
					middlePoint = pointDatas[j];

					if (ComparePoints(startPoint, middlePoint, endPoint, out middleSlope, out endSlope))
					{
						WriteWarning(warningOutput, startPoint, middlePoint, middleSlope, endPoint, endSlope);
					}
				}
			}
		}


	}

	private static void WriteWarning(IWarningOutput warningOutput, PointData startPoint, PointData middlePoint, IEnumerable<float> middleSlope, PointData endPoint, IEnumerable<float> endSlope) =>
		warningOutput.WriteWarning($"Point data slope and y intercept are closely intercepting and match easing/smooth {DIFFERENCE_THRESHOLD}: " +
		                           $"Point1 {startPoint.Data.ArrayToString()}:{startPoint.Time} " +
		                           $"Point2: {middlePoint.Data.ArrayToString()}:{middlePoint.Time} slope ({middleSlope.ArrayToString()}) " +
		                           $"Point3: {endPoint.Data.ArrayToString()}:{endPoint.Time} slope ({endSlope.ArrayToString()})");

	/// <summary>
	///
	/// </summary>
	/// <param name="startPoint"></param>
	/// <param name="middlePoint"></param>
	/// <param name="endPoint"></param>
	/// <param name="middleSlope"></param>
	/// <param name="endSlope"></param>
	/// <returns>true if similar</returns>
	private static bool ComparePoints(PointData startPoint, PointData middlePoint, PointData endPoint, [NotNullWhen(true)] out float[]? middleSlope, [NotNullWhen(true)] out float[]? endSlope)
	{
		middleSlope = null;
		endSlope = null;

		// Skip points where their easing or smoothness is different,
		// which would allow for middlePoint to cause a non-negligible difference
		if (endPoint.Easing != middlePoint.Easing || endPoint.Smooth != middlePoint.Smooth)
		{
			return false;
		}

		// Skip points that are identical
		// used for keyframe pause
		if (Math.Abs(endPoint.Time - middlePoint.Time) > DIFFERENCE_THRESHOLD && endPoint.Data.AreFloatsSimilar(middlePoint.Data, DIFFERENCE_THRESHOLD))
		{
			return false;
		}

		endSlope = SlopeOfPoint(startPoint, endPoint);
		middleSlope = SlopeOfPoint(startPoint, middlePoint);

		var middleYIntercepts = GetYIntercept(middlePoint, middleSlope);
		var endYIntercepts = GetYIntercept(endPoint, endSlope);



		// example point data
		// "_name":"colorWave","_points":[
		// [1,1,1,1,0],
		// [0,0,4,1,0.125],
		// [0,0.5,2,1,0.25],
		// [0,0,1,1,0.375],
		// [1,1,2,1,0.5],
		// [0,0,4,1,0.625],
		// [0,0.25,2,1,0.75],
		// [0.1,0.2,1,1,0.875],
		// [2,2,2,2,1]
		// ]}


		return
			// The points slope apply on the same Y intercept
			middleYIntercepts.AreFloatsSimilar(endYIntercepts, Y_INTERCEPT_DIFFERENCE_THRESHOLD) &&
			// Both points are identical
			middleSlope.AreFloatsSimilar(endSlope, DIFFERENCE_THRESHOLD);


	}

	private static float[] GetYIntercept(PointData pointData, IReadOnlyList<float> slopeArray)
	{
		var yIntercepts = new float[slopeArray.Count];
		for (var i = 0; i < slopeArray.Count; i++)
		{
			var slope = slopeArray[i];
			var x = pointData.Data[i];
			var y = pointData.Time;

			//y = mx + b
			// solve for y
			// b = y - mx
			var yIntercept = y - (slope * x);
			yIntercepts[i] = yIntercept;
		}

		return yIntercepts;
	}

	private static float[] SlopeOfPoint(PointData a, PointData b)
	{

		var yDiff = b.Time - a.Time;
		var slopes = new float[b.Data.Length];

		for (var i = 0; i < b.Data.Length; i++)
		{
			var xDiff = b.Data[i] - a.Data[i];
			if (xDiff == 0 || yDiff == 0)
			{
				slopes[i] = 0;
			}
			else
			{
				slopes[i] = yDiff / xDiff;
			}
		}

		return slopes;
	}
}