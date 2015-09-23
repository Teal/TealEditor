using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码块类型。代码块可以包含多个片段。
    /// </summary>
    public class BlockType : SegmentType {

        /// <summary>
        /// 当前块的结束模式表达式。
        /// </summary>
        public Pattern end;

        /// <summary>
        /// 获取或设置当前片段类型的子片段类型。
        /// </summary>
        public SegmentType[] children;

        /// <summary>
        /// 判断当前片段类型是否是块。
        /// </summary>
        public sealed override bool isBlock => true;

        /// <summary>
        /// 判断当前块是否可折叠。
        /// </summary>
        public virtual bool canFold => false;

        public BlockType(string name, Pattern start, Pattern end)
                : base(name, start) {
            this.end = end;
        }

    }

    /// <summary>
    /// 表示一个可折叠的块级类型。
    /// </summary>
    public sealed class FoldingBlockType : BlockType {

        /// <summary>
        /// 获取当前折叠块类型折叠后的文本。
        /// </summary>
        public string foldingText;

        /// <summary>
        /// 判断当前块是否可折叠。
        /// </summary>
        public override bool canFold => true;

        public FoldingBlockType(string name, Pattern start, Pattern end, string foldingText = "...")
                : base(name, start, end) {
            this.foldingText = foldingText;
        }

    }

}
