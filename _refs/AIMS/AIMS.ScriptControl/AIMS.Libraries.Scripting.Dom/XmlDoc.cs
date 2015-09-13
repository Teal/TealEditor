using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace AIMS.Libraries.Scripting.Dom
{
	public sealed class XmlDoc : IDisposable
	{
		private struct IndexEntry : IComparable<XmlDoc.IndexEntry>
		{
			public int HashCode;

			public int FileLocation;

			public int CompareTo(XmlDoc.IndexEntry other)
			{
				return this.HashCode.CompareTo(other.HashCode);
			}

			public IndexEntry(int HashCode, int FileLocation)
			{
				this.HashCode = HashCode;
				this.FileLocation = FileLocation;
			}
		}

		private const int cacheLength = 150;

		private const long magic = 4775050834460044632L;

		private const short version = 2;

		private static readonly List<string> xmlDocLookupDirectories = new List<string>(new string[]
		{
			RuntimeEnvironment.GetRuntimeDirectory()
		});

		private Dictionary<string, string> xmlDescription = new Dictionary<string, string>();

		private XmlDoc.IndexEntry[] index;

		private Queue<string> keyCacheQueue;

		private BinaryReader loader;

		private FileStream fs;

		public static IList<string> XmlDocLookupDirectories
		{
			get
			{
				return XmlDoc.xmlDocLookupDirectories;
			}
		}

		private void ReadMembersSection(XmlReader reader)
		{
			while (reader.Read())
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType == XmlNodeType.EndElement)
					{
						if (reader.LocalName == "members")
						{
							break;
						}
					}
				}
				else if (reader.LocalName == "member")
				{
					string memberAttr = reader.GetAttribute(0);
					string innerXml = reader.ReadInnerXml();
					this.xmlDescription[memberAttr] = innerXml;
				}
			}
		}

		public string GetDocumentation(string key)
		{
			if (this.xmlDescription == null)
			{
				throw new ObjectDisposedException("XmlDoc");
			}
			string result2;
			lock (this.xmlDescription)
			{
				string result;
				if (this.xmlDescription.TryGetValue(key, out result))
				{
					result2 = result;
				}
				else if (this.index == null)
				{
					result2 = null;
				}
				else
				{
					result2 = this.LoadDocumentation(key);
				}
			}
			return result2;
		}

		private void Save(string fileName, DateTime fileDate)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (BinaryWriter w = new BinaryWriter(fs))
				{
					w.Write(4775050834460044632L);
					w.Write(2);
					w.Write(fileDate.Ticks);
					XmlDoc.IndexEntry[] index = new XmlDoc.IndexEntry[this.xmlDescription.Count];
					w.Write(index.Length);
					int indexPointerPos = (int)fs.Position;
					w.Write(0);
					int i = 0;
					foreach (KeyValuePair<string, string> p in this.xmlDescription)
					{
						index[i] = new XmlDoc.IndexEntry(p.Key.GetHashCode(), (int)fs.Position);
						w.Write(p.Key);
						w.Write(p.Value.Trim());
						i++;
					}
					Array.Sort<XmlDoc.IndexEntry>(index);
					int indexStart = (int)fs.Position;
					XmlDoc.IndexEntry[] array = index;
					for (int j = 0; j < array.Length; j++)
					{
						XmlDoc.IndexEntry entry = array[j];
						w.Write(entry.HashCode);
						w.Write(entry.FileLocation);
					}
					w.Seek(indexPointerPos, SeekOrigin.Begin);
					w.Write(indexStart);
				}
			}
		}

		private bool LoadFromBinary(string fileName, DateTime fileDate)
		{
			this.keyCacheQueue = new Queue<string>(150);
			this.fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			int len = (int)this.fs.Length;
			this.loader = new BinaryReader(this.fs);
			bool result;
			try
			{
				if (this.loader.ReadInt64() != 4775050834460044632L)
				{
					LoggingService.Warn("Cannot load XmlDoc: wrong magic");
					result = false;
				}
				else if (this.loader.ReadInt16() != 2)
				{
					LoggingService.Warn("Cannot load XmlDoc: wrong version");
					result = false;
				}
				else if (this.loader.ReadInt64() != fileDate.Ticks)
				{
					LoggingService.Info("Not loading XmlDoc: file changed since cache was created");
					result = false;
				}
				else
				{
					int count = this.loader.ReadInt32();
					int indexStartPosition = this.loader.ReadInt32();
					if (indexStartPosition >= len)
					{
						LoggingService.Error("XmlDoc: Cannot find index, cache invalid!");
						result = false;
					}
					else
					{
						this.fs.Position = (long)indexStartPosition;
						XmlDoc.IndexEntry[] index = new XmlDoc.IndexEntry[count];
						for (int i = 0; i < index.Length; i++)
						{
							index[i] = new XmlDoc.IndexEntry(this.loader.ReadInt32(), this.loader.ReadInt32());
						}
						this.index = index;
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				LoggingService.Error("Cannot load from cache", ex);
				result = false;
			}
			return result;
		}

		private string LoadDocumentation(string key)
		{
			if (this.keyCacheQueue.Count > 149)
			{
				this.xmlDescription.Remove(this.keyCacheQueue.Dequeue());
			}
			int hashcode = key.GetHashCode();
			string resultDocu = null;
			int i = Array.BinarySearch<XmlDoc.IndexEntry>(this.index, new XmlDoc.IndexEntry(hashcode, 0));
			if (i >= 0)
			{
				while (--i >= 0 && this.index[i].HashCode == hashcode)
				{
				}
				while (++i < this.index.Length && this.index[i].HashCode == hashcode)
				{
					this.fs.Position = (long)this.index[i].FileLocation;
					string keyInFile = this.loader.ReadString();
					if (keyInFile == key)
					{
						resultDocu = this.loader.ReadString();
						break;
					}
					LoggingService.Warn("Found " + keyInFile + " instead of " + key);
				}
			}
			this.keyCacheQueue.Enqueue(key);
			this.xmlDescription.Add(key, resultDocu);
			return resultDocu;
		}

		public void Dispose()
		{
			if (this.loader != null)
			{
				this.loader.Close();
				this.fs.Close();
			}
			this.xmlDescription = null;
			this.index = null;
			this.keyCacheQueue = null;
			this.loader = null;
			this.fs = null;
		}

		public static XmlDoc Load(XmlReader reader)
		{
			XmlDoc newXmlDoc = new XmlDoc();
			while (reader.Read())
			{
				if (reader.IsStartElement())
				{
					string localName = reader.LocalName;
					if (localName != null)
					{
						if (localName == "members")
						{
							newXmlDoc.ReadMembersSection(reader);
						}
					}
				}
			}
			return newXmlDoc;
		}

		public static XmlDoc Load(string fileName, string cachePath)
		{
			LoggingService.Debug("Loading XmlDoc for " + fileName);
			Directory.CreateDirectory(cachePath);
			string cacheName = string.Concat(new string[]
			{
				cachePath,
				"/",
				Path.GetFileNameWithoutExtension(fileName),
				".",
				fileName.GetHashCode().ToString("x"),
				".dat"
			});
			XmlDoc doc;
			XmlDoc result;
			if (File.Exists(cacheName))
			{
				doc = new XmlDoc();
				if (doc.LoadFromBinary(cacheName, File.GetLastWriteTimeUtc(fileName)))
				{
					LoggingService.Debug("XmlDoc: Load from cache successful");
					result = doc;
					return result;
				}
				doc.Dispose();
				try
				{
					File.Delete(cacheName);
				}
				catch
				{
				}
			}
			try
			{
				using (XmlTextReader xmlReader = new XmlTextReader(fileName))
				{
					doc = XmlDoc.Load(xmlReader);
				}
			}
			catch (XmlException ex)
			{
				LoggingService.Warn("Error loading XmlDoc", ex);
				result = new XmlDoc();
				return result;
			}
			if (doc.xmlDescription.Count > 300)
			{
				LoggingService.Debug("XmlDoc: Creating cache");
				DateTime date = File.GetLastWriteTimeUtc(fileName);
				try
				{
					doc.Save(cacheName, date);
				}
				catch (Exception ex2)
				{
					LoggingService.Error("Cannot write to cache file", ex2);
					result = doc;
					return result;
				}
				doc.Dispose();
				doc = new XmlDoc();
				doc.LoadFromBinary(cacheName, date);
			}
			result = doc;
			return result;
		}
	}
}
