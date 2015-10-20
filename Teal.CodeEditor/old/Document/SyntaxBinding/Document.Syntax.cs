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
        /// 获取或设置当前文档的语法绑定。
        /// </summary>
        public SyntaxBinding syntaxBinding = new SyntaxBinding();

    }
}
