using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个用于读取文档的工具。
    /// </summary>
    public class DocumentReader : TextReader {

        /// <summary>
        /// 获取当前正在写入的文档。
        /// </summary>
        public readonly Document document;

        /// <summary>
        /// 获取当前行。
        /// </summary>
        public int currentLine;

        /// <summary>
        /// 获取当前列。
        /// </summary>
        public int currentColumn;

        /// <summary>
        /// 初始化一个新的 DocumentTextWriter 类。
        /// </summary>
        /// <param name="document">写入的文档。</param>
        /// <param name="startLine">初始化的行。</param>
        /// <param name="startColumn">初始化的列。</param>
        public DocumentReader(Document document, int startLine = 0, int startColumn = 0) {
            this.document = document;
            this.currentLine = startLine;
            this.currentColumn = startColumn;
        }

        ///// <summary>
        ///// Gets/Sets the current insertion offset.
        ///// </summary>
        //public int InsertionOffset {
        //    get { return insertionOffset; }
        //    set { insertionOffset = value; }
        //}

        ///// <inheritdoc/>
        //public override void Write(char value) {
        //    document.Insert(insertionOffset, value.ToString());
        //    insertionOffset++;
        //}

        ///// <inheritdoc/>
        //public override void Write(char[] buffer, int index, int count) {
        //    document.Insert(insertionOffset, new string(buffer, index, count));
        //    insertionOffset += count;
        //}

        ///// <inheritdoc/>
        //public override void Write(string value) {
        //    document.Insert(insertionOffset, value);
        //    insertionOffset += value.Length;
        //}

    }

}
