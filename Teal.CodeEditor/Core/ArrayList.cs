using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个可动态扩展长度的数组。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ArrayList<T> where T : struct {

        /// <summary>
        /// 获取当前列表的所有数据。
        /// </summary>
        public T[] data { get; private set; }

        /// <summary>
        /// 存储当前数据的实际长度。
        /// </summary>
        private int _length;

        /// <summary>
        /// 获取当前列表的长度。
        /// </summary>
        public int length {
            get {
                return _length;
            }
            set {
                if (value >= capacity) {
                    capacity = value < 2048 ? value << 1 : value + 2048;
                }
                _length = value;
            }
        }

        /// <summary>
        /// 获取或设置当前数组最多可容纳的项数。
        /// </summary>
        public int capacity {
            get {
                return data.Length;
            }
            set {
                var dataLength = Math.Min(_length, value);
                var newData = new T[value];
                Array.Copy(data, newData, dataLength);
                data = newData;
            }
        }

        public ArrayList(int capicity = 20) {
            data = new T[capicity];
            _length = 0;
        }

        /// <summary>
        /// 向当前列表添加项。
        /// </summary>
        /// <param name="value">要添加的项。</param>
        public void add(T value) {
            int currentLength = length;
            length++;
            data[currentLength] = value;
        }

        /// <summary>
        /// 在指定位置插入一个字符。
        /// </summary>
        /// <param name="index">插入的位置。</param>
        /// <param name="value">要插入的字符。</param>
        public void insert(int index, T value) {
            if (index >= _length) {
                append(value);
                return;
            }
            int currentLength = length;
            length++;
            fixed (char* c = chars)
            {
                fixed (CharStyles* s = styles)
                {

                    char* cs = c + length;
                    char* cd = cs + 1;

                    CharStyles* ss = s + length;
                    CharStyles* sd = ss + 1;

                    char* ce = c + index;

                    while (cs >= ce) {
                        *--cd = *--cs;
                        *--sd = *--ss;
                    }

                    *ce = value;

                }
            }
        }

    }

}
