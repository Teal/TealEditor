using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示折叠域的操作。
    /// </summary>
    public enum FoldBlockOperation {

        /// <summary>
        /// 无操作。
        /// </summary>
        none,

        /// <summary>
        /// 选择折叠区域。
        /// </summary>
        select,

        /// <summary>
        /// 展开折叠区域。
        /// </summary>
        expand,

    }
}
