using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个语法绑定。用于提供特定语法的相关处理。
    /// </summary>
    public class SyntaxBinding {

        /// <summary>
        /// 获取当前语法绑定对应的编辑器。
        /// </summary>
        public CodeEditor codeEditor;

        /// <summary>
        /// 判断一个字符是否是单词的分隔符。
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public virtual bool isWordPart(char ch) {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch == '@' || ch == '$' || ch == '_' || ch == '.';
        }

        /// <summary>
        /// 判断一个字符是否是空格。
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public virtual bool isWhitespace(char ch) {
            return ch == ' ' || ch == '\t' || ch == '\u3000';
        }

        /// <summary>
        /// 查找指定行列的上一个单词起始处。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public int findPrevWordStart(int line, int column) {
            if (column > 0) {
                var chars = codeEditor.document.lines[line].chars;

                // 跳到第一个改变字符类型的位置。
                CharacterType t = getCharacterType(chars[column - 1]);

                // 如果本来就在空格处，则先忽略空格。
                if (t == CharacterType.whitespace) {
                    while (column > 0 && (t = getCharacterType(chars[column - 1])) == CharacterType.whitespace) {
                        column--;
                    }
                }

                // 从当前位置开始找到第一个类型不同的位置。
                while (column > 0 && getCharacterType(chars[column - 1]) == t) {
                    column--;
                }

            }

            return column;
        }

        /// <summary>
        /// 查找指定行列的下一个单词结束处。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public int findNextWordEnd(int line, int column) {
            var chars = codeEditor.document.lines[line].chars;
            if (column < chars.Length) {

                // 跳到第一个改变字符类型的位置。
                CharacterType t = getCharacterType(chars[column]);

                // 如果本来就在空格处，则先忽略空格。
                if (t == CharacterType.whitespace) {
                    while (column < chars.Length && (t = getCharacterType(chars[column])) == CharacterType.whitespace) {
                        column++;
                    }
                }

                // 从当前位置开始找到第一个类型不同的位置。
                while (column < chars.Length && getCharacterType(chars[column]) == t) {
                    column++;
                }

            }

            return column;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public CharacterType getCharacterType(char c) {
            if (isWordPart(c)) {
                return CharacterType.wordPart;
            }
            if (isWhitespace(c)) {
                return CharacterType.whitespace;
            }
            return CharacterType.other;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="endLine"></param>
        public virtual void generateFoldings(int startLine, int endLine) {
            
        }
        
    }

    public enum CharacterType {
        wordPart,
        whitespace,
        other
    }

}
