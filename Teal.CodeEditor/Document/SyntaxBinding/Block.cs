using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码块。
    /// </summary>
    public sealed class Block {

        /// <summary>
        /// 获取上一级代码块。
        /// </summary>
        public Block parent;

        /// <summary>
        /// 获取当前代码块的类型。
        /// </summary>
        public BlockType type;

        /// <summary>
        /// 获取当前代码块的起始行。
        /// </summary>
        public DocumentLine startLine;

        /// <summary>
        /// 获取当前代码块的起始列。
        /// </summary>
        public int startColumn;

        /// <summary>
        /// 获取当前代码块的结束行。
        /// </summary>
        public DocumentLine endLine;

        /// <summary>
        /// 获取当前代码块的结束列。
        /// </summary>
        public int endColumn;

        /// <summary>
        /// 判断或设置当前块的折叠状态。
        /// </summary>
        public bool collapsed;

        public Block(Block parent, BlockType type, DocumentLine startLine, int startColumn) {
            this.parent = parent;
            this.type = type;
            this.startLine = startLine;
            this.startColumn = startColumn;
        }

        public override string ToString() {
            return $"{type}";
        }
    }

}
