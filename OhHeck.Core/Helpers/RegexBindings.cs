using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace OhHeck.Core.Helpers;

public class RegexBindings
{

	public static bool IsRegexValid(string regexStr, [NotNullWhen(false)] out string? message)
	{
		try
		{
			Regex unused = new(regexStr, RegexOptions.CultureInvariant | RegexOptions.ECMAScript);
		}
		catch (Exception e)
		{
			message = e.Message;
			return false;
		}

		message = null;
		return true;
	}

}