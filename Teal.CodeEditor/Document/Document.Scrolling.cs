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
        /// 当前展示的滚动 x 位置。
        /// </summary>
        private int _scrollLeft;

        /// <summary>
        /// 当前展示的滚动 y 位置。
        /// </summary>
        private int _scrollTop;

    }
}
