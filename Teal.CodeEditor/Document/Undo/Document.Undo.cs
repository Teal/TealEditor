using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed partial class Document {

        /// <summary>
        /// 获取当前的编辑撤销列表。
        /// </summary>
        public UndoStack undoStack = new UndoStack();

        /// <summary>
        /// 判断当前是否可进行撤销操作。
        /// </summary>
        public bool canUndo {
            get {
                return undoStack.canUndo;
            }
        }

        /// <summary>
        /// 判断当前是否可进行恢复操作。
        /// </summary>
        public bool canRedo {
            get {
                return undoStack.canRedo;
            }
        }

        /// <summary>
        /// 添加一个撤销记录。
        /// </summary>
        /// <param name="op">要撤销的操作。</param>
        public void addUndo(UndoableOperation op) {
            //op.oldLine = _caretLine;
            //op.oldColumn = _caretColumn;
            undoStack.add(op);
        }

        /// <summary>
        /// 删除所有撤销记录。
        /// </summary>
        public void clearUndo() {
            undoStack.clear();
        }

        /// <summary>
        /// 删除指定条数以外的所有撤销记录。
        /// </summary>
        /// <param name="undoLimit">限制的最大撤销记录数。</param>
        public void clearUndo(int undoLimit) {
            undoStack.clear(undoLimit);
        }

        /// <summary>
        /// 对当前编辑器执行撤销操作。
        /// </summary>
        public void undo() {
            undoStack.undo(this);
        }

        /// <summary>
        /// 对当前编辑器执行恢复操作。
        /// </summary>
        public void redo() {
            undoStack.redo(this);
        }

    }

}
