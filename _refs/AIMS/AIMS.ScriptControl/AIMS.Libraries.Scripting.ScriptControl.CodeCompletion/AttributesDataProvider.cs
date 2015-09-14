using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using System;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public class AttributesDataProvider : CtrlSpaceCompletionDataProvider
	{
		private bool removeAttributeSuffix = true;

		public bool RemoveAttributeSuffix
		{
			get
			{
				return this.removeAttributeSuffix;
			}
			set
			{
				this.removeAttributeSuffix = value;
			}
		}

		public AttributesDataProvider(IProjectContent pc) : this(ExpressionContext.TypeDerivingFrom(pc.GetClass("System.Attribute"), true))
		{
		}

		public AttributesDataProvider(ExpressionContext context) : base(context)
		{
			base.ForceNewExpression = true;
		}

		public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
		{
			ICompletionData[] data = base.GenerateCompletionData(fileName, textArea, charTyped);
			if (this.removeAttributeSuffix)
			{
				ICompletionData[] array = data;
				for (int i = 0; i < array.Length; i++)
				{
					ICompletionData d = array[i];
					if (d.Text.EndsWith("Attribute"))
					{
						d.Text = d.Text.Substring(0, d.Text.Length - 9);
					}
				}
			}
			return data;
		}
	}
}
