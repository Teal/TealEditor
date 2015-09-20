using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个字符串缓存。
    /// </summary>
    public struct StringBuffer : IEnumerable<char>, IEquatable<StringBuffer> {

        /// <summary>
        /// 获取当前缓存的所有原始数据。
        /// </summary>
        public string data;

        /// <summary>
        /// 存储当前缓存的实际字符长度。
        /// </summary>
        private int _length;

        /// <summary>
        /// 获取当前字符串缓存的字符长度。
        /// </summary>
        public int length {
            get {
                return _length;
            }
            set {
                // 确保字符串长度足够。
                // 如果数据长度和字符串长度一致，说明当前字符串缓存的数据是只读的。
                // 此次必须重新创建一份长度不一致的字符串缓存对象。
                if (data.Length == _length || value >= data.Length) {
                    capacity = Math.Max(_length << 1, value + 2);
                }
                _length = value;
            }
        }

        /// <summary>
        /// 获取或设置当前字符串缓存最多可容纳的字符数。
        /// </summary>
        public int capacity {
            get {
                return data.Length;
            }
            set {
                // 容器大小不能低于长度。
                if (value < _length) {
                    value = _length;
                }

                var newData = allocateString(value);
                unsafe
                {
                    fixed (char* src = data)
                    {
                        fixed (char* dest = newData)
                        {
                            wstrcpy(dest, src, _length);
                        }
                    }
                }
                data = newData;
            }
        }

        /// <summary>
        /// 获取或设置当前字符串缓存指定位置的字符。
        /// </summary>
        /// <param name="index">要获取的索引。</param>
        /// <returns>返回对应的字符。</returns>
        public char this[int index] {
            get {
                return index >= 0 && index < _length ? data[index] : '\0';
            }
            set {
                if (index < 0) {
                    return;
                }

                if (index >= capacity) {
                    capacity = index + 2;
                }

                unsafe
                {
                    fixed (char* dest = data)
                    {
                        dest[index] = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置当前字符串缓存指定区域的子字符串。
        /// </summary>
        /// <param name="startIndex">要获取的开始索引。</param>
        /// <param name="endIndex">要获取的结束索引。</param>
        /// <returns>返回子字符串。</returns>
        public string this[int startIndex, int endIndex] {
            get {
                if (startIndex < 0) {
                    startIndex = 0;
                }

                if (endIndex > _length) {
                    endIndex = _length;
                }

                return startIndex >= endIndex ? String.Empty : data.Substring(startIndex, endIndex - startIndex);
            }
            set {
                remove(startIndex, endIndex - startIndex);
                insert(startIndex, value);
            }
        }

        /// <summary>
        /// 返回当前字符串缓存的字符串形式。
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString() => data.Substring(0, _length);

        /// <summary>
        /// 获取从指定位置开始指定长度的子字符串。
        /// </summary>
        /// <param name="startIndex">开始的索引。</param>
        /// <returns>返回子字符串。</returns>
        public string substring(int startIndex) => substring(startIndex, length - startIndex);

        /// <summary>
        /// 获取从指定位置开始指定长度的子字符串。
        /// </summary>
        /// <param name="startIndex">开始的索引。</param>
        /// <param name="length">获取的字符串长度。</param>
        /// <returns>返回子字符串。</returns>
        public string substring(int startIndex, int length) {
            return data.Substring(startIndex, length);
        }

        /// <summary>
        /// 初始化 <see cref="StringBuffer"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">预设的容器大小。</param>
        public StringBuffer(int capacity = 2) {
            data = allocateString(capacity);
            _length = 0;
        }

        /// <summary>
        /// 初始化 <see cref="StringBuffer"/> 类的新实例。
        /// </summary>
        /// <param name="data">预设的数据。</param>
        public StringBuffer(string data) {
            this.data = data;
            _length = data.Length;
        }

        /// <summary>
        /// 设置当前缓存的原始数据。
        /// </summary>
        /// <param name="data">要设置的数据。</param>
        public void set(string data) {
            this.data = data;
            _length = data.Length;
        }

        #region 字符串底层操作

        private static string allocateString(int length) {
            return new String('\0', length);
        }

        private unsafe static void wstrcpy(char* dest, char* src, int charCount) {
            memcpy((byte*)dest, (byte*)src, charCount << 1);
        }

        /// <summary>
        /// 实现高效的内存复制算法。
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        /// <param name="byteCount"></param>
        private unsafe static void memcpy(byte* dest, byte* src, int byteCount) {
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

        #region 追加

        /// <summary>
        /// 向当前字符串缓存追加字符。
        /// </summary>
        /// <param name="value">要追加的字符。</param>
        public void append(char value) {
            var currentLength = _length;
            length++;
            unsafe
            {
                fixed (char* dest = data)
                {
                    dest[currentLength] = value;
                }
            }
        }

        /// <summary>
        /// 向当前字符串缓存追加字符。
        /// </summary>
        /// <param name="value">要追加的字符。</param>
        /// <param name="repeatCount">字符重复的次数。</param>
        public void append(char value, int repeatCount) {
            var currentLength = length;
            length += repeatCount;
            unsafe
            {
                fixed (char* dest = data)
                {
                    while (repeatCount-- > 0) {
                        dest[currentLength++] = value;
                    }
                }
            }
        }

        /// <summary>
        /// 向当前字符串缓存追加字符。
        /// </summary>
        /// <param name="value">要追加的字符指针。</param>
        /// <param name="charCount">要追加的字符数。</param>
        public unsafe void append(char* value, int charCount) {
            var currentLength = length;
            length += charCount;
            fixed (char* n = data)
            {
                wstrcpy(n + currentLength, value, charCount);
            }
        }

        /// <summary>
        /// 向当前字符串缓存追加字符。
        /// </summary>
        /// <param name="value">要追加的字符串。</param>
        public void append(string value) => append(value, 0, value.Length);

        /// <summary>
        /// 向当前字符串缓存追加数据。
        /// </summary>
        /// <param name="value">要追加的字符串。</param>
        /// <param name="startIndex">插入的起始位置。</param>
        /// <param name="charCount">插入的个数。</param>
        public void append(string value, int startIndex, int charCount) {
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex + charCount <= value.Length);
            unsafe
            {
                fixed (char* src = value)
                {
                    append(src + startIndex, charCount);
                }
            }
        }

        /// <summary>
        /// 向当前字符串缓存追加字符。
        /// </summary>
        /// <param name="value">要追加的字符数组。</param>
        /// <param name="startIndex">数组的起始位置。</param>
        /// <param name="charCount">要追加的字符数。</param>
        public void append(char[] value, int startIndex, int charCount) {
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex + charCount <= value.Length);
            unsafe
            {
                fixed (char* src = value)
                {
                    append(src + startIndex, charCount);
                }
            }
        }

        #endregion

        #region 插入

        /// <summary>
        /// 在指定位置插入一个字符。
        /// </summary>
        /// <param name="index">插入的位置。</param>
        /// <param name="value">要插入的字符串。</param>
        /// <param name="charCount">插入的长度。</param>
        public unsafe void insert(int index, char* value, int charCount) {
            if (index < 0)
                index = 0;

            if (index < _length) {
                length += charCount;
                fixed (char* dest = data)
                {
                    var cs = dest + length;
                    var cd = cs + charCount;

                    var ce = dest + index;

                    while (cs >= ce) {
                        *--cd = *--cs;
                    }

                    wstrcpy(ce, value, charCount);

                }
                return;
            }

            if (index == _length) {
                append(value, charCount);
                return;
            }

            length = index + charCount;
            fixed (char* dest = data)
            {
                wstrcpy(dest + index, value, charCount);
            }

        }

        /// <summary>
        /// 在指定位置插入一个字符。
        /// </summary>
        /// <param name="index">插入的位置。</param>
        /// <param name="value">要插入的字符。</param>
        public void insert(int index, char value) {
            if (index < 0)
                index = 0;

            if (index < _length) {
                length++;
                unsafe
                {
                    fixed (char* dest = data)
                    {
                        var cs = dest + length;
                        var cd = cs + 1;

                        var ce = dest + index;

                        while (cs >= ce) {
                            *--cd = *--cs;
                        }

                        *ce = value;

                    }
                }
                return;
            }

            if (index == _length) {
                append(value);
                return;
            }

            this[index] = value;

        }

        /// <summary>
        /// 在指定位置插入一个字符串。
        /// </summary>
        /// <param name="index">插入的位置。</param>
        /// <param name="value">要插入的字符串。</param>
        /// <param name="startIndex">开始的索引。</param>
        /// <param name="length">插入的长度。</param>
        public void insert(int index, string value, int startIndex, int length) {
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex + length <= value.Length);
            unsafe
            {
                fixed (char* p = value)
                {
                    insert(index, p + startIndex, length);
                }
            }
        }

        /// <summary>
        /// 在指定位置插入一个字符串。
        /// </summary>
        /// <param name="index">插入的位置。</param>
        /// <param name="value">要插入的字符串。</param>
        public void insert(int index, string value) => insert(index, value, 0, value.Length);

        /// <summary>
        /// 在指定位置插入一个字符数组。
        /// </summary>
        /// <param name="index">插入的位置。</param>
        /// <param name="value">要插入的字符数组。</param>
        /// <param name="startIndex">开始的索引。</param>
        /// <param name="length">插入的长度。</param>
        public void insert(int index, char[] value, int startIndex, int length) {
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex + length <= value.Length);
            unsafe
            {
                fixed (char* p = value)
                {
                    insert(index, p + startIndex, length);
                }
            }
        }

        /// <summary>
        /// 在指定位置插入一个字符数组。
        /// </summary>
        /// <param name="index">插入的位置。</param>
        /// <param name="value">要插入的字符数组。</param>
        public void insert(int index, char[] value) => insert(index, value, 0, value.Length);

        #endregion

        #region 删除

        /// <summary>
        /// 清空当前字符串缓存内容。
        /// </summary>
        public void clear() => _length = 0;

        /// <summary>
        /// 从当前缓存删除指定索引的字符串。
        /// </summary>
        /// <param name="startIndex">开始的索引。</param>
        public void remove(int startIndex) => remove(startIndex, _length - startIndex);

        /// <summary>
        /// 从当前缓存删除指定索引的字符串。
        /// </summary>
        /// <param name="startIndex">开始的索引。</param>
        /// <param name="charCount">删除的长度。</param>
        public void remove(int startIndex, int charCount) {
            Debug.Assert(startIndex >= 0);
            Debug.Assert(startIndex + charCount <= _length);
            unsafe
            {
                fixed (char* p = data)
                {
                    var ps = p + startIndex;

                    var copyLength = length - startIndex - charCount;
                    wstrcpy(ps, ps + charCount, copyLength);

                    _length -= charCount;
                }
            }
        }

        #endregion

        #region 迭代器

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 用于循环访问集合的枚举数。
        /// </returns>
        public IEnumerator<char> GetEnumerator() => new Enumerator(this);

        private struct Enumerator : IEnumerator<char> {

            private readonly string _data;

            private readonly int _length;

            private int _index;

            public Enumerator(StringBuffer list) {
                _data = list.data;
                _length = list._length;
                _index = -1;
            }

            /// <summary>
            /// 获取集合中位于枚举数当前位置的元素。
            /// </summary>
            /// <returns>
            /// 集合中位于枚举数当前位置的元素。
            /// </returns>
            public char Current => _data[_index];

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext() => ++_index < _length;

            public void Reset() => _index = -1;
        }

        #endregion

        #region 接口实现

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator"/> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IEquatable<StringBuffer>.Equals(StringBuffer other) {
            return _length == other._length && String.CompareOrdinal(data, 0, other.data, 0, _length) == 0;
        }

        #endregion

    }

}
