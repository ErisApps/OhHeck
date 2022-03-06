using System;

namespace OhHeck.Core.Analyzer;

public record WarningContext(string Type, string MemberLocation, IAnalyzable? Parent);
public record Warning(string Message, WarningContext WarningContext, Type AnalyzerType, object?[]? FormatParams);