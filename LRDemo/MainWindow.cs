using System;
using Gtk;
using LRDemo;
using Magnis.Web.Services.LibertyReserve;

public partial class MainWindow: Gtk.Window, IApiCredentialsProvider
{	
	public MainWindow(): base (Gtk.WindowType.Toplevel)
	{
		Build();
		
		balanceWidget.ApiCredentialsProvider = this;
		accountWidget.ApiCredentialsProvider = this;
		transferWidget.ApiCredentialsProvider = this;
	}
	
	#region IApiCredentialsProvider Members
	
	public bool Validate()
	{
		if (!UIHelper.ValidateEmptyEntry(txtAccountNumber, "Account number is not specified."))
			return false;
		if (!UIHelper.ValidateEmptyEntry(txtApiName, "API name is not specified."))
			return false;
		if (!UIHelper.ValidateEmptyEntry(txtSecurityWord, "Security word is not specified.", false))
			return false;
		
		return true;
	}
	
	public ApiCredentials Credentials 
	{
		get 
		{
			return new ApiCredentials
				{
					AccountNumber = txtAccountNumber.Text.Trim(),
					ApiName = txtApiName.Text.Trim(),
					SecurityWord = txtSecurityWord.Text,
				};
		}
	}
	
	#endregion
	
	#region Event handlers
	
	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}
	
	#endregion
}
