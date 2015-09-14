﻿using System;
using System.Diagnostics;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个片段类型。
    /// </summary>
    public abstract class SegmentType {

        ///// <summary>
        ///// 获取当前判断类型的名字。
        ///// </summary>
        //public string name { get; }

        /// <summary>
        /// 获取当前片段类型的开始部分的模式表达式。
        /// </summary>
        public Pettern start { get; }

        /// <summary>
        /// 获取当前片段类型的子片段类型。
        /// </summary>
        public SegmentType[] children { get; }

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public abstract bool isMultiLine { get; }

        /// <summary>
        /// 判断当前片段是否属于块级片段。块级片段即可能横跨多行的块，一般会拥有一个折叠域。
        /// </summary>
        public abstract bool isBlock { get; }

        protected SegmentType(Pettern start, SegmentType[] children = null) {
            this.start = start;
            this.children = children;
        }

    }

    /// <summary>
    /// 表示一个单词片段类型。
    /// </summary>
    public sealed class WordSegmentType : SegmentType {

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public override bool isMultiLine => false;

        /// <summary>
        /// 判断当前片段类型是否是块。
        /// </summary>
        public override bool isBlock => false;

        public WordSegmentType(Pettern start, SegmentType[] children = null)
                : base(start, children) {
        }

        public override string ToString() {
            return $"{start}";
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
        public sealed override bool isBlock => true;

        protected BlockSegmentType(Pettern start, Pettern end, SegmentType[] children = null)
                : base(start, children) {
            this.end = end;
        }

        public override string ToString() {
            return $"{start}...{end}";
        }

    }

    /// <summary>
    /// 表示一个块级类型。
    /// </summary>
    public sealed class MultiLineBlockSegmentType : BlockSegmentType {

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public override bool isMultiLine => true;

        public MultiLineBlockSegmentType(Pettern start, Pettern end, SegmentType[] children = null)
                : base(start, end, children) {

        }

    }

    /// <summary>
    /// 表示一个内联块级类型。
    /// </summary>
    public sealed class SingleLineBlockSegmentType : BlockSegmentType {

        /// <summary>
        /// 判断当前片段类型是否是跨行的。
        /// </summary>
        public override bool isMultiLine => false;

        public SingleLineBlockSegmentType(Pettern start, Pettern end, SegmentType[] children = null)
                : base(start, end, children) {

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
        /// <returns>返回匹配结果。</returns>
        public abstract PatternMatchResult match(string text, int startIndex, int endIndex);

    }

    /// <summary>
    /// 表示模式匹配的结果。
    /// </summary>
    public struct PatternMatchResult {

        /// <summary>
        /// 获取匹配的开始位置。
        /// </summary>
        public int startIndex;

        /// <summary>
        /// 获取匹配的结束位置。
        /// </summary>
        public int endIndex;

        /// <summary>
        /// 判断当前匹配是否成功。
        /// </summary>
        public bool success => startIndex >= 0;

        public PatternMatchResult(int startIndex, int endIndex) {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString() {
            return success ? $"{startIndex}-{endIndex}" : "(Dismatch)";
        }

    }

    /// <summary>
    /// 表示一个字符串模式表达式。
    /// </summary>
    public abstract class StringPettern : Pettern {

        /// <summary>
        /// 获取当前模式的内容。
        /// </summary>
        public string content { get; }

        protected StringPettern(string content) {
            this.content = content;
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

    /// <summary>
    /// 表示一个区分大小写的字符串模式表达式。
    /// </summary>
    public sealed class CaseSensitiveStringPettern : StringPettern {

        public CaseSensitiveStringPettern(string content)
                : base(content) { }

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <returns>返回匹配结果。</returns>
        public override PatternMatchResult match(string text, int startIndex, int endIndex) {
            PatternMatchResult result;
            result.startIndex = text.IndexOf(content, startIndex, endIndex - startIndex, StringComparison.Ordinal);
            result.endIndex = result.startIndex + content.Length;
            return result;
        }

    }

    /// <summary>
    /// 表示一个不区分大小写的字符串模式表达式。
    /// </summary>
    public sealed class CaseInsensitiveStringPettern : StringPettern {

        public CaseInsensitiveStringPettern(string content)
                : base(content) { }

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <returns>返回匹配结果。</returns>
        public override PatternMatchResult match(string text, int startIndex, int endIndex) {
            PatternMatchResult result;
            result.startIndex = text.IndexOf(content, startIndex, endIndex - startIndex + 1, StringComparison.OrdinalIgnoreCase);
            result.endIndex = result.startIndex + content.Length;
            return result;
        }

    }

    /// <summary>
    /// 表示一个不区分大小写的字符串模式表达式。
    /// </summary>
    [DebuggerStepThrough]
    public sealed class AnyMatchPettern : Pettern {

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <returns>返回匹配结果。</returns>
        public override PatternMatchResult match(string text, int startIndex, int endIndex) {
            return new PatternMatchResult(startIndex, startIndex);
        }

        public override string ToString() {
            return "*";
        }

    }

    /// <summary>
    /// 表示一个不区分大小写的字符串模式表达式。
    /// </summary>
    [DebuggerStepThrough]
    public sealed class AnyDismatchPettern : Pettern {

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <returns>返回匹配结果。</returns>
        public override PatternMatchResult match(string text, int startIndex, int endIndex) {
            return new PatternMatchResult(-1, -1);
        }

        public override string ToString() {
            return "";
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