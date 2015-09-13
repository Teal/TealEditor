using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public abstract class ProjectItem : IDisposable
	{
		private IProject project;

		private volatile string fileNameCache;

		private string virtualInclude;

		private ItemType virtualItemType;

		private bool treatIncludeAsLiteral;

		private Dictionary<string, string> virtualMetadata = new Dictionary<string, string>();

		private bool disposed;

		[Browsable(false)]
		public IProject Project
		{
			get
			{
				return this.project;
			}
		}

		[Browsable(false)]
		public bool TreatIncludeAsLiteral
		{
			get
			{
				return this.treatIncludeAsLiteral;
			}
			set
			{
				this.treatIncludeAsLiteral = value;
			}
		}

		private object SyncRoot
		{
			get
			{
				return this.virtualMetadata;
			}
		}

		[Browsable(false)]
		public ItemType ItemType
		{
			get
			{
				return this.virtualItemType;
			}
			set
			{
				this.virtualItemType = value;
			}
		}

		[Browsable(false)]
		public string Include
		{
			get
			{
				return this.virtualInclude;
			}
			set
			{
				this.virtualInclude = (value ?? "");
			}
		}

		[Browsable(false)]
		public IEnumerable<string> MetadataNames
		{
			get
			{
				IEnumerable<string> result;
				lock (this.SyncRoot)
				{
					result = this.ToArray<string>(this.virtualMetadata.Keys);
				}
				return result;
			}
		}

		[Browsable(false)]
		public virtual string FileName
		{
			get
			{
				string result;
				if (this.project == null)
				{
					result = this.Include;
				}
				else
				{
					string fileName = this.fileNameCache;
					if (fileName == null)
					{
						lock (this.SyncRoot)
						{
							fileName = Path.Combine(this.project.Directory, this.Include);
							try
							{
								if (Path.IsPathRooted(fileName))
								{
									fileName = Path.GetFullPath(fileName);
								}
							}
							catch
							{
							}
							this.fileNameCache = fileName;
						}
					}
					result = fileName;
				}
				return result;
			}
			set
			{
				if (this.project == null)
				{
					throw new NotSupportedException("Not supported for items without project.");
				}
				this.Include = FileUtility.GetRelativePath(this.project.Directory, value);
			}
		}

		[Browsable(false)]
		public bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		protected ProjectItem(IProject project, ItemType itemType) : this(project, itemType, null)
		{
		}

		protected ProjectItem(IProject project, ItemType itemType, string include) : this(project, itemType, include, true)
		{
		}

		protected ProjectItem(IProject project, ItemType itemType, string include, bool treatIncludeAsLiteral)
		{
			this.project = project;
			this.virtualItemType = itemType;
			this.virtualInclude = (include ?? "");
			this.virtualMetadata = new Dictionary<string, string>();
			this.treatIncludeAsLiteral = treatIncludeAsLiteral;
		}

		public bool HasMetadata(string metadataName)
		{
			bool result;
			lock (this.SyncRoot)
			{
				result = this.virtualMetadata.ContainsKey(metadataName);
			}
			return result;
		}

		public string GetEvaluatedMetadata(string metadataName)
		{
			string result;
			lock (this.SyncRoot)
			{
				string val;
				this.virtualMetadata.TryGetValue(metadataName, out val);
				if (val == null)
				{
					result = "";
				}
				else
				{
					result = this.Unescape(val);
				}
			}
			return result;
		}

		public T GetEvaluatedMetadata<T>(string metadataName, T defaultValue)
		{
			return GenericConverter.FromString<T>(this.GetEvaluatedMetadata(metadataName), defaultValue);
		}

		public string GetMetadata(string metadataName)
		{
			string result;
			lock (this.SyncRoot)
			{
				string val;
				this.virtualMetadata.TryGetValue(metadataName, out val);
				result = (val ?? "");
			}
			return result;
		}

		public void SetEvaluatedMetadata<T>(string metadataName, T value)
		{
			this.SetEvaluatedMetadata(metadataName, GenericConverter.ToString<T>(value));
		}

		public void SetEvaluatedMetadata(string metadataName, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.RemoveMetadata(metadataName);
			}
			else
			{
				lock (this.SyncRoot)
				{
					this.virtualMetadata[metadataName] = this.Escape(value);
				}
			}
		}

		public void SetMetadata(string metadataName, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.RemoveMetadata(metadataName);
			}
			else
			{
				lock (this.SyncRoot)
				{
					this.virtualMetadata[metadataName] = value;
				}
			}
		}

		public void RemoveMetadata(string metadataName)
		{
			lock (this.SyncRoot)
			{
				this.virtualMetadata.Remove(metadataName);
			}
		}

		private T[] ToArray<T>(IEnumerable<T> input)
		{
			T[] result;
			if (input is ICollection<T>)
			{
				ICollection<T> c = (ICollection<T>)input;
				T[] arr = new T[c.Count];
				c.CopyTo(arr, 0);
				result = arr;
			}
			else
			{
				result = new List<T>(input).ToArray();
			}
			return result;
		}

		private string Escape(string text)
		{
			return text;
		}

		private string Unescape(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			StringBuilder b = null;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '%' && i + 2 < text.Length)
				{
					if (b == null)
					{
						b = new StringBuilder(text, 0, i, text.Length);
					}
					string a = text[i + 1].ToString() + text[i + 2].ToString();
					int num;
					if (int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
					{
						b.Append((char)num);
						i += 2;
					}
					else
					{
						b.Append('%');
					}
				}
				else if (b != null)
				{
					b.Append(c);
				}
			}
			string result;
			if (b != null)
			{
				result = b.ToString();
			}
			else
			{
				result = text;
			}
			return result;
		}

		public virtual void CopyMetadataTo(ProjectItem targetItem)
		{
			lock (this.SyncRoot)
			{
				lock (targetItem.SyncRoot)
				{
					foreach (string name in this.MetadataNames)
					{
						targetItem.SetMetadata(name, this.GetMetadata(name));
					}
				}
			}
		}

		public virtual void Dispose()
		{
			this.disposed = true;
		}

		public override string ToString()
		{
			return string.Format("[{0}: <{1} Include='{2}'>]", base.GetType().Name, this.ItemType.ItemName, this.Include);
		}
	}
}
