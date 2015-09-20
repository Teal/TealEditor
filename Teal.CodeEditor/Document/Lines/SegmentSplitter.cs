using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个行内片段分割器。同一行的文本可能被多个分割器分成多个部分。
    /// </summary>
    public struct SegmentSplitter {

        /// <summary>
        /// 获取当前分割列号之前的样式。
        /// </summary>
        public SegmentType type;

        /// <summary>
        /// 获取当前分割的列号。
        /// </summary>
        public int index;

        public SegmentSplitter(int index, SegmentType type) {
            this.index = index;
            this.type = type;
        }

        public override string ToString() {
            return $"{index}({type})";
        }
    }

}
