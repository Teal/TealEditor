using System;
using System.ComponentModel;
using System.Drawing;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        /// <summary>
        /// 当更新字体大小后，重新计算相关的尺寸。
        /// </summary>
        private void updateFontSize(double scale) {
            foldMarkerWidth = (int)(foldMarkerWidth * scale);
        }


    }

}
