using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	public abstract class AbstractParser : IParser, IDisposable
	{
		protected const int MinErrDist = 2;

		protected const string ErrMsgFormat = "-- line {0} col {1}: {2}";

		private Errors errors;

		private ILexer lexer;

		protected int errDist = 2;

		[CLSCompliant(false)]
		protected CompilationUnit compilationUnit;

		private bool parseMethodContents = true;

		public bool ParseMethodBodies
		{
			get
			{
				return this.parseMethodContents;
			}
			set
			{
				this.parseMethodContents = value;
			}
		}

		public ILexer Lexer
		{
			get
			{
				return this.lexer;
			}
		}

		public Errors Errors
		{
			get
			{
				return this.errors;
			}
		}

		public CompilationUnit CompilationUnit
		{
			get
			{
				return this.compilationUnit;
			}
		}

		internal AbstractParser(ILexer lexer)
		{
			this.errors = lexer.Errors;
			this.lexer = lexer;
			this.errors.SynErr = new ErrorCodeProc(this.SynErr);
		}

		public abstract void Parse();

		public abstract Expression ParseExpression();

		public abstract BlockStatement ParseBlock();

		public abstract List<INode> ParseTypeMembers();

		protected abstract void SynErr(int line, int col, int errorNumber);

		protected void SynErr(int n)
		{
			if (this.errDist >= 2)
			{
				this.errors.SynErr(this.lexer.LookAhead.line, this.lexer.LookAhead.col, n);
			}
			this.errDist = 0;
		}

		protected void SemErr(string msg)
		{
			if (this.errDist >= 2)
			{
				this.errors.Error(this.lexer.Token.line, this.lexer.Token.col, msg);
			}
			this.errDist = 0;
		}

		protected void Expect(int n)
		{
			if (this.lexer.LookAhead.kind == n)
			{
				this.lexer.NextToken();
			}
			else
			{
				this.SynErr(n);
			}
		}

		public void Dispose()
		{
			this.errors = null;
			if (this.lexer != null)
			{
				this.lexer.Dispose();
			}
			this.lexer = null;
		}
	}
}
