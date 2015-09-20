﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个文档行标记。
    /// </summary>
    [Flags]
    public enum DocumentLineFlags {

        /// <summary>
        /// 无任何书签。
        /// </summary>
        none = 0,

        /// <summary>
        /// 表示当前行由 \r\n 换行。
        /// </summary>
        newLineTypeWin = 0,

        /// <summary>
        /// 表示当前行由 \r 换行。
        /// </summary>
        newLineTypeMac = 1 << 0,

        /// <summary>
        /// 表示当前行由 \n 换行。
        /// </summary>
        newLineTypeUnix = 1 << 1,

        /// <summary>
        /// 表示换行符类型。
        /// </summary>
        NEW_LINE_TYPE = newLineTypeWin | newLineTypeMac | newLineTypeUnix,

        ///// <summary>
        ///// 已修改。
        ///// </summary>
        //modified = 1 << 0,

        ///// <summary>
        ///// 已修改并保存。
        ///// </summary>
        //modifiedAndSaved = 1 << 1,

        ///// <summary>
        ///// 获取书签状态。
        ///// </summary>
        //MODIFIE_STATE = modified | modifiedAndSaved,

        ///// <summary>
        ///// 指示当前行需要重新布局。
        ///// </summary>
        //needRelayout,

        /// <summary>
        /// 普通书签。
        /// </summary>
        bookmark = 1 << 3,

        /// <summary>
        /// 包含断点。
        /// </summary>
        breakpoint = 1 << 4,

        /// <summary>
        /// 当前行已解析。
        /// </summary>
        parsed = 1 << 2,

        /// <summary>
        /// 正在激活。
        /// </summary>
        actived = 1 << 5,

        /// <summary>
        /// 表示当前行包含一个定义。
        /// </summary>
        definition = 1 << 6,

    }

}
