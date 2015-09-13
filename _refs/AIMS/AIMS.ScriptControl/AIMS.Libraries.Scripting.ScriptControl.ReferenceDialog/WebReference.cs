using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.ScriptControl.Parser;
using AIMS.Libraries.Scripting.ScriptControl.Project;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class WebReference
	{
		private List<ProjectItem> items;

		private string url = string.Empty;

		private string relativePath = string.Empty;

		private DiscoveryClientProtocol protocol;

		private IProject project;

		private string webReferencesDirectory = string.Empty;

		private string proxyNamespace = string.Empty;

		private string name = string.Empty;

		private WebReferenceUrl webReferenceUrl;

		public WebReferencesProjectItem WebReferencesProjectItem
		{
			get
			{
				return WebReference.GetWebReferencesProjectItem(this.Items);
			}
		}

		public WebReferenceUrl WebReferenceUrl
		{
			get
			{
				if (this.webReferenceUrl == null)
				{
					this.items = this.CreateProjectItems();
				}
				return this.webReferenceUrl;
			}
		}

		public string WebReferencesDirectory
		{
			get
			{
				return this.webReferencesDirectory;
			}
		}

		public string Directory
		{
			get
			{
				return Path.Combine(this.project.Directory, this.relativePath);
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.OnNameChanged();
			}
		}

		public string ProxyNamespace
		{
			get
			{
				return this.proxyNamespace;
			}
			set
			{
				this.proxyNamespace = value;
			}
		}

		public List<ProjectItem> Items
		{
			get
			{
				if (this.items == null)
				{
					this.items = this.CreateProjectItems();
				}
				return this.items;
			}
		}

		public string WebProxyFileName
		{
			get
			{
				return this.GetFullProxyFileName();
			}
		}

		public WebReference(IProject project, string url, string name, string proxyNamespace, DiscoveryClientProtocol protocol)
		{
			this.project = project;
			this.url = url;
			this.protocol = protocol;
			this.proxyNamespace = proxyNamespace;
			this.name = name;
			this.GetRelativePath();
		}

		public static bool ProjectContainsWebReferencesFolder(IProject project)
		{
			return WebReference.GetWebReferencesProjectItem(project) != null;
		}

		public static bool ProjectContainsWebServicesReference(IProject project)
		{
			bool result;
			foreach (ProjectItem item in project.Items)
			{
				if (item.ItemType == ItemType.Reference && item.Include != null)
				{
					if (item.Include.Trim().StartsWith("System.Web.Services", StringComparison.InvariantCultureIgnoreCase))
					{
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		public static WebReferencesProjectItem GetWebReferencesProjectItem(IProject project)
		{
			return WebReference.GetWebReferencesProjectItem(project.Items);
		}

		public static string GetReferenceName(string webReferenceFolder, string name)
		{
			int count = 1;
			string referenceName = name;
			string folder = Path.Combine(webReferenceFolder, name);
			while (System.IO.Directory.Exists(folder))
			{
				referenceName = name + count.ToString();
				folder = Path.Combine(webReferenceFolder, referenceName);
				count++;
			}
			return referenceName;
		}

		public static List<ProjectItem> GetFileItems(IProject project, string name)
		{
			List<ProjectItem> items = new List<ProjectItem>();
			WebReferencesProjectItem webReferencesProjectItem = WebReference.GetWebReferencesProjectItem(project);
			if (webReferencesProjectItem != null)
			{
				string webReferenceDirectory = Path.Combine(Path.Combine(project.Directory, webReferencesProjectItem.Include), name);
				foreach (ProjectItem item in project.Items)
				{
					FileProjectItem fileItem = item as FileProjectItem;
					if (fileItem != null)
					{
						if (FileUtility.IsBaseDirectory(webReferenceDirectory, fileItem.FileName))
						{
							items.Add(fileItem);
						}
					}
				}
			}
			return items;
		}

		public WebReferenceChanges GetChanges(IProject project)
		{
			WebReferenceChanges changes = new WebReferenceChanges();
			List<ProjectItem> existingItems = WebReference.GetFileItems(project, this.name);
			changes.NewItems.AddRange(this.GetNewItems(existingItems));
			changes.ItemsRemoved.AddRange(this.GetRemovedItems(existingItems));
			return changes;
		}

		public void Save()
		{
			System.IO.Directory.CreateDirectory(this.Directory);
			this.GenerateWebProxy();
			this.protocol.WriteAll(this.Directory, "Reference.map");
		}

		public string GetSourceCode()
		{
			string proxynamespace = this.proxyNamespace;
			ServiceDescriptionCollection serviceDescriptions = this.GetServiceDescriptionCollection(this.protocol);
			XmlSchemas schemas = this.GetXmlSchemas(this.protocol);
			ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
			foreach (ServiceDescription description in serviceDescriptions)
			{
				importer.AddServiceDescription(description, null, null);
			}
			foreach (XmlSchema schema in schemas)
			{
				importer.Schemas.Add(schema);
			}
			importer.Style = ServiceDescriptionImportStyle.Client;
			importer.CodeGenerationOptions = (CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateNewAsync);
			CodeNamespace codeNamespace = new CodeNamespace(this.proxyNamespace);
			CodeCompileUnit codeUnit = new CodeCompileUnit();
			codeUnit.Namespaces.Add(codeNamespace);
			ServiceDescriptionImportWarnings warnings = importer.Import(codeNamespace, codeUnit);
			CodeDomProvider provider = this.project.LanguageProperties.CodeDomProvider;
			string SourceCode = "";
			if (provider != null)
			{
				StringWriter sw = new StringWriter();
				provider.GenerateCodeFromCompileUnit(codeUnit, sw, new CodeGeneratorOptions
				{
					BracingStyle = "C"
				});
				SourceCode = sw.ToString();
				sw.Close();
			}
			return SourceCode;
		}

		public void Delete()
		{
			System.IO.Directory.Delete(this.Directory, true);
		}

		private ServiceDescriptionCollection GetServiceDescriptionCollection(DiscoveryClientProtocol protocol)
		{
			ServiceDescriptionCollection services = new ServiceDescriptionCollection();
			foreach (DictionaryEntry entry in protocol.References)
			{
				ContractReference contractRef = entry.Value as ContractReference;
				DiscoveryDocumentReference discoveryRef = entry.Value as DiscoveryDocumentReference;
				if (contractRef != null)
				{
					services.Add(contractRef.Contract);
				}
			}
			return services;
		}

		private XmlSchemas GetXmlSchemas(DiscoveryClientProtocol protocol)
		{
			XmlSchemas schemas = new XmlSchemas();
			foreach (DictionaryEntry entry in protocol.References)
			{
				SchemaReference schemaRef = entry.Value as SchemaReference;
				if (schemaRef != null)
				{
					schemas.Add(schemaRef.Schema);
				}
			}
			return schemas;
		}

		private void GenerateWebProxy()
		{
			WebReference.GenerateWebProxy(this.proxyNamespace, this.GetFullProxyFileName(), this.GetServiceDescriptionCollection(this.protocol), this.GetXmlSchemas(this.protocol));
		}

		private static void GenerateWebProxy(string proxyNamespace, string fileName, ServiceDescriptionCollection serviceDescriptions, XmlSchemas schemas)
		{
			ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
			foreach (ServiceDescription description in serviceDescriptions)
			{
				importer.AddServiceDescription(description, null, null);
			}
			foreach (XmlSchema schema in schemas)
			{
				importer.Schemas.Add(schema);
			}
			CodeNamespace codeNamespace = new CodeNamespace(proxyNamespace);
			CodeCompileUnit codeUnit = new CodeCompileUnit();
			codeUnit.Namespaces.Add(codeNamespace);
			ServiceDescriptionImportWarnings warnings = importer.Import(codeNamespace, codeUnit);
			CodeDomProvider provider;
			if (ProjectParser.Language == SupportedLanguage.CSharp)
			{
				provider = new CSharpCodeProvider();
			}
			else
			{
				provider = new VBCodeProvider();
			}
			if (provider != null)
			{
				StreamWriter sw = new StreamWriter(fileName);
				provider.GenerateCodeFromCompileUnit(codeUnit, sw, new CodeGeneratorOptions
				{
					BracingStyle = "C"
				});
				sw.Close();
			}
		}

		private string GetFullProxyFileName()
		{
			return Path.Combine(this.project.Directory, this.GetProxyFileName());
		}

		private string GetProxyFileName()
		{
			string fileName;
			if (ProjectParser.Language == SupportedLanguage.CSharp)
			{
				fileName = "Reference" + ".cs";
			}
			else
			{
				fileName = "Reference" + ".vb";
			}
			return Path.Combine(this.relativePath, fileName);
		}

		private static WebReferencesProjectItem GetWebReferencesProjectItem(IEnumerable<ProjectItem> items)
		{
			WebReferencesProjectItem result;
			foreach (ProjectItem item in items)
			{
				if (item.ItemType == ItemType.WebReferences)
				{
					result = (WebReferencesProjectItem)item;
					return result;
				}
			}
			result = null;
			return result;
		}

		private void OnNameChanged()
		{
			this.GetRelativePath();
			if (this.items != null)
			{
				this.items = this.CreateProjectItems();
			}
		}

		private void GetRelativePath()
		{
			ProjectItem webReferencesProjectItem = WebReference.GetWebReferencesProjectItem(this.project);
			string webReferencesDirectoryName;
			if (webReferencesProjectItem != null)
			{
				webReferencesDirectoryName = webReferencesProjectItem.Include.Trim(new char[]
				{
					'\\',
					'/'
				});
			}
			else
			{
				webReferencesDirectoryName = "Web References";
			}
			this.webReferencesDirectory = Path.Combine(this.project.Directory, webReferencesDirectoryName);
			this.relativePath = Path.Combine(webReferencesDirectoryName, this.name);
		}

		private List<ProjectItem> CreateProjectItems()
		{
			List<ProjectItem> items = new List<ProjectItem>();
			if (!WebReference.ProjectContainsWebReferencesFolder(this.project))
			{
				items.Add(new WebReferencesProjectItem(this.project)
				{
					Include = "Web References\\"
				});
			}
			this.webReferenceUrl = new WebReferenceUrl(this.project);
			this.webReferenceUrl.Include = this.url;
			this.webReferenceUrl.UpdateFromURL = this.url;
			this.webReferenceUrl.RelPath = this.relativePath;
			this.webReferenceUrl.Namespace = this.proxyNamespace;
			items.Add(this.webReferenceUrl);
			foreach (DictionaryEntry entry in this.protocol.References)
			{
				DiscoveryReference discoveryRef = entry.Value as DiscoveryReference;
				if (discoveryRef != null)
				{
					items.Add(new FileProjectItem(this.project, ItemType.None)
					{
						Include = Path.Combine(this.relativePath, discoveryRef.DefaultFilename)
					});
				}
			}
			FileProjectItem proxyItem = new FileProjectItem(this.project, ItemType.Compile);
			proxyItem.Include = this.GetProxyFileName();
			proxyItem.SetEvaluatedMetadata("AutoGen", "True");
			proxyItem.SetEvaluatedMetadata("DesignTime", "True");
			proxyItem.DependentUpon = "Reference.map";
			items.Add(proxyItem);
			FileProjectItem mapItem = new FileProjectItem(this.project, ItemType.None);
			mapItem.Include = Path.Combine(this.relativePath, "Reference.map");
			mapItem.SetEvaluatedMetadata("Generator", "MSDiscoCodeGenerator");
			mapItem.SetEvaluatedMetadata("LastGenOutput", "Reference.cs");
			items.Add(mapItem);
			if (!WebReference.ProjectContainsWebServicesReference(this.project))
			{
				ReferenceProjectItem webServicesReferenceItem = new ReferenceProjectItem(this.project, "System.Web.Services");
				items.Add(webServicesReferenceItem);
			}
			return items;
		}

		private List<ProjectItem> GetNewItems(List<ProjectItem> projectWebReferenceItems)
		{
			List<ProjectItem> newItems = new List<ProjectItem>();
			foreach (ProjectItem item in this.Items)
			{
				if (!(item is WebReferenceUrl))
				{
					if (!WebReference.ContainsFileName(projectWebReferenceItems, item.FileName))
					{
						newItems.Add(item);
					}
				}
			}
			return newItems;
		}

		private List<ProjectItem> GetRemovedItems(List<ProjectItem> projectWebReferenceItems)
		{
			List<ProjectItem> removedItems = new List<ProjectItem>();
			foreach (ProjectItem item in projectWebReferenceItems)
			{
				if (!WebReference.ContainsFileName(this.Items, item.FileName))
				{
					removedItems.Add(item);
				}
			}
			return removedItems;
		}

		private static bool ContainsFileName(List<ProjectItem> items, string fileName)
		{
			bool result;
			foreach (ProjectItem item in items)
			{
				if (FileUtility.IsEqualFileName(item.FileName, fileName))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}
	}
}
