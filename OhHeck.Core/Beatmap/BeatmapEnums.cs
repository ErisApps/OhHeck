namespace OhHeck.Core.Beatmap;


public enum BeatmapObjectType
{
	Note,
	LongNote,
	Obstacle
}

public enum NoteType
{
	NoteA,
	NoteB,
	GhostNote,
	Bomb,
	None = -1
}

public enum ColorType
{
	ColorA,
	ColorB,
	None = -1
}

public enum BeatmapEventType
{
	Event0,
	Event1,
	Event2,
	Event3,
	Event4,
	Event5,
	Event6,
	Event7,
	Event8,
	Event9,
	Event10,
	Event11,
	Event12,
	Event13,
	Event14,
	Event15,
	Event16,
	Event17,
	VoidEvent = -1,
	Special0 = 40,
	Special1,
	Special2,
	Special3,
	BpmChange = 100
}

public enum NoteCutDirection
{
	Up,
	Down,
	Left,
	Right,
	UpLeft,
	UpRight,
	DownLeft,
	DownRight,
	Any,
	None
}

public enum ObstacleType
{
	FullHeight,
	Top
}
