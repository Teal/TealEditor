using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        #region 选区获取

        /// <summary>
        /// 所有选区列表。
        /// </summary>
        private Selection _selections;

        public Selection getSelection() {
            return null;
        }

        ///// <summary>
        ///// 获取当前的选区。
        ///// </summary>
        //public Selection selection {
        //    get {
        //        return _selections;
        //    }
        //}

        /// <summary>
        /// 获取或设置当前的选区文本。
        /// </summary>
        public string selectionText {
            get {
                //if (_selections != null) {
                //    for (var selection = _selections; selection != null; selection = selection.prev) {
                        
                //    }
                //}
                //return _selections;

                // 无选区返回空。
                if (_selections == null) {
                    return null;
                }

                return _document.getText(_selections.startLine, _selections.startColumn, _selections.endLine, _selections.endColumn);
            }
            set {
                if (_selections == null) {
                    insertBlock(_caretVisualLine, _caretColumn, value);
                    return ;
                }

                //replaceBlock(_selections.startLine, _selections.startColumn, _selections.endLine, _selections.endColumn, value);
            }
        }

        /// <summary>
        /// 遍历当前编辑器的全部选区。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Selection> getSelections() {
            for (var selection = _selections; selection != null; selection = selection.prev) {
                yield return selection;
            }
        }

        #endregion

        #region 选区控制

        public void select(int startLine, int startColumn, int endLine, int endColumn) {
            Utility.mark(startLine, startColumn, endLine, endColumn);
        }

        public void select(int startLine, int startColumn, int endLine, int endColumn, bool multipart) {

        }

        public void selectVertial(int startLine, int startColumn, int endLine, int endColumn) {

        }

        public void selectVertial(int startLine, int startColumn, int endLine, int endColumn, bool multipart) {

        }

        #endregion

    }

    /// <summary>
    /// 表示一个选区。
    /// </summary>
    public sealed class Selection : Range {

        /// <summary>
        /// 获取上一个选区。
        /// </summary>
        public Selection prev;

    }

}