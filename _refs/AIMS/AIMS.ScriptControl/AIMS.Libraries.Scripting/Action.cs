using System;

namespace AIMS.Libraries.Scripting
{
	public delegate void Action();
	public delegate void Action<A, B>(A arg1, B arg2);
	public delegate void Action<A, B, C>(A arg1, B arg2, C arg3);
}
