using System;
using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Helpers.Enumerable;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("excessive-parenting")]
public class TooManyParentingTracks : IBeatmapAnalyzer
{
	// grrr 2000 objects

	// TODO: Inject?
	private const int TRACK_PARENTING_EXCESS = 20;

	public void Validate(Type fieldType, object? value, IWarningOutput warningOutput)
	{
		if (value is not List<BeatmapCustomEvent> events)
		{
			return;
		}

		var parentTrackCount = events.Count(e => e.Type == EventTypes.ASSIGN_TRACK_PARENT);

		if (parentTrackCount > TRACK_PARENTING_EXCESS)
		{
			warningOutput.WriteWarning($"Excessive amounts of parent track events. Parenting is inefficient, consider using less. Found {parentTrackCount} > {TRACK_PARENTING_EXCESS}");
		}
	}
}