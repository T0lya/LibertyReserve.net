using System;
using System.Collections.Generic;
using Magnis.Web.Services.LibertyReserve;

namespace LRDemo
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class AccountWidget : Gtk.Bin
	{
		#region Declarations
		
		[Gtk.TreeNode(ListOnly = true)]
		class OperationTreeNode : Gtk.TreeNode
		{
			[Gtk.TreeNodeValue(Column = 0)]
			public string AccountId { get; set; }
			
			[Gtk.TreeNodeValue(Column = 1)]
			public string AccountToRetrieve { get; set; }
		}
		
		[Gtk.TreeNode(ListOnly = true)]
		class ResponseTreeNode : Gtk.TreeNode
		{
			[Gtk.TreeNodeValue(Column = 0)]
			public string AccountId { get; set; }
			
			[Gtk.TreeNodeValue(Column = 1)]
			public string Name { get; set; }
			
			[Gtk.TreeNodeValue(Column = 2)]
			public string Date { get; set; }
		}
		
		#endregion

		public IApiCredentialsProvider ApiCredentialsProvider { get; set; }
		
		private Gtk.NodeStore operationStore = new Gtk.NodeStore(typeof(OperationTreeNode));
		private Gtk.NodeStore accountStore = new Gtk.NodeStore(typeof(ResponseTreeNode));
		
		public AccountWidget ()
		{
			this.Build ();
			InitOperationsNodeView();
			InitAccountNodeView();
			UpdateUI();
		}
		
		#region Initialization
		
		private void InitOperationsNodeView()
		{
			operationsNodeView.NodeStore = operationStore;
			operationsNodeView.AppendColumn("Account ID", new Gtk.CellRendererText(), "text", 0);
			operationsNodeView.AppendColumn("Account To Retrieve", new Gtk.CellRendererText(), "text", 1);
			operationsNodeView.ShowAll();
			operationsNodeView.NodeSelection.Changed += (sender, args) => UpdateUI();
		}
		
		private void InitAccountNodeView()
		{
			accountsNodeView.NodeStore = accountStore;
			accountsNodeView.AppendColumn("Account ID", new Gtk.CellRendererText(), "text", 0);
			accountsNodeView.AppendColumn("Name", new Gtk.CellRendererText(), "text", 1);
			accountsNodeView.AppendColumn("Date", new Gtk.CellRendererText(), "text", 2);
			accountsNodeView.ShowAll();
		}

		#endregion
		
		#region UI
		
		private void UpdateUI()
		{
			btnRemoveOperation.Sensitive = operationsNodeView.NodeSelection.SelectedNode != null;
			btnSendRequest.Sensitive = operationStore.GetNode(Gtk.TreePath.NewFirst()) != null;
		}
		
		private void ShowAccounts(AccountNameResponse response)
		{
			txtRawResponse.Buffer.Text = response.ResponseText;
			foreach (AccountName account in response.AccountNames)
			{
				var node = new ResponseTreeNode
				{
					AccountId = account.AccountToRetrieve,
					Name = account.Name,
					Date = account.Date.ToString(),
				};
				accountStore.AddNode(node);
			}
		}
				
		private void ClearResponseData()
		{
			accountStore.Clear();
			txtRawResponse.Buffer.Clear();
		}
		
		#endregion
		
		#region Request routines
		
		private void AddOperationToRequest()
		{
			if (ValidateOperationParams()) 
			{
				var node = new OperationTreeNode
				{
					AccountId = txtAccountNumber.Text.Trim(),
					AccountToRetrieve = txtAccountToRetrieve.Text.Trim(),
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
				
				AccountNameRequest request = PrepareRequest();
				request.Auth = AuthToken.FromApiCredentials(ApiCredentialsProvider.Credentials);
				try
				{
					AccountNameResponse response = request.GetResponse();
					ShowAccounts(response);
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
		
		private AccountNameRequest PrepareRequest()
		{
			var operations = new List<AccountNameOperation>();
			foreach (OperationTreeNode node in operationStore) 
			{
				var op = new AccountNameOperation
				{
					AccountId = node.AccountId,
					AccountToRetrieve = node.AccountToRetrieve,
				};
				operations.Add(op);
			}
			var request = new AccountNameRequest()
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
			if (!UIHelper.ValidateEmptyEntry(txtAccountToRetrieve, "Account number to retrieve is not specified."))
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

