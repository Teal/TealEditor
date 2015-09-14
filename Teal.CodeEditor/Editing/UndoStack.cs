using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个撤销栈。
    /// </summary>
    public sealed class UndoStack {

        /// <summary>
        /// 第一个撤销操作。
        /// </summary>
        private UndoableOperation _undoList;

        /// <summary>
        /// 第一个恢复操作。
        /// </summary>
        private UndoableOperation _redoList;

        /// <summary>
        /// 当可撤销或可恢复的状态改变时触发。
        /// </summary>
        public event Action stateChanged;

        /// <summary>
        /// 判断当前是否可进行撤销操作。
        /// </summary>
        public bool canUndo {
            get {
                return _undoList != null;
            }
        }

        /// <summary>
        /// 判断当前是否可进行恢复操作。
        /// </summary>
        public bool canRedo {
            get {
                return _redoList != null;
            }
        }

        /// <summary>
        /// 获取第一个撤销操作。如果不可撤销则返回 null。
        /// </summary>
        public UndoableOperation undoTop {
            get {
                return _undoList;
            }
        }

        /// <summary>
        /// 获取第一个恢复操作。如果不可恢复则返回 null。
        /// </summary>
        public UndoableOperation redoTop {
            get {
                return _redoList;
            }
        }

        ///// <summary>
        ///// 用于标记撤销操作是否被禁用。
        ///// </summary>
        //private int _undoLock;

        ///// <summary>
        ///// 判断当前撤销是否启用。
        ///// </summary>
        //public bool isUndoEnabled {
        //    get {
        //        return _undoLock <= 0;
        //    }
        //}

        ///// <summary>
        ///// 禁用撤销操作。
        ///// </summary>
        //public void enableUndo() {
        //    _undoLock++;
        //}

        ///// <summary>
        ///// 启用撤销操作。
        ///// </summary>
        //public void disableUndo() {
        //    _undoLock--;
        //}

        /// <summary>
        /// 在当前列表添加一个撤销记录。
        /// </summary>
        /// <param name="op">要撤销的操作。</param>
        public void add(UndoableOperation op) {

            bool undoWasEmpty = _undoList == null;

            op.prev = _undoList;
            _undoList = op;
            _redoList = null;

            // 触发状态改变事件。
            if (undoWasEmpty && stateChanged != null) {
                stateChanged();
            }
        }

        /// <summary>
        /// 删除所有撤销记录。
        /// </summary>
        public void clear() {
            if (_undoList != null || _redoList != null) {
                _undoList = _redoList = null;
                if (stateChanged != null) {
                    stateChanged();
                }
            }
        }

        /// <summary>
        /// 删除指定条数以外的所有撤销记录。
        /// </summary>
        /// <param name="undoLimit">限制的最大撤销记录数。</param>
        public void clear(int undoLimit) {

            // 全部删除。
            if (undoLimit == 0) {
                clear();
                return;
            }

            // 定位到指定数的回退操作。
            var undoList = _undoList;
            while (--undoLimit > 0 && undoList != null)
                undoList = undoList.prev;

            // 执行清除。
            if (undoLimit == 0 && undoList != null) {
                undoList.prev = null;
            }

        }

        /// <summary>
        /// 对指定编辑器执行撤销操作。
        /// </summary>
        /// <param name="editor">引发操作的编辑器。</param>
        public void undo(CodeEditor editor) {

            // 不存在撤销列表，忽略。
            if (_undoList == null) {
                return;
            }

            var wasRedoListEmpty = _redoList == null;

            editor.beginUpdate();

            UndoableOperation op;

            do {

                // 要执行的撤销操作。
                op = _undoList;

                // 从撤销列表删除。
                _undoList = op.prev;

                // 执行当前操作。
                op.undo(editor);

                // 将当前操作添加到恢复列表。
                op.prev = _redoList;
                _redoList = op;

                // 检查是否可同时执行上一个操作。
            } while (_undoList != null && _undoList.canTrain(editor, op));

            editor.setCaretLocation(op.oldLine, op.oldColumn);

            // 触发状态改变事件。
            if (stateChanged != null && (wasRedoListEmpty || _undoList == null)) {
                stateChanged();
            }

            editor.endUpdate();

        }

        /// <summary>
        /// 对指定编辑器执行恢复操作。
        /// </summary>
        /// <param name="editor">引发操作的编辑器。</param>
        public void redo(CodeEditor editor) {

            // 不存在恢复列表，忽略。
            if (_redoList == null) {
                return;
            }

            var wasUndoListEmpty = _undoList == null;

            editor.beginUpdate();

            UndoableOperation op;

            do {
                
                // 要执行的恢复操作。
                op = _redoList;

                // 从恢复列表删除。
                _redoList = op.prev;

                // 执行当前操作。
                op.redo(editor);

                // 将当前操作添加到撤销列表。
                op.prev = _undoList;
                _undoList = op;

            } while (_redoList != null && _redoList.canTrain(editor, op));

            editor.setCaretLocation(op.newLine, op.newColumn);

            // 触发状态改变事件。
            if (stateChanged != null && (wasUndoListEmpty || _redoList == null)) {
                stateChanged();
            }

            editor.endUpdate();

        }

    }

}
