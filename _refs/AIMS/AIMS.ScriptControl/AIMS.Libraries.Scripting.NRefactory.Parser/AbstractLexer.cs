using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	public abstract class AbstractLexer : ILexer, IDisposable
	{
		private TextReader reader;

		private int col = 1;

		private int line = 1;

		[CLSCompliant(false)]
		protected Errors errors = new Errors();

		protected Token lastToken = null;

		protected Token curToken = null;

		protected Token peekToken = null;

		private string[] specialCommentTags = null;

		protected Hashtable specialCommentHash = null;

		private List<TagComment> tagComments = new List<TagComment>();

		protected StringBuilder sb = new StringBuilder();

		[CLSCompliant(false)]
		protected SpecialTracker specialTracker = new SpecialTracker();

		protected StringBuilder originalValue = new StringBuilder();

		private bool skipAllComments = false;

		public bool SkipAllComments
		{
			get
			{
				return this.skipAllComments;
			}
			set
			{
				this.skipAllComments = value;
			}
		}

		protected int Line
		{
			get
			{
				return this.line;
			}
		}

		protected int Col
		{
			get
			{
				return this.col;
			}
		}

		public Errors Errors
		{
			get
			{
				return this.errors;
			}
		}

		public List<TagComment> TagComments
		{
			get
			{
				return this.tagComments;
			}
		}

		public SpecialTracker SpecialTracker
		{
			get
			{
				return this.specialTracker;
			}
		}

		public string[] SpecialCommentTags
		{
			get
			{
				return this.specialCommentTags;
			}
			set
			{
				this.specialCommentTags = value;
				this.specialCommentHash = null;
				if (this.specialCommentTags != null && this.specialCommentTags.Length > 0)
				{
					this.specialCommentHash = new Hashtable();
					string[] array = this.specialCommentTags;
					for (int i = 0; i < array.Length; i++)
					{
						string str = array[i];
						this.specialCommentHash.Add(str, null);
					}
				}
			}
		}

		public Token Token
		{
			get
			{
				return this.lastToken;
			}
		}

		public Token LookAhead
		{
			get
			{
				return this.curToken;
			}
		}

		protected int ReaderRead()
		{
			this.col++;
			return this.reader.Read();
		}

		protected int ReaderPeek()
		{
			return this.reader.Peek();
		}

		protected AbstractLexer(TextReader reader)
		{
			this.reader = reader;
		}

		public virtual void Dispose()
		{
			this.reader.Close();
			this.reader = null;
			this.errors = null;
			this.lastToken = (this.curToken = (this.peekToken = null));
			this.specialCommentHash = null;
			this.tagComments = null;
			this.sb = (this.originalValue = null);
		}

		public void StartPeek()
		{
			this.peekToken = this.curToken;
		}

		public Token Peek()
		{
			if (this.peekToken.next == null)
			{
				this.peekToken.next = this.Next();
				this.specialTracker.InformToken(this.peekToken.next.kind);
			}
			this.peekToken = this.peekToken.next;
			return this.peekToken;
		}

		public virtual Token NextToken()
		{
			Token result;
			if (this.curToken == null)
			{
				this.curToken = this.Next();
				this.specialTracker.InformToken(this.curToken.kind);
				result = this.curToken;
			}
			else
			{
				this.lastToken = this.curToken;
				if (this.curToken.next == null)
				{
					this.curToken.next = this.Next();
					if (this.curToken.next != null)
					{
						this.specialTracker.InformToken(this.curToken.next.kind);
					}
				}
				this.curToken = this.curToken.next;
				result = this.curToken;
			}
			return result;
		}

		protected abstract Token Next();

		protected static bool IsIdentifierPart(int ch)
		{
			return ch == 95 || (ch != -1 && char.IsLetterOrDigit((char)ch));
		}

		protected static bool IsHex(char digit)
		{
			return char.IsDigit(digit) || ('A' <= digit && digit <= 'F') || ('a' <= digit && digit <= 'f');
		}

		protected int GetHexNumber(char digit)
		{
			int result;
			if (char.IsDigit(digit))
			{
				result = (int)(digit - '0');
			}
			else if ('A' <= digit && digit <= 'F')
			{
				result = (int)(digit - 'A' + '\n');
			}
			else if ('a' <= digit && digit <= 'f')
			{
				result = (int)(digit - 'a' + '\n');
			}
			else
			{
				this.errors.Error(this.line, this.col, string.Format("Invalid hex number '" + digit + "'", new object[0]));
				result = 0;
			}
			return result;
		}

		protected bool WasLineEnd(char ch)
		{
			if (ch == '\r')
			{
				if (this.reader.Peek() == 10)
				{
					ch = (char)this.reader.Read();
					this.col++;
				}
				else
				{
					ch = '\n';
				}
			}
			return ch == '\n';
		}

		protected bool HandleLineEnd(char ch)
		{
			bool result;
			if (this.WasLineEnd(ch))
			{
				this.line++;
				this.col = 1;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected void SkipToEndOfLine()
		{
			int nextChar;
			while ((nextChar = this.reader.Read()) != -1)
			{
				if (this.HandleLineEnd((char)nextChar))
				{
					break;
				}
			}
		}

		protected string ReadToEndOfLine()
		{
			this.sb.Length = 0;
			int nextChar;
			string result;
			while ((nextChar = this.reader.Read()) != -1)
			{
				char ch = (char)nextChar;
				if (this.HandleLineEnd(ch))
				{
					result = this.sb.ToString();
					return result;
				}
				this.sb.Append(ch);
			}
			string retStr = this.sb.ToString();
			this.col += retStr.Length;
			result = retStr;
			return result;
		}

		public abstract void SkipCurrentBlock(int targetToken);
	}
}
