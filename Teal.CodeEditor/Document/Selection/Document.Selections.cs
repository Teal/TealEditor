using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed partial class Document {

        /// <summary>
        /// 所有选区列表。
        /// </summary>
        private Selection _selections;

        ///// <summary>
        ///// 获取当前的选区。
        ///// </summary>
        //public Selection selection {
        //    get {
        //        return _selections;
        //    }
        //}

        public Selection getSelection() {
            return null;
        }

        ///// <summary>
        ///// 获取或设置当前的选区文本。
        ///// </summary>
        //public string selectionText {
        //    get {
        //        //if (_selections != null) {
        //        //    for (var selection = _selections; selection != null; selection = selection.prev) {

        //        //    }
        //        //}
        //        //return _selections;

        //        // 无选区返回空。
        //        if (_selections == null) {
        //            return null;
        //        }

        //        return getText(_selections.startLine, _selections.startColumn, _selections.endLine, _selections.endColumn);
        //    }
        //    set {
        //        if (_selections == null) {
        //            insertBlock(_caretVisualLine, _caretColumn, value);
        //            return;
        //        }

        //        //replaceBlock(_selections.startLine, _selections.startColumn, _selections.endLine, _selections.endColumn, value);
        //    }
        //}

        /// <summary>
        /// 遍历当前编辑器的全部选区。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Selection> getSelections() {
            for (var selection = _selections; selection != null; selection = selection.prev) {
                yield return selection;
            }
        }

        public void select(int startLine, int startColumn, int endLine, int endColumn) {
            Utility.mark(startLine, startColumn, endLine, endColumn);
        }

        public void select(int startLine, int startColumn, int endLine, int endColumn, bool multipart) {

        }

        public void selectVertial(int startLine, int startColumn, int endLine, int endColumn) {

        }

        public void selectVertial(int startLine, int startColumn, int endLine, int endColumn, bool multipart) {

        }

    }

}