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


            var blocks = new {

                body = new MultiLineBlockType("body", new AnyDismatchPettern(), null),

                //comment_multiLine = new MultiLineBlockType("comment_multiLine", new CaseSensitiveStringPettern("/*"), new CaseSensitiveStringPettern("*/")),

                //comment_singleLine = new SingleLineBlockType("comment_singleLine", new CaseSensitiveStringPettern("//"), null),

                string_doubleQuote = new SingleLineBlockType("string_doubleQuote", new CaseSensitiveStringPettern("\""), new CaseSensitiveStringPettern("\"")) {
                    foreColor = 0x00FF0000
                },

                string_singleQuote = new SingleLineBlockType("string_singleQuote", new CaseSensitiveStringPettern("\'"), new CaseSensitiveStringPettern("\'")) {
                    foreColor = 0x0000FF00
                },

                string_escapedChar = new SegmentType("string_escapedChar", new RegexPettern(@"\\((u[\da-fA-F]{4})|(x[\da-fA-F]{2})|.)")) {
                    foreColor = 0x000000FF
                },

            };

            blocks.body.children = new SegmentType[] {
               blocks. string_doubleQuote,
               blocks. string_singleQuote
            };

            blocks.string_singleQuote.children = blocks.string_doubleQuote.children = new SegmentType[] {
                 blocks.string_escapedChar
            };


            ce.document.syntaxBinding.rootBlock = new Block(null, blocks.body, null, 0);

            ce.document.insert(0, 0, "a'b'c\"cc\\\"c\"d");
            //ce.document.lines[0].segments.add(new SegmentSplitter() {
            //    type = null,
            //    index = ce.document.lines[0].textLength
            //});

            Controls.Add(ce);

        }

        private void Form1_Load(object sender, EventArgs e) {

        }
    }
}
