using System;
using Magnis.Web.Services.LibertyReserve;
using System.Collections.Generic;

namespace LRDemo
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SCIPaymentRequestWidget : Gtk.Bin
	{
		public IApiCredentialsProvider ApiCredentialsProvider { get; set; }
		
		public SCIPaymentRequestWidget ()
		{
			this.Build ();
			
			UIHelper.FillCurrencyComboBox(cmbCurrency, true);
		}
		
		#region Validation
		
		private bool ValidateRequestParams()
		{
			if (!ApiCredentialsProvider.Validate())
				return false;
			if (!UIHelper.ValidateEmptyEntry(txtMerchantAccount, "Merchant's account number is missing."))
				return false;
			if (!String.IsNullOrEmpty(txtSuccessUrl.Text) && 
				!UIHelper.ValidateUrl(txtSuccessUrl, "Success ur is incorrect."))
				return false;
			if (!String.IsNullOrEmpty(txtFailUrl.Text) && 
				!UIHelper.ValidateUrl(txtFailUrl, "Fail url is incorrect."))
				return false;
			if (!String.IsNullOrEmpty(txtStatusUrl.Text) && 
				!UIHelper.ValidateUrl(txtStatusUrl, "Status url is incorrect."))
				return false;
			
			return true;
		}
		
		#endregion
		
		#region UI
		
		private HttpMethod GetSuccessUrlMethod()
		{
			if (rbtSuccessGetMethod.State == Gtk.StateType.Active)
				return HttpMethod.GET;
			else if (rbtSuccessPostMethod.State == Gtk.StateType.Active)
				return HttpMethod.POST;
			else
				return HttpMethod.LINK;
		}
		
		private HttpMethod GetFailUrlMethod()
		{
			if (rbtFailGetMethod.State == Gtk.StateType.Active)
				return HttpMethod.GET;
			else if (rbtFailPostMethod.State == Gtk.StateType.Active)
				return HttpMethod.POST;
			else
				return HttpMethod.LINK;
		}
		
		private HttpMethod GetStatusUrlMethod()
		{
			if (rbtStatusGetMethod.State == Gtk.StateType.Active)
				return HttpMethod.GET;
			else if (rbtStatusPostMethod.State == Gtk.StateType.Active)
				return HttpMethod.POST;
			else
				return HttpMethod.LINK;
		}
		
		#endregion
		
		#region Request routines
		
		private SCIRequest PrepareRequest()
		{
			var sci = new SCIRequest
			{
				MerchantAccount = txtMerchantAccount.Text.Trim(),
				BuyerAccount = txtBuyerAccount.Text.Trim(),
				Store = txtStore.Text.Trim(),
				Amount = txtAmount.Value,
				Currency = UIHelper.GetCurrency(cmbCurrency),
				Comments = txtComments.Buffer.Text,
				MerchantRef = txtMerchantRef.Text,
				SuccessUrl = UIHelper.GetUri(txtSuccessUrl),
				SuccessUrlMethod = GetSuccessUrlMethod(),
				FailUrl = UIHelper.GetUri(txtFailUrl),
				FailUrlMethod = GetFailUrlMethod(),
				StatusUrl = UIHelper.GetUri(txtStatusUrl),
				StatusUrlMethod = GetStatusUrlMethod(),
				BaggageFields = new Dictionary<string, string>(),
			};
			var lineSeparator = new string[] { Environment.NewLine };
			string[] lines = txtBaggageFields.Buffer.Text.Split(lineSeparator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in lines)
			{
				string[] values = line.Split('=');
				if (values.Length > 1)
					sci.BaggageFields.Add(values[0], values[1]);
			}
			
			return sci;
		}
		
		private void GenerateForm()
		{
			txtPaymentForm.Buffer.Clear();
			if (ValidateRequestParams())
			{
				try
				{
					SCIRequest sci = PrepareRequest();
					string submitButtonHtml = "<input type='submit' value='Pay'/>";
					txtPaymentForm.Buffer.Text = sci.GeneratePaymentForm(submitButtonHtml);
				}
				catch (Exception e)
				{
					string message = "Generate request form failed: " + e.Message;
					UIHelper.DisplayError((Gtk.Window)Toplevel, message);
				}
			}
		}
		
		#endregion
		
		#region Event handlers
		
		protected void OnGenerateFormClicked (object sender, System.EventArgs e)
		{
			GenerateForm();
		}
		
		#endregion
	}
}

