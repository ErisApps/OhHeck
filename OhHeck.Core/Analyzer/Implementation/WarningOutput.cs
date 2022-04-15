using System;
using System.Collections.Generic;

namespace OhHeck.Core.Analyzer.Implementation;

public class WarningOutput : IWarningOutput
{
	private readonly Stack<WarningContext> _contexts = new();
	private readonly List<Warning> _warningOutput = new();


	public WarningContext GetCurrentWarningInfo() => _contexts.Peek();

	public void PushWarningInfo(WarningContext warningContext) => _contexts.Push(warningContext);

	public void PopWarningInfo() => _contexts.Pop();

	public void WriteWarning(string message, Type analyzerType, params object?[]? formatArgs) => _warningOutput.Add(new Warning(message, _contexts.Peek(), analyzerType, formatArgs?.Length is null or 0 ? null : formatArgs));

	public IEnumerable<Warning> GetWarnings() => _warningOutput;
}