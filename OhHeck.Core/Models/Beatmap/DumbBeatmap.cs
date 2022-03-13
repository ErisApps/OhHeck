using System.Text.Json.Serialization;

namespace OhHeck.Core.Models.Beatmap;

public class DumbBeatmap
{
	[JsonPropertyName("_version")]
	public string? Version1 { get; }

	[JsonPropertyName("version")]
	public string? Version2 { get; }

	public DumbBeatmap(string? version2, string? version1)
	{
		Version1 = version1;
		Version2 = version2;
	}
}