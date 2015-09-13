using AIMS.Libraries.Scripting.NRefactory.Parser.CSharp;
using System;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom.CSharp
{
	public class CSharpExpressionFinder : IExpressionFinder
	{
		private string fileName;

		private int lastExpressionStartPosition;

		private int initialOffset;

		private string text;

		private int offset;

		private static int Err = 0;

		private static int Dot = 1;

		private static int StrLit = 2;

		private static int Ident = 3;

		private static int New = 4;

		private static int Bracket = 5;

		private static int Parent = 6;

		private static int Curly = 7;

		private static int Using = 8;

		private static int Digit = 9;

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
			"Using",
			"Digit"
		};

		private bool hadParenthesis;

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

		public int LastExpressionStartPosition
		{
			get
			{
				return this.lastExpressionStartPosition;
			}
		}

		public CSharpExpressionFinder(string fileName)
		{
			this.fileName = fileName;
		}

		private ExpressionResult CreateResult(string expression, string inText, int offset)
		{
			ExpressionResult result;
			if (expression == null)
			{
				result = new ExpressionResult(null);
			}
			else if (expression.StartsWith("using "))
			{
				result = new ExpressionResult(expression.Substring(6).TrimStart(new char[0]), ExpressionContext.Namespace, null);
			}
			else if (!this.hadParenthesis && expression.StartsWith("new "))
			{
				result = new ExpressionResult(expression.Substring(4).TrimStart(new char[0]), this.GetCreationContext(), null);
			}
			else if (this.IsInAttribute(inText, offset))
			{
				result = new ExpressionResult(expression, ExpressionContext.GetAttribute(HostCallback.GetCurrentProjectContent()));
			}
			else
			{
				result = new ExpressionResult(expression);
			}
			return result;
		}

		private ExpressionContext GetCreationContext()
		{
			this.UnGetToken();
			ExpressionContext result;
			if (this.GetNextNonWhiteSpace() == '=')
			{
				this.ReadNextToken();
				if (this.curTokenType == CSharpExpressionFinder.Ident)
				{
					int typeEnd = this.offset;
					this.ReadNextToken();
					int typeStart = -1;
					while (this.curTokenType == CSharpExpressionFinder.Ident)
					{
						typeStart = this.offset + 1;
						this.ReadNextToken();
						if (this.curTokenType != CSharpExpressionFinder.Dot)
						{
							break;
						}
						this.ReadNextToken();
					}
					if (typeStart >= 0)
					{
						string className = this.text.Substring(typeStart, typeEnd - typeStart);
						int pos = className.IndexOf('<');
						int typeParameterCount = 0;
						string nonGenericClassName;
						string genericPart;
						if (pos > 0)
						{
							nonGenericClassName = className.Substring(0, pos);
							genericPart = className.Substring(pos);
							pos = 0;
							do
							{
								typeParameterCount++;
								pos = genericPart.IndexOf(',', pos + 1);
							}
							while (pos > 0);
						}
						else
						{
							nonGenericClassName = className;
							genericPart = null;
						}
						ClassFinder finder = new ClassFinder(this.fileName, this.text, typeStart);
						IReturnType t = finder.SearchType(nonGenericClassName, typeParameterCount);
						IClass c = (t != null) ? t.GetUnderlyingClass() : null;
						if (c != null)
						{
							ExpressionContext context = ExpressionContext.TypeDerivingFrom(c, true);
							if (context.ShowEntry(c))
							{
								if (genericPart != null)
								{
									context.SuggestedItem = new DefaultClass(c.CompilationUnit, c.ClassType, c.Modifiers, c.Region, c.DeclaringType)
									{
										FullyQualifiedName = c.FullyQualifiedName + genericPart,
										Documentation = c.Documentation
									};
								}
								else
								{
									context.SuggestedItem = c;
								}
							}
							result = context;
							return result;
						}
					}
				}
			}
			else
			{
				this.UnGet();
				if (this.ReadIdentifier(this.GetNextNonWhiteSpace()) == "throw")
				{
					result = ExpressionContext.TypeDerivingFrom(HostCallback.GetCurrentProjectContent().GetClass("System.Exception"), true);
					return result;
				}
			}
			result = ExpressionContext.ObjectCreation;
			return result;
		}

		private bool IsInAttribute(string txt, int offset)
		{
			int lineStart = offset;
			while (--lineStart > 0 && txt[lineStart] != '\n')
			{
			}
			bool inAttribute = false;
			int parens = 0;
			bool result;
			for (int i = lineStart + 1; i < offset; i++)
			{
				char ch = txt[i];
				if (!char.IsWhiteSpace(ch))
				{
					if (!inAttribute)
					{
						if (ch != '[')
						{
							result = false;
							return result;
						}
						inAttribute = true;
					}
					else if (parens == 0)
					{
						if (ch == ']')
						{
							inAttribute = false;
						}
						else if (ch == '(')
						{
							parens = 1;
						}
						else if (!char.IsLetterOrDigit(ch) && ch != ',')
						{
							result = false;
							return result;
						}
					}
					else if (ch == '(')
					{
						parens++;
					}
					else if (ch == ')')
					{
						parens--;
					}
				}
			}
			result = (inAttribute && parens == 0);
			return result;
		}

		public string RemoveLastPart(string expression)
		{
			this.text = expression;
			this.offset = this.text.Length - 1;
			this.ReadNextToken();
			if (this.curTokenType == CSharpExpressionFinder.Ident && this.Peek() == '.')
			{
				this.GetNext();
			}
			return this.text.Substring(0, this.offset + 1);
		}

		public ExpressionResult FindExpression(string inText, int offset)
		{
			inText = this.FilterComments(inText, ref offset);
			return this.CreateResult(this.FindExpressionInternal(inText, offset), inText, offset);
		}

		public string FindExpressionInternal(string inText, int offset)
		{
			this.text = inText;
			this.lastAccept = offset;
			this.offset = offset;
			this.state = CSharpExpressionFinder.START;
			this.hadParenthesis = false;
			string result;
			if (this.text == null)
			{
				result = null;
			}
			else
			{
				while (this.state != CSharpExpressionFinder.ERROR)
				{
					this.ReadNextToken();
					this.state = CSharpExpressionFinder.stateTable[this.state, this.curTokenType];
					if (this.state == CSharpExpressionFinder.ACCEPT || this.state == CSharpExpressionFinder.ACCEPT2)
					{
						this.lastAccept = this.offset;
					}
					if (this.state == CSharpExpressionFinder.ACCEPTNOMORE)
					{
						this.lastExpressionStartPosition = this.offset + 1;
						result = this.text.Substring(this.offset + 1, offset - this.offset);
						return result;
					}
				}
				if (this.lastAccept < -1)
				{
					result = null;
				}
				else
				{
					this.lastExpressionStartPosition = this.lastAccept + 1;
					result = this.text.Substring(this.lastAccept + 1, offset - this.lastAccept);
				}
			}
			return result;
		}

		public ExpressionResult FindFullExpression(string inText, int offset)
		{
			int offsetWithoutComments = offset;
			string textWithoutComments = this.FilterComments(inText, ref offsetWithoutComments);
			string expressionBeforeOffset = this.FindExpressionInternal(textWithoutComments, offsetWithoutComments);
			ExpressionResult result;
			if (expressionBeforeOffset == null || expressionBeforeOffset.Length == 0)
			{
				result = this.CreateResult(null, textWithoutComments, offsetWithoutComments);
			}
			else
			{
				StringBuilder b = new StringBuilder(expressionBeforeOffset);
				bool wordFollowing = false;
				int i;
				for (i = offset + 1; i < inText.Length; i++)
				{
					char c = inText[i];
					if (char.IsLetterOrDigit(c) || c == '_')
					{
						if (char.IsWhiteSpace(inText, i - 1))
						{
							wordFollowing = true;
							break;
						}
						b.Append(c);
					}
					else if (!char.IsWhiteSpace(c))
					{
						if (c == '(' || c == '[')
						{
							int otherBracket = this.SearchBracketForward(inText, i + 1, c, (c == '(') ? ')' : ']');
							if (otherBracket < 0)
							{
								break;
							}
							if (c == '[')
							{
								bool ok = false;
								for (int j = i + 1; j < otherBracket; j++)
								{
									if (inText[j] != ',' && !char.IsWhiteSpace(inText, j))
									{
										ok = true;
										break;
									}
								}
								if (!ok)
								{
									break;
								}
							}
							b.Append(inText, i, otherBracket - i + 1);
							break;
						}
						else
						{
							if (c != '<')
							{
								break;
							}
							int typeParameterEnd = this.FindEndOfTypeParameters(inText, i);
							if (typeParameterEnd < 0)
							{
								break;
							}
							b.Append(inText, i, typeParameterEnd - i + 1);
							i = typeParameterEnd;
						}
					}
				}
				ExpressionResult res = this.CreateResult(b.ToString(), textWithoutComments, offsetWithoutComments);
				if (res.Context == ExpressionContext.Default && wordFollowing)
				{
					b = new StringBuilder();
					while (i < inText.Length)
					{
						char c = inText[i];
						if (!char.IsLetterOrDigit(c) && c != '_')
						{
							break;
						}
						b.Append(c);
						i++;
					}
					if (b.Length > 0)
					{
						if (Keywords.GetToken(b.ToString()) < 0)
						{
							res.Context = ExpressionContext.Type;
						}
					}
				}
				result = res;
			}
			return result;
		}

		private int FindEndOfTypeParameters(string inText, int offset)
		{
			int level = 0;
			int i = offset;
			int result;
			while (i < inText.Length)
			{
				char c = inText[i];
				if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c))
				{
					if (c != ',' && c != '?' && c != '[' && c != ']')
					{
						if (c == '<')
						{
							level++;
						}
						else
						{
							if (c != '>')
							{
								result = -1;
								return result;
							}
							level--;
						}
					}
				}
				if (level != 0)
				{
					i++;
					continue;
				}
				result = i;
				return result;
			}
			result = -1;
			return result;
		}

		private int SearchBracketForward(string text, int offset, char openBracket, char closingBracket)
		{
			bool inString = false;
			bool inChar = false;
			bool verbatim = false;
			bool lineComment = false;
			bool blockComment = false;
			int result;
			if (offset < 0)
			{
				result = -1;
			}
			else
			{
				int brackets = 1;
				while (offset < text.Length)
				{
					char ch = text[offset];
					char c = ch;
					if (c <= '"')
					{
						if (c != '\n' && c != '\r')
						{
							if (c != '"')
							{
								goto IL_1DA;
							}
							if (!inChar && !lineComment && !blockComment)
							{
								if (inString && verbatim)
								{
									if (offset + 1 < text.Length && text[offset + 1] == '"')
									{
										offset++;
										inString = false;
									}
									else
									{
										verbatim = false;
									}
								}
								else if (!inString && offset > 0 && text[offset - 1] == '@')
								{
									verbatim = true;
								}
								inString = !inString;
							}
						}
						else
						{
							lineComment = false;
							inChar = false;
							if (!verbatim)
							{
								inString = false;
							}
						}
					}
					else if (c != '\'')
					{
						if (c != '/')
						{
							if (c != '\\')
							{
								goto IL_1DA;
							}
							if ((inString && !verbatim) || inChar)
							{
								offset++;
							}
						}
						else
						{
							if (blockComment)
							{
								if (offset > 0 && text[offset - 1] == '*')
								{
									blockComment = false;
								}
							}
							if (!inString && !inChar && offset + 1 < text.Length)
							{
								if (!blockComment && text[offset + 1] == '/')
								{
									lineComment = true;
								}
								if (!lineComment && text[offset + 1] == '*')
								{
									blockComment = true;
								}
							}
						}
					}
					else if (!inString && !lineComment && !blockComment)
					{
						inChar = !inChar;
					}
					IL_24B:
					offset++;
					continue;
					IL_1DA:
					if (ch == openBracket)
					{
						if (!inString && !inChar && !lineComment && !blockComment)
						{
							brackets++;
						}
					}
					else if (ch == closingBracket)
					{
						if (!inString && !inChar && !lineComment && !blockComment)
						{
							brackets--;
							if (brackets == 0)
							{
								result = offset;
								return result;
							}
						}
					}
					goto IL_24B;
				}
				result = -1;
			}
			return result;
		}

		public string FilterComments(string text, ref int offset)
		{
			string result;
			if (text.Length < offset)
			{
				result = null;
			}
			else if (text.Length == offset)
			{
				result = text;
			}
			else
			{
				this.initialOffset = offset;
				StringBuilder outText = new StringBuilder();
				for (int curOffset = 0; curOffset <= this.initialOffset; curOffset++)
				{
					char ch = text[curOffset];
					char c = ch;
					if (c <= '\'')
					{
						switch (c)
						{
						case '"':
							outText.Append(ch);
							curOffset++;
							if (!this.ReadString(outText, text, ref curOffset))
							{
								result = null;
								return result;
							}
							break;
						case '#':
							if (!this.ReadToEOL(text, ref curOffset, ref offset))
							{
								result = null;
								return result;
							}
							break;
						default:
							if (c != '\'')
							{
								goto IL_206;
							}
							outText.Append(ch);
							curOffset++;
							if (!this.ReadChar(outText, text, ref curOffset))
							{
								result = null;
								return result;
							}
							break;
						}
					}
					else if (c != '/')
					{
						if (c != '@')
						{
							goto IL_206;
						}
						if (curOffset + 1 < text.Length && text[curOffset + 1] == '"')
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
					else if (curOffset + 1 < text.Length && text[curOffset + 1] == '/')
					{
						offset -= 2;
						curOffset += 2;
						if (!this.ReadToEOL(text, ref curOffset, ref offset))
						{
							result = null;
							return result;
						}
					}
					else
					{
						if (curOffset + 1 >= text.Length || text[curOffset + 1] != '*')
						{
							goto IL_206;
						}
						offset -= 2;
						curOffset += 2;
						if (!this.ReadMultiLineComment(text, ref curOffset, ref offset))
						{
							result = null;
							return result;
						}
					}
					continue;
					IL_206:
					outText.Append(ch);
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

		private bool ReadChar(StringBuilder outText, string text, ref int curOffset)
		{
			bool result;
			if (curOffset > this.initialOffset)
			{
				result = false;
			}
			else
			{
				char first = text[curOffset++];
				outText.Append(first);
				if (curOffset > this.initialOffset)
				{
					result = false;
				}
				else
				{
					char second = text[curOffset++];
					outText.Append(second);
					if (first == '\\')
					{
						while (curOffset <= this.initialOffset)
						{
							char next = text[curOffset++];
							outText.Append(next);
							if ((second != 'u' && second != 'x') || !char.IsLetterOrDigit(next))
							{
								goto IL_C5;
							}
						}
						result = false;
						return result;
					}
					IL_C5:
					result = (text[curOffset - 1] == '\'');
				}
			}
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
				if (ch == '\\')
				{
					if (curOffset <= this.initialOffset)
					{
						outText.Append(text[curOffset++]);
					}
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

		private char GetNextNonWhiteSpace()
		{
			char ch;
			do
			{
				ch = this.GetNext();
			}
			while (char.IsWhiteSpace(ch));
			return ch;
		}

		private char Peek(int n)
		{
			char result;
			if (this.offset - n >= 0)
			{
				result = this.text[this.offset - n];
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

		private void UnGetToken()
		{
			do
			{
				this.UnGet();
			}
			while (char.IsLetterOrDigit(this.Peek()));
		}

		private string GetTokenName(int state)
		{
			return CSharpExpressionFinder.tokenStateName[state];
		}

		private void ReadNextToken()
		{
			this.curTokenType = CSharpExpressionFinder.Err;
			char ch = this.GetNextNonWhiteSpace();
			if (ch != '\0')
			{
				char c = ch;
				if (c <= '.')
				{
					if (c != '"')
					{
						switch (c)
						{
						case '\'':
							break;
						case '(':
							goto IL_15C;
						case ')':
							if (this.ReadBracket('(', ')'))
							{
								this.hadParenthesis = true;
								this.curTokenType = CSharpExpressionFinder.Parent;
							}
							return;
						default:
							if (c != '.')
							{
								goto IL_15C;
							}
							this.curTokenType = CSharpExpressionFinder.Dot;
							return;
						}
					}
					if (this.ReadStringLiteral(ch))
					{
						this.curTokenType = CSharpExpressionFinder.StrLit;
					}
					return;
				}
				if (c <= '>')
				{
					if (c == ':')
					{
						if (this.GetNext() == ':')
						{
							this.curTokenType = CSharpExpressionFinder.Dot;
						}
						return;
					}
					if (c == '>')
					{
						if (this.ReadTypeParameters())
						{
							this.ReadNextToken();
						}
						return;
					}
				}
				else
				{
					if (c == ']')
					{
						if (this.ReadBracket('[', ']'))
						{
							this.curTokenType = CSharpExpressionFinder.Bracket;
						}
						return;
					}
					if (c == '}')
					{
						if (this.ReadBracket('{', '}'))
						{
							this.curTokenType = CSharpExpressionFinder.Curly;
						}
						return;
					}
				}
				IL_15C:
				if (this.IsNumber(ch))
				{
					this.ReadDigit(ch);
					this.curTokenType = CSharpExpressionFinder.Digit;
				}
				else if (this.IsIdentifierPart(ch))
				{
					string ident = this.ReadIdentifier(ch);
					if (ident != null)
					{
						string text = ident;
						switch (text)
						{
						case "new":
							this.curTokenType = CSharpExpressionFinder.New;
							goto IL_275;
						case "using":
							this.curTokenType = CSharpExpressionFinder.Using;
							goto IL_275;
						case "return":
						case "throw":
						case "in":
						case "else":
							goto IL_275;
						}
						this.curTokenType = CSharpExpressionFinder.Ident;
						this.lastIdentifier = ident;
						IL_275:;
					}
				}
			}
		}

		private bool IsNumber(char ch)
		{
			bool result;
			if (!char.IsDigit(ch))
			{
				result = false;
			}
			else
			{
				int i = 0;
				while (true)
				{
					ch = this.Peek(i);
					if (!char.IsDigit(ch))
					{
						break;
					}
					i++;
				}
				result = (i > 0 && !char.IsLetter(ch));
			}
			return result;
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

		private bool ReadTypeParameters()
		{
			int level = 1;
			bool result;
			while (level > 0)
			{
				char ch = this.GetNext();
				char c = ch;
				if (c != ',')
				{
					switch (c)
					{
					case '<':
						level--;
						continue;
					case '=':
						break;
					case '>':
						level++;
						continue;
					case '?':
						continue;
					default:
						switch (c)
						{
						case '[':
						case ']':
							continue;
						}
						break;
					}
					if (!char.IsWhiteSpace(ch) && !char.IsLetterOrDigit(ch))
					{
						result = false;
						return result;
					}
				}
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
				char c = ch;
				if (c <= ')')
				{
					if (c == '\0')
					{
						result = false;
						return result;
					}
					switch (c)
					{
					case '(':
						parenthesisLevel--;
						break;
					case ')':
						parenthesisLevel++;
						break;
					}
				}
				else
				{
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

		private void ReadDigit(char ch)
		{
			while (char.IsDigit(this.Peek()) || this.Peek() == '.')
			{
				this.GetNext();
			}
		}

		private bool IsIdentifierPart(char ch)
		{
			return char.IsLetterOrDigit(ch) || ch == '_' || ch == '@';
		}

		private string GetStateName(int state)
		{
			return CSharpExpressionFinder.stateName[state];
		}

		static CSharpExpressionFinder()
		{
			// Note: this type is marked as 'beforefieldinit'.
			int[,] array = new int[10, 10];
			array[0, 0] = CSharpExpressionFinder.ERROR;
			array[0, 1] = CSharpExpressionFinder.ERROR;
			array[0, 2] = CSharpExpressionFinder.ERROR;
			array[0, 3] = CSharpExpressionFinder.ERROR;
			array[0, 4] = CSharpExpressionFinder.ERROR;
			array[0, 5] = CSharpExpressionFinder.ERROR;
			array[0, 6] = CSharpExpressionFinder.ERROR;
			array[0, 7] = CSharpExpressionFinder.ERROR;
			array[0, 8] = CSharpExpressionFinder.ERROR;
			array[0, 9] = CSharpExpressionFinder.ERROR;
			array[1, 0] = CSharpExpressionFinder.ERROR;
			array[1, 1] = CSharpExpressionFinder.DOT;
			array[1, 2] = CSharpExpressionFinder.ACCEPT;
			array[1, 3] = CSharpExpressionFinder.ACCEPT;
			array[1, 4] = CSharpExpressionFinder.ERROR;
			array[1, 5] = CSharpExpressionFinder.MORE;
			array[1, 6] = CSharpExpressionFinder.ACCEPT2;
			array[1, 7] = CSharpExpressionFinder.CURLY;
			array[1, 8] = CSharpExpressionFinder.ACCEPTNOMORE;
			array[1, 9] = CSharpExpressionFinder.ERROR;
			array[2, 0] = CSharpExpressionFinder.ERROR;
			array[2, 1] = CSharpExpressionFinder.ERROR;
			array[2, 2] = CSharpExpressionFinder.ACCEPT;
			array[2, 3] = CSharpExpressionFinder.ACCEPT;
			array[2, 4] = CSharpExpressionFinder.ERROR;
			array[2, 5] = CSharpExpressionFinder.MORE;
			array[2, 6] = CSharpExpressionFinder.ACCEPT;
			array[2, 7] = CSharpExpressionFinder.CURLY;
			array[2, 8] = CSharpExpressionFinder.ERROR;
			array[2, 9] = CSharpExpressionFinder.ACCEPT;
			array[3, 0] = CSharpExpressionFinder.ERROR;
			array[3, 1] = CSharpExpressionFinder.ERROR;
			array[3, 2] = CSharpExpressionFinder.ACCEPT;
			array[3, 3] = CSharpExpressionFinder.ACCEPT;
			array[3, 4] = CSharpExpressionFinder.ERROR;
			array[3, 5] = CSharpExpressionFinder.MORE;
			array[3, 6] = CSharpExpressionFinder.ACCEPT2;
			array[3, 7] = CSharpExpressionFinder.CURLY;
			array[3, 8] = CSharpExpressionFinder.ERROR;
			array[3, 9] = CSharpExpressionFinder.ACCEPT;
			array[4, 0] = CSharpExpressionFinder.ERROR;
			array[4, 1] = CSharpExpressionFinder.ERROR;
			array[4, 2] = CSharpExpressionFinder.ERROR;
			array[4, 3] = CSharpExpressionFinder.ERROR;
			array[4, 4] = CSharpExpressionFinder.ERROR;
			array[4, 5] = CSharpExpressionFinder.CURLY2;
			array[4, 6] = CSharpExpressionFinder.ERROR;
			array[4, 7] = CSharpExpressionFinder.ERROR;
			array[4, 8] = CSharpExpressionFinder.ERROR;
			array[4, 9] = CSharpExpressionFinder.ERROR;
			array[5, 0] = CSharpExpressionFinder.ERROR;
			array[5, 1] = CSharpExpressionFinder.ERROR;
			array[5, 2] = CSharpExpressionFinder.ERROR;
			array[5, 3] = CSharpExpressionFinder.CURLY3;
			array[5, 4] = CSharpExpressionFinder.ERROR;
			array[5, 5] = CSharpExpressionFinder.ERROR;
			array[5, 6] = CSharpExpressionFinder.ERROR;
			array[5, 7] = CSharpExpressionFinder.ERROR;
			array[5, 8] = CSharpExpressionFinder.ERROR;
			array[5, 9] = CSharpExpressionFinder.CURLY3;
			array[6, 0] = CSharpExpressionFinder.ERROR;
			array[6, 1] = CSharpExpressionFinder.ERROR;
			array[6, 2] = CSharpExpressionFinder.ERROR;
			array[6, 3] = CSharpExpressionFinder.ERROR;
			array[6, 4] = CSharpExpressionFinder.ACCEPTNOMORE;
			array[6, 5] = CSharpExpressionFinder.ERROR;
			array[6, 6] = CSharpExpressionFinder.ERROR;
			array[6, 7] = CSharpExpressionFinder.ERROR;
			array[6, 8] = CSharpExpressionFinder.ERROR;
			array[6, 9] = CSharpExpressionFinder.ERROR;
			array[7, 0] = CSharpExpressionFinder.ERROR;
			array[7, 1] = CSharpExpressionFinder.MORE;
			array[7, 2] = CSharpExpressionFinder.ERROR;
			array[7, 3] = CSharpExpressionFinder.ERROR;
			array[7, 4] = CSharpExpressionFinder.ACCEPT;
			array[7, 5] = CSharpExpressionFinder.ERROR;
			array[7, 6] = CSharpExpressionFinder.ERROR;
			array[7, 7] = CSharpExpressionFinder.ERROR;
			array[7, 8] = CSharpExpressionFinder.ACCEPTNOMORE;
			array[7, 9] = CSharpExpressionFinder.ERROR;
			array[8, 0] = CSharpExpressionFinder.ERROR;
			array[8, 1] = CSharpExpressionFinder.ERROR;
			array[8, 2] = CSharpExpressionFinder.ERROR;
			array[8, 3] = CSharpExpressionFinder.ERROR;
			array[8, 4] = CSharpExpressionFinder.ERROR;
			array[8, 5] = CSharpExpressionFinder.ERROR;
			array[8, 6] = CSharpExpressionFinder.ERROR;
			array[8, 7] = CSharpExpressionFinder.ERROR;
			array[8, 8] = CSharpExpressionFinder.ERROR;
			array[8, 9] = CSharpExpressionFinder.ERROR;
			array[9, 0] = CSharpExpressionFinder.ERROR;
			array[9, 1] = CSharpExpressionFinder.MORE;
			array[9, 2] = CSharpExpressionFinder.ERROR;
			array[9, 3] = CSharpExpressionFinder.ACCEPT;
			array[9, 4] = CSharpExpressionFinder.ACCEPT;
			array[9, 5] = CSharpExpressionFinder.ERROR;
			array[9, 6] = CSharpExpressionFinder.ERROR;
			array[9, 7] = CSharpExpressionFinder.ERROR;
			array[9, 8] = CSharpExpressionFinder.ERROR;
			array[9, 9] = CSharpExpressionFinder.ACCEPT;
			CSharpExpressionFinder.stateTable = array;
		}
	}
}
