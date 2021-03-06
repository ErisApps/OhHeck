using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OhHeck.Core.Models.ModData.Tracks;

public class AnimateTrackEvent : AnimateEvent
{
	[JsonConstructor]
	public AnimateTrackEvent(float time, string type, Dictionary<string, object> data, string track, Functions? easing, float? duration) : base(time, type, data, track, easing, duration)
	{
	}

	public override string GetFriendlyName() => nameof(AnimateTrackEvent);
}