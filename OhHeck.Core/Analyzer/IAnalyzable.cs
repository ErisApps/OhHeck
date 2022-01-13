namespace OhHeck.Core.Analyzer;

// TODO: Make attribute? We need the string through the type anyways
public interface IAnalyzable
{
	string GetFriendlyName();

	string? ExtraData() => null;
}