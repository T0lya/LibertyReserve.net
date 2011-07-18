using System;
using Magnis.Web.Services.LibertyReserve;
using System.Text;
using System.Collections.Generic;

namespace LRDemo
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class TransferWidget : Gtk.Bin
	{
		#region Declarations
		
		[Gtk.TreeNode(ListOnly = true)]
		class OperationTreeNode : Gtk.TreeNode
		{
			[Gtk.TreeNodeValue(Column = 0)]
			[TreeNodeColumn("Transfer ID")]
			public string TransferId { get; set; }
			
			[Gtk.TreeNodeValue(Column = 1)]
			[TreeNodeColumn("Payer")]
			public string Payer { get; set; }
			
			[Gtk.TreeNodeValue(Column = 2)]
			[TreeNodeColumn("Payee")]
			public string Payee { get; set; }
			
			public Currency Currency { get; set; }
			
			[Gtk.TreeNodeValue(Column = 3)]
			[TreeNodeColumn("Currency")]
			public string CurrencyName { get; set; }
			
			[Gtk.TreeNodeValue(Column = 4)]
			[TreeNodeColumn("Amount")]
			public double Amount { get; set; }
			
			[Gtk.TreeNodeValue(Column = 5)]
			[TreeNodeColumn("Anonymous")]
			public string IsAnonymous { get; set; }
			
			[Gtk.TreeNodeValue(Column = 6)]
			[TreeNodeColumn("Description")]
			public string Description { get; set; }
		}
		
		[Gtk.TreeNode(ListOnly = true)]
		class ReceiptTreeNode : Gtk.TreeNode
		{
			[Gtk.TreeNodeValue(Column = 0)]
			[TreeNodeColumn("Transfer ID")]
			public string TransferId { get; set; }
			
			[Gtk.TreeNodeValue(Column = 1)]
			[TreeNodeColumn("Payer")]
			public string Payer { get; set; }
			
			[Gtk.TreeNodeValue(Column = 2)]
			[TreeNodeColumn("Payer Name")]
			public string PayerName { get; set; }
			
			[Gtk.TreeNodeValue(Column = 3)]
			[TreeNodeColumn("Payee")]
			public string Payee { get; set; }
			
			[Gtk.TreeNodeValue(Column = 4)]
			[TreeNodeColumn("Payee Name")]
			public string PayeeName { get; set; }
			
			[Gtk.TreeNodeValue(Column = 5)]
			[TreeNodeColumn("Currency")]
			public string Currency { get; set; }
			
			[Gtk.TreeNodeValue(Column = 6)]
			[TreeNodeColumn("Amount")]
			public double Amount { get; set; }
			
			[Gtk.TreeNodeValue(Column = 7)]
			[TreeNodeColumn("Fee")]
			public double Fee { get; set; }
			
			[Gtk.TreeNodeValue(Column = 8)]
			[TreeNodeColumn("Closing Balance")]
			public double ClosingBalace { get; set; }
			
			[Gtk.TreeNodeValue(Column = 9)]
			[TreeNodeColumn("Anonymous")]
			public string IsAnonymous { get; set; }
			
			[Gtk.TreeNodeValue(Column = 10)]
			[TreeNodeColumn("Description")]
			public string Description { get; set; }
			
			[Gtk.TreeNodeValue(Column = 11)]
			[TreeNodeColumn("Source")]
			public string Source { get; set; }
		}
		
		#endregion
		
		public IApiCredentialsProvider ApiCredentialsProvider { get; set; }
		
		private Gtk.NodeStore operationStore = new Gtk.NodeStore(typeof(OperationTreeNode));
		private Gtk.NodeStore receiptStore = new Gtk.NodeStore(typeof(ReceiptTreeNode));
		
		public TransferWidget ()
		{
			this.Build ();
			
			UIHelper.FillCurrencyComboBox(cmbCurrency);
			InitOperationsNodeView();
			InitTransfersNodeView();
		}
		
		#region Initialization
		
		private void InitOperationsNodeView()
		{
			operationsNodeView.NodeStore = operationStore;
			operationsNodeView.AutoGenerateColumns(typeof(OperationTreeNode));
			operationsNodeView.ShowAll();
			operationsNodeView.NodeSelection.Changed += (sender, args) => UpdateUI();
		}
		
		private void InitTransfersNodeView()
		{
			receiptNodeView.NodeStore = receiptStore;
			receiptNodeView.AutoGenerateColumns(typeof(ReceiptTreeNode));
			receiptNodeView.ShowAll();
		}
		
		#endregion
		
		#region UI
		
		private void UpdateUI()
		{
			btnRemoveOperation.Sensitive = operationsNodeView.NodeSelection.SelectedNode != null;
			btnSendRequest.Sensitive = operationStore.GetNode(Gtk.TreePath.NewFirst()) != null;
		}
		
		private void ShowReceipts(TransferResponse response)
		{
			txtRawResponse.Buffer.Text = response.ResponseText;
			foreach (Receipt r in response.Receipts)
			{
				var node = new ReceiptTreeNode
				{
					TransferId = r.Transfer.TransferId,
					Payer = r.Transfer.Payer,
					PayerName = r.PayerName,
					Payee = r.Transfer.Payee,
					PayeeName = r.PayeeName,
					IsAnonymous = r.Transfer.Anonymous.ToString(),
					Amount = r.Amount,
					Currency = LRConverter.ToString(r.Transfer.Currency),
					ClosingBalace = r.ClosingBalance,
					Fee = r.Fee,
					Description = r.Transfer.Description,
					Source = r.Transfer.Source,
				};
				receiptStore.AddNode(node);
			}
		}
				
		private void ClearResponseData()
		{
			receiptStore.Clear();
			txtRawResponse.Buffer.Clear();
		}
		
		#endregion
		
		#region Request routines
		
		private void AddOperationToRequest()
		{
			if (ValidateOperationParams()) 
			{
				Gtk.TreeIter currencyIter;
				cmbCurrency.GetActiveIter(out currencyIter);
				Currency currency = (Currency)cmbCurrency.Model.GetValue(currencyIter, 1);
				
				var node = new OperationTreeNode
				{
					TransferId = txtTransferId.Text.Trim(),
					Payer = txtPayer.Text.Trim(),
					Payee = txtPayee.Text.Trim(),
					Currency = currency,
					CurrencyName = currency.ToString(),
					Amount = Double.Parse(txtAmount.Text),
					Description = txtDescription.Buffer.Text,
					IsAnonymous = chkAnonymous.Active.ToString(),
				};
				operationStore.AddNode(node);
				UpdateUI();
			}
		}
		
		private void RemoveOperationFromRequest()
		{
			Gtk.ITreeNode selectedNode = operationsNodeView.NodeSelection.SelectedNode;
			if (selectedNode != null)
				operationStore.RemoveNode(selectedNode);
		}
		
		private void SendRequest()
		{
			if (ValidateRequestParams())
			{
				ClearResponseData();
				
				TransferRequest request = PrepareRequest();
				request.Auth = AuthToken.FromApiCredentials(ApiCredentialsProvider.Credentials);
				try
				{
					TransferResponse response = request.GetResponse();
					ShowReceipts(response);
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
		
		private TransferRequest PrepareRequest()
		{
			var operations = new List<TransferOperation>();
			foreach (OperationTreeNode node in operationStore) 
			{
				var op = new TransferOperation
				{
					TransferId = node.TransferId,
					Payer = node.Payer,
					Payee = node.Payee,
					Amount = node.Amount,
					Currency = node.Currency,
					Anonymous = Boolean.Parse(node.IsAnonymous),
					Description = node.Description,
				};
				operations.Add(op);
			}
			var request = new TransferRequest()
			{
				Id = Convert.ToString(DateTime.UtcNow.Ticks),
				Auth = new AuthToken(),
				Operations = operations,
			};
			
			return request;
		}
		
		#endregion
		
		#region Validation
		
		private bool ValidateOperationParams()
		{
			if (!UIHelper.ValidateEmptyEntry(txtPayer, "Payer account number is not specified."))
				return false;
			if (!UIHelper.ValidateEmptyEntry(txtPayee, "Payee account number is not specified."))
				return false;
			if (!UIHelper.ValidateEmptyComboBox(cmbCurrency, "Currency is not specified."))
				return false;
			
			double dblValue;
			if (!Double.TryParse(txtAmount.Text, out dblValue) || dblValue <- 0.0)
			{
				txtAmount.GrabFocus();
				UIHelper.DisplayError((Gtk.Window)Toplevel, "Invalid price value.");
				return false;
			}
			
			return true;
		}
		
		private bool ValidateRequestParams()
		{
			if (!ApiCredentialsProvider.Validate())
				return false;
			if (operationStore.GetNode(Gtk.TreePath.NewFirst()) == null)
			{
				UIHelper.DisplayError((Gtk.Window)Toplevel, "You have to add at least one operation to the request.");
				return false;
			}
			
			return true;
		}
		
		#endregion

		#region Event handlers
		
		protected void OnAddOperationClicked(object sender, System.EventArgs e)
		{
			AddOperationToRequest();
		}

		protected void OnRemoveOperationClicked(object sender, System.EventArgs e)
		{
			RemoveOperationFromRequest();
		}

		protected void OnSendRequestClicked(object sender, System.EventArgs e)
		{
			SendRequest();
		}
		
		#endregion
	}
}

