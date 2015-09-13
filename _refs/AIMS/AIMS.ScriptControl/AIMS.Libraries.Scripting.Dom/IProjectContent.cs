using System;
using System.Collections;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IProjectContent : IDisposable
	{
		event EventHandler ReferencedContentsChanged;

		XmlDoc XmlDoc
		{
			get;
		}

		bool IsUpToDate
		{
			get;
		}

		ICollection<IClass> Classes
		{
			get;
		}

		ICollection<string> NamespaceNames
		{
			get;
		}

		ICollection<IProjectContent> ReferencedContents
		{
			get;
		}

		LanguageProperties Language
		{
			get;
		}

		IUsing DefaultImports
		{
			get;
		}

		object Project
		{
			get;
		}

		SystemTypes SystemTypes
		{
			get;
		}

		string GetXmlDocumentation(string memberTag);

		void AddClassToNamespaceList(IClass addClass);

		void RemoveCompilationUnit(ICompilationUnit oldUnit);

		void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName);

		IClass GetClass(string typeName);

		IClass GetClass(string typeName, int typeParameterCount);

		bool NamespaceExists(string name);

		ArrayList GetNamespaceContents(string nameSpace);

		IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, bool lookInReferences);

		bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences);

		void AddNamespaceContents(ArrayList list, string subNameSpace, LanguageProperties language, bool lookInReferences);

		string SearchNamespace(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn);

		SearchTypeResult SearchType(SearchTypeRequest request);

		FilePosition GetPosition(string fullMemberName);
	}
}
