using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {

        [Category("SyntaxEdit"), Description("Occurs when user splits Edit control horizontally.")]
        public event EventHandler SplitHorz;

        [Category("SyntaxEdit"), Description("Occurs when user splits Edit control vertically.")]
        public event EventHandler SplitVert;

        [Category("SyntaxEdit"), Description("Occurs when horizontal split view is removed.")]
        public event EventHandler UnsplitHorz;

        [Category("SyntaxEdit"), Description("Occurs when vertical split view is removed.")]
        public event EventHandler UnsplitVert;

        protected virtual CodeEditor CreateSplitEdit(bool vert) {
            CodeEditor syntaxEdit = new CodeEditor();
            syntaxEdit.Visible = false;
            syntaxEdit.Parent = base.Parent;
            syntaxEdit.BringToFront();
            syntaxEdit.Source = this.Source;
            syntaxEdit.Location = base.Location;
            syntaxEdit.Dock = (vert ? DockStyle.Left : DockStyle.Top);
            if (vert) {
                syntaxEdit.Width = 0;
            } else {
                syntaxEdit.Height = 0;
            }
            this.components.Add((Component)syntaxEdit);
            if (vert) {
                this.vertSplitEdit = syntaxEdit;
            } else {
                this.horzSplitEdit = syntaxEdit;
            }
            return syntaxEdit;
        }
        protected void SplitterMoved(object sender, SplitterEventArgs e) {
            if (sender == this.horzSplitter && (this.horzSplitEdit == null || e.SplitY - this.horzSplitEdit.Top <= EditConsts.DefaltScrollSplitterSize + this.horzSplitter.Height)) {
                this.UnsplitView(false);
            } else {
                if (sender == this.vertSplitter && (this.vertSplitEdit == null || e.SplitX - this.vertSplitEdit.Top <= EditConsts.DefaltScrollSplitterSize + this.vertSplitter.Width)) {
                    this.UnsplitView(true);
                }
            }
        }
        protected virtual Splitter CreateSplitter(bool vert) {
            Splitter splitter = new Splitter();
            splitter.Visible = false;
            splitter.Parent = base.Parent;
            splitter.BringToFront();
            splitter.Dock = (vert ? DockStyle.Left : DockStyle.Top);
            splitter.Location = base.Location;
            splitter.MinSize = 0;
            splitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            splitter.SplitterMoved += new SplitterEventHandler(this.SplitterMoved);
            this.components.Add(splitter);
            if (vert) {
                this.vertSplitter = splitter;
            } else {
                this.horzSplitter = splitter;
            }
            return splitter;
        }
        protected CodeEditor GetSplitEdit(bool vert) {
            return vert ? this.vertSplitEdit : this.horzSplitEdit;
        }
        protected Splitter GetSplitter(bool vert) {
            return vert ? this.vertSplitter : this.horzSplitter;
        }
        protected bool CanSplit(bool vert) {
            Splitter splitter = this.GetSplitter(vert);
            return base.IsHandleCreated && this.Dock == DockStyle.Fill && (splitter == null || !splitter.Visible);
        }
        protected bool CanUnsplit(bool vert) {
            Splitter splitter = this.GetSplitter(vert);
            return splitter != null && splitter.Visible;
        }
        protected void SplitView(bool vert) {
            if (this.CanSplit(vert)) {
                Splitter splitter = this.GetSplitter(vert);
                if (splitter == null) {
                    splitter = this.CreateSplitter(vert);
                }
                CodeEditor syntaxEdit = this.GetSplitEdit(vert);
                if (syntaxEdit == null) {
                    syntaxEdit = this.CreateSplitEdit(vert);
                }
                //  syntaxEdit.Assign(this);
                if (vert) {
                    syntaxEdit.Width = 0;
                } else {
                    syntaxEdit.Height = 0;
                }
                syntaxEdit.Scrolling.WindowOriginY = this.Scrolling.WindowOriginY;
                syntaxEdit.Scrolling.WindowOriginX = this.Scrolling.WindowOriginX;
                splitter.Visible = true;
                syntaxEdit.Visible = true;
                base.BringToFront();
                if (vert) {
                    if (this.SplitVert != null) {
                        this.SplitVert(this, EventArgs.Empty);
                    }
                } else {
                    if (this.SplitHorz != null) {
                        this.SplitHorz(this, EventArgs.Empty);
                    }
                }
            }
        }
        protected void UnsplitView(bool vert) {
            if (this.CanUnsplit(vert)) {
                Splitter splitter = this.GetSplitter(vert);
                if (splitter != null) {
                    splitter.Visible = false;
                }
                CodeEditor splitEdit = this.GetSplitEdit(vert);
                if (splitEdit != null) {
                    splitEdit.Visible = false;
                }
                if (vert) {
                    if (this.UnsplitVert != null) {
                        this.UnsplitVert(this, EventArgs.Empty);
                    }
                } else {
                    if (this.UnsplitHorz != null) {
                        this.UnsplitHorz(this, EventArgs.Empty);
                    }
                }
            }
        }
        public virtual bool CanSplitHorz() {
            return this.CanSplit(false);
        }
        public virtual bool CanSplitVert() {
            return this.CanSplit(true);
        }
        public virtual bool CanUnsplitHorz() {
            return this.CanUnsplit(false);
        }
        public virtual bool CanUnsplitVert() {
            return this.CanUnsplit(true);
        }
        public virtual void SplitViewHorz() {
            this.SplitView(false);
        }
        public virtual void SplitViewVert() {
            this.SplitView(true);
        }
        public virtual void UnsplitViewHorz() {
            this.UnsplitView(false);
        }
        public virtual void UnsplitViewVert() {
            this.UnsplitView(true);
        }
    }
}