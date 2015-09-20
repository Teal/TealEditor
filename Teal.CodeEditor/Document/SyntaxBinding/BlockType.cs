using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码块类型。代码块可以包含多个片段。
    /// </summary>
    public abstract class BlockType : SegmentType {

        /// <summary>
        /// 当前块的结束模式表达式。
        /// </summary>
        public Pettern end;

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public abstract bool isMultiLine { get; }

        /// <summary>
        /// 获取或设置当前片段类型的子片段类型。
        /// </summary>
        public SegmentType[] children;

        /// <summary>
        /// 判断当前片段类型是否是块。
        /// </summary>
        public sealed override bool isBlock => true;

        protected BlockType(string name, Pettern start, Pettern end, SegmentType[] children = null)
                : base(name, start) {
            this.end = end;
            this.children = children;
        }

    }

    /// <summary>
    /// 表示一个块级类型。
    /// </summary>
    public sealed class MultiLineBlockType : BlockType {

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public override bool isMultiLine => true;

        public MultiLineBlockType(string name, Pettern start, Pettern end, SegmentType[] children = null)
                : base(name, start, end, children) {

        }

    }

    /// <summary>
    /// 表示一个内联块级类型。
    /// </summary>
    public sealed class SingleLineSegmentSegmentType : BlockType {

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public override bool isMultiLine => false;

        public SingleLineSegmentSegmentType(string name, Pettern start, Pettern end, SegmentType[] children = null)
                : base(name, start, end, children) {

        }

    }

}
