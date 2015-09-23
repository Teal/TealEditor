using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor : Control {

        /// <summary>
        /// 获取所有键盘事件绑定。
        /// </summary>
        public readonly Dictionary<Keys, Action> actions = new Dictionary<Keys, Action>(30);

        /// <summary>
        /// 初始化默认事件列表。
        /// </summary>
        private void initDefaultActions() {

            // 控制。
            actions[Keys.Back] = backspace;
            actions[Keys.Control | Keys.Back] = backspaceWord;

            actions[Keys.Delete] = delete;
            actions[Keys.Control | Keys.Delete] = deleteWord;

            actions[Keys.Return] = breakLine;
            actions[Keys.Tab] = indent;
            actions[Keys.Shift | Keys.Tab] = unindent;

            // 剪贴板。
            actions[Keys.Control | Keys.C] = copy;
            actions[Keys.Control | Keys.Insert] = copy;
            actions[Keys.Control | Keys.V] = paste;
            actions[Keys.Shift | Keys.Insert] = paste;
            actions[Keys.Control | Keys.X] = cut;
            actions[Keys.Shift | Keys.Delete] = cut;

            // 撤销。
            actions[Keys.Control | Keys.Z] = undo;
            actions[Keys.Control | Keys.Y] = redo;
            actions[Keys.Alt | Keys.Back] = undo;

            // 其它。
            actions[Keys.Control | Keys.A] = selectAll;
            actions[Keys.Control | Keys.R] = dumpLine;
            actions[Keys.Escape] = clearSelections;

            //// 方向键。
            //actions[Keys.Left] = moveLeft;
            //actions[Keys.Left | Keys.Shift] = selectLeft;
            //actions[Keys.Left | Keys.Control] = moveWordLeft;
            //actions[Keys.Left | Keys.Control | Keys.Shift] = selectWordLeft;

            //    actions[Keys.Right] = moveRight;
            //    actions[Keys.Right | Keys.Shift] = selectRight;
            //    actions[Keys.Right | Keys.Control] = moveWordRight;
            //    actions[Keys.Right | Keys.Control | Keys.Shift] = selectWordRight;

            //    actions[Keys.Up] = moveUp;
            //    actions[Keys.Up | Keys.Shift] = selectUp;
            //    actions[Keys.Up | Keys.Control] = scrollUp;

            //    actions[Keys.Down] = moveDown;
            //    actions[Keys.Down | Keys.Shift] = selectDown;
            //    actions[Keys.Down | Keys.Control] = scrollDown;

            //    actions[Keys.Insert] = toggleCaretMode;

            //    actions[Keys.Home] = moveToHome;
            //    actions[Keys.Home | Keys.Shift] = selectToHome;
            //    actions[Keys.End] = moveToEnd;
            //    actions[Keys.End | Keys.Shift] = selectToEnd;

            //    actions[Keys.PageUp] = movePageUp;
            //    actions[Keys.PageUp | Keys.Shift] = selectPageUp;
            //    actions[Keys.PageDown] = movePageDown;
            //    actions[Keys.PageDown | Keys.Shift] = selectPageDown;

            //    actions[Keys.Control | Keys.Divide] = toggleComment;

        }

    }
}
