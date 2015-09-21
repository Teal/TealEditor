using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

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
