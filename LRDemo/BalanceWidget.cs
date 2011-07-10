using System;
using System.Linq;
using Magnis.Web.Services.LibertyReserve;
using System.Collections.Generic;

namespace LRDemo
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class BalanceWidget : Gtk.Bin
	{
		#region Declarations
		
		[Gtk.TreeNode(ListOnly = true)]
		class BalanceOperationTreeNode : Gtk.TreeNode
		{
			[Gtk.TreeNodeValue(Column = 0)]
			public string AccountNumber { get; set; }
			
			public Currency Currency { get; set; }
			
			[Gtk.TreeNodeValue(Column = 1)]
			public string CurrencyName { get; set; }
		}
		
		#endregion
		
		public IApiCredentialsProvider ApiCredentialsProvider { get; set; }
		
		private Gtk.NodeStore operationStore = new Gtk.NodeStore(typeof(BalanceOperationTreeNode));
		
		public BalanceWidget()
		{
			this.Build();
			
			UIHelper.FillCurrencyComboBox(cmbCurrency);
			InitOperationsNodeView();
			UpdateUI();
		}
		
		#region Initialization
		
		private void InitOperationsNodeView()
		{
			operationsNodeView.NodeStore = operationStore;
			operationsNodeView.AppendColumn("Account Name", new Gtk.CellRendererText(), "text", 0);
			operationsNodeView.AppendColumn("Currency", new Gtk.CellRendererText(), "text", 1);
			operationsNodeView.ShowAll();
			operationsNodeView.NodeSelection.Changed += (sender, args) => UpdateUI();
		}

		#endregion
		
		#region UI
		
		private void UpdateUI()
		{
			btnRemoveOperation.Sensitive = operationsNodeView.NodeSelection.SelectedNode != null;
			btnSendRequest.Sensitive = operationStore.GetNode(Gtk.TreePath.NewFirst()) != null;
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
				
				var node = new BalanceOperationTreeNode
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
			BalanceRequest request = PrepareRequest();
			request.Auth = AuthenticationBlock.FromApiCredentials(ApiCredentialsProvider.Credentials);
			BalanceResponse response = request.GetResponse();
		}
		
		private BalanceRequest PrepareRequest()
		{
			var operations = new List<BalanceOperation>();
			foreach (BalanceOperationTreeNode node in operationStore) 
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
				Auth = new AuthenticationBlock(),
				Operations = operations,
			};
			
			return request;
		}
		
		#endregion
		
		private bool ValidateOperationParams()
		{
			if (!ApiCredentialsProvider.Validate())
				return false;
			if (!UIHelper.ValidateEmptyEntry(txtAccountNumber, "Account number is not specified."))
				return false;
			if (!UIHelper.ValidateEmptyComboBox(cmbCurrency, "Currency is not specified."))
				return false;
			
			return true;
		}
		
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

