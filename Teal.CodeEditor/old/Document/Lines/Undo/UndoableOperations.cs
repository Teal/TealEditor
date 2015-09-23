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

}
