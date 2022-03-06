using System;

namespace OhHeck.Core.Analyzer;

public interface IFieldAnalyzer
{
	/// <summary>
	/// Checks if the field is invalid according to the implementation
	/// </summary>
	/// <param name="fieldType"></param>
	/// <param name="value"></param>
	/// <param name="outerWarningOutput"></param>
	/// <returns>The warning message or null if valid</returns>
	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput);
}