using System;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom.VBNet
{
	public class VBExpressionFinder : IExpressionFinder
	{
		private int initialOffset;

		private string text;

		private int offset;

		private static int Err = 0;

		private static int Dot = 1;

		private static int StrLit = 2;

		private static int Ident = 3;

		private static int New = 4;

		private static int Parent = 6;

		private static int Curly = 7;

		private static int Using = 8;

		private int curTokenType;

		private static readonly string[] tokenStateName = new string[]
		{
			"Err",
			"Dot",
			"StrLit",
			"Ident",
			"New",
			"Bracket",
			"Paren",
			"Curly",
			"Using"
		};

		private string lastIdentifier;

		private static readonly int ERROR = 0;

		private static readonly int START = 1;

		private static readonly int DOT = 2;

		private static readonly int MORE = 3;

		private static readonly int CURLY = 4;

		private static readonly int CURLY2 = 5;

		private static readonly int CURLY3 = 6;

		private static readonly int ACCEPT = 7;

		private static readonly int ACCEPTNOMORE = 8;

		private static readonly int ACCEPT2 = 9;

		private static readonly string[] stateName = new string[]
		{
			"ERROR",
			"START",
			"DOT",
			"MORE",
			"CURLY",
			"CURLY2",
			"CURLY3",
			"ACCEPT",
			"ACCEPTNOMORE",
			"ACCEPT2"
		};

		private int state = 0;

		private int lastAccept = 0;

		private static int[,] stateTable;

		internal int LastExpressionStartPosition
		{
			get
			{
				return ((this.state == VBExpressionFinder.ACCEPTNOMORE) ? this.offset : this.lastAccept) + 1;
			}
		}

		private ExpressionResult CreateResult(string expression)
		{
			ExpressionResult result;
			if (expression == null)
			{
				result = new ExpressionResult(null);
			}
			else if (expression.Length > 8 && expression.Substring(0, 8).Equals("Imports ", StringComparison.InvariantCultureIgnoreCase))
			{
				result = new ExpressionResult(expression.Substring(8).TrimStart(new char[0]), ExpressionContext.Type, null);
			}
			else if (expression.Length > 4 && expression.Substring(0, 4).Equals("New ", StringComparison.InvariantCultureIgnoreCase))
			{
				result = new ExpressionResult(expression.Substring(4).TrimStart(new char[0]), ExpressionContext.ObjectCreation, null);
			}
			else if (this.curTokenType == VBExpressionFinder.Ident && this.lastIdentifier.Equals("as", StringComparison.InvariantCultureIgnoreCase))
			{
				result = new ExpressionResult(expression, ExpressionContext.Type);
			}
			else
			{
				result = new ExpressionResult(expression);
			}
			return result;
		}

		public ExpressionResult FindExpression(string inText, int offset)
		{
			return this.CreateResult(this.FindExpressionInternal(inText, offset));
		}

		public string FindExpressionInternal(string inText, int offset)
		{
			this.text = this.FilterComments(inText, ref offset);
			this.offset = (this.lastAccept = offset);
			this.state = VBExpressionFinder.START;
			string result;
			if (this.text == null)
			{
				result = null;
			}
			else
			{
				while (this.state != VBExpressionFinder.ERROR)
				{
					this.ReadNextToken();
					this.state = VBExpressionFinder.stateTable[this.state, this.curTokenType];
					if (this.state == VBExpressionFinder.ACCEPT || this.state == VBExpressionFinder.ACCEPT2 || this.state == VBExpressionFinder.DOT)
					{
						this.lastAccept = this.offset;
					}
					if (this.state == VBExpressionFinder.ACCEPTNOMORE)
					{
						result = this.text.Substring(this.offset + 1, offset - this.offset);
						return result;
					}
				}
				result = this.text.Substring(this.lastAccept + 1, offset - this.lastAccept);
			}
			return result;
		}

		public ExpressionResult FindFullExpression(string inText, int offset)
		{
			string expressionBeforeOffset = this.FindExpressionInternal(inText, offset);
			ExpressionResult result;
			if (expressionBeforeOffset == null || expressionBeforeOffset.Length == 0)
			{
				result = this.CreateResult(null);
			}
			else
			{
				StringBuilder b = new StringBuilder(expressionBeforeOffset);
				for (int i = offset + 1; i < inText.Length; i++)
				{
					char c = inText[i];
					if (char.IsLetterOrDigit(c) || c == '_')
					{
						if (char.IsWhiteSpace(inText, i - 1))
						{
							break;
						}
						b.Append(c);
					}
					else if (c == ' ')
					{
						b.Append(c);
					}
					else
					{
						if (c != '(')
						{
							break;
						}
						int otherBracket = this.SearchBracketForward(inText, i + 1, '(', ')');
						if (otherBracket < 0)
						{
							break;
						}
						b.Append(inText, i, otherBracket - i + 1);
						break;
					}
				}
				if (b.Length > 0 && b[b.Length - 1] == ' ')
				{
					b.Length--;
				}
				result = this.CreateResult(b.ToString());
			}
			return result;
		}

		private int SearchBracketForward(string text, int offset, char openBracket, char closingBracket)
		{
			bool inString = false;
			bool inComment = false;
			int brackets = 1;
			int result;
			for (int i = offset; i < text.Length; i++)
			{
				char ch = text[i];
				if (ch == '\n')
				{
					inString = false;
					inComment = false;
				}
				if (!inComment)
				{
					if (ch == '"')
					{
						inString = !inString;
					}
					if (!inString)
					{
						if (ch == '\'')
						{
							inComment = true;
						}
						else if (ch == openBracket)
						{
							brackets++;
						}
						else if (ch == closingBracket)
						{
							brackets--;
							if (brackets == 0)
							{
								result = i;
								return result;
							}
						}
					}
				}
			}
			result = -1;
			return result;
		}

		public string RemoveLastPart(string expression)
		{
			this.text = expression;
			this.offset = this.text.Length - 1;
			this.ReadNextToken();
			if (this.curTokenType == VBExpressionFinder.Ident && this.Peek() == '.')
			{
				this.GetNext();
			}
			return this.text.Substring(0, this.offset + 1);
		}

		public string FilterComments(string text, ref int offset)
		{
			string result;
			if (text.Length <= offset)
			{
				result = null;
			}
			else
			{
				this.initialOffset = offset;
				StringBuilder outText = new StringBuilder();
				int curOffset = 0;
				while (curOffset <= this.initialOffset)
				{
					char ch = text[curOffset];
					char c = ch;
					if (c != '"')
					{
						if (c != '\'')
						{
							if (c != '@')
							{
								outText.Append(ch);
								curOffset++;
							}
							else if (curOffset + 1 < text.Length && text[curOffset + 1] == '"')
							{
								outText.Append(text[curOffset++]);
								outText.Append(text[curOffset++]);
								if (!this.ReadVerbatimString(outText, text, ref curOffset))
								{
									result = null;
									return result;
								}
							}
							else
							{
								outText.Append(ch);
								curOffset++;
							}
						}
						else
						{
							offset--;
							curOffset++;
							if (!this.ReadToEOL(text, ref curOffset, ref offset))
							{
								result = null;
								return result;
							}
						}
					}
					else
					{
						outText.Append(ch);
						curOffset++;
						if (!this.ReadString(outText, text, ref curOffset))
						{
							result = null;
							return result;
						}
					}
				}
				result = outText.ToString();
			}
			return result;
		}

		private bool ReadToEOL(string text, ref int curOffset, ref int offset)
		{
			bool result;
			while (curOffset <= this.initialOffset)
			{
				char ch = text[curOffset++];
				offset--;
				if (ch == '\n')
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private bool ReadString(StringBuilder outText, string text, ref int curOffset)
		{
			bool result;
			while (curOffset <= this.initialOffset)
			{
				char ch = text[curOffset++];
				outText.Append(ch);
				if (ch == '"')
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private bool ReadVerbatimString(StringBuilder outText, string text, ref int curOffset)
		{
			bool result;
			while (curOffset <= this.initialOffset)
			{
				char ch = text[curOffset++];
				outText.Append(ch);
				if (ch == '"')
				{
					if (curOffset >= text.Length || text[curOffset] != '"')
					{
						result = true;
						return result;
					}
					outText.Append(text[curOffset++]);
				}
			}
			result = false;
			return result;
		}

		private bool ReadMultiLineComment(string text, ref int curOffset, ref int offset)
		{
			bool result;
			while (curOffset <= this.initialOffset)
			{
				char ch = text[curOffset++];
				offset--;
				if (ch == '*')
				{
					if (curOffset < text.Length && text[curOffset] == '/')
					{
						curOffset++;
						offset--;
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		private char GetNext()
		{
			char result;
			if (this.offset >= 0)
			{
				result = this.text[this.offset--];
			}
			else
			{
				result = '\0';
			}
			return result;
		}

		private char Peek()
		{
			char result;
			if (this.offset >= 0)
			{
				result = this.text[this.offset];
			}
			else
			{
				result = '\0';
			}
			return result;
		}

		private void UnGet()
		{
			this.offset++;
		}

		private string GetTokenName(int state)
		{
			return VBExpressionFinder.tokenStateName[state];
		}

		private void ReadNextToken()
		{
			char ch = this.GetNext();
			this.curTokenType = VBExpressionFinder.Err;
			if (ch != '\0' && ch != '\n' && ch != '\r')
			{
				while (char.IsWhiteSpace(ch))
				{
					ch = this.GetNext();
					if (ch == '\n' || ch == '\r')
					{
						return;
					}
				}
				char c = ch;
				if (c <= ')')
				{
					if (c != '"')
					{
						switch (c)
						{
						case '\'':
							break;
						case '(':
							goto IL_132;
						case ')':
							if (this.ReadBracket('(', ')'))
							{
								this.curTokenType = VBExpressionFinder.Parent;
							}
							return;
						default:
							goto IL_132;
						}
					}
					if (this.ReadStringLiteral(ch))
					{
						this.curTokenType = VBExpressionFinder.StrLit;
					}
					return;
				}
				if (c == '.')
				{
					this.curTokenType = VBExpressionFinder.Dot;
					return;
				}
				if (c == ']')
				{
					if (this.ReadBracket('[', ']'))
					{
						this.curTokenType = VBExpressionFinder.Ident;
					}
					return;
				}
				if (c == '}')
				{
					if (this.ReadBracket('{', '}'))
					{
						this.curTokenType = VBExpressionFinder.Curly;
					}
					return;
				}
				IL_132:
				if (this.IsIdentifierPart(ch))
				{
					string ident = this.ReadIdentifier(ch);
					if (ident != null)
					{
						string text = ident.ToLowerInvariant();
						if (text != null)
						{
							if (text == "new")
							{
								this.curTokenType = VBExpressionFinder.New;
								goto IL_1AA;
							}
							if (text == "imports")
							{
								this.curTokenType = VBExpressionFinder.Using;
								goto IL_1AA;
							}
						}
						this.lastIdentifier = ident;
						this.curTokenType = VBExpressionFinder.Ident;
						IL_1AA:;
					}
				}
			}
		}

		private bool ReadStringLiteral(char litStart)
		{
			while (true)
			{
				char ch = this.GetNext();
				if (ch == '\0')
				{
					break;
				}
				if (ch == litStart)
				{
					goto Block_2;
				}
			}
			bool result = false;
			return result;
			Block_2:
			if (this.Peek() == '@' && litStart == '"')
			{
				this.GetNext();
			}
			result = true;
			return result;
		}

		private bool ReadBracket(char openBracket, char closingBracket)
		{
			int curlyBraceLevel = 0;
			int squareBracketLevel = 0;
			int parenthesisLevel = 0;
			if (openBracket != '(')
			{
				if (openBracket != '[')
				{
					if (openBracket == '{')
					{
						curlyBraceLevel++;
					}
				}
				else
				{
					squareBracketLevel++;
				}
			}
			else
			{
				parenthesisLevel++;
			}
			bool result;
			while (parenthesisLevel != 0 || squareBracketLevel != 0 || curlyBraceLevel != 0)
			{
				char ch = this.GetNext();
				if (ch == '\0')
				{
					result = false;
					return result;
				}
				char c = ch;
				switch (c)
				{
				case '(':
					parenthesisLevel--;
					break;
				case ')':
					parenthesisLevel++;
					break;
				default:
					switch (c)
					{
					case '[':
						squareBracketLevel--;
						break;
					case '\\':
						break;
					case ']':
						squareBracketLevel++;
						break;
					default:
						switch (c)
						{
						case '{':
							curlyBraceLevel--;
							break;
						case '}':
							curlyBraceLevel++;
							break;
						}
						break;
					}
					break;
				}
			}
			result = true;
			return result;
		}

		private string ReadIdentifier(char ch)
		{
			string identifier = ch.ToString();
			while (this.IsIdentifierPart(this.Peek()))
			{
				identifier = this.GetNext() + identifier;
			}
			return identifier;
		}

		private bool IsIdentifierPart(char ch)
		{
			return char.IsLetterOrDigit(ch) || ch == '_';
		}

		private string GetStateName(int state)
		{
			return VBExpressionFinder.stateName[state];
		}

		static VBExpressionFinder()
		{
			// Note: this type is marked as 'beforefieldinit'.
			int[,] array = new int[10, 9];
			array[0, 0] = VBExpressionFinder.ERROR;
			array[0, 1] = VBExpressionFinder.ERROR;
			array[0, 2] = VBExpressionFinder.ERROR;
			array[0, 3] = VBExpressionFinder.ERROR;
			array[0, 4] = VBExpressionFinder.ERROR;
			array[0, 5] = VBExpressionFinder.ERROR;
			array[0, 6] = VBExpressionFinder.ERROR;
			array[0, 7] = VBExpressionFinder.ERROR;
			array[0, 8] = VBExpressionFinder.ERROR;
			array[1, 0] = VBExpressionFinder.ERROR;
			array[1, 1] = VBExpressionFinder.ERROR;
			array[1, 2] = VBExpressionFinder.ACCEPT;
			array[1, 3] = VBExpressionFinder.ACCEPT;
			array[1, 4] = VBExpressionFinder.ERROR;
			array[1, 5] = VBExpressionFinder.MORE;
			array[1, 6] = VBExpressionFinder.ACCEPT2;
			array[1, 7] = VBExpressionFinder.CURLY;
			array[1, 8] = VBExpressionFinder.ACCEPTNOMORE;
			array[2, 0] = VBExpressionFinder.ERROR;
			array[2, 1] = VBExpressionFinder.ERROR;
			array[2, 2] = VBExpressionFinder.ACCEPT;
			array[2, 3] = VBExpressionFinder.ACCEPT;
			array[2, 4] = VBExpressionFinder.ERROR;
			array[2, 5] = VBExpressionFinder.MORE;
			array[2, 6] = VBExpressionFinder.ACCEPT2;
			array[2, 7] = VBExpressionFinder.CURLY;
			array[2, 8] = VBExpressionFinder.ERROR;
			array[3, 0] = VBExpressionFinder.ERROR;
			array[3, 1] = VBExpressionFinder.ERROR;
			array[3, 2] = VBExpressionFinder.ACCEPT;
			array[3, 3] = VBExpressionFinder.ACCEPT;
			array[3, 4] = VBExpressionFinder.ERROR;
			array[3, 5] = VBExpressionFinder.MORE;
			array[3, 6] = VBExpressionFinder.ACCEPT2;
			array[3, 7] = VBExpressionFinder.CURLY;
			array[3, 8] = VBExpressionFinder.ERROR;
			array[4, 0] = VBExpressionFinder.ERROR;
			array[4, 1] = VBExpressionFinder.ERROR;
			array[4, 2] = VBExpressionFinder.ERROR;
			array[4, 3] = VBExpressionFinder.ERROR;
			array[4, 4] = VBExpressionFinder.ERROR;
			array[4, 5] = VBExpressionFinder.CURLY2;
			array[4, 6] = VBExpressionFinder.ERROR;
			array[4, 7] = VBExpressionFinder.ERROR;
			array[4, 8] = VBExpressionFinder.ERROR;
			array[5, 0] = VBExpressionFinder.ERROR;
			array[5, 1] = VBExpressionFinder.ERROR;
			array[5, 2] = VBExpressionFinder.ERROR;
			array[5, 3] = VBExpressionFinder.CURLY3;
			array[5, 4] = VBExpressionFinder.ERROR;
			array[5, 5] = VBExpressionFinder.ERROR;
			array[5, 6] = VBExpressionFinder.ERROR;
			array[5, 7] = VBExpressionFinder.ERROR;
			array[5, 8] = VBExpressionFinder.ERROR;
			array[6, 0] = VBExpressionFinder.ERROR;
			array[6, 1] = VBExpressionFinder.ERROR;
			array[6, 2] = VBExpressionFinder.ERROR;
			array[6, 3] = VBExpressionFinder.ERROR;
			array[6, 4] = VBExpressionFinder.ACCEPTNOMORE;
			array[6, 5] = VBExpressionFinder.ERROR;
			array[6, 6] = VBExpressionFinder.ERROR;
			array[6, 7] = VBExpressionFinder.ERROR;
			array[6, 8] = VBExpressionFinder.ERROR;
			array[7, 0] = VBExpressionFinder.ERROR;
			array[7, 1] = VBExpressionFinder.DOT;
			array[7, 2] = VBExpressionFinder.ERROR;
			array[7, 3] = VBExpressionFinder.ERROR;
			array[7, 4] = VBExpressionFinder.ACCEPT;
			array[7, 5] = VBExpressionFinder.ERROR;
			array[7, 6] = VBExpressionFinder.ERROR;
			array[7, 7] = VBExpressionFinder.ERROR;
			array[7, 8] = VBExpressionFinder.ACCEPTNOMORE;
			array[8, 0] = VBExpressionFinder.ERROR;
			array[8, 1] = VBExpressionFinder.ERROR;
			array[8, 2] = VBExpressionFinder.ERROR;
			array[8, 3] = VBExpressionFinder.ERROR;
			array[8, 4] = VBExpressionFinder.ERROR;
			array[8, 5] = VBExpressionFinder.ERROR;
			array[8, 6] = VBExpressionFinder.ERROR;
			array[8, 7] = VBExpressionFinder.ERROR;
			array[8, 8] = VBExpressionFinder.ERROR;
			array[9, 0] = VBExpressionFinder.ERROR;
			array[9, 1] = VBExpressionFinder.DOT;
			array[9, 2] = VBExpressionFinder.ERROR;
			array[9, 3] = VBExpressionFinder.ACCEPT;
			array[9, 4] = VBExpressionFinder.ACCEPT;
			array[9, 5] = VBExpressionFinder.ERROR;
			array[9, 6] = VBExpressionFinder.ERROR;
			array[9, 7] = VBExpressionFinder.ERROR;
			array[9, 8] = VBExpressionFinder.ERROR;
			VBExpressionFinder.stateTable = array;
		}
	}
}
