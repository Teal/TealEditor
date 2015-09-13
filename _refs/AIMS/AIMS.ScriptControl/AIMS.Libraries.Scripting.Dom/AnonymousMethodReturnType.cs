using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class AnonymousMethodReturnType : ProxyReturnType
	{
		private IReturnType returnType;

		private IList<IParameter> parameters = new List<IParameter>();

		private ICompilationUnit cu;

		private volatile DefaultClass cachedClass;

		public IReturnType MethodReturnType
		{
			get
			{
				return this.returnType;
			}
			set
			{
				this.returnType = value;
			}
		}

		public IList<IParameter> MethodParameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.parameters = value;
			}
		}

		public override bool IsDefaultReturnType
		{
			get
			{
				return false;
			}
		}

		public override IReturnType BaseType
		{
			get
			{
				return this.GetUnderlyingClass().DefaultReturnType;
			}
		}

		public override string Name
		{
			get
			{
				return "delegate";
			}
		}

		public override string FullyQualifiedName
		{
			get
			{
				StringBuilder b = new StringBuilder("delegate(");
				bool first = true;
				foreach (IParameter p in this.parameters)
				{
					if (first)
					{
						first = false;
					}
					else
					{
						b.Append(", ");
					}
					b.Append(p.Name);
					if (p.ReturnType != null)
					{
						b.Append(":");
						b.Append(p.ReturnType.Name);
					}
				}
				b.Append(")");
				if (this.returnType != null)
				{
					b.Append(":");
					b.Append(this.returnType.Name);
				}
				return b.ToString();
			}
		}

		public override string Namespace
		{
			get
			{
				return "";
			}
		}

		public override string DotNetName
		{
			get
			{
				return this.Name;
			}
		}

		public AnonymousMethodReturnType(ICompilationUnit cu)
		{
			this.cu = cu;
		}

		public override IClass GetUnderlyingClass()
		{
			IClass result;
			if (this.cachedClass != null)
			{
				result = this.cachedClass;
			}
			else
			{
				DefaultClass c = new DefaultClass(this.cu, ClassType.Delegate, ModifierEnum.None, DomRegion.Empty, null);
				c.BaseTypes.Add(this.cu.ProjectContent.SystemTypes.Delegate);
				AnonymousMethodReturnType.AddDefaultDelegateMethod(c, this.returnType ?? this.cu.ProjectContent.SystemTypes.Object, this.parameters);
				this.cachedClass = c;
				result = c;
			}
			return result;
		}

		internal static void AddDefaultDelegateMethod(DefaultClass c, IReturnType returnType, IList<IParameter> parameters)
		{
			ModifierEnum modifiers = ModifierEnum.Public | ModifierEnum.Synthetic;
			DefaultMethod invokeMethod = new DefaultMethod("Invoke", returnType, modifiers, c.Region, DomRegion.Empty, c);
			foreach (IParameter par in parameters)
			{
				invokeMethod.Parameters.Add(par);
			}
			c.Methods.Add(invokeMethod);
			invokeMethod = new DefaultMethod("BeginInvoke", c.ProjectContent.SystemTypes.IAsyncResult, modifiers, c.Region, DomRegion.Empty, c);
			foreach (IParameter par in parameters)
			{
				invokeMethod.Parameters.Add(par);
			}
			invokeMethod.Parameters.Add(new DefaultParameter("callback", c.ProjectContent.SystemTypes.AsyncCallback, DomRegion.Empty));
			invokeMethod.Parameters.Add(new DefaultParameter("object", c.ProjectContent.SystemTypes.Object, DomRegion.Empty));
			c.Methods.Add(invokeMethod);
			invokeMethod = new DefaultMethod("EndInvoke", returnType, modifiers, c.Region, DomRegion.Empty, c);
			invokeMethod.Parameters.Add(new DefaultParameter("result", c.ProjectContent.SystemTypes.IAsyncResult, DomRegion.Empty));
			c.Methods.Add(invokeMethod);
		}
	}
}
