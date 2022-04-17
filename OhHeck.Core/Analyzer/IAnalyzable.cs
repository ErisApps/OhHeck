namespace OhHeck.Core.Analyzer;

// TODO: Make attribute? We need the string through the type anyways
/// <summary>
/// Represents a type with fields that can be analyzed by IFieldAnalyzer
/// </summary>
public interface IAnalyzable
{
	string GetFriendlyName();

	string? ExtraData() => null;

	IAnalyzable? Redirect() => null;
}