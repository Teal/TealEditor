using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.ScriptControl.Project;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.Parser
{
	internal class CSharpParser : IParser
	{
		private string[] lexerTags;

		private Errors lastErrors = null;

		public string[] LexerTags
		{
			get
			{
				return this.lexerTags;
			}
			set
			{
				this.lexerTags = value;
			}
		}

		public LanguageProperties Language
		{
			get
			{
				return LanguageProperties.CSharp;
			}
		}

		public Errors LastErrors
		{
			get
			{
				return this.lastErrors;
			}
		}

		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return new CSharpExpressionFinder(fileName);
		}

		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".CS", StringComparison.OrdinalIgnoreCase);
		}

		public bool CanParse(IProject project)
		{
			return true;
		}

		private void RetrieveRegions(ICompilationUnit cu, SpecialTracker tracker)
		{
			for (int i = 0; i < tracker.CurrentSpecials.Count; i++)
			{
				PreprocessingDirective directive = tracker.CurrentSpecials[i] as PreprocessingDirective;
				if (directive != null)
				{
					if (directive.Cmd == "#region")
					{
						int deep = 1;
						for (int j = i + 1; j < tracker.CurrentSpecials.Count; j++)
						{
							PreprocessingDirective nextDirective = tracker.CurrentSpecials[j] as PreprocessingDirective;
							if (nextDirective != null)
							{
								string cmd = nextDirective.Cmd;
								if (cmd != null)
								{
									if (!(cmd == "#region"))
									{
										if (cmd == "#endregion")
										{
											deep--;
											if (deep == 0)
											{
												cu.FoldingRegions.Add(new FoldingRegion(directive.Arg.Trim(), new DomRegion(directive.StartPosition, nextDirective.EndPosition)));
												break;
											}
										}
									}
									else
									{
										deep++;
									}
								}
							}
						}
					}
				}
			}
		}

		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
		{
			ICompilationUnit result;
			using (AIMS.Libraries.Scripting.NRefactory.IParser p = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(fileContent)))
			{
				result = this.Parse(p, fileName, projectContent);
			}
			return result;
		}

		private ICompilationUnit Parse(AIMS.Libraries.Scripting.NRefactory.IParser p, string fileName, IProjectContent projectContent)
		{
			p.Lexer.SpecialCommentTags = this.lexerTags;
			p.ParseMethodBodies = true;
			p.Parse();
			this.lastErrors = p.Errors;
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(projectContent);
			visitor.Specials = p.Lexer.SpecialTracker.CurrentSpecials;
			visitor.VisitCompilationUnit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = (p.Errors.Count > 0);
			this.RetrieveRegions(visitor.Cu, p.Lexer.SpecialTracker);
			this.AddCommentTags(visitor.Cu, p.Lexer.TagComments);
			return visitor.Cu;
		}

		private void AddCommentTags(ICompilationUnit cu, List<AIMS.Libraries.Scripting.NRefactory.Parser.TagComment> tagComments)
		{
			foreach (AIMS.Libraries.Scripting.NRefactory.Parser.TagComment tagComment in tagComments)
			{
				DomRegion tagRegion = new DomRegion(tagComment.StartPosition.Y, tagComment.StartPosition.X);
				AIMS.Libraries.Scripting.Dom.TagComment tag = new AIMS.Libraries.Scripting.Dom.TagComment(tagComment.Tag, tagRegion);
				tag.CommentString = tagComment.CommentText;
				cu.TagComments.Add(tag);
			}
		}

		public IResolver CreateResolver()
		{
			return new NRefactoryResolver(ProjectParser.CurrentProjectContent, LanguageProperties.CSharp);
		}
	}
}
