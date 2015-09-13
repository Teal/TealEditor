using System;
using System.CodeDom;

namespace ICSharpCode.EasyCodeDom
{
	public class EasyNamespace : CodeNamespace
	{
		public EasyNamespace()
		{
		}

		public EasyNamespace(string name) : base(name)
		{
		}

		public EasyTypeDeclaration AddType(string name)
		{
			EasyTypeDeclaration i = new EasyTypeDeclaration(name);
			base.Types.Add(i);
			return i;
		}

		public CodeNamespaceImport AddImport(string nameSpace)
		{
			CodeNamespaceImport cni = new CodeNamespaceImport(nameSpace);
			base.Imports.Add(cni);
			return cni;
		}
	}
}
