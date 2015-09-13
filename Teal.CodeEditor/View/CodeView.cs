using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码视图。
    /// </summary>
    public sealed class CodeView {

        /// <summary>
        /// 切换指定区域的折叠状态。
        /// </summary>
        /// <param name="line">折叠区域。</param>
        /// <returns>如果确实折叠行，则返回 true。</returns>
        public bool toggleFold(int line) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 确保指定位置未被折叠。
        /// </summary>
        /// <param name="line">要显示的行。</param>
        /// <param name="column">要显示的列。</param>
        /// <returns>如果已展开行则返回 true，否则返回 false。</returns>
        public bool ensureVisible(int line, int column) {
            throw new NotImplementedException();
        }

        private FoldingRange getFoldingRange(int line, int column) {
            throw new NotImplementedException();
        }

        private FoldingRange getFoldBlockByStart(int line, int column) {
            throw new NotImplementedException();
        }

        private FoldingRange getFoldingRangeByEnd(int line, int column) {
            throw new NotImplementedException();
        }

        private FoldingRange getFoldingRangeByLine(int line) {
            throw new NotImplementedException();
        }



    }
}
