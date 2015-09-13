using System;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public class CodeGeneratorOptions
	{
		public bool BracesOnSameLine = true;

		public bool EmptyLinesBetweenMembers = true;

		private string indentString = "\t";

		public string IndentString
		{
			get
			{
				return this.indentString;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("value");
				}
				this.indentString = value;
			}
		}
	}
}
