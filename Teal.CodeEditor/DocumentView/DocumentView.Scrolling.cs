using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档视图。
    /// </summary>
    public sealed partial class DocumentView {

        /// <summary>
        /// 存储当前滚动的水平位置。
        /// </summary>
        private int _scrollLeft;

        /// <summary>
        /// 当前展示的滚动 y 位置。
        /// </summary>
        private int _scrollTop;

    }
}
