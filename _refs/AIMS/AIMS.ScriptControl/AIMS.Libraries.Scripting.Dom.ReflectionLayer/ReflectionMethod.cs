using System;
using System.Reflection;

namespace AIMS.Libraries.Scripting.Dom.ReflectionLayer
{
	internal class ReflectionMethod : DefaultMethod
	{
		internal static void ApplySpecialsFromAttributes(DefaultMethod m)
		{
			if (m.IsStatic)
			{
				foreach (IAttribute a in m.Attributes)
				{
					string attributeName = a.Name;
					if (attributeName == "System.Runtime.CompilerServices.ExtensionAttribute" || attributeName == "Boo.Lang.ExtensionAttribute")
					{
						m.IsExtensionMethod = true;
					}
				}
			}
		}

		public ReflectionMethod(MethodBase methodBase, ReflectionClass declaringType) : base(declaringType, (methodBase is ConstructorInfo) ? "#ctor" : methodBase.Name)
		{
			if (methodBase is MethodInfo)
			{
				this.ReturnType = ReflectionReturnType.Create(this, ((MethodInfo)methodBase).ReturnType, false);
			}
			else if (methodBase is ConstructorInfo)
			{
				this.ReturnType = base.DeclaringType.DefaultReturnType;
			}
			ParameterInfo[] parameters = methodBase.GetParameters();
			for (int j = 0; j < parameters.Length; j++)
			{
				ParameterInfo paramInfo = parameters[j];
				this.Parameters.Add(new ReflectionParameter(paramInfo, this));
			}
			if (methodBase.IsGenericMethodDefinition)
			{
				Type[] genericArguments = methodBase.GetGenericArguments();
				for (int j = 0; j < genericArguments.Length; j++)
				{
					Type g = genericArguments[j];
					this.TypeParameters.Add(new DefaultTypeParameter(this, g));
				}
				int i = 0;
				genericArguments = methodBase.GetGenericArguments();
				for (int j = 0; j < genericArguments.Length; j++)
				{
					Type g = genericArguments[j];
					declaringType.AddConstraintsFromType(this.TypeParameters[i++], g);
				}
			}
			ModifierEnum modifiers = ModifierEnum.None;
			if (methodBase.IsStatic)
			{
				modifiers |= ModifierEnum.Static;
			}
			if (methodBase.IsPrivate)
			{
				modifiers |= ModifierEnum.Private;
			}
			else if (methodBase.IsFamily || methodBase.IsFamilyOrAssembly)
			{
				modifiers |= ModifierEnum.Protected;
			}
			else if (methodBase.IsPublic)
			{
				modifiers |= ModifierEnum.Public;
			}
			else
			{
				modifiers |= ModifierEnum.Internal;
			}
			if (methodBase.IsVirtual)
			{
				modifiers |= ModifierEnum.Virtual;
			}
			if (methodBase.IsAbstract)
			{
				modifiers |= ModifierEnum.Dim;
			}
			if (methodBase.IsFinal)
			{
				modifiers |= ModifierEnum.Sealed;
			}
			base.Modifiers = modifiers;
			ReflectionClass.AddAttributes(declaringType.ProjectContent, base.Attributes, CustomAttributeData.GetCustomAttributes(methodBase));
			ReflectionMethod.ApplySpecialsFromAttributes(this);
		}
	}
}
