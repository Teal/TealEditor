using AIMS.Libraries.Scripting.Dom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public interface IProject
	{
		ReadOnlyCollection<ProjectItem> Items
		{
			get;
		}

		LanguageProperties LanguageProperties
		{
			get;
		}

		IAmbience Ambience
		{
			get;
		}

		string Directory
		{
			get;
		}

		string AssemblyName
		{
			get;
			set;
		}

		string RootNamespace
		{
			get;
			set;
		}

		string OutputAssemblyFullPath
		{
			get;
		}

		string FileName
		{
			get;
			set;
		}

		void AddProjectItem(ProjectItem item);

		bool RemoveProjectItem(ProjectItem item);

		IEnumerable<ProjectItem> GetItemsOfType(ItemType type);
	}
}
