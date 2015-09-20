using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teal.CodeEditor.Test {
    public partial class Form1 : Form {
        public Form1() {
        }

        protected override void OnLoad(EventArgs e) {

            CodeEditor ce = new CodeEditor();

            ce.Dock = DockStyle.Fill;

            ce.document.lines.add(new DocumentLine(DocumentLineFlags.newLineTypeWin, "aaaa"));
            ce.document.lines[0].segments.add(new SegmentSplitter() {
                type = null,
                index = ce.document.lines[0].textLength
            });

            Controls.Add(ce);

        }

        private void Form1_Load(object sender, EventArgs e) {

        }
    }
}
