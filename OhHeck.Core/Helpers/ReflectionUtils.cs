using System;
using System.Collections.Generic;
using System.Reflection;
using DryIoc;
using OhHeck.Core.Analyzer;

namespace OhHeck.Core.Helpers;

internal static class ReflectionUtils
{
	internal static List<MemberInfo> GetTypeFieldsSuperRecursive(IReflect type, List<MemberInfo>? memberInfos = null)
	{
		memberInfos ??= new List<MemberInfo>();

		while (true)
		{
			var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			var propInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			memberInfos.AddRange(fieldInfos);
			memberInfos.AddRange(propInfos);

			var parentType = type.UnderlyingSystemType.BaseType;

			// done
			if (parentType is null || !parentType.IsAssignableTo<IAnalyzable>())
			{
				return memberInfos;
			}

			type = parentType;
		}
	}

	// Parses any type including string as T
	public static T? GetAsT<T>(object value) => (T?) GetAsT(value, typeof(T));

	public static object GetAsT(object value, Type type)
	{
		var underlyingType = Nullable.GetUnderlyingType(type);
		// Can convert string to int/float/bool nullable
		if (underlyingType != null)
		{
			return Convert.ChangeType(value, underlyingType);
		}

		// Can convert string to int/float/bool
		return value is IConvertible ? Convert.ChangeType(value, type) : value;
	}
}