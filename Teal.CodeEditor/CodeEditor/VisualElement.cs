
//namespace Circus.CodeEditor {

//    /// <summary>
//    /// 表示一个显示元素。
//    /// </summary>
//    public abstract class VisualElement {

//        /// <summary>
//        /// 获取同行下一个折叠块。
//        /// </summary>
//        public VisualFoldBlock nextFoldBlock;

//        /// <summary>
//        /// 获取当前元素的左边距。
//        /// </summary>
//        public int left;

//        /// <summary>
//        /// 获取当前元素的右边距。
//        /// </summary>
//        public int right;

//    }

//    /// <summary>
//    /// 表示一个显示行。
//    /// </summary>
//    public sealed class VisualLine : VisualElement {

//        /// <summary>
//        /// 获取当前行第一个字符的实际行。
//        /// </summary>
//        public int startLine;

//        /// <summary>
//        /// 获取当前行第一个字符的实际列。
//        /// </summary>
//        public int startColumn;

//        /// <summary>
//        /// 获取当前行最后一个字符的实际行号。
//        /// </summary>
//        public int endLine {
//            get {
//                if (nextFoldBlock == null) {
//                    return startLine;
//                }
//                var foldBlock = nextFoldBlock;
//                while (foldBlock.nextFoldBlock != null) {
//                    foldBlock = foldBlock.nextFoldBlock;
//                }
//                return foldBlock.foldingRange.endLine;
//            }
//        }

//        /// <summary>
//        /// 判断当前行是否是由于自动换行产生的行。
//        /// </summary>
//        public bool isWrapLine {
//            get {
//                return startColumn != 0;
//            }
//        }

//        /// <summary>
//        /// 返回该实例的完全限定类型名。
//        /// </summary>
//        /// <returns>包含完全限定类型名的 <see cref="T:System.String"/>。</returns>
//        public override string ToString() {
//            var result = startLine + ", " + startColumn;
//            if (nextFoldBlock != null) {
//                result += " " + nextFoldBlock;
//            }
//            return result + ": " + left + " - " + right;
//        }

//        ///// <summary>
//        ///// 获取当前行的最后一个折叠块。
//        ///// </summary>
//        //public VisualFoldBlock lastFoldBlock {
//        //    get {
//        //        var foldBlock = nextFoldBlock;
//        //        if (foldBlock == null) {
//        //            return null;
//        //        }
//        //        while (foldBlock.nextFoldBlock != null) {
//        //            foldBlock = foldBlock.nextFoldBlock;
//        //        }
//        //        return foldBlock;
//        //    }
//        //}

//        /// <summary>
//        /// 获取当前行内以指定位置结尾的折叠块。
//        /// </summary>
//        /// <param name="line"></param>
//        /// <param name="column"></param>
//        /// <returns></returns>
//        public VisualFoldBlock getFoldBlockByEnd(int line, int column) {
//            for (var foldBlock = nextFoldBlock; foldBlock != null; foldBlock = foldBlock.nextFoldBlock) {
//                if (foldBlock.foldingRange.endColumn == column && foldBlock.foldingRange.endLine == line) {
//                    return foldBlock;
//                }
//            }
//            return null;
//        }

//        /// <summary>
//        /// 获取当前行内以指定位置开始的折叠块。
//        /// </summary>
//        /// <param name="line"></param>
//        /// <param name="column"></param>
//        /// <returns></returns>
//        public VisualFoldBlock getFoldBlockByStart(int line, int column) {
//            for (var foldBlock = nextFoldBlock; foldBlock != null; foldBlock = foldBlock.nextFoldBlock) {
//                if (foldBlock.foldingRange.startColumn == column && foldBlock.foldingRange.startLine == line) {
//                    return foldBlock;
//                }
//            }
//            return null;
//        }


//    }

//    /// <summary>
//    /// 表示一个显示折叠区域。
//    /// </summary>
//    public sealed class VisualFoldBlock : VisualElement {

//        /// <summary>
//        /// 获取当前折叠块对应的折叠区域。
//        /// </summary>
//        public FoldingRange foldingRange;

//        /// <summary>
//        /// 返回该实例的完全限定类型名。
//        /// </summary>
//        /// <returns>包含完全限定类型名的 <see cref="T:System.String"/>。</returns>
//        public override string ToString() {
//            var result = "[" + foldingRange + "]";
//            if (nextFoldBlock != null) {
//                result += " " + nextFoldBlock;
//            }
//            return result;
//        }

//    }

//}
