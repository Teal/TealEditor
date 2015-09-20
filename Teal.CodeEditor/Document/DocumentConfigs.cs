namespace Teal.CodeEditor {

    /// <summary>
    /// ��ʾ�༭����Ĭ�����á�
    /// </summary>
    public class DocumentConfigs {

        #region ��Ⱦ

        public const int defaultTabWidth = 40;

        /// <summary>
        /// ��ȡ������ TAB �ַ���ȡ�����Ǹ�����ʾǿ�ƿ��ֵ���������Զ����롣
        /// </summary>
        public int tabWidth = defaultTabWidth;

        //#region �߽�

        ///// <summary>
        ///// Ĭ�ϱ߽������ȡ�
        ///// </summary>
        //public const int defaultGutterWidth = 64;
        //public uint defaultGutterForeColor = 0x8F8F8F;
        //public uint defaultGutterBackColor = 0xE9E9E9;

        ///// <summary>
        ///// Ĭ����ǩ����Ŀ�ȡ�
        ///// </summary>
        //public int defaultBookmarksWidth = 20;
        //public int defaultLineNumbersWidth = 30;
        //public int defaultLineNumbersStart = 1;

        ///// <summary>
        ///// �۵���ǵĿ�ȡ�
        ///// </summary>
        //public int defaultFoldMarkerWidth = 4;

        ///// <summary>
        ///// �۵�������ߵ����ࡣ
        ///// </summary>
        //public int foldMarkerMargin = 2;

        ///// <summary>
        ///// �۵�������ߵ��ڼ�ࡣ
        ///// </summary>
        //public int foldMarkerPadding = 2;

        //#endregion

        //#region �ĵ�

        ///// <summary>
        ///// ָ���༭�������Ĭ�ϱ�����
        ///// </summary>
        //public uint backgroundColor = 0xF0F0F0;

        ///// <summary>
        ///// ָ�����ֵ�Ĭ����ɫ��
        ///// </summary>
        //public uint textColor = 0x454545;

        //public int initialVisualLineCount = 100;


        ///// <summary>
        ///// �Զ����к������Ƿ��Զ��̳����е�������
        ///// </summary>
        //public bool defaultInheritIndents = true;

        ///// <summary>
        ///// ���Զ����к����е������ո�����
        ///// </summary>
        //public int defaultWrapIndentCount = 2;

        /// <summary>
        /// ���Զ����к����е������ո�����
        /// </summary>
        public int wrapIndentCount;

        //public int documentPaddingLeft = 2;
        //public int documentPaddingRight = 2;
        //public int autoBreakMarkWidth = 10;

        public const string defaultFontName = "Yahei Consolas Hybrid";
        public const float defaultFontSize = 30;

        //public int defaultVScrollBarSize = 21;
        //public int defaultHScrollBarSize = 21;


        //public RichTextBoxScrollBars defaultScrollBars = RichTextBoxScrollBars.ForcedBoth;

        ///// <summary>
        ///// Ĭ���Ƿ��Զ����С�
        ///// </summary>
        //public bool defaultWordWrap = false;

        /// <summary>
        /// �༭���ؼ�Ĭ�Ͽ�ȡ�
        /// </summary>
        public const int defaultWidth = 800;

        /// <summary>
        /// �༭���ؼ�Ĭ�ϸ߶ȡ�
        /// </summary>
        public const int defaultHeight = 600;

        //#endregion

        //#endregion

        //#region �༭

        //public uint defaultSelectionBackColor = Painter.toRGB(Color.FromArgb(173, 214, 255));

        //public uint defaultInactivedSelectionBackColor = Painter.toRGB(SystemColors.InactiveCaption);

        /// <summary>
        /// ����ȡ�
        /// </summary>
        public int caretWidth = 2;

        public const DocumentLineFlags defaultNewLineType = DocumentLineFlags.newLineTypeWin;

        /// <summary>
        /// ��ȡ���е�Ĭ��������С��
        /// </summary>
        public const int defaultLineCapacity = 16;

        #endregion

    }
}