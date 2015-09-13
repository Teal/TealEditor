using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor;

namespace Teal.CodeEditor.Test {
    public partial class Form1 : Form {
        public Form1() {
            CodeEditorControl editor = new CodeEditorControl();
            editor.Dock = DockStyle.Fill;
            editor.ScrollBars = ScrollBars.Vertical;
            editor.SmoothScroll = true;
            Controls.Add(editor);
        }
    }
}
