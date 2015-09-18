using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示特定平台的绘图器。
    /// </summary>
    public sealed class Painter {

        #region 核心

        /// <summary>
        /// 当前画板的设备。
        /// </summary>
        private IntPtr _hdc;

        /// <summary>
        /// 当前画板的兼容设备。
        /// </summary>
        private readonly IntPtr _measureDC;

        /// <summary>
        /// 初始化新的 Painter 对象。
        /// </summary>
        /// <param name="initalFont">初始字体。</param>
        public Painter(Font initalFont) {
            _hdc = _measureDC = Win32Api.CreateCompatibleDC(IntPtr.Zero);
            setFontCore(initalFont);
        }

        ~Painter() {
            Win32Api.DeleteDC(_measureDC);
        }

        /// <summary>
        /// 获取一个颜色的 RGB 值。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static uint toRGB(Color color) {
            var num = (uint)color.ToArgb() & 16777215;
            return (num >> 16) + (num & 65280) + ((num & 255) << 16);
        }

        /// <summary>
        /// 根据 RGB 值获取颜色对象。
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color fromRGB(uint color) {
            var colorId = (int)color;
            return colorId < 0 ? Color.Transparent : Color.FromArgb(colorId & 255, colorId >> 8 & 255, colorId >> 16 & 255);
        }

        /// <summary>
        /// 获取或设置当前画布的前景色。
        /// </summary>
        public uint foreColor {
            get {
                return Win32Api.GetDCPenColor(_hdc);
            }
            set {
                Win32Api.SetDCPenColor(_hdc, value);
            }
        }

        /// <summary>
        /// 获取或设置当前画布的填充色。
        /// </summary>
        public uint fillColor {
            get {
                return Win32Api.GetDCBrushColor(_hdc);
            }
            set {
                Win32Api.SetDCBrushColor(_hdc, value);
            }
        }

        /// <summary>
        /// 获取或设置当前画布的背景色。
        /// </summary>
        public uint backColor {
            get {
                return Win32Api.GetBkColor(_hdc);
            }
            set {
                Win32Api.SetBkColor(_hdc, value);
            }
        }

        /// <summary>
        /// 获取或设置当前画布的文本颜色。
        /// </summary>
        public uint textColor {
            get {
                return Win32Api.GetTextColor(_hdc);
            }
            set {
                Win32Api.SetTextColor(_hdc, value);
            }
        }

        /// <summary>
        /// 启用对指定图面的绘图操作。
        /// </summary>
        /// <param name="graphics">要描绘的图形。</param>
        public void beginPaint(Graphics graphics) {
            _hdc = graphics.GetHdc();

            Win32Api.SelectObject(_hdc, Win32Api.GetStockObject(Win32Api.StockObjects.DC_BRUSH));
            Win32Api.SelectObject(_hdc, Win32Api.GetStockObject(Win32Api.StockObjects.DC_PEN));
            Win32Api.SetBkMode(_hdc, Win32Api.BkMode.TRANSPARENT);
            Win32Api.SelectObject(_hdc, _currentFontInfo.hFont);

        }

        /// <summary>
        /// 关闭对指定图面的绘图操作。
        /// </summary>
        /// <param name="graphics">要描绘的图形。</param>
        public void endPaint(Graphics graphics) {
            graphics.ReleaseHdc(_hdc);
            _hdc = _measureDC;
        }

        #endregion

        #region 字体

        /// <summary>
        /// 存储已缓存的字体信息。
        /// </summary>
        public sealed class FontInfo {

            /// <summary>
            /// 获取当前字体。
            /// </summary>
            public readonly Font font;

            /// <summary>
            /// 获取当前字体的句柄。
            /// </summary>
            public readonly IntPtr hFont;

            /// <summary>
            /// 获取当前字体的各个字符的宽度。
            /// </summary>
            public readonly int[] charWidths = new int[65536];

            /// <summary>
            /// 获取当前字体前导高度。
            /// </summary>
            public readonly int leadingHeight;

            /// <summary>
            /// 获取当前字体基线高度。
            /// </summary>
            public readonly int baselineHeight;

            /// <summary>
            /// 获取当前字体的高度。
            /// </summary>
            public readonly int lineHeight;

            public FontInfo(Font font, IntPtr hdc) {
                this.font = font;
                hFont = font.ToHfont();

                Win32Api.SelectObject(hdc, hFont);

                // 计算字体相关常量。
                Win32Api.TextMetrics tm;
                Win32Api.GetTextMetrics(hdc, out tm);

                lineHeight = tm.Height;
                leadingHeight = tm.PublicLeading;
                baselineHeight = tm.Accent;

                // 计算每个字符宽度。

                // 32 - 127
                Win32Api.getCharABCWidths(hdc, 32, 127, charWidths, 32);

                // 设置控制字符宽度。 0 - 32
                for (var i = 0; i < 32; i++) {
                    charWidths[i] = 0;
                    foreach (char c in c0Table[i]) {
                        charWidths[i] += charWidths[c];
                    }
                }

                // 127 - 160
                for (var i = 127; i < 160; i++) {
                    charWidths[i] = 0;
                    foreach (char c in delAndC1Table[i - 127]) {
                        charWidths[i] += charWidths[c];
                    }
                }

            }

        }

        /// <summary>
        /// 存储所有可用的字体。
        /// </summary>
        private static Dictionary<Font, FontInfo> _fonts = new Dictionary<Font, FontInfo>();

        /// <summary>
        /// 存储当前的字体信息。
        /// </summary>
        private FontInfo _currentFontInfo;

        /// <summary>
        /// 创建并初始化字体。
        /// </summary>
        /// <param name="value"></param>
        private void setFontCore(Font value) {
            _fonts[value] = _currentFontInfo = new FontInfo(value, _hdc);
        }

        /// <summary>
        /// 获取或设置当前画布的字体。
        /// </summary>
        public Font font {
            get {
                return _currentFontInfo.font;
            }
            set {

                // 如果缓存的字体不同，则尝试重新计算字体缓存。
                if (_currentFontInfo.font != value && !_fonts.TryGetValue(value, out _currentFontInfo)) {
                    setFontCore(value);
                }

                Win32Api.SelectObject(_hdc, _currentFontInfo.hFont);

            }
        }

        /// <summary>
        /// 获取或设置当前画布的字体样式。
        /// </summary>
        public FontStyle fontStyle {
            get {
                return font.Style;
            }
            set {
                font = new Font(font, value);
            }
        }

        /// <summary>
        /// 获取或设置当前画布的字体大小。
        /// </summary>
        public int fontSize {
            get {
                return (int)font.SizeInPoints;
            }
            set {
                font = new Font(fontFamily, (float)value, fontStyle);
            }
        }

        /// <summary>
        /// 获取或设置当前画布的字体名称。
        /// </summary>
        public FontFamily fontFamily {
            get {
                return font.FontFamily;
            }
            set {
                font = new Font(value, fontSize);
            }
        }

        /// <summary>
        /// 获取当前字体的平均宽度。
        /// </summary>
        public int fontWidth {
            get {
                return _currentFontInfo.charWidths['a'];
            }
        }

        /// <summary>
        /// 获取当前字体的行高。
        /// </summary>
        public int lineHeight {
            get {
                return _currentFontInfo.lineHeight;
            }
        }

        /// <summary>
        /// 获取当前字体基线高度。
        /// </summary>
        public int baselineHeight {
            get {
                return _currentFontInfo.baselineHeight;
            }
        }

        /// <summary>
        /// 获取当前字体前导高度。
        /// </summary>
        public int leadingHeight {
            get {
                return _currentFontInfo.leadingHeight;
            }
        }

        private static readonly string[] c0Table = {
            "NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", "BS", "HT",
            "LF", "VT", "FF", "CR", "SO", "SI", "DLE", "DC1", "DC2", "DC3",
            "DC4", "NAK", "SYN", "ETB", "CAN", "EM", "SUB", "ESC", "FS", "GS",
            "RS", "US"
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
        public static string getControlCharacterName(char controlCharacter) {
            if (controlCharacter < 32) {
                return c0Table[controlCharacter];
            }

            if (controlCharacter >= 127 && controlCharacter <= 159) {
                return delAndC1Table[controlCharacter - 127];
            }

            return null;
        }

        #endregion

        #region 绘制和测量文本

        /// <summary>
        /// 计算加上指定字符串后新坐标。
        /// </summary>
        /// <param name="x">当前的左边距。</param>
        /// <param name="s">追加的字符。</param>
        /// <returns>返回添加指定字符后的新坐标。</returns>
        public int alignString(int x, string s) {
            for (var i = 0; i < s.Length; i++) {
                x = alignChar(x, s[i]);
            }
            return x;
        }

        /// <summary>
        /// 计算加上指定字符串后新坐标。
        /// </summary>
        /// <param name="x">当前的左边距。</param>
        /// <param name="s">追加的字符数组。</param>
        /// <param name="startIndex">开始的索引。</param>
        /// <param name="endIndex">结束的索引。</param>
        /// <returns>返回添加指定字符后的新坐标。</returns>
        public int alignChars(int x, char[] chars, int startIndex, int endIndex) {
            for (var i = startIndex; i < endIndex; i++) {
                x = alignChar(x, chars[i]);
            }
            return x;
        }

        /// <summary>
        /// 测量指定字符的宽度。
        /// </summary>
        /// <param name="c">要测量的字符。</param>
        /// <returns>返回字符宽度。</returns>
        public int measureString(char c) {
            var width = _currentFontInfo.charWidths[c];
            if (width == 0) {
                Size size;
                unsafe
                {
                    Win32Api.GetTextExtentPoint32(_measureDC, &c, 1, out size);
                }
                _currentFontInfo.charWidths[c] = width = size.Width;
            }

            return width;
        }

        /// <summary>
        /// 测量指定字符宽度。
        /// </summary>
        /// <param name="value">要测量的字符数组。</param>
        /// <param name="length">要测量的长度。</param>
        /// <returns>返回字符宽度。</returns>
        public unsafe int measureString(char* value, int length) {
            var sum = 0;
            for (var end = value + length; value < end; value++) {
                sum += measureString(*value);
            }
            return sum;
        }

        /// <summary>
        /// 测量指定字符串宽度。
        /// </summary>
        /// <param name="value">要测量的字符数组。</param>
        /// <param name="length">要测量的长度。</param>
        /// <returns>返回字符串宽度。</returns>
        public unsafe int measureString(char[] value, int length) {
            fixed (char* p = value)
            {
                return measureString(p, length);
            }
        }

        /// <summary>
        /// 测量指定字符串宽度。
        /// </summary>
        /// <param name="value">要测量的字符数组。</param>
        /// <param name="startIndex">开始测量的索引。</param>
        /// <param name="length">要测量的长度。</param>
        /// <returns>返回字符串宽度。</returns>
        public unsafe int measureString(char[] value, int startIndex, int length) {
            fixed (char* p = value)
            {
                return measureString(p + startIndex, length);
            }
        }

        /// <summary>
        /// 测量指定字符串宽度。
        /// </summary>
        /// <param name="value">要测量的字符串。</param>
        /// <param name="length">要测量的长度。</param>
        /// <returns>返回字符串宽度。</returns>
        public unsafe int measureString(string value) {
            fixed (char* p = value)
            {
                return measureString(p, value.Length);
            }
        }

        /// <summary>
        /// 绘制单行字符串。
        /// </summary>
        /// <param name="value">要绘制的字符串。</param>
        /// <param name="length">要绘制的长度。</param>
        /// <param name="pt">绘制的位置。</param>
        public unsafe void drawString(char* value, int length, int x, int y) {
            Win32Api.ExtTextOut(_hdc, x, y, 0, null, value, length, null);
        }

        /// <summary>
        /// 绘制单行字符串。
        /// </summary>
        /// <param name="value">要绘制的字符数组。</param>
        /// <param name="pt">绘制的位置。</param>
        public unsafe void drawString(string value, int x, int y) {
            fixed (char* p = value)
            {
                drawString(p, value.Length, x, y);
            }
        }

        /// <summary>
        /// 绘制单行字符串。
        /// </summary>
        /// <param name="value">要绘制的字符数组。</param>
        /// <param name="startIndex">开始绘制的索引。</param>
        /// <param name="length">要绘制的长度。</param>
        /// <param name="pt">绘制的位置。</param>
        public unsafe void drawString(char[] value, int startIndex, int length, int x, int y) {
            fixed (char* p = value)
            {
                drawString(p + startIndex, length, x, y);
            }
        }

        /// <summary>
        /// 绘制单行字符串。
        /// </summary>
        /// <param name="value">要绘制的字符数组。</param>
        /// <param name="startIndex">开始绘制的索引。</param>
        /// <param name="length">要绘制的长度。</param>
        /// <param name="pt">绘制的位置。</param>
        public unsafe void drawString(string value, int startIndex, int length, int x, int y) {
            fixed (char* p = value)
            {
                drawString(p + startIndex, length, x, y);
            }
        }

        /// <summary>
        /// 绘制单行字符串。
        /// </summary>
        /// <param name="value">要绘制的字符串。</param>
        /// <param name="length">要绘制的长度。</param>
        /// <param name="rect">绘制的区域。如果超过此区域将自动裁剪。</param>
        public unsafe void drawString(char* value, int length, Win32Api.Rect rect) {
            Win32Api.ExtTextOut(_hdc, rect.left, rect.top, 4, ref rect, value, length, null);
        }

        /// <summary>
        /// 绘制单行字符串。
        /// </summary>
        /// <param name="value">要绘制的字符数组。</param>
        /// <param name="rect">绘制的区域。如果超过此区域将自动裁剪。</param>
        public unsafe void drawString(string value, Win32Api.Rect rect) {
            fixed (char* p = value)
            {
                drawString(p, value.Length, rect);
            }
        }

        /// <summary>
        /// 绘制单行字符串。
        /// </summary>
        /// <param name="value">要绘制的字符数组。</param>
        /// <param name="value">开始绘制的索引。</param>
        /// <param name="length">要绘制的长度。</param>
        /// <param name="rect">绘制的区域。如果超过此区域将自动裁剪。</param>
        public unsafe void drawString(char[] value, int startIndex, int length, Win32Api.Rect rect) {
            fixed (char* p = value)
            {
                drawString(p + startIndex, length, rect);
            }
        }

        /// <summary>
        /// 绘制多行文本。
        /// </summary>
        /// <param name="value">要绘制的内容。</param>
        /// <param name="rect">绘制的区域。</param>
        /// <param name="clipped">指示是否裁剪。</param>
        public void drawText(string value, Win32Api.Rect rect) {
            Win32Api.DrawText(_hdc, value, value.Length, ref rect, 2048);
        }

        /// <summary>
        /// 绘制多行文本。
        /// </summary>
        /// <param name="value">要绘制的内容。</param>
        /// <param name="rect">绘制的区域。</param>
        /// <param name="clipped">指示是否裁剪。</param>
        public void drawText(string value, Point pt) {
            Win32Api.Rect rect = new Win32Api.Rect(pt.X, pt.Y, pt.X, pt.Y);
            Win32Api.DrawText(_hdc, value, value.Length, ref rect, 2304);
        }

        #endregion

        #region 绘制多边形和椭圆

        /// <summary>
        /// 绘制指定的矩形。
        /// </summary>
        /// <param name="left">要绘制的左边距。</param>
        /// <param name="top">要绘制的上边距。</param>
        /// <param name="right">要绘制的右边距。</param>
        /// <param name="bottom">要绘制的下边距。</param>
        public void drawRectangle(int left, int top, int right, int bottom) {
            Win32Api.Rect rect = new Win32Api.Rect(left, top, right, bottom);
            Win32Api.FrameRect(_hdc, ref rect, Win32Api.GetStockObject(Win32Api.StockObjects.DC_PEN));
        }

        /// <summary>
        /// 填充指定的矩形的内部。
        /// </summary>
        /// <param name="left">要绘制的左边距。</param>
        /// <param name="top">要绘制的上边距。</param>
        /// <param name="right">要绘制的右边距。</param>
        /// <param name="bottom">要绘制的下边距。</param>
        public void fillRectangle(int left, int top, int right, int bottom) {
            Win32Api.Rect rect = new Win32Api.Rect(left, top, right, bottom);
            Win32Api.FillRect(_hdc, ref rect, Win32Api.GetStockObject(Win32Api.StockObjects.DC_BRUSH));
        }

        /// <summary>
        /// 绘制指定的圆角矩形。
        /// </summary>
        /// <param name="left">要绘制的左边距。</param>
        /// <param name="top">要绘制的上边距。</param>
        /// <param name="right">要绘制的右边距。</param>
        /// <param name="bottom">要绘制的下边距。</param>
        /// <param name="roundWidth">圆角的宽度。</param>
        /// <param name="roundHeight">圆角的高度。</param>
        public void drawRoundRectangle(int left, int top, int right, int bottom, int roundWidth = 5, int roundHeight = 5) {
            IntPtr obj = Win32Api.SelectObject(_hdc, Win32Api.GetStockObject(Win32Api.StockObjects.NULL_BRUSH));
            fillRoundRectangle(left, top, right, bottom, roundWidth, roundHeight);
            Win32Api.SelectObject(_hdc, obj);
        }

        /// <summary>
        /// 填充指定的圆角矩形的内部。
        /// </summary>
        /// <param name="left">要绘制的左边距。</param>
        /// <param name="top">要绘制的上边距。</param>
        /// <param name="right">要绘制的右边距。</param>
        /// <param name="bottom">要绘制的下边距。</param>
        /// <param name="roundWidth">圆角的宽度。</param>
        /// <param name="roundHeight">圆角的高度。</param>
        public void fillRoundRectangle(int left, int top, int right, int bottom, int roundWidth = 5, int roundHeight = 5) {
            Win32Api.RoundRect(_hdc, left, top, right, bottom, roundWidth, roundHeight);
        }

        /// <summary>
        /// 绘制指定的点数组所定义的多边形。
        /// </summary>
        /// <param name="points">要绘制的多边形的顶点。</param>
        public void drawPolygon(params Point[] points) {
            IntPtr obj = Win32Api.SelectObject(_hdc, Win32Api.GetStockObject(Win32Api.StockObjects.NULL_BRUSH));
            fillPolygon(points);
            Win32Api.SelectObject(_hdc, obj);
        }

        /// <summary>
        /// 填充指定的点数组所定义的多边形的内部。
        /// </summary>
        /// <param name="points">要填充的多边形的顶点。</param>
        public unsafe void fillPolygon(params Point[] points) {
            fixed (Point* p = points)
            {
                Win32Api.Polygon(_hdc, p, points.Length);
            }
        }

        /// <summary>
        /// 绘制指定的点数组所定义的椭圆。
        /// </summary>
        /// <param name="left">要绘制的左边距。</param>
        /// <param name="top">要绘制的上边距。</param>
        /// <param name="right">要绘制的右边距。</param>
        /// <param name="bottom">要绘制的下边距。</param>
        public void drawEllipse(int left, int top, int right, int bottom) {
            var obj = Win32Api.SelectObject(_hdc, Win32Api.GetStockObject(Win32Api.StockObjects.NULL_BRUSH));
            fillEllipse(left, top, right, bottom);
            Win32Api.SelectObject(_hdc, obj);
        }

        /// <summary>
        /// 填充指定的点数组所定义的椭圆的内部。
        /// </summary>
        /// <param name="left">要绘制的左边距。</param>
        /// <param name="top">要绘制的上边距。</param>
        /// <param name="right">要绘制的右边距。</param>
        /// <param name="bottom">要绘制的下边距。</param>
        public void fillEllipse(int left, int top, int right, int bottom) {
            Win32Api.Ellipse(_hdc, left, top, right, bottom);
        }

        #endregion

        #region 绘制点线

        /// <summary>
        /// 绘制一条直线。
        /// </summary>
        /// <param name="pt1">要连接的第一个点。</param>
        /// <param name="pt2">要连接的第二个点。</param>
        public void drawLine(Point pt1, Point pt2) {
            drawLine(pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        /// 绘制一条直线。
        /// </summary>
        /// <param name="x1">要连接的第一个点的 x 值。</param>
        /// <param name="y1">要连接的第一个点的 y 值。</param>
        /// <param name="x2">要连接的第二个点的 x 值。</param>
        /// <param name="y2">要连接的第二个点的 y 值。</param>
        public void drawLine(int x1, int y1, int x2, int y2) {
            Win32Api.MoveToEx(_hdc, x1, y1, IntPtr.Zero);
            Win32Api.LineTo(_hdc, x2, y2);
        }

        /// <summary>
        /// 绘制一条线指定样式的线。
        /// </summary>
        /// <param name="x1">要连接的第一个点的 x 值。</param>
        /// <param name="y1">要连接的第一个点的 y 值。</param>
        /// <param name="x2">要连接的第二个点的 x 值。</param>
        /// <param name="y2">要连接的第一个点的 y 值。</param>
        /// <param name="dashStyle">指示线条样式。</param>
        public void drawLine(int x1, int y1, int x2, int y2, DashStyle dashStyle) {
            var intPtr = Win32Api.CreatePen(dashStyle, 1, foreColor);
            IntPtr obj = Win32Api.SelectObject(_hdc, intPtr);
            drawLine(x1, y1, x2, y2);
            Win32Api.SelectObject(_hdc, obj);
            Win32Api.DeleteObject(intPtr);
        }

        /// <summary>
        /// 绘制一条线虚线。
        /// </summary>
        /// <param name="x1">要连接的第一个点的 x 值。</param>
        /// <param name="y1">要连接的第一个点的 y 值。</param>
        /// <param name="x2">要连接的第二个点的 x 值。</param>
        /// <param name="y2">要连接的第一个点的 y 值。</param>
        public void drawDashLine(int x1, int y1, int x2, int y2) {
            drawLine(x1, y1, x2, y2, DashStyle.Dash);
        }

        /// <summary>
        /// 绘制一条横向或众向虚线。
        /// </summary>
        /// <param name="x1">要连接的第一个点的 x 值。</param>
        /// <param name="y1">要连接的第一个点的 y 值。</param>
        /// <param name="x2">要连接的第二个点的 x 值。</param>
        /// <param name="y2">要连接的第一个点的 y 值。</param>
        public void drawDotLine(int x1, int y1, int x2, int y2) {
            var color = foreColor;

            for (var y = y1; y < y2; y += 2) {
                setPixel(x1, y, color);
            }

            for (var x = x1; x < x2; x += 2) {
                setPixel(x, y1, color);
            }

        }

        /// <summary>
        /// 绘制一条水平波浪线。
        /// </summary>
        /// <param name="x1">要连接的第一个点的 x 值。</param>
        /// <param name="x2">要连接的第二个点的 x 值。</param>
        /// <param name="y">要连接的两个点的 y 值。</param>
        public void drawWave(int x1, int x2, int y) {
            int pt1x = x1 - x1 % 6;
            int t = x2 % 6;
            int pt2x = (t != 0) ? (x2 + (6 - t)) : x2;
            int count = pt2x - pt1x >> 1;
            if (count < 4) {
                count = 4;
            } else {
                t = (count - 4) / 3;
                if ((count - 4) % 3 != 0) {
                    t++;
                }
                count = 4 + t * 3;
            }
            var points = new Point[count];
            for (int i = 0; i < count; i++) {
                points[i].X = pt1x + i << 1;
                points[i].Y = y - 1;
                switch (i % 3) {
                    case 0:
                        {
                            points[i].Y -= 2;
                            break;
                        }
                    case 2:
                        {
                            points[i].Y += 2;
                            break;
                        }
                }
            }

            unsafe
            {
                fixed (Point* p = points)
                {
                    Win32Api.PolyBezier(_hdc, p, count);
                }
            }

        }

        /// <summary>
        /// 设置一个点的像素值。
        /// </summary>
        /// <param name="x">要设置的点的 x 值。</param>
        /// <param name="y">要设置的点的 y 值。</param>
        /// <param name="color">要设置的点。</param>
        public void setPixel(int x, int y, uint color) {
            Win32Api.SetPixel(_hdc, x, y, color);
        }

        /// <summary>
        /// 获取一个点的像素值。
        /// </summary>
        /// <param name="x">要获取的点的 x 值。</param>
        /// <param name="y">要获取的点的 y 值。</param>
        public uint getPixel(int x, int y) {
            return Win32Api.GetPixel(_hdc, x, y);
        }

        #endregion

        #region 绘制图片

        /// <summary>
        /// 在指定的位置使用原始物理大小绘制指定的图片。
        /// </summary>
        public void drawImage(Image image, int x, int y) {
            //Win32API.BitBlt(_hdc, 0, 0, image.Width, image.Height, image.)
            Graphics e = Graphics.FromHdc(_hdc);
            e.DrawImageUnscaled(image, x, y, image.Width, image.Height);
        }

        public void drawImage(System.Windows.Forms.ImageList images, int index, Win32Api.Rect rect) {
            Win32Api.ImageList_DrawEx(images.Handle, index, _hdc, rect.left, rect.top, rect.width, rect.height, unchecked((int)uint.MaxValue), unchecked((int)uint.MaxValue), 1);
        }

        #endregion

        #region 绘图区域裁剪和缩放

        /// <summary>
        /// 指定在绘图时忽略重绘指定区域。
        /// </summary>
        /// <param name="left">要裁剪的左边距。</param>
        /// <param name="top">要裁剪的上边距。</param>
        /// <param name="right">要裁剪的右边距。</param>
        /// <param name="bottom">要裁剪的下边距。</param>
        public void excludeClipRect(int left, int top, int right, int bottom) {
            Win32Api.ExcludeClipRect(_hdc, left, top, right, bottom);
        }

        #endregion

        #region 绘制边界

        /// <summary>
        /// 绘制一个边界。
        /// </summary>
        /// <param name="left">要绘制的左边距。</param>
        /// <param name="top">要绘制的上边距。</param>
        /// <param name="right">要绘制的右边距。</param>
        /// <param name="bottom">要绘制的下边距。</param>
        /// <param name="style">要绘制的边界样式。</param>
        /// <param name="sides">要绘制的边。</param>
        public void drawEdge(int left, int top, int right, int bottom, System.Windows.Forms.Border3DStyle style, System.Windows.Forms.Border3DSide sides) {
            Win32Api.Rect rect = new Win32Api.Rect(left, top, right, bottom);
            Win32Api.DrawEdge(_hdc, ref rect, style, sides);
        }

        /// <summary>
        /// 绘制一个边界。
        /// </summary>
        /// <param name="left">要绘制的边距。绘制结束后将更新为最新值。</param>
        /// <param name="style">要绘制的边界样式。</param>
        /// <param name="sides">要绘制的边。</param>
        public void drawEdge(ref Win32Api.Rect rect, System.Windows.Forms.Border3DStyle style, System.Windows.Forms.Border3DSide sides) {
            Win32Api.DrawEdge(_hdc, ref rect, style, sides);
        }

        #endregion

    }

}
