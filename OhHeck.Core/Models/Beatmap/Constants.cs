namespace OhHeck.Core.Models.Beatmap;

public static class EventTypes
{
	// Tracks
	public const string ANIMATE_TRACK = "AnimateTrack";
	public const string ASSIGN_PATH_ANIMATION = "AssignPathAnimation";

	// Noozle Extensions
	public const string ASSIGN_TRACK_PARENT = "AssignTrackParent";
	public const string ASSIGN_PLAYER_TO_TRACK = "AssignPlayerToTrack";

	// Chroma
	public const string ASSIGN_FOR_TRACK = "AssignFogTrack";
}

public static class AnimationProperties
{
	// Noozle
	public const string CUTTABLE = "_interactable";
	public const string DEFINITE_POSITION = "_definitePosition";
	public const string DISSOLVE = "_dissolve";
	public const string DISSOLVE_ARROW = "_dissolveArrow";
	public const string LOCAL_ROTATION = "_localRotation";
	public const string POSITION = "_position";
	public const string ROTATION = "_rotation";
	public const string SCALE = "_scale";
	public const string TIME = "_time";

	// Chroma
	public const string COLOR = "_color";
}

public static class JsonKeys
{
	public const string CUSTOM_DATA_KEY_V2 = "_customData";
	public const string CUSTOM_DATA_KEY_V3 = "customData";
}