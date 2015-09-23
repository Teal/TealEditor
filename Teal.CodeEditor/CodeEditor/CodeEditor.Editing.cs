using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        /// <summary>
        /// 执行一个插入字符操作。(默认地，按下“字母键”将执行此操作）
        /// </summary>
        /// <param name="ch">要插入的字符。</param>
        public void input(char ch) {

            _document.beiginUpdate();

            try {

                // 支持自定义事件。
                if (inputChar != null && !inputChar(ch)) {
                    return;
                }

                // 特殊字符成对插入。比如插入 ( 后自动插入 )。
                var pairChar = _document.syntaxBinding.getPairChar(ch);
                if (pairChar != '\0') {

                }

                // 存在选区则直接替换选区。
                if (_selections != null) {
                    replaceSelections(ch.ToString(), false);
                    return;
                }

                // 插入模式：插入字符。
                if (_caretMode == CaretMode.insertMode || _caretColumn >= _document.lines[_caretLine].buffer.length) {
                    _document.insert(caretLine, caretColumn, ch);
                } else {
                    _document.replace(_caretLine, _caretColumn, _caretLine, _caretColumn, _caretLine, _caretColumn + 1, ch.ToString());
                }

            } finally {
                _document.endUpdate();
            }

        }

        /// <summary>
        /// 执行一个删除操作。(默认地，按下“Delete”将执行此操作）
        /// </summary>
        public void delete() {

            //if (_selections != null) {
            //    return;
            //}

            //    deleteChar(_caretVisualLine, _caretColumn - 1);
            //backspace();
        }

        /// <summary>
        /// 执行一个删除单词操作。(默认地，按下“Ctrl+Delete”将执行此操作）
        /// </summary>
        public void deleteWord() {

        }

        /// <summary>
        /// 执行一个删除到行尾操作。(默认地，按下“Shift+Delete”将执行此操作）
        /// </summary>
        public void deleteToEnd() {

            //if (_selections != null) {
            //    return;
            //}

            //    deleteChar(_caretVisualLine, _caretColumn - 1);
            //backspace();
        }

        /// <summary>
        /// 执行一个退格键。(默认地，按下“Backspace”将执行此操作）
        /// </summary>
        public void backspace() {

            //// 1. 删除选区。
            //if (_selections != null) {
            //    replaceSelections(String.Empty, false);
            //    return;
            //}

            //var oldCaretLine = _caretLine;
            //var oldCaretColumn = _caretColumn;

            //switch (moveLeftCore()) {

            //    // 2. 现在在某字符前，删除字符。
            //    case MoveOperation.column:
            //        addUndo(new BackCharOperation(oldCaretLine, oldCaretColumn, document.lines[_caretLine].chars[_caretColumn]));
            //        document.delete(_caretLine, _caretColumn);
            //        return;

            //    // 3. 现在行首，删除整行。
            //    case MoveOperation.line:
            //        var nl = document.unbreakLine(oldCaretLine).newLine;
            //        //addUndo(new UnbreakLineUndoableOperation(oldCaretLine, oldCaretColumn, document.lines[_caretLine].chars[_caretColumn]));
            //        return;

            //    // 4. 现在在某折叠域前，删除折叠域。
            //    case MoveOperation.block:
            //        return;

            //    // 5. 现在在最开头，不操作。
            //    default:
            //        return;
            //}

            //// 其他情况。
            ////addUndo(new DeleteBlockUndoableOperation() {
            ////    line = startLine,
            ////    column = startColumn,
            ////    value = _document.getText(startLine, startColumn, endLine, endColumn),
            ////    endLocation = new Point(endLine, endColumn)
            ////});
            //_document.deleteBlock(startLine, startColumn, endLine, endColumn);

            //delete(_caretVisualLine, _caretColumn, oldCaretLine, oldCaretColumn, oldCaretLine, oldCaretColumn);

            // 有选区先撤销选区。
        }

        /// <summary>
        /// 执行一个退格键。(默认地，按下“Ctrl+Backspace”将执行此操作）
        /// </summary>
        private void backspaceWord() {

            //if (_selections != null) {
            //    replaceSelections(String.Empty, false);
            //    return;
            //}

            //var oldCaretLine = caretLine;
            //var oldCaretColumn = caretColumn;
            //moveWordLeft();
            //delete(oldCaretLine, oldCaretColumn, caretLine, caretColumn, oldCaretLine, oldCaretColumn);

        }

        /// <summary>
        /// 执行一个删除到行首操作。(默认地，按下“Shift+Backspace”将执行此操作）
        /// </summary>
        public void backToStart() {

            //if (_selections != null) {
            //    return;
            //}

            //    deleteChar(_caretVisualLine, _caretColumn - 1);
            //backspace();
        }

        /// <summary>
        /// 执行一个换行操作。(默认地，按下“回车键”将执行此操作）
        /// </summary>
        public void breakLine() {

        }

        /// <summary>
        /// 执行一个对上一行换行操作。(默认地，按下“Ctrl+回车键”将执行此操作）
        /// </summary>
        public void breakLastLine() {

        }

        /// <summary>
        /// 执行一个对当前行换行操作。(默认地，按下“Shift+回车键”将执行此操作）
        /// </summary>
        public void breakCurrentLine() {

        }

        /// <summary>
        /// 执行一个剪切操作。(默认地，按下“Ctrl+X”将执行此操作）
        /// </summary>
        public void cut() {
            Utility.mark();
        }

        /// <summary>
        /// 执行一个复制操作。(默认地，按下“Ctrl+C”将执行此操作）
        /// </summary>
        public void copy() {
            Utility.mark();
        }

        /// <summary>
        /// 执行一个粘贴操作。(默认地，按下“Ctrl+V”将执行此操作）
        /// </summary>
        public void paste() {
            Utility.mark();
        }

        /// <summary>
        /// 执行一个全选操作。(默认地，按下“Ctrl+A”将执行此操作）
        /// </summary>
        public void selectAll() {

        }

        /// <summary>
        /// 执行一个不全选操作。(默认地，按下“Ctrl+Shift+A”将执行此操作）
        /// </summary>
        public void deselectAll() {

        }

        /// <summary>
        /// 执行一个左操作。(默认地，按下“←”将执行此操作）
        /// </summary>
        public void moveLeft() {

        }

        /// <summary>
        /// 执行一个左单词操作。(默认地，按下“Ctrl+←”将执行此操作）
        /// </summary>
        public void moveLeftWord() {

        }

        /// <summary>
        /// 执行一个左选中操作。(默认地，按下“Shift+←”将执行此操作）
        /// </summary>
        public void selectLeft() {

        }

        /// <summary>
        /// 执行一个左单词选中操作。(默认地，按下“Ctrl+Shift+←”将执行此操作）
        /// </summary>
        public void selectLeftWord() {

        }

        /// <summary>
        /// 执行一个复制当前行操作。(默认地，按下“Ctrl+R”将执行此操作）
        /// </summary>
        public void dumpLine() {

        }

        /// <summary>
        /// 执行一个撤销操作。(默认地，按下“Ctrl+Z”将执行此操作）
        /// </summary>
        public void undo() {
            // undostack.undo(this);
        }

        /// <summary>
        /// 执行一个重做操作。(默认地，按下“Ctrl+Y”将执行此操作）
        /// </summary>
        public void redo() {
            // undoStack.redo(this);
        }

        /// <summary>
        /// 执行一个向上滚动的操作。(默认地，按下“PageUp”将执行此操作）
        /// </summary>
        public void pageUp() {

        }

        /// <summary>
        /// 执行一个向上滚动一页的操作。(默认地，按下“PageUp”将执行此操作）
        /// </summary>
        public void pageUpToView() {

        }

        /// <summary>
        /// 执行一个转到行首的操作。(默认地，按下“Home”将执行此操作）
        /// </summary>
        public void gotoLineStart() {

        }

    }
}
