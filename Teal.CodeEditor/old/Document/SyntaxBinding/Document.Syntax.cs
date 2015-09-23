using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed partial class Document {

        /// <summary>
        /// 获取或设置当前文档的语法绑定。
        /// </summary>
        public SyntaxBinding syntaxBinding = new SyntaxBinding();

        /// <summary>
        /// 从指定行开始解析行的片段。
        /// </summary>
        /// <param name="startLine"></param>
        private void parseSegments(int startLine) {

            // 从 startLine 到行尾依次解析，如果每一行

            // 获取上一行结束后的块。
            var lastLineEndingBlock = startLine == 0 ? syntaxBinding.rootBlock : lines[startLine - 1].endingBlock;

            for (; startLine < lines.length; startLine++) {

                var documentLine = lines[startLine];

                // 如果当前行未更新且当前行的初始块未发生变化，则无需重新解析。
                if (documentLine.parsed && documentLine.startingBlock == lastLineEndingBlock) {
                    break;
                }

                // 解析行。
                lastLineEndingBlock = documentLine.parseSegments(lastLineEndingBlock);
            }
        }

    }
}
