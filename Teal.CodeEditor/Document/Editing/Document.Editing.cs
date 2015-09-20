using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed partial class Document {

        #region 修改状态

        /// <summary>
        /// 当前文档的修改状态。
        /// </summary>
        private ModifyState _modifyState;

        /// <summary>
        /// 当文档修改状态更改时触发。
        /// </summary>
        public event Action modifyStateChange;

        /// <summary>
        /// 判断或设置当前文档的修改状态。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModifyState modifyState {
            get {
                return _modifyState;
            }
            set {
                if (_modifyState != value) {
                    _modifyState = value;
                    modifyStateChange?.Invoke();
                }
            }
        }

        /// <summary>
        /// 当文档被更新时触发。
        /// </summary>
        public event Action<int, int, int, int, int, int> update;

        /// <summary>
        /// 触发文档被更新事件。
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        private void ononDocumentUpdated(int startLine, int endLine) {

        }

        /// <summary>
        /// 触发文档被更新事件。
        /// </summary>
        /// <param name="line">发生改变的行。</param>
        /// <param name="column">发生改变的列。</param>
        /// <param name="deleteLineCount">删除的行数。</param>
        /// <param name="insertLineCount">插入的行数。</param>
        /// <param name="deleteColumnCount">删除的列数。</param>
        /// <param name="insertColumnCount">插入的列数。</param>
        private void onLineUpdated(int startLine, int endLine) {

            // 更新文档修改状态。
            modifyState = ModifyState.modified;

            //if (update != null) {
            //    update(line, column, deleteLineCount, insertLineCount, deleteColumnCount, insertColumnCount);
            //}

        }

        /// <summary>
        /// 触发文档被更新事件。
        /// </summary>
        /// <param name="line">发生改变的行。</param>
        /// <param name="column">发生改变的列。</param>
        /// <param name="deleteLineCount">删除的行数。</param>
        /// <param name="insertLineCount">插入的行数。</param>
        /// <param name="deleteColumnCount">删除的列数。</param>
        /// <param name="insertColumnCount">插入的列数。</param>
        private void onUpdate(int line, int column, int deleteLineCount, int insertLineCount, int deleteColumnCount, int insertColumnCount) {

            // 更新文档修改状态。
            modifyState = ModifyState.modified;

            if (update != null) {
                update(line, column, deleteLineCount, insertLineCount, deleteColumnCount, insertColumnCount);
            }

        }

        #endregion

        /// <summary>
        /// 在指定位置插入一个非换行字符。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符。不允许插入换行符。</param>
        public void insert(int line, int column, char value) {

            // 插入数据。
            lines[line].buffer.insert(column, value);

            // 保存操作记录。
            if (undoStack.isUndoEnabled) {
                addUndo(new InsertCharOperation(_caretLine, _caretColumn, value));
            }

            // 更新光标位置。
            _caretLine = line;
            _caretColumn = column + 1;

            // 同步更新位置。
            onUpdate(line, column, 0, 0, 0, 1);
        }

        ///// <summary>
        ///// 在指定区域插入一个字符串。
        ///// </summary>
        ///// <param name="line">插入的行号。</param>
        ///// <param name="column">插入的列号。</param>
        ///// <param name="value">要插入的字符串。</param>
        ///// <param name="enableUndo">指示是否将当前操作添加到撤销列表。</param>
        //public void insert(int line, int column, string value, bool enableUndo = true) {
        //    var newEnd = document.insert(line, column, value);
        //    addUndo(new InsertBlockOperation(line, column, newEnd.Y, newEnd.X, value));
        //    setCaretLocation(newEnd.Y, newEnd.X);
        //}

        /// <summary>
        /// 在指定位置插入一个多行字符串。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符串。</param>
        public void insert(int line, int column, string value) => insert(line, column, value, 0, value.Length);

        /// <summary>
        /// 在指定位置插入一个多行字符串。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符串。</param>
        /// <param name="startIndex">插入的字符串起始位置。</param>
        /// <param name="length">插入的字符串长度。</param>
        public void insert(int line, int column, string value, int startIndex, int length) {

            // 如果 value 中不包含换行，则可以加速插入。

            string lineContent;
            DocumentLineFlags newLineStyle;

            var firstLineBreak = readLine(value, startIndex, length, out lineContent, out newLineStyle);

            // 加速不存在换行的情况。
            if (firstLineBreak < 0) {
                lines[line].buffer.insert(column, value, startIndex, length);
            }

            var currentLineNumber = line;
            var currentLine = lines[currentLineNumber];

            // 当前行将被截断。

            // 保存当前行被
            string rest = null;

            string restNewLine = currentLine.newLine;

            // 剪切被删除的行尾。
            var restCount = currentLine.textLength - column;
            if (restCount > 0) {
                rest = new String(currentLine.TextData, column, restCount);
                currentLine.remove(column);
            }

            // 插入行。
            int index = startIndex, count;
            string newLine;
            while ((count = Utility.readLine(value, length, ref index, out newLine)) > 0) {

                // 插入当前行字符串。
                currentLine.append(value, startIndex, count);
                startIndex = index;

                // 如果发现了换行符，则进行换行。
                if (newLine != null) {

                    // 更新当前行的换行符。
                    currentLine.newLine = newLine;

                    // 创建新行。
                    currentLine = new DocumentLine();
                    lines.Insert(++currentLineNumber, currentLine);

                }

            }

            var newColumn = currentLine.textLength;

            // 重新复制被粘贴的末尾。
            if (rest != null) {
                currentLine.append(rest);
            }

            // 重新设置新换行符。
            currentLine.newLine = restNewLine;

            // 保存操作记录。
            if (undoStack.isUndoEnabled) {
                addUndo(new InsertBlockOperation(_caretLine, _caretColumn, value));
            }

            onUpdate(line, column, 0, currentLineNumber - line, 0, newColumn - column);

        }

        private static int readLine(string value, int startIndex, int length, out string line, out DocumentLineFlags newLineStyle) {
            for (var i = index; i < length; i++) {
                if (value[i] == '\r') {
                    var c = i - index;
                    index = i + 1;
                    if (index < length && value[index] == '\n') {
                        index++;
                        newLine = "\r\n";
                    } else {
                        newLine = "\r";
                    }
                    return c;
                }
                if (value[i] == '\n') {
                    var c = i - index;
                    index = i + 1;
                    newLine = "\n";
                    return c;
                }
            }

            var t = length - index;
            index = length;
            newLine = null;
            return t;
        }



        /// <summary>
        /// 在指定位置插入一行。
        /// </summary>
        /// <param name="line">插入的行号。新插入的行将在指定行之后。</param>
        /// <param name="column">插入的列号。新插入的行将从指定列开始。</param>
        /// <param name="inheritIndents">指示新行是否继承原行的缩进。</param>
        /// <param name="newLineType">新行的换行符。如果为 null 则继承上一行。</param>
        /// <returns>返回插入的新行。</returns>
        public DocumentLine breakLine(int line, int column, bool inheritIndents = true, DocumentLineFlags newLineType = DocumentLineFlags.NEW_LINE_TYPE) {
            int indentCount;
            return breakLine(line, column, inheritIndents, newLineType, out indentCount);
        }

        /// <summary>
        /// 在指定位置插入一行。
        /// </summary>
        /// <param name="line">插入的行号。新插入的行将在指定行之后。</param>
        /// <param name="column">插入的列号。新插入的行将从指定列开始。</param>
        /// <param name="inheritIndents">指示新行是否继承原行的缩进。</param>
        /// <param name="newLineType">新行的换行符。如果为 null 则继承上一行。</param>
        /// <param name="indentCount">输出缩进的个数。</param>
        /// <returns>返回插入的新行。</returns>
        public DocumentLine breakLine(int line, int column, bool inheritIndents, DocumentLineFlags newLineType, out int indentCount) {

            var oldDocumentLine = lines[line];

            // 生成新行。
            var newDocumentLine = new DocumentLine();

            // 插入换行。
            if (newLineType == DocumentLineFlags.NEW_LINE_TYPE) {
                newLineType = oldDocumentLine.newLineType;
            }
            newDocumentLine.newLineType = newLineType;

            // 拷贝继承缩进。
            if (inheritIndents) {
                newDocumentLine.buffer.append(oldDocumentLine.buffer.data, 0, oldDocumentLine.indentCount);
            }

            // 生成新列。
            var newColumn = indentCount = newDocumentLine.textLength;

            // 将原行数据分成两行。
            var restCount = oldDocumentLine.buffer.length - column;
            if (restCount > 0) {
                newDocumentLine.buffer.append(oldDocumentLine.buffer.data, column, restCount);
                oldDocumentLine.buffer.remove(column);
            }

            // 保存新行。
            lines.insert(line + 1, newDocumentLine);

            onUpdate(line, column, 0, 1, column, newColumn);

            return newDocumentLine;
        }

        /// <summary>
        /// 将指定的行将和上一行合并为一行。
        /// </summary>
        /// <param name="line">要删除的行号。</param>
        /// <param name="column">要删除的列号。</param>
        /// <returns>返回删除的行。</returns>
        public DocumentLine unbreakLine(int line, int column = 0) {
            if (line == 0) {
                return null;
            }

            var oldLine = lines[line];
            lines[line - 1].buffer.append(oldLine.buffer.data, column, oldLine.buffer.length - column);
            lines.removeAt(line);

            onUpdate(line, column, 1, 0, column, lines[line - 1].buffer.length - column);

            return oldLine;
        }

        /// <summary>
        /// 替换指定位置的字符。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符。不允许插入换行符。</param>
        public void replace(int line, int column, char value) {
            lines[line][column] = value;
            onUpdate(line, column + 1, 0, 0, 1, 1);
        }

        /// <summary>
        /// 删除指定位置的字符。
        /// </summary>
        /// <param name="line">删除的行号。</param>
        /// <param name="column">删除的列号。</param>
        public void delete(int line, int column) {
            lines[line].buffer.remove(column, 1);
            onUpdate(line, column, 0, 0, 1, 0);
        }

        ///// <summary>
        ///// 替换指定位置的多行字符串。
        ///// </summary>
        ///// <param name="startLine">替换的起始行。</param>
        ///// <param name="startColumn">替换的起始列。</param>
        ///// <param name="endLine">替换的结束行。</param>
        ///// <param name="endColumn">替换的结束列。</param>
        ///// <param name="value">插入的字符串。</param>
        ///// <returns>返回插入后的新位置。</returns>
        //public Point replace(int startLine, int startColumn, int endLine, int endColumn, string value) {
        //    return replace(startLine, startColumn, endLine, endColumn, value, 0, value.Length);
        //}

        ///// <summary>
        ///// 替换指定位置的多行字符串。
        ///// </summary>
        ///// <param name="startLine">替换的起始行。</param>
        ///// <param name="startColumn">替换的起始列。</param>
        ///// <param name="endLine">替换的结束行。</param>
        ///// <param name="endColumn">替换的结束列。</param>
        ///// <param name="value">替换的字符串。</param>
        ///// <param name="startIndex">替换的字符串起始位置。</param>
        ///// <param name="value">替换的字符串长度。</param>
        ///// <returns>返回插入后的新位置。</returns>
        //public Point replace(int startLine, int startColumn, int endLine, int endColumn, string value, int startIndex, int textLength) {

        //    var currentLineNumber = startLine;
        //    var currentLine = lines[currentLineNumber];
        //    string rest = null;

        //    // 剪切被删除的行尾。
        //    var restCount = lines[endLine].textLength - endColumn;
        //    if (restCount > 0) {
        //        rest = new String(lines[endLine].chars, endColumn, restCount);
        //    }

        //    // 删除当前行多余的内容。
        //    currentLine.remove(startColumn);

        //    // 插入行。
        //    int index = startIndex, count;
        //    string newLine;
        //    while ((count = Utility.readLine(value, textLength, ref index, out newLine)) > 0) {

        //        // 插入当前行字符串。
        //        currentLine.append(value, startIndex, count);
        //        startIndex = index;

        //        // 如果发现了换行符，则进行换行。
        //        if (newLine != null) {

        //            // 创建新行。
        //            if (++currentLineNumber <= endLine) {
        //                currentLine = lines[currentLineNumber];
        //                currentLine.clear();
        //            } else {
        //                currentLine = new DocumentLine();
        //                lines.Insert(currentLineNumber, currentLine);
        //            }

        //            // 更新当前行的换行符。
        //            currentLine.newLine = newLine;

        //        }

        //    }

        //    var newColumn = currentLine.textLength;

        //    // 粘贴被剪切的末尾。
        //    if (rest != null) {
        //        currentLine.append(rest);
        //    }

        //    // 去除多余的行。
        //    for (var i = endLine; i > currentLineNumber; i++) {
        //        lines.RemoveAt(i);
        //    }

        //    onUpdate(endLine, endColumn, endLine - startLine, currentLineNumber - startLine, endColumn - startColumn, newColumn - startColumn);

        //    return new Point(newColumn, currentLineNumber);
        //}

        ///// <summary>
        ///// 删除指定区间的字符串。
        ///// </summary>
        ///// <param name="startLine">删除的起始行。</param>
        ///// <param name="startColumn">删除的起始列。</param>
        ///// <param name="endLine">删除的结束行。</param>
        ///// <param name="endColumn">删除的结束列。</param>
        //public void delete(int startLine, int startColumn, int endLine, int endColumn) {

        //    if (startLine == endLine) {
        //        lines[startLine].remove(startColumn, endColumn - startColumn);
        //        onUpdate(endLine, endColumn, 0, 0, endColumn - startColumn, 0);
        //        return;
        //    }

        //    var oldLine = lines[startLine];

        //    oldLine.remove(startColumn);
        //    oldLine.append(lines[endLine].chars, endColumn, lines[endLine].textLength - endColumn);

        //    for (var i = endLine; i > startLine; i++) {
        //        lines.RemoveAt(i);
        //    }

        //    onUpdate(endLine, endColumn, endLine - startLine, 0, endColumn - startColumn, 0);

        //}

        /// <summary>
        /// 在当前光标位置插入换行符。
        /// </summary>
        public void breakLine() {
            int indentCount;
            _document.breakLine(caretLine, caretColumn, inheritIndents, DocumentLineFlags.NEW_LINE_TYPE, out indentCount);
            addUndo(new BreakLineOperation(caretLine, caretColumn, indentCount));
            setCaretLocation(caretLine + 1, indentCount);
        }

        /// <summary>
        /// 在指定位置插入一个多行字符串。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符串。</param>
        public void insertBlock(int line, int column, string value) {
            //var undo = new InsertBlockUndoableOperation() {
            //    line = line,
            //    column = column,
            //    value = value
            //};
            //addUndo(undo);
            //undo.endLocation = _document.insertBlock(line, column, value);
        }

        /// <summary>
        /// 将指定的行将和上一行合并为一行。
        /// </summary>
        /// <param name="line">要删除的行号。</param>
        public void unbreakLine(int line) {
            //addUndo(new UnbreakLineUndoableOperation() {
            //    line = line
            //});

            _document.unbreakLine(line);
        }

        #region 通用编辑处理程序

        /// <summary>
        /// 替换当前选区内容为指定内容。
        /// </summary>
        /// <param name="value"></param>
        private void replaceSelections(string value, bool select) {

        }

        /// <summary>
        /// 删除指定区域的字符串。
        /// </summary>
        /// <param name="caretLine">操作之前的光标所在行，用于撤销后回复光标位置。</param>
        /// <param name="caretColumn">操作之前的光标所在列，用于撤销后回复光标位置。</param>
        /// <param name="startLine">替换的起始行。</param>
        /// <param name="startColumn">替换的起始列。</param>
        /// <param name="endLine">替换的结束行。</param>
        /// <param name="endColumn">替换的结束列。</param>
        public void delete(int caretLine, int caretColumn, int startLine, int startColumn, int endLine, int endColumn) {
            var oldValue = document.getText(startLine, startColumn, endLine, endColumn);
            document.delete(startLine, startColumn, endLine, endColumn);
            addUndo(new DeleteBlockOperation(caretLine, caretColumn, startLine, startColumn, endLine, endColumn, oldValue));
            setCaretLocation(startLine, startColumn);
        }

        /// <summary>
        /// 替换指定区域的字符串。
        /// </summary>
        /// <param name="caretLine">操作之前的光标所在行，用于撤销后回复光标位置。</param>
        /// <param name="caretColumn">操作之前的光标所在列，用于撤销后回复光标位置。</param>
        /// <param name="startLine">替换的起始行。</param>
        /// <param name="startColumn">替换的起始列。</param>
        /// <param name="endLine">替换的结束行。</param>
        /// <param name="endColumn">替换的结束列。</param>
        /// <param name="value">替换的字符串。</param>
        public void replace(int caretLine, int caretColumn, int startLine, int startColumn, int endLine, int endColumn, string value) {
            var oldValue = document.getText(startLine, startColumn, endLine, endColumn);
            var newEnd = document.replace(startLine, startColumn, endLine, endColumn, value);
            addUndo(new ReplaceBlockOperation(caretLine, caretColumn, startLine, startColumn, endLine, endColumn, oldValue, newEnd.Y, newEnd.X, value));
            setCaretLocation(newEnd.Y, newEnd.X);
        }

        #endregion

    }

}
