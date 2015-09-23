using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        #region 分隔条

        /// <summary>
        /// 表示当前的分屏按钮。
        /// </summary>
        private SplitButton _splitButton = new SplitButton();

        /// <summary>
        /// 获取或设置当前编辑器的分割按钮。
        /// </summary>
        public SplitButton splitButton {
            get {
                return _splitButton;
            }
            set {
                if (_splitButton != value) {
                    _splitButton.MouseDown -= splitButton_MouseDown;
                    Controls.Remove(_splitButton);

                    _splitButton = value;

                    value.MouseDown += splitButton_MouseDown;
                    Controls.Add(value);

                    updateLayout();
                }
            }
        }

        private void splitButton_MouseDown(object sender, MouseEventArgs e) {
            return;
            Scrolling tempQualifier = Scrolling;
            Splitter splitter = null;
            if (true) {
                tempQualifier.owner.SplitViewVert();
                splitter = tempQualifier.owner.VertSplitter;
            } else {
                if (true) {
                    tempQualifier.owner.SplitViewHorz();
                    splitter = tempQualifier.owner.HorzSplitter;
                }
            }
            if (splitter != null && splitter.IsHandleCreated) {
                if (tempQualifier.owner.IsHandleCreated) {
                    tempQualifier.owner.Update();
                    if (tempQualifier.vScrollBar != null) {
                        tempQualifier.vScrollBar.Update();
                    }
                    if (tempQualifier.hScrollBar != null) {
                        tempQualifier.hScrollBar.Update();
                    }
                }
                OSUtils.SendMessage(splitter.Handle, 513, IntPtr.Zero, IntPtr.Zero);
            }
        }

        #endregion

        #region 滚动条大小

        /// <summary>
        /// 当前编辑器的滚动条样式。
        /// </summary>
        RichTextBoxScrollBars _scrollBars = Configs.defaultScrollBars;

        /// <summary>
        /// 正在显示的水平滚动条。
        /// </summary>
        private HScrollBar _hScrollBar;

        /// <summary>
        /// 正在显示的垂直滚动条。
        /// </summary>
        private VScrollBar _vScrollBar;

        /// <summary>
        /// 获取或设置当前编辑器的滚动条样式。
        /// </summary>
        [Description("获取或设置当前编辑器的滚动条样式。")]
        [DefaultValue(Configs.defaultScrollBars)]
        public RichTextBoxScrollBars scrollBars {
            get {
                return _scrollBars;
            }
            set {
                if (_scrollBars != value) {
                    _scrollBars = value;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置正在显示的水平滚动条。
        /// </summary>
        public HScrollBar hScrollBar {
            get {
                return _hScrollBar ?? (hScrollBar = new HScrollBar());
            }
            set {
                if (_hScrollBar != value) {
                    if (_hScrollBar != null) {
                        Controls.Remove(_hScrollBar);
                        _hScrollBar.Scroll -= hScrollBar_Scroll;
                    }
                    _hScrollBar = value;
                    _hScrollBar.Visible = false;
                    _hScrollBar.Cursor = Cursors.Default;
                    _hScrollBar.Scroll += hScrollBar_Scroll;
                    Controls.Add(_hScrollBar);
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置正在显示的垂直滚动条。
        /// </summary>
        public VScrollBar vScrollBar {
            get {
                return _vScrollBar ?? (vScrollBar = new VScrollBar());
            }
            set {
                if (_vScrollBar != value) {
                    if (_vScrollBar != null) {
                        Controls.Remove(_vScrollBar);
                        _vScrollBar.Scroll -= vScrollBar_Scroll;
                    }
                    _vScrollBar = value;
                    _vScrollBar.Visible = false;
                    _vScrollBar.Cursor = Cursors.Default;
                    _vScrollBar.Scroll += vScrollBar_Scroll;
                    Controls.Add(_vScrollBar);
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 判断当前编辑器是否显示水平滚动条。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool hasHScrollBar {
            get {
                return _hScrollBar != null && _hScrollBar.Visible;
            }
        }

        /// <summary>
        /// 判断当前编辑器是否显示水平滚动条。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool hasVScrollBar {
            get {
                return _vScrollBar != null && _vScrollBar.Visible;
            }
        }

        /// <summary>
        /// 获取当前编辑器的滚动区域宽度。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int scrollWidth {
            get {

                // 如果之前有缓存。
                if (_maxWidthLine >= 0) {
                    return visualLines[_maxWidthLine].right;
                }

                // 分析每行搜索最长区域。

                int value = -1;
                for (var i = 0; i < visualLines.Count; i++) {
                    if (visualLines[i].right > value) {
                        value = visualLines[i].right;
                        _maxWidthLine = i;
                    }
                }

                return value;
            }
        }

        /// <summary>
        /// 获取当前编辑器的滚动区域高度。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int scrollHeight {
            get {
                return visualLines.Count * painter.lineHeight;
            }
        }

        /// <summary>
        /// 更新滚动条状态。
        /// </summary>
        private void updateScrollBarSizes() {
            Utility.mark();

            // 确定是否显示滚动条。

            var clientSize = base.ClientSize;

            // 先获取目前滚动条的状态。
            var vScrollBarSize = _vScrollBar != null ? _vScrollBar.Width : Configs.defaultVScrollBarSize;
            var hScrollBarSize = _hScrollBar != null ? _hScrollBar.Height : Configs.defaultHScrollBarSize;

            // 获取减去滚动条之后实际可用文档大小。
            var documentWidth = clientSize.Width - documentLeft - vScrollBarSize - (showAutoBreakMark ? Configs.autoBreakMarkWidth : 0);
            var documentHeight = clientSize.Height - hScrollBarSize;

            // 获取目前需要滚动的文档大小。
            var scrollHeight = this.scrollHeight;
            var scrollWidth = this.scrollWidth;

            switch (scrollBars) {
                case RichTextBoxScrollBars.Horizontal:
                    if (scrollWidth <= documentWidth) {
                        hScrollBarSize = 0;
                    }
                    vScrollBarSize = 0;
                    break;
                case RichTextBoxScrollBars.Vertical:
                    hScrollBarSize = 0;
                    if (scrollHeight <= documentHeight) {
                        vScrollBarSize = 0;
                    }
                    break;
                case RichTextBoxScrollBars.Both:
                    if (scrollWidth <= documentWidth) {
                        hScrollBarSize = 0;
                    }
                    if (scrollHeight <= documentHeight) {
                        vScrollBarSize = 0;
                    }
                    break;
                case RichTextBoxScrollBars.ForcedHorizontal:
                    vScrollBarSize = 0;
                    break;
                case RichTextBoxScrollBars.ForcedVertical:
                    hScrollBarSize = 0;
                    break;
                case RichTextBoxScrollBars.ForcedBoth:
                    break;
                default:
                    goto case RichTextBoxScrollBars.Both;
            }

            // 更新分割按钮。
            _splitButton.Visible = _splitButton.Enabled && (vScrollBarSize + hScrollBarSize > 0);
            _splitButton.SetBounds(clientSize.Width - vScrollBarSize, clientSize.Height - hScrollBarSize, vScrollBarSize, hScrollBarSize);

            // 对滚动条重新布局。

            if (vScrollBarSize > 0) {
                vScrollBar.SetBounds(_splitButton.Left, 0, _splitButton.Width, clientSize.Height - (_splitButton.Visible ? _splitButton.Height : 0));
                _vScrollBar.Maximum = scrollHeight;
                _vScrollBar.LargeChange = documentWidth;
                _vScrollBar.Enabled = scrollHeight > documentWidth;
                _vScrollBar.Visible = true;
            } else if (_vScrollBar != null) {
                _vScrollBar.Visible = false;
            }

            if (hScrollBarSize > 0) {
                hScrollBar.SetBounds(0, _splitButton.Top, clientSize.Width - (_splitButton.Visible ? _splitButton.Width : 0), _splitButton.Height);
                _hScrollBar.Maximum = scrollWidth;
                _hScrollBar.LargeChange = documentHeight;
                _hScrollBar.Enabled = scrollWidth > documentHeight;
                _hScrollBar.Visible = true;
            } else if (_hScrollBar != null) {
                _hScrollBar.Visible = false;
            }

        }

        /// <summary>
        /// 取消初始化滚动条。
        /// </summary>
        private void uninitScrollBars() {
            if (_splitButton != null) {
                _splitButton.Dispose();
                _splitButton = null;
            }
            if (_vScrollBar != null) {
                _vScrollBar.Dispose();
                _vScrollBar = null;
            }
            if (_hScrollBar != null) {
                _hScrollBar.Dispose();
                _hScrollBar = null;
            }
        }

        #endregion

        #region 滚动条位置

        /// <summary>
        /// 当前展示的滚动 y 位置。
        /// </summary>
        private int _scrollTop;

        /// <summary>
        /// 当前展示的滚动 x 位置。
        /// </summary>
        private int _scrollLeft;

        /// <summary>
        /// 存储目前最宽的行。如果值等于 -1 则需重新计算。
        /// </summary>
        private int _maxWidthLine = 0;

        /// <summary>
        /// 获取或设置当前编辑器水平滚动的位置。
        /// </summary>
        public int scrollLeft {
            get {
                return _scrollLeft;
            }
            set {
                if (_scrollLeft != value) {
                    _scrollLeft = value;
                    if (_hScrollBar != null) {
                        _hScrollBar.Value = value;
                    }
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置当前编辑器垂直滚动的位置。
        /// </summary>
        public int scrollTop {
            get {
                return _scrollTop;
            }
            set {
                if (_scrollTop != value) {
                    _scrollTop = value;
                    if (_vScrollBar != null) {
                        _vScrollBar.Value = value;
                    }
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 获取或设置当前编辑器滚动区域。
        /// </summary>
        public Point scrollPosition {
            get {
                return new Point(scrollLeft, scrollTop);
            }
            set {
                setScroll(value.X, value.Y);
            }
        }

        /// <summary>
        /// 获取当前控件的滚动大小。
        /// </summary>
        public Size scrollSize {
            get {
                return new Size(scrollWidth, scrollHeight);
            }
        }

        /// <summary>
        /// 获取或设置当前滚动的行号。
        /// </summary>
        public int scrollLine {
            get {
                return scrollTop / painter.lineHeight;
            }
            set {
                scrollTop = value * painter.lineHeight;
            }
        }

        /// <summary>
        /// 获取或设置当前滚动的列号。
        /// </summary>
        public int scrollColumn {
            get {
                return scrollLeft / painter.fontWidth;
            }
            set {
                scrollLeft = value * painter.fontWidth;
            }
        }

        /// <summary>
        /// 更新滚动条的位置。
        /// </summary>
        private void updateScrollPositions() {



        }

        /// <summary>
        /// 滚动到指定位置。
        /// </summary>
        /// <param name="top"></param>
        /// <param name="left"></param>
        public void setScroll(int top, int left) {
            beginUpdate();
            scrollTop = top;
            scrollLeft = left;
            endUpdate();
        }

        /// <summary>
        /// 平滑滚动到指定位置。
        /// </summary>
        /// <param name="top"></param>
        /// <param name="left"></param>
        public void scrollTo(int top, int left) {

        }

        /// <summary>
        /// 平滑滚动到指定位置。
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public void scrollBy(int deltaX, int deltaY) {
            scrollTo(scrollLeft + deltaX, scrollTop + deltaY);
        }

        #endregion

        #region 事件回调

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e) {
            handlerHScrollEvent(e.Type, e.NewValue);
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e) {
            handlerVScrollEvent(e.Type, e.NewValue);
        }

        /// <summary>
        /// 处理鼠标滚轮事件。
        /// </summary>
        /// <param name="delta">滚轮的系数。</param>
        public void handleMouseScroll(int delta) {
            return;
            var scrollSize = SystemInformation.MouseWheelScrollLines;

            if (delta == 120) {
                delta = scrollSize;
            } else if (delta == -120) {
                delta = -scrollSize;
            } else {
                delta = delta / Math.Min(Math.Abs(delta), 120) * scrollSize;
            }

            if (delta != 0) {
                if (this.scrolling.ScrollByPixels) {
                    this.scrolling.WindowOriginY -= delta * this.Painter.lineHeight;
                } else {
                    this.scrolling.WindowOriginY -= delta;
                }
            }
        }

        /// <summary>
        /// 处理消息循环的滚轮事件。
        /// </summary>
        /// <param name="m"></param>
        private void handleWmHorzScroll(ref Message m) {
            m.Result = (IntPtr)1;
            //handlerHScrollEvent(OSUtils.LoWord(m.WParam))
            //switch ((ScrollEventType)OSUtils.LoWord(m.WParam)) {
            //    case ScrollEventType.SmallDecrement:
            //        scrollLeft--;
            //        break;
            //    case ScrollEventType.SmallIncrement:
            //        scrollLeft++;
            //        break;
            //    case ScrollEventType.LargeDecrement:
            //        scrollLeft -= _clientRight - _clientLeft;
            //        break;
            //    //case ScrollEventType.LargeIncrement:
            //    //    this.WindowOriginX += (this.ScrollByPixels ? this.owner.ClientRect.Width : this.owner.CharsInWidth);
            //    //    break;
            //    //case ScrollEventType.ThumbPosition:
            //    //    this.WindowOriginX = pos;
            //    //    break;
            //    //case ScrollEventType.ThumbTrack:
            //    //    if ((this.options & ScrollingOptions.SmoothScroll) != ScrollingOptions.None) {
            //    //        this.WindowOriginX = pos;
            //    //    }
            //    //    break;
            //    //case ScrollEventType.First:
            //    //    this.WindowOriginX = 0;
            //    //    break;
            //    //case ScrollEventType.Last:
            //    //    this.WindowOriginX = this.ScrollWidth();
            //    //    break;
            //}
        }

        /// <summary>
        /// 处理消息循环的滚轮事件。
        /// </summary>
        /// <param name="m"></param>
        private void handleWmVertScroll(ref Message m) {
            m.Result = (IntPtr)1;
            switch ((ScrollEventType)OSUtils.LoWord(m.WParam)) {
                case ScrollEventType.SmallDecrement:
                    scrollLine--;
                    break;
                case ScrollEventType.SmallIncrement:
                    scrollLine++;
                    break;
                case ScrollEventType.LargeDecrement:
                    scrollLine -= visibleLineCount;
                    break;
                case ScrollEventType.LargeIncrement:
                    scrollLine += visibleLineCount;
                    break;
                case ScrollEventType.ThumbPosition:
                    //  OSUtils.GetScrollPos(this.owner.Handle, this.flatBars, true)
                    //scrollLine = pos;
                    break;
                case ScrollEventType.ThumbTrack:
                    //if ((this.options & ScrollingOptions.ShowScrollHint) != ScrollingOptions.None) {
                    //    if ((this.options & ScrollingOptions.SmoothScroll) != ScrollingOptions.None) {
                    //        pos = this.WindowOriginY;
                    //    } else {
                    //        pos = this.VScrollBar.Value;
                    //    }
                    //    this.owner.ShowScrollHint(pos);
                    //}
                    break;
                case ScrollEventType.First:
                    scrollLine = 0;
                    break;
                case ScrollEventType.Last:
                    scrollLine = visualLines.Count;
                    break;
                case ScrollEventType.EndScroll:
                    //if (this.WordWrap) {
                    //    this.UpdateScroll();
                    //}
                    break;
            }
        }

        private void handlerHScrollEvent(ScrollEventType type, int newValue) {
            Scrolling tempQualifier = Scrolling;
            int pos = newValue;
            if (tempQualifier.scrollUpdateCount <= 0 && tempQualifier.owner != null) {
                switch (type) {
                    case ScrollEventType.SmallDecrement:
                        tempQualifier.WindowOriginX -= (tempQualifier.ScrollByPixels ? tempQualifier.owner.Painter.FontWidth : 1);
                        break;
                    case ScrollEventType.SmallIncrement:
                        tempQualifier.WindowOriginX += (tempQualifier.ScrollByPixels ? tempQualifier.owner.Painter.FontWidth : 1);
                        break;
                    case ScrollEventType.LargeDecrement:
                        tempQualifier.WindowOriginX -= (tempQualifier.ScrollByPixels ? tempQualifier.owner.ClientRect.Width : tempQualifier.owner.CharsInWidth);
                        break;
                    case ScrollEventType.LargeIncrement:
                        tempQualifier.WindowOriginX += (tempQualifier.ScrollByPixels ? tempQualifier.owner.ClientRect.Width : tempQualifier.owner.CharsInWidth);
                        break;
                    case ScrollEventType.ThumbPosition:
                        tempQualifier.WindowOriginX = pos;
                        break;
                    case ScrollEventType.ThumbTrack:
                        break;
                    case ScrollEventType.First:
                        tempQualifier.WindowOriginX = 0;
                        break;
                    case ScrollEventType.Last:
                        tempQualifier.WindowOriginX = tempQualifier.owner.scrollWidth;
                        break;
                }
            }
            //switch (type) {
            //    case ScrollEventType.SmallDecrement:
            //        scrollLeft--;
            //        break;
            //    case ScrollEventType.SmallIncrement:
            //        scrollLeft++;
            //        break;
            //    case ScrollEventType.LargeDecrement:
            //        scrollLeft -= _clientRight - _clientLeft;
            //        break;
            //    //case ScrollEventType.LargeIncrement:
            //    //    this.WindowOriginX += (this.ScrollByPixels ? this.owner.ClientRect.Width : this.owner.CharsInWidth);
            //    //    break;
            //    //case ScrollEventType.ThumbPosition:
            //    //    this.WindowOriginX = pos;
            //    //    break;
            //    //case ScrollEventType.ThumbTrack:
            //    //    if ((this.options & ScrollingOptions.SmoothScroll) != ScrollingOptions.None) {
            //    //        this.WindowOriginX = pos;
            //    //    }
            //    //    break;
            //    //case ScrollEventType.First:
            //    //    this.WindowOriginX = 0;
            //    //    break;
            //    //case ScrollEventType.Last:
            //    //    this.WindowOriginX = this.ScrollWidth();
            //    //    break;
            //}
        }

        private void handlerVScrollEvent(ScrollEventType type, int newValue) {
            Scrolling tempQualifier = Scrolling;
            int pos = newValue;
            if (tempQualifier.scrollUpdateCount <= 0 && tempQualifier.owner != null) {
                switch (type) {
                    case ScrollEventType.SmallDecrement:
                        tempQualifier.WindowOriginY -= (tempQualifier.ScrollByPixels ? tempQualifier.owner.Painter.lineHeight : 1);
                        break;
                    case ScrollEventType.SmallIncrement:
                        tempQualifier.WindowOriginY += (tempQualifier.ScrollByPixels ? tempQualifier.owner.Painter.lineHeight : 1);
                        break;
                    case ScrollEventType.LargeDecrement:
                        tempQualifier.WindowOriginY -= (tempQualifier.ScrollByPixels ? tempQualifier.owner.ClientRect.Height : tempQualifier.owner.LinesInHeight);
                        break;
                    case ScrollEventType.LargeIncrement:
                        tempQualifier.WindowOriginY += (tempQualifier.ScrollByPixels ? tempQualifier.owner.ClientRect.Height : tempQualifier.owner.LinesInHeight);
                        break;
                    case ScrollEventType.ThumbPosition:
                        tempQualifier.WindowOriginY = pos;
                        break;
                    case ScrollEventType.ThumbTrack:

                        if ((tempQualifier.options & ScrollingOptions.ShowScrollHint) != ScrollingOptions.None) {
                            {
                                pos = tempQualifier.owner.vScrollBar.Value;
                            }
                            tempQualifier.owner.ShowScrollHint(pos);
                        }
                        break;
                    case ScrollEventType.First:
                        tempQualifier.WindowOriginY = 0;
                        break;
                    case ScrollEventType.Last:
                        tempQualifier.WindowOriginY = tempQualifier.owner.scrollHeight / painter.lineHeight;
                        break;
                    case ScrollEventType.EndScroll:
                        if (tempQualifier.owner.WordWrap) {
                            tempQualifier.UpdateScroll();
                        }
                        if ((tempQualifier.options & ScrollingOptions.ShowScrollHint) != ScrollingOptions.None) {
                            tempQualifier.owner.HideScrollHint();
                        }
                        break;
                }
            }
        }

        #endregion

    }

}
