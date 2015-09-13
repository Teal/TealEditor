using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	internal class DefaultProject : IProject
	{
		private IList<ProjectItem> _defProjectItems = null;

		private string _AssemblyName = "";

		private string _OutputAssemblyFullPath = "";

		private string _RootNamespace = "";

		private string _FileName = "";

		ReadOnlyCollection<ProjectItem> IProject.Items
		{
			get
			{
				return new ReadOnlyCollection<ProjectItem>(this._defProjectItems);
			}
		}

		LanguageProperties IProject.LanguageProperties
		{
			get
			{
				return new DefaultProjectContent
				{
					Language = (ProjectParser.Language == SupportedLanguage.CSharp) ? LanguageProperties.CSharp : LanguageProperties.VBNet
				}.Language;
			}
		}

		IAmbience IProject.Ambience
		{
			get
			{
				return ProjectParser.CurrentAmbience;
			}
		}

		string IProject.Directory
		{
			get
			{
				return ProjectParser.ProjectPath;
			}
		}

		string IProject.AssemblyName
		{
			get
			{
				return this._AssemblyName;
			}
			set
			{
				this._AssemblyName = value;
			}
		}

		string IProject.RootNamespace
		{
			get
			{
				return this._RootNamespace;
			}
			set
			{
				this._RootNamespace = value;
			}
		}

		string IProject.OutputAssemblyFullPath
		{
			get
			{
				return Path.Combine(this._OutputAssemblyFullPath, this._AssemblyName);
			}
		}

		string IProject.FileName
		{
			get
			{
				return this._FileName;
			}
			set
			{
				this._FileName = value;
			}
		}

		public DefaultProject()
		{
			this._defProjectItems = new List<ProjectItem>();
			this._AssemblyName = "AIMSScript.dll";
			this._RootNamespace = "AIMS.Script";
			if (ProjectParser.Language == SupportedLanguage.CSharp)
			{
				this._FileName = "AIMS Script.cs";
			}
			else
			{
				this._FileName = "AIMS Script.vb";
			}
			this._OutputAssemblyFullPath = ProjectParser.ProjectPath;
		}

		void IProject.AddProjectItem(ProjectItem item)
		{
			this._defProjectItems.Add(item);
			try
			{
				lock (ProjectParser.CurrentProjectContent.ReferencedContents)
				{
					ProjectParser.CurrentProjectContent.ReferencedContents.Add(ProjectParser.ProjectContentRegistry.GetProjectContentForReference(item.Include, item.FileName));
				}
				this.UpdateReferenceInterDependencies();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void UpdateReferenceInterDependencies()
		{
			IProjectContent[] referencedContents;
			lock (ProjectParser.CurrentProjectContent.ReferencedContents)
			{
				referencedContents = new IProjectContent[ProjectParser.CurrentProjectContent.ReferencedContents.Count];
				ProjectParser.CurrentProjectContent.ReferencedContents.CopyTo(referencedContents, 0);
			}
			IProjectContent[] array = referencedContents;
			for (int i = 0; i < array.Length; i++)
			{
				IProjectContent referencedContent = array[i];
				if (referencedContent is ReflectionProjectContent)
				{
					((ReflectionProjectContent)referencedContent).InitializeReferences();
				}
			}
		}

		bool IProject.RemoveProjectItem(ProjectItem item)
		{
			return this._defProjectItems.Remove(item);
		}

		IEnumerable<ProjectItem> IProject.GetItemsOfType(ItemType type)
		{
			DefaultProject.GetItemsOfType>d__0 getItemsOfType>d__ = new DefaultProject.GetItemsOfType>d__0(-2);
			getItemsOfType>d__.<>4__this = this;
			getItemsOfType>d__.<>3__type = type;
			return getItemsOfType>d__;
		}
	}
}
