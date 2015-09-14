using AIMS.Libraries.Scripting.ScriptControl.Parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
	public static class CodeCompletionDataUsageCache
	{
		private struct UsageStruct
		{
			public int Uses;

			public int ShowCount;

			public UsageStruct(int Uses, int ShowCount)
			{
				this.Uses = Uses;
				this.ShowCount = ShowCount;
			}
		}

		private class SaveItemsComparer : IComparer<KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct>>
		{
			public int Compare(KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct> x, KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct> y)
			{
				return ((double)x.Value.Uses / (double)x.Value.ShowCount).CompareTo((double)y.Value.Uses / (double)y.Value.ShowCount);
			}
		}

		private const long magic = 7306916068411589443L;

		private const short version = 1;

		private const int MinUsesForSave = 2;

		private static Dictionary<string, CodeCompletionDataUsageCache.UsageStruct> dict;

		public static string CacheFilename
		{
			get
			{
				return Path.Combine(ProjectParser.DomPersistencePath, "CodeCompletionUsageCache.dat");
			}
		}

		private static void LoadCache()
		{
			CodeCompletionDataUsageCache.dict = new Dictionary<string, CodeCompletionDataUsageCache.UsageStruct>();
			if (File.Exists(CodeCompletionDataUsageCache.CacheFilename))
			{
				using (FileStream fs = new FileStream(CodeCompletionDataUsageCache.CacheFilename, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader reader = new BinaryReader(fs))
					{
						if (reader.ReadInt64() == 7306916068411589443L)
						{
							if (reader.ReadInt16() == 1)
							{
								int itemCount = reader.ReadInt32();
								for (int i = 0; i < itemCount; i++)
								{
									string key = reader.ReadString();
									int uses = reader.ReadInt32();
									int showCount = reader.ReadInt32();
									if (showCount > 1000)
									{
										showCount /= 3;
										uses /= 3;
									}
									CodeCompletionDataUsageCache.dict.Add(key, new CodeCompletionDataUsageCache.UsageStruct(uses, showCount));
								}
							}
						}
					}
				}
			}
		}

		public static void SaveCache()
		{
			if (CodeCompletionDataUsageCache.dict != null)
			{
				using (FileStream fs = new FileStream(CodeCompletionDataUsageCache.CacheFilename, FileMode.Create, FileAccess.Write))
				{
					using (BinaryWriter writer = new BinaryWriter(fs))
					{
						int count = CodeCompletionDataUsageCache.SaveCache(writer);
					}
				}
			}
		}

		private static int SaveCache(BinaryWriter writer)
		{
			writer.Write(7306916068411589443L);
			writer.Write(1);
			int maxSaveItems = 50;
			int result;
			if (CodeCompletionDataUsageCache.dict.Count < maxSaveItems)
			{
				writer.Write(CodeCompletionDataUsageCache.dict.Count);
				foreach (KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct> entry in CodeCompletionDataUsageCache.dict)
				{
					writer.Write(entry.Key);
					writer.Write(entry.Value.Uses);
					writer.Write(entry.Value.ShowCount);
				}
				result = CodeCompletionDataUsageCache.dict.Count;
			}
			else
			{
				List<KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct>> saveItems = new List<KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct>>();
				foreach (KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct> entry in CodeCompletionDataUsageCache.dict)
				{
					KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct> entry;
					if (entry.Value.Uses > 2)
					{
						saveItems.Add(entry);
					}
				}
				if (saveItems.Count > maxSaveItems)
				{
					saveItems.Sort(new CodeCompletionDataUsageCache.SaveItemsComparer());
				}
				int count = Math.Min(maxSaveItems, saveItems.Count);
				writer.Write(count);
				for (int i = 0; i < count; i++)
				{
					KeyValuePair<string, CodeCompletionDataUsageCache.UsageStruct> entry = saveItems[i];
					writer.Write(entry.Key);
					writer.Write(entry.Value.Uses);
					writer.Write(entry.Value.ShowCount);
				}
				result = count;
			}
			return result;
		}

		public static void ResetCache()
		{
			CodeCompletionDataUsageCache.dict = new Dictionary<string, CodeCompletionDataUsageCache.UsageStruct>();
			try
			{
				if (File.Exists(CodeCompletionDataUsageCache.CacheFilename))
				{
					File.Delete(CodeCompletionDataUsageCache.CacheFilename);
				}
			}
			catch
			{
			}
		}

		public static double GetPriority(string dotnetName, bool incrementShowCount)
		{
			double result;
			if (!CodeCompletionOptions.DataUsageCacheEnabled)
			{
				result = 0.0;
			}
			else
			{
				if (CodeCompletionDataUsageCache.dict == null)
				{
					CodeCompletionDataUsageCache.LoadCache();
				}
				CodeCompletionDataUsageCache.UsageStruct usage;
				if (!CodeCompletionDataUsageCache.dict.TryGetValue(dotnetName, out usage))
				{
					result = 0.0;
				}
				else
				{
					double priority = (double)usage.Uses / (double)usage.ShowCount;
					if (usage.Uses < 2)
					{
						priority *= 0.2;
					}
					if (incrementShowCount)
					{
						usage.ShowCount++;
						CodeCompletionDataUsageCache.dict[dotnetName] = usage;
					}
					result = priority;
				}
			}
			return result;
		}

		public static void IncrementUsage(string dotnetName)
		{
			if (CodeCompletionOptions.DataUsageCacheEnabled)
			{
				if (CodeCompletionDataUsageCache.dict == null)
				{
					CodeCompletionDataUsageCache.LoadCache();
				}
				CodeCompletionDataUsageCache.UsageStruct usage;
				if (!CodeCompletionDataUsageCache.dict.TryGetValue(dotnetName, out usage))
				{
					usage = new CodeCompletionDataUsageCache.UsageStruct(0, 2);
				}
				usage.Uses++;
				CodeCompletionDataUsageCache.dict[dotnetName] = usage;
			}
		}
	}
}
