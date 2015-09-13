using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
	public class TypeReference : AbstractNode, INullable, ICloneable
	{
		public static readonly TypeReference StructConstraint;

		public static readonly TypeReference ClassConstraint;

		public static readonly TypeReference NewConstraint;

		private string type = "";

		private string systemType = "";

		private int pointerNestingLevel;

		private int[] rankSpecifier;

		private List<TypeReference> genericTypes = new List<TypeReference>();

		private bool isGlobal;

		private static Dictionary<string, string> types;

		private static Dictionary<string, string> vbtypes;

		private static Dictionary<string, string> typesReverse;

		private static Dictionary<string, string> vbtypesReverse;

		public static IDictionary<string, string> PrimitiveTypesCSharp
		{
			get
			{
				return TypeReference.types;
			}
		}

		public static IDictionary<string, string> PrimitiveTypesVB
		{
			get
			{
				return TypeReference.vbtypes;
			}
		}

		public static IDictionary<string, string> PrimitiveTypesCSharpReverse
		{
			get
			{
				return TypeReference.typesReverse;
			}
		}

		public static IDictionary<string, string> PrimitiveTypesVBReverse
		{
			get
			{
				return TypeReference.vbtypesReverse;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				Debug.Assert(value != null);
				this.type = value;
				this.systemType = TypeReference.GetSystemType(this.type);
			}
		}

		public string SystemType
		{
			get
			{
				return this.systemType;
			}
		}

		public int PointerNestingLevel
		{
			get
			{
				return this.pointerNestingLevel;
			}
			set
			{
				this.pointerNestingLevel = value;
			}
		}

		public int[] RankSpecifier
		{
			get
			{
				return this.rankSpecifier;
			}
			set
			{
				this.rankSpecifier = value;
			}
		}

		public List<TypeReference> GenericTypes
		{
			get
			{
				return this.genericTypes;
			}
		}

		public bool IsArrayType
		{
			get
			{
				return this.rankSpecifier != null && this.rankSpecifier.Length > 0;
			}
		}

		public static NullTypeReference Null
		{
			get
			{
				return NullTypeReference.Instance;
			}
		}

		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		public bool IsGlobal
		{
			get
			{
				return this.isGlobal;
			}
			set
			{
				this.isGlobal = value;
			}
		}

		static TypeReference()
		{
			TypeReference.StructConstraint = new TypeReference("constraint: struct");
			TypeReference.ClassConstraint = new TypeReference("constraint: class");
			TypeReference.NewConstraint = new TypeReference("constraint: new");
			TypeReference.types = new Dictionary<string, string>();
			TypeReference.vbtypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			TypeReference.typesReverse = new Dictionary<string, string>();
			TypeReference.vbtypesReverse = new Dictionary<string, string>();
			TypeReference.types.Add("bool", "System.Boolean");
			TypeReference.types.Add("byte", "System.Byte");
			TypeReference.types.Add("char", "System.Char");
			TypeReference.types.Add("decimal", "System.Decimal");
			TypeReference.types.Add("double", "System.Double");
			TypeReference.types.Add("float", "System.Single");
			TypeReference.types.Add("int", "System.Int32");
			TypeReference.types.Add("long", "System.Int64");
			TypeReference.types.Add("object", "System.Object");
			TypeReference.types.Add("sbyte", "System.SByte");
			TypeReference.types.Add("short", "System.Int16");
			TypeReference.types.Add("string", "System.String");
			TypeReference.types.Add("uint", "System.UInt32");
			TypeReference.types.Add("ulong", "System.UInt64");
			TypeReference.types.Add("ushort", "System.UInt16");
			TypeReference.types.Add("void", "System.Void");
			TypeReference.vbtypes.Add("Boolean", "System.Boolean");
			TypeReference.vbtypes.Add("Byte", "System.Byte");
			TypeReference.vbtypes.Add("SByte", "System.SByte");
			TypeReference.vbtypes.Add("Date", "System.DateTime");
			TypeReference.vbtypes.Add("Char", "System.Char");
			TypeReference.vbtypes.Add("Decimal", "System.Decimal");
			TypeReference.vbtypes.Add("Double", "System.Double");
			TypeReference.vbtypes.Add("Single", "System.Single");
			TypeReference.vbtypes.Add("Integer", "System.Int32");
			TypeReference.vbtypes.Add("Long", "System.Int64");
			TypeReference.vbtypes.Add("UInteger", "System.UInt32");
			TypeReference.vbtypes.Add("ULong", "System.UInt64");
			TypeReference.vbtypes.Add("Object", "System.Object");
			TypeReference.vbtypes.Add("Short", "System.Int16");
			TypeReference.vbtypes.Add("UShort", "System.UInt16");
			TypeReference.vbtypes.Add("String", "System.String");
			foreach (KeyValuePair<string, string> pair in TypeReference.types)
			{
				TypeReference.typesReverse.Add(pair.Value, pair.Key);
			}
			foreach (KeyValuePair<string, string> pair in TypeReference.vbtypes)
			{
				TypeReference.vbtypesReverse.Add(pair.Value, pair.Key);
			}
		}

		private static string GetSystemType(string type)
		{
			string result;
			string systemType;
			if (TypeReference.types == null)
			{
				result = type;
			}
			else if (TypeReference.types.TryGetValue(type, out systemType))
			{
				result = systemType;
			}
			else if (TypeReference.vbtypes.TryGetValue(type, out systemType))
			{
				result = systemType;
			}
			else
			{
				result = type;
			}
			return result;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public virtual TypeReference Clone()
		{
			TypeReference c = new TypeReference(this.type, this.systemType);
			TypeReference.CopyFields(this, c);
			return c;
		}

		protected static void CopyFields(TypeReference from, TypeReference to)
		{
			to.pointerNestingLevel = from.pointerNestingLevel;
			if (from.rankSpecifier != null)
			{
				to.rankSpecifier = (int[])from.rankSpecifier.Clone();
			}
			foreach (TypeReference r in from.genericTypes)
			{
				to.genericTypes.Add(r.Clone());
			}
			to.isGlobal = from.isGlobal;
		}

		public static string StripLastIdentifierFromType(ref TypeReference tr)
		{
			string result;
			if (tr is InnerClassTypeReference && ((InnerClassTypeReference)tr).Type.IndexOf('.') < 0)
			{
				string ident = ((InnerClassTypeReference)tr).Type;
				tr = ((InnerClassTypeReference)tr).BaseType;
				result = ident;
			}
			else
			{
				int pos = tr.Type.LastIndexOf('.');
				if (pos < 0)
				{
					result = tr.Type;
				}
				else
				{
					string ident = tr.Type.Substring(pos + 1);
					tr.Type = tr.Type.Substring(0, pos);
					result = ident;
				}
			}
			return result;
		}

		public static TypeReference CheckNull(TypeReference typeReference)
		{
			return typeReference ?? NullTypeReference.Instance;
		}

		public TypeReference(string type)
		{
			this.Type = type;
		}

		public TypeReference(string type, string systemType)
		{
			this.type = type;
			this.systemType = systemType;
		}

		public TypeReference(string type, List<TypeReference> genericTypes) : this(type)
		{
			if (genericTypes != null)
			{
				this.genericTypes = genericTypes;
			}
		}

		public TypeReference(string type, int[] rankSpecifier) : this(type, 0, rankSpecifier)
		{
		}

		public TypeReference(string type, int pointerNestingLevel, int[] rankSpecifier) : this(type, pointerNestingLevel, rankSpecifier, null)
		{
		}

		public TypeReference(string type, int pointerNestingLevel, int[] rankSpecifier, List<TypeReference> genericTypes)
		{
			Debug.Assert(type != null);
			this.type = type;
			this.systemType = TypeReference.GetSystemType(type);
			this.pointerNestingLevel = pointerNestingLevel;
			this.rankSpecifier = rankSpecifier;
			if (genericTypes != null)
			{
				this.genericTypes = genericTypes;
			}
		}

		protected TypeReference()
		{
		}

		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitTypeReference(this, data);
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder(this.type);
			if (this.genericTypes != null && this.genericTypes.Count > 0)
			{
				b.Append('<');
				for (int i = 0; i < this.genericTypes.Count; i++)
				{
					if (i > 0)
					{
						b.Append(',');
					}
					b.Append(this.genericTypes[i].ToString());
				}
				b.Append('>');
			}
			if (this.pointerNestingLevel > 0)
			{
				b.Append('*', this.pointerNestingLevel);
			}
			if (this.IsArrayType)
			{
				int[] array = this.rankSpecifier;
				for (int j = 0; j < array.Length; j++)
				{
					int rank = array[j];
					b.Append('[');
					if (rank < 0)
					{
						b.Append('`', -rank);
					}
					else
					{
						b.Append(',', rank);
					}
					b.Append(']');
				}
			}
			return b.ToString();
		}

		public static bool AreEqualReferences(TypeReference a, TypeReference b)
		{
			bool result;
			if (a == b)
			{
				result = true;
			}
			else if (a == null || b == null)
			{
				result = false;
			}
			else
			{
				if (a is InnerClassTypeReference)
				{
					a = ((InnerClassTypeReference)a).CombineToNormalTypeReference();
				}
				if (b is InnerClassTypeReference)
				{
					b = ((InnerClassTypeReference)b).CombineToNormalTypeReference();
				}
				if (a.systemType != b.systemType)
				{
					result = false;
				}
				else if (a.pointerNestingLevel != b.pointerNestingLevel)
				{
					result = false;
				}
				else if (a.IsArrayType != b.IsArrayType)
				{
					result = false;
				}
				else
				{
					if (a.IsArrayType)
					{
						if (a.rankSpecifier.Length != b.rankSpecifier.Length)
						{
							result = false;
							return result;
						}
						for (int i = 0; i < a.rankSpecifier.Length; i++)
						{
							if (a.rankSpecifier[i] != b.rankSpecifier[i])
							{
								result = false;
								return result;
							}
						}
					}
					if (a.genericTypes.Count != b.genericTypes.Count)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < a.genericTypes.Count; i++)
						{
							if (!TypeReference.AreEqualReferences(a.genericTypes[i], b.genericTypes[i]))
							{
								result = false;
								return result;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}
	}
}
