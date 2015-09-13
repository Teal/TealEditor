using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory
{
	public interface IParser : IDisposable
	{
		Errors Errors
		{
			get;
		}

		ILexer Lexer
		{
			get;
		}

		CompilationUnit CompilationUnit
		{
			get;
		}

		bool ParseMethodBodies
		{
			get;
			set;
		}

		void Parse();

		Expression ParseExpression();

		BlockStatement ParseBlock();

		List<INode> ParseTypeMembers();
	}
}
