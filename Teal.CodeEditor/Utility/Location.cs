using System;

namespace Teal.Compiler {

    /// <summary>
    /// ��ʾһ��Դ��λ�á�
    /// </summary>
    public struct Location : IEquatable<Location>, IComparable<Location> {

        #region ����

        /// <summary>
        /// ��ȡ�����õ�ǰλ�õ��кš��кŴ� 0 ��ʼ�� 
        /// </summary>
        public int line;

        /// <summary>
        /// ��ȡ�����õ�ǰλ�õ��кš��кŴ� 0 ��ʼ�� 
        /// </summary>
        public int column;

        #endregion

        #region �ӿ�

        /// <summary>
        /// ���ظ�ʵ������ȫ�޶���������
        /// </summary>
        /// <returns>
        /// ������ȫ�޶��������� <see cref="T:System.String" />��
        /// </returns>
        public override string ToString() {
            return $"{line}, {column}";
        }

        /// <summary>
        /// ���ش�ʵ���Ĺ�ϣ���롣
        /// </summary>
        /// <returns>һ�� 32 λ�з������������Ǹ�ʵ���Ĺ�ϣ���롣</returns>
        public override int GetHashCode() {
            return line.GetHashCode() ^ column.GetHashCode();
        }

        /// <summary>
        /// ָʾ��ʵ����ָ�������Ƿ���ȡ�
        /// </summary>
        /// <param name="obj">Ҫ�Ƚϵ���һ������</param>
        /// <returns>
        /// ��� <paramref name="obj"/> �͸�ʵ��������ͬ�����Ͳ���ʾ��ͬ��ֵ����Ϊ true������Ϊ false��
        /// </returns>
        public override bool Equals(object obj) {
            if (!(obj is Location))
                return false;
            return Equals((Location)obj);
        }

        /// <summary>
        /// ָʾ��ʵ����ָ�������Ƿ���ȡ�
        /// </summary>
        /// <param name="other">Ҫ�Ƚϵ���һ������</param>
        /// <returns>
        /// ��� <paramref name="other"/> �͸�ʵ��������ͬ�����Ͳ���ʾ��ͬ��ֵ����Ϊ true������Ϊ false��
        /// </returns>
        public bool Equals(Location other) {
            return this == other;
        }

        /// <summary>
        /// ��ȡ��ǰ�����ָ������Ĳ�ֵ��
        /// </summary>
        /// <param name="other">�ȽϵĶ���</param>
        /// <returns>�����ǰֵС�����ظ�������ͬ������ 0 ��</returns>
        public int CompareTo(Location other) {
            var lineOffset = line - other.line;

            return lineOffset > 0 ? 1 : lineOffset < 0 ? -1 : column > other.column ? 1 : column == other.column ? 0 : -1;
        }

        #endregion

        #region ����������

        /// <summary>
        /// ʵ�ֲ��� operator ==��
        /// </summary>
        /// <param name="locA">Ҫ����ĵ�1��ֵ��</param>
        /// <param name="locB">Ҫ����ĵ�2��ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static bool operator ==(Location locA, Location locB) {
            return locA.line == locB.line && locA.column == locB.column;
        }

        /// <summary>
        /// ʵ�ֲ��� operator !=��
        /// </summary>
        /// <param name="locA">Ҫ����ĵ�1��ֵ��</param>
        /// <param name="locB">Ҫ����ĵ�2��ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static bool operator !=(Location locA, Location locB) {
            return locA.line != locB.line || locA.column != locB.column;
        }

        /// <summary>
        /// ʵ�ֲ��� operator &lt;��
        /// </summary>
        /// <param name="locA">Ҫ����ĵ�1��ֵ��</param>
        /// <param name="locB">Ҫ����ĵ�2��ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static bool operator <(Location locA, Location locB) {
            return locA.CompareTo(locB) < 0;
        }

        /// <summary>
        /// ʵ�ֲ��� operator &gt;��
        /// </summary>
        /// <param name="locA">Ҫ����ĵ�1��ֵ��</param>
        /// <param name="locB">Ҫ����ĵ�2��ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static bool operator >(Location locA, Location locB) {
            return locA.CompareTo(locB) > 0;
        }

        /// <summary>
        /// ʵ�ֲ��� operator &lt;=��
        /// </summary>
        /// <param name="locA">Ҫ����ĵ�1��ֵ��</param>
        /// <param name="locB">Ҫ����ĵ�2��ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static bool operator <=(Location locA, Location locB) {
            return locA.CompareTo(locB) <= 0;
        }

        /// <summary>
        /// ʵ�ֲ��� operator &gt;=��
        /// </summary>
        /// <param name="locA">Ҫ����ĵ�1��ֵ��</param>
        /// <param name="locB">Ҫ����ĵ�2��ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static bool operator >=(Location locA, Location locB) {
            return locA.CompareTo(locB) >= 0;
        }

        /// <summary>
        /// ʵ�ֲ��� operator +��
        /// </summary>
        /// <param name="loc">Ҫ����ĵ�1��ֵ��</param>
        /// <param name="offset">Ҫ����ĵ�2��ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static Location operator +(Location loc, int offset) {
            loc.column += offset;
            return loc;
        }

        /// <summary>
        /// ʵ�ֲ��� operator -��
        /// </summary>
        /// <param name="loc">Ҫ����ĵ�1��ֵ��</param>
        /// <param name="offset">Ҫ����ĵ�2��ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static Location operator -(Location loc, int offset) {
            loc.column -= offset;
            return loc;
        }

        /// <summary>
        /// ʵ�ֲ��� operator +��
        /// </summary>
        /// <param name="loc">Ҫ�����ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static Location operator ++(Location loc) {
            loc.column++;
            return loc;
        }

        /// <summary>
        /// ʵ�ֲ��� operator -��
        /// </summary>
        /// <param name="loc">Ҫ�����ֵ��</param>
        /// <returns>�����Ľ����</returns>
        public static Location operator --(Location loc) {
            loc.column--;
            return loc;
        }

        #endregion

    }

}