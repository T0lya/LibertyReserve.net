using System;
using System.Linq;
using System.Reflection;

namespace LRDemo
{
	public static class NodeViewExtensions
	{
		public static void AutoGenerateColumns(this Gtk.NodeView view, Type type)
		{
			foreach (FieldInfo field in type.GetFields())
			{
				object[] customAttributes = field.GetCustomAttributes(true);
				AppendColumn(view, field.Name, customAttributes);
			}
			foreach (PropertyInfo prop in type.GetProperties())
			{
				object[] customAttributes = prop.GetCustomAttributes(true);
				AppendColumn(view, prop.Name, customAttributes);
			}
		}
		
		private static void AppendColumn(Gtk.NodeView view, string propName, object[] customAttributes)
		{
			Gtk.TreeNodeValueAttribute valueAttr = customAttributes.OfType<Gtk.TreeNodeValueAttribute>().FirstOrDefault();
			if (valueAttr != null)
			{
				
				TreeNodeColumnAttribute columnAttr = customAttributes.OfType<TreeNodeColumnAttribute>().FirstOrDefault();
				string columnName = columnAttr != null ? columnAttr.Name : propName;
				view.AppendColumn(columnName, new Gtk.CellRendererText(), "text", valueAttr.Column);
			}
		}
	}
}

