using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        /// 当前文档的标记。
        /// </summary>
        public DocumentLineFlags flags = DocumentConfigs.defaultNewLineType;

        /// <summary>
        /// 当文档修改状态更改时触发。
        /// </summary>
        public event Action modifyStateChange;

        /// <summary>
        /// 获取或设置当前文档的修改状态。
        /// </summary>
        public DocumentLineFlags modifyState {
            get {
                return flags & DocumentLineFlags.modifiedAndSaved;
            }
            set {
                if ((flags & DocumentLineFlags.modifiedAndSaved) != value) {
                    flags = (flags & ~DocumentLineFlags.modifiedAndSaved) | value;
                    if (value == DocumentLineFlags.saved) {
                        foreach (var line in lines) {
                            line.modifyState |= DocumentLineFlags.saved;
                        }
                    }
                    modifyStateChange?.Invoke();
                }
            }
        }

        /// <summary>
        /// 存储当前文档的修改版本。
        /// </summary>
        public int version;

        /// <summary>
        /// 当文档被更新时触发。
        /// </summary>
        public event Action<int, int> update;

        /// <summary>
        /// 触发文档更新事件。
        /// </summary>
        /// <param name="line">发生改变的行。</param>
        /// <param name="column">发生改变的列。</param>
        private void onUpdate(int line, int column) {

            // 更新文档修改状态。
            version++;
            modifyState = DocumentLineFlags.modified;

            var deltaLine = _caretLine - line;
            var deltaColumn = _caretLine - column;

            update?.Invoke(line, column);

        }

        private int _caretLine, _caretColumn;

        #endregion

        #region 撤销

        /// <summary>
        /// 获取当前的编辑撤销列表。
        /// </summary>
        public UndoStack undoStack = new UndoStack();

        #endregion

        #region 编辑底层

        /// <summary>
        /// 底层实现插入一个多行字符串。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符串。</param>
        /// <param name="startIndex">插入的字符串起始位置。</param>
        /// <param name="length">插入的字符串长度。</param>
        private void insertBlock(int line, int column, string value, int startIndex, int length) {

            var endIndex = startIndex + length;
            var firstLine = lines[line];
            var firstLineBreak = findLineBreak(value, startIndex, endIndex);

            // 加速不插入新换行的情况。
            if (firstLineBreak == endIndex) {
                firstLine.buffer.insert(column, value, startIndex, length);
                firstLine.modifyState = DocumentLineFlags.modified;
                _caretLine = line;
                _caretColumn = column + length;
                return;
            }

            // 依次插入每一行。
            var currentLine = (DocumentLine)null;
            var currentLineNumber = line;
            for (var currentIndex = firstLineBreak; currentIndex < endIndex;) {
                currentLine = readLine(value, currentIndex, endIndex, ref currentIndex);
                lines.insert(++currentLineNumber, currentLine);
            }

            Debug.Assert(currentLine != null);

            // 更新光标到最新插入的后一行最后一列。
            _caretLine = currentLineNumber;
            _caretColumn = currentLine.buffer.length;

            // 处理首行被截断的部分，移动到新行末尾。
            var restCount = firstLine.buffer.length - column;
            if (restCount > 0) {
                currentLine.buffer.append(firstLine.buffer.data, column, restCount);
                firstLine.buffer.remove(column);
            }

            // 处理首行新增的部分。
            firstLine.buffer.append(value, startIndex, firstLineBreak);

        }

        /// <summary>
        /// 底层实现删除一个区块。
        /// </summary>
        /// <param name="startLine">删除的起始行。</param>
        /// <param name="startColumn">删除的起始列。</param>
        /// <param name="endLine">删除的结束行。</param>
        /// <param name="endColumn">删除的结束列。</param>
        private void removeBlock(int startLine, int startColumn, int endLine, int endColumn) {

            _caretLine = startLine;
            _caretColumn = startColumn;

            var firstLine = lines[startLine];

            // 加速不删除行的情况。
            if (startLine == endLine) {
                firstLine.buffer.remove(startColumn, endColumn - startColumn);
                firstLine.modifyState = DocumentLineFlags.modified;
                return;
            }

            var lastLine = lines[endLine];

            // 删除首行末尾字符。
            firstLine.buffer.remove(startColumn);

            // 删除中间的行。
            lines.removeRange(startLine, endLine - startLine + 1);

            // 插入尾行末尾字符。
            firstLine.buffer.append(lastLine.buffer.data, endColumn, lastLine.buffer.length - endColumn);
            firstLine.modifyState = DocumentLineFlags.modified;

        }

        private static int findLineBreak(string value, int startIndex, int endIndex) {
            for (; startIndex < endIndex; startIndex++) {
                switch (value[startIndex]) {
                    case '\r':
                    case '\n':
                        return startIndex;
                }
            }
            return startIndex;
        }

        private static DocumentLine readLine(string value, int startIndex, int endIndex, ref int nextStartIndex) {

            DocumentLineFlags lineStyle;

            switch (value[startIndex]) {
                case '\r':
                    startIndex++;
                    if (startIndex < endIndex && value[startIndex] == '\n') {
                        startIndex++;
                        lineStyle = DocumentLineFlags.newLineTypeWindows;
                    } else {
                        lineStyle = DocumentLineFlags.newLineTypeMac;
                    }
                    break;
                case '\n':
                    startIndex++;
                    lineStyle = DocumentLineFlags.newLineTypeUnix;
                    break;
                default:
                    Debug.Assert(false, "只有当前位置是换行符才能调用 readLine(...)");
                    return new DocumentLine();
            }

            endIndex = findLineBreak(value, startIndex, endIndex);

            nextStartIndex = endIndex;
            return new DocumentLine(lineStyle, value, startIndex, endIndex - startIndex);
        }

        private static string substring(string value, int startIndex, int length) {
            return startIndex + length == value.Length ? value : value.Substring(startIndex, length);
        }

        /// <summary>
        /// 计算在指定位置插入指定字符串后得到的新位置。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符串。</param>
        /// <returns>新位置。</returns>
        public static Location getLocation(int line, int column, string value) => getLocation(line, column, value, 0, value.Length);

        /// <summary>
        /// 计算在指定位置插入指定字符串后得到的新位置。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符串。</param>
        /// <param name="startIndex">插入的字符串起始位置。</param>
        /// <param name="length">插入的字符串长度。</param>
        /// <returns>新位置。</returns>
        public static Location getLocation(int line, int column, string value, int startIndex, int length) {
            for (var endIndex = startIndex + length; startIndex < endIndex; startIndex++) {
                switch (value[startIndex]) {
                    case '\r':
                        if (startIndex + 1 < endIndex && value[startIndex + 1] == '\n') {
                            startIndex++;
                        }
                        goto case '\n';
                    case '\n':
                        line++;
                        column = 0;
                        break;
                    default:
                        column++;
                        break;
                }
            }
            return new Location(line, column);
        }

        #endregion

        #region 文档操作

        /// <summary>
        /// 在指定位置插入一个非换行字符。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符。不允许插入换行符。</param>
        public void insert(int line, int column, char value) {

            // 保存撤销记录。
            if (undoStack.isUndoEnabled) {
                undoStack.add(new InsertCharOperation(line, column, value));
            }

            // 执行操作。
            lines[line].buffer.insert(column, value);
            lines[line].modifyState = DocumentLineFlags.modified;

            // 更新光标位置。
            _caretLine = line;
            _caretColumn = column + 1;

            // 执行更新回调。
            onUpdate(line, column);
        }

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

            // 保存撤销记录。
            if (undoStack.isUndoEnabled) {
                undoStack.add(new InsertBlockOperation(line, column, substring(value, startIndex, length)));
            }

            // 执行操作并更新光标位置。
            insertBlock(line, column, value, startIndex, length);

            // 执行更新回调。
            onUpdate(line, column);

        }

        /// <summary>
        /// 在指定位置插入一行。
        /// </summary>
        /// <param name="line">插入的行号。新插入的行将在指定行之后。</param>
        /// <param name="column">插入的列号。新插入的行将从指定列开始。</param>
        /// <param name="inheritIndents">指示新行是否继承原行的缩进。</param>
        /// <param name="newLineType">新行的换行符。如果为 null 则继承上一行。</param>
        /// <returns>返回插入的新行。</returns>
        public void breakLine(int line, int column) {

            // 保存撤销记录。
            if (undoStack.isUndoEnabled) {
                undoStack.add(new BreakLineOperation(line, column));
            }

            // 执行操作。
            var oldDocumentLine = lines[line];

            // 设置换行类型。
            var indentCount = oldDocumentLine.indentCount;
            var restCount = oldDocumentLine.buffer.length - column;

            var newDocumentLine = new DocumentLine(oldDocumentLine.newLineType, indentCount + restCount + DocumentConfigs.defaultLineCapacity);

            // 拷贝继承缩进。
            if (indentCount > 0) {
                newDocumentLine.buffer.append(oldDocumentLine.buffer.data, 0, indentCount);
            }

            // 将原行数据分成两行。
            if (restCount > 0) {
                newDocumentLine.buffer.append(oldDocumentLine.buffer.data, column, restCount);
                oldDocumentLine.buffer.remove(column);
                oldDocumentLine.modifyState = DocumentLineFlags.modified;
            }

            // 插入换行。
            lines.insert(line + 1, newDocumentLine);

            // 更新光标位置。
            _caretLine = line + 1;
            _caretColumn = indentCount;

            // 执行更新回调。
            onUpdate(line, column);

        }

        /// <summary>
        /// 将指定的行将和上一行合并为一行。
        /// </summary>
        /// <param name="line">要删除的行号。</param>
        /// <param name="column">要删除的列号。在此列号之前的文本将被删除。</param>
        public void unbreakLine(int line, int column = 0) {

            // 无法删除第一行。
            if (line <= 0) {
                return;
            }

            // 保存撤销记录。
            if (undoStack.isUndoEnabled) {
                undoStack.add(new UnbreakLineOperation(line, column));
            }

            // 执行操作。
            var oldDocumentLine = lines[line];
            var lastDocumentLine = lines[line - 1];

            // 更新光标位置。
            _caretLine = line - 1;
            _caretColumn = lastDocumentLine.buffer.length;

            lastDocumentLine.buffer.append(oldDocumentLine.buffer.data, column, oldDocumentLine.buffer.length - column);
            lastDocumentLine.modifyState = DocumentLineFlags.modified;
            lines.removeAt(line);

            // 执行更新回调。
            onUpdate(line, column);

        }

        /// <summary>
        /// 替换指定位置的字符。
        /// </summary>
        /// <param name="line">插入的行号。</param>
        /// <param name="column">插入的列号。</param>
        /// <param name="value">插入的字符。不允许插入换行符。</param>
        public void replace(int line, int column, char value) => replace(line, column, line, column, value.ToString(), 0, 1);

        /// <summary>
        /// 删除指定位置的字符。
        /// </summary>
        /// <param name="line">删除的行号。</param>
        /// <param name="column">删除的列号。</param>
        public void delete(int line, int column) {

            // 保存撤销记录。
            if (undoStack.isUndoEnabled) {
                undoStack.add(new DeleteCharOperation(line, column, lines[line].buffer[column]));
            }

            // 执行操作并更新光标位置。
            lines[line].buffer.remove(column, 1);
            lines[line].modifyState = DocumentLineFlags.modified;
            _caretLine = line;
            _caretColumn = column;

            // 执行更新回调。
            onUpdate(line, column);

        }

        /// <summary>
        /// 替换指定位置的多行字符串。
        /// </summary>
        /// <param name="startLine">替换的起始行。</param>
        /// <param name="startColumn">替换的起始列。</param>
        /// <param name="endLine">替换的结束行。</param>
        /// <param name="endColumn">替换的结束列。</param>
        /// <param name="value">替换的字符串。</param>
        public void replace(int startLine, int startColumn, int endLine, int endColumn, string value) => replace(startLine, startColumn, endLine, endColumn, value, 0, value.Length);

        /// <summary>
        /// 替换指定位置的多行字符串。
        /// </summary>
        /// <param name="startLine">替换的起始行。</param>
        /// <param name="startColumn">替换的起始列。</param>
        /// <param name="endLine">替换的结束行。</param>
        /// <param name="endColumn">替换的结束列。</param>
        /// <param name="value">替换的字符串。</param>
        /// <param name="startIndex">替换的字符串起始位置。</param>
        /// <param name="length">替换的字符串长度。</param>
        public void replace(int startLine, int startColumn, int endLine, int endColumn, string value, int startIndex, int length) {

            // 保存撤销记录。
            if (undoStack.isUndoEnabled) {
                undoStack.add(new ReplaceBlockOperation(startLine, startColumn, getText(startLine, startColumn, endLine, endColumn), substring(value, startIndex, length)));
            }

            // 执行操作并更新光标位置。
            removeBlock(startLine, startColumn, endLine, endColumn);
            insertBlock(startLine, startColumn, value, startIndex, length);

            // 执行更新回调。
            onUpdate(endLine, endColumn);

        }

        /// <summary>
        /// 删除指定区间的字符串。
        /// </summary>
        /// <param name="startLine">删除的起始行。</param>
        /// <param name="startColumn">删除的起始列。</param>
        /// <param name="endLine">删除的结束行。</param>
        /// <param name="endColumn">删除的结束列。</param>
        public void delete(int startLine, int startColumn, int endLine, int endColumn) {

            // 保存撤销记录。
            if (undoStack.isUndoEnabled) {
                undoStack.add(new DeleteBlockOperation(startLine, startColumn, getText(startLine, startColumn, endLine, endColumn)));
            }

            // 执行操作并更新光标位置。
            removeBlock(startLine, startColumn, endLine, endColumn);

            // 执行更新回调。
            onUpdate(endLine, endColumn);

        }

        #endregion

    }

}
