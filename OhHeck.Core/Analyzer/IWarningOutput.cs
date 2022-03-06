using System.Collections.Generic;

namespace OhHeck.Core.Analyzer;

public interface IWarningOutput
{
	WarningContext GetCurrentWarningInfo();

	void PushWarningInfo(WarningContext warningContext);
	void PopWarningInfo();

	void WriteWarning(string message);

	IEnumerable<Warning> GetWarnings();
}