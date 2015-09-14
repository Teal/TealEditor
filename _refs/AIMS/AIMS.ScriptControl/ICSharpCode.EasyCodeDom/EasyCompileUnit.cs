using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public class EasyCompileUnit : CodeCompileUnit
	{
		public EasyNamespace AddNamespace(string name)
		{
			EasyNamespace i = new EasyNamespace(name);
			base.Namespaces.Add(i);
			return i;
		}
	}
}
