using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示文档的修改状态。
    /// </summary>
    public enum ModifyState {

        /// <summary>
        /// 文档未修改。
        /// </summary>
        unmodified,

        /// <summary>
        /// 文档已修改。
        /// </summary>
        modified,

        /// <summary>
        /// 文档已修改并保存。
        /// </summary>
        modifiedAndSaved,

    }

}
