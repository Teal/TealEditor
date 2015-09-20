using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个片段类型。
    /// </summary>
    public class SegmentType {

        #region 命名

        /// <summary>
        /// 获取或设置当前片段类型的名字。
        /// </summary>
        public string name;

        /// <summary>
        /// 判断当前片段类型是否是内置的指定类型。
        /// </summary>
        /// <param name="internalName">要判断的类型。</param>
        /// <returns></returns>
        public bool isInternal(string internalName) {
            return Object.ReferenceEquals(name, internalName);
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString() {
            return name;
        }

        #endregion

        #region 匹配

        /// <summary>
        /// 获取或设置当前片段类型的开始部分的模式表达式。
        /// </summary>
        public Pettern start;

        /// <summary>
        /// 判断当前片段是否属于块级片段。块级片段即可能横跨多行的块，一般会拥有一个折叠域。
        /// </summary>
        public virtual bool isBlock => false;

        public SegmentType() { }

        public SegmentType(string name, Pettern start) {
            this.name = name;
            this.start = start;
        }

        #endregion

        #region 样式

        /// <summary>
        /// 获取或设置当前片段的背景色。
        /// </summary>
        public uint backColor { get; set; }

        /// <summary>
        /// 获取或设置当前片段的前景色。
        /// </summary>
        public uint foreColor { get; set; }
        public bool acceptTabs {
            get;
            internal set;
        }

        #endregion

    }

}
