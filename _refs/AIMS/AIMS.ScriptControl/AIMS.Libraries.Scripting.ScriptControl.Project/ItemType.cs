using System;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
	public struct ItemType : IEquatable<ItemType>, IComparable<ItemType>
	{
		public static readonly ItemType Reference = new ItemType("Reference");

		public static readonly ItemType ProjectReference = new ItemType("ProjectReference");

		public static readonly ItemType COMReference = new ItemType("COMReference");

		public static readonly ItemType Import = new ItemType("Import");

		public static readonly ItemType WebReferenceUrl = new ItemType("WebReferenceUrl");

		public static readonly ItemType WebReferences = new ItemType("WebReferences");

		public static readonly ItemType Content = new ItemType("Content");

		public static readonly ItemType None = new ItemType("None");

		public static readonly ItemType Compile = new ItemType("Compile");

		public static readonly ReadOnlyCollectionWrapper<ItemType> DefaultFileItems = new Set<ItemType>(new ItemType[]
		{
			ItemType.Compile,
			ItemType.None,
			ItemType.Content
		}).AsReadOnly();

		private readonly string itemName;

		public string ItemName
		{
			get
			{
				return this.itemName;
			}
		}

		public ItemType(string itemName)
		{
			if (itemName == null)
			{
				throw new ArgumentNullException("itemName");
			}
			this.itemName = itemName;
		}

		public override string ToString()
		{
			return this.itemName;
		}

		public override bool Equals(object obj)
		{
			return obj is ItemType && this.Equals((ItemType)obj);
		}

		public bool Equals(ItemType other)
		{
			return this.itemName == other.itemName;
		}

		public override int GetHashCode()
		{
			return this.itemName.GetHashCode();
		}

		public static bool operator ==(ItemType lhs, ItemType rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ItemType lhs, ItemType rhs)
		{
			return !lhs.Equals(rhs);
		}

		public int CompareTo(ItemType other)
		{
			return this.itemName.CompareTo(other.itemName);
		}
	}
}
