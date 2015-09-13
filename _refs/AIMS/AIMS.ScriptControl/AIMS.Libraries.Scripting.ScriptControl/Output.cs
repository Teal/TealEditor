using AIMS.Libraries.Forms.Docking;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class Output : DockableWindow
	{
		private IContainer components = null;

		private TextBox txtOutput;

		public Output()
		{
			this.InitializeComponent();
		}

		public void AppendLine(string msg)
		{
			TextBox expr_07 = this.txtOutput;
			expr_07.Text = expr_07.Text + msg + Environment.NewLine;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager resources = new ComponentResourceManager(typeof(Output));
			this.txtOutput = new TextBox();
			base.SuspendLayout();
			this.txtOutput.BorderStyle = BorderStyle.None;
			this.txtOutput.Dock = DockStyle.Fill;
			this.txtOutput.Location = new Point(0, 0);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.Size = new Size(292, 273);
			this.txtOutput.TabIndex = 0;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(292, 273);
			base.Controls.Add(this.txtOutput);
			base.Icon = (Icon)resources.GetObject("$this.Icon");
			base.Name = "Output";
			base.TabText = "Output";
			this.Text = "Output";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
