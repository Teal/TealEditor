using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultReturnType : AbstractReturnType
	{
		private IClass c;

		private bool getMembersBusy;

		public override int TypeParameterCount
		{
			get
			{
				return this.c.TypeParameters.Count;
			}
		}

		public override string FullyQualifiedName
		{
			get
			{
				return this.c.FullyQualifiedName;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override string Name
		{
			get
			{
				return this.c.Name;
			}
		}

		public override string Namespace
		{
			get
			{
				return this.c.Namespace;
			}
		}

		public override string DotNetName
		{
			get
			{
				return this.c.DotNetName;
			}
		}

		public static bool Equals(IReturnType rt1, IReturnType rt2)
		{
			return rt1.FullyQualifiedName == rt2.FullyQualifiedName && rt1.TypeParameterCount == rt2.TypeParameterCount;
		}

		public DefaultReturnType(IClass c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			this.c = c;
		}

		public override string ToString()
		{
			return this.c.FullyQualifiedName;
		}

		public override IClass GetUnderlyingClass()
		{
			return this.c;
		}

		public override List<IMethod> GetMethods()
		{
			List<IMethod> result;
			if (this.getMembersBusy)
			{
				result = new List<IMethod>();
			}
			else
			{
				this.getMembersBusy = true;
				List<IMethod> i = new List<IMethod>();
				i.AddRange(this.c.Methods);
				if (this.c.ClassType == ClassType.Interface)
				{
					if (this.c.BaseTypes.Count == 0)
					{
						this.AddMethodsFromBaseType(i, this.c.ProjectContent.SystemTypes.Object);
					}
					else
					{
						foreach (IReturnType baseType in this.c.BaseTypes)
						{
							this.AddMethodsFromBaseType(i, baseType);
						}
					}
				}
				else
				{
					this.AddMethodsFromBaseType(i, this.c.BaseType);
				}
				this.getMembersBusy = false;
				result = i;
			}
			return result;
		}

		private void AddMethodsFromBaseType(List<IMethod> l, IReturnType baseType)
		{
			if (baseType != null)
			{
				foreach (IMethod i in baseType.GetMethods())
				{
					if (!i.IsConstructor)
					{
						bool ok = true;
						if (i.IsOverridable)
						{
							StringComparer comparer = i.DeclaringType.ProjectContent.Language.NameComparer;
							foreach (IMethod oldMethod in this.c.Methods)
							{
								if (comparer.Equals(oldMethod.Name, i.Name))
								{
									if (i.IsStatic == oldMethod.IsStatic && object.Equals(i.ReturnType, oldMethod.ReturnType))
									{
										if (DiffUtility.Compare<IParameter>(oldMethod.Parameters, i.Parameters) == 0)
										{
											ok = false;
											break;
										}
									}
								}
							}
						}
						if (ok)
						{
							l.Add(i);
						}
					}
				}
			}
		}

		public override List<IProperty> GetProperties()
		{
			List<IProperty> result;
			if (this.getMembersBusy)
			{
				result = new List<IProperty>();
			}
			else
			{
				this.getMembersBusy = true;
				List<IProperty> i = new List<IProperty>();
				i.AddRange(this.c.Properties);
				if (this.c.ClassType == ClassType.Interface)
				{
					foreach (IReturnType baseType in this.c.BaseTypes)
					{
						this.AddPropertiesFromBaseType(i, baseType);
					}
				}
				else
				{
					this.AddPropertiesFromBaseType(i, this.c.BaseType);
				}
				this.getMembersBusy = false;
				result = i;
			}
			return result;
		}

		private void AddPropertiesFromBaseType(List<IProperty> l, IReturnType baseType)
		{
			if (baseType != null)
			{
				foreach (IProperty p in baseType.GetProperties())
				{
					bool ok = true;
					if (p.IsOverridable)
					{
						StringComparer comparer = p.DeclaringType.ProjectContent.Language.NameComparer;
						foreach (IProperty oldProperty in this.c.Properties)
						{
							if (comparer.Equals(oldProperty.Name, p.Name))
							{
								if (p.IsStatic == oldProperty.IsStatic && object.Equals(p.ReturnType, oldProperty.ReturnType))
								{
									if (DiffUtility.Compare<IParameter>(oldProperty.Parameters, p.Parameters) == 0)
									{
										ok = false;
										break;
									}
								}
							}
						}
					}
					if (ok)
					{
						l.Add(p);
					}
				}
			}
		}

		public override List<IField> GetFields()
		{
			List<IField> result;
			if (this.getMembersBusy)
			{
				result = new List<IField>();
			}
			else
			{
				this.getMembersBusy = true;
				List<IField> i = new List<IField>();
				i.AddRange(this.c.Fields);
				if (this.c.ClassType == ClassType.Interface)
				{
					foreach (IReturnType baseType in this.c.BaseTypes)
					{
						i.AddRange(baseType.GetFields());
					}
				}
				else
				{
					IReturnType baseType = this.c.BaseType;
					if (baseType != null)
					{
						i.AddRange(baseType.GetFields());
					}
				}
				this.getMembersBusy = false;
				result = i;
			}
			return result;
		}

		public override List<IEvent> GetEvents()
		{
			List<IEvent> result;
			if (this.getMembersBusy)
			{
				result = new List<IEvent>();
			}
			else
			{
				this.getMembersBusy = true;
				List<IEvent> i = new List<IEvent>();
				i.AddRange(this.c.Events);
				if (this.c.ClassType == ClassType.Interface)
				{
					foreach (IReturnType baseType in this.c.BaseTypes)
					{
						i.AddRange(baseType.GetEvents());
					}
				}
				else
				{
					IReturnType baseType = this.c.BaseType;
					if (baseType != null)
					{
						i.AddRange(baseType.GetEvents());
					}
				}
				this.getMembersBusy = false;
				result = i;
			}
			return result;
		}
	}
}
