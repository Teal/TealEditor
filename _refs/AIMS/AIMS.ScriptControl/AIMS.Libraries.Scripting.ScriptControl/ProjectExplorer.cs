using AIMS.Libraries.Forms.Docking;
using AIMS.Libraries.Scripting.ScriptControl.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl
{
	public class ProjectExplorer : DockableWindow
	{
		private TreeNode rootNode = null;

		private TreeNode referencesNode = null;

		private ScriptLanguage _scriptLanguage = ScriptLanguage.CSharp;

		private IContainer components = null;

		private TreeView tvwSolutionExplorer;

		private ImageList ilImages;

		private ContextMenuStrip cMenuFolder;

		private ToolStripMenuItem addToolStripMenuItem;

		private ToolStripMenuItem tFolderNewItem;

		private ToolStripMenuItem tFolderExisting;

		private ToolStripMenuItem tFolderNewFolder;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripMenuItem tFolderCut;

		private ToolStripMenuItem tFolderCopy;

		private ToolStripMenuItem tFolderPaste;

		private ToolStripMenuItem tFolderDelete;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem tFolderRename;

		private ContextMenuStrip cMenuProject;

		private ToolStripMenuItem tProjectBuild;

		private ToolStripMenuItem tProjectClean;

		private ToolStripSeparator tProjectSep1;

		private ToolStripMenuItem tProjectAdd;

		private ToolStripMenuItem tProjectNewItem;

		private ToolStripMenuItem tProjectExistingItem;

		private ToolStripMenuItem tProjectNewFolder;

		private ToolStripSeparator tProjectSep2;

		private ToolStripMenuItem tProjectAddRef;

		private ToolStripSeparator tProjectSep3;

		private ToolStripMenuItem tProjectProp;

		private ContextMenuStrip cMenuFile;

		private ToolStripMenuItem tFileOpen;

		private ToolStripSeparator tFileSep1;

		private ToolStripMenuItem tFileCut;

		private ToolStripMenuItem tFileCopy;

		private ToolStripMenuItem tFilePaste;

		private ToolStripMenuItem tFileDelete;

		private ToolStripSeparator tFileSep2;

		private ToolStripMenuItem tFileRename;

		private ToolStripMenuItem tProjectAddWebRef;

		public event EventHandler<ExplorerClickEventArgs> FileClick
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.FileClick = (EventHandler<ExplorerClickEventArgs>)Delegate.Combine(this.FileClick, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.FileClick = (EventHandler<ExplorerClickEventArgs>)Delegate.Remove(this.FileClick, value);
			}
		}

		public event EventHandler<ExplorerLabelEditEventArgs> FileNameChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.FileNameChanged = (EventHandler<ExplorerLabelEditEventArgs>)Delegate.Combine(this.FileNameChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.FileNameChanged = (EventHandler<ExplorerLabelEditEventArgs>)Delegate.Remove(this.FileNameChanged, value);
			}
		}

		public event EventHandler NewItemAdd
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.NewItemAdd = (EventHandler)Delegate.Combine(this.NewItemAdd, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.NewItemAdd = (EventHandler)Delegate.Remove(this.NewItemAdd, value);
			}
		}

		public event EventHandler FileItemDeleted
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.FileItemDeleted = (EventHandler)Delegate.Combine(this.FileItemDeleted, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.FileItemDeleted = (EventHandler)Delegate.Remove(this.FileItemDeleted, value);
			}
		}

		public event EventHandler AddRefrenceItem
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.AddRefrenceItem = (EventHandler)Delegate.Combine(this.AddRefrenceItem, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.AddRefrenceItem = (EventHandler)Delegate.Remove(this.AddRefrenceItem, value);
			}
		}

		public event EventHandler AddWebRefrenceItem
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.AddWebRefrenceItem = (EventHandler)Delegate.Combine(this.AddWebRefrenceItem, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.AddWebRefrenceItem = (EventHandler)Delegate.Remove(this.AddWebRefrenceItem, value);
			}
		}

		public TreeNode RefrenceNode
		{
			get
			{
				return this.referencesNode;
			}
		}

		public ScriptLanguage Language
		{
			get
			{
				return this._scriptLanguage;
			}
			set
			{
				this._scriptLanguage = value;
				this.UpdateLanguageUI(this.rootNode);
			}
		}

		public ProjectExplorer()
		{
			this.InitializeComponent();
			this.InitilizeTree();
		}

		protected virtual void OnFileClick(ExplorerClickEventArgs e)
		{
			if (this.FileClick != null)
			{
				this.FileClick(this, e);
			}
		}

		protected virtual void OnNewItemAdd()
		{
			if (this.NewItemAdd != null)
			{
				this.NewItemAdd(this, null);
			}
		}

		protected virtual void OnAddRefrenceItem(TreeNode node)
		{
			if (this.AddRefrenceItem != null)
			{
				this.AddRefrenceItem(node, null);
			}
		}

		protected virtual void OnAddWebRefrenceItem(TreeNode node)
		{
			if (this.AddWebRefrenceItem != null)
			{
				this.AddWebRefrenceItem(node, null);
			}
		}

		protected virtual void OnFileItemDeleted(TreeNode node)
		{
			if (this.FileItemDeleted != null)
			{
				TreeNode parent = node.Parent;
				this.FileItemDeleted(node, null);
				this.DeleteWebReferenceFolder(parent);
			}
		}

		private void DeleteWebReferenceFolder(TreeNode Item)
		{
			if (Item.Nodes.Count == 0)
			{
				Item.Remove();
				this.UpdateLanguageUI(this.rootNode);
			}
		}

		protected virtual void OnFileNameChanged(ExplorerLabelEditEventArgs e)
		{
			if (this.FileNameChanged != null)
			{
				this.FileNameChanged(this, e);
			}
		}

		private void InitilizeTree()
		{
			this.tvwSolutionExplorer.Nodes.Clear();
			this.rootNode = this.tvwSolutionExplorer.Nodes.Add("Project");
			this.rootNode.Expand();
			this.rootNode.Tag = NodeType.Project;
			this.referencesNode = this.rootNode.Nodes.Add("References");
			this.referencesNode.Tag = NodeType.References;
			this.UpdateLanguageUI(this.rootNode);
			this.tvwSolutionExplorer.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.tvwSolutionExplorer_NodeDblClick);
			this.tvwSolutionExplorer.KeyUp += new KeyEventHandler(this.tvwSolutionExplorer_KeyUp);
			this.tvwSolutionExplorer.AfterLabelEdit += new NodeLabelEditEventHandler(this.tvwSolutionExplorer_AfterLabelEdit);
		}

		private bool ValidateFolderName(TreeNode startNode, string NewName)
		{
			bool result;
			foreach (TreeNode t in startNode.Nodes)
			{
				if (t.Text.ToLower() == NewName.ToLower() && (NodeType)t.Tag == NodeType.Folder)
				{
					result = false;
					return result;
				}
				this.ValidateFolderName(t, NewName);
			}
			result = true;
			return result;
		}

		private void tvwSolutionExplorer_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			this.tvwSolutionExplorer.LabelEdit = false;
			if (e.Label == null)
			{
				e.CancelEdit = true;
			}
			else
			{
				TreeNode node = e.Node;
				NodeType t = (NodeType)node.Tag;
				if (t == NodeType.Folder)
				{
					e.CancelEdit = true;
					TreeNode parentNode = node.Parent;
					if (this.ValidateFolderName(parentNode, e.Label))
					{
						e.Node.Text = e.Label;
					}
					else
					{
						MessageBox.Show("Folder Name '" + e.Label + "' already exists in current hirerachy.Please select another name.");
					}
				}
				else if (Path.GetFileNameWithoutExtension(e.Label).Length == 0)
				{
					e.CancelEdit = true;
				}
				else
				{
					string newName = Path.GetFileNameWithoutExtension(e.Label);
					string oldExtension = Path.GetExtension(e.Node.Text);
					string validName = newName + oldExtension;
					ExplorerLabelEditEventArgs evnt = new ExplorerLabelEditEventArgs(validName, e.Node.Text);
					this.OnFileNameChanged(evnt);
					e.CancelEdit = true;
					if (!evnt.Cancel)
					{
						e.Node.Text = validName;
					}
				}
			}
		}

		public void StartLabelEdit(TreeNode node)
		{
			NodeType t = (NodeType)node.Tag;
			if (t == NodeType.File || t == NodeType.Folder)
			{
				this.tvwSolutionExplorer.LabelEdit = true;
				node.BeginEdit();
			}
		}

		private void tvwSolutionExplorer_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F2)
			{
				TreeNode node = this.tvwSolutionExplorer.SelectedNode;
				if (node != null)
				{
					this.StartLabelEdit(node);
				}
			}
		}

		private void tvwSolutionExplorer_NodeDblClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			NodeType t = (NodeType)e.Node.Tag;
			if (t == NodeType.File)
			{
				this.OnFileClick(new ExplorerClickEventArgs(e.Node.Text));
			}
			this.UpdateImage(e.Node);
		}

		public void AddWebReference(string fileName)
		{
			TreeNode parent = this.GetWebReferenceNode();
			if (!parent.IsExpanded)
			{
				parent.Expand();
			}
			TreeNode node = parent.Nodes.Add(fileName, fileName);
			node.Tag = NodeType.File;
			this.UpdateImage(node);
		}

		public TreeNode GetWebReferenceNode()
		{
			TreeNode refNode = null;
			foreach (TreeNode node in this.referencesNode.Nodes)
			{
				NodeType t = (NodeType)node.Tag;
				if (t == NodeType.WebRererenceFolder)
				{
					refNode = node;
					break;
				}
			}
			if (refNode == null)
			{
				refNode = this.referencesNode.Nodes.Add("Web Reference");
				refNode.Tag = NodeType.WebRererenceFolder;
			}
			if (!refNode.IsExpanded)
			{
				refNode.Expand();
			}
			this.UpdateImage(refNode);
			return refNode;
		}

		public void AddFile(string fileName)
		{
			TreeNode selfolder = this.tvwSolutionExplorer.SelectedNode;
			NodeType t = NodeType.Project;
			if (selfolder != null)
			{
				t = (NodeType)selfolder.Tag;
			}
			if (t != NodeType.Folder)
			{
				selfolder = null;
			}
			TreeNode parent = (selfolder == null) ? this.rootNode : selfolder;
			if (!parent.IsExpanded)
			{
				parent.Expand();
			}
			TreeNode node = parent.Nodes.Add(fileName, fileName);
			node.Tag = NodeType.File;
			this.UpdateImage(node);
		}

		public void ActiveNode(string fileName)
		{
			this.tvwSolutionExplorer.SelectedNode = null;
		}

		public TreeNode AddFolder(string folderName)
		{
			TreeNode selfolder = this.tvwSolutionExplorer.SelectedNode;
			NodeType t = (NodeType)selfolder.Tag;
			if (t != NodeType.Folder)
			{
				selfolder = null;
			}
			TreeNode parent = (selfolder == null) ? this.rootNode : selfolder;
			TreeNode node = parent.Nodes.Add(folderName);
			if (!parent.IsExpanded)
			{
				parent.Expand();
			}
			node.Tag = NodeType.Folder;
			this.UpdateImage(node);
			return node;
		}

		public void AddRefrence(string referenceName)
		{
			TreeNode node = this.referencesNode.Nodes.Add(referenceName);
			node.Tag = NodeType.Reference;
			this.UpdateImage(node);
		}

		private void UpdateImage(TreeNode node)
		{
			switch ((NodeType)node.Tag)
			{
			case NodeType.Project:
				node.ImageKey = ((this._scriptLanguage == ScriptLanguage.CSharp) ? "CSharpProject.ico" : "VbProject.ico");
				break;
			case NodeType.File:
				node.ImageKey = ((this._scriptLanguage == ScriptLanguage.CSharp) ? "CSharpFile.ico" : "VbFile.ico");
				node.Text = Path.GetFileNameWithoutExtension(node.Text) + ((this._scriptLanguage == ScriptLanguage.CSharp) ? ".cs" : ".vb");
				break;
			case NodeType.Folder:
				node.ImageKey = (node.IsExpanded ? "OpenFolder.ico" : "ClosedFolder.ico");
				break;
			case NodeType.References:
			case NodeType.WebRererenceFolder:
				node.ImageKey = (node.IsExpanded ? "ReferencesOpen.ico" : "ReferencesClosed.ico");
				break;
			case NodeType.Reference:
				node.ImageKey = "Reference.ico";
				break;
			}
			node.SelectedImageKey = node.ImageKey;
		}

		private void UpdateLanguageUI(TreeNode StartNode)
		{
			this.UpdateImage(StartNode);
			foreach (TreeNode node in StartNode.Nodes)
			{
				this.UpdateLanguageUI(node);
			}
		}

		private void tvwSolutionExplorer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			TreeNode node = e.Node;
			this.tvwSolutionExplorer.SelectedNode = node;
			ContextMenuStrip c = null;
			if (e.Button == MouseButtons.Right && node != null)
			{
				switch ((NodeType)node.Tag)
				{
				case NodeType.Project:
					this.tProjectAdd.Visible = true;
					this.tProjectBuild.Visible = true;
					this.tProjectAdd.Visible = true;
					this.tProjectClean.Visible = true;
					this.tProjectProp.Visible = true;
					this.tProjectSep1.Visible = true;
					this.tProjectSep2.Visible = true;
					this.tProjectSep3.Visible = true;
					c = this.cMenuProject;
					break;
				case NodeType.File:
					this.tFileCopy.Visible = true;
					this.tFileCut.Visible = true;
					this.tFileOpen.Visible = true;
					this.tFilePaste.Visible = true;
					this.tFileRename.Visible = true;
					this.tFileSep1.Visible = true;
					this.tFileSep2.Visible = true;
					c = this.cMenuFile;
					break;
				case NodeType.Folder:
					c = this.cMenuFolder;
					break;
				case NodeType.References:
					this.tProjectAdd.Visible = false;
					this.tProjectBuild.Visible = false;
					this.tProjectAdd.Visible = false;
					this.tProjectClean.Visible = false;
					this.tProjectProp.Visible = false;
					this.tProjectSep1.Visible = false;
					this.tProjectSep2.Visible = false;
					this.tProjectSep3.Visible = false;
					c = this.cMenuProject;
					break;
				case NodeType.Reference:
					this.tFileCopy.Visible = false;
					this.tFileCut.Visible = false;
					this.tFileOpen.Visible = false;
					this.tFilePaste.Visible = false;
					this.tFileRename.Visible = false;
					this.tFileSep1.Visible = false;
					this.tFileSep2.Visible = false;
					c = this.cMenuFile;
					break;
				}
				if (c != null)
				{
					c.Show(this.tvwSolutionExplorer, e.Location);
				}
			}
			this.UpdateImage(node);
		}

		private void AddNewItem(object sender, EventArgs e)
		{
			this.OnNewItemAdd();
		}

		private void AddExistingItem(object sender, EventArgs e)
		{
		}

		private void AddNewFolder(object sender, EventArgs e)
		{
			TreeNode node = this.AddFolder("New Folder");
			this.StartLabelEdit(node);
		}

		private void RenameItem(object sender, EventArgs e)
		{
			TreeNode node = this.tvwSolutionExplorer.SelectedNode;
			if (node != null)
			{
				this.StartLabelEdit(node);
			}
		}

		private void DeleteFolder(TreeNode root)
		{
			foreach (TreeNode node in root.Nodes)
			{
				if ((NodeType)node.Tag == NodeType.File)
				{
					this.OnFileItemDeleted(node);
				}
				else if ((NodeType)node.Tag == NodeType.Folder)
				{
					this.DeleteFolder(node);
					node.Remove();
				}
			}
		}

		private void DeleteItem(object sender, EventArgs e)
		{
			TreeNode node = this.tvwSolutionExplorer.SelectedNode;
			if (node != null)
			{
				switch ((NodeType)node.Tag)
				{
				case NodeType.File:
					this.OnFileItemDeleted(node);
					break;
				case NodeType.Folder:
					this.DeleteFolder(node);
					node.Remove();
					break;
				}
			}
		}

		private void OpenItem(object sender, EventArgs e)
		{
			TreeNode node = this.tvwSolutionExplorer.SelectedNode;
			NodeType t = (NodeType)node.Tag;
			if (t == NodeType.File)
			{
				this.OnFileClick(new ExplorerClickEventArgs(node.Text));
			}
		}

		private void CutItem(object sender, EventArgs e)
		{
		}

		private void CopyItem(object sender, EventArgs e)
		{
		}

		private void PasteItem(object sender, EventArgs e)
		{
		}

		private void BuildProject(object sender, EventArgs e)
		{
		}

		private void CleanProject(object sender, EventArgs e)
		{
		}

		private void AddRefrence(object sender, EventArgs e)
		{
			this.OnAddRefrenceItem(this.referencesNode);
		}

		private void ProjectProperties(object sender, EventArgs e)
		{
		}

		private void AddWebReference(object sender, EventArgs e)
		{
			this.OnAddWebRefrenceItem(this.referencesNode);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			TreeNode treeNode = new TreeNode("Node0");
			ComponentResourceManager resources = new ComponentResourceManager(typeof(ProjectExplorer));
			this.tvwSolutionExplorer = new TreeView();
			this.ilImages = new ImageList(this.components);
			this.cMenuFolder = new ContextMenuStrip(this.components);
			this.addToolStripMenuItem = new ToolStripMenuItem();
			this.tFolderNewItem = new ToolStripMenuItem();
			this.tFolderExisting = new ToolStripMenuItem();
			this.tFolderNewFolder = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.tFolderCut = new ToolStripMenuItem();
			this.tFolderCopy = new ToolStripMenuItem();
			this.tFolderPaste = new ToolStripMenuItem();
			this.tFolderDelete = new ToolStripMenuItem();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.tFolderRename = new ToolStripMenuItem();
			this.cMenuProject = new ContextMenuStrip(this.components);
			this.tProjectBuild = new ToolStripMenuItem();
			this.tProjectClean = new ToolStripMenuItem();
			this.tProjectSep1 = new ToolStripSeparator();
			this.tProjectAdd = new ToolStripMenuItem();
			this.tProjectNewItem = new ToolStripMenuItem();
			this.tProjectExistingItem = new ToolStripMenuItem();
			this.tProjectNewFolder = new ToolStripMenuItem();
			this.tProjectSep2 = new ToolStripSeparator();
			this.tProjectAddRef = new ToolStripMenuItem();
			this.tProjectSep3 = new ToolStripSeparator();
			this.tProjectProp = new ToolStripMenuItem();
			this.cMenuFile = new ContextMenuStrip(this.components);
			this.tFileOpen = new ToolStripMenuItem();
			this.tFileSep1 = new ToolStripSeparator();
			this.tFileCut = new ToolStripMenuItem();
			this.tFileCopy = new ToolStripMenuItem();
			this.tFilePaste = new ToolStripMenuItem();
			this.tFileDelete = new ToolStripMenuItem();
			this.tFileSep2 = new ToolStripSeparator();
			this.tFileRename = new ToolStripMenuItem();
			this.tProjectAddWebRef = new ToolStripMenuItem();
			this.cMenuFolder.SuspendLayout();
			this.cMenuProject.SuspendLayout();
			this.cMenuFile.SuspendLayout();
			base.SuspendLayout();
			this.tvwSolutionExplorer.BorderStyle = BorderStyle.None;
			this.tvwSolutionExplorer.Dock = DockStyle.Fill;
			this.tvwSolutionExplorer.FullRowSelect = true;
			this.tvwSolutionExplorer.ImageKey = "CSharpProject.ico";
			this.tvwSolutionExplorer.ImageList = this.ilImages;
			this.tvwSolutionExplorer.Indent = 23;
			this.tvwSolutionExplorer.ItemHeight = 18;
			this.tvwSolutionExplorer.Location = new Point(0, 0);
			this.tvwSolutionExplorer.Margin = new Padding(0);
			this.tvwSolutionExplorer.Name = "tvwSolutionExplorer";
			treeNode.Name = "Node0";
			treeNode.Text = "Node0";
			this.tvwSolutionExplorer.Nodes.AddRange(new TreeNode[]
			{
				treeNode
			});
			this.tvwSolutionExplorer.SelectedImageIndex = 0;
			this.tvwSolutionExplorer.ShowRootLines = false;
			this.tvwSolutionExplorer.Size = new Size(292, 273);
			this.tvwSolutionExplorer.TabIndex = 0;
			this.tvwSolutionExplorer.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.tvwSolutionExplorer_NodeMouseClick);
			this.ilImages.ImageStream = (ImageListStreamer)resources.GetObject("ilImages.ImageStream");
			this.ilImages.TransparentColor = Color.Fuchsia;
			this.ilImages.Images.SetKeyName(0, "VbProject.ico");
			this.ilImages.Images.SetKeyName(1, "CSharpFile.ico");
			this.ilImages.Images.SetKeyName(2, "CSharpProject.ico");
			this.ilImages.Images.SetKeyName(3, "OpenFolder.ico");
			this.ilImages.Images.SetKeyName(4, "Reference.ico");
			this.ilImages.Images.SetKeyName(5, "ReferencesClosed.ico");
			this.ilImages.Images.SetKeyName(6, "ReferencesOpen.ico");
			this.ilImages.Images.SetKeyName(7, "VbFile.ico");
			this.ilImages.Images.SetKeyName(8, "ClosedFolder.ico");
			this.cMenuFolder.Items.AddRange(new ToolStripItem[]
			{
				this.addToolStripMenuItem,
				this.toolStripSeparator1,
				this.tFolderCut,
				this.tFolderCopy,
				this.tFolderPaste,
				this.tFolderDelete,
				this.toolStripSeparator2,
				this.tFolderRename
			});
			this.cMenuFolder.Name = "cMenuExplorer";
			this.cMenuFolder.Size = new Size(125, 148);
			this.addToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.tFolderNewItem,
				this.tFolderExisting,
				this.tFolderNewFolder
			});
			this.addToolStripMenuItem.ImageTransparentColor = Color.Fuchsia;
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new Size(124, 22);
			this.addToolStripMenuItem.Text = "Add";
			this.tFolderNewItem.Image = Resources.NewItem;
			this.tFolderNewItem.ImageTransparentColor = Color.Fuchsia;
			this.tFolderNewItem.Name = "tFolderNewItem";
			this.tFolderNewItem.Size = new Size(147, 22);
			this.tFolderNewItem.Text = "New Item";
			this.tFolderNewItem.Click += new EventHandler(this.AddNewItem);
			this.tFolderExisting.Image = Resources.ExistingItem;
			this.tFolderExisting.ImageTransparentColor = Color.Fuchsia;
			this.tFolderExisting.Name = "tFolderExisting";
			this.tFolderExisting.Size = new Size(147, 22);
			this.tFolderExisting.Text = "Existing Item";
			this.tFolderExisting.Click += new EventHandler(this.AddExistingItem);
			this.tFolderNewFolder.Image = Resources.NewFolder;
			this.tFolderNewFolder.ImageTransparentColor = Color.Fuchsia;
			this.tFolderNewFolder.Name = "tFolderNewFolder";
			this.tFolderNewFolder.Size = new Size(147, 22);
			this.tFolderNewFolder.Text = "New Folder";
			this.tFolderNewFolder.Click += new EventHandler(this.AddNewFolder);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(121, 6);
			this.tFolderCut.Image = Resources.Cut;
			this.tFolderCut.ImageTransparentColor = Color.Fuchsia;
			this.tFolderCut.Name = "tFolderCut";
			this.tFolderCut.Size = new Size(124, 22);
			this.tFolderCut.Text = "Cut";
			this.tFolderCut.Click += new EventHandler(this.CutItem);
			this.tFolderCopy.Image = Resources.Copy;
			this.tFolderCopy.ImageTransparentColor = Color.Fuchsia;
			this.tFolderCopy.Name = "tFolderCopy";
			this.tFolderCopy.Size = new Size(124, 22);
			this.tFolderCopy.Text = "Copy";
			this.tFolderCopy.Click += new EventHandler(this.CopyItem);
			this.tFolderPaste.Image = Resources.Paste;
			this.tFolderPaste.ImageTransparentColor = Color.Fuchsia;
			this.tFolderPaste.Name = "tFolderPaste";
			this.tFolderPaste.Size = new Size(124, 22);
			this.tFolderPaste.Text = "Paste";
			this.tFolderPaste.Click += new EventHandler(this.PasteItem);
			this.tFolderDelete.Image = Resources.Delete;
			this.tFolderDelete.ImageTransparentColor = Color.Fuchsia;
			this.tFolderDelete.Name = "tFolderDelete";
			this.tFolderDelete.Size = new Size(124, 22);
			this.tFolderDelete.Text = "Delete";
			this.tFolderDelete.Click += new EventHandler(this.DeleteItem);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(121, 6);
			this.tFolderRename.Name = "tFolderRename";
			this.tFolderRename.Size = new Size(124, 22);
			this.tFolderRename.Text = "Rename";
			this.tFolderRename.Click += new EventHandler(this.RenameItem);
			this.cMenuProject.Items.AddRange(new ToolStripItem[]
			{
				this.tProjectBuild,
				this.tProjectClean,
				this.tProjectSep1,
				this.tProjectAdd,
				this.tProjectSep2,
				this.tProjectAddRef,
				this.tProjectAddWebRef,
				this.tProjectSep3,
				this.tProjectProp
			});
			this.cMenuProject.Name = "cMenuExplorer";
			this.cMenuProject.Size = new Size(198, 176);
			this.tProjectBuild.Image = Resources.Build;
			this.tProjectBuild.ImageTransparentColor = Color.Fuchsia;
			this.tProjectBuild.Name = "tProjectBuild";
			this.tProjectBuild.Size = new Size(197, 22);
			this.tProjectBuild.Text = "Build";
			this.tProjectBuild.Click += new EventHandler(this.BuildProject);
			this.tProjectClean.ImageTransparentColor = Color.Fuchsia;
			this.tProjectClean.Name = "tProjectClean";
			this.tProjectClean.Size = new Size(197, 22);
			this.tProjectClean.Text = "Clean";
			this.tProjectClean.Click += new EventHandler(this.CleanProject);
			this.tProjectSep1.Name = "tProjectSep1";
			this.tProjectSep1.Size = new Size(194, 6);
			this.tProjectAdd.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.tProjectNewItem,
				this.tProjectExistingItem,
				this.tProjectNewFolder
			});
			this.tProjectAdd.ImageTransparentColor = Color.Fuchsia;
			this.tProjectAdd.Name = "tProjectAdd";
			this.tProjectAdd.Size = new Size(197, 22);
			this.tProjectAdd.Text = "Add";
			this.tProjectNewItem.Image = Resources.NewItem;
			this.tProjectNewItem.ImageTransparentColor = Color.Fuchsia;
			this.tProjectNewItem.Name = "tProjectNewItem";
			this.tProjectNewItem.Size = new Size(147, 22);
			this.tProjectNewItem.Text = "New Item";
			this.tProjectNewItem.Click += new EventHandler(this.AddNewItem);
			this.tProjectExistingItem.Image = Resources.ExistingItem;
			this.tProjectExistingItem.ImageTransparentColor = Color.Fuchsia;
			this.tProjectExistingItem.Name = "tProjectExistingItem";
			this.tProjectExistingItem.Size = new Size(147, 22);
			this.tProjectExistingItem.Text = "Existing Item";
			this.tProjectExistingItem.Click += new EventHandler(this.AddExistingItem);
			this.tProjectNewFolder.Image = Resources.NewFolder;
			this.tProjectNewFolder.ImageTransparentColor = Color.Fuchsia;
			this.tProjectNewFolder.Name = "tProjectNewFolder";
			this.tProjectNewFolder.Size = new Size(147, 22);
			this.tProjectNewFolder.Text = "New Folder";
			this.tProjectNewFolder.Click += new EventHandler(this.AddNewFolder);
			this.tProjectSep2.Name = "tProjectSep2";
			this.tProjectSep2.Size = new Size(194, 6);
			this.tProjectAddRef.ImageTransparentColor = Color.Fuchsia;
			this.tProjectAddRef.Name = "tProjectAddRef";
			this.tProjectAddRef.Size = new Size(197, 22);
			this.tProjectAddRef.Text = "Add Reference ...";
			this.tProjectAddRef.Click += new EventHandler(this.AddRefrence);
			this.tProjectSep3.Name = "tProjectSep3";
			this.tProjectSep3.Size = new Size(194, 6);
			this.tProjectProp.Image = Resources.Properties;
			this.tProjectProp.ImageTransparentColor = Color.Fuchsia;
			this.tProjectProp.Name = "tProjectProp";
			this.tProjectProp.Size = new Size(197, 22);
			this.tProjectProp.Text = "Properties";
			this.tProjectProp.Click += new EventHandler(this.ProjectProperties);
			this.cMenuFile.Items.AddRange(new ToolStripItem[]
			{
				this.tFileOpen,
				this.tFileSep1,
				this.tFileCut,
				this.tFileCopy,
				this.tFilePaste,
				this.tFileDelete,
				this.tFileSep2,
				this.tFileRename
			});
			this.cMenuFile.Name = "cMenuExplorer";
			this.cMenuFile.Size = new Size(125, 148);
			this.tFileOpen.Image = Resources.Open;
			this.tFileOpen.ImageTransparentColor = Color.Fuchsia;
			this.tFileOpen.Name = "tFileOpen";
			this.tFileOpen.Size = new Size(124, 22);
			this.tFileOpen.Text = "Open";
			this.tFileOpen.Click += new EventHandler(this.OpenItem);
			this.tFileSep1.Name = "tFileSep1";
			this.tFileSep1.Size = new Size(121, 6);
			this.tFileCut.Image = Resources.Cut;
			this.tFileCut.ImageTransparentColor = Color.Fuchsia;
			this.tFileCut.Name = "tFileCut";
			this.tFileCut.Size = new Size(124, 22);
			this.tFileCut.Text = "Cut";
			this.tFileCut.Click += new EventHandler(this.CutItem);
			this.tFileCopy.Image = Resources.Copy;
			this.tFileCopy.ImageTransparentColor = Color.Fuchsia;
			this.tFileCopy.Name = "tFileCopy";
			this.tFileCopy.Size = new Size(124, 22);
			this.tFileCopy.Text = "Copy";
			this.tFileCopy.Click += new EventHandler(this.CopyItem);
			this.tFilePaste.Image = Resources.Paste;
			this.tFilePaste.ImageTransparentColor = Color.Fuchsia;
			this.tFilePaste.Name = "tFilePaste";
			this.tFilePaste.Size = new Size(124, 22);
			this.tFilePaste.Text = "Paste";
			this.tFilePaste.Click += new EventHandler(this.PasteItem);
			this.tFileDelete.Image = Resources.Delete;
			this.tFileDelete.ImageTransparentColor = Color.Fuchsia;
			this.tFileDelete.Name = "tFileDelete";
			this.tFileDelete.Size = new Size(124, 22);
			this.tFileDelete.Text = "Delete";
			this.tFileDelete.Click += new EventHandler(this.DeleteItem);
			this.tFileSep2.Name = "tFileSep2";
			this.tFileSep2.Size = new Size(121, 6);
			this.tFileRename.Name = "tFileRename";
			this.tFileRename.Size = new Size(124, 22);
			this.tFileRename.Text = "Rename";
			this.tFileRename.Click += new EventHandler(this.RenameItem);
			this.tProjectAddWebRef.Name = "tProjectAddWebRef";
			this.tProjectAddWebRef.Size = new Size(197, 22);
			this.tProjectAddWebRef.Text = "Add Web Reference ...";
			this.tProjectAddWebRef.Click += new EventHandler(this.AddWebReference);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.ControlLight;
			base.ClientSize = new Size(292, 273);
			base.Controls.Add(this.tvwSolutionExplorer);
			this.DoubleBuffered = true;
			base.Icon = (Icon)resources.GetObject("$this.Icon");
			base.Name = "ProjectExplorer";
			base.TabText = "Project Explorer";
			this.Text = "Project Explorer";
			this.cMenuFolder.ResumeLayout(false);
			this.cMenuProject.ResumeLayout(false);
			this.cMenuFile.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
