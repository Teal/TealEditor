using System;
using System.IO;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 管理程序内置资源文件。
    /// </summary>
    public static class Resources {

        /// <summary>
        /// 获取指定资源的流。
        /// </summary>
        /// <param name="resourceName">要获取的资源名。如“Properties.LeftArrow.cur”</param>
        /// <returns></returns>
        public static Stream getStream(string resourceName) {
            return typeof (Resources).Assembly.GetManifestResourceStream(typeof (Resources), resourceName);
        }

        /// <summary>
        /// 从指定的流载入光标。
        /// </summary>
        /// <param name="cursorStream">要载入的流。</param>
        /// <returns></returns>
        public static Cursor loadCursor(Stream cursorStream) {
            string tempFileName = Path.GetTempFileName();
            Cursor result;
            try {
                using (BinaryReader binaryReader = new BinaryReader(cursorStream)) {
                    using (FileStream fileStream = new FileStream(tempFileName, FileMode.Open, FileAccess.Write, FileShare.None)) {
                        byte[] array = binaryReader.ReadBytes(4096);
                        int i;
                        for (i = array.Length; i >= 4096; i = binaryReader.Read(array, 0, 4096)) {
                            fileStream.Write(array, 0, 4096);
                        }
                        fileStream.Write(array, 0, i);
                    }
                }
                result = new Cursor(Win32.LoadImageCursor(IntPtr.Zero, tempFileName, 2, 0, 0, 16));
            } finally {
                File.Delete(tempFileName);
            }
            return result;
        }

        private static Cursor _leftArrow;

        /// <summary>
        /// 获取反转鼠标光标。
        /// </summary>
        public static Cursor leftArrow {
            get {
                if (_leftArrow == null) {
                    _leftArrow = loadCursor(getStream("Properties.LeftArrow.cur"));
                }
                return _leftArrow;
            }
        }

    }
}
