using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public class CompoundClass : DefaultClass
	{
		internal List<IClass> parts = new List<IClass>();

		public override IList<ITypeParameter> TypeParameters
		{
			get
			{
				IList<ITypeParameter> typeParameters;
				lock (this)
				{
					typeParameters = this.parts[0].TypeParameters;
				}
				return typeParameters;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override List<IClass> InnerClasses
		{
			get
			{
				List<IClass> result;
				lock (this)
				{
					List<IClass> i = new List<IClass>();
					foreach (IClass part in this.parts)
					{
						i.AddRange(part.InnerClasses);
					}
					result = i;
				}
				return result;
			}
		}

		public override List<IField> Fields
		{
			get
			{
				List<IField> result;
				lock (this)
				{
					List<IField> i = new List<IField>();
					foreach (IClass part in this.parts)
					{
						i.AddRange(part.Fields);
					}
					result = i;
				}
				return result;
			}
		}

		public override List<IProperty> Properties
		{
			get
			{
				List<IProperty> result;
				lock (this)
				{
					List<IProperty> i = new List<IProperty>();
					foreach (IClass part in this.parts)
					{
						i.AddRange(part.Properties);
					}
					result = i;
				}
				return result;
			}
		}

		public override List<IMethod> Methods
		{
			get
			{
				List<IMethod> result;
				lock (this)
				{
					List<IMethod> i = new List<IMethod>();
					foreach (IClass part in this.parts)
					{
						i.AddRange(part.Methods);
					}
					result = i;
				}
				return result;
			}
		}

		public override List<IEvent> Events
		{
			get
			{
				List<IEvent> result;
				lock (this)
				{
					List<IEvent> i = new List<IEvent>();
					foreach (IClass part in this.parts)
					{
						i.AddRange(part.Events);
					}
					result = i;
				}
				return result;
			}
		}

		public IList<IClass> GetParts()
		{
			IList<IClass> result;
			lock (this)
			{
				result = this.parts.ToArray();
			}
			return result;
		}

		public CompoundClass(IClass firstPart) : base(new DefaultCompilationUnit(firstPart.ProjectContent), firstPart.FullyQualifiedName)
		{
			base.CompilationUnit.Classes.Add(this);
			this.parts.Add(firstPart);
			this.UpdateInformationFromParts();
		}

		internal void UpdateInformationFromParts()
		{
			base.ClassType = this.parts[0].ClassType;
			ModifierEnum modifier = ModifierEnum.None;
			base.BaseTypes.Clear();
			base.Attributes.Clear();
			string shortestFileName = null;
			foreach (IClass part in this.parts)
			{
				if (!string.IsNullOrEmpty(part.CompilationUnit.FileName))
				{
					if (shortestFileName == null || part.CompilationUnit.FileName.Length < shortestFileName.Length)
					{
						shortestFileName = part.CompilationUnit.FileName;
						base.Region = part.Region;
					}
				}
				if ((part.Modifiers & ModifierEnum.VisibilityMask) != ModifierEnum.Internal)
				{
					modifier |= part.Modifiers;
				}
				else
				{
					modifier |= (part.Modifiers & ~(ModifierEnum.Private | ModifierEnum.Internal | ModifierEnum.Protected | ModifierEnum.Public));
				}
				foreach (IReturnType rt in part.BaseTypes)
				{
					if (!rt.IsDefaultReturnType || rt.FullyQualifiedName != "System.Object")
					{
						base.BaseTypes.Add(rt);
					}
				}
				foreach (IAttribute attribute in part.Attributes)
				{
					base.Attributes.Add(attribute);
				}
			}
			base.CompilationUnit.FileName = shortestFileName;
			if ((modifier & ModifierEnum.VisibilityMask) == ModifierEnum.None)
			{
				modifier |= ModifierEnum.Internal;
			}
			base.Modifiers = modifier;
		}

		protected override IReturnType CreateDefaultReturnType()
		{
			return new DefaultReturnType(this);
		}
	}
}
