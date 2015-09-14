using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个片段类型。
    /// </summary>
    public abstract class SegmentType {

        /// <summary>
        /// 获取当前片段类型的开始部分的模式表达式。
        /// </summary>
        public Pettern start { get; }

        private static SegmentType[] _emptyChildren = new SegmentType[0];

        /// <summary>
        /// 获取当前片段类型的子片段类型。
        /// </summary>
        public SegmentType[] children { get; } = _emptyChildren;

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public abstract bool isMultiLine {
            get {
                return false;
            }
        }

        ///// <summary>
        ///// 在指定的文本内找到当前片段的开始位置。如果找不到则返回 -1。
        ///// </summary>
        ///// <param name="text">要匹配的文本。</param>
        ///// <param name="startIndex">文本的开始位置。</param>
        ///// <param name="endIndex">文本的结束位置。</param>
        ///// <param name="start">返回匹配的起始位置。</param>
        ///// <param name="endIndex">返回匹配的结束位置。</param>
        //public abstract bool matchStart(string text, int startIndex, int endIndex, out int start, out int end);

        /// <summary>
        /// 判断当前片段是否属于块级片段。块级片段即可能横跨多行的块，一般会拥有一个折叠域。
        /// </summary>
        public abstract bool isBlock { get; }

    }

    /// <summary>
    /// 表示一个单词片段类型。
    /// </summary>
    public sealed class WordSegmentType : SegmentType {

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public override bool isMultiLine {
            get {
                return false;
            }
        }

        /// <summary>
        /// 判断当前片段类型是否是块。
        /// </summary>
        public override bool isBlock {
            get {
                return false;
            }
        }

    }

    /// <summary>
    /// 表示一个块片段类型。
    /// </summary>
    public abstract class BlockSegmentType : SegmentType {

        /// <summary>
        /// 当前块的结束模式表达式。
        /// </summary>
        public Pettern end { get; }
        
        /// <summary>
        /// 判断当前片段类型是否是块。
        /// </summary>
        public sealed override bool isBlock {
            get {
                return true;
            }
        }

    }

    /// <summary>
    /// 表示一个块级类型。
    /// </summary>
    public sealed class MultiLineBlockSegmentType : BlockSegmentType {
        
        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public override bool isMultiLine {
            get {
                return true;
            }
        }

    }
    
    /// <summary>
    /// 表示一个内联块级类型。
    /// </summary>
    public sealed class SingleLineBlockSegmentType : BlockSegmentType {
        
        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public override bool isMultiLine {
            get {
                return false;
            }
        }

    }

    /// <summary>
    /// 表示一个模式表达式。
    /// </summary>
    public abstract class Pettern {

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <param name="resultStartIndex">返回匹配的起始位置。</param>
        /// <param name="resultEndIndex">返回匹配的结束位置。</param>
        public abstract void match(string text, int startIndex, int endIndex, out int resultStartIndex, out int resultEndIndex);

    }
    
    /// <summary>
    /// 表示一个字符串模式表达式。
    /// </summary>
    public abstract class StringPettern {
                
        /// <summary>
        /// 获取当前模式的内容。
        /// </summary>
        /// <value>
        public string content{ get; }
        
        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString() {
 	         return content;
        }

    }

    /// <summary>
    /// 表示一个区分大小写的字符串模式表达式。
    /// </summary>
    public sealed class CaseSensitiveStringPettern : StringPettern {

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <param name="resultStartIndex">返回匹配的起始位置。</param>
        /// <param name="resultEndIndex">返回匹配的结束位置。</param>
        public override void match(string text, int startIndex, int endIndex, out int resultStartIndex, out int resultEndIndex) {
            resultStartIndex = text.IndexOf(content, startIndex, endIndex - startIndex + 1);
            resultEndIndex = resultStartIndex + content.Length;
        }

    }
    
    /// <summary>
    /// 表示一个不区分大小写的字符串模式表达式。
    /// </summary>
    public sealed class CaseInsensitiveStringPettern : StringPettern  {

        public string content{ get; }

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <param name="resultStartIndex">返回匹配的起始位置。</param>
        /// <param name="resultEndIndex">返回匹配的结束位置。</param>
        public override void match(string text, int startIndex, int endIndex, out int resultStartIndex, out int resultEndIndex) {
           resultStartIndex = text.IndexOf(content, startIndex, endIndex - startIndex + 1, StringComparison.OrdinalIgnoreCase);
            resultEndIndex = resultStartIndex + content.Length;
        }
        
        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString() {
 	         return content;
        }

    }

}

/*
 * 
 * 
 * code: {
 *    children: [  
 *      'comment.mulineLine',
 *      'comment.singleLine',
 *      'string.doubleQuote',
 *      'string.singleQuote',
 *      'block',
 *      'region',
 *      'keyword',
 *      'number'
 *   ]
 * },
 * 'region': {
 *     children: [
 *          'code'
 *     ]
 * }
 * 
 * 
 * 
 * 
*/