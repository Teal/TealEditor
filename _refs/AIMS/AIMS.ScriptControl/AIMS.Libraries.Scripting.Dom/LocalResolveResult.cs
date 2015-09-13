using System;

namespace AIMS.Libraries.Scripting.Dom
{
	public class LocalResolveResult : ResolveResult
	{
		private IField field;

		private bool isParameter;

		public IField Field
		{
			get
			{
				return this.field;
			}
		}

		public bool IsParameter
		{
			get
			{
				return this.isParameter;
			}
		}

		public LocalResolveResult(IMember callingMember, IField field) : base(callingMember.DeclaringType, callingMember, field.ReturnType)
		{
			if (callingMember == null)
			{
				throw new ArgumentNullException("callingMember");
			}
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			this.field = field;
			this.isParameter = field.IsParameter;
			if (!this.isParameter && !field.IsLocalVariable)
			{
				throw new ArgumentException("the field must either be a local variable-field or a parameter-field");
			}
		}

		public override FilePosition GetDefinitionPosition()
		{
			ICompilationUnit cu = base.CallingClass.CompilationUnit;
			FilePosition result;
			if (cu == null)
			{
				result = FilePosition.Empty;
			}
			else if (cu.FileName == null || cu.FileName.Length == 0)
			{
				result = FilePosition.Empty;
			}
			else
			{
				DomRegion reg = this.field.Region;
				if (!reg.IsEmpty)
				{
					result = new FilePosition(cu.FileName, reg.BeginLine, reg.BeginColumn);
				}
				else
				{
					LoggingService.Warn("GetDefinitionPosition: field.Region is empty");
					result = new FilePosition(cu.FileName);
				}
			}
			return result;
		}
	}
}
