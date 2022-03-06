using System;
using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Helpers.Enumerable;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("similar-point-data-slope")]
public class SimilarPointDataSlope : IFieldAnalyzer
{

	// These numbers at quick glance seem to be fairly reliable, nice
	// however they should be configurable or looked at later
	private const float DIFFERENCE_THRESHOLD = 0.003f; // TODO: Configurable
	private const float Y_INTERCEPT_DIFFERENCE_THRESHOLD = 0.5f; // TODO: Configurable
	private const bool COMPARE_ALL_PREVIOUS_POINTS = true; // TODO: Configurable

	// Compares points a, b and c where b is between a and c.
	// If a-b's slope is similar to a-c, it deems it unnecessary
	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput) =>
		PointLintHelper.AnalyzePoints(value, outerWarningOutput, (pointDataDictionary, warningOutput) =>
		{

			foreach (var (s, pointDatas) in pointDataDictionary)
			{
				// we need at minimum 3 points to check
				if (pointDatas.Count <= 2)
				{
					continue;
				}

				// compare 3 points consecutively
				var prevPoint = pointDatas.First(); // if empty, we have a problem

				// Reduce allocations by reusing the same array
				var middleSlope = new float[prevPoint.Data.Length];
				var endSlope = new float[prevPoint.Data.Length];
				var middleYIntercepts = new float[prevPoint.Data.Length];
				var endYIntercepts = new float[prevPoint.Data.Length];

				for (var i = 1; i < pointDatas.Count - 1; i++)
				{
					var endPoint = pointDatas[i + 1];
					var middlePoint = pointDatas[i];


					if (ComparePoints(prevPoint, middlePoint, endPoint, middleSlope, endSlope, middleYIntercepts, endYIntercepts))
					{
						WriteWarning(warningOutput, s, prevPoint, middlePoint, middleSlope, endPoint, endSlope);
						continue;
					}


					var oldPrevPoint = prevPoint;
					prevPoint = middlePoint;

#pragma warning disable CS0162
					if (!COMPARE_ALL_PREVIOUS_POINTS)
					{
						continue;
					}

					// We don't need to recheck points when we're only checking the first 3
					if (i < 4)
					{
						continue;
					}

					// Check every point before end
					for (var j = 0; j < i - 2; j++)
					{
						// compare every point between start and end
						for (var k = j + 1; k < i - 1; k++)
						{
							var startPoint = pointDatas[k];
							var middlePoint2 = pointDatas[j];

							// Skip these points, we just checked them
							if (oldPrevPoint == startPoint && middlePoint2 == middlePoint || middlePoint2 == startPoint)
							{
								continue;
							}

							if (ComparePoints(startPoint, middlePoint2, endPoint, middleSlope, endSlope, middleYIntercepts, endYIntercepts))
							{
								WriteWarning(warningOutput, s, startPoint, middlePoint2, middleSlope, endPoint, endSlope);
							}
						}
					}
#pragma warning restore CS0162
				}
			}
		});

	private static void WriteWarning(IWarningOutput warningOutput, string s, PointData startPoint, PointData middlePoint, IEnumerable<float> middleSlope, PointData endPoint, IEnumerable<float> endSlope) =>
		warningOutput.WriteWarning($"Point data {s} slope and y intercept are closely intercepting and match easing/smooth {DIFFERENCE_THRESHOLD} slope ({middleSlope.ArrayToString()}): " +
		                           $"Point1 {startPoint.Data.ArrayToString()}:{startPoint.Time} " +
		                           $"Point2: {middlePoint.Data.ArrayToString()}:{middlePoint.Time} " +
		                           $"Point3: {endPoint.Data.ArrayToString()}:{endPoint.Time}", typeof(SimilarPointDataSlope));

	/// <summary>
	///
	/// </summary>
	/// <param name="startPoint"></param>
	/// <param name="middlePoint"></param>
	/// <param name="endPoint"></param>
	/// <param name="middleSlope"></param>
	/// <param name="endSlope"></param>
	/// <param name="middleYIntercepts"></param>
	/// <param name="endYIntercepts"></param>
	/// <returns>true if similar</returns>
	private static bool ComparePoints(PointData startPoint, PointData middlePoint, PointData endPoint, in float[] middleSlope, in float[] endSlope, in float[] middleYIntercepts, in float[] endYIntercepts)
	{
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

		SlopeOfPoint(startPoint, endPoint, endSlope);
		SlopeOfPoint(startPoint, middlePoint, middleSlope);

		GetYIntercept(middlePoint, middleSlope, middleYIntercepts);
		GetYIntercept(endPoint, endSlope, endYIntercepts);



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

	private static void GetYIntercept(PointData pointData, IReadOnlyList<float> slopeArray, IList<float> yIntercepts)
	{
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
	}

	private static void SlopeOfPoint(PointData a, PointData b, IList<float> slopes)
	{
		var yDiff = b.Time - a.Time;

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
	}
}