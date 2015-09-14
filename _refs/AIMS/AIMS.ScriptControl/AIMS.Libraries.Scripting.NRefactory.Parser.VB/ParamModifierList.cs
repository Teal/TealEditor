using AIMS.Libraries.Scripting.NRefactory.Ast;
using System;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.VB
{
	internal class ParamModifierList
	{
		private ParameterModifiers cur;

		private Parser parser;

		public ParameterModifiers Modifier
		{
			get
			{
				return this.cur;
			}
		}

		public bool isNone
		{
			get
			{
				return this.cur == ParameterModifiers.None;
			}
		}

		public ParamModifierList(Parser parser)
		{
			this.parser = parser;
			this.cur = ParameterModifiers.None;
		}

		public void Add(ParameterModifiers m)
		{
			if ((this.cur & m) == ParameterModifiers.None)
			{
				this.cur |= m;
			}
			else
			{
				this.parser.Error("param modifier " + m + " already defined");
			}
		}

		public void Add(ParamModifierList m)
		{
			this.Add(m.cur);
		}

		public void Check()
		{
			if ((this.cur & ParameterModifiers.In) != ParameterModifiers.None && (this.cur & ParameterModifiers.Ref) != ParameterModifiers.None)
			{
				this.parser.Error("ByRef and ByVal are not allowed at the same time.");
			}
		}
	}
}
