using System;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档。
    /// </summary>
    public sealed partial class Document {

        /// <summary>
        /// 获取或设置当前文档的配置。
        /// </summary>
        public DocumentConfigs configs = new DocumentConfigs();

        public Document() {
            
        }

    }
}
