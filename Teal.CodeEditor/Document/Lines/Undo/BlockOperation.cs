namespace Teal.CodeEditor {
    /// <summary>
    /// 表示一个多行字符串的操作。
    /// </summary>
    abstract class BlockOperation : UndoableOperation {

        /// <summary>
        /// 获取当前操作的字符串。
        /// </summary>
        public readonly string value;

        /// <summary>
        /// 初始化 <see cref="BlockOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        /// <param name="value">当前操作的字符串。</param>
        protected BlockOperation(int line, int column, string value)
            : base(line, column) {
            this.value = value;
        }

    }

    /// <summary>
    /// 表示插入字符串的操作。
    /// </summary>
    sealed class InsertBlockOperation : BlockOperation {

        /// <summary>
        /// 初始化 <see cref="InsertBlockOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        /// <param name="value">当前操作的字符串。</param>
        public InsertBlockOperation(int line, int column, string value)
            : base(line, column, value) {

        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            var location = Document.getLocation(line, column, value);
            document.delete(line, column, location.line, location.column);
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
    /// 表示删除字符串的操作。
    /// </summary>
    sealed class DeleteBlockOperation : BlockOperation {

        /// <summary>
        /// 初始化 <see cref="DeleteBlockOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        /// <param name="value">当前操作的字符串。</param>
        public DeleteBlockOperation(int line, int column, string value)
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
            var location = Document.getLocation(line, column, value);
            document.delete(line, column, location.line, location.column);
        }

    }

    /// <summary>
    /// 表示替换多行字符串的操作。
    /// </summary>
    sealed class ReplaceBlockOperation : UndoableOperation {

        /// <summary>
        /// 获取之前的字符串。
        /// </summary>
        public readonly string oldValue;

        /// <summary>
        /// 获取替换的字符串。
        /// </summary>
        public readonly string newValue;

        /// <summary>
        /// 初始化 <see cref="ReplaceBlockOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        /// <param name="oldValue">替换的原字符串。</param>
        /// <param name="newValue">替换的新字符串。</param>
        public ReplaceBlockOperation(int line, int column, string oldValue, string newValue)
            : base(line, column) {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            var location = Document.getLocation(line, column, newValue);
            document.replace(line, column, location.line, location.column, oldValue);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            var location = Document.getLocation(line, column, oldValue);
            document.replace(line, column, location.line, location.column, newValue);
        }

    }

}