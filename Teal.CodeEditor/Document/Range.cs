using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档区域。文档区域会在文档更新后自动更新。
    /// </summary>
    public partial class Range {

        /// <summary>
        /// 获取当前区域的起始行。
        /// </summary>
        public int startLine;

        /// <summary>
        /// 获取当前区域的起始列。
        /// </summary>
        public int startColumn;

        /// <summary>
        /// 获取当前区域的结束行。
        /// </summary>
        public int endLine;

        /// <summary>
        /// 获取当前区域的结束列。
        /// </summary>
        public int endColumn;

        /// <summary>
        /// 阻止手动创建 Range 对象。
        /// </summary>
        internal Range() {}

        /// <summary>
        /// 判断指定的行是否包含在此区间。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool contains(int line) {
            return line >= startLine && line < endLine;
        }

        /// <summary>
        /// 判断指定的位置是否包含在此区间。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool contains(int line, int column) {
            if (line < startLine || line > endLine) {
                return false;
            }
            if (line == startLine) {
                return column >= startColumn;
            }
            if (line == endLine) {
                return column < endColumn;
            }
            return true;
        }

        /// <summary>
        /// 判断此区间是否在指定位置之前。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool isBefore(int line, int column) {
            return line > endLine || (line == endLine && column >= endLine);
        }

        /// <summary>
        /// 判断此区间是否在指定位置之后。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool isAfter(int line, int column) {
            return line < startLine || (line == startLine && column < startColumn);
        }

        /// <summary>
        /// 判断指定行列和结尾更近。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool isClosestToEnd(int line, int column) {
            var deltaLine = endLine - line - line + startLine;
            if (deltaLine != 0) {
                return deltaLine < 0;
            }
            return endColumn - column < column - startColumn;
        }

        /// <summary>
        /// 由于文档更新同步当前区域。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <param name="deltaLine"></param>
        /// <param name="deltaColumn"></param>
        public void update(int line, int column, int deltaLine, int deltaColumn) {
            // 当前点可能在目标区域之前，之中或之后。

            //int startLine, ;

            //if (deltaLine < 0) {
            //    minLine = line + deltaLine;
            //} else {
            //    minLine = line;
            //}

        }

        private void update(ref int currentLine, ref int currentColumn, int line, int column, int deltaLine, int deltaColumn) {
            if (currentLine < line || (currentLine == line && currentColumn < column)) {
                
            }
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() {
            return String.Concat(startLine, ", ", startColumn, " - ", endLine, ", ", endColumn);
        }

    }
}
