using System;
using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("excessive-parenting")]
public class TooManyParentingTracks : IFieldAnalyzer
{
	// grrr 2000 objects

	[WarningConfigProperty("track_parenting_maximum")]
	private int _trackParentingMaximum = 20;

	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput)
	{
		if (value is not List<BeatmapCustomEvent> events)
		{
			return;
		}

		var parentTrackCount = events.Count(e => e.Type == EventTypes.ASSIGN_TRACK_PARENT);

		if (parentTrackCount > _trackParentingMaximum)
		{
			outerWarningOutput.WriteWarning("Excessive amounts of parent track events. Parenting is inefficient, consider using less. Found {ParentTrackCount} > {TrackParentingExcess}", GetType(),
				parentTrackCount.ToString(), _trackParentingMaximum.ToString());
		}
	}
}