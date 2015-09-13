using System;
using System.Reflection;

namespace AIMS.Libraries.Scripting.Dom.ReflectionLayer
{
	internal class ReflectionProperty : DefaultProperty
	{
		public ReflectionProperty(PropertyInfo propertyInfo, IClass declaringType) : base(declaringType, propertyInfo.Name)
		{
			this.ReturnType = ReflectionReturnType.Create(this, propertyInfo.PropertyType, false);
			base.CanGet = propertyInfo.CanRead;
			base.CanSet = propertyInfo.CanWrite;
			ParameterInfo[] parameterInfo = propertyInfo.GetIndexParameters();
			if (parameterInfo != null && parameterInfo.Length > 0)
			{
				MemberInfo[] defaultMembers = propertyInfo.DeclaringType.GetDefaultMembers();
				for (int i = 0; i < defaultMembers.Length; i++)
				{
					MemberInfo memberInfo = defaultMembers[i];
					if (memberInfo == propertyInfo)
					{
						base.IsIndexer = true;
						break;
					}
				}
				ParameterInfo[] array = parameterInfo;
				for (int i = 0; i < array.Length; i++)
				{
					ParameterInfo info = array[i];
					this.Parameters.Add(new ReflectionParameter(info, this));
				}
			}
			MethodInfo methodBase = null;
			try
			{
				methodBase = propertyInfo.GetGetMethod(true);
			}
			catch (Exception)
			{
			}
			if (methodBase == null)
			{
				try
				{
					methodBase = propertyInfo.GetSetMethod(true);
				}
				catch (Exception)
				{
				}
			}
			ModifierEnum modifiers = ModifierEnum.None;
			if (methodBase != null)
			{
				if (methodBase.IsStatic)
				{
					modifiers |= ModifierEnum.Static;
				}
				if (methodBase.IsAssembly)
				{
					modifiers |= ModifierEnum.Internal;
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
			}
			else
			{
				modifiers = ModifierEnum.Public;
			}
			base.Modifiers = modifiers;
		}
	}
}
