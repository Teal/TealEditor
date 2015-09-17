using System;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示编辑器的默认配置。
    /// </summary>
    public class CodeEditorConfigs {

        #region 渲染

        //#region 边界

        ///// <summary>
        ///// 默认边界区域宽度。
        ///// </summary>
        //public const int defaultGutterWidth = 64;
        //public uint defaultGutterForeColor = 0x8F8F8F;
        //public uint defaultGutterBackColor = 0xE9E9E9;

        ///// <summary>
        ///// 默认书签区域的宽度。
        ///// </summary>
        //public int defaultBookmarksWidth = 20;
        //public int defaultLineNumbersWidth = 30;
        //public int defaultLineNumbersStart = 1;

        ///// <summary>
        ///// 折叠标记的宽度。
        ///// </summary>
        //public int defaultFoldMarkerWidth = 4;

        ///// <summary>
        ///// 折叠标记两边的外间距。
        ///// </summary>
        //public int foldMarkerMargin = 2;

        ///// <summary>
        ///// 折叠标记两边的内间距。
        ///// </summary>
        //public int foldMarkerPadding = 2;

        //#endregion

        //#region 文档

        ///// <summary>
        ///// 指定编辑器区域的默认背景。
        ///// </summary>
        //public uint backgroundColor = 0xF0F0F0;

        ///// <summary>
        ///// 指定文字的默认颜色。
        ///// </summary>
        //public uint textColor = 0x454545;

        //public int initialVisualLineCount = 100;
        //public int defaultTabWidth = 4;

        ///// <summary>
        ///// 自动换行后新行是否自动继承上行的缩进。
        ///// </summary>
        //public bool defaultInheritIndents = true;

        ///// <summary>
        ///// 当自动换行后新行的缩进空格数。
        ///// </summary>
        //public int defaultWrapIndentCount = 2;

        //public int documentPaddingLeft = 2;
        //public int documentPaddingRight = 2;
        //public int autoBreakMarkWidth = 10;

        public const string defaultFontName = "Yahei Consolas Hybrid";
        public const float defaultFontSize = 30;

        //public int defaultVScrollBarSize = 21;
        //public int defaultHScrollBarSize = 21;


        //public RichTextBoxScrollBars defaultScrollBars = RichTextBoxScrollBars.ForcedBoth;

        ///// <summary>
        ///// 默认是否自动换行。
        ///// </summary>
        //public bool defaultWordWrap = false;

        //public int defaultWidth = 800;
        //public int defaultHeight = 600;

        //#endregion

        //#endregion

        //#region 编辑

        //public uint defaultSelectionBackColor = Painter.toRGB(Color.FromArgb(173, 214, 255));

        //public uint defaultInactivedSelectionBackColor = Painter.toRGB(SystemColors.InactiveCaption);

        ///// <summary>
        ///// 光标宽度。
        ///// </summary>
        //public int caretWidth = 2;

        public const DocumentLineFlags defaultNewLineType = DocumentLineFlags.newLineTypeWin;

        #endregion

    }

}
