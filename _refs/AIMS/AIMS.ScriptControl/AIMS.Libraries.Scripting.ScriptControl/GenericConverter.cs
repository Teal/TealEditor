using System;
using System.ComponentModel;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	internal static class GenericConverter
	{
		public static T FromString<T>(string v, T defaultValue)
		{
			T result;
			if (string.IsNullOrEmpty(v))
			{
				result = defaultValue;
			}
			else if (typeof(T) == typeof(string))
			{
				result = (T)((object)v);
			}
			else
			{
				try
				{
					TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
					result = (T)((object)c.ConvertFromInvariantString(v));
				}
				catch
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public static string ToString<T>(T val)
		{
			string result;
			if (typeof(T) == typeof(string))
			{
				string s = (string)((object)val);
				result = (string.IsNullOrEmpty(s) ? null : s);
			}
			else
			{
				try
				{
					TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
					string s = c.ConvertToInvariantString(val);
					result = (string.IsNullOrEmpty(s) ? null : s);
				}
				catch
				{
					result = null;
				}
			}
			return result;
		}
	}
}
