using System;
using System.Reflection;

namespace AIMS.Libraries.Scripting.Dom.ReflectionLayer
{
	internal class ReflectionEvent : DefaultEvent
	{
		public ReflectionEvent(EventInfo eventInfo, IClass declaringType) : base(declaringType, eventInfo.Name)
		{
			this.ReturnType = ReflectionReturnType.Create(this, eventInfo.EventHandlerType, false);
			MethodInfo methodBase = null;
			try
			{
				methodBase = eventInfo.GetAddMethod(true);
			}
			catch (Exception)
			{
			}
			if (methodBase == null)
			{
				try
				{
					methodBase = eventInfo.GetRemoveMethod(true);
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
			}
			else
			{
				modifiers = ModifierEnum.Public;
			}
			base.Modifiers = modifiers;
		}
	}
}
