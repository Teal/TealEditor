using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 指示移动的操作。
    /// </summary>
    public enum MoveOperation {

        /// <summary>
        /// 未进行任何移动操作。
        /// </summary>
        none,

        /// <summary>
        /// 仅移动了一列。
        /// </summary>
        column,

        /// <summary>
        /// 仅移动了一行。
        /// </summary>
        line,

        /// <summary>
        /// 移动了多行多列。
        /// </summary>
        block

    }

}
