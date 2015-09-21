using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个可动态扩展长度的数组。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("length = {length}"), DebuggerTypeProxy(typeof(ArrayList<>.DebugView)),]
    public struct ArrayList<T> : IList<T> where T : new() {

        /// <summary>
        /// 获取当前列表的所有数据。
        /// </summary>
        public T[] data;

        /// <summary>
        /// 存储当前数据的实际长度。
        /// </summary>
        private int _length;

        /// <summary>
        /// 获取或设置当前列表的长度。
        /// </summary>
        public int length {
            get {
                return _length;
            }
            set {
                if (value > data.Length) {
                    var newLength = data.Length << 1 + 1;
                    if (newLength < value) {
                        newLength = value;
                    }
                    if (newLength < 128) {
                        newLength = 128;
                    }
                    capacity = newLength;
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
                // 容器大小不能低于长度。
                if (value < _length) {
                    value = _length;
                }

                var newData = new T[value];
                Array.Copy(data, newData, _length);
                data = newData;
            }
        }

        /// <summary>
        /// 获取或设置指定索引的项。
        /// </summary>
        /// <param name="index">要获取的索引。</param>
        /// <returns>返回值。</returns>
        public T this[int index] {
            get {
                if (index < 0) {
                    index = 0;
                }

                if (index >= _length) {
                    return this[index] = new T();
                }

                return data[index];
            }
            set {
                if (index < 0) {
                    return;
                }

                if (index >= capacity) {
                    capacity = index + 2;
                }

                data[index] = value;
            }
        }

        /// <summary>
        /// 初始化 <see cref="ArrayList"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">预设的容器大小。</param>
        public ArrayList(int capacity = 2) {
            data = new T[capacity];
            _length = 0;
        }

        /// <summary>
        /// 向当前列表末尾添加项。
        /// </summary>
        /// <param name="item">要添加的项。</param>
        public void add(T item) {
            var last = _length;
            length = last + 1;
            data[last] = item;
        }

        /// <summary>
        /// 在指定位置插入项。
        /// </summary>
        /// <param name="index">插入的位置。</param>
        /// <param name="item">要插入的字符。</param>
        public void insert(int index, T item) {
            if (index < 0)
                index = 0;

            if (index < _length) {
                length++;
                Array.Copy(data, index, data, index + 1, _length - index - 1);
                data[index] = item;
                return;
            }

            if (index == _length) {
                add(item);
                return;
            }

            this[index] = item;

        }

        /// <summary>
        /// 删除指定索引的项。
        /// </summary>
        /// <param name="index"></param>
        public void removeAt(int index) {
            if (index < 0 || index >= _length)
                return;

            Array.Copy(data, index + 1, data, index, _length - index - 1);
            _length--;
        }

        /// <summary>
        /// 删除指定索引之间的项。
        /// </summary>
        /// <param name="startIndex">从零开始的搜索的起始索引。空数组中 0（零）为有效值。</param>
        /// <param name="length">要删除的部分中的元素数。</param>
        public void removeRange(int startIndex, int length) {
            var endIndex = startIndex + length;
            if (endIndex > _length) {
                endIndex = _length;
                length = endIndex - startIndex;
            }
            if (startIndex < 0) {
                startIndex = 0;
                length = endIndex;
            }

            Array.Copy(data, endIndex, data, startIndex, length);
            _length -= length;
        }

        /// <summary>
        /// 删除指定索引的项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// 如果已从集合中成功移除 <paramref name="item"/>，则为 true；否则为 false。如果在原始集合中没有找到 <paramref name="item"/>，该方法也会返回 false。
        /// </returns>
        public bool remove(T item) {
            var index = indexOf(item);
            if (index >= 0) {
                removeAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清空所有项。
        /// </summary>
        public void clear() => _length = 0;

        /// <summary>
        /// 查找指定项在当前列表的索引。
        /// </summary>
        /// <param name="item">要查找的项。</param>
        /// <returns></returns>
        public int indexOf(T item) => Array.IndexOf(data, item, 0, _length);

        /// <summary>
        /// 查找指定项在当前列表的索引。
        /// </summary>
        /// <param name="item">要查找的项。</param>
        /// <param name="startIndex">开始查找的索引。</param>
        /// <returns></returns>
        public int indexOf(T item, int startIndex) => Array.IndexOf(data, item, startIndex, _length - startIndex);

        /// <summary>
        /// 查找指定项在当前列表的索引。
        /// </summary>
        /// <param name="item">要查找的项。</param>
        /// <param name="startIndex">从零开始的搜索的起始索引。空数组中 0（零）为有效值。</param>
        /// <param name="length">要搜索的部分中的元素数。</param>
        /// <returns>如果在当前列表中从 <paramref name="startIndex"/> 开始、包含 <paramref name="length"/> 所指定的元素个数的这部分元素中，找到 <paramref name="item"/> 的匹配项，则为第一个匹配项的从零开始的索引；否则为 -1。</returns>
        public int indexOf(T item, int startIndex, int length) => Array.IndexOf(data, item, startIndex, length);

        /// <summary>
        /// 确定当前集合是否包含特定值。
        /// </summary>
        /// <param name="item">要查找的项。</param>
        /// <returns></returns>
        public bool contains(T item) => indexOf(item) >= 0;

        /// <summary>
        /// 返回当前列表的等效数组。
        /// </summary>
        /// <returns></returns>
        public T[] toArray() {
            var result = new T[_length];
            Array.Copy(data, result, _length);
            return result;
        }

        #region 迭代器

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 用于循环访问集合的枚举数。
        /// </returns>
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        private struct Enumerator : IEnumerator<T> {

            private readonly T[] _data;

            private readonly int _length;

            private int _index;

            public Enumerator(ArrayList<T> list) {
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
            public T Current => _data[_index];

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext() => ++_index < _length;

            public void Reset() => _index = -1;
        }

        #endregion

        #region 接口实现

        public int IndexOf(T item) => indexOf(item);

        public void Insert(int index, T item) => insert(index, item);

        public void RemoveAt(int index) => removeAt(index);

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator"/> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 将某项添加到 <see cref="T:System.Collections.Generic.ICollection`1"/> 中。
        /// </summary>
        /// <param name="item">要添加到 <see cref="T:System.Collections.Generic.ICollection`1"/> 的对象。</param><exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.ICollection`1"/> 为只读。</exception>
        public void Add(T item) => add(item);

        /// <summary>
        /// 从 <see cref="T:System.Collections.Generic.ICollection`1"/> 中移除所有项。
        /// </summary>
        /// <exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.ICollection`1"/> 为只读。</exception>
        public void Clear() => clear();

        /// <summary>
        /// 确定 <see cref="T:System.Collections.Generic.ICollection`1"/> 是否包含特定值。
        /// </summary>
        /// <returns>
        /// 如果在 <see cref="T:System.Collections.Generic.ICollection`1"/> 中找到 <paramref name="item"/>，则为 true；否则为 false。
        /// </returns>
        /// <param name="item">要在 <see cref="T:System.Collections.Generic.ICollection`1"/> 中定位的对象。</param>
        public bool Contains(T item) => contains(item);

        /// <summary>
        /// 从特定的 <see cref="T:System.Array"/> 索引开始，将 <see cref="T:System.Collections.Generic.ICollection`1"/> 的元素复制到一个 <see cref="T:System.Array"/> 中。
        /// </summary>
        /// <param name="array">作为从 <see cref="T:System.Collections.Generic.ICollection`1"/> 复制的元素的目标的一维 <see cref="T:System.Array"/>。<see cref="T:System.Array"/> 必须具有从零开始的索引。</param><param name="arrayIndex"><paramref name="array"/> 中从零开始的索引，从此索引处开始进行复制。</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> 为 null。</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> 小于 0。</exception><exception cref="T:System.ArgumentException">源 <see cref="T:System.Collections.Generic.ICollection`1"/> 中的元素数目大于从 <paramref name="arrayIndex"/> 到目标 <paramref name="array"/> 末尾之间的可用空间。</exception>
        public void CopyTo(T[] array, int arrayIndex) => Array.Copy(data, 0, array, arrayIndex, _length);

        /// <summary>
        /// 从 <see cref="T:System.Collections.Generic.ICollection`1"/> 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <returns>
        /// 如果已从 <see cref="T:System.Collections.Generic.ICollection`1"/> 中成功移除 <paramref name="item"/>，则为 true；否则为 false。如果在原始 <see cref="T:System.Collections.Generic.ICollection`1"/> 中没有找到 <paramref name="item"/>，该方法也会返回 false。
        /// </returns>
        /// <param name="item">要从 <see cref="T:System.Collections.Generic.ICollection`1"/> 中移除的对象。</param><exception cref="T:System.NotSupportedException"><see cref="T:System.Collections.Generic.ICollection`1"/> 为只读。</exception>
        public bool Remove(T item) => remove(item);

        /// <summary>
        /// 获取 <see cref="T:System.Collections.Generic.ICollection`1"/> 中包含的元素数。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.Collections.Generic.ICollection`1"/> 中包含的元素个数。
        /// </returns>
        public int Count => _length;

        /// <summary>
        /// 获取一个值，该值指示 <see cref="T:System.Collections.Generic.ICollection`1"/> 是否为只读。
        /// </summary>
        /// <returns>
        /// 如果 <see cref="T:System.Collections.Generic.ICollection`1"/> 为只读，则为 true；否则为 false。
        /// </returns>
        bool ICollection<T>.IsReadOnly => false;

        #endregion

        #region 调试视图

        class DebugView {
            private ArrayList<T> arrayList;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] items => arrayList.toArray();

            public DebugView(ArrayList<T> arrayList) {
                this.arrayList = arrayList;
            }
        }

        #endregion

    }

}
