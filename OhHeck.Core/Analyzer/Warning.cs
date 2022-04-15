using System;

namespace OhHeck.Core.Analyzer;

public record WarningContext(string MemberName, IAnalyzable DeclaringInstance, WarningContext? Parent);
public record Warning(string Message, WarningContext WarningContext, Type AnalyzerType);