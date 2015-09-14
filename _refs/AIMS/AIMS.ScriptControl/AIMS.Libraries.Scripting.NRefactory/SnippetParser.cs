using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public class SnippetParser
	{
		private sealed class NodeListNode : INode
		{
			private List<INode> nodes;

			public INode Parent
			{
				get
				{
					return null;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public List<INode> Children
			{
				get
				{
					return this.nodes;
				}
			}

			public Location StartLocation
			{
				get
				{
					return Location.Empty;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public Location EndLocation
			{
				get
				{
					return Location.Empty;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public NodeListNode(List<INode> nodes)
			{
				this.nodes = nodes;
			}

			public object AcceptChildren(IAstVisitor visitor, object data)
			{
				foreach (INode i in this.nodes)
				{
					i.AcceptVisitor(visitor, data);
				}
				return null;
			}

			public object AcceptVisitor(IAstVisitor visitor, object data)
			{
				return this.AcceptChildren(visitor, data);
			}
		}

		private readonly SupportedLanguage language;

		private Errors errors;

		private List<ISpecial> specials;

		public Errors Errors
		{
			get
			{
				return this.errors;
			}
		}

		public List<ISpecial> Specials
		{
			get
			{
				return this.specials;
			}
		}

		public SnippetParser(SupportedLanguage language)
		{
			this.language = language;
		}

		public INode Parse(string code)
		{
			IParser parser = ParserFactory.CreateParser(this.language, new StringReader(code));
			parser.Parse();
			this.errors = parser.Errors;
			this.specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
			INode result = parser.CompilationUnit;
			if (this.errors.Count > 0)
			{
				if (this.language == SupportedLanguage.CSharp)
				{
					parser = ParserFactory.CreateParser(this.language, new StringReader(code + ";"));
				}
				else
				{
					parser = ParserFactory.CreateParser(this.language, new StringReader(code));
				}
				Expression expression = parser.ParseExpression();
				if (expression != null && parser.Errors.Count < this.errors.Count)
				{
					this.errors = parser.Errors;
					this.specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
					result = expression;
				}
			}
			if (this.errors.Count > 0)
			{
				parser = ParserFactory.CreateParser(this.language, new StringReader(code));
				BlockStatement block = parser.ParseBlock();
				if (block != null && parser.Errors.Count < this.errors.Count)
				{
					this.errors = parser.Errors;
					this.specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
					result = block;
				}
			}
			if (this.errors.Count > 0)
			{
				parser = ParserFactory.CreateParser(this.language, new StringReader(code));
				List<INode> members = parser.ParseTypeMembers();
				if (members != null && members.Count > 0 && parser.Errors.Count < this.errors.Count)
				{
					this.errors = parser.Errors;
					this.specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
					result = new SnippetParser.NodeListNode(members);
				}
			}
			return result;
		}
	}
}
