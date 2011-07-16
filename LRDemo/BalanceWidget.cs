using System;
using System.Collections.Generic;
using System.Linq;
using Magnis.Web.Services.LibertyReserve;
using System.Text;

namespace LRDemo
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class BalanceWidget : Gtk.Bin
	{
		#region Declarations
		
		[Gtk.TreeNode(ListOnly = true)]
		class BalanceTreeNode : Gtk.TreeNode
		{
			[Gtk.TreeNodeValue(Column = 0)]
			public string AccountNumber { get; set; }
			
			public Currency Currency { get; set; }
			
			[Gtk.TreeNodeValue(Column = 1)]
			public string CurrencyName { get; set; }
			
			[Gtk.TreeNodeValue(Column = 2)]
			public double Value { get; set; }
		}
		
		#endregion
		
		public IApiCredentialsProvider ApiCredentialsProvider { get; set; }
		
		private Gtk.NodeStore operationStore = new Gtk.NodeStore(typeof(BalanceTreeNode));
		private Gtk.NodeStore balanceStore = new Gtk.NodeStore(typeof(BalanceTreeNode));
		
		public BalanceWidget()
		{
			this.Build();
			
			UIHelper.FillCurrencyComboBox(cmbCurrency);
			InitOperationsNodeView();
			InitBalanceNodeView();
			UpdateUI();
		}
		
		#region Initialization
		
		private void InitOperationsNodeView()
		{
			operationsNodeView.NodeStore = operationStore;
			operationsNodeView.AppendColumn("Account ID", new Gtk.CellRendererText(), "text", 0);
			operationsNodeView.AppendColumn("Currency", new Gtk.CellRendererText(), "text", 1);
			operationsNodeView.ShowAll();
			operationsNodeView.NodeSelection.Changed += (sender, args) => UpdateUI();
		}
		
		private void InitBalanceNodeView()
		{
			balanceNodeView.NodeStore = balanceStore;
			balanceNodeView.AppendColumn("Account ID", new Gtk.CellRendererText(), "text", 0);
			balanceNodeView.AppendColumn("Currency", new Gtk.CellRendererText(), "text", 1);
			balanceNodeView.AppendColumn("Value", new Gtk.CellRendererText(), "text", 2);
			balanceNodeView.ShowAll();
		}

		#endregion
		
		#region UI
		
		private void UpdateUI()
		{
			btnRemoveOperation.Sensitive = operationsNodeView.NodeSelection.SelectedNode != null;
			btnSendRequest.Sensitive = operationStore.GetNode(Gtk.TreePath.NewFirst()) != null;
		}
		
		private void ShowBalances(BalanceResponse response)
		{
			txtRawResponse.Buffer.Text = response.ResponseText;
			foreach (Balance b in response.Balances)
			{
				var node = new BalanceTreeNode
				{
					AccountNumber = b.AccountId,
					Currency = b.Currency,
					CurrencyName = b.Currency.ToString(),
					Value = b.Value,
				};
				balanceStore.AddNode(node);
			}
		}
		
		private void ClearResponseData()
		{
			balanceStore.Clear();
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
				
				var node = new BalanceTreeNode
				{
					AccountNumber = txtAccountNumber.Text.Trim(),
					Currency = currency,
					CurrencyName = currency.ToString(),
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
				
				BalanceRequest request = PrepareRequest();
				request.Auth = AuthToken.FromApiCredentials(ApiCredentialsProvider.Credentials);
				try
				{
					BalanceResponse response = request.GetResponse();
					ShowBalances(response);
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
		
		private BalanceRequest PrepareRequest()
		{
			var operations = new List<BalanceOperation>();
			foreach (BalanceTreeNode node in operationStore) 
			{
				var op = new BalanceOperation
				{
					AccountId = node.AccountNumber,
					Currency = node.Currency,
				};
				operations.Add(op);
			}
			var request = new BalanceRequest()
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
			if (!UIHelper.ValidateEmptyEntry(txtAccountNumber, "Account number is not specified."))
				return false;
			if (!UIHelper.ValidateEmptyComboBox(cmbCurrency, "Currency is not specified."))
				return false;
			
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

