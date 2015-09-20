using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个可以撤销的操作。
    /// </summary>
    public abstract class UndoableOperation {

        private int _oldLine;
        private int _oldColumn;

        /// <summary>
        /// 获取前一个撤销操作。
        /// </summary>
        public UndoableOperation prev;

        /// <summary>
        /// 获取当前操作前的行号。
        /// </summary>
        public int oldLine {
            get {
                return _oldLine;
            }
        }

        /// <summary>
        /// 获取当前操作前的列号。
        /// </summary>
        public int oldColumn {
            get {
                return _oldColumn;
            }
        }

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public abstract int newLine {
            get;
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public abstract int newColumn {
            get;
        }

        protected UndoableOperation(int oldLine, int oldColumn) {
            _oldLine = oldLine;
            _oldColumn = oldColumn;
        }

        /// <summary>
        /// 判断当前操作是否可以和指定操作同时执行。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        /// <param name="op">要判断的操作。</param>
        /// <returns>如果可以同时执行则返回 true，否则返回 false。</returns>
        public virtual bool canTrain(Document document, UndoableOperation op) {
            return false;
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public abstract void undo(Document document);

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public abstract void redo(Document document);

    }

    #region 字符操作

    /// <summary>
    /// 表示一个单个字符的操作。
    /// </summary>
    abstract class CharOperation : UndoableOperation {

        private char _value;

        /// <summary>
        /// 获取当前操作的字符
        /// </summary>
        public char value {
            get {
                return _value;
            }
        }

        protected CharOperation(int oldLine, int oldColumn, char value)
            : base(oldLine, oldColumn) {
            _value = value;
        }

    }

    /// <summary>
    /// 表示删除左边字符的操作。
    /// </summary>
    sealed class BackCharOperation : CharOperation {

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public override int newLine {
            get {
                return oldLine;
            }
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public override int newColumn {
            get {
                return oldColumn - 1;
            }
        }

        public BackCharOperation(int oldLine, int oldColumn, char value)
            : base(oldLine, oldColumn, value) {
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            editor.document.insert(newLine, newColumn, value);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            editor.document.delete(oldLine, oldColumn);
        }

    }

    /// <summary>
    /// 表示插入字符的操作。
    /// </summary>
    sealed class InsertCharOperation : CharOperation {

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public override int newLine {
            get {
                return oldLine;
            }
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public override int newColumn {
            get {
                return oldColumn + 1;
            }
        }

        public InsertCharOperation(int oldLine, int oldColumn, char value)
            : base(oldLine, oldColumn, value) {

        }

        /// <summary>
        /// 判断当前操作是否可以和指定操作同时执行。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        /// <param name="op">要判断的操作。</param>
        /// <returns>如果可以同时执行则返回 true，否则返回 false。</returns>
        public override bool canTrain(Document document, UndoableOperation op) {
            return op is InsertCharOperation &&
                editor.synataxBinding.getCharacterType(value) == editor.synataxBinding.getCharacterType(((InsertCharOperation)op).value) &&
                op.oldColumn == newColumn &&
                op.oldLine == newLine;
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            document.delete(newLine, newColumn);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            document.insert(oldLine, oldColumn, value, false);
        }

    }

    /// <summary>
    /// 表示删除右边字符的操作。
    /// </summary>
    sealed class DeleteCharOperation : CharOperation {

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public override int newLine {
            get {
                return oldLine;
            }
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public override int newColumn {
            get {
                return oldColumn - 1;
            }
        }

        public DeleteCharOperation(int oldLine, int oldColumn, char value)
            : base(oldLine, oldColumn, value) {

        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            editor.document.insert(newLine, newColumn, value);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            editor.document.delete(oldLine, oldColumn);
        }

    }

    #endregion

    #region 块操作

    /// <summary>
    /// 表示插入字符串的操作。
    /// </summary>
    sealed class InsertBlockOperation : UndoableOperation {

        private int _newEndLine, _newEndColumn;

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public override int newLine {
            get {
                return _newEndLine;
            }
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public override int newColumn {
            get {
                return _newEndColumn;
            }
        }

        /// <summary>
        /// 获取当前插入的字符
        /// </summary>
        public string _newValue;

        /// <summary>
        /// 获取替换的字符串。
        /// </summary>
        public string newValue {
            get {
                return _newValue;
            }
        }

        /// <summary>
        /// 初始化 <see cref="InsertBlockOperation"/> 类的新实例。
        /// </summary>
        /// <param name="caretLine">插入之前的光标行。</param>
        /// <param name="caretColumn">插入之前的光标列。</param>
        /// <param name="newEndLine">插入的新结束行。</param>
        /// <param name="newEndColumn">插入的新结束列。</param>
        /// <param name="newValue">插入的新字符串。</param>
        public InsertBlockOperation(int caretLine, int caretColumn, int newEndLine, int newEndColumn, string newValue)
            : base(caretLine, caretColumn) {
            _newEndLine = newEndLine;
            _newEndColumn = newEndColumn;
            _newValue = newValue;
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            editor.document.delete(oldLine, oldColumn, newLine, newColumn);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            editor.document.insert(oldLine, oldColumn, _newValue);
        }

    }

    /// <summary>
    /// 表示删除字符串的操作。
    /// </summary>
    sealed class DeleteBlockOperation : UndoableOperation {

        private int _oldStartLine, _oldStartColumn, _oldEndLine, _oldEndColumn;

        private string _oldValue;

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public override int newLine {
            get {
                return oldStartLine;
            }
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public override int newColumn {
            get {
                return oldStartColumn;
            }
        }

        public int oldStartLine {
            get {
                return _oldStartLine;
            }
        }

        public int oldStartColumn {
            get {
                return _oldStartColumn;
            }
        }

        public int oldEndLine {
            get {
                return _oldEndLine;
            }
        }

        public int oldEndColumn {
            get {
                return _oldEndColumn;
            }
        }

        /// <summary>
        /// 获取删除的字符串。
        /// </summary>
        public string oldValue {
            get {
                return _oldValue;
            }
        }

        /// <summary>
        /// 初始化 <see cref="DeleteBlockOperation"/> 类的新实例。
        /// </summary>
        /// <param name="caretLine">替换之前的光标行。</param>
        /// <param name="caretColumn">替换之前的光标列。</param>
        /// <param name="oldStartLine">替换的起始行。</param>
        /// <param name="oldStartColumn">替换的起始列。</param>
        /// <param name="oldEndLine">替换的结束行。</param>
        /// <param name="oldEndColumn">替换的结束列。</param>
        /// <param name="oldValue">替换的字符串。</param>
        public DeleteBlockOperation(int caretLine, int caretColumn, int oldStartLine, int oldStartColumn, int oldEndLine, int oldEndColumn, string oldValue)
            : base(caretLine, caretColumn) {
            _oldStartLine = oldStartLine;
            _oldStartColumn = oldStartColumn;
            _oldEndLine = oldEndLine;
            _oldEndColumn = oldEndColumn;
            _oldValue = oldValue;
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            editor.document.insert(newLine, newColumn, oldValue);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            editor.document.delete(oldStartLine, oldStartColumn, oldEndLine, oldEndColumn);
        }

    }

    /// <summary>
    /// 表示替换多行字符串的操作。
    /// </summary>
    sealed class ReplaceBlockOperation : UndoableOperation {

        private int _oldStartLine, _oldStartColumn, _oldEndLine, _oldEndColumn;

        private int _newEndLine, _newEndColumn;

        private string _oldValue, _newValue;

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public override int newLine {
            get {
                return _newEndLine;
            }
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public override int newColumn {
            get {
                return _newEndColumn;
            }
        }

        public int oldStartLine {
            get {
                return _oldStartLine;
            }
        }

        public int oldStartColumn {
            get {
                return _oldStartColumn;
            }
        }

        public int oldEndLine {
            get {
                return _oldEndLine;
            }
        }

        public int oldEndColumn {
            get {
                return _oldEndColumn;
            }
        }

        public int newStartLine {
            get {
                return _oldStartLine;
            }
        }

        public int newStartColumn {
            get {
                return _oldStartColumn;
            }
        }

        public int newEndLine {
            get {
                return _newEndLine;
            }
        }

        public int newEndColumn {
            get {
                return _newEndColumn;
            }
        }

        /// <summary>
        /// 获取之前的字符串。
        /// </summary>
        public string oldValue {
            get {
                return _oldValue;
            }
        }

        /// <summary>
        /// 获取替换的字符串。
        /// </summary>
        public string newValue {
            get {
                return _newValue;
            }
        }

        /// <summary>
        /// 初始化 <see cref="ReplaceBlockOperation"/> 类的新实例。
        /// </summary>
        /// <param name="caretLine">替换之前的光标行。</param>
        /// <param name="caretColumn">替换之前的光标列。</param>
        /// <param name="oldStartLine">替换的起始行。</param>
        /// <param name="oldStartColumn">替换的起始列。</param>
        /// <param name="oldEndLine">替换的结束行。</param>
        /// <param name="oldEndColumn">替换的结束列。</param>
        /// <param name="oldValue">替换的字符串。</param>
        /// <param name="newEndLine">替换的新结束行。</param>
        /// <param name="newEndColumn">替换的新结束列。</param>
        /// <param name="newValue">替换的新字符串。</param>
        public ReplaceBlockOperation(int caretLine, int caretColumn, int oldStartLine, int oldStartColumn, int oldEndLine, int oldEndColumn, string oldValue, int newEndLine, int newEndColumn, string newValue)
            : base(caretLine, caretColumn) {
            _oldStartLine = oldStartLine;
            _oldStartColumn = oldStartColumn;
            _oldEndLine = oldEndLine;
            _oldEndColumn = oldEndColumn;
            _oldValue = oldValue;

            _newEndLine = newEndLine;
            _newEndColumn = newEndColumn;
            _newValue = newValue;
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            editor.document.replace(newStartLine, newStartColumn, newEndLine, newEndColumn, oldValue);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            editor.document.replace(oldStartLine, oldStartColumn, oldEndLine, oldEndColumn, newValue);
        }

    }

    #endregion

    #region 换行操作

    /// <summary>
    /// 表示分行的操作。
    /// </summary>
    sealed class BreakLineOperation : UndoableOperation {

        public int _newColumn;

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public override int newLine {
            get {
                return newLine + 1;
            }
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public override int newColumn {
            get {
                return _newColumn;
            }
        }

        /// <summary>
        /// 初始化 <see cref="BreakLineOperation"/> 类的新实例。
        /// </summary>
        /// <param name="oldLine">The old line.</param>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        public BreakLineOperation(int oldLine, int oldColumn, int newColumn)
            : base(oldLine, oldColumn) {
            _newColumn = newColumn;
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            editor.document.unbreakLine(newLine, newColumn);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            editor.document.breakLine(oldLine, oldColumn, newColumn > 0);
        }

    }

    /// <summary>
    /// 表示合并行的操作。
    /// </summary>
    sealed class UnbreakLineUndoableOperation : UndoableOperation {

        private DocumentLineFlags _newLineType;

        private int _newColumn;

        /// <summary>
        /// 获取当前操作后的行号。
        /// </summary>
        public override int newLine {
            get {
                return oldLine - 1;
            }
        }

        /// <summary>
        /// 获取当前操作后的列号。
        /// </summary>
        public override int newColumn {
            get {
                return _newColumn;
            }
        }

        public UnbreakLineUndoableOperation(int oldLine, int oldColumn, int newLine, int newColumn, DocumentLineFlags newLineType)
            : base(oldLine, oldColumn) {
            _newColumn = newColumn;
            _newLineType = newLineType;
        }

        /// <summary>
        /// 对指定编辑器执行当前的撤销操作。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        public override void undo(Document document) {
            editor.document.breakLine(newLine, newColumn, false, _newLineType);
        }

        /// <summary>
        /// 对指定编辑器执行当前的恢复操作。
        /// </summary>
        /// <param name="document">要恢复的文档。</param>
        public override void redo(Document document) {
            editor.document.unbreakLine(oldLine);
        }



    }

    #endregion

    ///// <summary>
    ///// 表示移动光标的撤销操作。
    ///// </summary>
    //sealed class MoveCaretOperation : UndoableOperation {

    //    /// <summary>
    //    /// 判断当前操作是否可以和指定操作同时执行。
    //    /// </summary>
    //    /// <param name="document">要撤销的文档。</param>
    //    /// <param name="op">要判断的操作。</param>
    //    /// <returns>如果可以同时执行则返回 true，否则返回 false。</returns>
    //    public override bool canTrain(Document document, UndoableOperation op) {
    //        return true;
    //    }

    //    /// <summary>
    //    /// 对指定编辑器执行当前的撤销操作。
    //    /// </summary>
    //    /// <param name="document">要撤销的文档。</param>
    //    public override void undo(Document document) {
    //        editor.setCaretLocation(oldLine, oldColumn);
    //    }

    //    /// <summary>
    //    /// 对指定编辑器执行当前的恢复操作。
    //    /// </summary>
    //    /// <param name="document">要恢复的文档。</param>
    //    public override void redo(Document document) {
    //        editor.setCaretLocation(newLine, newColumn);
    //    }

    //}

    ///// <summary>
    ///// 表示插入字符的撤销操作。
    ///// </summary>
    //sealed class ReplaceCharUndoableOperation : UndoableOperation {

    //    /// <summary>
    //    /// 获取之前的字符。
    //    /// </summary>
    //    public char oldValue;

    //    /// <summary>
    //    /// 获取替换的字符。
    //    /// </summary>
    //    public char newValue;

    //    /// <summary>
    //    /// 判断当前操作是否可以和指定操作同时执行。
    //    /// </summary>
    //    /// <param name="document">要撤销的文档。</param>
    //    /// <param name="op">要判断的操作。</param>
    //    /// <returns>如果可以同时执行则返回 true，否则返回 false。</returns>
    //    public override bool canTrain(Document document, UndoableOperation op) {
    //        return op is ReplaceCharUndoableOperation && !editor.isDelimiter(((ReplaceCharUndoableOperation)op).newValue);
    //    }

    //    /// <summary>
    //    /// 对指定编辑器执行当前的撤销操作。
    //    /// </summary>
    //    /// <param name="document">要撤销的文档。</param>
    //    public override void undo(Document document) {
    //        editor.document.replaceChar(oldLine, oldColumn, oldValue);
    //    }

    //    /// <summary>
    //    /// 对指定编辑器执行当前的恢复操作。
    //    /// </summary>
    //    /// <param name="document">要恢复的文档。</param>
    //    public override void redo(Document document) {
    //        editor.document.replaceChar(oldLine, oldColumn, newValue);
    //    }

    //}





}
