using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public static class MemberLookupHelper
	{
		private const int Byte = 1;

		private const int Short = 2;

		private const int Int = 3;

		private const int Long = 4;

		private const int SByte = 5;

		private const int UShort = 6;

		private const int UInt = 7;

		private const int ULong = 8;

		private const int Float = 9;

		private const int Double = 10;

		private const int Char = 11;

		private const int Decimal = 12;

		public static IMethod FindOverload(IList<IMethod> methods, IReturnType[] typeParameters, IReturnType[] arguments)
		{
			IMethod result;
			if (methods.Count == 0)
			{
				result = null;
			}
			else
			{
				bool tmp;
				int[] ranking = MemberLookupHelper.RankOverloads(methods, typeParameters, arguments, false, out tmp);
				int bestRanking = -1;
				int best = 0;
				for (int i = 0; i < ranking.Length; i++)
				{
					if (ranking[i] > bestRanking)
					{
						bestRanking = ranking[i];
						best = i;
					}
				}
				result = methods[best];
			}
			return result;
		}

		public static IProperty FindOverload(IList<IProperty> properties, IReturnType[] arguments)
		{
			IProperty result;
			if (properties.Count == 0)
			{
				result = null;
			}
			else
			{
				List<IMethodOrProperty> newList = new List<IMethodOrProperty>(properties.Count);
				foreach (IProperty p in properties)
				{
					newList.Add(p);
				}
				bool tmp;
				IReturnType[][] tmp2;
				int[] ranking = MemberLookupHelper.RankOverloads(newList, arguments, false, out tmp, out tmp2);
				int bestRanking = -1;
				int best = 0;
				for (int i = 0; i < ranking.Length; i++)
				{
					if (ranking[i] > bestRanking)
					{
						bestRanking = ranking[i];
						best = i;
					}
				}
				result = properties[best];
			}
			return result;
		}

		public static int[] RankOverloads(IList<IMethod> list, IReturnType[] typeParameters, IReturnType[] arguments, bool allowAdditionalArguments, out bool acceptableMatch)
		{
			acceptableMatch = false;
			int[] result;
			if (list.Count == 0)
			{
				result = new int[0];
			}
			else
			{
				List<IMethodOrProperty> l2 = new List<IMethodOrProperty>(list.Count);
				if (typeParameters != null && typeParameters.Length > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						IMethod j = list[i];
						if (j.TypeParameters.Count == typeParameters.Length)
						{
							j = (IMethod)j.Clone();
							j.ReturnType = ConstructedReturnType.TranslateType(j.ReturnType, typeParameters, true);
							for (int k = 0; k < j.Parameters.Count; k++)
							{
								j.Parameters[k].ReturnType = ConstructedReturnType.TranslateType(j.Parameters[k].ReturnType, typeParameters, true);
							}
							list[i] = j;
							l2.Add(j);
						}
					}
					IReturnType[][] inferredTypeParameters;
					int[] innerRanking = MemberLookupHelper.RankOverloads(l2, arguments, allowAdditionalArguments, out acceptableMatch, out inferredTypeParameters);
					int[] ranking = new int[list.Count];
					int innerIndex = 0;
					for (int i = 0; i < ranking.Length; i++)
					{
						if (list[i].TypeParameters.Count == typeParameters.Length)
						{
							ranking[i] = innerRanking[innerIndex++];
						}
						else
						{
							ranking[i] = 0;
						}
					}
					result = ranking;
				}
				else
				{
					foreach (IMethod j in list)
					{
						l2.Add(j);
					}
					IReturnType[][] inferredTypeParameters;
					int[] ranking = MemberLookupHelper.RankOverloads(l2, arguments, allowAdditionalArguments, out acceptableMatch, out inferredTypeParameters);
					MemberLookupHelper.ApplyInferredTypeParameters(list, inferredTypeParameters);
					result = ranking;
				}
			}
			return result;
		}

		private static void ApplyInferredTypeParameters(IList<IMethod> list, IReturnType[][] inferredTypeParameters)
		{
			if (inferredTypeParameters != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					IReturnType[] inferred = inferredTypeParameters[i];
					if (inferred != null && inferred.Length > 0)
					{
						IMethod j = (IMethod)list[i].Clone();
						j.ReturnType = ConstructedReturnType.TranslateType(j.ReturnType, inferred, true);
						for (int k = 0; k < j.Parameters.Count; k++)
						{
							j.Parameters[k].ReturnType = ConstructedReturnType.TranslateType(j.Parameters[k].ReturnType, inferred, true);
						}
						list[i] = j;
					}
				}
			}
		}

		public static int[] RankOverloads(IList<IMethodOrProperty> list, IReturnType[] arguments, bool allowAdditionalArguments, out bool acceptableMatch, out IReturnType[][] inferredTypeParameters)
		{
			acceptableMatch = false;
			inferredTypeParameters = null;
			int[] result;
			if (list.Count == 0)
			{
				result = new int[0];
			}
			else
			{
				int[] ranking = new int[list.Count];
				bool[] needToExpand = new bool[list.Count];
				int maxScore = 0;
				int baseScore = 0;
				int score;
				for (int i = 0; i < list.Count; i++)
				{
					bool expanded;
					if (MemberLookupHelper.IsApplicable(list[i].Parameters, arguments, allowAdditionalArguments, out score, out expanded))
					{
						acceptableMatch = true;
						score = 2147483647;
					}
					else
					{
						baseScore = Math.Max(baseScore, score);
					}
					needToExpand[i] = expanded;
					ranking[i] = score;
					maxScore = Math.Max(maxScore, score);
				}
				IReturnType[][] expandedParameters = MemberLookupHelper.ExpandParametersAndSubstitute(list, arguments, maxScore, ranking, needToExpand, out inferredTypeParameters);
				score = baseScore + 2;
				int bestIndex = -1;
				for (int i = 0; i < ranking.Length; i++)
				{
					if (ranking[i] == maxScore)
					{
						if (bestIndex < 0)
						{
							ranking[i] = score;
							bestIndex = i;
						}
						else
						{
							switch (MemberLookupHelper.GetBetterFunctionMember(arguments, list[i], expandedParameters[i], needToExpand[i], list[bestIndex], expandedParameters[bestIndex], needToExpand[bestIndex]))
							{
							case 0:
								ranking[i] = score;
								break;
							case 1:
								score = (ranking[i] = score + 1);
								bestIndex = i;
								break;
							case 2:
								ranking[i] = score - 1;
								break;
							}
						}
					}
				}
				result = ranking;
			}
			return result;
		}

		private static IReturnType[][] ExpandParametersAndSubstitute(IList<IMethodOrProperty> list, IReturnType[] arguments, int maxScore, int[] ranking, bool[] needToExpand, out IReturnType[][] inferredTypeParameters)
		{
			IReturnType[][] expandedParameters = new IReturnType[list.Count][];
			inferredTypeParameters = new IReturnType[list.Count][];
			for (int i = 0; i < ranking.Length; i++)
			{
				if (ranking[i] == maxScore)
				{
					IList<IParameter> parameters = list[i].Parameters;
					IReturnType[] typeParameters = (list[i] is IMethod) ? MemberLookupHelper.InferTypeArguments((IMethod)list[i], arguments) : null;
					inferredTypeParameters[i] = typeParameters;
					IReturnType paramsType = null;
					expandedParameters[i] = new IReturnType[arguments.Length];
					for (int j = 0; j < arguments.Length; j++)
					{
						if (j < parameters.Count)
						{
							IParameter parameter = parameters[j];
							if (parameter.IsParams && needToExpand[i])
							{
								if (parameter.ReturnType.IsArrayReturnType)
								{
									paramsType = parameter.ReturnType.CastToArrayReturnType().ArrayElementType;
									paramsType = ConstructedReturnType.TranslateType(paramsType, typeParameters, true);
								}
								expandedParameters[i][j] = paramsType;
							}
							else
							{
								expandedParameters[i][j] = ConstructedReturnType.TranslateType(parameter.ReturnType, typeParameters, true);
							}
						}
						else
						{
							expandedParameters[i][j] = paramsType;
						}
					}
				}
			}
			return expandedParameters;
		}

		private static int GetBetterFunctionMember(IReturnType[] arguments, IMethodOrProperty m1, IReturnType[] parameters1, bool isExpanded1, IMethodOrProperty m2, IReturnType[] parameters2, bool isExpanded2)
		{
			int length = Math.Min(Math.Min(parameters1.Length, parameters2.Length), arguments.Length);
			bool foundBetterParamIn = false;
			bool foundBetterParamIn2 = false;
			for (int i = 0; i < length; i++)
			{
				if (arguments[i] != null)
				{
					int res = MemberLookupHelper.GetBetterConversion(arguments[i], parameters1[i], parameters2[i]);
					if (res == 1)
					{
						foundBetterParamIn = true;
					}
					if (res == 2)
					{
						foundBetterParamIn2 = true;
					}
				}
			}
			int result;
			if (foundBetterParamIn && !foundBetterParamIn2)
			{
				result = 1;
			}
			else if (foundBetterParamIn2 && !foundBetterParamIn)
			{
				result = 2;
			}
			else if (foundBetterParamIn && foundBetterParamIn2)
			{
				result = 0;
			}
			else
			{
				for (int i = 0; i < length; i++)
				{
					if (!object.Equals(parameters1[i], parameters2[i]))
					{
						result = 0;
						return result;
					}
				}
				bool m1IsGeneric = m1 is IMethod && ((IMethod)m1).TypeParameters.Count > 0;
				bool m2IsGeneric = m2 is IMethod && ((IMethod)m2).TypeParameters.Count > 0;
				if (m1IsGeneric && !m2IsGeneric)
				{
					result = 2;
				}
				else if (m2IsGeneric && !m1IsGeneric)
				{
					result = 1;
				}
				else if (isExpanded1 && !isExpanded2)
				{
					result = 2;
				}
				else if (isExpanded2 && !isExpanded1)
				{
					result = 1;
				}
				else if (m1.Parameters.Count > m2.Parameters.Count)
				{
					result = 1;
				}
				else if (m2.Parameters.Count > m1.Parameters.Count)
				{
					result = 2;
				}
				else
				{
					IReturnType[] m1ParamTypes = new IReturnType[m1.Parameters.Count];
					IReturnType[] m2ParamTypes = new IReturnType[m2.Parameters.Count];
					for (int i = 0; i < m1ParamTypes.Length; i++)
					{
						m1ParamTypes[i] = m1.Parameters[i].ReturnType;
						m2ParamTypes[i] = m2.Parameters[i].ReturnType;
					}
					result = MemberLookupHelper.GetMoreSpecific(m1ParamTypes, m2ParamTypes);
				}
			}
			return result;
		}

		private static int GetMoreSpecific(IList<IReturnType> r, IList<IReturnType> s)
		{
			bool foundMoreSpecificParamIn = false;
			bool foundMoreSpecificParamIn2 = false;
			int length = Math.Min(r.Count, s.Count);
			for (int i = 0; i < length; i++)
			{
				int res = MemberLookupHelper.GetMoreSpecific(r[i], s[i]);
				if (res == 1)
				{
					foundMoreSpecificParamIn = true;
				}
				if (res == 2)
				{
					foundMoreSpecificParamIn2 = true;
				}
			}
			int result;
			if (foundMoreSpecificParamIn && !foundMoreSpecificParamIn2)
			{
				result = 1;
			}
			else if (foundMoreSpecificParamIn2 && !foundMoreSpecificParamIn)
			{
				result = 2;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private static int GetMoreSpecific(IReturnType r, IReturnType s)
		{
			int result;
			if (r == null && s == null)
			{
				result = 0;
			}
			else if (r == null)
			{
				result = 2;
			}
			else if (s == null)
			{
				result = 1;
			}
			else if (r.IsGenericReturnType && !s.IsGenericReturnType)
			{
				result = 2;
			}
			else if (s.IsGenericReturnType && !r.IsGenericReturnType)
			{
				result = 1;
			}
			else if (r.IsArrayReturnType && s.IsArrayReturnType)
			{
				result = MemberLookupHelper.GetMoreSpecific(r.CastToArrayReturnType().ArrayElementType, s.CastToArrayReturnType().ArrayElementType);
			}
			else if (r.IsConstructedReturnType && s.IsConstructedReturnType)
			{
				result = MemberLookupHelper.GetMoreSpecific(r.CastToConstructedReturnType().TypeArguments, s.CastToConstructedReturnType().TypeArguments);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private static IReturnType[] InferTypeArguments(IMethod method, IReturnType[] arguments)
		{
			int count = method.TypeParameters.Count;
			IReturnType[] result2;
			if (count == 0)
			{
				result2 = null;
			}
			else
			{
				IReturnType[] result = new IReturnType[count];
				IList<IParameter> parameters = method.Parameters;
				for (int i = 0; i < arguments.Length; i++)
				{
					if (i >= parameters.Count)
					{
						break;
					}
					if (!MemberLookupHelper.InferTypeArgument(parameters[i].ReturnType, arguments[i], result))
					{
						if (parameters[i].IsParams && parameters[i].ReturnType.IsArrayReturnType)
						{
							ArrayReturnType art = parameters[i].ReturnType.CastToArrayReturnType();
							if (art.ArrayDimensions == 1)
							{
								MemberLookupHelper.InferTypeArgument(art.ArrayElementType, arguments[i], result);
							}
						}
					}
				}
				for (int i = 0; i < result.Length; i++)
				{
					if (result[i] != null)
					{
						result2 = result;
						return result2;
					}
				}
				result2 = null;
			}
			return result2;
		}

		public static bool InferTypeArgument(IReturnType expectedArgument, IReturnType passedArgument, IReturnType[] outputArray)
		{
			bool result;
			if (expectedArgument == null)
			{
				result = true;
			}
			else if (passedArgument == null || passedArgument == NullReturnType.Instance)
			{
				result = true;
			}
			else if (passedArgument.IsArrayReturnType)
			{
				IReturnType passedArrayElementType = passedArgument.CastToArrayReturnType().ArrayElementType;
				if (expectedArgument.IsArrayReturnType && expectedArgument.CastToArrayReturnType().ArrayDimensions == passedArgument.CastToArrayReturnType().ArrayDimensions)
				{
					result = MemberLookupHelper.InferTypeArgument(expectedArgument.CastToArrayReturnType().ArrayElementType, passedArrayElementType, outputArray);
				}
				else
				{
					if (expectedArgument.IsConstructedReturnType)
					{
						string fullyQualifiedName = expectedArgument.FullyQualifiedName;
						if (fullyQualifiedName != null)
						{
							if (fullyQualifiedName == "System.Collections.Generic.IList" || fullyQualifiedName == "System.Collections.Generic.ICollection" || fullyQualifiedName == "System.Collections.Generic.IEnumerable")
							{
								result = MemberLookupHelper.InferTypeArgument(expectedArgument.CastToConstructedReturnType().TypeArguments[0], passedArrayElementType, outputArray);
								return result;
							}
						}
					}
					result = false;
				}
			}
			else
			{
				if (expectedArgument.IsGenericReturnType)
				{
					GenericReturnType methodTP = expectedArgument.CastToGenericReturnType();
					if (methodTP.TypeParameter.Method != null)
					{
						if (methodTP.TypeParameter.Index < outputArray.Length)
						{
							outputArray[methodTP.TypeParameter.Index] = passedArgument;
						}
						result = true;
						return result;
					}
				}
				if (expectedArgument.IsConstructedReturnType)
				{
					if (!passedArgument.IsConstructedReturnType)
					{
						result = false;
						return result;
					}
					IList<IReturnType> expectedTA = expectedArgument.CastToConstructedReturnType().TypeArguments;
					IList<IReturnType> passedTA = passedArgument.CastToConstructedReturnType().TypeArguments;
					int count = Math.Min(expectedTA.Count, passedTA.Count);
					for (int i = 0; i < count; i++)
					{
						MemberLookupHelper.InferTypeArgument(expectedTA[i], passedTA[i], outputArray);
					}
				}
				result = true;
			}
			return result;
		}

		private static bool IsApplicable(IList<IParameter> parameters, IReturnType[] arguments, bool allowAdditionalArguments, out int score, out bool expanded)
		{
			expanded = false;
			score = 0;
			bool result;
			if (parameters.Count == 0)
			{
				result = (arguments.Length == 0);
			}
			else if (!allowAdditionalArguments && parameters.Count > arguments.Length + 1)
			{
				result = false;
			}
			else
			{
				int lastParameter = parameters.Count - 1;
				bool ok = true;
				for (int i = 0; i < Math.Min(lastParameter, arguments.Length); i++)
				{
					if (MemberLookupHelper.IsApplicable(arguments[i], parameters[i].ReturnType))
					{
						score++;
					}
					else
					{
						ok = false;
					}
				}
				if (!ok)
				{
					result = false;
				}
				else
				{
					if (parameters.Count == arguments.Length)
					{
						if (MemberLookupHelper.IsApplicable(arguments[lastParameter], parameters[lastParameter].ReturnType))
						{
							result = true;
							return result;
						}
					}
					if (!parameters[lastParameter].IsParams)
					{
						result = false;
					}
					else
					{
						expanded = true;
						score++;
						IReturnType rt = parameters[lastParameter].ReturnType;
						if (rt == null || !rt.IsArrayReturnType)
						{
							result = false;
						}
						else
						{
							for (int i = lastParameter; i < arguments.Length; i++)
							{
								if (MemberLookupHelper.IsApplicable(arguments[i], rt.CastToArrayReturnType().ArrayElementType))
								{
									score++;
								}
								else
								{
									ok = false;
								}
							}
							result = ok;
						}
					}
				}
			}
			return result;
		}

		private static bool IsApplicable(IReturnType argument, IReturnType expected)
		{
			bool result;
			if (argument == null)
			{
				result = true;
			}
			else
			{
				if (expected.IsGenericReturnType)
				{
					foreach (IReturnType constraint in expected.CastToGenericReturnType().TypeParameter.Constraints)
					{
						if (!MemberLookupHelper.ConversionExists(argument, constraint))
						{
							result = false;
							return result;
						}
					}
				}
				result = MemberLookupHelper.ConversionExists(argument, expected);
			}
			return result;
		}

		public static bool ConversionExists(IReturnType from, IReturnType to)
		{
			bool result;
			if (from == to)
			{
				result = true;
			}
			else if (from == null || to == null)
			{
				result = false;
			}
			else if (from.Equals(to))
			{
				result = true;
			}
			else
			{
				bool fromIsDefault = from.IsDefaultReturnType;
				bool toIsDefault = to.IsDefaultReturnType;
				if (fromIsDefault && toIsDefault)
				{
					int f = MemberLookupHelper.GetPrimitiveType(from);
					int t = MemberLookupHelper.GetPrimitiveType(to);
					if (f == 5 && (t == 2 || t == 3 || t == 4 || t == 9 || t == 10 || t == 12))
					{
						result = true;
						return result;
					}
					if (f == 1 && (t == 2 || t == 6 || t == 3 || t == 7 || t == 4 || t == 8 || t == 9 || t == 10 || t == 12))
					{
						result = true;
						return result;
					}
					if (f == 2 && (t == 3 || t == 4 || t == 9 || t == 10 || t == 12))
					{
						result = true;
						return result;
					}
					if (f == 6 && (t == 3 || t == 7 || t == 4 || t == 8 || t == 9 || t == 10 || t == 12))
					{
						result = true;
						return result;
					}
					if (f == 3 && (t == 4 || t == 9 || t == 10 || t == 12))
					{
						result = true;
						return result;
					}
					if (f == 7 && (t == 4 || t == 8 || t == 9 || t == 10 || t == 12))
					{
						result = true;
						return result;
					}
					if ((f == 4 || f == 8) && (t == 9 || t == 10 || t == 12))
					{
						result = true;
						return result;
					}
					if (f == 11 && (t == 6 || t == 3 || t == 7 || t == 4 || t == 8 || t == 9 || t == 10 || t == 12))
					{
						result = true;
						return result;
					}
					if (f == 9 && t == 10)
					{
						result = true;
						return result;
					}
				}
				if (toIsDefault && to.FullyQualifiedName == "System.Object")
				{
					result = true;
				}
				else
				{
					if (toIsDefault && (fromIsDefault || from.IsArrayReturnType))
					{
						IClass c = from.GetUnderlyingClass();
						IClass c2 = to.GetUnderlyingClass();
						if (c != null && c.IsTypeInInheritanceTree(c2))
						{
							result = true;
							return result;
						}
					}
					if (from.IsArrayReturnType && to.IsArrayReturnType)
					{
						ArrayReturnType fromArt = from.CastToArrayReturnType();
						ArrayReturnType toArt = to.CastToArrayReturnType();
						if (fromArt.ArrayDimensions == toArt.ArrayDimensions)
						{
							result = MemberLookupHelper.ConversionExists(fromArt.ArrayElementType, toArt.ArrayElementType);
							return result;
						}
					}
					if (from.IsConstructedReturnType && to.IsConstructedReturnType)
					{
						if (from.FullyQualifiedName == to.FullyQualifiedName)
						{
							IList<IReturnType> fromTypeArguments = from.CastToConstructedReturnType().TypeArguments;
							IList<IReturnType> toTypeArguments = to.CastToConstructedReturnType().TypeArguments;
							if (fromTypeArguments.Count == toTypeArguments.Count)
							{
								for (int i = 0; i < fromTypeArguments.Count; i++)
								{
									if (fromTypeArguments[i] != toTypeArguments[i])
									{
										if (!object.Equals(fromTypeArguments[i], toTypeArguments[i]))
										{
											if (!toTypeArguments[i].IsGenericReturnType)
											{
												result = false;
												return result;
											}
										}
									}
								}
								result = true;
								return result;
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		public static int GetBetterConversion(IReturnType from, IReturnType to1, IReturnType to2)
		{
			int result;
			if (from == null)
			{
				result = 0;
			}
			else if (to1 == null)
			{
				result = 2;
			}
			else if (to2 == null)
			{
				result = 1;
			}
			else if (to1.Equals(to2))
			{
				result = 0;
			}
			else if (from.Equals(to1))
			{
				result = 1;
			}
			else if (from.Equals(to2))
			{
				result = 2;
			}
			else
			{
				bool canConvertFrom1To2 = MemberLookupHelper.ConversionExists(to1, to2);
				bool canConvertFrom2To = MemberLookupHelper.ConversionExists(to2, to1);
				if (canConvertFrom1To2 && !canConvertFrom2To)
				{
					result = 1;
				}
				else if (canConvertFrom2To && !canConvertFrom1To2)
				{
					result = 2;
				}
				else if (to1.IsDefaultReturnType && to2.IsDefaultReturnType)
				{
					result = MemberLookupHelper.GetBetterPrimitiveConversion(to1, to2);
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		private static int GetBetterPrimitiveConversion(IReturnType to1, IReturnType to2)
		{
			int t = MemberLookupHelper.GetPrimitiveType(to1);
			int t2 = MemberLookupHelper.GetPrimitiveType(to2);
			int result;
			if (t == 0 || t2 == 0)
			{
				result = 0;
			}
			else if (t == 5 && (t2 == 1 || t2 == 6 || t2 == 7 || t2 == 8))
			{
				result = 1;
			}
			else if (t2 == 5 && (t == 1 || t == 6 || t == 7 || t == 8))
			{
				result = 2;
			}
			else if (t == 2 && (t2 == 6 || t2 == 7 || t2 == 8))
			{
				result = 1;
			}
			else if (t2 == 2 && (t == 6 || t == 7 || t == 8))
			{
				result = 2;
			}
			else if (t == 3 && (t2 == 7 || t2 == 8))
			{
				result = 1;
			}
			else if (t2 == 3 && (t == 7 || t == 8))
			{
				result = 2;
			}
			else if (t == 4 && t2 == 8)
			{
				result = 1;
			}
			else if (t2 == 4 && t == 8)
			{
				result = 2;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private static int GetPrimitiveType(IReturnType t)
		{
			string fullyQualifiedName = t.FullyQualifiedName;
			int result;
			switch (fullyQualifiedName)
			{
			case "System.SByte":
				result = 5;
				return result;
			case "System.Byte":
				result = 1;
				return result;
			case "System.Int16":
				result = 2;
				return result;
			case "System.UInt16":
				result = 6;
				return result;
			case "System.Int32":
				result = 3;
				return result;
			case "System.UInt32":
				result = 7;
				return result;
			case "System.Int64":
				result = 4;
				return result;
			case "System.UInt64":
				result = 8;
				return result;
			case "System.Single":
				result = 9;
				return result;
			case "System.Double":
				result = 10;
				return result;
			case "System.Char":
				result = 11;
				return result;
			case "System.Decimal":
				result = 12;
				return result;
			}
			result = 0;
			return result;
		}

		public static IReturnType GetCommonType(IProjectContent projectContent, IReturnType a, IReturnType b)
		{
			if (projectContent == null)
			{
				throw new ArgumentNullException("projectContent");
			}
			IReturnType result;
			if (a == null)
			{
				result = b;
			}
			else if (b == null)
			{
				result = a;
			}
			else if (MemberLookupHelper.ConversionExists(a, b))
			{
				result = b;
			}
			else if (MemberLookupHelper.ConversionExists(b, a))
			{
				result = a;
			}
			else
			{
				IClass c = a.GetUnderlyingClass();
				if (c != null)
				{
					foreach (IClass baseClass in c.ClassInheritanceTree)
					{
						IReturnType baseType = baseClass.DefaultReturnType;
						if (baseClass.TypeParameters.Count > 0)
						{
							IReturnType[] typeArguments = new IReturnType[baseClass.TypeParameters.Count];
							for (int i = 0; i < typeArguments.Length; i++)
							{
								typeArguments[i] = MemberLookupHelper.GetTypeParameterPassedToBaseClass(a, baseClass, i);
							}
							baseType = new ConstructedReturnType(baseType, typeArguments);
						}
						if (MemberLookupHelper.ConversionExists(b, baseType))
						{
							result = baseType;
							return result;
						}
					}
				}
				result = projectContent.SystemTypes.Object;
			}
			return result;
		}

		public static IReturnType GetTypeParameterPassedToBaseClass(IReturnType parentType, IClass baseClass, int baseClassTypeParameterIndex)
		{
			IReturnType result2;
			if (!parentType.IsConstructedReturnType)
			{
				result2 = null;
			}
			else
			{
				ConstructedReturnType returnType = parentType.CastToConstructedReturnType();
				IClass c = returnType.GetUnderlyingClass();
				if (c == null)
				{
					result2 = null;
				}
				else if (baseClass.CompareTo(c) == 0)
				{
					if (baseClassTypeParameterIndex >= returnType.TypeArguments.Count)
					{
						result2 = null;
					}
					else
					{
						result2 = returnType.TypeArguments[baseClassTypeParameterIndex];
					}
				}
				else
				{
					foreach (IReturnType baseType in c.BaseTypes)
					{
						if (baseClass.CompareTo(baseType.GetUnderlyingClass()) == 0)
						{
							if (!baseType.IsConstructedReturnType)
							{
								result2 = null;
								return result2;
							}
							ConstructedReturnType baseTypeCRT = baseType.CastToConstructedReturnType();
							if (baseClassTypeParameterIndex >= baseTypeCRT.TypeArguments.Count)
							{
								result2 = null;
								return result2;
							}
							IReturnType result = baseTypeCRT.TypeArguments[baseClassTypeParameterIndex];
							if (returnType.TypeArguments != null)
							{
								result = ConstructedReturnType.TranslateType(result, returnType.TypeArguments, false);
							}
							result2 = result;
							return result2;
						}
					}
					result2 = null;
				}
			}
			return result2;
		}
	}
}
