using System;
using System.Collections.Generic;
using OhHeck.Core.Helpers;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

public static class PointLintHelper
{
	public static void AnalyzePoints(object? value, IWarningOutput warningOutput, Action<Dictionary<string, List<PointData>>> analyzeFn)
	{
		if (value is List<BeatmapCustomEvent> customEvents)
		{
			foreach (var beatmapCustomEvent in customEvents)
			{
				if (beatmapCustomEvent is not AnimateEvent animateEvent)
				{
					continue;
				}

				var friendlyName = beatmapCustomEvent.GetFriendlyName();


				warningOutput.PushWarningInfo(new WarningInfo(friendlyName, warningOutput.GetCurrentWarningInfo().MemberLocation, warningOutput.GetCurrentWarningInfo().Parent));
				analyzeFn(PointHelper.GetPointDataDictionary(animateEvent.PointProperties)!);
				warningOutput.PopWarningInfo();
			}
		}

		var pointDataDictionary = PointHelper.GetPointDataDictionary(value);

		if (pointDataDictionary is null)
		{
			return;
		}

		analyzeFn(pointDataDictionary);
	}
}