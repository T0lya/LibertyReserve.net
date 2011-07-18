using System;

namespace LRDemo
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class TreeNodeColumnAttribute : Attribute
	{
		public string Name { get; set; }
		
		public TreeNodeColumnAttribute(string name)
		{
			Name = name;
		}
	}
}

