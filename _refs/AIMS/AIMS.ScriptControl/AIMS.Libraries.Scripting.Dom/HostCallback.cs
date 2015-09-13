using AIMS.Libraries.Scripting.Dom.Refactoring;
using System;
using System.Runtime.CompilerServices;

namespace AIMS.Libraries.Scripting.Dom
{
	public static class HostCallback
	{
		public static Action<string, Exception> ShowError = delegate(string message, Exception ex)
		{
			LoggingService.Error(message, ex);
		};

		public static Action<string> ShowMessage = delegate(string message)
		{
			LoggingService.Info(message);
		};

		public static Func<string, ParseInformation> GetParseInformation = delegate
		{
			throw new NotImplementedException("GetParseInformation was not implemented by the host.");
		};

		public static Func<IProjectContent> GetCurrentProjectContent = delegate
		{
			throw new NotImplementedException("GetCurrentProjectContent was not implemented by the host.");
		};

		public static Func<IMember, string, bool> RenameMember = (IMember member, string text) => false;

		public static Action<string> BeginAssemblyLoad = delegate
		{
		};

		public static Action FinishAssemblyLoad = delegate
		{
		};

		public static Action<string, string, string> ShowAssemblyLoadError = delegate
		{
		};

		public static Action<CodeGenerator> InitializeCodeGeneratorOptions = delegate
		{
		};

		[CompilerGenerated]
		private static Action<string, Exception> <>9__CachedAnonymousMethodDelegate9;

		[CompilerGenerated]
		private static Action<string> <>9__CachedAnonymousMethodDelegatea;

		[CompilerGenerated]
		private static Func<string, ParseInformation> <>9__CachedAnonymousMethodDelegateb;

		[CompilerGenerated]
		private static Func<IProjectContent> <>9__CachedAnonymousMethodDelegatec;

		[CompilerGenerated]
		private static Func<IMember, string, bool> <>9__CachedAnonymousMethodDelegated;

		[CompilerGenerated]
		private static Action<string> <>9__CachedAnonymousMethodDelegatee;

		[CompilerGenerated]
		private static Action <>9__CachedAnonymousMethodDelegatef;

		[CompilerGenerated]
		private static Action<string, string, string> <>9__CachedAnonymousMethodDelegate10;

		[CompilerGenerated]
		private static Action<CodeGenerator> <>9__CachedAnonymousMethodDelegate11;

		internal static void ShowAssemblyLoadErrorInternal(string fileName, string include, string message)
		{
			LoggingService.Warn(string.Concat(new string[]
			{
				"Error loading code-completion information for ",
				include,
				" from ",
				fileName,
				":\r\n",
				message,
				"\r\n"
			}));
			HostCallback.ShowAssemblyLoadError(fileName, include, message);
		}
	}
}
