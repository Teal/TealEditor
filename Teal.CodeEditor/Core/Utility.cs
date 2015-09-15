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
    public static class Utility {

        ///// <summary>
        ///// 在指定数组插入指定数量元素。
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="array">原数组。</param>
        ///// <param name="length">原数组的长度。</param>
        ///// <param name="index">要插入的位置。</param>
        ///// <param name="count">要插入的数量。</param>
        ///// <returns>返回已插入的数组。</returns>
        //public static T[] insert<T>(T[] array, int length, int index, int count) {

        //    var restCount = length - index;

        //    // 判断数组是否需要扩容。
        //    if (length + count >= array.Length) {
        //        var result = new T[length + count];

        //        // 插入到数组末尾。
        //        if (restCount <= 0) {
        //            Array.Copy(array, result, length);
        //        } else {
        //            Array.Copy(array, result, index);
        //            Array.Copy(array, index, result, index + count, restCount);
        //        }

        //        return result;
        //    }

        //    if (restCount > 0) {
        //        for (int from = length, to = length + count; restCount > 0; restCount--) {
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
        ///// <param name="length">原数组的长度。</param>
        ///// <param name="index">要插入的位置。</param>
        ///// <param name="index">要插入的内容。</param>
        //public static void insert<T>(ref T[] array, ref int length, int index, T value) {

        //    // 检查是否需扩容。
        //    if (array.Length < ++length) {
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


        public static void appendArrayList<T>(ref T[] array, ref int length) {
            if (array.Length <= ++length) {
                var newArray = new T[array.Length << 1];
                Array.Copy(array, newArray, length);
                array = newArray;
            }
        }

        public static void prependArrayList<T>(ref T[] array, ref int length) {
            if (array.Length <= ++length) {
                var newArray = new T[array.Length << 1];
                Array.Copy(array, 0, newArray, 1, length);
                array = newArray;
            }
        }

        #region 字符串

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern string FastAllocateString(int length);

        public unsafe static void wstrcpy(char* dest, char* src, int charCount) {
            memcpy((byte*)dest, (byte*)src, charCount << 1);
        }

        /// <summary>
        /// 实现高效的内存复制算法。
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        /// <param name="byteCount"></param>
        public unsafe static void memcpy(byte* dest, byte* src, int byteCount) {
            switch (byteCount) {
                case 0:
                    return;
                case 1:
                    *dest = *src;
                    return;
                case 2:
                    *(short*)dest = *(short*)src;
                    return;
                case 3:
                    *(short*)dest = *(short*)src;
                    dest[2] = src[2];
                    return;
                case 4:
                    *(int*)dest = *(int*)src;
                    return;
                case 5:
                    *(int*)dest = *(int*)src;
                    dest[4] = src[4];
                    return;
                case 6:
                    *(int*)dest = *(int*)src;
                    *(short*)(dest + 4) = *(short*)(src + 4);
                    return;
                case 7:
                    *(int*)dest = *(int*)src;
                    *(short*)(dest + 4) = *(short*)(src + 4);
                    dest[6] = src[6];
                    return;
                case 8:
                    *(long*)dest = *(long*)src;
                    return;
                case 9:
                    *(long*)dest = *(long*)src;
                    dest[8] = src[8];
                    return;
                case 10:
                    *(long*)dest = *(long*)src;
                    *(short*)(dest + 8) = *(short*)(src + 8);
                    return;
                case 11:
                    *(long*)dest = *(long*)src;
                    *(short*)(dest + 8) = *(short*)(src + 8);
                    dest[10] = src[10];
                    return;
                case 12:
                    *(long*)dest = *(long*)src;
                    *(int*)(dest + 8) = *(int*)(src + 8);
                    return;
                case 13:
                    *(long*)dest = *(long*)src;
                    *(int*)(dest + 8) = *(int*)(src + 8);
                    dest[12] = src[12];
                    return;
                case 14:
                    *(long*)dest = *(long*)src;
                    *(int*)(dest + 8) = *(int*)(src + 8);
                    *(short*)(dest + 12) = *(short*)(src + 12);
                    return;
                case 15:
                    *(long*)dest = *(long*)src;
                    *(int*)(dest + 8) = *(int*)(src + 8);
                    *(short*)(dest + 12) = *(short*)(src + 12);
                    dest[14] = src[14];
                    return;
                case 16:
                    *(long*)dest = *(long*)src;
                    *(long*)(dest + 8) = *(long*)(src + 8);
                    return;
                default:
                    // 每 16 位一拷贝。
                    while (byteCount >= 16) {
                        *(long*)dest = *(long*)src;
                        *(long*)(dest + 8) = *(long*)(src + 8);
                        dest += 16;
                        src += 16;
                        byteCount -= 16;
                    }

                    if (byteCount >= 8) {
                        *(long*)dest = *(long*)src;
                        dest += 8;
                        src += 8;
                        byteCount -= 8;
                    }

                    if (byteCount >= 4) {
                        *(int*)dest = *(int*)src;
                        dest += 4;
                        src += 4;
                        byteCount -= 4;
                    }

                    if (byteCount >= 2) {
                        *(short*)dest = *(short*)src;
                        dest += 2;
                        src += 2;
                        byteCount -= 2;
                    }

                    if (byteCount >= 1) {
                        *dest = *src;
                    }
                    return;
            }
        }

        #endregion
    }
}
