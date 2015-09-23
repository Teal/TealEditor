using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个折叠区域。
    /// </summary>
    public sealed class FoldingRange {

        /// <summary>
        /// 获取创建当前折叠区域原始区块。
        /// </summary>
        public Block block;

        /// <summary>
        /// 判断或设置当前块的折叠状态。
        /// </summary>
        public bool isFolded;

    }

}
