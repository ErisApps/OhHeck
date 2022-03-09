using System;
using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Helpers.Enumerable;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("duplicate-point-data")]
public class DuplicatePointData : IFieldAnalyzer {

	// Checks if points are duplicated
	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput) =>
		PointLintHelper.AnalyzePoints(this, value, outerWarningOutput, static (pointDataDictionary, self, warningOutput) =>
		{
			foreach (var (s, pointDatas) in pointDataDictionary)
			{
				// If there's only one point data, don't bother
				if (pointDatas.Count <= 1)
				{
					continue;
				}

				var prevPoint = pointDatas.First(); // if empty, we have a problem
				for (var i = 1; i < pointDatas.Count; i++)
				{
					var point = pointDatas[i];

					// we're comparing 3 points
					// left, middle and right
					PointData? nextPoint = null;
					if (i + 1 < pointDatas.Count)
					{
						nextPoint = pointDatas[i + 1];
					}

					// Both points are identical
					// or left, middle and right are identical
					if (prevPoint.Data.AreArrayElementsIdentical(point.Data) && (nextPoint is null || nextPoint.Data.AreArrayElementsIdentical(point.Data)))
					{
						var args = new List<object> { s, prevPoint.Data, prevPoint.Time, point.Data, point.Time };
						const string message = "Point data {S} are identical: Point1 {P1}:{P1T} " +
						              "Point2: {P2}:{P2T}";
						const string nextPointMsg = message + " Point3: {P3}:{P3T}";

						if (nextPoint is not null)
						{
							args.Add(nextPoint.Data);
							args.Add(nextPoint.Time);
							warningOutput.WriteWarning(nextPointMsg, GetType(), args.ToArray());
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