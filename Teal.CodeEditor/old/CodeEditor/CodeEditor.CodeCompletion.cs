using Teal.CodeEditor.CodeCompletion;
using Teal.CodeEditor.TextSource;
using Teal.Syntax;
using Teal.Syntax.CodeCompletion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Teal.CodeEditor {

    /// <summary>
    /// 表示一个代码编辑器。
    /// </summary>
    public partial class CodeEditor {


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual char[] AutoCorrectDelimiters {
            get {
                return this.autoCorrectDelimiters;
            }
            set {
                if (this.autoCorrectDelimiters != value) {
                    this.autoCorrectDelimiters = value;
                    this.OnAutoCorrectDelimitersChanged();
                }
            }
        }
        [Category("SyntaxEdit"), DefaultValue(false), Description("Gets or sets a boolean value indicating whether to auto correct words being typed.")]
        public virtual bool AutoCorrection {
            get {
                return this.autoCorrection;
            }
            set {
                if (this.autoCorrection != value) {
                    this.autoCorrection = value;
                    this.OnAutoCorrectionChanged();
                }
            }
        }
        private ICodeCompletionProvider GetQuickInfo(string text, bool useHtmlFormatting) {
            return new QuickInfo {
                Text = text,
                UseHtmlFormatting = useHtmlFormatting
            };
        }
        private void DisableCodeCompletionTimer() {
            if (this.codeCompletionTimer != null) {
                this.codeCompletionTimer.Enabled = false;
            }
        }
        private Point PointToBlockPoint(int x, Point pt, string str) {
            Point point = new Point(0, 0);
            string[] array = StringItem.Split(str.Substring(0, Math.Min(x, str.Length)));
            if (array.Length > 0) {
                point.X = array[array.Length - 1].Length;
                point.Y = array.Length - 1;
            }
            Point result;
            if (point.Y == 0) {
                result = new Point(point.X + pt.X, pt.Y);
            } else {
                result = new Point(point.X, point.Y + pt.Y);
            }
            return result;
        }
        protected void CodeCompletionAction() {
            if (this.Lexer is SyntaxParser) {
                this.DoCodeCompletion((SyntaxParser)this.Lexer, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.NeedReparse, this.codeCompletionArgs);
            }
        }
        protected void CodeCompletionStart() {
            SyntaxParser syntaxParser = this.Lexer as SyntaxParser;
            if (syntaxParser != null) {
                syntaxParser.Prepare(this.Source.FileName, this.Lines, syntaxParser.SyntaxTree, this.codeCompletionArgs.NeedReparse);
            }
        }
        protected void CodeCompletionEnd() {
            this.OnNeedCompletion(this.codeCompletionArgs);
        }
        protected virtual void OnCodeCompletionCharsChanged() {
        }
        protected void StopCodeCompletionThread() {
            if (this.Source != null) {
                this.Source.StopThread(this.codeCompletionAction, false);
            }
            this.DisableCodeCompletionTimer();
        }
        protected bool CodeCompletion(char ch, byte style, bool isValidText) {
            this.DisableCodeCompletionTimer();
            bool flag = false;
            int defaultCompletionStartDelay = EditConsts.DefaultCompletionStartDelay;
            if (this.Source.NeedCodeCompletion() && (this.codeCompletionBox == null || !this.codeCompletionBox.Visible)) {
                SyntaxParser syntaxParser = this.Lexer as SyntaxParser;
                if (syntaxParser.IsCodeCompletionChar(ch, style, ref defaultCompletionStartDelay)) {
                    this.codeCompletionArgs.Init(CodeCompletionType.None, this.Position);
                    this.codeCompletionArgs.KeyChar = ch;
                    this.codeCompletionArgs.Interval = defaultCompletionStartDelay;
                    this.DoCodeCompletion();
                    flag = true;
                }
            }
            if (isValidText && Array.IndexOf<char>(this.codeCompletionChars, ch) >= 0) {
                if (!flag) {
                    this.codeCompletionArgs.Init(CodeCompletionType.None, this.Position);
                    this.codeCompletionArgs.KeyChar = ch;
                    this.codeCompletionArgs.Interval = defaultCompletionStartDelay;
                    this.DoCodeCompletion();
                }
                flag = true;
            }
            return flag;
        }
        protected bool OnNeedCompletion(CodeCompletionArgs e) {
            this.DisableCodeCompletionTimer();
            if (this.NeedCodeCompletion != null) {
                this.NeedCodeCompletion(this, e);
            }
            if ((e.CompletionType == CodeCompletionType.CompleteWord || e.CompletionType == CodeCompletionType.CompleteComment) && e.NeedShow && e.SelIndex >= 0 && e.Provider != null && e.SelIndex < e.Provider.Count) {
                e.Handled = true;
                this.InsertTextFromProvider(e.Provider, e.Provider.GetText(e.SelIndex), e.StartPosition, e.EndPosition, e.Provider.UseIndent, e.UseFormat);
            }
            if (!e.Handled && e.NeedShow && e.Provider != null) {
                this.codeCompletionArgs.Provider = e.Provider;
                this.DoCodeCompletion();
            }
            return e.Handled;
        }
        protected void DoCodeToolTip(string s, int x, int y, Point startPt, bool useHtmlFormatting) {
            this.codeCompletionArgs.CompletionType = CodeCompletionType.QuickInfo;
            this.codeCompletionArgs.Interval = EditConsts.DefaultHintDelay;
            this.codeCompletionArgs.NeedShow = true;
            this.codeCompletionArgs.ToolTip = true;
            this.codeCompletionArgs.StartPosition = startPt;
            this.codeCompletionArgs.Provider = this.GetQuickInfo(s, useHtmlFormatting);
            this.codeCompletionArgs.DisplayPosition = this.ScreenToText(x, y);
            this.DoCodeCompletion();
        }
        protected void DoCodeCompletion(SyntaxParser parser, Point position, bool useThread, CodeCompletionArgs e) {
            StringItem item = this.Lines.GetItem(position.Y);
            if (item != null) {
                parser.CodeCompletion(item.String, item.TextData, position, useThread, e);
            }
        }
        protected void DoCodeCompletion() {
            this.DisableCodeCompletionTimer();
            if (this.codeCompletionArgs.Interval == 0) {
                this.OnCodeCompletion(this, EventArgs.Empty);
            } else {
                this.CodeCompletionTimer.Interval = this.codeCompletionArgs.Interval;
                this.CodeCompletionTimer.Enabled = true;
            }
        }
        protected void OnCodeCompletion(object source, EventArgs e) {
            this.DisableCodeCompletionTimer();
            if (this.codeCompletionArgs.Provider != null) {
                if (this.codeCompletionArgs.CompletionType == CodeCompletionType.QuickInfo) {
                    if (this.snippetRange != null) {
                        Point pt = base.PointToScreen(this.TextToScreen(this.snippetRange.StartPoint, false));
                        pt.Y += this.painter222.FontHeight + EditConsts.DefaultHintOffsetY;
                        pt.X = Cursor.Position.X + EditConsts.DefaultHintOffsetX;
                        this.ShowCodeCompletionHint(this.codeCompletionArgs.Provider, pt, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.EndPosition, this.codeCompletionArgs.DisplayPosition, true, false, null);
                        return;
                    }
                    if (this.mouseRange != null) {
                        Point pt = base.PointToScreen(this.TextToScreen(this.mouseRange.StartPoint, false));
                        pt.Y += this.painter222.FontHeight + EditConsts.DefaultHintOffsetY;
                        pt.X = Cursor.Position.X + EditConsts.DefaultHintOffsetX;
                        this.ShowCodeCompletionHint(this.codeCompletionArgs.Provider, pt, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.EndPosition, this.codeCompletionArgs.DisplayPosition, true, false, this.Lexer);
                        return;
                    }
                    if (this.mouseUrl != string.Empty && this.mouseUrl != null) {
                        Point pt = base.PointToScreen(this.DisplayToScreen(this.mouseUrlPoint.X, this.mouseUrlPoint.Y));
                        pt.Y += this.painter222.FontHeight + EditConsts.DefaultHintOffsetY;
                        pt.X = Cursor.Position.X + EditConsts.DefaultHintOffsetX;
                        this.ShowCodeCompletionHint(this.codeCompletionArgs.Provider, pt, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.EndPosition, this.codeCompletionArgs.DisplayPosition, true, false, null);
                        return;
                    }
                    if (this.mouseError != null) {
                        Point pt = base.PointToScreen(this.TextToScreen(this.mouseError.Position, false));
                        pt.Y += this.painter222.FontHeight + EditConsts.DefaultHintOffsetY;
                        pt.X = Cursor.Position.X + EditConsts.DefaultHintOffsetX;
                        this.ShowCodeCompletionHint(this.codeCompletionArgs.Provider, pt, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.EndPosition, this.codeCompletionArgs.DisplayPosition, true, false, null);
                        return;
                    }
                    if (this.mouseBookMark != null) {
                        Point pt = base.PointToScreen(this.mouseBookMarkPt);
                        pt.Y += this.painter222.FontHeight + EditConsts.DefaultHintOffsetY;
                        pt.X = Cursor.Position.X + EditConsts.DefaultHintOffsetX;
                        this.ShowCodeCompletionHint(this.codeCompletionArgs.Provider, pt, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.EndPosition, this.codeCompletionArgs.DisplayPosition, true, false, null);
                        return;
                    }
                    if (this.dragMargin) {
                        if (!this.margin.IsDragging) {
                            Point pt = base.PointToClient(Control.MousePosition);
                            pt = base.PointToScreen(this.DisplayToScreen(this.margin.Position, this.ScreenToDisplay(pt.X, pt.Y).Y));
                            pt.Y = Control.MousePosition.Y + EditConsts.DefaultHintOffsetY;
                            pt.X += EditConsts.DefaultHintOffsetX;
                            this.ShowCodeCompletionHint(this.codeCompletionArgs.Provider, pt, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.EndPosition, this.codeCompletionArgs.DisplayPosition, true, false, null);
                        }
                        return;
                    }
                }
                if (this.codeCompletionArgs.ToolTip) {
                    this.ShowCodeCompletionHint(this.codeCompletionArgs.Provider, this.codeCompletionArgs.Lexer);
                } else {
                    this.ShowCodeCompletionBox(this.codeCompletionArgs.Provider, this.codeCompletionArgs.CompletionType);
                }
            } else {
                if (this.codeCompletionArgs.KeyChar != '\0') {
                    this.ExecuteCodeCompletion(this.codeCompletionArgs);
                }
            }
        }
        protected void InsertCodeSnippet(CodeSnippet snippet, Point startPos, Point endPos, bool useIndent) {
            this.selection.BeginUpdate();
            this.Source.BeginUpdate(UpdateReason.InsertBlock);
            try {
                if (!this.selection.IsEmpty) {
                    this.selection.SelectionType = SelectionType.Stream;
                }
                string text = this.selection.IsEmpty ? string.Empty : this.selection.SelectedText;
                if (!this.selection.IsEmpty) {
                    int num = startPos.Equals(this.selection.SelectionRect.Location) ? -1 : this.Source.StorePosition(startPos);
                    int num2 = endPos.Equals(this.selection.SelectionRect.Location) ? -1 : this.Source.StorePosition(endPos);
                    try {
                        this.selection.Delete();
                    } finally {
                        if (num2 >= 0) {
                            endPos = this.Source.RestorePosition(num2);
                        }
                        if (num >= 0) {
                            startPos = this.Source.RestorePosition(num);
                        }
                    }
                }
                IList<ICodeSnippetLiteral> list = new List<ICodeSnippetLiteral>();
                CodeSnippetRanges codeSnippetRanges = new CodeSnippetRanges();
                foreach (ICodeSnippetDeclaration current in snippet.Declarations) {
                    foreach (ICodeSnippetLiteral codeSnippetLiteral in current.Literals) {
                        list.Add(codeSnippetLiteral);
                    }
                    foreach (ICodeSnippetObject current2 in current.Objects) {
                        list.Add(current2);
                    }
                }
                string text2 = snippet.Code.Code;
                string delimiter = snippet.Code.Delimiter;
                Regex regex = new Regex(string.Concat(new string[]
				{
					"\\",
					delimiter,
					EditConsts.DefaultSnippetPattern,
					"\\",
					delimiter
				}), RegexOptions.Multiline);
                bool flag = false;
                int num3 = 0;
                foreach (Match match in regex.Matches(text2)) {
                    if (match.Success) {
                        ICodeSnippetLiteral codeSnippetLiteral = null;
                        bool flag2 = match.Value == delimiter + EditConsts.DefaultSnippetSelectedPattern + delimiter;
                        bool flag3 = match.Value == delimiter + EditConsts.DefaultSnippetEndPattern + delimiter;
                        flag |= flag3;
                        if (!flag2) {
                            foreach (ICodeSnippetLiteral current3 in list) {
                                if (match.Value == delimiter + current3.ID + delimiter) {
                                    codeSnippetLiteral = current3;
                                    break;
                                }
                            }
                        }
                        string text3 = flag3 ? SyntaxConsts.DefaultCaretSymbol.ToString() : (flag2 ? text : ((codeSnippetLiteral != null) ? codeSnippetLiteral.Default : string.Empty));
                        text2 = text2.Remove(match.Index - num3, match.Length).Insert(match.Index - num3, text3);
                        if (codeSnippetLiteral != null && codeSnippetLiteral.Editable) {
                            codeSnippetRanges.Add(new CodeSnippetRange(new Point(match.Index - num3, 0), new Point(match.Index - num3 + codeSnippetLiteral.Default.Length, 0), codeSnippetLiteral.ID, codeSnippetLiteral.ToolTip, codeSnippetLiteral.ID != string.Empty));
                        } else {
                            if (flag3) {
                                codeSnippetRanges.Add(new CodeSnippetRange(new Point(match.Index - num3, 0), new Point(match.Index - num3, 0), "$" + EditConsts.DefaultSnippetEndPattern + "$", string.Empty, false));
                            }
                        }
                        num3 += match.Length - text3.Length;
                    }
                }
                if (codeSnippetRanges.Count != 0) {
                    codeSnippetRanges.Add(new CodeSnippetRange(new Point(text2.Length, 0), new Point(text2.Length, 0), string.Empty, string.Empty, false));
                }
                startPos.X = Math.Min(startPos.X, this.Lines.GetLength(startPos.Y));
                Point position;
                this.InsertTextFromProvider(null, text2, startPos, endPos, useIndent, out position, text == string.Empty, false);
                Teal.CodeEditor.TextSource.TextSource textSource = this.Source;
                foreach (Range current4 in codeSnippetRanges) {
                    current4.StartPoint = this.PointToBlockPoint(current4.StartPoint.X, startPos, text2);
                    current4.EndPoint = this.PointToBlockPoint(current4.EndPoint.X, startPos, text2);
                }
                textSource.CodeSnippets = codeSnippetRanges;
                textSource.BeginUpdateSnippet();
                try {
                    this.selection.SetSelection(SelectionType.Stream, startPos, this.Position);
                    this.selection.SmartFormat();
                    this.selection.Clear();
                } finally {
                    textSource.EndUpdateSnippet();
                }
                if (!this.SelectFirstSnippet() && flag && position.X >= 0 && position.Y >= 0) {
                    this.MoveTo(position);
                }
            } finally {
                this.Source.EndUpdate();
                this.selection.EndUpdate();
            }
        }
        protected void InsertTextFromProvider(ICodeCompletionProvider provider, string text, Point startPos, Point endPos, bool useIndent, bool format) {
            this.selection.BeginUpdate();
            this.Source.BeginUpdate(UpdateReason.Insert);
            try {
                Point position;
                this.InsertTextFromProvider(provider, text, startPos, endPos, useIndent, out position, true, format);
                if (position.X >= 0 && position.Y >= 0) {
                    this.MoveTo(position);
                }
            } finally {
                this.Source.EndUpdate();
                this.selection.EndUpdate();
            }
        }
        protected void InsertTextFromProvider(ICodeCompletionProvider provider, string text, Point startPos, Point endPos, bool useIndent, out Point curPos, bool needSelect, bool format) {
            Teal.CodeEditor.TextSource.TextSource textSource = this.Source;
            textSource.BeginUpdate(UpdateReason.Insert);
            try {
                if (provider != null && provider.UseHtmlFormatting) {
                    text = text.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "@").Replace("&quot;", "\"").Replace("&nbsp;", " ");
                }
                this.MoveTo(startPos);
                string text2 = this.Lines[this.Position.Y];
                if (needSelect) {
                    if (startPos.X >= 0 && startPos.Y >= 0 && endPos.X >= 0 && endPos.Y >= 0) {
                        if (endPos.X == int.MaxValue) {
                            endPos.X = this.Lines.GetLength(endPos.Y);
                        }
                        this.selection.SetSelection(SelectionType.Stream, startPos, endPos);
                    } else {
                        if (this.Position.X >= 0 && this.Position.X < text2.Length && !this.Lines.IsDelimiter(text2, this.Position.X)) {
                            this.selection.SelectWord();
                            if (!this.selection.IsEmpty) {
                                this.selection.SetSelection(this.Selection.SelectionType, startPos, new Point(this.Selection.SelectionRect.Right, this.Selection.SelectionRect.Bottom));
                            }
                        }
                    }
                }
                string[] array = StringItem.Split(text);
                curPos = new Point(-1, -1);
                int num = array[0].IndexOf(SyntaxConsts.DefaultCaretSymbol);
                if (num >= 0) {
                    curPos.X = startPos.X + num;
                    curPos.Y = startPos.Y;
                    array[0] = array[0].Remove(num, 1);
                }
                string str = (useIndent && array.Length > 1) ? this.Lines.GetIndentString(this.Lines.TabPosToPos(this.Lines[this.Position.Y], startPos.X), 0) : string.Empty;
                for (int i = 1; i < array.Length; i++) {
                    array[i] = str + array[i];
                    num = array[i].IndexOf(SyntaxConsts.DefaultCaretSymbol);
                    if (num >= 0) {
                        array[i] = array[i].Remove(num, 1);
                        curPos.X = num;
                        curPos.Y = this.Position.Y + i;
                    }
                }
                if (!this.selection.IsEmpty) {
                    this.selection.Delete();
                }
                Point position = textSource.Position;
                textSource.InsertBlock(array);
                if (format) {
                    int index = textSource.StorePosition(curPos);
                    try {
                        this.selection.SetSelection(SelectionType.Stream, position, this.Position);
                        this.selection.SmartFormat();
                    } finally {
                        curPos = textSource.RestorePosition(index);
                    }
                }
                this.selection.Clear();
            } finally {
                textSource.EndUpdate();
            }
        }
        protected bool SelectFirstSnippet() {
            Teal.CodeEditor.TextSource.TextSource textSource = this.Source;
            int firstSnippet = textSource.CodeSnippets.GetFirstSnippet();
            if (firstSnippet >= 0) {
                Range range = textSource.CodeSnippets[firstSnippet];
                this.MoveTo(range.StartPoint);
                this.Selection.SetSelection(SelectionType.Stream, range.StartPoint, range.EndPoint);
            }
            return firstSnippet >= 0;
        }
        public virtual bool GetWordAt(StringItem item, int pos, out int left, out int right) {
            left = 0;
            right = 0;
            bool result;
            if (item == null) {
                result = false;
            } else {
                if (this.Lexer == null) {
                    result = this.Lines.GetWord(item.String, pos, out left, out right);
                } else {
                    int length = item.String.Length;
                    if (length > 0 && pos <= length) {
                        if (pos == length) {
                            pos--;
                        }
                        if (pos >= 0) {
                            StringItemInfo itemInfo = item.TextData[pos];
                            right = pos;
                            while (pos > 0 && item.TextData[pos - 1].Equals(itemInfo)) {
                                pos--;
                            }
                            left = pos;
                            result = true;
                            return result;
                        }
                    }
                    result = false;
                }
            }
            return result;
        }
        protected bool SelectSnippet(Point position, bool selectNext, bool insertNew) {
            Teal.CodeEditor.TextSource.TextSource textSource = this.Source;
            int num;
            bool result;
            if (textSource.CodeSnippets.FindSnippet(position, false, out num)) {
                if (!textSource.CodeSnippets.IsFirstSnippet(num)) {
                    if (!selectNext) {
                        result = true;
                    } else {
                        textSource.UnhighlightCodeSnippets();
                        result = false;
                    }
                } else {
                    int num2 = selectNext ? textSource.CodeSnippets.GetNextSnippet(num) : textSource.CodeSnippets.GetPrevSnippet(num);
                    if (num2 != num) {
                        Range range = textSource.CodeSnippets[num2];
                        this.MoveTo(range.StartPoint);
                        this.Selection.SetSelection(SelectionType.Stream, range.StartPoint, range.EndPoint);
                    }
                    result = true;
                }
            } else {
                Point position1 = this.Position;
                if (insertNew && !this.ReadOnly && !false) {
                    string text = this.Lines[this.Position.Y];
                    if (text.Trim() != string.Empty && this.Source.NeedCodeCompletion()) {
                        if (this.Position.X <= text.Length && !this.Lines.IsDelimiter(text, Math.Max(this.Position.X - 1, 0)) && (this.Position.X == text.Length || this.Lines.IsDelimiter(this.Position.Y, Math.Max(this.Position.X, 0)))) {
                            int num3;
                            int num4;
                            if (this.GetWordAt(this.Lines.GetItem(this.Position.Y), Math.Max(this.Position.X - 1, 0), out num3, out num4)) {
                                SyntaxParser syntaxParser = this.Lexer as SyntaxParser;
                                ICodeSnippetsProvider codeSnippets = syntaxParser.CodeSnippets;
                                if (codeSnippets != null) {
                                    CodeSnippet codeSnippet = codeSnippets.FindByShortcut(text.Substring(num3, num4 - num3 + 1), syntaxParser.CaseSensitive);
                                    if (codeSnippet != null) {
                                        this.InsertCodeSnippet(codeSnippet, new Point(num3, this.Position.Y), new Point(num4 + 1, this.Position.Y), codeSnippets.ShouldIndent(codeSnippet));
                                        result = true;
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }
                result = false;
            }
            return result;
        }
        protected void ClosePopupWindow() {
            if (this.codeCompletionBox != null && this.codeCompletionBox.Visible) {
                this.codeCompletionBox.Close(false);
            }
        }
        protected void CloseCodeCompletionBox(object sender, ClosingEventArgs e) {
            if (e.Accepted && e.Text != null && e.Text != string.Empty) {
                if (e.Provider is CodeSnippets) {
                    if (e.Provider.SelIndex >= 0 && e.Provider.SelIndex < e.Provider.Count) {
                        this.InsertCodeSnippet(((CodeSnippets)e.Provider)[e.Provider.SelIndex], e.StartPosition, e.EndPosition, e.UseIndent);
                    }
                } else {
                    this.InsertTextFromProvider(e.Provider, e.Text, e.StartPosition, e.EndPosition, e.UseIndent, e.UseFormat);
                }
            }
        }
        protected void DisposeCodeCompletionBox(object sender, EventArgs e) {
            if (sender == this.codeCompletionBox) {
                this.codeCompletionBox = null;
            } else {
                if (sender == this.codeCompletionHint) {
                    this.codeCompletionHint = null;
                }
            }
        }
        protected void CloseCodeCompletionHint(object sender, ClosingEventArgs e) {
            if (e.Accepted && e.Text != null && e.Text != string.Empty) {
                this.InsertTextFromProvider(e.Provider, e.Text, e.StartPosition, e.EndPosition, e.UseIndent, e.UseFormat);
            }
        }
        protected virtual void ExecuteCodeCompletion(CodeCompletionArgs e) {
            if (this.Source.NeedCodeCompletion()) {
                SyntaxParser syntaxParser = this.Lexer as SyntaxParser;
                if (syntaxParser.SupportsThread) {
                    this.codeCompletionArgs = e;
                    this.Source.StartThread(this.codeCompletionAction, this.codeCompletionStart, this.codeCompletionEnd, null);
                } else {
                    syntaxParser.Prepare(this.Source.FileName, this.Lines, null, false);
                    this.DoCodeCompletion(syntaxParser, e.StartPosition, false, e);
                    this.OnNeedCompletion(e);
                }
            } else {
                this.OnNeedCompletion(e);
            }
        }
        protected virtual Point GetInfoTipPos(Point textPt) {
            string text = this.Lines[textPt.Y];
            if (textPt.X > 0 && textPt.X < text.Length && this.Lines.IsDelimiter(text, textPt.X)) {
                textPt.X--;
            }
            int num;
            int num2;
            Point result;
            if (this.Lines.GetWord(text, textPt.X, out num, out num2) && text.Substring(num, num2 - num + 1).Trim() != string.Empty) {
                if (!this.Lines.IsDelimiter(text, num)) {
                    result = new Point(num, textPt.Y);
                    return result;
                }
            }
            result = new Point(-1, -1);
            return result;
        }
        protected virtual void ListMembers(CodeCompletionArgs e, CodeCompletionType completionType) {
            e.Init(completionType, this.Position);
            this.ExecuteCodeCompletion(e);
        }
        protected void ListMembers(CodeCompletionArgs e) {
            this.ListMembers(e, CodeCompletionType.ListMembers);
        }
        protected void CompleteWord(CodeCompletionArgs e) {
            this.ListMembers(e, CodeCompletionType.CompleteWord);
        }
        protected virtual void ParameterInfo(CodeCompletionArgs e) {
            e.Init(CodeCompletionType.ParameterInfo, this.Position);
            e.ToolTip = true;
            this.ExecuteCodeCompletion(e);
        }
        protected virtual void QuickInfo(CodeCompletionArgs e) {
            this.QuickInfo(e, this.Position, false);
        }
        protected virtual void QuickInfo(CodeCompletionArgs e, Point position, bool needReparse) {
            e.Init(CodeCompletionType.QuickInfo, position, needReparse);
            e.ToolTip = true;
            this.ExecuteCodeCompletion(e);
        }
        protected virtual void CodeSnippets(CodeCompletionArgs e) {
            e.Init(CodeCompletionType.CodeSnippets, this.Position, false);
            this.ExecuteCodeCompletion(e);
        }
        protected virtual CodeCompletionBox CreateCodeCompletionBox() {
            return new CodeCompletionBox(this);
        }
        protected virtual CodeCompletionHint CreateCodeCompletionHint() {
            return new CodeCompletionHint(this);
        }
        protected virtual void ShowCodeCompletionBox(ICodeCompletionProvider provider, CodeCompletionType completionType, Point pt, Point startPoint, Point endPoint) {
            this.CodeCompletionBox.Provider = provider;
            this.CodeCompletionBox.StartPos = startPoint;
            this.CodeCompletionBox.EndPos = endPoint;
            this.CodeCompletionBox.ShowTabs = this.Source.NeedCodeCompletionTabs(completionType);
            this.CodeCompletionBox.PopupAt(pt);
        }
        protected virtual void ShowCodeCompletionHint(ICodeCompletionProvider provider, Point pt, Point startPoint, Point endPoint, Point displayPoint, bool keepActive, bool acceptOnTab, Teal.Syntax.Lexer.Lexer lexer) {
            CodeCompletionHint codeCompletionHint = this.CodeCompletionHint;
            codeCompletionHint.StartPos = startPoint;
            codeCompletionHint.DisplayPos = displayPoint;
            codeCompletionHint.EndPos = endPoint;
            if (codeCompletionHint.Provider != provider) {
                codeCompletionHint.Lexer = lexer;
                codeCompletionHint.Provider = provider;
            }
            if (keepActive) {
                codeCompletionHint.CompletionFlags |= CodeCompletionFlags.KeepActive;
            } else {
                codeCompletionHint.CompletionFlags &= ~CodeCompletionFlags.KeepActive;
            }
            if (acceptOnTab) {
                codeCompletionHint.CompletionFlags |= CodeCompletionFlags.AcceptOnTab;
            } else {
                codeCompletionHint.CompletionFlags &= ~CodeCompletionFlags.AcceptOnTab;
            }
            codeCompletionHint.PopupAt(pt);
        }
        protected virtual bool IsValidText(byte style) {
            return this.Lexer == null || !this.Lexer.Scheme.IsPlainText((int)style);
        }
        public virtual byte TextStyleAt(Point position) {
            byte result;
            if (this.Lexer != null) {
                StringItem item = this.Lines.GetItem(position.Y);
                if (item != null) {
                    if (position.X == item.String.Length) {
                        position.X--;
                    }
                    if (position.X >= 0 && position.X < item.TextData.Length) {
                        result = item.TextData[position.X].Data;
                        return result;
                    }
                }
            }
            result = 0;
            return result;
        }
        public virtual void ShowCodeCompletionBox(ICodeCompletionProvider provider, CodeCompletionType completionType) {
            Point position = base.PointToScreen(this.TextToScreen((this.codeCompletionArgs.StartPosition.Y >= 0 && this.codeCompletionArgs.StartPosition.X >= 0) ? this.codeCompletionArgs.StartPosition : this.Position));
            position.Y += this.painter222.FontHeight;
            if (this.codeCompletionHint != null && this.codeCompletionHint.Visible && this.codeCompletionHint.Provider is ParameterInfo) {
                position.Y += this.codeCompletionHint.Height;
            }
            this.ShowCodeCompletionBox(provider, completionType, position);
        }
        public virtual void ShowCodeCompletionBox(ICodeCompletionProvider provider, CodeCompletionType completionType, Point position) {
            this.ShowCodeCompletionBox(provider, completionType, position, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.EndPosition);
        }
        public virtual void ShowCodeCompletionHint(ICodeCompletionProvider provider, Teal.Syntax.Lexer.Lexer lexer) {
            Point position = base.PointToScreen(this.TextToScreen((this.codeCompletionArgs.StartPosition.Y >= 0 && this.codeCompletionArgs.StartPosition.X >= 0) ? this.codeCompletionArgs.StartPosition : this.Position));
            position.Y += this.painter222.FontHeight;
            this.ShowCodeCompletionHint(provider, position, lexer);
        }
        public virtual void ShowCodeCompletionHint(ICodeCompletionProvider provider, Point position, Teal.Syntax.Lexer.Lexer lexer) {
            this.ShowCodeCompletionHint(provider, position, this.codeCompletionArgs.StartPosition, this.codeCompletionArgs.EndPosition, this.codeCompletionArgs.DisplayPosition, false, this.codeCompletionArgs.CompletionType == CodeCompletionType.SpecialListMembers, lexer);
        }
        public virtual bool IsValidText(Point position) {
            byte b = this.TextStyleAt(position);
            return b <= 0 || this.IsValidText((byte)(b - 1));
        }
        public virtual void CompleteWord() {
            if (this.IsValidText(this.Position)) {
                this.ListMembers(this.codeCompletionArgs, CodeCompletionType.CompleteWord);
            }
        }
        public virtual void CodeCompletion(CodeCompletionArgs e) {
            if (this.Lexer is SyntaxParser) {
                this.DoCodeCompletion((SyntaxParser)this.Lexer, this.Position, false, e);
            }
            if (this.NeedCodeCompletion != null) {
                this.NeedCodeCompletion(this, e);
            }
        }
        public virtual bool CodeCompletionWindowFocused(out Control control) {
            bool result;
            if (this.codeCompletionBox != null && this.codeCompletionBox.IsFocused) {
                control = (Control)this.codeCompletionBox;
                result = true;
            } else {
                if (this.codeCompletionHint != null && this.codeCompletionHint.IsFocused) {
                    control = (Control)this.codeCompletionHint;
                    result = true;
                } else {
                    control = null;
                    result = false;
                }
            }
            return result;
        }
        public virtual void ListMembers() {
            if (this.IsValidText(this.Position)) {
                this.ListMembers(this.codeCompletionArgs, CodeCompletionType.ListMembers);
            }
        }
        public virtual void QuickInfo() {
            this.QuickInfo(this.codeCompletionArgs);
        }
        public virtual void ParameterInfo() {
            this.ParameterInfo(this.codeCompletionArgs);
        }
        public virtual void CodeSnippets() {
            this.CodeSnippets(this.codeCompletionArgs);
        }
    }

}