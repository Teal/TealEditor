using System;

namespace Teal.Compiler {

    /// <summary>
    /// 表示一个源码位置。
    /// </summary>
    public struct Location : IEquatable<Location>, IComparable<Location> {

        #region 属性

        /// <summary>
        /// 获取或设置当前位置的行号。行号从 0 开始。 
        /// </summary>
        public int line;

        /// <summary>
        /// 获取或设置当前位置的列号。列号从 0 开始。 
        /// </summary>
        public int column;

        #endregion

        #region 接口

        /// <summary>
        /// 返回该实例的完全限定类型名。
        /// </summary>
        /// <returns>
        /// 包含完全限定类型名的 <see cref="T:System.String" />。
        /// </returns>
        public override string ToString() {
            return $"{line}, {column}";
        }

        /// <summary>
        /// 返回此实例的哈希代码。
        /// </summary>
        /// <returns>一个 32 位有符号整数，它是该实例的哈希代码。</returns>
        public override int GetHashCode() {
            return line.GetHashCode() ^ column.GetHashCode();
        }

        /// <summary>
        /// 指示此实例与指定对象是否相等。
        /// </summary>
        /// <param name="obj">要比较的另一个对象。</param>
        /// <returns>
        /// 如果 <paramref name="obj"/> 和该实例具有相同的类型并表示相同的值，则为 true；否则为 false。
        /// </returns>
        public override bool Equals(object obj) {
            if (!(obj is Location))
                return false;
            return Equals((Location)obj);
        }

        /// <summary>
        /// 指示此实例与指定对象是否相等。
        /// </summary>
        /// <param name="other">要比较的另一个对象。</param>
        /// <returns>
        /// 如果 <paramref name="other"/> 和该实例具有相同的类型并表示相同的值，则为 true；否则为 false。
        /// </returns>
        public bool Equals(Location other) {
            return this == other;
        }

        /// <summary>
        /// 获取当前对象和指定对象的差值。
        /// </summary>
        /// <param name="other">比较的对象。</param>
        /// <returns>如果当前值小，返回负数，相同，返回 0 。</returns>
        public int CompareTo(Location other) {
            var lineOffset = line - other.line;

            return lineOffset > 0 ? 1 : lineOffset < 0 ? -1 : column > other.column ? 1 : column == other.column ? 0 : -1;
        }

        #endregion

        #region 操作符重载

        /// <summary>
        /// 实现操作 operator ==。
        /// </summary>
        /// <param name="locA">要计算的第1个值。</param>
        /// <param name="locB">要计算的第2个值。</param>
        /// <returns>操作的结果。</returns>
        public static bool operator ==(Location locA, Location locB) {
            return locA.line == locB.line && locA.column == locB.column;
        }

        /// <summary>
        /// 实现操作 operator !=。
        /// </summary>
        /// <param name="locA">要计算的第1个值。</param>
        /// <param name="locB">要计算的第2个值。</param>
        /// <returns>操作的结果。</returns>
        public static bool operator !=(Location locA, Location locB) {
            return locA.line != locB.line || locA.column != locB.column;
        }

        /// <summary>
        /// 实现操作 operator &lt;。
        /// </summary>
        /// <param name="locA">要计算的第1个值。</param>
        /// <param name="locB">要计算的第2个值。</param>
        /// <returns>操作的结果。</returns>
        public static bool operator <(Location locA, Location locB) {
            return locA.CompareTo(locB) < 0;
        }

        /// <summary>
        /// 实现操作 operator &gt;。
        /// </summary>
        /// <param name="locA">要计算的第1个值。</param>
        /// <param name="locB">要计算的第2个值。</param>
        /// <returns>操作的结果。</returns>
        public static bool operator >(Location locA, Location locB) {
            return locA.CompareTo(locB) > 0;
        }

        /// <summary>
        /// 实现操作 operator &lt;=。
        /// </summary>
        /// <param name="locA">要计算的第1个值。</param>
        /// <param name="locB">要计算的第2个值。</param>
        /// <returns>操作的结果。</returns>
        public static bool operator <=(Location locA, Location locB) {
            return locA.CompareTo(locB) <= 0;
        }

        /// <summary>
        /// 实现操作 operator &gt;=。
        /// </summary>
        /// <param name="locA">要计算的第1个值。</param>
        /// <param name="locB">要计算的第2个值。</param>
        /// <returns>操作的结果。</returns>
        public static bool operator >=(Location locA, Location locB) {
            return locA.CompareTo(locB) >= 0;
        }

        /// <summary>
        /// 实现操作 operator +。
        /// </summary>
        /// <param name="loc">要计算的第1个值。</param>
        /// <param name="offset">要计算的第2个值。</param>
        /// <returns>操作的结果。</returns>
        public static Location operator +(Location loc, int offset) {
            loc.column += offset;
            return loc;
        }

        /// <summary>
        /// 实现操作 operator -。
        /// </summary>
        /// <param name="loc">要计算的第1个值。</param>
        /// <param name="offset">要计算的第2个值。</param>
        /// <returns>操作的结果。</returns>
        public static Location operator -(Location loc, int offset) {
            loc.column -= offset;
            return loc;
        }

        /// <summary>
        /// 实现操作 operator +。
        /// </summary>
        /// <param name="loc">要计算的值。</param>
        /// <returns>操作的结果。</returns>
        public static Location operator ++(Location loc) {
            loc.column++;
            return loc;
        }

        /// <summary>
        /// 实现操作 operator -。
        /// </summary>
        /// <param name="loc">要计算的值。</param>
        /// <returns>操作的结果。</returns>
        public static Location operator --(Location loc) {
            loc.column--;
            return loc;
        }

        #endregion

    }

}