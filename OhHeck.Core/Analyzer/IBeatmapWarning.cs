using System;
using System.Reflection;

namespace OhHeck.Core.Analyzer;

public interface IBeatmapWarning
{
	/// <summary>
	/// Checks if the field is invalid according to the implementation
	/// </summary>
	/// <param name="fieldType"></param>
	/// <param name="value"></param>
	/// <returns>The warning message or null if valid</returns>
	public string? Validate(Type fieldType, object? value);
}