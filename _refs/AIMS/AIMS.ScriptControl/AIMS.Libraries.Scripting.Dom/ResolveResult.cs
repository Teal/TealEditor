using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.Dom
{
	public class ResolveResult
	{
		private IClass callingClass;

		private IMember callingMember;

		private IReturnType resolvedType;

		public IClass CallingClass
		{
			get
			{
				return this.callingClass;
			}
		}

		public IMember CallingMember
		{
			get
			{
				return this.callingMember;
			}
		}

		public IReturnType ResolvedType
		{
			get
			{
				return this.resolvedType;
			}
			set
			{
				this.resolvedType = value;
			}
		}

		public ResolveResult(IClass callingClass, IMember callingMember, IReturnType resolvedType)
		{
			this.callingClass = callingClass;
			this.callingMember = callingMember;
			this.resolvedType = resolvedType;
		}

		public virtual ArrayList GetCompletionData(IProjectContent projectContent)
		{
			return this.GetCompletionData(projectContent.Language, false);
		}

		protected ArrayList GetCompletionData(LanguageProperties language, bool showStatic)
		{
			ArrayList result;
			if (this.resolvedType == null)
			{
				result = null;
			}
			else
			{
				ArrayList res = new ArrayList();
				bool isClassInInheritanceTree = false;
				if (this.callingClass != null)
				{
					isClassInInheritanceTree = this.callingClass.IsTypeInInheritanceTree(this.resolvedType.GetUnderlyingClass());
				}
				foreach (IMethod i in this.resolvedType.GetMethods())
				{
					if (language.ShowMember(i, showStatic) && i.IsAccessible(this.callingClass, isClassInInheritanceTree))
					{
						res.Add(i);
					}
				}
				foreach (IEvent e in this.resolvedType.GetEvents())
				{
					if (language.ShowMember(e, showStatic) && e.IsAccessible(this.callingClass, isClassInInheritanceTree))
					{
						res.Add(e);
					}
				}
				foreach (IField f in this.resolvedType.GetFields())
				{
					if (language.ShowMember(f, showStatic) && f.IsAccessible(this.callingClass, isClassInInheritanceTree))
					{
						res.Add(f);
					}
				}
				foreach (IProperty p in this.resolvedType.GetProperties())
				{
					if (language.ShowMember(p, showStatic) && p.IsAccessible(this.callingClass, isClassInInheritanceTree))
					{
						res.Add(p);
					}
				}
				if (!showStatic && this.callingClass != null)
				{
					ResolveResult.AddExtensions(language, res, this.callingClass, this.resolvedType);
				}
				result = res;
			}
			return result;
		}

		public static void AddExtensions(LanguageProperties language, ArrayList res, IClass callingClass, IReturnType resolvedType)
		{
			if (language == null)
			{
				throw new ArgumentNullException("language");
			}
			if (res == null)
			{
				throw new ArgumentNullException("res");
			}
			if (callingClass == null)
			{
				throw new ArgumentNullException("callingClass");
			}
			if (resolvedType == null)
			{
				throw new ArgumentNullException("resolvedType");
			}
			bool supportsExtensionMethods = language.SupportsExtensionMethods;
			bool supportsExtensionProperties = language.SupportsExtensionProperties;
			if (supportsExtensionMethods || supportsExtensionProperties)
			{
				ArrayList list = new ArrayList();
				IMethod dummyMethod = new DefaultMethod("dummy", VoidReturnType.Instance, ModifierEnum.Static, DomRegion.Empty, DomRegion.Empty, callingClass);
				CtrlSpaceResolveHelper.AddContentsFromCalling(list, callingClass, dummyMethod);
				CtrlSpaceResolveHelper.AddImportedNamespaceContents(list, callingClass.CompilationUnit, callingClass);
				bool searchExtensionsInClasses = language.SearchExtensionsInClasses;
				foreach (object o in list)
				{
					if ((supportsExtensionMethods && o is IMethod) || (supportsExtensionProperties && o is IProperty))
					{
						ResolveResult.TryAddExtension(language, res, o as IMethodOrProperty, resolvedType);
					}
					else if (searchExtensionsInClasses && o is IClass)
					{
						IClass c = o as IClass;
						if (c.HasExtensionMethods)
						{
							if (supportsExtensionProperties)
							{
								foreach (IProperty p in c.Properties)
								{
									ResolveResult.TryAddExtension(language, res, p, resolvedType);
								}
							}
							if (supportsExtensionMethods)
							{
								foreach (IMethod i in c.Methods)
								{
									ResolveResult.TryAddExtension(language, res, i, resolvedType);
								}
							}
						}
					}
				}
			}
		}

		private static void TryAddExtension(LanguageProperties language, ArrayList res, IMethodOrProperty ext, IReturnType resolvedType)
		{
			if (ext.IsExtensionMethod)
			{
				foreach (IMember member in res)
				{
					IMethodOrProperty p = member as IMethodOrProperty;
					if (p == null || !p.IsExtensionMethod)
					{
						if (language.NameComparer.Equals(member.Name, ext.Name))
						{
							return;
						}
					}
				}
				if (MemberLookupHelper.ConversionExists(resolvedType, ext.Parameters[0].ReturnType))
				{
					IMethod method = ext as IMethod;
					if (method != null && method.TypeParameters.Count > 0)
					{
						IReturnType[] typeArguments = new IReturnType[method.TypeParameters.Count];
						MemberLookupHelper.InferTypeArgument(method.Parameters[0].ReturnType, resolvedType, typeArguments);
						for (int i = 0; i < typeArguments.Length; i++)
						{
							if (typeArguments[i] != null)
							{
								ext = (IMethod)ext.Clone();
								ext.ReturnType = ConstructedReturnType.TranslateType(ext.ReturnType, typeArguments, true);
								for (int j = 0; j < ext.Parameters.Count; j++)
								{
									ext.Parameters[j].ReturnType = ConstructedReturnType.TranslateType(ext.Parameters[j].ReturnType, typeArguments, true);
								}
								break;
							}
						}
					}
					res.Add(ext);
				}
			}
		}

		public virtual FilePosition GetDefinitionPosition()
		{
			return FilePosition.Empty;
		}
	}
}
