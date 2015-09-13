using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.Services.Description;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
	public class WebServicesView : UserControl
	{
		private const int ServiceDescriptionImageIndex = 0;

		private const int ServiceImageIndex = 1;

		private const int PortImageIndex = 2;

		private const int OperationImageIndex = 3;

		private ImageList WebServiceImageList;

		private IContainer components;

		private ColumnHeader propertyColumnHeader;

		private ColumnHeader valueColumnHeader;

		private TreeView webServicesTreeView;

		private ListView webServicesListView;

		private SplitContainer splitContainer;

		public WebServicesView()
		{
			this.InitializeComponent();
			this.webServicesTreeView.ImageList = this.WebServiceImageList;
		}

		public void Clear()
		{
			this.webServicesListView.Items.Clear();
			this.webServicesTreeView.Nodes.Clear();
		}

		public void Add(ServiceDescriptionCollection serviceDescriptions)
		{
			if (serviceDescriptions.Count != 0)
			{
				this.webServicesListView.BeginUpdate();
				try
				{
					foreach (ServiceDescription description in serviceDescriptions)
					{
						this.Add(description);
					}
				}
				finally
				{
					this.webServicesListView.EndUpdate();
				}
			}
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(WebServicesView));
			this.splitContainer = new SplitContainer();
			this.webServicesTreeView = new TreeView();
			this.webServicesListView = new ListView();
			this.propertyColumnHeader = new ColumnHeader();
			this.valueColumnHeader = new ColumnHeader();
			this.WebServiceImageList = new ImageList(this.components);
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			base.SuspendLayout();
			this.splitContainer.Dock = DockStyle.Fill;
			this.splitContainer.Location = new Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Panel1.Controls.Add(this.webServicesTreeView);
			this.splitContainer.Panel2.Controls.Add(this.webServicesListView);
			this.splitContainer.Size = new Size(471, 305);
			this.splitContainer.SplitterDistance = 156;
			this.splitContainer.TabIndex = 1;
			this.webServicesTreeView.Dock = DockStyle.Fill;
			this.webServicesTreeView.Location = new Point(0, 0);
			this.webServicesTreeView.Name = "webServicesTreeView";
			this.webServicesTreeView.Size = new Size(156, 305);
			this.webServicesTreeView.TabIndex = 0;
			this.webServicesTreeView.AfterSelect += new TreeViewEventHandler(this.WebServicesTreeViewAfterSelect);
			this.webServicesListView.Columns.AddRange(new ColumnHeader[]
			{
				this.propertyColumnHeader,
				this.valueColumnHeader
			});
			this.webServicesListView.Dock = DockStyle.Fill;
			this.webServicesListView.Location = new Point(0, 0);
			this.webServicesListView.Name = "webServicesListView";
			this.webServicesListView.Size = new Size(311, 305);
			this.webServicesListView.TabIndex = 2;
			this.webServicesListView.UseCompatibleStateImageBehavior = false;
			this.webServicesListView.View = View.Details;
			this.propertyColumnHeader.Text = "Property";
			this.propertyColumnHeader.Width = 120;
			this.valueColumnHeader.Text = "Value";
			this.valueColumnHeader.Width = 191;
			this.WebServiceImageList.ImageStream = (ImageListStreamer)resources.GetObject("WebServiceImageList.ImageStream");
			this.WebServiceImageList.TransparentColor = Color.Transparent;
			this.WebServiceImageList.Images.SetKeyName(0, "Icons.16x16.Library");
			this.WebServiceImageList.Images.SetKeyName(1, "Icons.16x16.Interface");
			this.WebServiceImageList.Images.SetKeyName(2, "Icons.16x16.Class");
			this.WebServiceImageList.Images.SetKeyName(3, "Icons.16x16.Method");
			base.Controls.Add(this.splitContainer);
			base.Name = "WebServicesView";
			base.Size = new Size(471, 305);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void WebServicesTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			this.webServicesListView.Items.Clear();
			if (e.Node.Tag is ServiceDescription)
			{
				ServiceDescription desc = (ServiceDescription)e.Node.Tag;
				ListViewItem item = new ListViewItem();
				item.Text = "RetrievalUri";
				item.SubItems.Add(desc.RetrievalUrl);
				this.webServicesListView.Items.Add(item);
			}
			else if (e.Node.Tag is Service)
			{
				Service service = (Service)e.Node.Tag;
				ListViewItem item = new ListViewItem();
				item.Text = "Documentation";
				item.SubItems.Add(service.Documentation);
				this.webServicesListView.Items.Add(item);
			}
			else if (e.Node.Tag is Port)
			{
				Port port = (Port)e.Node.Tag;
				ListViewItem item = new ListViewItem();
				item.Text = "Documentation";
				item.SubItems.Add(port.Documentation);
				this.webServicesListView.Items.Add(item);
				item = new ListViewItem();
				item.Text = "Binding";
				item.SubItems.Add(port.Binding.Name);
				this.webServicesListView.Items.Add(item);
				item = new ListViewItem();
				item.Text = "ServiceName";
				item.SubItems.Add(port.Service.Name);
				this.webServicesListView.Items.Add(item);
			}
			else if (e.Node.Tag is Operation)
			{
				Operation operation = (Operation)e.Node.Tag;
				ListViewItem item = new ListViewItem();
				item.Text = "Documentation";
				item.SubItems.Add(operation.Documentation);
				this.webServicesListView.Items.Add(item);
				item = new ListViewItem();
				item.Text = "Parameters";
				item.SubItems.Add(operation.ParameterOrderString);
				this.webServicesListView.Items.Add(item);
			}
		}

		private void Add(ServiceDescription description)
		{
			TreeNode rootNode = new TreeNode(this.GetName(description));
			rootNode.Tag = description;
			rootNode.ImageIndex = 0;
			rootNode.SelectedImageIndex = 0;
			this.webServicesTreeView.Nodes.Add(rootNode);
			foreach (Service service in description.Services)
			{
				TreeNode serviceNode = new TreeNode(service.Name);
				serviceNode.Tag = service;
				serviceNode.ImageIndex = 1;
				serviceNode.SelectedImageIndex = 1;
				rootNode.Nodes.Add(serviceNode);
				foreach (Port port in service.Ports)
				{
					TreeNode portNode = new TreeNode(port.Name);
					portNode.Tag = port;
					portNode.ImageIndex = 2;
					portNode.SelectedImageIndex = 2;
					serviceNode.Nodes.Add(portNode);
					System.Web.Services.Description.Binding binding = description.Bindings[port.Binding.Name];
					if (binding != null)
					{
						PortType portType = description.PortTypes[binding.Type.Name];
						if (portType != null)
						{
							foreach (Operation operation in portType.Operations)
							{
								TreeNode operationNode = new TreeNode(operation.Name);
								operationNode.Tag = operation;
								operationNode.ImageIndex = 3;
								operationNode.SelectedImageIndex = 3;
								portNode.Nodes.Add(operationNode);
							}
						}
					}
				}
			}
			this.webServicesTreeView.ExpandAll();
		}

		private string GetName(ServiceDescription description)
		{
			string result;
			if (description.Name != null)
			{
				result = description.Name;
			}
			else if (description.RetrievalUrl != null)
			{
				Uri uri = new Uri(description.RetrievalUrl);
				if (uri.Segments.Length > 0)
				{
					result = uri.Segments[uri.Segments.Length - 1];
				}
				else
				{
					result = uri.Host;
				}
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}
	}
}
