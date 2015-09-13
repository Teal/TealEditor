using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个滚动条上的分屏按钮。
    /// </summary>
    public sealed class SplitButton  : Button {

        public SplitButton() {
            Cursor = Cursors.Default;
            BackColor = SystemColors.Control;
        }

        /// <returns>
        /// 控件的默认 <see cref="T:System.Drawing.Size"/>。
        /// </returns>
        protected override Size DefaultSize {
            get {
                return new Size(Configs.defaultVScrollBarSize, Configs.defaultHScrollBarSize);
            }
        }

    }
}
