using AIMS.Libraries.Scripting.Engine;
using AIMS.Libraries.Scripting.ScriptControl;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Test
{
	public class Main : Form
	{
		private IContainer components = null;

		private ScriptControl scriptControl1;

		private Engine engine1;

		public Main()
		{
			this.InitializeComponent();
			this.scriptControl1.AddObject("Container", this);
			this.scriptControl1.StartEditor();
			this.scriptControl1.Execute += new EventHandler(this.scriptControl1_Execute);
		}

		private void scriptControl1_Execute(object sender, EventArgs e)
		{
			try
			{
				this.engine1.OutputAssemblyName = this.scriptControl1.OutputAssemblyName;
				this.engine1.StartMethodName = this.scriptControl1.StartMethodName;
				this.engine1.DefaultNameSpace = this.scriptControl1.DefaultNameSpace;
				this.engine1.RemoteVariables = this.scriptControl1.RemoteVariables;
				this.engine1.DefaultClassName = this.scriptControl1.DefaultClassName;
				object ret = this.engine1.Execute(null);
				MessageBox.Show("Return Code : " + ret.ToString());
			}
			catch
			{
			}
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
			this.components = new Container();
			this.scriptControl1 = new ScriptControl();
			this.engine1 = new Engine(this.components);
			base.SuspendLayout();
			this.scriptControl1.Dock = DockStyle.Fill;
			this.scriptControl1.Location = new Point(0, 0);
			this.scriptControl1.Name = "scriptControl1";
			this.scriptControl1.ScriptLanguage = ScriptLanguage.CSharp;
			this.scriptControl1.Size = new Size(639, 428);
			this.scriptControl1.TabIndex = 0;
			this.scriptControl1.Execute += new EventHandler(this.scriptControl1_Execute);
			this.engine1.DefaultClassName = "";
			this.engine1.DefaultNameSpace = "";
			this.engine1.OutputAssemblyName = "";
			this.engine1.RemoteVariables = null;
			this.engine1.StartMethodName = "";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(639, 428);
			base.Controls.Add(this.scriptControl1);
			this.DoubleBuffered = true;
			base.Name = "Main";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = ".Net Script Control VSA Replacement : Rajneesh Noonia";
			base.ResumeLayout(false);
		}
	}
}
