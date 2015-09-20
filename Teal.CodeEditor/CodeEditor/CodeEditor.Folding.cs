using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor : Control {

        #region 属性

        /// <summary>
        /// 折叠区域的宽度。
        /// </summary>
        private int _foldMarkerWidth = Configs.defaultFoldMarkerWidth;

        /// <summary>
        /// 折叠区域前景色。
        /// </summary>
        private uint _foldMarkerForeColor = Painter.toRGB(Color.Gray);

        /// <summary>
        /// 折叠区域边框颜色。
        /// </summary>
        private uint _foldMarkerBorderColor = Painter.toRGB(Color.Gray);

        /// <summary>
        /// 折叠区域边框选中颜色。
        /// </summary>
        private uint _foldMarkerBorderSelectedColor = Painter.toRGB(Color.Gray);

        /// <summary>
        /// 获取或设置折叠标记的宽度。
        /// </summary>
        /// <value>返回非正数表示不显示折叠区域。</value>
        [Description("获取或设置折叠标记的宽度。")]
        [DefaultValue(Configs.defaultFoldMarkerWidth)]
        public int foldMarkerWidth {
            get {
                return _foldMarkerWidth;
            }
            set {
                if (foldMarkerWidth != value) {
                    _foldMarkerWidth = value;
                    updateLayout();
                }
            }
        }

        /// <summary>
        /// 判断或设置是否显示折叠标记。
        /// </summary>
        [Description("判断或设置是否显示折叠标记。")]
        [DefaultValue(Configs.defaultFoldMarkerWidth > 0)]
        public bool showFoldMarkers {
            get {
                return _foldMarkerWidth > 0;
            }
            set {
                if (showFoldMarkers != value) {
                    _foldMarkerWidth = ~_foldMarkerWidth;
                    updateLayout();
                }
            }
        }

        #endregion

        #region 绘制

        /// <summary>
        /// 绘制指定区域的折叠标记。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="drawBackground"></param>
        private void drawFoldMarkers(int line, int top, int bottom, bool drawBackground) {
            if (!showFoldMarkers) {
                return;
            }

            var right = gutterWidth;
            var left = right - foldMarkerWidth;

            if (drawBackground) {
                painter.backColor = _gutterBackColor;
                painter.fillRectangle(left, top, right, bottom);
            }

            // 计算内部小方块大小。
            left += Configs.foldMarkerMargin;
            right -= Configs.foldMarkerMargin;
            var height = right - left;
            var marginTop = (painter.lineHeight - height) >> 1;

            for (int y; (y = _layoutLines[line].top - scrollTop) < bottom; line++) {

                y += marginTop;
                drawFoldMarker(left, y, right, y + height, false);

                // todo

            }

        }

        /// <summary>
        /// 绘制单个折叠标记。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="isFolded"></param>
        private void drawFoldMarker(int left, int top, int right, int bottom, bool isFolded) {
            
            // 绘制边框。
            painter.foreColor = _foldMarkerBorderColor;
            painter.drawRectangle(left, top, right, bottom);

            // 绘制减号。
            var middle = (bottom + top) >> 1;
            painter.foreColor = _foldMarkerForeColor;
            painter.drawLine(left + Configs.foldMarkerPadding, middle, right - Configs.foldMarkerPadding, middle);

            // 绘制加号。
            if (isFolded) {
                var center = (left + right) >> 1;
                painter.drawLine(center, top + Configs.foldMarkerPadding, center, bottom - Configs.foldMarkerPadding);
            }

        }

        private void drawFoldEndMarker() {
            
        }

        #endregion

        #region 折叠区域存储

        /// <summary>
        /// 所有折叠区域的有序列表。
        /// </summary>
        private List<FoldingRange> _foldingRanges;

        public void addFoldingRange(FoldingRange range) {
            
        }

        public void removeFoldingRange(FoldingRange range) {

        }

        #endregion

        /// <summary>
        /// 判断指定字符是否已被折叠。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool isFolded(int line, int column) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 折叠指定区域。
        /// </summary>
        /// <param name="range">折叠区域。</param>
        /// <returns>如果确实折叠行，则返回 true。</returns>
        public bool fold(FoldingRange range) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 折叠指定区域。
        /// </summary>
        /// <param name="range">折叠区域。</param>
        /// <returns>如果确实折叠行，则返回 true。</returns>
        public bool unfold(FoldingRange range) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 切换指定行的折叠状态。
        /// </summary>
        /// <param name="line">要切换的行。</param>
        public void toggleFold(int line) {
            
        }

        /// <summary>
        /// 获取从指定行开始的折叠区域。
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public FoldingRange getFoldingRange(int line) {
            return null;
        }

        /// <summary>
        /// 获取包含指定字符最近的折叠区域。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public FoldingRange getFoldingRange(int line, int column) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定区域的所有折叠区域。
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public IEnumerable<FoldingRange> getFoldingRanges(int startLine, int startColumn, int endLine, int endColumn) {
            throw new NotImplementedException();
        }

        public FoldingRange addFoldingRange(int startLine, int startColumn, int endLine, int endColumn, string text, bool isFolded = false) {
            throw new NotImplementedException();
        }

        public int getFoldingLeval(FoldingRange range) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 折叠到指定层级。
        /// </summary>
        /// <param name="leval"></param>
        /// <returns></returns>
        public int foldTo(int leval) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 展开到指定层级。
        /// </summary>
        /// <param name="leval"></param>
        /// <returns></returns>
        public int unfoldTo(int leval) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 折叠到定义。
        /// </summary>
        /// <returns></returns>
        public int foldToDefinition() {

            throw new NotImplementedException();
        }

        public FoldingRange activedFoldingRange;

        /// <summary>
        /// 判断或设置当前编辑器是否支持折叠。
        /// </summary>
        public bool enableFolding {
            get {

                throw new NotImplementedException();
            }
            set {
                
            }
        }

        /// <summary>
        /// 折叠全部。
        /// </summary>
        public void foldAll() {

        }

        /// <summary>
        /// 展开全部。
        /// </summary>
        public void unfoldAll() {

        }

        /// <summary>
        /// 折叠指定区域全部折叠区域。
        /// </summary>
        public void foldAll(int startLine, int startColumn, int endLine, int endColumn) {

        }

        /// <summary>
        /// 展开指定区域全部折叠区域。
        /// </summary>
        public void unfoldAll(int startLine, int startColumn, int endLine, int endColumn) {

        }

        /// <summary>
        /// 切换指定区域的折叠状态。
        /// </summary>
        /// <param name="range">折叠区域。</param>
        /// <returns>如果确实折叠行，则返回 true。</returns>
        public bool toggleFold(FoldingRange range) {
            return range.isFolded ? unfold(range) : fold(range);
        }

        /// <summary>
        /// 折叠指定行。
        /// </summary>
        /// <param name="line"></param>
        /// <returns>如果确实折叠行，则返回 true。</returns>
        public bool fold(int line) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// 展开指定行。
        /// </summary>
        /// <param name="line"></param>
        /// <returns>如果确实折叠行，则返回 true。</returns>
        public void unfold(int line) {

        }

        #region 事件


        private void onLineNumberClick() {

        }


        private void onGutterContextMenu() {

        }


        #endregion
    }

    /// <summary>
    /// 表示一个折叠区域。
    /// </summary>
    public sealed class FoldingRange : Range {

        /// <summary>
        /// 判断当前折叠区域是否已折叠。
        /// </summary>
        public bool isFolded;

        /// <summary>
        /// 获取当前区域折叠后显示的文本。
        /// </summary>
        public string text;

        /// <summary>
        /// 获取当前区域折叠后工具提示文本。
        /// </summary>
        public string tooltipText;

    }

}
