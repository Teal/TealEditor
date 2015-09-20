using System.Diagnostics;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个布局中断点。
    /// </summary>
    abstract class LayoutBreakPoint {

        /// <summary>
        /// 获取同个布局行的下一个中断点。
        /// </summary>
        public LayoutBreakPoint next;

        /// <summary>
        /// 获取当前中断点的起始列。
        /// </summary>
        public abstract int startColumn {
            get;
        }

        /// <summary>
        /// 判断当前中断点是否是自动换行中断点。
        /// </summary>
        public abstract bool isWrapPoint {
            get;
        }

    }

    /// <summary>
    /// 表示一个布局行。
    /// </summary>
    [DebuggerDisplay("line top={top}")]
    sealed class LayoutLine : LayoutBreakPoint {

        /// <summary>
        /// 获取当前布局行第一个字符的垂直坐标。如果当前布局行被隐藏，则返回 -1。
        /// </summary>
        public int top;

        /// <summary>
        /// 获取当前布局行最后一个字符的右边水平坐标。
        /// </summary>
        public int right;

        /// <summary>
        /// 获取当前中断点的起始列。
        /// </summary>
        public override int startColumn {
            get {
                return 0;
            }
        }

        /// <summary>
        /// 判断当前中断点是否是自动换行中断点。
        /// </summary>
        public override bool isWrapPoint {
            get {
                return false;
            }
        }

    }

    /// <summary>
    /// 表示一个自动换行中断点。
    /// </summary>
    [DebuggerDisplay("wrap column={column}")]
    sealed class LayoutWrapPoint : LayoutBreakPoint {

        /// <summary>
        /// 获取自动换行的列。
        /// </summary>
        public readonly int column;

        public LayoutWrapPoint(int column) {
            this.column = column;
        }

        ///// <summary>
        ///// 获取当前行自动换行的新行缩进。
        ///// </summary>
        //public int indent;

        /// <summary>
        /// 获取当前中断点的起始列。
        /// </summary>
        public override int startColumn {
            get {
                return column;
            }
        }

        /// <summary>
        /// 判断当前中断点是否是自动换行中断点。
        /// </summary>
        public override bool isWrapPoint {
            get {
                return true;
            }
        }

    }

    /// <summary>
    /// 表示一个已折叠的折叠块。
    /// </summary>
    [DebuggerDisplay("fold range={foldingRange}")]
    sealed class LayoutFoldingBlock : LayoutBreakPoint {

        /// <summary>
        /// 获取当前的折叠域。
        /// </summary>
        public readonly FoldingRange foldingRange;

        public LayoutFoldingBlock(FoldingRange foldingRange) {
            this.foldingRange = foldingRange;
        }

        /// <summary>
        /// 获取当前中断点的起始列。
        /// </summary>
        public override int startColumn {
            get {
                return foldingRange.startColumn;
            }
        }

        /// <summary>
        /// 判断当前中断点是否是自动换行中断点。
        /// </summary>
        public override bool isWrapPoint {
            get {
                return false;
            }
        }

    }

}
