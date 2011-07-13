using System;
using Gtk;
using Magnis.Web.Services.LibertyReserve;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace LRDemo
{
	public static class UIHelper
	{
		public static void FillCurrencyComboBox(ComboBox combobox)
		{
			combobox.Clear();
			
			var cell = new CellRendererText();
			combobox.PackStart(cell, false);
			combobox.AddAttribute(cell, "text", 0);
			
			var store = new ListStore(typeof(string), typeof(Currency));
			combobox.Model = store;
			var currencies = new SortedList<string, Currency>();
			foreach (int value in Enum.GetValues(typeof(Currency))) 
			{
				Currency c = (Currency)value;
				currencies.Add(c.ToString(), c);
			}
			foreach (KeyValuePair<string, Currency> pair in currencies)
				store.AppendValues(pair.Key, pair.Value);
			
			TreeIter firstIter;
			if (combobox.Model.GetIterFirst(out firstIter))
				combobox.SetActiveIter(firstIter);
		}
		
		public static void DisplayError(Window parent, string message)
		{
			using (var dlg = new MessageDialog(parent, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, message)) 
			{
				dlg.Run();
				dlg.Destroy();
			}
		}
		
		public static void DisplayResponseErrors(Window parent, Response response)
		{
			if (response.Errors.Count > 0)
			{
				var sb = new StringBuilder("The server returned the following errors:");
				sb.AppendLine();
				foreach (ApiError error in response.Errors)
				{
					sb.AppendFormat("{0}: {1}", error.Code, error.Text);
					sb.AppendLine();
				}
				UIHelper.DisplayError(parent, sb.ToString());
			}
		}
		
		public static bool ValidateEmptyEntry(Entry entry, string errorMessage, bool trim = true)
		{
			string value = entry.Text;
			if (trim)
				value = value.Trim();
			
			if (String.IsNullOrEmpty(value)) 
			{
				entry.GrabFocus();
				UIHelper.DisplayError((Window)entry.Toplevel, errorMessage);
				
				return false;
			}
			
			return true;
		}
		
		public static bool ValidateEmptyComboBox(ComboBox combobox, string errorMessage)
		{
			TreeIter currencyIter;
			if (!combobox.GetActiveIter(out currencyIter)) 
			{
				combobox.GrabFocus();
				UIHelper.DisplayError((Gtk.Window)combobox.Toplevel, errorMessage);
				
				return false;
			}
			
			return true;
		}
	}
}

