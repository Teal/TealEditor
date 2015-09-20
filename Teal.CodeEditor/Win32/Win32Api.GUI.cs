using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Teal.CodeEditor {

    /// <summary>
    /// 提供 WIN32 API 的调用。
    /// </summary>
    public static partial class Win32Api {

        #region 光标

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateCaret(IntPtr handle, IntPtr hBitmap, int nWidth, int nHeight);

        public static bool CreateCaret(IntPtr handle, int nWidth, int nHeight) {
            return CreateCaret(handle, IntPtr.Zero, nWidth, nHeight);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyCaret();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCaretPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCaretPos(out Point p);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowCaret(IntPtr handle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool HideCaret(IntPtr handle);

        #endregion

        #region 鼠标

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr handle);

        #endregion

    }
}
