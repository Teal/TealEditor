using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个折叠列表。
    /// </summary>
    public sealed class FoldingList {

        public void add(FoldingRange range) {
            
        }

        public void clear() {
            
        }

        private FoldingRange getFoldingRange(int line, int column) {
            throw new NotImplementedException();
        }

        private FoldingRange getFoldingRangeByEnd(int line, int column) {
            throw new NotImplementedException();
        }

        private FoldingRange getFoldBlockByStart(int line, int column) {
            throw new NotImplementedException();
        }

        private FoldingRange getFoldBlockByLine(int line) {
            throw new NotImplementedException();
        }

    }

}
