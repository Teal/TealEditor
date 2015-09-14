using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.CSharp
{
	internal sealed class Lexer : AbstractLexer
	{
		private const int MAX_IDENTIFIER_LENGTH = 512;

		private char[] identBuffer = new char[512];

		private char[] escapeSequenceBuffer = new char[12];

		public Lexer(TextReader reader) : base(reader)
		{
		}

		private void ReadPreProcessingDirective()
		{
			Location start = new Location(base.Col - 1, base.Line);
			bool canBeKeyword;
			string directive = this.ReadIdent('#', out canBeKeyword);
			string argument = base.ReadToEndOfLine();
			this.specialTracker.AddPreprocessingDirective(directive, argument.Trim(), start, new Location(start.X + directive.Length + argument.Length, start.Y));
		}

		protected override Token Next()
		{
			bool hadLineEnd = false;
			if (base.Line == 1 && base.Col == 1)
			{
				hadLineEnd = true;
			}
			int nextChar;
			Token result;
			while ((nextChar = base.ReaderRead()) != -1)
			{
				int num = nextChar;
				Token token;
				if (num <= 35)
				{
					switch (num)
					{
					case 9:
						break;
					case 10:
					case 13:
						if (hadLineEnd)
						{
							this.specialTracker.AddEndOfLine(new Location(base.Col, base.Line));
						}
						base.HandleLineEnd((char)nextChar);
						hadLineEnd = true;
						break;
					case 11:
					case 12:
						goto IL_208;
					default:
						switch (num)
						{
						case 32:
							break;
						case 33:
							goto IL_208;
						case 34:
							token = this.ReadString();
							goto IL_2BA;
						case 35:
							this.ReadPreProcessingDirective();
							break;
						default:
							goto IL_208;
						}
						break;
					}
					continue;
				}
				int x;
				int y;
				char ch;
				bool canBeKeyword;
				if (num != 39)
				{
					if (num != 47)
					{
						if (num != 64)
						{
							goto IL_208;
						}
						int next = base.ReaderRead();
						if (next == -1)
						{
							this.errors.Error(base.Line, base.Col, string.Format("EOF after @", new object[0]));
							continue;
						}
						x = base.Col - 1;
						y = base.Line;
						ch = (char)next;
						if (ch == '"')
						{
							token = this.ReadVerbatimString();
						}
						else
						{
							if (!char.IsLetterOrDigit(ch) && ch != '_')
							{
								this.errors.Error(y, x, string.Format("Unexpected char in Lexer.Next() : {0}", ch));
								continue;
							}
							token = new Token(1, x - 1, y, this.ReadIdent(ch, out canBeKeyword));
						}
					}
					else
					{
						int peek = base.ReaderPeek();
						if (peek == 47 || peek == 42)
						{
							this.ReadComment();
							continue;
						}
						token = this.ReadOperator('/');
					}
				}
				else
				{
					token = this.ReadChar();
				}
				IL_2BA:
				if (token != null)
				{
					result = token;
					return result;
				}
				continue;
				IL_208:
				ch = (char)nextChar;
				if (!char.IsLetter(ch) && ch != '_' && ch != '\\')
				{
					if (char.IsDigit(ch))
					{
						token = this.ReadDigit(ch, base.Col - 1);
					}
					else
					{
						token = this.ReadOperator(ch);
					}
					goto IL_2BA;
				}
				x = base.Col - 1;
				y = base.Line;
				string s = this.ReadIdent(ch, out canBeKeyword);
				if (canBeKeyword)
				{
					int keyWordToken = Keywords.GetToken(s);
					if (keyWordToken >= 0)
					{
						result = new Token(keyWordToken, x, y);
						return result;
					}
				}
				result = new Token(1, x, y, s);
				return result;
			}
			result = new Token(0, base.Col, base.Line, string.Empty);
			return result;
		}

		private string ReadIdent(char ch, out bool canBeKeyword)
		{
			int curPos = 0;
			canBeKeyword = true;
			while (true)
			{
				int peek;
				if (ch == '\\')
				{
					peek = base.ReaderPeek();
					if (peek != 117 && peek != 85)
					{
						this.errors.Error(base.Line, base.Col, "Identifiers can only contain unicode escape sequences");
					}
					canBeKeyword = false;
					string surrogatePair;
					this.ReadEscapeSequence(out ch, out surrogatePair);
					if (surrogatePair != null)
					{
						if (!char.IsLetterOrDigit(surrogatePair, 0))
						{
							this.errors.Error(base.Line, base.Col, "Unicode escape sequences in identifiers cannot be used to represent characters that are invalid in identifiers");
						}
						for (int i = 0; i < surrogatePair.Length - 1; i++)
						{
							if (curPos < 512)
							{
								this.identBuffer[curPos++] = surrogatePair[i];
							}
						}
						ch = surrogatePair[surrogatePair.Length - 1];
					}
					else if (!AbstractLexer.IsIdentifierPart((int)ch))
					{
						this.errors.Error(base.Line, base.Col, "Unicode escape sequences in identifiers cannot be used to represent characters that are invalid in identifiers");
					}
				}
				if (curPos >= 512)
				{
					break;
				}
				this.identBuffer[curPos++] = ch;
				peek = base.ReaderPeek();
				if (!AbstractLexer.IsIdentifierPart(peek) && peek != 92)
				{
					goto IL_1CB;
				}
				ch = (char)base.ReaderRead();
			}
			this.errors.Error(base.Line, base.Col, string.Format("Identifier too long", new object[0]));
			while (AbstractLexer.IsIdentifierPart(base.ReaderPeek()))
			{
				base.ReaderRead();
			}
			IL_1CB:
			return new string(this.identBuffer, 0, curPos);
		}

		private Token ReadDigit(char ch, int x)
		{
			int y = base.Line;
			this.sb.Length = 0;
			this.sb.Append(ch);
			string prefix = null;
			string suffix = null;
			bool ishex = false;
			bool isunsigned = false;
			bool islong = false;
			bool isfloat = false;
			bool isdouble = false;
			bool isdecimal = false;
			char peek = (char)base.ReaderPeek();
			if (ch == '.')
			{
				isdouble = true;
				while (char.IsDigit((char)base.ReaderPeek()))
				{
					this.sb.Append((char)base.ReaderRead());
				}
				peek = (char)base.ReaderPeek();
			}
			else if (ch == '0' && (peek == 'x' || peek == 'X'))
			{
				base.ReaderRead();
				this.sb.Length = 0;
				while (AbstractLexer.IsHex((char)base.ReaderPeek()))
				{
					this.sb.Append((char)base.ReaderRead());
				}
				if (this.sb.Length == 0)
				{
					this.sb.Append('0');
					this.errors.Error(y, x, "Invalid hexadecimal integer literal");
				}
				ishex = true;
				prefix = "0x";
				peek = (char)base.ReaderPeek();
			}
			else
			{
				while (char.IsDigit((char)base.ReaderPeek()))
				{
					this.sb.Append((char)base.ReaderRead());
				}
				peek = (char)base.ReaderPeek();
			}
			Token nextToken = null;
			if (peek == '.')
			{
				base.ReaderRead();
				peek = (char)base.ReaderPeek();
				if (!char.IsDigit(peek))
				{
					nextToken = new Token(15, base.Col - 1, base.Line);
					peek = '.';
				}
				else
				{
					isdouble = true;
					if (ishex)
					{
						this.errors.Error(y, x, string.Format("No hexadecimal floating point values allowed", new object[0]));
					}
					this.sb.Append('.');
					while (char.IsDigit((char)base.ReaderPeek()))
					{
						this.sb.Append((char)base.ReaderRead());
					}
					peek = (char)base.ReaderPeek();
				}
			}
			if (peek == 'e' || peek == 'E')
			{
				isdouble = true;
				this.sb.Append((char)base.ReaderRead());
				peek = (char)base.ReaderPeek();
				if (peek == '-' || peek == '+')
				{
					this.sb.Append((char)base.ReaderRead());
				}
				while (char.IsDigit((char)base.ReaderPeek()))
				{
					this.sb.Append((char)base.ReaderRead());
				}
				isunsigned = true;
				peek = (char)base.ReaderPeek();
			}
			if (peek == 'f' || peek == 'F')
			{
				base.ReaderRead();
				suffix = "f";
				isfloat = true;
			}
			else if (peek == 'd' || peek == 'D')
			{
				base.ReaderRead();
				suffix = "d";
				isdouble = true;
			}
			else if (peek == 'm' || peek == 'M')
			{
				base.ReaderRead();
				suffix = "m";
				isdecimal = true;
			}
			else if (!isdouble)
			{
				if (peek == 'u' || peek == 'U')
				{
					base.ReaderRead();
					suffix = "u";
					isunsigned = true;
					peek = (char)base.ReaderPeek();
				}
				if (peek == 'l' || peek == 'L')
				{
					base.ReaderRead();
					peek = (char)base.ReaderPeek();
					islong = true;
					if (!isunsigned && (peek == 'u' || peek == 'U'))
					{
						base.ReaderRead();
						suffix = "lu";
						isunsigned = true;
					}
					else
					{
						suffix = (isunsigned ? "ul" : "l");
					}
				}
			}
			string digit = this.sb.ToString();
			string stringValue = prefix + digit + suffix;
			Token result2;
			if (isfloat)
			{
				float num;
				if (float.TryParse(digit, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
				{
					result2 = new Token(2, x, y, stringValue, num);
				}
				else
				{
					this.errors.Error(y, x, string.Format("Can't parse float {0}", digit));
					result2 = new Token(2, x, y, stringValue, 0f);
				}
			}
			else if (isdecimal)
			{
				decimal num2;
				if (decimal.TryParse(digit, NumberStyles.Any, CultureInfo.InvariantCulture, out num2))
				{
					result2 = new Token(2, x, y, stringValue, num2);
				}
				else
				{
					this.errors.Error(y, x, string.Format("Can't parse decimal {0}", digit));
					result2 = new Token(2, x, y, stringValue, 0m);
				}
			}
			else if (isdouble)
			{
				double num3;
				if (double.TryParse(digit, NumberStyles.Any, CultureInfo.InvariantCulture, out num3))
				{
					result2 = new Token(2, x, y, stringValue, num3);
				}
				else
				{
					this.errors.Error(y, x, string.Format("Can't parse double {0}", digit));
					result2 = new Token(2, x, y, stringValue, 0.0);
				}
			}
			else
			{
				ulong result;
				if (ishex)
				{
					if (!ulong.TryParse(digit, NumberStyles.HexNumber, null, out result))
					{
						this.errors.Error(y, x, string.Format("Can't parse hexadecimal constant {0}", digit));
						result2 = new Token(2, x, y, stringValue.ToString(), 0);
						return result2;
					}
				}
				else if (!ulong.TryParse(digit, NumberStyles.Integer, null, out result))
				{
					this.errors.Error(y, x, string.Format("Can't parse integral constant {0}", digit));
					result2 = new Token(2, x, y, stringValue.ToString(), 0);
					return result2;
				}
				if (result > 9223372036854775807uL)
				{
					islong = true;
					isunsigned = true;
				}
				else if (result > (ulong)-1)
				{
					islong = true;
				}
				else if (result > 2147483647uL)
				{
					isunsigned = true;
				}
				Token token;
				int num7;
				if (islong)
				{
					long num5;
					if (isunsigned)
					{
						ulong num4;
						if (ulong.TryParse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number, CultureInfo.InvariantCulture, out num4))
						{
							token = new Token(2, x, y, stringValue, num4);
						}
						else
						{
							this.errors.Error(y, x, string.Format("Can't parse unsigned long {0}", digit));
							token = new Token(2, x, y, stringValue, 0uL);
						}
					}
					else if (long.TryParse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number, CultureInfo.InvariantCulture, out num5))
					{
						token = new Token(2, x, y, stringValue, num5);
					}
					else
					{
						this.errors.Error(y, x, string.Format("Can't parse long {0}", digit));
						token = new Token(2, x, y, stringValue, 0L);
					}
				}
				else if (isunsigned)
				{
					uint num6;
					if (uint.TryParse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number, CultureInfo.InvariantCulture, out num6))
					{
						token = new Token(2, x, y, stringValue, num6);
					}
					else
					{
						this.errors.Error(y, x, string.Format("Can't parse unsigned int {0}", digit));
						token = new Token(2, x, y, stringValue, 0u);
					}
				}
				else if (int.TryParse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number, CultureInfo.InvariantCulture, out num7))
				{
					token = new Token(2, x, y, stringValue, num7);
				}
				else
				{
					this.errors.Error(y, x, string.Format("Can't parse int {0}", digit));
					token = new Token(2, x, y, stringValue, 0);
				}
				token.next = nextToken;
				result2 = token;
			}
			return result2;
		}

		private Token ReadString()
		{
			int x = base.Col - 1;
			int y = base.Line;
			this.sb.Length = 0;
			this.originalValue.Length = 0;
			this.originalValue.Append('"');
			bool doneNormally = false;
			int nextChar;
			while ((nextChar = base.ReaderRead()) != -1)
			{
				char ch = (char)nextChar;
				if (ch == '"')
				{
					doneNormally = true;
					this.originalValue.Append('"');
					break;
				}
				if (ch == '\\')
				{
					this.originalValue.Append('\\');
					string surrogatePair;
					this.originalValue.Append(this.ReadEscapeSequence(out ch, out surrogatePair));
					if (surrogatePair != null)
					{
						this.sb.Append(surrogatePair);
					}
					else
					{
						this.sb.Append(ch);
					}
				}
				else
				{
					if (ch == '\n')
					{
						this.errors.Error(y, x, string.Format("No new line is allowed inside a string literal", new object[0]));
						break;
					}
					this.originalValue.Append(ch);
					this.sb.Append(ch);
				}
			}
			if (!doneNormally)
			{
				this.errors.Error(y, x, string.Format("End of file reached inside string literal", new object[0]));
			}
			return new Token(2, x, y, this.originalValue.ToString(), this.sb.ToString());
		}

		private Token ReadVerbatimString()
		{
			this.sb.Length = 0;
			this.originalValue.Length = 0;
			this.originalValue.Append("@\"");
			int x = base.Col - 2;
			int y = base.Line;
			int nextChar;
			while ((nextChar = base.ReaderRead()) != -1)
			{
				char ch = (char)nextChar;
				if (ch == '"')
				{
					if (base.ReaderPeek() != 34)
					{
						this.originalValue.Append('"');
						break;
					}
					this.originalValue.Append("\"\"");
					this.sb.Append('"');
					base.ReaderRead();
				}
				else if (base.HandleLineEnd(ch))
				{
					this.sb.Append("\r\n");
					this.originalValue.Append("\r\n");
				}
				else
				{
					this.sb.Append(ch);
					this.originalValue.Append(ch);
				}
			}
			if (nextChar == -1)
			{
				this.errors.Error(y, x, string.Format("End of file reached inside verbatim string literal", new object[0]));
			}
			return new Token(2, x, y, this.originalValue.ToString(), this.sb.ToString());
		}

		private string ReadEscapeSequence(out char ch, out string surrogatePair)
		{
			surrogatePair = null;
			int nextChar = base.ReaderRead();
			string result;
			if (nextChar == -1)
			{
				this.errors.Error(base.Line, base.Col, string.Format("End of file reached inside escape sequence", new object[0]));
				ch = '\0';
				result = string.Empty;
			}
			else
			{
				char c = (char)nextChar;
				int curPos = 1;
				this.escapeSequenceBuffer[0] = c;
				char c2 = c;
				if (c2 <= 'U')
				{
					if (c2 <= '\'')
					{
						if (c2 == '"')
						{
							ch = '"';
							goto IL_310;
						}
						if (c2 == '\'')
						{
							ch = '\'';
							goto IL_310;
						}
					}
					else
					{
						if (c2 == '0')
						{
							ch = '\0';
							goto IL_310;
						}
						if (c2 == 'U')
						{
							int number = 0;
							for (int i = 0; i < 8; i++)
							{
								if (!AbstractLexer.IsHex((char)base.ReaderPeek()))
								{
									this.errors.Error(base.Line, base.Col - 1, string.Format("Invalid char in literal : {0}", (char)base.ReaderPeek()));
									break;
								}
								c = (char)base.ReaderRead();
								int idx = base.GetHexNumber(c);
								this.escapeSequenceBuffer[curPos++] = c;
								number = 16 * number + idx;
							}
							if (number > 65535)
							{
								ch = '\0';
								surrogatePair = char.ConvertFromUtf32(number);
							}
							else
							{
								ch = (char)number;
							}
							goto IL_310;
						}
					}
				}
				else if (c2 <= 'b')
				{
					if (c2 == '\\')
					{
						ch = '\\';
						goto IL_310;
					}
					switch (c2)
					{
					case 'a':
						ch = '\a';
						goto IL_310;
					case 'b':
						ch = '\b';
						goto IL_310;
					}
				}
				else
				{
					if (c2 == 'f')
					{
						ch = '\f';
						goto IL_310;
					}
					switch (c2)
					{
					case 'n':
						ch = '\n';
						goto IL_310;
					case 'r':
						ch = '\r';
						goto IL_310;
					case 't':
						ch = '\t';
						goto IL_310;
					case 'u':
					case 'x':
					{
						c = (char)base.ReaderRead();
						int number = base.GetHexNumber(c);
						this.escapeSequenceBuffer[curPos++] = c;
						if (number < 0)
						{
							this.errors.Error(base.Line, base.Col - 1, string.Format("Invalid char in literal : {0}", c));
						}
						for (int i = 0; i < 3; i++)
						{
							if (!AbstractLexer.IsHex((char)base.ReaderPeek()))
							{
								break;
							}
							c = (char)base.ReaderRead();
							int idx = base.GetHexNumber(c);
							this.escapeSequenceBuffer[curPos++] = c;
							number = 16 * number + idx;
						}
						ch = (char)number;
						goto IL_310;
					}
					case 'v':
						ch = '\v';
						goto IL_310;
					}
				}
				this.errors.Error(base.Line, base.Col, string.Format("Unexpected escape sequence : {0}", c));
				ch = '\0';
				IL_310:
				result = new string(this.escapeSequenceBuffer, 0, curPos);
			}
			return result;
		}

		private Token ReadChar()
		{
			int x = base.Col - 1;
			int y = base.Line;
			int nextChar = base.ReaderRead();
			Token result;
			if (nextChar == -1)
			{
				this.errors.Error(y, x, string.Format("End of file reached inside character literal", new object[0]));
				result = null;
			}
			else
			{
				char ch = (char)nextChar;
				char chValue = ch;
				string escapeSequence = string.Empty;
				if (ch == '\\')
				{
					string surrogatePair;
					escapeSequence = this.ReadEscapeSequence(out chValue, out surrogatePair);
					if (surrogatePair != null)
					{
						this.errors.Error(y, x, string.Format("The unicode character must be represented by a surrogate pair and does not fit into a System.Char", new object[0]));
					}
				}
				if ((ushort)base.ReaderRead() != 39)
				{
					this.errors.Error(y, x, string.Format("Char not terminated", new object[0]));
				}
				result = new Token(2, x, y, string.Concat(new object[]
				{
					"'",
					ch,
					escapeSequence,
					"'"
				}), chValue);
			}
			return result;
		}

		private Token ReadOperator(char ch)
		{
			int x = base.Col - 1;
			int y = base.Line;
			Token result;
			switch (ch)
			{
			case '!':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(24, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(34, x, y);
				return result;
			}
			case '"':
			case '#':
			case '$':
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
				break;
			case '%':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(8, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(42, x, y);
				return result;
			}
			case '&':
			{
				int num = base.ReaderPeek();
				if (num == 38)
				{
					base.ReaderRead();
					result = new Token(25, x, y);
					return result;
				}
				if (num != 61)
				{
					result = new Token(28, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(43, x, y);
				return result;
			}
			case '(':
				result = new Token(20, x, y);
				return result;
			case ')':
				result = new Token(21, x, y);
				return result;
			case '*':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(6, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(40, x, y);
				return result;
			}
			case '+':
			{
				int num = base.ReaderPeek();
				if (num == 43)
				{
					base.ReaderRead();
					result = new Token(31, x, y);
					return result;
				}
				if (num != 61)
				{
					result = new Token(4, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(38, x, y);
				return result;
			}
			case ',':
				result = new Token(14, x, y);
				return result;
			case '-':
			{
				int num = base.ReaderPeek();
				if (num == 45)
				{
					base.ReaderRead();
					result = new Token(32, x, y);
					return result;
				}
				switch (num)
				{
				case 61:
					base.ReaderRead();
					result = new Token(39, x, y);
					return result;
				case 62:
					base.ReaderRead();
					result = new Token(47, x, y);
					return result;
				default:
					result = new Token(5, x, y);
					return result;
				}
				break;
			}
			case '.':
			{
				int tmp = base.ReaderPeek();
				if (tmp > 0 && char.IsDigit((char)tmp))
				{
					result = this.ReadDigit('.', base.Col - 1);
					return result;
				}
				result = new Token(15, x, y);
				return result;
			}
			case '/':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(7, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(41, x, y);
				return result;
			}
			case ':':
				if (base.ReaderPeek() == 58)
				{
					base.ReaderRead();
					result = new Token(10, x, y);
					return result;
				}
				result = new Token(9, x, y);
				return result;
			case ';':
				result = new Token(11, x, y);
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
						result = new Token(37, x, y);
						return result;
					}
					base.ReaderRead();
					result = new Token(46, x, y);
					return result;
				}
				case 61:
					base.ReaderRead();
					result = new Token(36, x, y);
					return result;
				default:
					result = new Token(23, x, y);
					return result;
				}
				break;
			case '=':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(3, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(33, x, y);
				return result;
			}
			case '>':
			{
				int num = base.ReaderPeek();
				if (num != 61)
				{
					result = new Token(22, x, y);
					return result;
				}
				base.ReaderRead();
				result = new Token(35, x, y);
				return result;
			}
			case '?':
				if (base.ReaderPeek() == 63)
				{
					base.ReaderRead();
					result = new Token(13, x, y);
					return result;
				}
				result = new Token(12, x, y);
				return result;
			default:
				switch (ch)
				{
				case '[':
					result = new Token(18, x, y);
					return result;
				case '\\':
					break;
				case ']':
					result = new Token(19, x, y);
					return result;
				case '^':
				{
					int num = base.ReaderPeek();
					if (num != 61)
					{
						result = new Token(30, x, y);
						return result;
					}
					base.ReaderRead();
					result = new Token(45, x, y);
					return result;
				}
				default:
					switch (ch)
					{
					case '{':
						result = new Token(16, x, y);
						return result;
					case '|':
					{
						int num = base.ReaderPeek();
						if (num == 61)
						{
							base.ReaderRead();
							result = new Token(44, x, y);
							return result;
						}
						if (num != 124)
						{
							result = new Token(29, x, y);
							return result;
						}
						base.ReaderRead();
						result = new Token(26, x, y);
						return result;
					}
					case '}':
						result = new Token(17, x, y);
						return result;
					case '~':
						result = new Token(27, x, y);
						return result;
					}
					break;
				}
				break;
			}
			result = null;
			return result;
		}

		private void ReadComment()
		{
			int num = base.ReaderRead();
			if (num != 42)
			{
				if (num != 47)
				{
					this.errors.Error(base.Line, base.Col, string.Format("Error while reading comment", new object[0]));
				}
				else if (base.ReaderPeek() == 47)
				{
					base.ReaderRead();
					this.ReadSingleLineComment(CommentType.Documentation);
				}
				else
				{
					this.ReadSingleLineComment(CommentType.SingleLine);
				}
			}
			else
			{
				this.ReadMultiLineComment();
			}
		}

		private string ReadCommentToEOL()
		{
			string result;
			if (this.specialCommentHash == null)
			{
				result = base.ReadToEndOfLine();
			}
			else
			{
				this.sb.Length = 0;
				StringBuilder curWord = new StringBuilder();
				int nextChar;
				while ((nextChar = base.ReaderRead()) != -1)
				{
					char ch = (char)nextChar;
					if (base.HandleLineEnd(ch))
					{
						break;
					}
					this.sb.Append(ch);
					if (AbstractLexer.IsIdentifierPart(nextChar))
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
				result = this.sb.ToString();
			}
			return result;
		}

		private void ReadSingleLineComment(CommentType commentType)
		{
			if (base.SkipAllComments)
			{
				base.SkipToEndOfLine();
			}
			else
			{
				this.specialTracker.StartComment(commentType, new Location(base.Col, base.Line));
				this.specialTracker.AddString(this.ReadCommentToEOL());
				this.specialTracker.FinishComment(new Location(base.Col, base.Line));
			}
		}

		private void ReadMultiLineComment()
		{
			if (base.SkipAllComments)
			{
				int nextChar;
				while ((nextChar = base.ReaderRead()) != -1)
				{
					char ch = (char)nextChar;
					if (ch == '*' && base.ReaderPeek() == 47)
					{
						base.ReaderRead();
						return;
					}
					base.HandleLineEnd(ch);
				}
			}
			else
			{
				this.specialTracker.StartComment(CommentType.Block, new Location(base.Col, base.Line));
				string scTag = null;
				StringBuilder scCurWord = new StringBuilder();
				Location scStartLocation = Location.Empty;
				int nextChar;
				while ((nextChar = base.ReaderRead()) != -1)
				{
					char ch = (char)nextChar;
					if (base.HandleLineEnd(ch))
					{
						if (scTag != null)
						{
							base.TagComments.Add(new TagComment(scTag, scCurWord.ToString(), scStartLocation, new Location(base.Col, base.Line)));
							scTag = null;
						}
						scCurWord.Length = 0;
						this.specialTracker.AddString(Environment.NewLine);
					}
					else
					{
						if (ch == '*' && base.ReaderPeek() == 47)
						{
							if (scTag != null)
							{
								base.TagComments.Add(new TagComment(scTag, scCurWord.ToString(), scStartLocation, new Location(base.Col, base.Line)));
							}
							base.ReaderRead();
							this.specialTracker.FinishComment(new Location(base.Col, base.Line));
							return;
						}
						this.specialTracker.AddChar(ch);
						if (scTag != null || AbstractLexer.IsIdentifierPart((int)ch))
						{
							scCurWord.Append(ch);
						}
						else
						{
							if (this.specialCommentHash != null && this.specialCommentHash.ContainsKey(scCurWord.ToString()))
							{
								scTag = scCurWord.ToString();
								scStartLocation = new Location(base.Col, base.Line);
							}
							scCurWord.Length = 0;
						}
					}
				}
				this.specialTracker.FinishComment(new Location(base.Col, base.Line));
			}
			this.errors.Error(base.Line, base.Col, string.Format("Reached EOF before the end of a multiline comment", new object[0]));
		}

		public override void SkipCurrentBlock(int targetToken)
		{
			int braceCount = 0;
			while (this.curToken != null)
			{
				if (this.curToken.kind == 16)
				{
					braceCount++;
				}
				else if (this.curToken.kind == 17)
				{
					if (--braceCount < 0)
					{
						return;
					}
				}
				this.lastToken = this.curToken;
				this.curToken = this.curToken.next;
			}
			int nextChar;
			while ((nextChar = base.ReaderRead()) != -1)
			{
				int num = nextChar;
				if (num <= 35)
				{
					if (num != 10 && num != 13)
					{
						switch (num)
						{
						case 34:
							this.ReadString();
							break;
						case 35:
							this.ReadPreProcessingDirective();
							break;
						}
					}
					else
					{
						base.HandleLineEnd((char)nextChar);
					}
				}
				else if (num <= 47)
				{
					if (num != 39)
					{
						if (num == 47)
						{
							int peek = base.ReaderPeek();
							if (peek == 47 || peek == 42)
							{
								this.ReadComment();
							}
						}
					}
					else
					{
						this.ReadChar();
					}
				}
				else if (num != 64)
				{
					switch (num)
					{
					case 123:
						braceCount++;
						break;
					case 125:
						if (--braceCount < 0)
						{
							this.curToken = new Token(17, base.Col - 1, base.Line);
							return;
						}
						break;
					}
				}
				else
				{
					int next = base.ReaderRead();
					if (next == -1)
					{
						this.errors.Error(base.Line, base.Col, string.Format("EOF after @", new object[0]));
					}
					else if (next == 34)
					{
						this.ReadVerbatimString();
					}
				}
			}
			this.curToken = new Token(0, base.Col, base.Line);
		}
	}
}
