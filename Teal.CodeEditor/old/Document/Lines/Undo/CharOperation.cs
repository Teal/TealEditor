namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个单个字符的操作。
    /// </summary>
    abstract class CharOperation : UndoableOperation {

        /// <summary>
        /// 获取当前操作的字符。
        /// </summary>
        public readonly char value;

        /// <summary>
        /// 初始化 <see cref="CharOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        /// <param name="value">当前操作的字符。</param>
        protected CharOperation(int line, int column, char value)
            : base(line, column) {
            this.value = value;
        }

    }

    /// <summary>
    /// 表示插入字符的操作。
    /// </summary>
    sealed class InsertCharOperation : CharOperation {

        /// <summary>
        /// 初始化 <see cref="InsertCharOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        /// <param name="value">当前操作的字符。</param>
        public InsertCharOperation(int line, int column, char value)
            : base(line, column, value) {
        }

        /// <summary>
        /// 判断当前操作是否可以和指定操作同时执行。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        /// <param name="op">要判断的操作。</param>
        /// <returns>如果可以同时执行则返回 true，否则返回 false。</returns>
        public override bool canChain(Document document, UndoableOperation op) {
            var other = op as InsertCharOperation;
            return other != null &&
                line == other.line &&
                column == other.column + 1 &&
                document.syntaxBinding.getCharacterType(value) == document.syntaxBinding.getCharacterType(other.value);
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            document.delete(line, column);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            document.insert(line, column, value);
        }

    }

    /// <summary>
    /// 表示删除字符的操作。
    /// </summary>
    sealed class DeleteCharOperation : CharOperation {

        /// <summary>
        /// 初始化 <see cref="DeleteCharOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        /// <param name="value">当前操作的字符。</param>
        public DeleteCharOperation(int line, int column, char value)
            : base(line, column, value) {

        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            document.insert(line, column, value);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            document.delete(line, column);
        }

    }

}