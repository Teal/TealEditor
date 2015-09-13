using AIMS.Libraries.Scripting.CodeCompletion;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.Dom.VBNet;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.ScriptControl.Converter;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.Parser
{
	internal class ProjectParser
	{
		private static IProjectContent projectContent = null;

		private static ProjectContentRegistry projectContentRegistry = null;

		private static Dictionary<string, ProjectContentItem> projContentInfo = null;

		private static string domPersistencePath;

		private static string projectPath;

		private static SupportedLanguage language;

		private static Errors lastParserError = null;

		public static string DomPersistencePath
		{
			get
			{
				return ProjectParser.domPersistencePath;
			}
		}

		public static string ProjectPath
		{
			get
			{
				return ProjectParser.projectPath;
			}
		}

		public static IProjectContent CurrentProjectContent
		{
			get
			{
				return ProjectParser.projectContent;
			}
		}

		public static ProjectContentRegistry ProjectContentRegistry
		{
			get
			{
				return ProjectParser.projectContentRegistry;
			}
		}

		public static SupportedLanguage Language
		{
			get
			{
				return ProjectParser.language;
			}
			set
			{
				ProjectParser.ConvertToLanguage(ProjectParser.language, value);
				ProjectParser.language = value;
			}
		}

		public static Dictionary<string, ProjectContentItem> ProjectFiles
		{
			get
			{
				return ProjectParser.projContentInfo;
			}
		}

		public static Errors LastParserErrors
		{
			get
			{
				return ProjectParser.lastParserError;
			}
		}

		public static AmbienceReflectionDecorator CurrentAmbience
		{
			get
			{
				IAmbience defAmbience;
				if (ProjectParser.language == SupportedLanguage.CSharp)
				{
					defAmbience = new CSharpAmbience();
				}
				else
				{
					defAmbience = new VBNetAmbience();
				}
				return new AmbienceReflectionDecorator(defAmbience);
			}
		}

		public static void Initilize(SupportedLanguage lang)
		{
			ProjectParser.language = lang;
			ProjectParser.projContentInfo = new Dictionary<string, ProjectContentItem>();
			ProjectParser.projectPath = AppDomain.CurrentDomain.BaseDirectory;
			ProjectParser.projectContentRegistry = new ProjectContentRegistry();
			ProjectParser.domPersistencePath = Path.Combine(Path.GetTempPath(), "AIMSDomCache");
			Directory.CreateDirectory(ProjectParser.domPersistencePath);
			ProjectParser.projectContentRegistry.ActivatePersistence(ProjectParser.domPersistencePath);
			ProjectParser.projectContent = new DefaultProjectContent();
			ProjectParser.projectContent.ReferencedContents.Add(ProjectParser.projectContentRegistry.Mscorlib);
		}

		private static void ConvertToLanguage(SupportedLanguage OldLang, SupportedLanguage NewLang)
		{
			Dictionary<string, ProjectContentItem> projInfo = new Dictionary<string, ProjectContentItem>();
			foreach (ProjectContentItem pc in ProjectParser.projContentInfo.Values)
			{
				string fileName = pc.FileName;
				ProjectParser.ClearParseInformation(fileName);
				pc.FileName = Path.GetFileNameWithoutExtension(fileName) + ((NewLang == SupportedLanguage.CSharp) ? ".cs" : ".vb");
				pc.Contents = CodeConverter.ConvertCode(pc.Contents, (OldLang == SupportedLanguage.CSharp) ? ScriptLanguage.CSharp : ScriptLanguage.VBNET, (NewLang == SupportedLanguage.CSharp) ? ScriptLanguage.CSharp : ScriptLanguage.VBNET);
				projInfo.Add(pc.FileName, pc);
			}
			ProjectParser.language = NewLang;
			ProjectParser.projContentInfo = projInfo;
			foreach (ProjectContentItem pc in ProjectParser.projContentInfo.Values)
			{
				ProjectParser.ParseProjectContents(pc.FileName, pc.Contents, pc.IsOpened);
			}
		}

		public static IParser GetParser(string fileName)
		{
			IParser result;
			if (Path.GetExtension(fileName).ToLower().Trim() == ".cs")
			{
				result = new CSharpParser();
			}
			else
			{
				result = new VbParser();
			}
			return result;
		}

		public static IResolver CreateResolver(string fileName)
		{
			IParser parser = ProjectParser.GetParser(fileName);
			IResolver result;
			if (parser != null)
			{
				result = parser.CreateResolver();
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static ResolveResult Resolve(ExpressionResult expressionResult, int caretLineNumber, int caretColumn, string fileName, string fileContent)
		{
			IResolver resolver = ProjectParser.CreateResolver(fileName);
			ResolveResult result;
			if (resolver != null)
			{
				result = resolver.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, fileContent);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static string GetFileContents(string fileName)
		{
			string result;
			if (ProjectParser.projContentInfo.ContainsKey(fileName))
			{
				result = ProjectParser.projContentInfo[fileName].Contents;
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		public static NRefactoryResolver GetResolver()
		{
			return new NRefactoryResolver(ProjectParser.projectContent, (ProjectParser.language == SupportedLanguage.CSharp) ? LanguageProperties.CSharp : LanguageProperties.VBNet);
		}

		public static void RemoveContentFile(string fileName)
		{
			if (ProjectParser.projContentInfo.ContainsKey(fileName))
			{
				ProjectParser.ClearParseInformation(fileName);
				ProjectParser.projContentInfo.Remove(fileName);
			}
		}

		public static ParseInformation ParseProjectContents(string fileName, string Content)
		{
			return ProjectParser.ParseProjectContents(fileName, Content, false);
		}

		public static ParseInformation ParseProjectContents(string fileName, string Content, bool IsOpened)
		{
			if (!ProjectParser.projContentInfo.ContainsKey(fileName))
			{
				ProjectParser.projContentInfo[fileName] = new ProjectContentItem(fileName, Content, IsOpened);
			}
			ProjectParser.projContentInfo[fileName].Contents = Content;
			IParser parser = ProjectParser.GetParser(fileName);
			ParseInformation result;
			if (parser == null)
			{
				result = null;
			}
			else
			{
				ICompilationUnit parserOutput = parser.Parse(ProjectParser.projectContent, fileName, Content);
				ProjectParser.lastParserError = parser.LastErrors;
				if (ProjectParser.projContentInfo.ContainsKey(fileName))
				{
					ParseInformation parseInformation = ProjectParser.projContentInfo[fileName].ParsedContents;
					if (parseInformation == null)
					{
						parseInformation = new ParseInformation();
						ProjectParser.projContentInfo[fileName].ParsedContents = parseInformation;
					}
					ProjectParser.projectContent.UpdateCompilationUnit(parseInformation.MostRecentCompilationUnit, parserOutput, fileName);
				}
				else
				{
					ProjectParser.projectContent.UpdateCompilationUnit(null, parserOutput, fileName);
				}
				result = ProjectParser.UpdateParseInformation(parserOutput, fileName);
			}
			return result;
		}

		public static ParseInformation GetParseInformation(string fileName)
		{
			ParseInformation result;
			if (fileName == null || fileName.Length == 0)
			{
				result = null;
			}
			else if (!ProjectParser.projContentInfo.ContainsKey(fileName))
			{
				result = ProjectParser.ParseProjectContents(fileName, ProjectParser.projContentInfo[fileName].Contents);
			}
			else
			{
				result = ProjectParser.projContentInfo[fileName].ParsedContents;
			}
			return result;
		}

		public static void ClearParseInformation(string fileName)
		{
			if (fileName != null && fileName.Length != 0)
			{
				if (ProjectParser.projContentInfo.ContainsKey(fileName))
				{
					ParseInformation parseInfo = ProjectParser.projContentInfo[fileName].ParsedContents;
					if (parseInfo != null && parseInfo.MostRecentCompilationUnit != null)
					{
						parseInfo.MostRecentCompilationUnit.ProjectContent.RemoveCompilationUnit(parseInfo.MostRecentCompilationUnit);
					}
					ProjectParser.projContentInfo[fileName].ParsedContents = null;
				}
			}
		}

		public static ParseInformation UpdateParseInformation(ICompilationUnit parserOutput, string fileName)
		{
			ParseInformation parseInformation = ProjectParser.projContentInfo[fileName].ParsedContents;
			if (parserOutput.ErrorsDuringCompile)
			{
				parseInformation.DirtyCompilationUnit = parserOutput;
			}
			else
			{
				parseInformation.ValidCompilationUnit = parserOutput;
				parseInformation.DirtyCompilationUnit = null;
			}
			ProjectParser.projContentInfo[fileName].ParsedContents = parseInformation;
			return parseInformation;
		}
	}
}
