using AIMS.Libraries.Scripting.Dom.Refactoring;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;

namespace AIMS.Libraries.Scripting.Dom
{
	public class LanguageProperties
	{
		private sealed class DummyCodeDomProvider : CodeDomProvider
		{
			public static readonly LanguageProperties.DummyCodeDomProvider Instance = new LanguageProperties.DummyCodeDomProvider();

			[Obsolete("Callers should not use the ICodeGenerator interface and should instead use the methods directly on the CodeDomProvider class.")]
			public override ICodeGenerator CreateGenerator()
			{
				return null;
			}

			[Obsolete("Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class.")]
			public override ICodeCompiler CreateCompiler()
			{
				return null;
			}
		}

		internal sealed class CSharpProperties : LanguageProperties
		{
			public override RefactoringProvider RefactoringProvider
			{
				get
				{
					return NRefactoryRefactoringProvider.NRefactoryCSharpProviderInstance;
				}
			}

			public override AIMS.Libraries.Scripting.Dom.Refactoring.CodeGenerator CodeGenerator
			{
				get
				{
					return CSharpCodeGenerator.Instance;
				}
			}

			public override CodeDomProvider CodeDomProvider
			{
				get
				{
					return new CSharpCodeProvider();
				}
			}

			public override bool SupportsImplicitInterfaceImplementation
			{
				get
				{
					return true;
				}
			}

			public override bool ExplicitInterfaceImplementationIsPrivateScope
			{
				get
				{
					return true;
				}
			}

			public override bool RequiresAddRemoveRegionInExplicitInterfaceImplementation
			{
				get
				{
					return true;
				}
			}

			public CSharpProperties() : base(StringComparer.InvariantCulture)
			{
			}

			public override string ToString()
			{
				return "[LanguageProperties: C#]";
			}
		}

		internal sealed class VBNetProperties : LanguageProperties
		{
			public override bool ImportNamespaces
			{
				get
				{
					return true;
				}
			}

			public override bool ImportModules
			{
				get
				{
					return true;
				}
			}

			public override bool CanImportClasses
			{
				get
				{
					return true;
				}
			}

			public override string IndexerExpressionStartToken
			{
				get
				{
					return "(";
				}
			}

			public override RefactoringProvider RefactoringProvider
			{
				get
				{
					return NRefactoryRefactoringProvider.NRefactoryVBNetProviderInstance;
				}
			}

			public override AIMS.Libraries.Scripting.Dom.Refactoring.CodeGenerator CodeGenerator
			{
				get
				{
					return VBNetCodeGenerator.Instance;
				}
			}

			public override CodeDomProvider CodeDomProvider
			{
				get
				{
					return new VBCodeProvider();
				}
			}

			public VBNetProperties() : base(StringComparer.InvariantCultureIgnoreCase)
			{
			}

			public override bool ShowMember(IMember member, bool showStatic)
			{
				return !(member is ArrayReturnType.ArrayIndexer) && (member.IsStatic || !showStatic);
			}

			public override bool IsClassWithImplicitlyStaticMembers(IClass c)
			{
				return c.ClassType == ClassType.Module;
			}

			public override bool ShowInNamespaceCompletion(IClass c)
			{
				bool result;
				foreach (IAttribute attr in c.Attributes)
				{
					if (base.NameComparer.Equals(attr.Name, "Microsoft.VisualBasic.HideModuleNameAttribute"))
					{
						result = false;
						return result;
					}
					if (base.NameComparer.Equals(attr.Name, "HideModuleNameAttribute"))
					{
						result = false;
						return result;
					}
					if (base.NameComparer.Equals(attr.Name, "Microsoft.VisualBasic.HideModuleName"))
					{
						result = false;
						return result;
					}
					if (base.NameComparer.Equals(attr.Name, "HideModuleName"))
					{
						result = false;
						return result;
					}
				}
				result = base.ShowInNamespaceCompletion(c);
				return result;
			}

			public override IUsing CreateDefaultImports(IProjectContent pc)
			{
				return new DefaultUsing(pc)
				{
					Usings = 
					{
						"Microsoft.VisualBasic",
						"System",
						"System.Collections",
						"System.Collections.Generic",
						"System.Drawing",
						"System.Diagnostics",
						"System.Windows.Forms"
					}
				};
			}

			public override string ToString()
			{
				return "[LanguageProperties: VB.NET]";
			}
		}

		public static readonly LanguageProperties None = new LanguageProperties(StringComparer.InvariantCulture);

		public static readonly LanguageProperties CSharp = new LanguageProperties.CSharpProperties();

		public static readonly LanguageProperties VBNet = new LanguageProperties.VBNetProperties();

		private readonly StringComparer nameComparer;

		public StringComparer NameComparer
		{
			get
			{
				return this.nameComparer;
			}
		}

		public virtual AIMS.Libraries.Scripting.Dom.Refactoring.CodeGenerator CodeGenerator
		{
			get
			{
				return AIMS.Libraries.Scripting.Dom.Refactoring.CodeGenerator.DummyCodeGenerator;
			}
		}

		public virtual RefactoringProvider RefactoringProvider
		{
			get
			{
				return RefactoringProvider.DummyProvider;
			}
		}

		public virtual CodeDomProvider CodeDomProvider
		{
			get
			{
				return null;
			}
		}

		public virtual bool SupportsExtensionMethods
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportsExtensionProperties
		{
			get
			{
				return false;
			}
		}

		public virtual bool SearchExtensionsInClasses
		{
			get
			{
				return false;
			}
		}

		public virtual bool ImportNamespaces
		{
			get
			{
				return false;
			}
		}

		public virtual bool ImportModules
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanImportClasses
		{
			get
			{
				return false;
			}
		}

		public virtual bool ImplicitPartialClasses
		{
			get
			{
				return false;
			}
		}

		public virtual bool AllowObjectConstructionOutsideContext
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportsImplicitInterfaceImplementation
		{
			get
			{
				return false;
			}
		}

		public virtual bool ExplicitInterfaceImplementationIsPrivateScope
		{
			get
			{
				return false;
			}
		}

		public virtual bool RequiresAddRemoveRegionInExplicitInterfaceImplementation
		{
			get
			{
				return false;
			}
		}

		public virtual string IndexerExpressionStartToken
		{
			get
			{
				return "[";
			}
		}

		public LanguageProperties(StringComparer nameComparer)
		{
			this.nameComparer = nameComparer;
		}

		public virtual bool IsClassWithImplicitlyStaticMembers(IClass c)
		{
			return false;
		}

		public virtual bool ShowInNamespaceCompletion(IClass c)
		{
			return true;
		}

		public virtual bool ShowMember(IMember member, bool showStatic)
		{
			return (!(member is IProperty) || !((IProperty)member).IsIndexer) && member.IsStatic == showStatic;
		}

		public virtual IUsing CreateDefaultImports(IProjectContent pc)
		{
			return null;
		}

		public override string ToString()
		{
			return "[" + base.ToString() + "]";
		}
	}
}
