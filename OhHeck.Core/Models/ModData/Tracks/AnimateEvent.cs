﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Models.ModData.Tracks;

public abstract class AnimateEvent : BeatmapCustomEvent
{
	protected AnimateEvent(float time, string type, Dictionary<string, object> data, string track, Functions? easing, float? duration) : base(time, type, data)
	{
		Track = track;
		Easing = easing;
		Duration = duration;
	}


	[JsonPropertyName("_track")]
	public string Track { get; }

	[JsonPropertyName("_easing")]
	public Functions? Easing { get; }

	[JsonPropertyName("_duration")]
	public float? Duration { get; }

	[JsonIgnore]
	public Dictionary<string, PointDefinitionData> PointProperties = new();

	public abstract override string GetFriendlyName();
}