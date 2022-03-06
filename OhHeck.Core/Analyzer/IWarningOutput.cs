using System;
using System.Collections.Generic;
using Serilog.Core;

namespace OhHeck.Core.Analyzer;

public interface IWarningOutput
{
	WarningContext GetCurrentWarningInfo();

	void PushWarningInfo(WarningContext warningContext);
	void PopWarningInfo();

	[MessageTemplateFormatMethod("message")]
	void WriteWarning(string message, Type analyzeType, params object?[]? formatArgs);

	IEnumerable<Warning> GetWarnings();
}