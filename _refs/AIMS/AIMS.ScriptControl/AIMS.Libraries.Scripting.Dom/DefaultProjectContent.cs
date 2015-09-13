using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace AIMS.Libraries.Scripting.Dom
{
	public class DefaultProjectContent : IProjectContent, IDisposable
	{
		protected struct NamespaceStruct
		{
			public readonly List<IClass> Classes;

			public readonly List<string> SubNamespaces;

			public NamespaceStruct(string name)
			{
				this.Classes = new List<IClass>();
				this.SubNamespaces = new List<string>();
			}
		}

		private class GenericClassContainer : DefaultClass
		{
			private IClass[] realClasses = new IClass[4];

			public IEnumerable<IClass> RealClasses
			{
				get
				{
					DefaultProjectContent.GenericClassContainer.<get_RealClasses>d__0 <get_RealClasses>d__ = new DefaultProjectContent.GenericClassContainer.<get_RealClasses>d__0(-2);
					<get_RealClasses>d__.<>4__this = this;
					return <get_RealClasses>d__;
				}
			}

			public int RealClassCount
			{
				get
				{
					int count = 0;
					IClass[] array = this.realClasses;
					for (int i = 0; i < array.Length; i++)
					{
						IClass c = array[i];
						if (c != null)
						{
							count++;
						}
					}
					return count;
				}
			}

			public GenericClassContainer(string fullyQualifiedName) : base(null, fullyQualifiedName)
			{
			}

			public IClass Get(int typeParameterCount)
			{
				IClass result;
				if (this.realClasses.Length > typeParameterCount)
				{
					result = this.realClasses[typeParameterCount];
				}
				else
				{
					result = null;
				}
				return result;
			}

			public IClass GetBest(int typeParameterCount)
			{
				IClass result;
				for (int i = typeParameterCount; i < this.realClasses.Length; i++)
				{
					IClass c = this.Get(i);
					if (c != null)
					{
						result = c;
						return result;
					}
				}
				for (int i = typeParameterCount - 1; i >= 0; i--)
				{
					IClass c = this.Get(i);
					if (c != null)
					{
						result = c;
						return result;
					}
				}
				result = null;
				return result;
			}

			public void Set(IClass c)
			{
				int typeParameterCount = c.TypeParameters.Count;
				if (this.realClasses.Length <= typeParameterCount)
				{
					IClass[] newArray = new IClass[typeParameterCount + 2];
					this.realClasses.CopyTo(newArray, 0);
					this.realClasses = newArray;
				}
				this.realClasses[typeParameterCount] = c;
			}

			public void Remove(int typeParameterCount)
			{
				if (this.realClasses.Length > typeParameterCount)
				{
					this.realClasses[typeParameterCount] = null;
				}
			}
		}

		private class DummyContent : DefaultProjectContent
		{
			public override SystemTypes SystemTypes
			{
				get
				{
					return HostCallback.GetCurrentProjectContent().SystemTypes;
				}
			}

			public override string ToString()
			{
				return "[DummyProjectContent]";
			}
		}

		private readonly List<IProjectContent> referencedContents = new List<IProjectContent>();

		private List<Dictionary<string, IClass>> classLists = new List<Dictionary<string, IClass>>();

		private List<Dictionary<string, DefaultProjectContent.NamespaceStruct>> namespaces = new List<Dictionary<string, DefaultProjectContent.NamespaceStruct>>();

		private XmlDoc xmlDoc = new XmlDoc();

		private IUsing defaultImports;

		private SystemTypes systemTypes;

		private LanguageProperties language = LanguageProperties.CSharp;

		public static readonly IProjectContent DummyProjectContent = new DefaultProjectContent.DummyContent();

		public event EventHandler ReferencedContentsChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ReferencedContentsChanged = (EventHandler)Delegate.Combine(this.ReferencedContentsChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ReferencedContentsChanged = (EventHandler)Delegate.Remove(this.ReferencedContentsChanged, value);
			}
		}

		public IUsing DefaultImports
		{
			get
			{
				return this.defaultImports;
			}
			set
			{
				this.defaultImports = value;
			}
		}

		public virtual object Project
		{
			get
			{
				return null;
			}
		}

		public virtual bool IsUpToDate
		{
			get
			{
				return true;
			}
		}

		public List<Dictionary<string, IClass>> ClassLists
		{
			get
			{
				if (this.classLists.Count == 0)
				{
					this.classLists.Add(new Dictionary<string, IClass>(this.language.NameComparer));
				}
				return this.classLists;
			}
		}

		public ICollection<string> NamespaceNames
		{
			get
			{
				return this.Namespaces[0].Keys;
			}
		}

		protected List<Dictionary<string, DefaultProjectContent.NamespaceStruct>> Namespaces
		{
			get
			{
				if (this.namespaces.Count == 0)
				{
					this.namespaces.Add(new Dictionary<string, DefaultProjectContent.NamespaceStruct>(this.language.NameComparer));
				}
				return this.namespaces;
			}
		}

		public XmlDoc XmlDoc
		{
			get
			{
				return this.xmlDoc;
			}
			protected set
			{
				this.xmlDoc = value;
			}
		}

		public ICollection<IClass> Classes
		{
			get
			{
				ICollection<IClass> result;
				lock (this.namespaces)
				{
					List<IClass> list = new List<IClass>(this.ClassLists[0].Count + 10);
					foreach (IClass c in this.ClassLists[0].Values)
					{
						if (c is DefaultProjectContent.GenericClassContainer)
						{
							DefaultProjectContent.GenericClassContainer gcc = (DefaultProjectContent.GenericClassContainer)c;
							list.AddRange(gcc.RealClasses);
						}
						else
						{
							list.Add(c);
						}
					}
					result = list;
				}
				return result;
			}
		}

		public virtual SystemTypes SystemTypes
		{
			get
			{
				if (this.systemTypes == null)
				{
					this.systemTypes = new SystemTypes(this);
				}
				return this.systemTypes;
			}
		}

		public ICollection<IProjectContent> ReferencedContents
		{
			get
			{
				return this.referencedContents;
			}
		}

		public LanguageProperties Language
		{
			[DebuggerStepThrough]
			get
			{
				return this.language;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.language = value;
			}
		}

		protected Dictionary<string, IClass> GetClasses(LanguageProperties language)
		{
			Dictionary<string, IClass> result;
			for (int i = 0; i < this.classLists.Count; i++)
			{
				if (this.classLists[i].Comparer == language.NameComparer)
				{
					result = this.classLists[i];
					return result;
				}
			}
			Dictionary<string, IClass> d;
			if (this.classLists.Count > 0)
			{
				Dictionary<string, IClass> oldList = this.classLists[0];
				d = new Dictionary<string, IClass>(oldList.Count, language.NameComparer);
				foreach (KeyValuePair<string, IClass> pair in oldList)
				{
					d.Add(pair.Key, pair.Value);
				}
			}
			else
			{
				d = new Dictionary<string, IClass>(language.NameComparer);
			}
			this.classLists.Add(d);
			result = d;
			return result;
		}

		protected Dictionary<string, DefaultProjectContent.NamespaceStruct> GetNamespaces(LanguageProperties language)
		{
			Dictionary<string, DefaultProjectContent.NamespaceStruct> result;
			for (int i = 0; i < this.namespaces.Count; i++)
			{
				if (this.namespaces[i].Comparer == language.NameComparer)
				{
					result = this.namespaces[i];
					return result;
				}
			}
			Dictionary<string, DefaultProjectContent.NamespaceStruct> d;
			if (this.namespaces.Count > 0)
			{
				Dictionary<string, DefaultProjectContent.NamespaceStruct> oldList = this.namespaces[0];
				d = new Dictionary<string, DefaultProjectContent.NamespaceStruct>(oldList.Count, language.NameComparer);
				foreach (KeyValuePair<string, DefaultProjectContent.NamespaceStruct> pair in oldList)
				{
					d.Add(pair.Key, pair.Value);
				}
			}
			else
			{
				d = new Dictionary<string, DefaultProjectContent.NamespaceStruct>(language.NameComparer);
			}
			this.namespaces.Add(d);
			result = d;
			return result;
		}

		public string GetXmlDocumentation(string memberTag)
		{
			string desc = this.xmlDoc.GetDocumentation(memberTag);
			string result;
			if (desc != null)
			{
				result = desc;
			}
			else
			{
				lock (this.referencedContents)
				{
					foreach (IProjectContent referencedContent in this.referencedContents)
					{
						desc = referencedContent.XmlDoc.GetDocumentation(memberTag);
						if (desc != null)
						{
							result = desc;
							return result;
						}
					}
				}
				result = null;
			}
			return result;
		}

		protected static string LookupLocalizedXmlDoc(string fileName)
		{
			string xmlFileName = Path.ChangeExtension(fileName, ".xml");
			string localizedXmlDocFile = Path.GetDirectoryName(fileName);
			localizedXmlDocFile = Path.Combine(localizedXmlDocFile, Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
			localizedXmlDocFile = Path.Combine(localizedXmlDocFile, Path.GetFileName(xmlFileName));
			string result;
			if (File.Exists(localizedXmlDocFile))
			{
				result = localizedXmlDocFile;
			}
			else if (File.Exists(xmlFileName))
			{
				result = xmlFileName;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public virtual void Dispose()
		{
			this.xmlDoc.Dispose();
		}

		public void AddClassToNamespaceList(IClass addClass)
		{
			lock (this.namespaces)
			{
				this.AddClassToNamespaceListInternal(addClass);
			}
			SearchClassReturnType.ClearCache();
		}

		protected void AddClassToNamespaceListInternal(IClass addClass)
		{
			string fullyQualifiedName = addClass.FullyQualifiedName;
			IClass existingClass = this.GetClassInternal(fullyQualifiedName, addClass.TypeParameters.Count, this.language);
			if (existingClass != null && existingClass.TypeParameters.Count == addClass.TypeParameters.Count)
			{
				CompoundClass compound = existingClass as CompoundClass;
				if (compound != null)
				{
					addClass.IsPartial = true;
					lock (compound)
					{
						for (int i = 0; i < compound.parts.Count; i++)
						{
							if (compound.parts[i].CompilationUnit.FileName == addClass.CompilationUnit.FileName)
							{
								compound.parts[i] = addClass;
								compound.UpdateInformationFromParts();
								return;
							}
						}
						compound.parts.Add(addClass);
						compound.UpdateInformationFromParts();
					}
					return;
				}
				if (addClass.IsPartial || this.language.ImplicitPartialClasses)
				{
					addClass.IsPartial = true;
					existingClass.IsPartial = true;
					compound = (addClass = new CompoundClass(addClass));
					compound.parts.Add(existingClass);
					compound.UpdateInformationFromParts();
				}
			}
			else if (addClass.IsPartial)
			{
				addClass = new CompoundClass(addClass);
			}
			IClass oldDictionaryClass;
			if (this.GetClasses(this.language).TryGetValue(fullyQualifiedName, out oldDictionaryClass))
			{
				DefaultProjectContent.GenericClassContainer gcc = oldDictionaryClass as DefaultProjectContent.GenericClassContainer;
				if (gcc != null)
				{
					gcc.Set(addClass);
					return;
				}
				if (oldDictionaryClass.TypeParameters.Count != addClass.TypeParameters.Count)
				{
					gcc = new DefaultProjectContent.GenericClassContainer(fullyQualifiedName);
					gcc.Set(addClass);
					gcc.Set(oldDictionaryClass);
					addClass = gcc;
				}
			}
			foreach (Dictionary<string, IClass> classes in this.ClassLists)
			{
				classes[addClass.FullyQualifiedName] = addClass;
			}
			string nSpace = addClass.Namespace;
			if (nSpace == null)
			{
				nSpace = string.Empty;
			}
			this.CreateNamespace(nSpace);
			List<IClass> classList = this.GetNamespaces(this.language)[nSpace].Classes;
			for (int i = 0; i < classList.Count; i++)
			{
				if (classList[i].FullyQualifiedName == addClass.FullyQualifiedName)
				{
					classList[i] = addClass;
					return;
				}
			}
			classList.Add(addClass);
		}

		private void CreateNamespace(string nSpace)
		{
			Dictionary<string, DefaultProjectContent.NamespaceStruct> dict = this.GetNamespaces(this.language);
			if (!dict.ContainsKey(nSpace))
			{
				DefaultProjectContent.NamespaceStruct namespaceStruct = new DefaultProjectContent.NamespaceStruct(nSpace);
				dict.Add(nSpace, namespaceStruct);
				foreach (Dictionary<string, DefaultProjectContent.NamespaceStruct> otherDict in this.namespaces)
				{
					if (otherDict != dict)
					{
						otherDict.Add(nSpace, namespaceStruct);
					}
				}
				if (nSpace.Length != 0)
				{
					int pos = nSpace.LastIndexOf('.');
					string parent;
					string subNs;
					if (pos < 0)
					{
						parent = "";
						subNs = nSpace;
					}
					else
					{
						parent = nSpace.Substring(0, pos);
						subNs = nSpace.Substring(pos + 1);
					}
					this.CreateNamespace(parent);
					dict[parent].SubNamespaces.Add(subNs);
				}
			}
		}

		private void RemoveEmptyNamespace(string nSpace)
		{
			if (nSpace != null && nSpace.Length != 0)
			{
				Dictionary<string, DefaultProjectContent.NamespaceStruct> dict = this.GetNamespaces(this.language);
				if (dict.ContainsKey(nSpace))
				{
					if (dict[nSpace].Classes.Count <= 0 && dict[nSpace].SubNamespaces.Count <= 0)
					{
						foreach (Dictionary<string, DefaultProjectContent.NamespaceStruct> anyDict in this.namespaces)
						{
							anyDict.Remove(nSpace);
						}
						int pos = nSpace.LastIndexOf('.');
						string parent;
						string subNs;
						if (pos < 0)
						{
							parent = "";
							subNs = nSpace;
						}
						else
						{
							parent = nSpace.Substring(0, pos);
							subNs = nSpace.Substring(pos + 1);
						}
						dict[parent].SubNamespaces.Remove(subNs);
						this.RemoveEmptyNamespace(parent);
					}
				}
			}
		}

		public void RemoveCompilationUnit(ICompilationUnit unit)
		{
			lock (this.namespaces)
			{
				foreach (IClass c in unit.Classes)
				{
					this.RemoveClass(c);
				}
			}
			SearchClassReturnType.ClearCache();
		}

		public void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName)
		{
			lock (this.namespaces)
			{
				if (oldUnit != null)
				{
					this.RemoveClasses(oldUnit, parserOutput);
				}
				foreach (IClass c in parserOutput.Classes)
				{
					this.AddClassToNamespaceListInternal(c);
				}
			}
			SearchClassReturnType.ClearCache();
		}

		private void RemoveClasses(ICompilationUnit oldUnit, ICompilationUnit newUnit)
		{
			foreach (IClass c in oldUnit.Classes)
			{
				bool found = false;
				if (!c.IsPartial)
				{
					foreach (IClass c2 in newUnit.Classes)
					{
						if (c.FullyQualifiedName == c2.FullyQualifiedName)
						{
							found = true;
							break;
						}
					}
				}
				if (!found)
				{
					this.RemoveClass(c);
				}
			}
		}

		private void RemoveClass(IClass @class)
		{
			string fullyQualifiedName = @class.FullyQualifiedName;
			int typeParameterCount = @class.TypeParameters.Count;
			if (@class.IsPartial)
			{
				CompoundClass compound = this.GetClassInternal(fullyQualifiedName, typeParameterCount, this.language) as CompoundClass;
				if (compound == null)
				{
					return;
				}
				typeParameterCount = compound.TypeParameters.Count;
				lock (compound)
				{
					compound.parts.Remove(@class);
					if (compound.parts.Count > 0)
					{
						compound.UpdateInformationFromParts();
						return;
					}
					@class = compound;
				}
			}
			IClass classInDictionary;
			if (this.GetClasses(this.language).TryGetValue(fullyQualifiedName, out classInDictionary))
			{
				DefaultProjectContent.GenericClassContainer gcc = classInDictionary as DefaultProjectContent.GenericClassContainer;
				if (gcc != null)
				{
					gcc.Remove(typeParameterCount);
					if (gcc.RealClassCount > 0)
					{
						return;
					}
				}
				foreach (Dictionary<string, IClass> classes in this.ClassLists)
				{
					classes.Remove(fullyQualifiedName);
				}
				string nSpace = @class.Namespace;
				if (nSpace == null)
				{
					nSpace = string.Empty;
				}
				List<IClass> classList = this.GetNamespaces(this.language)[nSpace].Classes;
				for (int i = 0; i < classList.Count; i++)
				{
					if (this.language.NameComparer.Equals(classList[i].FullyQualifiedName, fullyQualifiedName))
					{
						classList.RemoveAt(i);
						break;
					}
				}
				if (classList.Count == 0)
				{
					this.RemoveEmptyNamespace(nSpace);
				}
			}
		}

		public IClass GetClass(string typeName)
		{
			return this.GetClass(typeName, 0);
		}

		public IClass GetClass(string typeName, int typeParameterCount)
		{
			return this.GetClass(typeName, typeParameterCount, this.language, true);
		}

		protected IClass GetClassInternal(string typeName, int typeParameterCount, LanguageProperties language)
		{
			IClass result;
			lock (this.namespaces)
			{
				IClass c;
				if (this.GetClasses(language).TryGetValue(typeName, out c))
				{
					DefaultProjectContent.GenericClassContainer gcc = c as DefaultProjectContent.GenericClassContainer;
					if (gcc != null)
					{
						result = gcc.GetBest(typeParameterCount);
					}
					else
					{
						result = c;
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, bool lookInReferences)
		{
			IClass c = this.GetClassInternal(typeName, typeParameterCount, language);
			IClass result;
			if (c != null && c.TypeParameters.Count == typeParameterCount)
			{
				result = c;
			}
			else
			{
				if (lookInReferences)
				{
					lock (this.referencedContents)
					{
						foreach (IProjectContent content in this.referencedContents)
						{
							IClass contentClass = content.GetClass(typeName, typeParameterCount, language, false);
							if (contentClass != null)
							{
								if (contentClass.TypeParameters.Count == typeParameterCount)
								{
									result = contentClass;
									return result;
								}
								c = contentClass;
							}
						}
					}
				}
				if (c != null)
				{
					result = c;
				}
				else
				{
					int lastIndex = typeName.LastIndexOf('.');
					if (lastIndex > 0)
					{
						string outerName = typeName.Substring(0, lastIndex);
						IClass upperClass = this.GetClassInternal(outerName, typeParameterCount, language);
						if (upperClass != null)
						{
							foreach (IClass upperBaseClass in upperClass.ClassInheritanceTree)
							{
								List<IClass> innerClasses = upperBaseClass.InnerClasses;
								if (innerClasses != null)
								{
									string innerName = typeName.Substring(lastIndex + 1);
									foreach (IClass innerClass in innerClasses)
									{
										if (language.NameComparer.Equals(innerClass.Name, innerName))
										{
											result = innerClass;
											return result;
										}
									}
								}
							}
						}
					}
					result = null;
				}
			}
			return result;
		}

		public ArrayList GetNamespaceContents(string nameSpace)
		{
			ArrayList namespaceList = new ArrayList();
			this.AddNamespaceContents(namespaceList, nameSpace, this.language, true);
			return namespaceList;
		}

		public void AddNamespaceContents(ArrayList list, string nameSpace, LanguageProperties language, bool lookInReferences)
		{
			if (nameSpace != null)
			{
				if (lookInReferences)
				{
					lock (this.referencedContents)
					{
						foreach (IProjectContent content in this.referencedContents)
						{
							if (content != null)
							{
								content.AddNamespaceContents(list, nameSpace, language, false);
							}
						}
					}
				}
				Dictionary<string, DefaultProjectContent.NamespaceStruct> dict = this.GetNamespaces(language);
				if (dict.ContainsKey(nameSpace))
				{
					DefaultProjectContent.NamespaceStruct ns = dict[nameSpace];
					int newCapacity = list.Count + ns.Classes.Count + ns.SubNamespaces.Count;
					if (list.Capacity < newCapacity)
					{
						list.Capacity = Math.Max(list.Count * 2, newCapacity);
					}
					foreach (IClass c in ns.Classes)
					{
						if (c is DefaultProjectContent.GenericClassContainer)
						{
							foreach (IClass realClass in ((DefaultProjectContent.GenericClassContainer)c).RealClasses)
							{
								this.AddNamespaceContentsClass(list, realClass, language, lookInReferences);
							}
						}
						else
						{
							this.AddNamespaceContentsClass(list, c, language, lookInReferences);
						}
					}
					foreach (string subns in ns.SubNamespaces)
					{
						if (!list.Contains(subns))
						{
							list.Add(subns);
						}
					}
				}
			}
		}

		private void AddNamespaceContentsClass(ArrayList list, IClass c, LanguageProperties language, bool lookInReferences)
		{
			if (!c.IsInternal || lookInReferences)
			{
				if (language.ShowInNamespaceCompletion(c))
				{
					list.Add(c);
				}
				if (language.ImportModules && c.ClassType == ClassType.Module)
				{
					foreach (IMember i in c.Methods)
					{
						if (i.IsAccessible(null, false))
						{
							list.Add(i);
						}
					}
					foreach (IMember i in c.Events)
					{
						if (i.IsAccessible(null, false))
						{
							list.Add(i);
						}
					}
					foreach (IMember i in c.Fields)
					{
						if (i.IsAccessible(null, false))
						{
							list.Add(i);
						}
					}
					foreach (IMember i in c.Properties)
					{
						if (i.IsAccessible(null, false))
						{
							list.Add(i);
						}
					}
				}
			}
		}

		public bool NamespaceExists(string name)
		{
			return this.NamespaceExists(name, this.language, true);
		}

		public bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences)
		{
			bool result;
			if (name == null)
			{
				result = false;
			}
			else
			{
				if (lookInReferences)
				{
					lock (this.referencedContents)
					{
						foreach (IProjectContent content in this.referencedContents)
						{
							if (content.NamespaceExists(name, language, false))
							{
								result = true;
								return result;
							}
						}
					}
				}
				result = this.GetNamespaces(language).ContainsKey(name);
			}
			return result;
		}

		public string SearchNamespace(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn)
		{
			string result;
			if (this.NamespaceExists(name))
			{
				result = name;
			}
			else if (unit == null)
			{
				result = null;
			}
			else
			{
				foreach (IUsing u in unit.Usings)
				{
					if (u != null)
					{
						string nameSpace = u.SearchNamespace(name);
						if (nameSpace != null)
						{
							result = nameSpace;
							return result;
						}
					}
				}
				if (this.defaultImports != null)
				{
					string nameSpace = this.defaultImports.SearchNamespace(name);
					if (nameSpace != null)
					{
						result = nameSpace;
						return result;
					}
				}
				if (curType != null)
				{
					string fullname = curType.Namespace;
					while (fullname != null && fullname.Length > 0)
					{
						string nameSpace = fullname + '.' + name;
						if (this.NamespaceExists(nameSpace))
						{
							result = nameSpace;
							return result;
						}
						int pos = fullname.LastIndexOf('.');
						if (pos < 0)
						{
							fullname = null;
						}
						else
						{
							fullname = fullname.Substring(0, pos);
						}
					}
				}
				result = null;
			}
			return result;
		}

		public SearchTypeResult SearchType(SearchTypeRequest request)
		{
			string name = request.Name;
			SearchTypeResult result;
			if (name == null || name.Length == 0)
			{
				result = SearchTypeResult.Empty;
			}
			else
			{
				IClass c = this.GetClass(name, request.TypeParameterCount);
				if (c != null)
				{
					result = new SearchTypeResult(c.DefaultReturnType);
				}
				else
				{
					SearchTypeResult fallbackResult = SearchTypeResult.Empty;
					if (request.CurrentType != null)
					{
						string fullname = request.CurrentType.Namespace;
						while (fullname != null && fullname.Length > 0)
						{
							string nameSpace = fullname + '.' + name;
							c = this.GetClass(nameSpace, request.TypeParameterCount);
							if (c != null)
							{
								if (c.TypeParameters.Count == request.TypeParameterCount)
								{
									result = new SearchTypeResult(c.DefaultReturnType);
									return result;
								}
								fallbackResult = new SearchTypeResult(c.DefaultReturnType);
							}
							int pos = fullname.LastIndexOf('.');
							if (pos < 0)
							{
								fullname = null;
							}
							else
							{
								fullname = fullname.Substring(0, pos);
							}
						}
						if (name.IndexOf('.') < 0)
						{
							foreach (IClass baseClass in request.CurrentType.ClassInheritanceTree)
							{
								if (baseClass.ClassType == ClassType.Class)
								{
									foreach (IClass innerClass in baseClass.InnerClasses)
									{
										if (this.language.NameComparer.Equals(innerClass.Name, name))
										{
											result = new SearchTypeResult(innerClass.DefaultReturnType);
											return result;
										}
									}
								}
							}
						}
					}
					if (request.CurrentCompilationUnit != null)
					{
						foreach (IUsing u in request.CurrentCompilationUnit.Usings)
						{
							if (u != null)
							{
								foreach (IReturnType r in u.SearchType(name, request.TypeParameterCount))
								{
									if (r.TypeParameterCount == request.TypeParameterCount)
									{
										result = new SearchTypeResult(r, u);
										return result;
									}
									fallbackResult = new SearchTypeResult(r, u);
								}
							}
						}
					}
					if (this.defaultImports != null)
					{
						foreach (IReturnType r in this.defaultImports.SearchType(name, request.TypeParameterCount))
						{
							if (r.TypeParameterCount == request.TypeParameterCount)
							{
								result = new SearchTypeResult(r, this.defaultImports);
								return result;
							}
							fallbackResult = new SearchTypeResult(r, this.defaultImports);
						}
					}
					result = fallbackResult;
				}
			}
			return result;
		}

		private IClass GetClassByDotNetName(string className, bool lookInReferences)
		{
			className = className.Replace('+', '.');
			IClass @class;
			if (className.Length > 2 && className[className.Length - 2] == '`')
			{
				int typeParameterCount = (int)(className[className.Length - 1] - '0');
				if (typeParameterCount < 0)
				{
					typeParameterCount = 0;
				}
				className = className.Substring(0, className.Length - 2);
				@class = this.GetClass(className, typeParameterCount, LanguageProperties.CSharp, lookInReferences);
			}
			else
			{
				@class = this.GetClass(className, 0, LanguageProperties.CSharp, lookInReferences);
			}
			return @class;
		}

		public IDecoration GetElement(string fullMemberName)
		{
			IClass curClass = this.GetClassByDotNetName(fullMemberName, false);
			IDecoration result;
			if (curClass != null)
			{
				result = curClass;
			}
			else
			{
				int pos = fullMemberName.IndexOf('(');
				if (pos > 0)
				{
					int colonPos = fullMemberName.LastIndexOf(':');
					if (colonPos > 0)
					{
						fullMemberName = fullMemberName.Substring(0, colonPos);
					}
					string memberName = fullMemberName.Substring(0, pos);
					int pos2 = memberName.LastIndexOf('.');
					if (pos2 > 0)
					{
						string className = memberName.Substring(0, pos2);
						memberName = memberName.Substring(pos2 + 1);
						curClass = this.GetClassByDotNetName(className, false);
						if (curClass != null)
						{
							IMethod firstMethod = null;
							foreach (IMethod i in curClass.Methods)
							{
								if (i.Name == memberName)
								{
									if (firstMethod == null)
									{
										firstMethod = i;
									}
									StringBuilder dotnetName = new StringBuilder(i.DotNetName);
									dotnetName.Append('(');
									for (int j = 0; j < i.Parameters.Count; j++)
									{
										if (j > 0)
										{
											dotnetName.Append(',');
										}
										if (i.Parameters[j].ReturnType != null)
										{
											dotnetName.Append(i.Parameters[j].ReturnType.DotNetName);
										}
									}
									dotnetName.Append(')');
									if (dotnetName.ToString() == fullMemberName)
									{
										result = i;
										return result;
									}
								}
							}
							result = firstMethod;
							return result;
						}
					}
				}
				else
				{
					pos = fullMemberName.LastIndexOf('.');
					if (pos > 0)
					{
						string className = fullMemberName.Substring(0, pos);
						string memberName = fullMemberName.Substring(pos + 1);
						curClass = this.GetClassByDotNetName(className, false);
						if (curClass != null)
						{
							IMethod firstMethod = null;
							foreach (IMethod i in curClass.Methods)
							{
								if (i.Name == memberName)
								{
									if (firstMethod == null || i.Parameters.Count == 0)
									{
										firstMethod = i;
									}
								}
							}
							if (firstMethod != null)
							{
								result = firstMethod;
								return result;
							}
							result = curClass.SearchMember(memberName, LanguageProperties.CSharp);
							return result;
						}
					}
				}
				result = null;
			}
			return result;
		}

		public FilePosition GetPosition(string fullMemberName)
		{
			IDecoration d = this.GetElement(fullMemberName);
			IMember i = d as IMember;
			IClass c = d as IClass;
			FilePosition result;
			if (i != null)
			{
				result = new FilePosition(i.DeclaringType.CompilationUnit, i.Region.BeginLine, i.Region.BeginColumn);
			}
			else if (c != null)
			{
				result = new FilePosition(c.CompilationUnit, c.Region.BeginLine, c.Region.BeginColumn);
			}
			else
			{
				result = FilePosition.Empty;
			}
			return result;
		}

		public void AddReferencedContent(IProjectContent pc)
		{
			if (pc != null)
			{
				lock (this.ReferencedContents)
				{
					this.ReferencedContents.Add(pc);
				}
			}
		}

		protected virtual void OnReferencedContentsChanged(EventArgs e)
		{
			this.systemTypes = null;
			SearchClassReturnType.ClearCache();
			if (this.ReferencedContentsChanged != null)
			{
				this.ReferencedContentsChanged(this, e);
			}
		}
	}
}
