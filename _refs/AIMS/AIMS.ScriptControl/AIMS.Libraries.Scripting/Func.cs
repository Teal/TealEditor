using System;

namespace AIMS.Libraries.Scripting
{
	public delegate R Func<R>();
	public delegate R Func<A, R>(A arg1);
	public delegate R Func<A, B, R>(A arg1, B arg2);
	public delegate R Func<A, B, C, R>(A arg1, B arg2, C arg3);
}
