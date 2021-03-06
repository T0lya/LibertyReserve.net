using System;
using Magnis.Web.Services.LibertyReserve;

namespace LRDemo
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class HistoryWidget : Gtk.Bin
	{
		#region Declarations
		
		[Gtk.TreeNode(ListOnly = true)]
		class HistoryTreeNode : Gtk.TreeNode
		{
			[Gtk.TreeNodeValue(Column = 0)]
			[TreeNodeColumn("Receipt ID")]
			public string ReceiptId { get; set; }
			
			[Gtk.TreeNodeValue(Column = 1)]
			[TreeNodeColumn("Transfer ID")]
			public string TransferId { get; set; }
			
			[Gtk.TreeNodeValue(Column = 2)]
			[TreeNodeColumn("Date")]
			public string Date { get; set; }
			
			[Gtk.TreeNodeValue(Column = 3)]
			[TreeNodeColumn("Payer")]
			public string Payer { get; set; }
			
			[Gtk.TreeNodeValue(Column = 4)]
			[TreeNodeColumn("Payer Name")]
			public string PayerName { get; set; }
			
			[Gtk.TreeNodeValue(Column = 5)]
			[TreeNodeColumn("Payee")]
			public string Payee { get; set; }
			
			[Gtk.TreeNodeValue(Column = 6)]
			[TreeNodeColumn("Payee Name")]
			public string PayeeName { get; set; }
			
			[Gtk.TreeNodeValue(Column = 7)]
			[TreeNodeColumn("Currency")]
			public string Currency { get; set; }
			
			[Gtk.TreeNodeValue(Column = 8)]
			[TreeNodeColumn("Amount")]
			public string Amount { get; set; }
						
			[Gtk.TreeNodeValue(Column = 9)]
			[TreeNodeColumn("Closing Balance")]
			public string ClosingBalance { get; set; }
			
			[Gtk.TreeNodeValue(Column = 10)]
			[TreeNodeColumn("Fee")]
			public string Fee { get; set; }
			
			[Gtk.TreeNodeValue(Column = 11)]
			[TreeNodeColumn("Source")]
			public string Source { get; set; }
			
			[Gtk.TreeNodeValue(Column = 12)]
			[TreeNodeColumn("Anonymous")]
			public string Anonymous { get; set; }
			
			[Gtk.TreeNodeValue(Column = 13)]
			[TreeNodeColumn("Description")]
			public string Description { get; set; }
		}
		
		#endregion

		public IApiCredentialsProvider ApiCredentialsProvider { get; set; }
		
		private Gtk.NodeStore historyStore = new Gtk.NodeStore(typeof(HistoryTreeNode));
		
		public HistoryWidget ()
		{
			this.Build ();

			FillAnonymity();
			FillDirection();
			UIHelper.FillCurrencyComboBox(cmbCurrency, true);
			InitHistoryNodeView();
		}
		
		#region Initialization
				
		private void InitHistoryNodeView()
		{
			historyNodeView.NodeStore = historyStore;
			historyNodeView.AutoGenerateColumns(typeof(HistoryTreeNode));
			historyNodeView.ShowAll();
		}
		
		private void FillAnonymity()
		{
			foreach (string item in Enum.GetNames(typeof(Anonymity)))
				cmbAnonymous.AppendText(item);
		}
		
		private void FillDirection()
		{
			foreach (string item in Enum.GetNames(typeof(TransactionDirection)))
				cmbDirection.AppendText(item);
		}
		
		#endregion
		
		#region UI
			
		private Anonymity GetAnonimity()
		{
			string value = cmbAnonymous.ActiveText;
			
			if (String.IsNullOrEmpty(value))
				return Anonymity.Any;
			else
				return (Anonymity)Enum.Parse(typeof(Anonymity), value);
		}
		
		private TransactionDirection GetDirection()
		{
			string value = cmbDirection.ActiveText;
			
			if (String.IsNullOrEmpty(value))
				return TransactionDirection.Any;
			else
				return (TransactionDirection)Enum.Parse(typeof(TransactionDirection), value);
		}
		
		private void ShowHistory(HistoryResponse response)
		{
			txtRawResponse.Buffer.Text = response.ResponseText;
			foreach (Receipt r in response.Receipts)
			{
				var node = new HistoryTreeNode
				{
					ReceiptId = r.ReceiptId,
					TransferId = r.Transfer.TransferId,
					Date = LRConverter.ToString(r.Timestamp),
					Payer = r.Transfer.Payer,
					PayerName = r.PayerName,
					Payee = r.Transfer.Payee,
					PayeeName = r.PayeeName,
					Currency = LRConverter.ToString(r.Transfer.Currency),
					Amount = LRConverter.ToString(r.Transfer.Amount),
					ClosingBalance = LRConverter.ToString(r.ClosingBalance),
					Fee = LRConverter.ToString(r.Fee),
					Source = r.Transfer.Source,
					Anonymous = r.Transfer.Anonymous.ToString(),
					Description = r.Transfer.Description,
				};
				historyStore.AddNode(node);
			}
		}
		
		private void ClearResponseData()
		{
			historyStore.Clear();
			txtRawResponse.Buffer.Clear();
		}
		
		#endregion
		
		#region Request routines
		
		private void SendRequest()
		{
			if (ValidateRequestParams())
			{
				ClearResponseData();
				
				HistoryRequest request = PrepareRequest();
				request.Auth = AuthToken.FromApiCredentials(ApiCredentialsProvider.Credentials);
				try
				{
					HistoryResponse response = request.GetResponse();
					ShowHistory(response);
					UIHelper.DisplayResponseErrors((Gtk.Window)Toplevel, response);
				}
				catch (LibertyReserveException e)
				{
					string message = "Process response failed: " + e.Message;
					UIHelper.DisplayError((Gtk.Window)Toplevel, message);
				}
				catch (Exception e)
				{
					string message = "Send request failed: " + e.Message;
					UIHelper.DisplayError((Gtk.Window)Toplevel, message);
				}
			}
		}
		
		private HistoryRequest PrepareRequest()
		{
			var operation = new HistoryOperation
			{
				AccountId = txtAccountNumber.Text.Trim(),
				Anonymity = GetAnonimity(),
				CorrespondingAccountId = txtCorrespondingAcount.Text.Trim(),
				Currency = UIHelper.GetCurrency(cmbCurrency),
				Direction = GetDirection(),
				StartAmount = UIHelper.GetDouble(txtStartAmount),
				EndAmount = UIHelper.GetDouble(txtEndAmount),
				StartDate = UIHelper.GetDate(txtStartDate),
				EndDate = UIHelper.GetDate(txtEndDate),
				ReceiptId = txtReceiptId.Text.Trim(),
				Source = txtSource.Text.Trim(),
				TransferId = txtTransferId.Text.Trim(),
				Pager = new Pager
				{
					PageNumber = UIHelper.GetInt(txtPageNumber),
					PageSize = UIHelper.GetInt(txtPageSize),
				},
			};
			var request = new HistoryRequest()
			{
				Id = Convert.ToString(DateTime.UtcNow.Ticks),
				Auth = new AuthToken(),
				Operation = operation,
			};
			
			return request;
		}
		
		#endregion
		
		#region Validation
		
		private bool ValidateRequestParams()
		{
			if (!ApiCredentialsProvider.Validate())
				return false;
			if (!UIHelper.ValidateEmptyEntry(txtAccountNumber, "Account number is not specified."))
				return false;
			if (!String.IsNullOrEmpty(txtStartDate.Text.Trim()) && 
				!UIHelper.ValidateDate(txtStartDate, "Start date value is invalid."))
				return false;
			if (!String.IsNullOrEmpty(txtEndDate.Text.Trim()) && 
				!UIHelper.ValidateDate(txtEndDate, "End date value is invalid."))
				return false;
			
			return true;
		}
		
		#endregion
		
		#region Event handlers
		
		protected void OnSendRequestClicked(object sender, System.EventArgs e)
		{
			SendRequest();
		}
		
		#endregion
		
	}
}

