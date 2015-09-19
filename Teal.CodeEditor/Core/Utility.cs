using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 提供工具函数。
    /// </summary>
    internal static class Utility {

        ///// <summary>
        ///// 在指定数组插入指定数量元素。
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="array">原数组。</param>
        ///// <param name="textLength">原数组的长度。</param>
        ///// <param name="index">要插入的位置。</param>
        ///// <param name="count">要插入的数量。</param>
        ///// <returns>返回已插入的数组。</returns>
        //public static T[] insert<T>(T[] array, int textLength, int index, int count) {

        //    var restCount = textLength - index;

        //    // 判断数组是否需要扩容。
        //    if (textLength + count >= array.Length) {
        //        var result = new T[textLength + count];

        //        // 插入到数组末尾。
        //        if (restCount <= 0) {
        //            Array.Copy(array, result, textLength);
        //        } else {
        //            Array.Copy(array, result, index);
        //            Array.Copy(array, index, result, index + count, restCount);
        //        }

        //        return result;
        //    }

        //    if (restCount > 0) {
        //        for (int from = textLength, to = textLength + count; restCount > 0; restCount--) {
        //            array[to--] = array[from--];
        //        }
        //    }

        //    return array;
        //}

        ///// <summary>
        ///// 在数组指定位置插入一格值。
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="array">原数组。</param>
        ///// <param name="textLength">原数组的长度。</param>
        ///// <param name="index">要插入的位置。</param>
        ///// <param name="index">要插入的内容。</param>
        //public static void insert<T>(ref T[] array, ref int textLength, int index, T value) {

        //    // 检查是否需扩容。
        //    if (array.Length < ++textLength) {
        //        var result = new T[array.Length << 1];

        //    }



        //}

        public static int readLine(string value, int length, ref int index, out string newLine) {
            for (var i = index; i < length; i++) {
                if (value[i] == '\r') {
                    var c = i - index;
                    index = i + 1;
                    if (index < length && value[index] == '\n') {
                        index++;
                        newLine = "\r\n";
                    } else {
                        newLine = "\r";
                    }
                    return c;
                }
                if (value[i] == '\n') {
                    var c = i - index;
                    index = i + 1;
                    newLine = "\n";
                    return c;
                }
            }

            var t = length - index;
            index = length;
            newLine = null;
            return t;
        }

        internal static void mark(params object[] values) {
            StackTrace stack = new StackTrace();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            var frame = stack.GetFrame(1);
            var m = frame.GetMethod();
            Console.Write(m.DeclaringType.Name + "::" + m.Name);
            foreach (var v in values) {
                Console.Write("    ");
                Console.Write(v);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static bool isCloserToAThanB(int value, int a, int b) {
            return b - value > value - a;
        }

        public static string newlineTypeToNewLine(DocumentLineFlags flags) {
            switch (flags) {
                case DocumentLineFlags.newLineTypeWin:
                    return "\r\n";
                case DocumentLineFlags.newLineTypeUnix:
                    return "\n";
                default:
                    return "\r";
            }
        }

        public static DocumentLineFlags newlineToNewLineType(string flags) {
            return flags.Length != 1 ? DocumentLineFlags.newLineTypeWin : flags[0] == '\r' ? DocumentLineFlags.newLineTypeMac : DocumentLineFlags.newLineTypeUnix;
        }


        public static bool isIndentChar(char c) {
            return c == ' ' || c == '\t' || c == '\u3000';
        }

        #region 控制字符

        private static readonly string[] c0Table = {
            "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS", "HT",
            "LF", "VT", "FF", "CR", "SO", "SI", "DLE", "DC1", "DC2", "DC3",
            "DC4", "NAK", "SYN", "ETB", "CAN", "EM", "SUB", "ESC", "FS", "GS",
            "RS", "US", "SPA"
        };

        private static readonly string[] delAndC1Table = {
            "DEL",
            "PAD", "HOP", "BPH", "NBH", "IND", "NEL", "SSA", "ESA", "HTS", "HTJ",
            "VTS", "PLD", "PLU", "RI", "SS2", "SS3", "DCS", "PU1", "PU2", "STS",
            "CCH", "MW", "SPA", "EPA", "SOS", "SGCI", "SCI", "CSI", "ST", "OSC",
            "PM", "APC"
        };

        /// <summary>
        /// 获取控制字符名。
        /// </summary>
        public static string getControlCharName(char controlCharacter) {
            if (controlCharacter <= 32) {
                return c0Table[controlCharacter];
            }

            if (controlCharacter >= 127 && controlCharacter <= 159) {
                return delAndC1Table[controlCharacter - 127];
            }

            return null;
        }

        #endregion

    }
}
