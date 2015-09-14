using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Teal.CodeEditor {

    /// <summary>
    /// 提供 WIN32 API 的调用。
    /// </summary>
    public static class Win32Api {

        /// <summary>
        /// 表示一个矩形的位置和大小。
        /// </summary>
        public struct Rect {

            /// <summary>
            /// 获取或设置此区域左上角的 x 坐标。
            /// </summary>
            public int left;

            /// <summary>
            /// 获取或设置此区域左上角的 y 坐标。
            /// </summary>
            public int top;

            /// <summary>
            /// 获取或设置此区域右下角的 x 坐标。
            /// </summary>
            public int right;

            /// <summary>
            /// 获取或设置此区域右下角的 y 坐标。
            /// </summary>
            public int bottom;

            /// <summary>
            /// 获取当前区域的宽度。
            /// </summary>
            public int width => right - left;

            /// <summary>
            /// 获取当前区域的高度。
            /// </summary>
            public int height => bottom - top;

            public Rect(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public static explicit operator Rectangle(Rect rect) {
                return new Rectangle(rect.left, rect.top, rect.width, rect.height);
            }

            public static explicit operator Rect(Rectangle rect) {
                return new Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr p1);

        public enum StockObjects {
            WHITE_BRUSH = 0,
            LTGRAY_BRUSH = 1,
            GRAY_BRUSH = 2,
            DKGRAY_BRUSH = 3,
            BLACK_BRUSH = 4,
            NULL_BRUSH = 5,
            HOLLOW_BRUSH = NULL_BRUSH,
            WHITE_PEN = 6,
            BLACK_PEN = 7,
            NULL_PEN = 8,
            OEM_FIXED_FONT = 10,
            ANSI_FIXED_FONT = 11,
            ANSI_VAR_FONT = 12,
            SYSTEM_FONT = 13,
            DEVICE_DEFAULT_FONT = 14,
            DEFAULT_PALETTE = 15,
            SYSTEM_FIXED_FONT = 16,
            DEFAULT_GUI_FONT = 17,
            DC_BRUSH = 18,
            DC_PEN = 19,
        }

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(StockObjects index);

        public enum BkMode {
            OPAQUE = 0,
            TRANSPARENT = 1,
        }

        [DllImport("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, BkMode bkMode);

        [DllImport("gdi32.dll")]
        public static extern int SetDCBrushColor(IntPtr hdc, uint color);

        [DllImport("gdi32.dll")]
        public static extern uint GetDCBrushColor(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern int SetDCPenColor(IntPtr hdc, uint color);

        [DllImport("gdi32.dll")]
        public static extern uint GetDCPenColor(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern int SetBkColor(IntPtr hdc, uint color);

        [DllImport("gdi32.dll")]
        public static extern uint GetBkColor(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern int SetTextColor(IntPtr hdc, uint color);

        [DllImport("gdi32.dll")]
        public static extern uint GetTextColor(IntPtr hdc);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public unsafe static extern bool Polygon(IntPtr hdc, Point* points, int count);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Ellipse(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(uint color);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(DashStyle style, int width, uint color);

        [DllImport("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, int bkMode);

        [DllImport("user32.dll")]
        public static extern int FrameRect(IntPtr hdc, ref Rect rect, IntPtr brush);

        [DllImport("user32.dll")]
        public static extern int FillRect(IntPtr hdc, ref Rect rect, IntPtr brush);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RoundRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveToEx(IntPtr hdc, int x, int y, IntPtr p);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LineTo(IntPtr hdc, int x, int y);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public unsafe static extern bool PolyBezier(IntPtr hdc, Point* points, int count);

        [DllImport("gdi32.dll")]
        public static extern int SetPixel(IntPtr hdc, int x, int y, uint color);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int x, int y);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static unsafe extern int ExtTextOut(IntPtr hdc, int x, int y, int options, Rect* rect, char* s, int len, int* buffer);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static unsafe extern int ExtTextOut(IntPtr hdc, int x, int y, int options, ref Rect rect, char* s, int len, int* buffer);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int DrawText(IntPtr hdc, string s, int len, ref Rect rect, int format);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public unsafe static extern bool GetTextExtentPoint32(IntPtr hdc, char* s, int count, out Size size);

        [StructLayout(LayoutKind.Sequential)]
        public struct ABC {
            public int abcA;
            public int abcB;
            public int abcC;
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public unsafe static extern bool GetCharABCWidths(IntPtr hdc, int iFirstChar, int iLastChar, ABC* lpabc);

        public static unsafe void getCharABCWidths(IntPtr hdc, int startChar, int endChar, int[] charWidths, int charWidthIndex) {
            ABC[] abc = new ABC[endChar - startChar];
            fixed (ABC* p = abc)
            {
                if (GetCharABCWidths(hdc, startChar, endChar - 1, p)) {
                    for (var i = 0; i < abc.Length; i++) {
                        charWidths[i + charWidthIndex] = abc[i].abcA + abc[i].abcB + abc[i].abcC;
                    }
                }
            }
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCharABCWidths(IntPtr hdc, int iFirstChar, int iLastChar, ABC[] lpabc);

        public static bool GetCharABCWidths(IntPtr hdc, int iFirstChar, int iLastChar, ref int[] buffer) {

            var size = new ABC[buffer.Length];

            unsafe
            {
                fixed (ABC* p = size)
                {
                    GetCharABCWidths(hdc, iFirstChar, iLastChar, p);
                }
            }

            for (var i = 0; i < size.Length; i++) {
                buffer[i] = size[i].abcA + size[i].abcB + size[i].abcC;
            }

            return true;

            //bool flag = true;
            //IntPtr intPtr = Marshal.AllocHGlobal(buffer.Length * 12);
            //// try {
            //flag = Win32API.GetCharABCWidths(hdc, iFirstChar, iLastChar, intPtr);
            //if (flag) {
            //    for (int i = 0; i < buffer.Length; i++) {
            //        buffer[i] = Marshal.ReadInt32(intPtr, i * 12) + Marshal.ReadInt32(intPtr, i * 12 + 4) + Marshal.ReadInt32(intPtr, i * 12 + 8);
            //    }
            //}
            //// } finally {
            //Marshal.FreeHGlobal(intPtr);
            ////  }

            //Console.WriteLine((DateTime.Now - d).TotalMilliseconds);
            //return flag;
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCharABCWidths(IntPtr hdc, int iFirstChar, int iLastChar, IntPtr lpabc);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int index);

        [DllImport("gdi32.dll")]
        public static extern int GetGraphicsMode(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern int SetGraphicsMode(IntPtr hdc, int mode);

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(int index);

        [DllImport("comctl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ImageList_DrawEx(IntPtr handle, int index, IntPtr hdc, int x, int y, int dx, int dy, int bk, int fr, int style);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        public static extern int ExcludeClipRect(IntPtr hdc, int l, int t, int r, int b);

        [DllImport("gdi32.dll")]
        public static extern int IntersectClipRect(IntPtr hdc, int l, int t, int r, int b);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWorldTransform(IntPtr hdc, ref XFORM p2);

        [StructLayout(LayoutKind.Explicit)]
        public struct XFORM {
            [FieldOffset(0)]
            public float eM11;
            [FieldOffset(4)]
            public float eM12;
            [FieldOffset(8)]
            public float eM21;
            [FieldOffset(12)]
            public float eM22;
            [FieldOffset(16)]
            public float eDx;
            [FieldOffset(20)]
            public float eDy;
            public XFORM(float m11, float m12, float m21, float m22, float dx, float dy) {
                eM11 = m11;
                eM12 = m12;
                eM21 = m21;
                eM22 = m22;
                eDx = dx;
                eDy = dy;
            }
        }

        [DllImport("gdi32.dll")]
        public static extern int GetTextMetrics(IntPtr hdc, out TextMetrics metrics);

        /// <summary>
        /// 表示字体信息。
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct TextMetrics {

            /// <summary>
            /// 字符高度。
            /// </summary>
            [FieldOffset(0)]
            public int Height;

            /// <summary>
            /// 字符上部高度(基线以上)。
            /// </summary>
            [FieldOffset(4)]
            public int Accent;

            /// <summary>
            /// 字符下部高度(基线以下)。
            /// </summary>
            [FieldOffset(8)]
            public int Descent;

            /// <summary>
            /// 由tmHeight定义的字符高度的顶部空间数目。
            /// </summary>
            [FieldOffset(12)]
            public int PublicLeading;

            /// <summary>
            /// 加在两行之间的空间数目。
            /// </summary>
            [FieldOffset(16)]
            public int ExternalLeading;

            /// <summary>
            /// 平均字符宽度。
            /// </summary>
            [FieldOffset(20)]
            public int AveCharWidth;

            /// <summary>
            /// 最宽字符的宽度。
            /// </summary>
            [FieldOffset(24)]
            public int MaxCharWidth;

            /// <summary>
            /// 字体的粗细轻重程度。
            /// </summary>
            [FieldOffset(28)]
            public int Weight;

            /// <summary>
            /// 加入某些拼接字体上的附加高度。
            /// </summary>
            [FieldOffset(32)]
            public int Overhang;

            /// <summary>
            /// 字体设计所针对的设备水平方向。
            /// </summary>
            [FieldOffset(36)]
            public int DigitizedAspectX;

            /// <summary>
            /// 字体设计所针对的设备垂直方向。
            /// </summary>
            [FieldOffset(40)]
            public int DigitizedAspectY;

            /// <summary>
            /// 为字体定义的第一个字符。
            /// </summary>
            [FieldOffset(44)]
            public byte FirstChar;

            /// <summary>
            /// 为字体定义的最后一个字符。
            /// </summary>
            [FieldOffset(45)]
            public byte LastChar;

            /// <summary>
            /// 字体中所没有字符的替代字符。
            /// </summary>
            [FieldOffset(46)]
            public byte DefaultChar;

            /// <summary>
            /// 用于拆字的字符。
            /// </summary>
            [FieldOffset(57)]
            public byte BreakChar;

            /// <summary>
            /// 字体为斜体时非零。
            /// </summary>
            [FieldOffset(58)]
            public byte Italic;

            /// <summary>
            /// 字体为下划线时非零。
            /// </summary>
            [FieldOffset(49)]
            public byte Underlined;

            /// <summary>
            /// 字体被删去时非零。
            /// </summary>
            [FieldOffset(50)]
            public byte StruckOut;

            /// <summary>
            /// 字体间距(低4位)和族(高4位)。
            /// </summary>
            [FieldOffset(51)]
            public byte PitchAndFamily;

            /// <summary>
            /// 字体的字符集。
            /// </summary>
            [FieldOffset(52)]
            public byte CharSet;

        }

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement, int nOrientaion, int fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, uint fdwCharSet, uint fdwOutputPrecision, uint fdwClipPrecision, uint fdwQuality, uint fdwPitchAndFamily, string lpszFace);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DrawEdge(IntPtr hdc, ref Rect rect, System.Windows.Forms.Border3DStyle style, System.Windows.Forms.Border3DSide side);

    }

}
