using System;

namespace AIMS.Libraries.Scripting.Dom
{
	internal static class LoggingService
	{
		public static bool IsDebugEnabled
		{
			get
			{
				return true;
			}
		}

		public static void Debug(object message)
		{
		}

		public static void Info(object message)
		{
		}

		public static void Warn(object message)
		{
		}

		public static void Warn(object message, Exception exception)
		{
		}

		public static void Error(object message)
		{
		}

		public static void Error(object message, Exception exception)
		{
		}
	}
}
