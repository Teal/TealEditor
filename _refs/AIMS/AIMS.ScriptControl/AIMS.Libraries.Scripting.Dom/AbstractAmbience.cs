using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public abstract class AbstractAmbience : IAmbience
	{
		private ConversionFlags conversionFlags = ConversionFlags.StandardConversionFlags;

		public ConversionFlags ConversionFlags
		{
			get
			{
				return this.conversionFlags;
			}
			set
			{
				this.conversionFlags = value;
			}
		}

		public bool ShowReturnType
		{
			get
			{
				return (this.conversionFlags & ConversionFlags.ShowReturnType) == ConversionFlags.ShowReturnType;
			}
		}

		public bool ShowAccessibility
		{
			get
			{
				return (this.conversionFlags & ConversionFlags.ShowAccessibility) == ConversionFlags.ShowAccessibility;
			}
		}

		public bool ShowParameterNames
		{
			get
			{
				return (this.conversionFlags & ConversionFlags.ShowParameterNames) == ConversionFlags.ShowParameterNames;
			}
		}

		public bool UseFullyQualifiedNames
		{
			get
			{
				return (this.conversionFlags & ConversionFlags.UseFullyQualifiedNames) == ConversionFlags.UseFullyQualifiedNames;
			}
		}

		public bool ShowModifiers
		{
			get
			{
				return (this.conversionFlags & ConversionFlags.ShowModifiers) == ConversionFlags.ShowModifiers;
			}
		}

		public bool ShowInheritanceList
		{
			get
			{
				return (this.conversionFlags & ConversionFlags.ShowInheritanceList) == ConversionFlags.ShowInheritanceList;
			}
		}

		public bool IncludeHTMLMarkup
		{
			get
			{
				return (this.conversionFlags & ConversionFlags.IncludeHTMLMarkup) == ConversionFlags.IncludeHTMLMarkup;
			}
		}

		public bool UseFullyQualifiedMemberNames
		{
			get
			{
				return this.UseFullyQualifiedNames && (this.conversionFlags & ConversionFlags.QualifiedNamesOnlyForReturnTypes) != ConversionFlags.QualifiedNamesOnlyForReturnTypes;
			}
		}

		public bool IncludeBodies
		{
			get
			{
				return (this.conversionFlags & ConversionFlags.IncludeBodies) == ConversionFlags.IncludeBodies;
			}
		}

		public abstract string Convert(ModifierEnum modifier);

		public abstract string Convert(IClass c);

		public abstract string ConvertEnd(IClass c);

		public abstract string Convert(IField c);

		public abstract string Convert(IProperty property);

		public abstract string Convert(IEvent e);

		public abstract string Convert(IMethod m);

		public abstract string ConvertEnd(IMethod m);

		public abstract string Convert(IParameter param);

		public abstract string Convert(IReturnType returnType);

		public abstract string WrapAttribute(string attribute);

		public abstract string WrapComment(string comment);

		public abstract string GetIntrinsicTypeName(string dotNetTypeName);
	}
}
