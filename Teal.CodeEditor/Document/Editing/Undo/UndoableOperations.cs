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

        /// <summary>
        /// 获取上一个撤销操作。
        /// </summary>
        public UndoableOperation prev;

        /// <summary>
        /// 获取当前操作的行号。
        /// </summary>
        public readonly int line;

        /// <summary>
        /// 获取当前操作的列号。
        /// </summary>
        public readonly int column;

        /// <summary>
        /// 判断当前操作是否可以和指定操作同时执行。
        /// </summary>
        /// <param name="document">要撤销的文档。</param>
        /// <param name="op">要判断的操作。</param>
        /// <returns>如果可以同时执行则返回 true，否则返回 false。</returns>
        public virtual bool canChain(Document document, UndoableOperation op) {
            return false;
        }

        /// <summary>
        /// 初始化 <see cref="UndoableOperation"/> 类的新实例。
        /// </summary>
        /// <param name="line">当前操作的行号。</param>
        /// <param name="column">当前操作的列号。</param>
        protected UndoableOperation(int line, int column) {
            this.line = line;
            this.column = column;
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

    #endregion

    #region 块操作

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

    #endregion

    #region 换行操作

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

    #endregion

    #region 选区操作



    #endregion

}
