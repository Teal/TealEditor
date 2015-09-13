using System;
using System.Collections.Generic;
using System.Reflection;

namespace AIMS.Libraries.Scripting.Dom.ReflectionLayer
{
	public class ReflectionClass : DefaultClass
	{
		private const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private void InitMembers(Type type)
		{
			Type[] nestedTypes = type.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < nestedTypes.Length; i++)
			{
				Type nestedType = nestedTypes[i];
				if (nestedType.IsVisible)
				{
					string name = nestedType.FullName.Replace('+', '.');
					this.InnerClasses.Add(new ReflectionClass(base.CompilationUnit, nestedType, name, this));
				}
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo field = fields[i];
				if (field.IsPublic || field.IsFamily || field.IsFamilyOrAssembly)
				{
					if (!field.IsSpecialName)
					{
						this.Fields.Add(new ReflectionField(field, this));
					}
				}
			}
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				ReflectionProperty prop = new ReflectionProperty(propertyInfo, this);
				if (prop.IsPublic || prop.IsProtected)
				{
					this.Properties.Add(prop);
				}
			}
			ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < constructors.Length; i++)
			{
				ConstructorInfo constructorInfo = constructors[i];
				if (constructorInfo.IsPublic || constructorInfo.IsFamily || constructorInfo.IsFamilyOrAssembly)
				{
					this.Methods.Add(new ReflectionMethod(constructorInfo, this));
				}
			}
			MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				if (methodInfo.IsPublic || methodInfo.IsFamily || methodInfo.IsFamilyOrAssembly)
				{
					if (!methodInfo.IsSpecialName)
					{
						this.Methods.Add(new ReflectionMethod(methodInfo, this));
					}
				}
			}
			EventInfo[] events = type.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < events.Length; i++)
			{
				EventInfo eventInfo = events[i];
				this.Events.Add(new ReflectionEvent(eventInfo, this));
			}
		}

		private static bool IsDelegate(Type type)
		{
			return type.IsSubclassOf(typeof(Delegate)) && type != typeof(MulticastDelegate);
		}

		internal static void AddAttributes(IProjectContent pc, IList<IAttribute> list, IList<CustomAttributeData> attributes)
		{
			foreach (CustomAttributeData att in attributes)
			{
				DefaultAttribute a = new DefaultAttribute(att.Constructor.DeclaringType.FullName);
				foreach (CustomAttributeTypedArgument arg in att.ConstructorArguments)
				{
					IReturnType type = ReflectionReturnType.Create(pc, null, arg.ArgumentType, false);
					a.PositionalArguments.Add(new AttributeArgument(type, arg.Value));
				}
				foreach (CustomAttributeNamedArgument arg2 in att.NamedArguments)
				{
					IReturnType type = ReflectionReturnType.Create(pc, null, arg2.TypedValue.ArgumentType, false);
					a.NamedArguments.Add(arg2.MemberInfo.Name, new AttributeArgument(type, arg2.TypedValue.Value));
				}
				list.Add(a);
			}
		}

		internal static void ApplySpecialsFromAttributes(DefaultClass c)
		{
			foreach (IAttribute att in c.Attributes)
			{
				if (att.Name == "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute" || att.Name == "System.Runtime.CompilerServices.CompilerGlobalScopeAttribute")
				{
					c.ClassType = ClassType.Module;
					break;
				}
			}
		}

		public ReflectionClass(ICompilationUnit compilationUnit, Type type, string fullName, IClass declaringType) : base(compilationUnit, declaringType)
		{
			if (fullName.Length > 2 && fullName[fullName.Length - 2] == '`')
			{
				base.FullyQualifiedName = fullName.Substring(0, fullName.Length - 2);
			}
			else
			{
				base.FullyQualifiedName = fullName;
			}
			this.UseInheritanceCache = true;
			try
			{
				ReflectionClass.AddAttributes(compilationUnit.ProjectContent, base.Attributes, CustomAttributeData.GetCustomAttributes(type));
			}
			catch (Exception ex)
			{
				HostCallback.ShowError("Error reading custom attributes", ex);
			}
			if (type.IsInterface)
			{
				base.ClassType = ClassType.Interface;
			}
			else if (type.IsEnum)
			{
				base.ClassType = ClassType.Enum;
			}
			else if (type.IsValueType)
			{
				base.ClassType = ClassType.Struct;
			}
			else if (ReflectionClass.IsDelegate(type))
			{
				base.ClassType = ClassType.Delegate;
			}
			else
			{
				base.ClassType = ClassType.Class;
				ReflectionClass.ApplySpecialsFromAttributes(this);
			}
			Type[] array;
			if (type.IsGenericTypeDefinition)
			{
				array = type.GetGenericArguments();
				for (int j = 0; j < array.Length; j++)
				{
					Type g = array[j];
					this.TypeParameters.Add(new DefaultTypeParameter(this, g));
				}
				int i = 0;
				array = type.GetGenericArguments();
				for (int j = 0; j < array.Length; j++)
				{
					Type g = array[j];
					this.AddConstraintsFromType(this.TypeParameters[i++], g);
				}
			}
			ModifierEnum modifiers = ModifierEnum.None;
			if (type.IsNestedAssembly)
			{
				modifiers |= ModifierEnum.Internal;
			}
			if (type.IsSealed)
			{
				modifiers |= ModifierEnum.Sealed;
			}
			if (type.IsAbstract)
			{
				modifiers |= ModifierEnum.Dim;
			}
			if (type.IsNestedPrivate)
			{
				modifiers |= ModifierEnum.Private;
			}
			else if (type.IsNestedFamily)
			{
				modifiers |= ModifierEnum.Protected;
			}
			else if (type.IsNestedPublic || type.IsPublic)
			{
				modifiers |= ModifierEnum.Public;
			}
			else if (type.IsNotPublic)
			{
				modifiers |= ModifierEnum.Internal;
			}
			else if (type.IsNestedFamORAssem || type.IsNestedFamANDAssem)
			{
				modifiers |= ModifierEnum.Protected;
				modifiers |= ModifierEnum.Internal;
			}
			base.Modifiers = modifiers;
			if (type.BaseType != null)
			{
				base.BaseTypes.Add(ReflectionReturnType.Create(this, type.BaseType, false));
			}
			array = type.GetInterfaces();
			for (int j = 0; j < array.Length; j++)
			{
				Type iface = array[j];
				base.BaseTypes.Add(ReflectionReturnType.Create(this, iface, false));
			}
			this.InitMembers(type);
		}

		internal void AddConstraintsFromType(ITypeParameter tp, Type type)
		{
			Type[] genericParameterConstraints = type.GetGenericParameterConstraints();
			for (int i = 0; i < genericParameterConstraints.Length; i++)
			{
				Type constraint = genericParameterConstraints[i];
				if (tp.Method != null)
				{
					tp.Constraints.Add(ReflectionReturnType.Create(tp.Method, constraint, false));
				}
				else
				{
					tp.Constraints.Add(ReflectionReturnType.Create(tp.Class, constraint, false));
				}
			}
		}
	}
}
