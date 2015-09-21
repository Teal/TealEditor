namespace Teal.CodeEditor {
    /// <summary>
    /// 表示分行的操作。
    /// </summary>
    sealed class BreakLineOperation : UndoableOperation {

        /// <summary>
        /// 初始化 <see cref="BreakLineOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        public BreakLineOperation(int line, int column)
            : base(line, column) {
            
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            document.unbreakLine(line, column);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            document.breakLine(line, column);
        }

    }

    /// <summary>
    /// 表示合并行的操作。
    /// </summary>
    sealed class UnbreakLineOperation : UndoableOperation {

        /// <summary>
        /// 初始化 <see cref="UnbreakLineOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        public UnbreakLineOperation(int line, int column)
                : base(line, column) {

        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            document.breakLine(line, column);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            document.unbreakLine(line, column);
        }

    }

}