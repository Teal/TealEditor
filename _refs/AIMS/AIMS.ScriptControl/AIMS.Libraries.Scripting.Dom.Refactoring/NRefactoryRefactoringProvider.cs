using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.NRefactory.PrettyPrinter;
using AIMS.Libraries.Scripting.NRefactory.Visitors;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public class NRefactoryRefactoringProvider : RefactoringProvider
	{
		protected class PossibleTypeReference
		{
			public string Name;

			public bool Global;

			public int TypeParameterCount;

			public PossibleTypeReference(string name)
			{
				this.Name = name;
			}

			public PossibleTypeReference(TypeReference tr)
			{
				this.Name = tr.SystemType;
				this.Global = tr.IsGlobal;
				this.TypeParameterCount = tr.GenericTypes.Count;
			}

			public override int GetHashCode()
			{
				return this.Name.GetHashCode() ^ this.Global.GetHashCode() ^ this.TypeParameterCount.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				bool result;
				if (!(obj is NRefactoryRefactoringProvider.PossibleTypeReference))
				{
					result = false;
				}
				else if (this == obj)
				{
					result = true;
				}
				else
				{
					NRefactoryRefactoringProvider.PossibleTypeReference myPossibleTypeReference = (NRefactoryRefactoringProvider.PossibleTypeReference)obj;
					result = (this.Name == myPossibleTypeReference.Name && this.Global == myPossibleTypeReference.Global && this.TypeParameterCount == myPossibleTypeReference.TypeParameterCount);
				}
				return result;
			}
		}

		private class FindPossibleTypeReferencesVisitor : AbstractAstVisitor
		{
			internal Dictionary<NRefactoryRefactoringProvider.PossibleTypeReference, object> list = new Dictionary<NRefactoryRefactoringProvider.PossibleTypeReference, object>();

			public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
			{
				this.list[new NRefactoryRefactoringProvider.PossibleTypeReference(identifierExpression.Identifier)] = data;
				return base.VisitIdentifierExpression(identifierExpression, data);
			}

			public override object VisitTypeReference(TypeReference typeReference, object data)
			{
				this.list[new NRefactoryRefactoringProvider.PossibleTypeReference(typeReference)] = data;
				return base.VisitTypeReference(typeReference, data);
			}

			public override object VisitAttribute(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
			{
				this.list[new NRefactoryRefactoringProvider.PossibleTypeReference(attribute.Name)] = data;
				this.list[new NRefactoryRefactoringProvider.PossibleTypeReference(attribute.Name + "Attribute")] = data;
				return base.VisitAttribute(attribute, data);
			}
		}

		private class RemoveTypesVisitor : AbstractAstTransformer
		{
			internal const string DummyIdentifier = "DummyNamespace!InsertionPos";

			internal int includeCommentsUpToLine;

			internal int includeCommentsAfterLine = 2147483647;

			internal bool firstType = true;

			public override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
			{
				if (this.firstType)
				{
					this.includeCommentsUpToLine = usingDeclaration.EndLocation.Y;
				}
				return null;
			}

			public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
			{
				this.includeCommentsAfterLine = namespaceDeclaration.EndLocation.Y;
				object result;
				if (this.firstType)
				{
					this.includeCommentsUpToLine = namespaceDeclaration.StartLocation.Y;
					result = base.VisitNamespaceDeclaration(namespaceDeclaration, data);
				}
				else
				{
					base.RemoveCurrentNode();
					result = null;
				}
				return result;
			}

			public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
			{
				if (typeDeclaration.EndLocation.Y > this.includeCommentsAfterLine)
				{
					this.includeCommentsAfterLine = typeDeclaration.EndLocation.Y;
				}
				if (this.firstType)
				{
					this.firstType = false;
					base.ReplaceCurrentNode(new UsingDeclaration("DummyNamespace!InsertionPos"));
				}
				else
				{
					base.RemoveCurrentNode();
				}
				return null;
			}
		}

		public static readonly NRefactoryRefactoringProvider NRefactoryCSharpProviderInstance = new NRefactoryRefactoringProvider(SupportedLanguage.CSharp);

		public static readonly NRefactoryRefactoringProvider NRefactoryVBNetProviderInstance = new NRefactoryRefactoringProvider(SupportedLanguage.VBNet);

		private SupportedLanguage language;

		public override bool SupportsFindUnusedUsingDeclarations
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsCreateNewFileLikeExisting
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsGetFullCodeRangeForType
		{
			get
			{
				return true;
			}
		}

		private NRefactoryRefactoringProvider(SupportedLanguage language)
		{
			this.language = language;
		}

		public override bool IsEnabledForFile(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			bool result;
			if (extension.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
			{
				result = (this.language == SupportedLanguage.CSharp);
			}
			else
			{
				result = (extension.Equals(".vb", StringComparison.InvariantCultureIgnoreCase) && this.language == SupportedLanguage.VBNet);
			}
			return result;
		}

		private static void ShowSourceCodeErrors(string errors)
		{
			HostCallback.ShowMessage("${res:SharpDevelop.Refactoring.CannotPerformOperationBecauseOfSyntaxErrors}\n" + errors);
		}

		private IParser ParseFile(string fileContent)
		{
			IParser parser = ParserFactory.CreateParser(this.language, new StringReader(fileContent));
			parser.Parse();
			IParser result;
			if (parser.Errors.Count > 0)
			{
				NRefactoryRefactoringProvider.ShowSourceCodeErrors(parser.Errors.ErrorOutput);
				parser.Dispose();
				result = null;
			}
			else
			{
				result = parser;
			}
			return result;
		}

		protected virtual Dictionary<NRefactoryRefactoringProvider.PossibleTypeReference, object> FindPossibleTypeReferences(string fileContent)
		{
			IParser parser = this.ParseFile(fileContent);
			Dictionary<NRefactoryRefactoringProvider.PossibleTypeReference, object> result;
			if (parser == null)
			{
				result = null;
			}
			else
			{
				NRefactoryRefactoringProvider.FindPossibleTypeReferencesVisitor visitor = new NRefactoryRefactoringProvider.FindPossibleTypeReferencesVisitor();
				parser.CompilationUnit.AcceptVisitor(visitor, null);
				parser.Dispose();
				result = visitor.list;
			}
			return result;
		}

		public override IList<IUsing> FindUnusedUsingDeclarations(string fileName, string fileContent, ICompilationUnit cu)
		{
			IClass @class = (cu.Classes.Count == 0) ? null : cu.Classes[0];
			Dictionary<NRefactoryRefactoringProvider.PossibleTypeReference, object> references = this.FindPossibleTypeReferences(fileContent);
			IList<IUsing> result;
			if (references == null)
			{
				result = new IUsing[0];
			}
			else
			{
				Dictionary<IUsing, object> dict = new Dictionary<IUsing, object>();
				foreach (NRefactoryRefactoringProvider.PossibleTypeReference tr in references.Keys)
				{
					SearchTypeRequest request = new SearchTypeRequest(tr.Name, tr.TypeParameterCount, @class, cu, 1, 1);
					SearchTypeResult response = cu.ProjectContent.SearchType(request);
					if (response.UsedUsing != null)
					{
						dict[response.UsedUsing] = null;
					}
				}
				List<IUsing> list = new List<IUsing>();
				foreach (IUsing import in cu.Usings)
				{
					if (!dict.ContainsKey(import))
					{
						if (import.HasAliases)
						{
							foreach (string key in import.Aliases.Keys)
							{
								if (references.ContainsKey(new NRefactoryRefactoringProvider.PossibleTypeReference(key)))
								{
									goto IL_174;
								}
							}
						}
						list.Add(import);
					}
					IL_174:;
				}
				result = list;
			}
			return result;
		}

		public override string CreateNewFileLikeExisting(string existingFileContent, string codeForNewType)
		{
			IParser parser = this.ParseFile(existingFileContent);
			string result;
			if (parser == null)
			{
				result = null;
			}
			else
			{
				NRefactoryRefactoringProvider.RemoveTypesVisitor visitor = new NRefactoryRefactoringProvider.RemoveTypesVisitor();
				parser.CompilationUnit.AcceptVisitor(visitor, null);
				List<ISpecial> comments = new List<ISpecial>();
				foreach (ISpecial c in parser.Lexer.SpecialTracker.CurrentSpecials)
				{
					if (c.StartPosition.Y <= visitor.includeCommentsUpToLine || c.StartPosition.Y > visitor.includeCommentsAfterLine)
					{
						comments.Add(c);
					}
				}
				IOutputAstVisitor arg_D8_0;
				if (this.language != SupportedLanguage.CSharp)
				{
					IOutputAstVisitor outputAstVisitor = new VBNetOutputVisitor();
					arg_D8_0 = outputAstVisitor;
				}
				else
				{
					arg_D8_0 = new CSharpOutputVisitor();
				}
				IOutputAstVisitor outputVisitor = arg_D8_0;
				using (SpecialNodesInserter.Install(comments, outputVisitor))
				{
					parser.CompilationUnit.AcceptVisitor(outputVisitor, null);
				}
				string expectedText;
				if (this.language == SupportedLanguage.CSharp)
				{
					expectedText = "using DummyNamespace!InsertionPos;";
				}
				else
				{
					expectedText = "Imports DummyNamespace!InsertionPos";
				}
				using (StringWriter w = new StringWriter())
				{
					using (StringReader r = new StringReader(outputVisitor.Text))
					{
						string line;
						while ((line = r.ReadLine()) != null)
						{
							string trimLine = line.TrimStart(new char[0]);
							if (trimLine == expectedText)
							{
								string indentation = line.Substring(0, line.Length - trimLine.Length);
								using (StringReader r2 = new StringReader(codeForNewType))
								{
									while ((line = r2.ReadLine()) != null)
									{
										w.Write(indentation);
										w.WriteLine(line);
									}
								}
							}
							else
							{
								w.WriteLine(line);
							}
						}
					}
					if (visitor.firstType)
					{
						w.WriteLine(codeForNewType);
					}
					result = w.ToString();
				}
			}
			return result;
		}

		public override DomRegion GetFullCodeRangeForType(string fileContent, IClass type)
		{
			ILexer lexer = ParserFactory.CreateLexer(this.language, new StringReader(fileContent));
			Stack<Location> stack = new Stack<Location>();
			Location lastPos = Location.Empty;
			Token t = lexer.NextToken();
			bool csharp = this.language == SupportedLanguage.CSharp;
			int eof = csharp ? 0 : 0;
			int attribStart = csharp ? 18 : 27;
			int attribEnd = csharp ? 19 : 26;
			while (t.kind != eof)
			{
				if (t.kind == attribStart)
				{
					stack.Push(lastPos);
				}
				if (t.EndLocation.Y >= type.Region.BeginLine)
				{
					break;
				}
				lastPos = t.EndLocation;
				if (t.kind == attribEnd && stack.Count > 0)
				{
					lastPos = stack.Pop();
				}
				t = lexer.NextToken();
			}
			while (t.kind != eof)
			{
				if (t.EndLocation.Y > type.BodyRegion.EndLine)
				{
					break;
				}
				t = lexer.NextToken();
			}
			int lastLineBefore = lastPos.IsEmpty ? 0 : lastPos.Y;
			int firstLineAfter = t.EndLocation.IsEmpty ? 2147483647 : t.EndLocation.Y;
			lexer.Dispose();
			StringReader myReader = new StringReader(fileContent);
			int resultBeginLine = lastLineBefore + 1;
			int resultEndLine = firstLineAfter - 1;
			int lineNumber = 0;
			int largestEmptyLineCount = 0;
			int emptyLinesInRow = 0;
			string line;
			while ((line = myReader.ReadLine()) != null)
			{
				lineNumber++;
				if (lineNumber > lastLineBefore)
				{
					if (lineNumber < type.Region.BeginLine)
					{
						string trimLine = line.TrimStart(new char[0]);
						if (trimLine.Length == 0)
						{
							if (++emptyLinesInRow > largestEmptyLineCount)
							{
								resultBeginLine = lineNumber;
							}
						}
					}
					else if (lineNumber != type.Region.BeginLine)
					{
						if (lineNumber == type.BodyRegion.EndLine)
						{
							largestEmptyLineCount = 0;
							emptyLinesInRow = 0;
							resultEndLine = lineNumber;
						}
						else if (lineNumber > type.BodyRegion.EndLine)
						{
							if (lineNumber >= firstLineAfter)
							{
								break;
							}
							string trimLine = line.TrimStart(new char[0]);
							if (trimLine.Length == 0)
							{
								if (++emptyLinesInRow > largestEmptyLineCount)
								{
									emptyLinesInRow = largestEmptyLineCount;
									resultEndLine = lineNumber - emptyLinesInRow;
								}
							}
						}
					}
				}
			}
			myReader.Dispose();
			return new DomRegion(resultBeginLine, 0, resultEndLine, 2147483647);
		}
	}
}
