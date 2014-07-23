/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 5/1/2014
 * Time: 10:02 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

namespace SpinMARTracker
{
	/// <summary>
	/// Description of ddlSearch
	/// </summary>
	/// 	
	public class EvArgs : EventArgs{
		private string Message;
		
		public EvArgs(string txtMess){
			Message = txtMess;
		}
		
		public string Mess{
			get{return Message;}
			set{Message = value;}
		}
	}
	public class ddlSearch : UserControl
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected DropDownList ddlList;
		protected TextBox txtSearch;
		protected string strDatasource;
		protected string strVField;
		protected string strTField;
		public EventHandler<EvArgs> IndChange;
		protected HtmlControl Spacer;
		
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Page Init & Exit (Open/Close DB connections here...)

		protected void PageInit(object sender, System.EventArgs e)
		{
			txtSearch.Attributes.Add("OnKeyUp","UpdateTimer" + txtSearch.ClientID.ToString() + "(this.value);");
		}
		//----------------------------------------------------------------------
		protected void PageExit(object sender, System.EventArgs e)
		{
			
		}

		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Page Load
		private void Page_Load(object sender, System.EventArgs e)
		{
			
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Initialize Component

		protected override void OnInit(EventArgs e)
		{	InitializeComponent();
			base.OnInit(e);
		}
		//----------------------------------------------------------------------
		private void InitializeComponent()
		{	//------------------------------------------------------------------
			this.Load	+= new System.EventHandler(Page_Load);
			this.Init   += new System.EventHandler(PageInit);
			this.Unload += new System.EventHandler(PageExit);
			this.ddlList.SelectedIndexChanged += new System.EventHandler(SelChanged);
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region ctrl procdures
		protected void SelChanged(object sender, System.EventArgs e){
			txtSearch.Text = ddlList.SelectedItem.Text.ToString();
			RaiseIndChange("test");
		}
		public void CtrlBind(){
			ddlList.DataSource = Utility.GetSQLDataView(strDatasource);
			ddlList.DataTextField = strTField;
			ddlList.DataValueField = strVField;
			ddlList.DataBind();
			
			ddlList.Items.Insert(0,new ListItem("None Selected","-1"));
			ddlList.SelectedValue = "-1";
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Properties
		public string SelectedValue{
			get{
				string val = ddlList.SelectedValue.ToString();
				if (val == "-1"){
					return "";
				}
				else{
					return val;
				}
			}
		}
		public int ConWidth{
			get{
				return Convert.ToInt32(ddlList.Width.Value);
			}
			set{
				ddlList.Width = value;
				txtSearch.Width = value - 16;
			}
		}
		public string CtrlDataSource{
			get{
				return strDatasource;
			}
			set{
				strDatasource = value;
			}
		}
		public string CtrlVField{
			get{
				return strVField;
			}
			set{
				strVField = value;
			}
		}
		public string CtrlTField{
			get{
				return strTField;
			}
			set{
				strTField = value;
			}
		}
		public string CtrlSelText{
			get{return ddlList.SelectedItem.Text.ToString();}
		}
		public string CtrlSelValue{
			get{return ddlList.SelectedValue.ToString();}
			set{
				ddlList.SelectedValue = value;
				txtSearch.Text = ddlList.SelectedItem.Text.ToString();
			}
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region event
		private void RaiseIndChange(string e){
			EventHandler<EvArgs> handle = IndChange;
			if (handle != null){
				handle(null, new EvArgs(e));
			}
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
	}
}
