using System;
using System.ComponentModel.Composition;

namespace OhHeck.Core.Analyzer.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class WarningConfigPropertyAttribute : ImportAttribute
{
	public WarningConfigPropertyAttribute(string contractName) : base(contractName) {}

	public WarningConfigPropertyAttribute(string contractName, Type contractType) : base(contractName, contractType) {}
}