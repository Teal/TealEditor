using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed partial class Document {

        /// <summary>
        /// 获取当前文档的所有行。
        /// </summary>
        public ArrayList<DocumentLine> lines = new ArrayList<DocumentLine>(2);

        /// <summary>
        /// 获取或设置指定索引的行。
        /// </summary>
        /// <param name="index">要获取的行号。行号从 0 开始。</param>
        /// <returns>返回指定的行。</returns>
        public DocumentLine this[int index] {
            get {
                return lines[index];
            }
            set {
                lines[index] = value;
            }
        }

        ///// <summary>
        ///// 创建用于读取当前文档内容的读取器。
        ///// </summary>
        ///// <returns></returns>
        //public TextReader createReader() {
        //    return new DocumentReader(this);
        //}

        ///// <summary>
        ///// 创建用于写入到当前文档的写入器。
        ///// </summary>
        ///// <returns></returns>
        //public TextWriter createWriter() {
        //    return new DocumentWriter(this);
        //}

        ///// <summary>
        ///// 从指定的读取器载入文档。
        ///// </summary>
        ///// <param name="reader">要载入的读取器。</param>
        //public void load(TextReader reader) {
        //    lines.Clear();

        //    DocumentLine currentLine = new DocumentLine();
        //    lines.Add(currentLine);
        //    int c;
        //    while ((c = reader.Read()) > 0) {
        //        if (c == '\r') {
        //            currentLine = new DocumentLine();
        //            if (reader.Peek() == '\n') {
        //                reader.Read();
        //            } else {
        //                currentLine.flags |= DocumentLineFlags.newLineTypeMac;
        //            }
        //            lines.Add(currentLine);
        //            continue;
        //        }

        //        if (c == '\n') {
        //            currentLine = new DocumentLine();
        //            currentLine.flags |= DocumentLineFlags.newLineTypeUnix;
        //            lines.Add(currentLine);
        //            continue;
        //        }

        //        currentLine.append((char)c);
        //    }
        //}

        ///// <summary>
        ///// 将当前文档的数据写到指定的输出器。
        ///// </summary>
        ///// <param name="writer">要写入的输出器。</param>
        //public void save(TextWriter writer) {
        //    for (var i = 0; i < lines.Count; i++) {
        //        var line = lines[i];
        //        if (line.newLine != null) {
        //            writer.Write(line.newLine);
        //        }
        //        writer.Write(line.chars, 0, line.textLength);
        //    }
        //}

        ///// <summary>
        ///// 获取或设置当前文档的全部文本。
        ///// </summary>
        //public string text {
        //    get {
        //        var writer = new StringWriter();
        //        save(writer);
        //        return writer.ToString();
        //    }
        //    set {
        //        load(new StringReader(value));
        //    }
        //}

        /// <summary>
        /// 获取指定区间的文本。
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="startColumn"></param>
        /// <param name="endLine"></param>
        /// <param name="endColumn"></param>
        /// <returns></returns>
        public string getText(int startLine, int startColumn, int endLine, int endColumn) {
            if (startLine == endLine) {
                return lines[startLine].buffer[startColumn, endColumn];
            }
            var sb = new StringBuilder();
            write(sb, startLine, startColumn, endLine, endColumn);
            return sb.ToString();
        }

        /// <summary>
        /// 将指定区间的文本写入缓存器。
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="startLine"></param>
        /// <param name="startColumn"></param>
        /// <param name="endLine"></param>
        /// <param name="endColumn"></param>
        public void write(StringBuilder sb, int startLine, int startColumn, int endLine, int endColumn) {
            if (endLine == startLine) {
                sb.Append(lines[startLine].buffer.data, startColumn, endColumn - startColumn);
            } else {
                sb.Append(lines[startLine].buffer.data, startColumn, lines[startLine].textLength - startColumn);
                for (var i = startLine + 1; i < endLine; i++) {
                    sb.Append(lines[i].newLine);
                    sb.Append(lines[i].buffer.data, 0, lines[i].textLength);
                }
                sb.Append(lines[endLine].newLine);
                sb.Append(lines[endLine].buffer.data, 0, endColumn);
            }
        }

        ///// <summary>
        ///// 返回表示当前对象的字符串。
        ///// </summary>
        ///// <returns>
        ///// 表示当前对象的字符串。
        ///// </returns>
        //public override string ToString() {
        //    return text;
        //}

        #region 换行

        /// <summary>
        /// 当前文档的换行符。
        /// </summary>
        private DocumentLineFlags _newLineType = DocumentConfigs.defaultNewLineType;

        /// <summary>
        /// 判断当前文档是否包含混合的换行符。
        /// </summary>
        public bool hasMixedNewLine {
            get {
                //if (lines.Count == 0) {
                //    return false;
                //}

                //var lastLine = lines[lines.Count - 1].newLineType;
                //for (var i = 1; i < lines.Count; i++) {
                //    if (lines[i].newLineType != lastLine) {
                //        return false;
                //    }
                //}


                return true;
            }
        }

        /// <summary>
        /// 获取或设置当前文档的换行符。
        /// </summary>
        public string newLine {
            get {
                return Utility.newlineTypeToNewLine(newLineType);
            }
            set {
                newLineType = Utility.newlineToNewLineType(value);
            }
        }

        /// <summary>
        /// 获取或设置当前文档的换行符。
        /// </summary>
        public DocumentLineFlags newLineType {
            get {
                if (lines.length == 0) {
                    return _newLineType;
                }

                return lines[lines.length - 1].newLineType;

            }
            set {
                if (newLineType != value || hasMixedNewLine) {
                    _newLineType = value;
                    for (var i = 1; i < lines.length; i++) {
                        lines[i].newLineType = value;
                    }
                    modifyState = ModifyState.modified;
                }
            }
        }

        #endregion

    }

}
