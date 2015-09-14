using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.VB
{
	internal sealed class Lexer : AbstractLexer
	{
		private bool lineEnd = true;

		public Lexer(TextReader reader) : base(reader)
		{
		}

		public override Token NextToken()
		{
			Token curToken;
			if (this.curToken == null)
			{
				this.curToken = this.Next();
				this.specialTracker.InformToken(this.curToken.kind);
				curToken = this.curToken;
			}
			else
			{
				this.lastToken = this.curToken;
				if (this.curToken.next == null)
				{
					this.curToken.next = this.Next();
					this.specialTracker.InformToken(this.curToken.next.kind);
				}
				this.curToken = this.curToken.next;
				if (this.curToken.kind == 0 && this.lastToken.kind != 1)
				{
					this.curToken = new Token(1, this.curToken.col, this.curToken.line, "\n");
					this.specialTracker.InformToken(this.curToken.kind);
					this.curToken.next = new Token(0, this.curToken.col, this.curToken.line, "\n");
					this.specialTracker.InformToken(this.curToken.next.kind);
				}
				curToken = this.curToken;
			}
			return curToken;
		}

		protected override Token Next()
		{
			int nextChar;
			Token result;
			while ((nextChar = base.ReaderRead()) != -1)
			{
				char ch = (char)nextChar;
				if (char.IsWhiteSpace(ch))
				{
					int x = base.Col - 1;
					int y = base.Line;
					if (base.HandleLineEnd(ch))
					{
						if (!this.lineEnd)
						{
							this.lineEnd = true;
							result = new Token(1, x, y);
							return result;
						}
						this.specialTracker.AddEndOfLine(new Location(x, y));
					}
					continue;
				}
				if (ch == '_')
				{
					if (base.ReaderPeek() == -1)
					{
						this.errors.Error(base.Line, base.Col, string.Format("No EOF expected after _", new object[0]));
						result = new Token(0);
					}
					else
					{
						if (char.IsWhiteSpace((char)base.ReaderPeek()))
						{
							ch = (char)base.ReaderRead();
							this.lineEnd = false;
							while (char.IsWhiteSpace(ch))
							{
								if (base.HandleLineEnd(ch))
								{
									this.lineEnd = true;
									break;
								}
								if (base.ReaderPeek() == -1)
								{
									this.errors.Error(base.Line, base.Col, string.Format("No EOF expected after _", new object[0]));
									result = new Token(0);
									return result;
								}
								ch = (char)base.ReaderRead();
							}
							if (!this.lineEnd)
							{
								this.errors.Error(base.Line, base.Col, string.Format("Return expected", new object[0]));
							}
							this.lineEnd = false;
							continue;
						}
						int x = base.Col - 1;
						int y = base.Line;
						string s = this.ReadIdent('_');
						this.lineEnd = false;
						result = new Token(2, x, y, s);
					}
				}
				else if (ch == '#')
				{
					while (char.IsWhiteSpace((char)base.ReaderPeek()))
					{
						base.ReaderRead();
					}
					if (!char.IsDigit((char)base.ReaderPeek()))
					{
						this.ReadPreprocessorDirective();
						continue;
					}
					int x = base.Col - 1;
					int y = base.Line;
					string s = this.ReadDate();
					DateTime time = new DateTime(1, 1, 1, 0, 0, 0);
					try
					{
						time = DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault);
					}
					catch (Exception e)
					{
						this.errors.Error(base.Line, base.Col, string.Format("Invalid date time {0}", e));
					}
					result = new Token(9, x, y, s, time);
				}
				else if (ch == '[')
				{
					this.lineEnd = false;
					if (base.ReaderPeek() == -1)
					{
						this.errors.Error(base.Line, base.Col, string.Format("Identifier expected", new object[0]));
					}
					ch = (char)base.ReaderRead();
					if (ch == ']' || char.IsWhiteSpace(ch))
					{
						this.errors.Error(base.Line, base.Col, string.Format("Identifier expected", new object[0]));
					}
					int x = base.Col - 1;
					int y = base.Line;
					string s = this.ReadIdent(ch);
					if (base.ReaderPeek() == -1)
					{
						this.errors.Error(base.Line, base.Col, string.Format("']' expected", new object[0]));
					}
					ch = (char)base.ReaderRead();
					if (ch != ']')
					{
						this.errors.Error(base.Line, base.Col, string.Format("']' expected", new object[0]));
					}
					result = new Token(2, x, y, s);
				}
				else if (char.IsLetter(ch))
				{
					int x = base.Col - 1;
					int y = base.Line;
					string s = this.ReadIdent(ch);
					int keyWordToken = Keywords.GetToken(s);
					if (keyWordToken >= 0)
					{
						this.lineEnd = false;
						result = new Token(keyWordToken, x, y, s);
					}
					else if (s.Equals("REM", StringComparison.InvariantCultureIgnoreCase))
					{
						this.ReadComment();
						if (this.lineEnd)
						{
							continue;
						}
						this.lineEnd = true;
						result = new Token(1, base.Col, base.Line, "\n");
					}
					else
					{
						this.lineEnd = false;
						result = new Token(2, x, y, s);
					}
				}
				else if (char.IsDigit(ch))
				{
					this.lineEnd = false;
					result = this.ReadDigit(ch, base.Col - 1);
				}
				else if (ch == '&')
				{
					this.lineEnd = false;
					if (base.ReaderPeek() == -1)
					{
						result = this.ReadOperator('&');
					}
					else
					{
						ch = (char)base.ReaderPeek();
						if (char.ToUpper(ch, CultureInfo.InvariantCulture) == 'H' || char.ToUpper(ch, CultureInfo.InvariantCulture) == 'O')
						{
							result = this.ReadDigit('&', base.Col - 1);
						}
						else
						{
							result = this.ReadOperator('&');
						}
					}
				}
				else if (ch == '\'' || ch == '‘' || ch == '’')
				{
					int x = base.Col - 1;
					int y = base.Line;
					this.ReadComment();
					if (this.lineEnd)
					{
						continue;
					}
					this.lineEnd = true;
					result = new Token(1, x, y, "\n");
				}
				else if (ch == '"')
				{
					this.lineEnd = false;
					int x = base.Col - 1;
					int y = base.Line;
					string s = this.ReadString();
					if (base.ReaderPeek() != -1 && (base.ReaderPeek() == 67 || base.ReaderPeek() == 99))
					{
						base.ReaderRead();
						if (s.Length != 1)
						{
							this.errors.Error(base.Line, base.Col, string.Format("Chars can only have Length 1 ", new object[0]));
						}
						if (s.Length == 0)
						{
							s = "\0";
						}
						result = new Token(4, x, y, '"' + s + "\"C", s[0]);
					}
					else
					{
						result = new Token(3, x, y, '"' + s + '"', s);
					}
				}
				else
				{
					Token token = this.ReadOperator(ch);
					if (token == null)
					{
						this.errors.Error(base.Line, base.Col, string.Format("Unknown char({0}) which can't be read", ch));
						continue;
					}
					this.lineEnd = false;
					result = token;
				}
				return result;
			}
			result = new Token(0);
			return result;
		}

		private string ReadIdent(char ch)
		{
			this.sb.Length = 0;
			this.sb.Append(ch);
			int peek;
			while ((peek = base.ReaderPeek()) != -1 && (char.IsLetterOrDigit(ch = (char)peek) || ch == '_'))
			{
				base.ReaderRead();
				this.sb.Append(ch.ToString());
			}
			string result;
			if (peek == -1)
			{
				result = this.sb.ToString();
			}
			else
			{
				if ("%&@!#$".IndexOf((char)peek) != -1)
				{
					base.ReaderRead();
				}
				result = this.sb.ToString();
			}
			return result;
		}

		private char PeekUpperChar()
		{
			return char.ToUpper((char)base.ReaderPeek(), CultureInfo.InvariantCulture);
		}

		private Token ReadDigit(char ch, int x)
		{
			this.sb.Length = 0;
			this.sb.Append(ch);
			int y = base.Line;
			string digit = "";
			if (ch != '&')
			{
				digit += ch;
			}
			bool ishex = false;
			bool isokt = false;
			bool issingle = false;
			bool isdouble = false;
			bool isdecimal = false;
			Token result;
			if (base.ReaderPeek() == -1)
			{
				if (ch == '&')
				{
					this.errors.Error(base.Line, base.Col, string.Format("digit expected", new object[0]));
				}
				result = new Token(5, x, y, this.sb.ToString(), (int)(ch - '0'));
			}
			else
			{
				if (ch == '.')
				{
					if (char.IsDigit((char)base.ReaderPeek()))
					{
						isdouble = true;
						if (ishex || isokt)
						{
							this.errors.Error(base.Line, base.Col, string.Format("No hexadecimal or oktadecimal floating point values allowed", new object[0]));
						}
						while (base.ReaderPeek() != -1 && char.IsDigit((char)base.ReaderPeek()))
						{
							digit += (char)base.ReaderRead();
						}
					}
				}
				else if (ch == '&' && this.PeekUpperChar() == 'H')
				{
					this.sb.Append((char)base.ReaderRead());
					while (base.ReaderPeek() != -1 && "0123456789ABCDEF".IndexOf(this.PeekUpperChar()) != -1)
					{
						ch = (char)base.ReaderRead();
						this.sb.Append(ch);
						digit += char.ToUpper(ch, CultureInfo.InvariantCulture);
					}
					ishex = true;
				}
				else if (base.ReaderPeek() != -1 && ch == '&' && this.PeekUpperChar() == 'O')
				{
					this.sb.Append((char)base.ReaderRead());
					while (base.ReaderPeek() != -1 && "01234567".IndexOf(this.PeekUpperChar()) != -1)
					{
						ch = (char)base.ReaderRead();
						this.sb.Append(ch);
						digit += char.ToUpper(ch, CultureInfo.InvariantCulture);
					}
					isokt = true;
				}
				else
				{
					while (base.ReaderPeek() != -1 && char.IsDigit((char)base.ReaderPeek()))
					{
						ch = (char)base.ReaderRead();
						digit += ch;
						this.sb.Append(ch);
					}
				}
				if (digit.Length == 0)
				{
					this.errors.Error(base.Line, base.Col, string.Format("digit expected", new object[0]));
					result = new Token(5, x, y, this.sb.ToString(), 0);
				}
				else
				{
					if (base.ReaderPeek() != -1 && ("%&SILU".IndexOf(this.PeekUpperChar()) != -1 || ishex || isokt))
					{
						ch = (char)base.ReaderPeek();
						this.sb.Append(ch);
						ch = char.ToUpper(ch, CultureInfo.InvariantCulture);
						bool unsigned = ch == 'U';
						if (unsigned)
						{
							base.ReaderRead();
							ch = (char)base.ReaderPeek();
							this.sb.Append(ch);
							ch = char.ToUpper(ch, CultureInfo.InvariantCulture);
							if (ch != 'I' && ch != 'L' && ch != 'S')
							{
								this.errors.Error(base.Line, base.Col, "Invalid type character: U" + ch);
							}
						}
						try
						{
							if (isokt)
							{
								base.ReaderRead();
								ulong number = 0uL;
								for (int i = 0; i < digit.Length; i++)
								{
									number = number * 8uL + (ulong)digit[i] - 48uL;
								}
								if (ch == 'S')
								{
									if (unsigned)
									{
										result = new Token(5, x, y, this.sb.ToString(), (ushort)number);
										return result;
									}
									result = new Token(5, x, y, this.sb.ToString(), (short)number);
									return result;
								}
								else if (ch == '%' || ch == 'I')
								{
									if (unsigned)
									{
										result = new Token(5, x, y, this.sb.ToString(), (uint)number);
										return result;
									}
									result = new Token(5, x, y, this.sb.ToString(), (int)number);
									return result;
								}
								else if (ch == '&' || ch == 'L')
								{
									if (unsigned)
									{
										result = new Token(5, x, y, this.sb.ToString(), number);
										return result;
									}
									result = new Token(5, x, y, this.sb.ToString(), (long)number);
									return result;
								}
								else
								{
									if (number > (ulong)-1)
									{
										result = new Token(5, x, y, this.sb.ToString(), (long)number);
										return result;
									}
									result = new Token(5, x, y, this.sb.ToString(), (int)number);
									return result;
								}
							}
							else if (ch == 'S')
							{
								base.ReaderRead();
								if (unsigned)
								{
									result = new Token(5, x, y, this.sb.ToString(), ushort.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
									return result;
								}
								result = new Token(5, x, y, this.sb.ToString(), short.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
								return result;
							}
							else if (ch == '%' || ch == 'I')
							{
								base.ReaderRead();
								if (unsigned)
								{
									result = new Token(5, x, y, this.sb.ToString(), uint.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
									return result;
								}
								result = new Token(5, x, y, this.sb.ToString(), int.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
								return result;
							}
							else if (ch == '&' || ch == 'L')
							{
								base.ReaderRead();
								if (unsigned)
								{
									result = new Token(5, x, y, this.sb.ToString(), ulong.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
									return result;
								}
								result = new Token(5, x, y, this.sb.ToString(), long.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
								return result;
							}
							else if (ishex)
							{
								ulong number = ulong.Parse(digit, NumberStyles.HexNumber);
								if (number > (ulong)-1)
								{
									result = new Token(5, x, y, this.sb.ToString(), (long)number);
									return result;
								}
								result = new Token(5, x, y, this.sb.ToString(), (int)number);
								return result;
							}
						}
						catch (OverflowException ex)
						{
							this.errors.Error(base.Line, base.Col, ex.Message);
							result = new Token(5, x, y, this.sb.ToString(), 0);
							return result;
						}
					}
					Token nextToken = null;
					if (!isdouble && base.ReaderPeek() == 46)
					{
						base.ReaderRead();
						if (base.ReaderPeek() != -1 && char.IsDigit((char)base.ReaderPeek()))
						{
							isdouble = true;
							if (ishex || isokt)
							{
								this.errors.Error(base.Line, base.Col, string.Format("No hexadecimal or oktadecimal floating point values allowed", new object[0]));
							}
							digit += '.';
							while (base.ReaderPeek() != -1 && char.IsDigit((char)base.ReaderPeek()))
							{
								digit += (char)base.ReaderRead();
							}
						}
						else
						{
							nextToken = new Token(10, base.Col - 1, base.Line);
						}
					}
					if (base.ReaderPeek() != -1 && this.PeekUpperChar() == 'E')
					{
						isdouble = true;
						digit += (char)base.ReaderRead();
						if (base.ReaderPeek() != -1 && (base.ReaderPeek() == 45 || base.ReaderPeek() == 43))
						{
							digit += (char)base.ReaderRead();
						}
						while (base.ReaderPeek() != -1 && char.IsDigit((char)base.ReaderPeek()))
						{
							digit += (char)base.ReaderRead();
						}
					}
					if (base.ReaderPeek() != -1)
					{
						char c = this.PeekUpperChar();
						if (c <= '@')
						{
							switch (c)
							{
							case '!':
								goto IL_A3B;
							case '"':
								goto IL_A47;
							case '#':
								break;
							default:
								if (c != '@')
								{
									goto IL_A47;
								}
								goto IL_A2F;
							}
						}
						else
						{
							switch (c)
							{
							case 'D':
								goto IL_A2F;
							case 'E':
								goto IL_A47;
							case 'F':
								goto IL_A3B;
							default:
								if (c != 'R')
								{
									goto IL_A47;
								}
								break;
							}
						}
						base.ReaderRead();
						isdouble = true;
						goto IL_A47;
						IL_A2F:
						base.ReaderRead();
						isdecimal = true;
						goto IL_A47;
						IL_A3B:
						base.ReaderRead();
						issingle = true;
						IL_A47:;
					}
					try
					{
						if (issingle)
						{
							result = new Token(7, x, y, this.sb.ToString(), float.Parse(digit, CultureInfo.InvariantCulture));
							return result;
						}
						if (isdecimal)
						{
							result = new Token(8, x, y, this.sb.ToString(), decimal.Parse(digit, NumberStyles.Any, CultureInfo.InvariantCulture));
							return result;
						}
						if (isdouble)
						{
							result = new Token(6, x, y, this.sb.ToString(), double.Parse(digit, CultureInfo.InvariantCulture));
							return result;
						}
					}
					catch (FormatException)
					{
						this.errors.Error(base.Line, base.Col, string.Format("{0} is not a parseable number", digit));
						if (issingle)
						{
							result = new Token(7, x, y, this.sb.ToString(), 0f);
							return result;
						}
						if (isdecimal)
						{
							result = new Token(8, x, y, this.sb.ToString(), 0m);
							return result;
						}
						if (isdouble)
						{
							result = new Token(6, x, y, this.sb.ToString(), 0.0);
							return result;
						}
					}
					Token token;
					try
					{
						token = new Token(5, x, y, this.sb.ToString(), int.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
					}
					catch (Exception)
					{
						try
						{
							token = new Token(5, x, y, this.sb.ToString(), long.Parse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number));
						}
						catch (FormatException)
						{
							this.errors.Error(base.Line, base.Col, string.Format("{0} is not a parseable number", digit));
							token = new Token(5, x, y, this.sb.ToString(), 0);
						}
						catch (OverflowException)
						{
							this.errors.Error(base.Line, base.Col, string.Format("{0} is too long for a integer literal", digit));
							token = new Token(5, x, y, this.sb.ToString(), 0);
						}
					}
					token.next = nextToken;
					result = token;
				}
			}
			return result;
		}

		private void ReadPreprocessorDirective()
		{
			Location start = new Location(base.Col - 1, base.Line);
			string directive = this.ReadIdent('#');
			string argument = base.ReadToEndOfLine();
			this.specialTracker.AddPreprocessingDirective(directive, argument.Trim(), start, new Location(start.X + directive.Length + argument.Length, start.Y));
		}

		private string ReadDate()
		{
			char ch = '\0';
			this.sb.Length = 0;
			int nextChar;
			while ((nextChar = base.ReaderRead()) != -1)
			{
				ch = (char)nextChar;
				if (ch == '#')
				{
					break;
				}
				if (ch == '\n')
				{
					this.errors.Error(base.Line, base.Col, string.Format("No return allowed inside Date literal", new object[0]));
				}
				else
				{
					this.sb.Append(ch);
				}
			}
			if (ch != '#')
			{
				this.errors.Error(base.Line, base.Col, string.Format("End of File reached before Date literal terminated", new object[0]));
			}
			return this.sb.ToString();
		}

		private string ReadString()
		{
			char ch = '\0';
			this.sb.Length = 0;
			int nextChar;
			while ((nextChar = base.ReaderRead()) != -1)
			{
				ch = (char)nextChar;
				if (ch == '"')
				{
					if (base.ReaderPeek() == -1 || base.ReaderPeek() != 34)
					{
						break;
					}
					this.sb.Append('"');
					base.ReaderRead();
				}
				else if (ch == '\n')
				{
					this.errors.Error(base.Line, base.Col, string.Format("No return allowed inside String literal", new object[0]));
				}
				else
				{
					this.sb.Append(ch);
				}
			}
			if (ch != '"')
			{
				this.errors.Error(base.Line, base.Col, string.Format("End of File reached before String terminated ", new object[0]));
			}
			return this.sb.ToString();
		}

		private void ReadComment()
		{
			Location startPos = new Location(base.Col, base.Line);
			this.sb.Length = 0;
			StringBuilder curWord = (this.specialCommentHash != null) ? new StringBuilder() : null;
			int missingApostrophes = 2;
			int nextChar;
			while ((nextChar = base.ReaderRead()) != -1)
			{
				char ch = (char)nextChar;
				if (base.HandleLineEnd(ch))
				{
					break;
				}
				this.sb.Append(ch);
				if (missingApostrophes > 0)
				{
					if (ch == '\'' || ch == '‘' || ch == '’')
					{
						if (--missingApostrophes == 0)
						{
							this.specialTracker.StartComment(CommentType.Documentation, startPos);
							this.sb.Length = 0;
						}
					}
					else
					{
						this.specialTracker.StartComment(CommentType.SingleLine, startPos);
						missingApostrophes = 0;
					}
				}
				if (this.specialCommentHash != null)
				{
					if (char.IsLetter(ch))
					{
						curWord.Append(ch);
					}
					else
					{
						string tag = curWord.ToString();
						curWord.Length = 0;
						if (this.specialCommentHash.ContainsKey(tag))
						{
							Location p = new Location(base.Col, base.Line);
							string comment = ch + base.ReadToEndOfLine();
							base.TagComments.Add(new TagComment(tag, comment, p, new Location(base.Col, base.Line)));
							this.sb.Append(comment);
							break;
						}
					}
				}
			}
			if (missingApostrophes > 0)
			{
				this.specialTracker.StartComment(CommentType.SingleLine, startPos);
			}
			this.specialTracker.AddString(this.sb.ToString());
			this.specialTracker.FinishComment(new Location(base.Col, base.Line));
		}

		private Token ReadOperator(char ch)
		{
			int x = base.Col - 1;
			int y = base.Line;
			Token result;
			switch (ch)
			{
			case '&':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(19, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(41, x, y);
				return result;
			}
			case '\'':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case ';':
				break;
			case '(':
				result = new Token(24, x, y);
				return result;
			case ')':
				result = new Token(25, x, y);
				return result;
			case '*':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(16, x, y, "*");
					return result;
				}
				base.ReaderRead();
				result = new Token(36, x, y);
				return result;
			}
			case '+':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(14, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(33, x, y);
				return result;
			}
			case ',':
				result = new Token(12, x, y);
				return result;
			case '-':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(15, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(35, x, y);
				return result;
			}
			case '.':
			{
				int tmp = base.ReaderPeek();
				if (tmp > 0 && char.IsDigit((char)tmp))
				{
					result = this.ReadDigit('.', base.Col);
					return result;
				}
				result = new Token(10, x, y);
				return result;
			}
			case '/':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(17, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(37, x, y);
				return result;
			}
			case ':':
				result = new Token(13, x, y);
				return result;
			case '<':
				switch (base.ReaderPeek())
				{
				case 60:
				{
					base.ReaderRead();
					int num = base.ReaderPeek();
					if (num != 61)
					{
						result = new Token(31, x, y);
						return result;
					}
					base.ReaderRead();
					result = new Token(39, x, y);
					return result;
				}
				case 61:
					base.ReaderRead();
					result = new Token(30, x, y);
					return result;
				case 62:
					base.ReaderRead();
					result = new Token(28, x, y);
					return result;
				default:
					result = new Token(27, x, y);
					return result;
				}
				break;
			case '=':
				result = new Token(11, x, y);
				return result;
			case '>':
				switch (base.ReaderPeek())
				{
				case 61:
					base.ReaderRead();
					result = new Token(29, x, y);
					return result;
				case 62:
					base.ReaderRead();
					if (base.ReaderPeek() != -1)
					{
						int num = base.ReaderPeek();
						if (num == 61)
						{
							base.ReaderRead();
							result = new Token(40, x, y);
							return result;
						}
					}
					result = new Token(32, x, y);
					return result;
				default:
					result = new Token(26, x, y);
					return result;
				}
				break;
			case '?':
				result = new Token(21, x, y);
				return result;
			default:
				switch (ch)
				{
				case '\\':
				{
					int num = base.ReaderPeek();
					if (num != 61)
					{
						result = new Token(18, x, y);
						return result;
					}
					base.ReaderRead();
					result = new Token(38, x, y);
					return result;
				}
				case ']':
					break;
				case '^':
				{
					int num = base.ReaderPeek();
					if (num != 61)
					{
						result = new Token(20, x, y);
						return result;
					}
					base.ReaderRead();
					result = new Token(34, x, y);
					return result;
				}
				default:
					switch (ch)
					{
					case '{':
						result = new Token(22, x, y);
						return result;
					case '}':
						result = new Token(23, x, y);
						return result;
					}
					break;
				}
				break;
			}
			result = null;
			return result;
		}

		public override void SkipCurrentBlock(int targetToken)
		{
			int lastKind = -1;
			int kind = this.lastToken.kind;
			while (kind != 0 && (lastKind != 88 || kind != targetToken))
			{
				lastKind = kind;
				this.NextToken();
				kind = this.lastToken.kind;
			}
		}
	}
}
