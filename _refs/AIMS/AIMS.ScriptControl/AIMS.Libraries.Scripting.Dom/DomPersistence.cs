using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class DomPersistence
	{
		private struct ClassNameTypeCountPair
		{
			public readonly string ClassName;

			public readonly byte TypeParameterCount;

			public ClassNameTypeCountPair(IClass c)
			{
				this.ClassName = c.FullyQualifiedName;
				this.TypeParameterCount = (byte)c.TypeParameters.Count;
			}

			public ClassNameTypeCountPair(IReturnType rt)
			{
				this.ClassName = rt.FullyQualifiedName;
				this.TypeParameterCount = (byte)rt.TypeParameterCount;
			}

			public override bool Equals(object obj)
			{
				bool result;
				if (!(obj is DomPersistence.ClassNameTypeCountPair))
				{
					result = false;
				}
				else
				{
					DomPersistence.ClassNameTypeCountPair myClassNameTypeCountPair = (DomPersistence.ClassNameTypeCountPair)obj;
					result = (this.ClassName.Equals(myClassNameTypeCountPair.ClassName, StringComparison.InvariantCultureIgnoreCase) && this.TypeParameterCount == myClassNameTypeCountPair.TypeParameterCount);
				}
				return result;
			}

			public override int GetHashCode()
			{
				return StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.ClassName) ^ (int)(this.TypeParameterCount * 5);
			}
		}

		private sealed class ReadWriteHelper
		{
			private const int ArrayRTCode = -1;

			private const int ConstructedRTCode = -2;

			private const int TypeGenericRTCode = -3;

			private const int MethodGenericRTCode = -4;

			private const int NullRTReferenceCode = -5;

			private const int VoidRTCode = -6;

			private ReflectionProjectContent pc;

			private readonly BinaryWriter writer;

			private readonly Dictionary<DomPersistence.ClassNameTypeCountPair, int> classIndices = new Dictionary<DomPersistence.ClassNameTypeCountPair, int>();

			private readonly Dictionary<string, int> stringDict = new Dictionary<string, int>();

			private readonly BinaryReader reader;

			private IReturnType[] types;

			private string[] stringArray;

			private IClass currentClass;

			private IMethod currentMethod;

			public ReadWriteHelper(BinaryWriter writer)
			{
				this.writer = writer;
			}

			public void WriteProjectContent(ReflectionProjectContent pc)
			{
				this.pc = pc;
				this.writer.Write(1252935504315237004L);
				this.writer.Write(11);
				this.writer.Write(pc.AssemblyFullName);
				this.writer.Write(pc.AssemblyLocation);
				long time = 0L;
				try
				{
					time = File.GetLastWriteTimeUtc(pc.AssemblyLocation).ToFileTime();
				}
				catch
				{
				}
				this.writer.Write(time);
				this.writer.Write(pc.ReferencedAssemblyNames.Count);
				foreach (DomAssemblyName name in pc.ReferencedAssemblyNames)
				{
					this.writer.Write(name.FullName);
				}
				this.WriteClasses();
			}

			public ReadWriteHelper(BinaryReader reader)
			{
				this.reader = reader;
			}

			public ReflectionProjectContent ReadProjectContent(ProjectContentRegistry registry)
			{
				ReflectionProjectContent result;
				if (this.reader.ReadInt64() != 1252935504315237004L)
				{
					LoggingService.Warn("Read dom: wrong magic");
					result = null;
				}
				else if (this.reader.ReadInt16() != 11)
				{
					LoggingService.Warn("Read dom: wrong version");
					result = null;
				}
				else
				{
					string assemblyName = this.reader.ReadString();
					string assemblyLocation = this.reader.ReadString();
					long time = 0L;
					try
					{
						time = File.GetLastWriteTimeUtc(assemblyLocation).ToFileTime();
					}
					catch
					{
					}
					if (this.reader.ReadInt64() != time)
					{
						LoggingService.Warn("Read dom: assembly changed since cache was created");
						result = null;
					}
					else
					{
						DomAssemblyName[] referencedAssemblies = new DomAssemblyName[this.reader.ReadInt32()];
						for (int i = 0; i < referencedAssemblies.Length; i++)
						{
							referencedAssemblies[i] = new DomAssemblyName(this.reader.ReadString());
						}
						this.pc = new ReflectionProjectContent(assemblyName, assemblyLocation, referencedAssemblies, registry);
						if (this.ReadClasses())
						{
							result = this.pc;
						}
						else
						{
							LoggingService.Warn("Read dom: error in file (invalid control mark)");
							result = null;
						}
					}
				}
				return result;
			}

			private void WriteClasses()
			{
				ICollection<IClass> classes = this.pc.Classes;
				this.classIndices.Clear();
				this.stringDict.Clear();
				int i = 0;
				foreach (IClass c in classes)
				{
					this.classIndices[new DomPersistence.ClassNameTypeCountPair(c)] = i;
					i++;
				}
				List<DomPersistence.ClassNameTypeCountPair> externalTypes = new List<DomPersistence.ClassNameTypeCountPair>();
				List<string> stringList = new List<string>();
				this.CreateExternalTypeList(externalTypes, stringList, classes.Count, classes);
				this.writer.Write(classes.Count);
				this.writer.Write(externalTypes.Count);
				foreach (IClass c in classes)
				{
					this.writer.Write(c.FullyQualifiedName);
				}
				foreach (DomPersistence.ClassNameTypeCountPair type in externalTypes)
				{
					this.writer.Write(type.ClassName);
					this.writer.Write(type.TypeParameterCount);
				}
				this.writer.Write(stringList.Count);
				foreach (string text in stringList)
				{
					this.writer.Write(text);
				}
				foreach (IClass c in classes)
				{
					this.WriteClass(c);
					this.writer.Write(64);
				}
			}

			private bool ReadClasses()
			{
				int classCount = this.reader.ReadInt32();
				int externalTypeCount = this.reader.ReadInt32();
				this.types = new IReturnType[classCount + externalTypeCount];
				DefaultClass[] classes = new DefaultClass[classCount];
				for (int i = 0; i < classes.Length; i++)
				{
					DefaultClass c = new DefaultClass(this.pc.AssemblyCompilationUnit, this.reader.ReadString());
					classes[i] = c;
					this.types[i] = c.DefaultReturnType;
				}
				for (int i = classCount; i < this.types.Length; i++)
				{
					string name = this.reader.ReadString();
					this.types[i] = new GetClassReturnType(this.pc, name, (int)this.reader.ReadByte());
				}
				this.stringArray = new string[this.reader.ReadInt32()];
				for (int i = 0; i < this.stringArray.Length; i++)
				{
					this.stringArray[i] = this.reader.ReadString();
				}
				bool result;
				for (int i = 0; i < classes.Length; i++)
				{
					this.ReadClass(classes[i]);
					this.pc.AddClassToNamespaceList(classes[i]);
					if (this.reader.ReadByte() != 64)
					{
						result = false;
						return result;
					}
				}
				result = true;
				return result;
			}

			private void WriteClass(IClass c)
			{
				this.currentClass = c;
				this.WriteTemplates(c.TypeParameters);
				this.writer.Write(c.BaseTypes.Count);
				foreach (IReturnType type in c.BaseTypes)
				{
					this.WriteType(type);
				}
				this.writer.Write((int)c.Modifiers);
				if (c is DefaultClass)
				{
					this.writer.Write(((DefaultClass)c).Flags);
				}
				else
				{
					this.writer.Write(0);
				}
				this.writer.Write((byte)c.ClassType);
				this.WriteAttributes(c.Attributes);
				this.writer.Write(c.InnerClasses.Count);
				foreach (IClass innerClass in c.InnerClasses)
				{
					this.writer.Write(innerClass.FullyQualifiedName);
					this.WriteClass(innerClass);
				}
				this.currentClass = c;
				this.writer.Write(c.Methods.Count);
				foreach (IMethod method in c.Methods)
				{
					this.WriteMethod(method);
				}
				this.writer.Write(c.Properties.Count);
				foreach (IProperty property in c.Properties)
				{
					this.WriteProperty(property);
				}
				this.writer.Write(c.Events.Count);
				foreach (IEvent evt in c.Events)
				{
					this.WriteEvent(evt);
				}
				this.writer.Write(c.Fields.Count);
				foreach (IField field in c.Fields)
				{
					this.WriteField(field);
				}
				this.currentClass = null;
			}

			private void WriteTemplates(IList<ITypeParameter> list)
			{
				this.writer.Write((byte)list.Count);
				foreach (ITypeParameter typeParameter in list)
				{
					this.WriteString(typeParameter.Name);
				}
				foreach (ITypeParameter typeParameter in list)
				{
					this.writer.Write(typeParameter.Constraints.Count);
					foreach (IReturnType type in typeParameter.Constraints)
					{
						this.WriteType(type);
					}
				}
			}

			private void ReadClass(DefaultClass c)
			{
				this.currentClass = c;
				int count = (int)this.reader.ReadByte();
				for (int i = 0; i < count; i++)
				{
					c.TypeParameters.Add(new DefaultTypeParameter(c, this.ReadString(), i));
				}
				if (count > 0)
				{
					foreach (ITypeParameter typeParameter in c.TypeParameters)
					{
						count = this.reader.ReadInt32();
						for (int i = 0; i < count; i++)
						{
							typeParameter.Constraints.Add(this.ReadType());
						}
					}
				}
				else
				{
					c.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
				}
				count = this.reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					c.BaseTypes.Add(this.ReadType());
				}
				c.Modifiers = (ModifierEnum)this.reader.ReadInt32();
				c.Flags = this.reader.ReadByte();
				c.ClassType = (ClassType)this.reader.ReadByte();
				this.ReadAttributes(c);
				count = this.reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					DefaultClass innerClass = new DefaultClass(c.CompilationUnit, c);
					innerClass.FullyQualifiedName = this.reader.ReadString();
					c.InnerClasses.Add(innerClass);
					this.ReadClass(innerClass);
				}
				this.currentClass = c;
				count = this.reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					c.Methods.Add(this.ReadMethod());
				}
				count = this.reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					c.Properties.Add(this.ReadProperty());
				}
				count = this.reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					c.Events.Add(this.ReadEvent());
				}
				count = this.reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					c.Fields.Add(this.ReadField());
				}
				this.currentClass = null;
			}

			private void CreateExternalTypeList(List<DomPersistence.ClassNameTypeCountPair> externalTypes, List<string> stringList, int classCount, ICollection<IClass> classes)
			{
				foreach (IClass c in classes)
				{
					this.CreateExternalTypeList(externalTypes, stringList, classCount, c.InnerClasses);
					this.AddStrings(stringList, c.Attributes);
					foreach (IReturnType returnType in c.BaseTypes)
					{
						this.AddExternalType(returnType, externalTypes, classCount);
					}
					foreach (ITypeParameter tp in c.TypeParameters)
					{
						this.AddString(stringList, tp.Name);
						foreach (IReturnType returnType in tp.Constraints)
						{
							this.AddExternalType(returnType, externalTypes, classCount);
						}
					}
					foreach (IField f in c.Fields)
					{
						this.CreateExternalTypeListMember(externalTypes, stringList, classCount, f);
					}
					foreach (IEvent f2 in c.Events)
					{
						this.CreateExternalTypeListMember(externalTypes, stringList, classCount, f2);
					}
					foreach (IProperty p in c.Properties)
					{
						this.CreateExternalTypeListMember(externalTypes, stringList, classCount, p);
						foreach (IParameter parameter in p.Parameters)
						{
							this.AddString(stringList, parameter.Name);
							this.AddStrings(stringList, parameter.Attributes);
							this.AddExternalType(parameter.ReturnType, externalTypes, classCount);
						}
					}
					foreach (IMethod i in c.Methods)
					{
						this.CreateExternalTypeListMember(externalTypes, stringList, classCount, i);
						foreach (IParameter parameter in i.Parameters)
						{
							this.AddString(stringList, parameter.Name);
							this.AddStrings(stringList, parameter.Attributes);
							this.AddExternalType(parameter.ReturnType, externalTypes, classCount);
						}
						foreach (ITypeParameter tp in i.TypeParameters)
						{
							this.AddString(stringList, tp.Name);
							foreach (IReturnType returnType in tp.Constraints)
							{
								this.AddExternalType(returnType, externalTypes, classCount);
							}
						}
					}
				}
			}

			private void CreateExternalTypeListMember(List<DomPersistence.ClassNameTypeCountPair> externalTypes, List<string> stringList, int classCount, IMember member)
			{
				this.AddString(stringList, member.Name);
				this.AddStrings(stringList, member.Attributes);
				foreach (ExplicitInterfaceImplementation eii in member.InterfaceImplementations)
				{
					this.AddString(stringList, eii.MemberName);
					this.AddExternalType(eii.InterfaceReference, externalTypes, classCount);
				}
				this.AddExternalType(member.ReturnType, externalTypes, classCount);
			}

			private void AddString(List<string> stringList, string text)
			{
				text = (text ?? string.Empty);
				if (!this.stringDict.ContainsKey(text))
				{
					this.stringDict.Add(text, stringList.Count);
					stringList.Add(text);
				}
			}

			private void AddExternalType(IReturnType rt, List<DomPersistence.ClassNameTypeCountPair> externalTypes, int classCount)
			{
				if (rt.IsDefaultReturnType)
				{
					DomPersistence.ClassNameTypeCountPair pair = new DomPersistence.ClassNameTypeCountPair(rt);
					if (!this.classIndices.ContainsKey(pair))
					{
						this.classIndices.Add(pair, externalTypes.Count + classCount);
						externalTypes.Add(pair);
					}
				}
				else if (!rt.IsGenericReturnType)
				{
					if (rt.IsArrayReturnType)
					{
						this.AddExternalType(rt.CastToArrayReturnType().ArrayElementType, externalTypes, classCount);
					}
					else if (rt.IsConstructedReturnType)
					{
						this.AddExternalType(rt.CastToConstructedReturnType().UnboundType, externalTypes, classCount);
						foreach (IReturnType typeArgument in rt.CastToConstructedReturnType().TypeArguments)
						{
							this.AddExternalType(typeArgument, externalTypes, classCount);
						}
					}
					else
					{
						LoggingService.Warn("Unknown return type: " + rt.ToString());
					}
				}
			}

			private void WriteType(IReturnType rt)
			{
				if (rt == null)
				{
					this.writer.Write(-5);
				}
				else if (rt.IsDefaultReturnType)
				{
					string name = rt.FullyQualifiedName;
					if (name == "System.Void")
					{
						this.writer.Write(-6);
					}
					else
					{
						this.writer.Write(this.classIndices[new DomPersistence.ClassNameTypeCountPair(rt)]);
					}
				}
				else if (rt.IsGenericReturnType)
				{
					GenericReturnType grt = rt.CastToGenericReturnType();
					if (grt.TypeParameter.Method != null)
					{
						this.writer.Write(-4);
					}
					else
					{
						this.writer.Write(-3);
					}
					this.writer.Write(grt.TypeParameter.Index);
				}
				else if (rt.IsArrayReturnType)
				{
					this.writer.Write(-1);
					this.writer.Write(rt.CastToArrayReturnType().ArrayDimensions);
					this.WriteType(rt.CastToArrayReturnType().ArrayElementType);
				}
				else if (rt.IsConstructedReturnType)
				{
					ConstructedReturnType crt = rt.CastToConstructedReturnType();
					this.writer.Write(-2);
					this.WriteType(crt.UnboundType);
					this.writer.Write((byte)crt.TypeArguments.Count);
					foreach (IReturnType typeArgument in crt.TypeArguments)
					{
						this.WriteType(typeArgument);
					}
				}
				else
				{
					this.writer.Write(-5);
					LoggingService.Warn("Unknown return type: " + rt.ToString());
				}
			}

			private IReturnType ReadType()
			{
				int index = this.reader.ReadInt32();
				IReturnType result;
				switch (index)
				{
				case -6:
					result = VoidReturnType.Instance;
					break;
				case -5:
					result = null;
					break;
				case -4:
					result = new GenericReturnType(this.currentMethod.TypeParameters[this.reader.ReadInt32()]);
					break;
				case -3:
					result = new GenericReturnType(this.currentClass.TypeParameters[this.reader.ReadInt32()]);
					break;
				case -2:
				{
					IReturnType baseType = this.ReadType();
					IReturnType[] typeArguments = new IReturnType[(int)this.reader.ReadByte()];
					for (int i = 0; i < typeArguments.Length; i++)
					{
						typeArguments[i] = this.ReadType();
					}
					result = new ConstructedReturnType(baseType, typeArguments);
					break;
				}
				case -1:
				{
					int dimensions = this.reader.ReadInt32();
					result = new ArrayReturnType(this.pc, this.ReadType(), dimensions);
					break;
				}
				default:
					result = this.types[index];
					break;
				}
				return result;
			}

			private void WriteString(string text)
			{
				text = (text ?? string.Empty);
				this.writer.Write(this.stringDict[text]);
			}

			private string ReadString()
			{
				return this.stringArray[this.reader.ReadInt32()];
			}

			private void WriteMember(IMember m)
			{
				this.WriteString(m.Name);
				this.writer.Write((int)m.Modifiers);
				this.WriteAttributes(m.Attributes);
				this.writer.Write((ushort)m.InterfaceImplementations.Count);
				foreach (ExplicitInterfaceImplementation iee in m.InterfaceImplementations)
				{
					this.WriteType(iee.InterfaceReference);
					this.WriteString(iee.MemberName);
				}
				if (!(m is IMethod))
				{
					this.WriteType(m.ReturnType);
				}
			}

			private void ReadMember(AbstractMember m)
			{
				m.Modifiers = (ModifierEnum)this.reader.ReadInt32();
				this.ReadAttributes(m);
				int interfaceImplCount = (int)this.reader.ReadUInt16();
				for (int i = 0; i < interfaceImplCount; i++)
				{
					m.InterfaceImplementations.Add(new ExplicitInterfaceImplementation(this.ReadType(), this.ReadString()));
				}
				if (!(m is IMethod))
				{
					m.ReturnType = this.ReadType();
				}
			}

			private void WriteAttributes(IList<IAttribute> attributes)
			{
				this.writer.Write((ushort)attributes.Count);
				foreach (IAttribute a in attributes)
				{
					this.WriteString(a.Name);
					this.writer.Write((byte)a.AttributeTarget);
				}
			}

			private void AddStrings(List<string> stringList, IList<IAttribute> attributes)
			{
				foreach (IAttribute a in attributes)
				{
					this.AddString(stringList, a.Name);
				}
			}

			private void ReadAttributes(DefaultParameter parameter)
			{
				int count = (int)this.reader.ReadUInt16();
				if (count > 0)
				{
					this.ReadAttributes(parameter.Attributes, count);
				}
				else
				{
					parameter.Attributes = DefaultAttribute.EmptyAttributeList;
				}
			}

			private void ReadAttributes(AbstractDecoration decoration)
			{
				int count = (int)this.reader.ReadUInt16();
				if (count > 0)
				{
					this.ReadAttributes(decoration.Attributes, count);
				}
				else
				{
					decoration.Attributes = DefaultAttribute.EmptyAttributeList;
				}
			}

			private void ReadAttributes(IList<IAttribute> attributes, int count)
			{
				for (int i = 0; i < count; i++)
				{
					string name = this.ReadString();
					attributes.Add(new DefaultAttribute(name, (AttributeTarget)this.reader.ReadByte()));
				}
			}

			private void WriteParameters(IList<IParameter> parameters)
			{
				this.writer.Write((ushort)parameters.Count);
				foreach (IParameter p in parameters)
				{
					this.WriteString(p.Name);
					this.WriteType(p.ReturnType);
					this.writer.Write((byte)p.Modifiers);
					this.WriteAttributes(p.Attributes);
				}
			}

			private void ReadParameters(DefaultMethod m)
			{
				int count = (int)this.reader.ReadUInt16();
				if (count > 0)
				{
					this.ReadParameters(m.Parameters, count);
				}
				else
				{
					m.Parameters = DefaultParameter.EmptyParameterList;
				}
			}

			private void ReadParameters(DefaultProperty m)
			{
				int count = (int)this.reader.ReadUInt16();
				if (count > 0)
				{
					this.ReadParameters(m.Parameters, count);
				}
				else
				{
					m.Parameters = DefaultParameter.EmptyParameterList;
				}
			}

			private void ReadParameters(IList<IParameter> parameters, int count)
			{
				for (int i = 0; i < count; i++)
				{
					string name = this.ReadString();
					DefaultParameter p = new DefaultParameter(name, this.ReadType(), DomRegion.Empty);
					p.Modifiers = (ParameterModifiers)this.reader.ReadByte();
					this.ReadAttributes(p);
					parameters.Add(p);
				}
			}

			private void WriteMethod(IMethod m)
			{
				this.currentMethod = m;
				this.WriteMember(m);
				this.WriteTemplates(m.TypeParameters);
				this.WriteType(m.ReturnType);
				this.writer.Write(m.IsExtensionMethod);
				this.WriteParameters(m.Parameters);
				this.currentMethod = null;
			}

			private IMethod ReadMethod()
			{
				DefaultMethod i = new DefaultMethod(this.currentClass, this.ReadString());
				this.currentMethod = i;
				this.ReadMember(i);
				int count = (int)this.reader.ReadByte();
				for (int j = 0; j < count; j++)
				{
					i.TypeParameters.Add(new DefaultTypeParameter(i, this.ReadString(), j));
				}
				if (count > 0)
				{
					foreach (ITypeParameter typeParameter in i.TypeParameters)
					{
						count = this.reader.ReadInt32();
						for (int j = 0; j < count; j++)
						{
							typeParameter.Constraints.Add(this.ReadType());
						}
					}
				}
				else
				{
					i.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
				}
				i.ReturnType = this.ReadType();
				i.IsExtensionMethod = this.reader.ReadBoolean();
				this.ReadParameters(i);
				this.currentMethod = null;
				return i;
			}

			private void WriteProperty(IProperty p)
			{
				this.WriteMember(p);
				DefaultProperty dp = p as DefaultProperty;
				if (dp != null)
				{
					this.writer.Write(dp.accessFlags);
				}
				else
				{
					this.writer.Write(0);
				}
				this.WriteParameters(p.Parameters);
			}

			private IProperty ReadProperty()
			{
				DefaultProperty p = new DefaultProperty(this.currentClass, this.ReadString());
				this.ReadMember(p);
				p.accessFlags = this.reader.ReadByte();
				this.ReadParameters(p);
				return p;
			}

			private void WriteEvent(IEvent p)
			{
				this.WriteMember(p);
			}

			private IEvent ReadEvent()
			{
				DefaultEvent p = new DefaultEvent(this.currentClass, this.ReadString());
				this.ReadMember(p);
				return p;
			}

			private void WriteField(IField p)
			{
				this.WriteMember(p);
			}

			private IField ReadField()
			{
				DefaultField p = new DefaultField(this.currentClass, this.ReadString());
				this.ReadMember(p);
				return p;
			}
		}

		public const long FileMagic = 1252935504315237004L;

		public const long IndexFileMagic = 1252935504315236989L;

		public const short FileVersion = 11;

		private ProjectContentRegistry registry;

		private string cacheDirectory;

		private Dictionary<string, string> cacheIndex;

		internal string CacheDirectory
		{
			get
			{
				return this.cacheDirectory;
			}
		}

		private Dictionary<string, string> CacheIndex
		{
			get
			{
				return this.cacheIndex;
			}
		}

		internal DomPersistence(string cacheDirectory, ProjectContentRegistry registry)
		{
			this.cacheDirectory = cacheDirectory;
			this.registry = registry;
			this.cacheIndex = this.LoadCacheIndex();
		}

		public string SaveProjectContent(ReflectionProjectContent pc)
		{
			string assemblyFullName = pc.AssemblyFullName;
			int pos = assemblyFullName.IndexOf(',');
			string fileName = Path.Combine(this.cacheDirectory, string.Concat(new string[]
			{
				assemblyFullName.Substring(0, pos),
				".",
				assemblyFullName.GetHashCode().ToString("x", CultureInfo.InvariantCulture),
				".",
				pc.AssemblyLocation.GetHashCode().ToString("x", CultureInfo.InvariantCulture),
				".dat"
			}));
			this.AddFileNameToCacheIndex(Path.GetFileName(fileName), pc);
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				DomPersistence.WriteProjectContent(pc, fs);
			}
			return fileName;
		}

		public ReflectionProjectContent LoadProjectContentByAssemblyName(string assemblyName)
		{
			string cacheFileName;
			ReflectionProjectContent result;
			if (this.CacheIndex.TryGetValue(assemblyName, out cacheFileName))
			{
				cacheFileName = Path.Combine(this.cacheDirectory, cacheFileName);
				if (File.Exists(cacheFileName))
				{
					result = this.LoadProjectContent(cacheFileName);
					return result;
				}
			}
			result = null;
			return result;
		}

		public ReflectionProjectContent LoadProjectContent(string cacheFileName)
		{
			ReflectionProjectContent result;
			using (FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read))
			{
				result = this.LoadProjectContent(fs);
			}
			return result;
		}

		private string GetIndexFileName()
		{
			return Path.Combine(this.cacheDirectory, "index.dat");
		}

		private Dictionary<string, string> LoadCacheIndex()
		{
			string indexFile = this.GetIndexFileName();
			Dictionary<string, string> list = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			Dictionary<string, string> result;
			if (File.Exists(indexFile))
			{
				try
				{
					using (FileStream fs = new FileStream(indexFile, FileMode.Open, FileAccess.Read))
					{
						using (BinaryReader reader = new BinaryReader(fs))
						{
							if (reader.ReadInt64() != 1252935504315236989L)
							{
								LoggingService.Warn("Index cache has wrong file magic");
								result = list;
								return result;
							}
							if (reader.ReadInt16() != 11)
							{
								LoggingService.Warn("Index cache has wrong file version");
								result = list;
								return result;
							}
							int count = reader.ReadInt32();
							for (int i = 0; i < count; i++)
							{
								string key = reader.ReadString();
								list[key] = reader.ReadString();
							}
						}
					}
				}
				catch (IOException ex)
				{
					LoggingService.Warn("Error reading DomPersistance cache index", ex);
				}
			}
			result = list;
			return result;
		}

		private void SaveCacheIndex(Dictionary<string, string> cacheIndex)
		{
			string indexFile = this.GetIndexFileName();
			using (FileStream fs = new FileStream(indexFile, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter writer = new BinaryWriter(fs))
				{
					writer.Write(1252935504315236989L);
					writer.Write(11);
					writer.Write(cacheIndex.Count);
					foreach (KeyValuePair<string, string> e in cacheIndex)
					{
						writer.Write(e.Key);
						writer.Write(e.Value);
					}
				}
			}
		}

		private void AddFileNameToCacheIndex(string cacheFile, ReflectionProjectContent pc)
		{
			Dictionary<string, string> i = this.LoadCacheIndex();
			i[pc.AssemblyLocation] = cacheFile;
			string txt = pc.AssemblyFullName;
			i[txt] = cacheFile;
			int pos = txt.LastIndexOf(',');
			do
			{
				txt = txt.Substring(0, pos);
				if (i.ContainsKey(txt))
				{
					break;
				}
				i[txt] = cacheFile;
				pos = txt.LastIndexOf(',');
			}
			while (pos >= 0);
			this.SaveCacheIndex(i);
			this.cacheIndex = i;
		}

		public static void WriteProjectContent(ReflectionProjectContent pc, Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			new DomPersistence.ReadWriteHelper(writer).WriteProjectContent(pc);
		}

		public ReflectionProjectContent LoadProjectContent(Stream stream)
		{
			return DomPersistence.LoadProjectContent(stream, this.registry);
		}

		public static ReflectionProjectContent LoadProjectContent(Stream stream, ProjectContentRegistry registry)
		{
			BinaryReader reader = new BinaryReader(stream);
			ReflectionProjectContent result;
			try
			{
				ReflectionProjectContent pc = new DomPersistence.ReadWriteHelper(reader).ReadProjectContent(registry);
				if (pc != null)
				{
					pc.InitializeSpecialClasses();
				}
				result = pc;
			}
			catch (EndOfStreamException)
			{
				LoggingService.Warn("Read dom: EndOfStreamException");
				result = null;
			}
			return result;
		}
	}
}
