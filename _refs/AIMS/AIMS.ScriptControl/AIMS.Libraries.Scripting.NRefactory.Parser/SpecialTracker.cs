using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
	public class SpecialTracker
	{
		private List<ISpecial> currentSpecials = new List<ISpecial>();

		private CommentType currentCommentType;

		private StringBuilder sb = new StringBuilder();

		private Location startPosition;

		public List<ISpecial> CurrentSpecials
		{
			get
			{
				return this.currentSpecials;
			}
		}

		public void InformToken(int kind)
		{
		}

		public List<ISpecial> RetrieveSpecials()
		{
			List<ISpecial> tmp = this.currentSpecials;
			this.currentSpecials = new List<ISpecial>();
			return tmp;
		}

		public void AddEndOfLine(Location point)
		{
			this.currentSpecials.Add(new BlankLine(point));
		}

		public void AddPreprocessingDirective(string cmd, string arg, Location start, Location end)
		{
			this.currentSpecials.Add(new PreprocessingDirective(cmd, arg, start, end));
		}

		public void StartComment(CommentType commentType, Location startPosition)
		{
			this.currentCommentType = commentType;
			this.startPosition = startPosition;
			this.sb.Length = 0;
		}

		public void AddChar(char c)
		{
			this.sb.Append(c);
		}

		public void AddString(string s)
		{
			this.sb.Append(s);
		}

		public void FinishComment(Location endPosition)
		{
			this.currentSpecials.Add(new Comment(this.currentCommentType, this.sb.ToString(), this.startPosition, endPosition));
		}
	}
}
