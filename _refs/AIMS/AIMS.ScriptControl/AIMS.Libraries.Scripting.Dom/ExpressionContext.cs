using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public abstract class ExpressionContext
	{
		private sealed class DefaultExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				return true;
			}

			public override string ToString()
			{
				return "[" + base.GetType().Name + "]";
			}
		}

		private sealed class ImportableExpressionContext : ExpressionContext
		{
			private bool allowImportClasses;

			public ImportableExpressionContext(bool allowImportClasses)
			{
				this.allowImportClasses = allowImportClasses;
			}

			public override bool ShowEntry(object o)
			{
				bool result;
				if (o is string)
				{
					result = true;
				}
				else
				{
					IClass c = o as IClass;
					result = (this.allowImportClasses && c != null && c.HasPublicOrInternalStaticMembers);
				}
				return result;
			}

			public override string ToString()
			{
				string result;
				if (this.allowImportClasses)
				{
					result = "[ImportableExpressionContext]";
				}
				else
				{
					result = "[NamespaceExpressionContext]";
				}
				return result;
			}
		}

		private sealed class TypeExpressionContext : ExpressionContext
		{
			private IClass baseClass;

			private bool isObjectCreation;

			public override bool IsObjectCreation
			{
				get
				{
					return this.isObjectCreation;
				}
				set
				{
					if (this.readOnly && value != this.isObjectCreation)
					{
						throw new NotSupportedException();
					}
					this.isObjectCreation = value;
				}
			}

			public override bool IsAttributeContext
			{
				get
				{
					return this.baseClass != null && this.baseClass.FullyQualifiedName == "System.Attribute";
				}
			}

			public TypeExpressionContext(IClass baseClass, bool isObjectCreation, bool readOnly)
			{
				this.baseClass = baseClass;
				this.isObjectCreation = isObjectCreation;
				this.readOnly = readOnly;
			}

			public override bool ShowEntry(object o)
			{
				bool result;
				if (o is string)
				{
					result = true;
				}
				else
				{
					IClass c = o as IClass;
					if (c == null)
					{
						result = false;
					}
					else
					{
						if (this.isObjectCreation)
						{
							if (c.IsAbstract || c.IsStatic)
							{
								result = false;
								return result;
							}
							if (c.ClassType == ClassType.Enum || c.ClassType == ClassType.Interface)
							{
								result = false;
								return result;
							}
						}
						result = (this.baseClass == null || c.IsTypeInInheritanceTree(this.baseClass));
					}
				}
				return result;
			}

			public override string ToString()
			{
				string result;
				if (this.baseClass != null)
				{
					result = string.Concat(new object[]
					{
						"[",
						base.GetType().Name,
						": ",
						this.baseClass.FullyQualifiedName,
						" IsObjectCreation=",
						this.IsObjectCreation,
						"]"
					});
				}
				else
				{
					result = string.Concat(new object[]
					{
						"[",
						base.GetType().Name,
						" IsObjectCreation=",
						this.IsObjectCreation,
						"]"
					});
				}
				return result;
			}
		}

		private sealed class CombinedExpressionContext : ExpressionContext
		{
			private byte opType;

			private ExpressionContext a;

			private ExpressionContext b;

			public CombinedExpressionContext(byte opType, ExpressionContext a, ExpressionContext b)
			{
				if (a == null)
				{
					throw new ArgumentNullException("a");
				}
				if (b == null)
				{
					throw new ArgumentNullException("a");
				}
				this.opType = opType;
				this.a = a;
				this.b = b;
			}

			public override bool ShowEntry(object o)
			{
				bool result;
				if (this.opType == 0)
				{
					result = (this.a.ShowEntry(o) || this.b.ShowEntry(o));
				}
				else if (this.opType == 1)
				{
					result = (this.a.ShowEntry(o) && this.b.ShowEntry(o));
				}
				else
				{
					result = (this.a.ShowEntry(o) ^ this.b.ShowEntry(o));
				}
				return result;
			}

			public override string ToString()
			{
				string op = " XOR ";
				if (this.opType == 0)
				{
					op = " OR ";
				}
				else if (this.opType == 1)
				{
					op = " AND ";
				}
				return string.Concat(new object[]
				{
					"[",
					base.GetType().Name,
					": ",
					this.a,
					op,
					this.b,
					"]"
				});
			}
		}

		public class InterfaceExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				bool result;
				if (o is string)
				{
					result = true;
				}
				else
				{
					IClass c = o as IClass;
					result = (c != null && c.ClassType == ClassType.Interface);
				}
				return result;
			}

			public override string ToString()
			{
				return "[" + base.GetType().Name + "]";
			}
		}

		protected bool readOnly = true;

		private object suggestedItem;

		public static ExpressionContext Default = new ExpressionContext.DefaultExpressionContext();

		public static ExpressionContext Namespace = new ExpressionContext.ImportableExpressionContext(false);

		public static ExpressionContext Importable = new ExpressionContext.ImportableExpressionContext(true);

		public static ExpressionContext Type = new ExpressionContext.TypeExpressionContext(null, false, true);

		public static ExpressionContext ObjectCreation = new ExpressionContext.TypeExpressionContext(null, true, true);

		public static ExpressionContext.InterfaceExpressionContext Interface = new ExpressionContext.InterfaceExpressionContext();

		public virtual bool IsObjectCreation
		{
			get
			{
				return false;
			}
			set
			{
				if (value)
				{
					throw new NotSupportedException();
				}
			}
		}

		public object SuggestedItem
		{
			get
			{
				return this.suggestedItem;
			}
			set
			{
				if (this.readOnly)
				{
					throw new NotSupportedException();
				}
				this.suggestedItem = value;
			}
		}

		public virtual bool IsAttributeContext
		{
			get
			{
				return false;
			}
		}

		public abstract bool ShowEntry(object o);

		public static ExpressionContext GetAttribute(IProjectContent projectContent)
		{
			return new ExpressionContext.TypeExpressionContext(projectContent.GetClass("System.Attribute"), false, true);
		}

		public static ExpressionContext TypeDerivingFrom(IClass baseClass, bool isObjectCreation)
		{
			return new ExpressionContext.TypeExpressionContext(baseClass, isObjectCreation, false);
		}

		public static ExpressionContext operator |(ExpressionContext a, ExpressionContext b)
		{
			return new ExpressionContext.CombinedExpressionContext(0, a, b);
		}

		public static ExpressionContext operator &(ExpressionContext a, ExpressionContext b)
		{
			return new ExpressionContext.CombinedExpressionContext(1, a, b);
		}

		public static ExpressionContext operator ^(ExpressionContext a, ExpressionContext b)
		{
			return new ExpressionContext.CombinedExpressionContext(2, a, b);
		}
	}
}
