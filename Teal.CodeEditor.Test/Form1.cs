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

                body = new BlockType("body", Pattern.starting, Pattern.none),

                comment_multiLine = new BlockType("comment_multiLine", new CaseSensitiveStringPattern("/*"), new CaseSensitiveStringPattern("*/")) {
                    foreColor = 0x00FF00F0
                },

                comment_singleLine = new BlockType("comment_singleLine", new CaseSensitiveStringPattern("//"), Pattern.ending) {
                    foreColor = 0x0023677D
                },

                string_doubleQuote = new BlockType("string_doubleQuote", new CaseSensitiveStringPattern("\""), new CaseSensitiveStringPattern("\"")) {
                    foreColor = 0x00FF0000
                },

                string_singleQuote = new BlockType("string_singleQuote", new CaseSensitiveStringPattern("\'"), new CaseSensitiveStringPattern("\'")) {
                    foreColor = 0x0000FF00
                },

                string_escapedChar = new SegmentType("string_escapedChar", new RegexPattern(@"\\((u[\da-fA-F]{4})|(x[\da-fA-F]{2})|.)")) {
                    foreColor = 0x000000FF
                },

            };

            blocks.body.children = new SegmentType[] {
                blocks.comment_multiLine,
                blocks.comment_singleLine,
               blocks. string_doubleQuote,
               blocks. string_singleQuote
            };

            blocks.string_singleQuote.children = blocks.string_doubleQuote.children = new SegmentType[] {
                 blocks.string_escapedChar
            };


            ce.document.syntaxBinding.rootBlock = new Block(null, blocks.body, null, 0);

            ce.document.insert(0, 0,"affgujgjhkjhnkjk\t" + @"a'b'c""cc\""c""d ghuihik hkj/*
sasas
  // aaaa
 */    asasss // asdasd");
            //ce.document.lines[0].segmentSplitters.add(new SegmentSplitter() {
            //    type = null,
            //    index = ce.document.lines[0].textLength
            //});

         //   ce.document.load("../Teal.CodeEditor.Test/a.txt", Encoding.UTF8);

            Controls.Add(ce);

        }

        private void Form1_Load(object sender, EventArgs e) {

        }
    }
}
