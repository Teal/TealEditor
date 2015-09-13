using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public enum BinaryOperatorType
	{
		None,
		BitwiseAnd,
		BitwiseOr,
		LogicalAnd,
		LogicalOr,
		ExclusiveOr,
		GreaterThan,
		GreaterThanOrEqual,
		Equality,
		InEquality,
		LessThan,
		LessThanOrEqual,
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		DivideInteger,
		Power,
		Concat,
		ShiftLeft,
		ShiftRight,
		ReferenceEquality,
		ReferenceInequality,
		Like,
		NullCoalescing
	}
}
