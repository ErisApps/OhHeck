﻿using System;
using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Helpers.Enumerable;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("similar-point-data")]
public class SimilarPointData : IFieldAnalyzer {

	// I've got no idea if this is a reliable algorithm
	// Compares two points and attempts to make a rough estimate of whether they're similar/redundant
	// based on their keyframe difference and time difference
	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput) =>
		PointLintHelper.AnalyzePoints(value,  outerWarningOutput, (pointDataDictionary, warningOutput) =>
		{

			foreach (var (s, pointDatas) in pointDataDictionary)
			{
				// redundant point checking
				if (pointDatas.Count <= 1)
				{
					continue;
				}

				var prevPoint = pointDatas.First(); // if empty, we have a problem
				for (var i = 1; i < pointDatas.Count; i++)
				{
					var point = pointDatas[i];
					var nextPoint = i + 1 < pointDatas.Count ? pointDatas[i + 1] : null;

					// ignore points who have different easing or smoothness since those can
					// be considered not similar even with small time differences
					if (point.Smooth != prevPoint.Smooth || point.Easing != prevPoint.Easing || (nextPoint is not null && (nextPoint.Smooth != point.Smooth || nextPoint.Easing != point.Easing)))
					{
						continue;
					}

					var leftMiddleTimeDifference = MathF.Abs(point.Time - prevPoint.Time);
					var middleRightTimeDifference = nextPoint is not null ? MathF.Abs(point.Time - nextPoint.Time) : (float?) null;

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

					// Both points are identical
					if (prevPoint.Data.AreFloatsSimilar(point.Data, leftMiddleTimeDifference)
					    && (middleRightTimeDifference is null || point.Data.AreFloatsSimilar(nextPoint!.Data, middleRightTimeDifference.Value)))
					{
						var args = new List<object>
						{
							s,
							leftMiddleTimeDifference,
							prevPoint.Data,
							prevPoint.Time,
							point.Data,
							point.Time
						};
						const string message = "Point data {s} are too similar relative to the time difference {leftMiddleTimeDifference}: Point1 {prevPointData}:{prevPointTime} " +
						              "Point2: {P2}:{P2t}";
						const string message2 = message + " 2nd time difference: {middleRightTimeDifference} Point3: {nextPoint.Data.ArrayToString()}:{nextPoint.Time}";

						if (nextPoint is not null)
						{
							args.Add(middleRightTimeDifference!);
							args.Add(nextPoint.Data);
							args.Add(nextPoint.Time);
							warningOutput.WriteWarning(message2, GetType(), args.ToArray());
						}
						else
						{
							warningOutput.WriteWarning(message, GetType(), args.ToArray());
						}


					}

					prevPoint = point;
				}
			}
		});
}