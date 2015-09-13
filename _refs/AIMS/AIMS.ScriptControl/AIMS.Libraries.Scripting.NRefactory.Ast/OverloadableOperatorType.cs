using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public enum OverloadableOperatorType
	{
		None,
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		Concat,
		Not,
		BitNot,
		BitwiseAnd,
		BitwiseOr,
		ExclusiveOr,
		ShiftLeft,
		ShiftRight,
		GreaterThan,
		GreaterThanOrEqual,
		Equality,
		InEquality,
		LessThan,
		LessThanOrEqual,
		Increment,
		Decrement,
		IsTrue,
		IsFalse,
		Like,
		Power,
		CType,
		DivideInteger
	}
}
