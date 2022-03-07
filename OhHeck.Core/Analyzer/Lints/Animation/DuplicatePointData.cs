using System;
using System.Linq;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Helpers.Enumerable;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("duplicate-point-data")]
public class DuplicatePointData : IFieldAnalyzer {

	// Checks if points are duplicated
	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput) =>
		PointLintHelper.AnalyzePoints(value, outerWarningOutput, (pointDataDictionary, warningOutput) =>
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
						var message = $"Point data {s} are identical: Point1 {prevPoint.Data.ArrayToString()}:{prevPoint.Time} " +
						              $"Point2: {point.Data.ArrayToString()}:{point.Time}";

						if (nextPoint is not null)
						{
							message += $" Point3: {nextPoint.Data.ArrayToString()}:{nextPoint.Time}";
						}

						warningOutput.WriteWarning(message, GetType());
					}



					prevPoint = point;
				}
			}
		});
}