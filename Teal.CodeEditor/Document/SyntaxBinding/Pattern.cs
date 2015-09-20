using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个模式表达式。
    /// </summary>
    public abstract class Pattern {

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <returns>返回匹配结果。</returns>
        public abstract PatternMatchResult match(string text, int startIndex, int endIndex);

        /// <summary>
        /// 表示一个匹配任何字符串的模式表达式。
        /// </summary>
        [DebuggerStepThrough]
        private sealed class MatchStartingPattern : Pattern {

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
                return "(Match Starting)";
            }

        }

        /// <summary>
        /// 表示一个不匹配任何字符串的模式表达式。
        /// </summary>
        [DebuggerStepThrough]
        private sealed class MatchNothingPattern : Pattern {

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
                return "(Match Nothing)";
            }

        }

        /// <summary>
        /// 表示一个不匹配任何字符串的模式表达式。
        /// </summary>
        [DebuggerStepThrough]
        private sealed class MatchEndingPattern : Pattern {

            /// <summary>
            /// 尝试使用当前模式表达式去匹配指定的文本。
            /// </summary>
            /// <param name="text">要匹配的文本。</param>
            /// <param name="startIndex">文本的开始位置。</param>
            /// <param name="endIndex">文本的结束位置。</param>
            /// <returns>返回匹配结果。</returns>
            public override PatternMatchResult match(string text, int startIndex, int endIndex) {
                return new PatternMatchResult(endIndex, endIndex);
            }

            public override string ToString() {
                return "(Match Ending)";
            }

        }

        /// <summary>
        /// 获取可以匹配任何字符串的表达式。
        /// </summary>
        public readonly static Pattern starting = new MatchStartingPattern();

        /// <summary>
        /// 获取可以匹配行尾的表达式。
        /// </summary>
        public readonly static Pattern none = new MatchNothingPattern();

        /// <summary>
        /// 获取可以匹配行尾的表达式。
        /// </summary>
        public readonly static Pattern ending = new MatchEndingPattern();

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

        ///// <summary>
        ///// 获取表示未找到的模式表达式。
        ///// </summary>
        //public static PatternMatchResult notFound => new PatternMatchResult(-1, -1);

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
    public abstract class StringPattern : Pattern {

        /// <summary>
        /// 获取当前模式的内容。
        /// </summary>
        public readonly string content;

        protected StringPattern(string content) {
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
    public sealed class CaseSensitiveStringPattern : StringPattern {

        public CaseSensitiveStringPattern(string content)
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
    public sealed class CaseInsensitiveStringPattern : StringPattern {

        public CaseInsensitiveStringPattern(string content)
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
    /// 表示一个区分大小写的字符串模式表达式。
    /// </summary>
    public sealed class RegexPattern : Pattern {

        /// <summary>
        /// 当前模式表达式对应的正则表示。
        /// </summary>
        public readonly Regex content;

        public RegexPattern(Regex content) {
            this.content = content;
        }

        public RegexPattern(string content, RegexOptions options = RegexOptions.Compiled | RegexOptions.Singleline)
                : this(new Regex(content, options)) { }

        /// <summary>
        /// 尝试使用当前模式表达式去匹配指定的文本。
        /// </summary>
        /// <param name="text">要匹配的文本。</param>
        /// <param name="startIndex">文本的开始位置。</param>
        /// <param name="endIndex">文本的结束位置。</param>
        /// <returns>返回匹配结果。</returns>
        public override PatternMatchResult match(string text, int startIndex, int endIndex) {
            var match = content.Match(text, startIndex, endIndex - startIndex);
            PatternMatchResult result;
            if (match.Success) {
                result.startIndex = match.Index;
                result.endIndex = result.startIndex + match.Length;
            } else {
                result.startIndex = result.endIndex = -1;
            }
            return result;
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString() {
            return $"{content}";
        }

    }

}
