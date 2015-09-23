using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public ArrayList<DocumentLine> lines = new ArrayList<DocumentLine>(2) { new DocumentLine() };

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

        /// <summary>
        /// 获取或设置当前文档的全部文本。
        /// </summary>
        public string text {
            get {
                if (lines.length == 0) {
                    return String.Empty;
                }
                var sb = new StringBuffer(DocumentConfigs.defaultLineCapacity);
                var line = lines[0];
                sb.append(line.buffer.data, 0, line.buffer.length);
                for (var i = 1; i < lines.length; i++) {
                    line = lines[i];
                    sb.append(line.newLine);
                    sb.append(line.buffer.data, 0, line.buffer.length);
                }
                return sb.ToString();
            }
            set {
                load(new StringReader(value));
            }
        }

        /// <summary>
        /// 获取指定区间的文本。
        /// </summary>
        /// <param name="startLine">获取的起始行。</param>
        /// <param name="startColumn">获取的起始列。</param>
        /// <param name="endLine">获取的结束行。</param>
        /// <param name="endColumn">获取的结束列。</param>
        /// <returns>返回获取的文本。</returns>
        public string getText(int startLine, int startColumn, int endLine, int endColumn) {
            Debug.Assert(startLine < endLine || (startLine == endLine && startColumn <= endColumn));

            // 同行文本直接获取。
            if (startLine == endLine) {
                return lines[startLine].buffer[startColumn, endColumn];
            }

            var sb = new StringBuffer(DocumentConfigs.defaultLineCapacity);
            sb.append(lines[startLine].buffer.data, startColumn, lines[startLine].textLength - startColumn);
            for (var i = startLine + 1; i < endLine; i++) {
                sb.append(lines[i].newLine);
                sb.append(lines[i].buffer.data, 0, lines[i].textLength);
            }
            sb.append(lines[endLine].newLine);
            sb.append(lines[endLine].buffer.data, 0, endColumn);
            return sb.ToString();
        }

        #region 读写

        /// <summary>
        /// 创建用于读取当前文档内容的读取器。
        /// </summary>
        /// <returns>返回读取器。</returns>
        public TextReader createReader() {
            throw new NotImplementedException();
            //return new DocumentReader(this);
        }

        /// <summary>
        /// 创建用于写入到当前文档的写入器。
        /// </summary>
        /// <returns>返回写入器。</returns>
        public TextWriter createWriter() {
            throw new NotImplementedException();
            // return new DocumentWriter(this);
        }

        /// <summary>
        /// 从指定的读取器载入文档。
        /// </summary>
        /// <param name="reader">要载入的读取器。</param>
        public void load(TextReader reader) {
            flags &= ~DocumentLineFlags.modifiedAndSaved;
            DocumentLine currentLine;
            int c;

            lines.clear();
            lines.add(currentLine = new DocumentLine(newLineType));
            while (true) {
                switch (c = reader.Read()) {
                    case '\r':
                        var newLineType = DocumentLineFlags.newLineTypeMac;
                        if (reader.Peek() == '\n') {
                            reader.Read();
                            newLineType = DocumentLineFlags.newLineTypeWindows;
                        }
                        lines.add(currentLine = new DocumentLine(newLineType));
                        continue;
                    case '\n':
                        lines.add(currentLine = new DocumentLine(DocumentLineFlags.newLineTypeUnix));
                        continue;
                    case -1:
                        return;
                }
                currentLine.buffer.append(unchecked((char)c));
            }
        }

        /// <summary>
        /// 从指定流载入文档。
        /// </summary>
        /// <param name="stream">要载入的流。</param>
        /// <param name="encoding">要载入的编码。</param>
        /// <param name="bufferSize">编码使用的临时缓存大小。</param>
        public void load(Stream stream, Encoding encoding, int bufferSize = 1024) {

            // TODO: 提示性能。

            load(new StreamReader(stream, encoding, true, bufferSize));

            //var buffer = new byte[bufferSize];

            //int bytesRead;
            //while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0) {

            //   // var maxCharCount = encoding.g


            //}

        }

        /// <summary>
        /// 从指定文件载入文档。
        /// </summary>
        /// <param name="path">要载入的路径。</param>
        /// <param name="encoding">要载入的编码。</param>
        /// <param name="bufferSize">编码使用的临时缓存大小。</param>
        public void load(string path, Encoding encoding, int bufferSize = 1024) {
            using (var fs = File.OpenRead(path)) {
                load(fs, encoding, bufferSize);
            }
        }

        /// <summary>
        /// 将当前文档的数据写到指定的输出器。
        /// </summary>
        /// <param name="writer">要写入的输出器。</param>
        public void save(TextWriter writer) {
            var line = lines[0];
            writer.Write(line.buffer.ToString());
            for (var i = 1; i < lines.length; i++) {
                line = lines[i];
                writer.Write(line.newLine);
                writer.Write(line.buffer.ToString());
            }
            flags |= DocumentLineFlags.saved;
        }

        /// <summary>
        /// 将当前文档的数据写到指定的流。
        /// </summary>
        /// <param name="stream">要写入的流。</param>
        /// <param name="encoding">要写入的编码。</param>
        public void save(Stream stream, Encoding encoding) {

            // 获取最长的行。
            var bufferSize = 0;
            for (int i = 0; i < lines.length; i++) {
                if (bufferSize < lines[i].buffer.length) {
                    bufferSize = lines[i].buffer.length;
                }
            }

            // 创建缓存。
            var buffer = new byte[encoding.GetMaxByteCount(bufferSize)];

            unsafe
            {
                fixed (byte* pBuffer = buffer)
                {
                    for (var i = 0; i < lines.length; i++) {
                        var line = lines[i];

                        if (i > 0) {
                            switch (line.newLineType) {
                                case DocumentLineFlags.newLineTypeWindows:
                                    stream.WriteByte((byte)'\r');
                                    stream.WriteByte((byte)'\n');
                                    break;
                                case DocumentLineFlags.newLineTypeMac:
                                    stream.WriteByte((byte)'\r');
                                    break;
                                case DocumentLineFlags.newLineTypeUnix:
                                    stream.WriteByte((byte)'\n');
                                    break;
                            }
                        }

                        fixed (char* p = line.buffer.data)
                        {
                            var byteEncoded = encoding.GetBytes(p, line.buffer.length, pBuffer, buffer.Length);
                            stream.Write(buffer, 0, byteEncoded);
                        }
                    }
                }
            }

            flags |= DocumentLineFlags.saved;
        }

        /// <summary>
        /// 将当前文档的数据写到指定的文件。
        /// </summary>
        /// <param name="writer">要写入的文件。</param>
        /// <param name="encoding">要写入的编码。</param>
        public void save(string path, Encoding encoding) {
            using (var fs = File.OpenWrite(path)) {
                save(fs, encoding);
            }
        }

        #endregion

        ///////// <summary>
        ///////// 将指定区间的文本写入缓存器。
        ///////// </summary>
        ///////// <param name="sb">写入的目标缓存。</param>
        ///////// <param name="startLine">获取的起始行。</param>
        ///////// <param name="startColumn">获取的起始列。</param>
        ///////// <param name="endLine">获取的结束行。</param>
        ///////// <param name="endColumn">获取的结束列。</param>
        //////public void write(StringBuilder sb, int startLine, int startColumn, int endLine, int endColumn) {
        //////    Debug.Assert(startLine < endLine || (startLine == endLine && startColumn <= endColumn));
        //////    if (endLine == startLine) {
        //////        sb.Append(lines[startLine].buffer.data, startColumn, endColumn - startColumn);
        //////    } else {
        //////        sb.Append(lines[startLine].buffer.data, startColumn, lines[startLine].textLength - startColumn);
        //////        for (var i = startLine + 1; i < endLine; i++) {
        //////            sb.Append(lines[i].newLine);
        //////            sb.Append(lines[i].buffer.data, 0, lines[i].textLength);
        //////        }
        //////        sb.Append(lines[endLine].newLine);
        //////        sb.Append(lines[endLine].buffer.data, 0, endColumn);
        //////    }
        //////}

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString() {
            return text;
        }

        #region 换行

        /// <summary>
        /// 判断当前文档是否包含混合的换行符。
        /// </summary>
        public bool hasMixedNewLine {
            get {
                if (lines.length == 0) {
                    return false;
                }

                var lastLine = lines[lines.length - 1].newLineType;
                for (var i = 1; i < lines.length; i++) {
                    if (lines[i].newLineType != lastLine) {
                        return false;
                    }
                }

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
                    return flags & DocumentLineFlags.NEW_LINE_TYPES;
                }
                return lines[lines.length - 1].newLineType;
            }
            set {
                if (newLineType != value || hasMixedNewLine) {
                    flags = (flags & ~DocumentLineFlags.NEW_LINE_TYPES) | value;
                    for (var i = 1; i < lines.length; i++) {
                        lines[i].newLineType = value;
                    }
                    modifyState = DocumentLineFlags.modified;
                }
            }
        }

        #endregion

    }

}
