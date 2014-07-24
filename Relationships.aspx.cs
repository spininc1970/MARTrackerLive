/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 9/20/2013
 * Time: 5:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections;

namespace SpinMARTracker
{
	/// <summary>
	/// Description of Relationships
	/// </summary>
	public class Relationships : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected DropDownList ddlUser;
		protected DropDownList ddlDSP;
		protected TextBox txtSdate;
		protected TextBox txtEdate;
		protected Button btnSaveUpdate;
		protected Button btnCancel;
		protected DataGrid dgDSPGrid;
		protected CheckBox chkPer;
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Page Init & Exit (Open/Close DB connections here...)

		protected void PageInit(object sender, System.EventArgs e)
		{
			Utility.CheckLogin(Convert.ToBoolean(Session["login"]),this.Response);
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
			//------------------------------------------------------------------
			if(!IsPostBack)
			{
				ddlUser.DataSource = Utility.GetSQLDataView("Select UserID,EmployeeName from v_Users where active = 1 order by EmployeeName");
				ddlUser.DataTextField = "EmployeeName";
				ddlUser.DataValueField = "UserID";
				ddlUser.DataBind();
				
				ddlUser.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlUser.SelectedValue = "-1";
				
				ddlDSP.DataSource = Utility.GetSQLDataView("Select EMP_ID, Last_Name + ', ' + First_Name as 'DSPName' from v_ActiveDSPs order by Last_Name");
				ddlDSP.DataTextField = "DSPName";
				ddlDSP.DataValueField = "EMP_ID";
				ddlDSP.DataBind();
				
				ddlDSP.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlDSP.SelectedValue = "-1";
				
			}
			SecurityChecks();
			//------------------------------------------------------------------
		}
		protected void SecurityChecks(){
			if (Session["EditRel"].ToString() != "True"){
				Response.Redirect("Default.aspx");
			}
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Add more events here...

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
			this.ddlUser.SelectedIndexChanged += new System.EventHandler(UserChange);
			this.dgDSPGrid.ItemDataBound += new DataGridItemEventHandler(_ItemDataBound);
			//------------------------------------------------------------------
		}
		protected void UserChange(object sender, EventArgs e)
		{
			if(ddlUser.SelectedValue.ToString() != "-1")
			{
				btnSaveUpdate.Enabled = true;	
				ddlDSP.Enabled = true;
				txtSdate.Enabled = true;
				txtEdate.Enabled = true;
				chkPer.Enabled = true;
				dgDSPGrid.DataSource = Utility.GetSQLDataView("Select * from v_UserDSPs where UserID = " + Convert.ToString(ddlUser.SelectedValue));
				dgDSPGrid.DataBind();
			}
			else
			{
				btnSaveUpdate.Enabled = false;	
				ddlDSP.Enabled = false;
				txtSdate.Enabled = false;
				txtEdate.Enabled = false;
				chkPer.Enabled = false;
				dgDSPGrid.DataSource = Utility.GetSQLDataView("Select * from v_UserDSPs where UserID = " + Convert.ToString(ddlUser.SelectedValue));
				dgDSPGrid.DataBind();
			}
		}
		protected void _ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
			{
				e.Item.Cells[8].Attributes.Add( "onClick", "return ConfirmDeletion();");
			}
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data Manipulation
		protected void RemoveDSP(object sender, DataGridCommandEventArgs e)
		{
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand delcmd = new SqlCommand("RemoveDSPRel",sqlconn);
			string test;
			
			delcmd.CommandType = CommandType.StoredProcedure;
			delcmd.Parameters.Add("@rid",SqlDbType.Int);
			
			delcmd.Parameters["@rid"].Value = Convert.ToInt32(e.Item.Cells[0].Text);
			
			sqlconn.Open();
			
			test = delcmd.CommandText.ToString();
			
			delcmd.ExecuteNonQuery();
			
			delcmd = null;
			
			sqlconn.Close();
			
			dgDSPGrid.DataSource = Utility.GetSQLDataView("Select * from v_UserDSPs where UserID = " + Convert.ToString(ddlUser.SelectedValue));
			dgDSPGrid.DataBind();
		}
		protected void EditDSP(object sender, DataGridCommandEventArgs e)
		{
			ddlUser.Enabled=false;
			ddlDSP.SelectedValue = e.Item.Cells[2].Text.ToString();
			ddlDSP.Enabled=false;
			btnCancel.Enabled=true;
			if (e.Item.Cells[5].Text.ToString() == "&nbsp;")
			{
				txtEdate.Text = "";
			}
			else
			{
				txtEdate.Text = e.Item.Cells[5].Text.ToString();
			}
			txtSdate.Text = e.Item.Cells[4].Text.ToString();
			
			if (e.Item.Cells[6].Text.ToString() == "True") {
				chkPer.Checked = true;
			}
			else{
				chkPer.Checked = false;
			}
			
			btnSaveUpdate.Text = "Update Relationship";
			
		}
		protected void AddDSP(object sender, EventArgs e)
		{
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand addcmd = new SqlCommand("CreateDSPRel",sqlconn);
			SqlCommand editcmd = new SqlCommand("UpdateDSPRel",sqlconn);
			
			if(btnSaveUpdate.Text == "Save Relationship")
			{
				addcmd.CommandType = CommandType.StoredProcedure;
				addcmd.Parameters.Add("@uid",SqlDbType.Int);
				addcmd.Parameters.Add("@dspid",SqlDbType.VarChar);
				addcmd.Parameters.Add("@sdate",SqlDbType.DateTime);
				addcmd.Parameters.Add("@edate",SqlDbType.DateTime);
				addcmd.Parameters.Add("@per",SqlDbType.Int);
				
				addcmd.Parameters["@uid"].Value = ddlUser.SelectedValue.ToString();
				addcmd.Parameters["@dspid"].Value = ddlDSP.SelectedValue.ToString();
				addcmd.Parameters["@sdate"].Value = Convert.ToDateTime(txtSdate.Text.ToString());
				
				if (txtEdate.Text.ToString() != "")
				{
					addcmd.Parameters["@edate"].Value = Convert.ToDateTime(txtEdate.Text.ToString());
				}
				else
				{
					addcmd.Parameters["@edate"].Value = DBNull.Value;
				}
				
				if (chkPer.Checked == true){
					addcmd.Parameters["@per"].Value = 1;
				}
				else{
					addcmd.Parameters["@per"].Value = 0;
				}
				sqlconn.Open();
				
				addcmd.ExecuteNonQuery();
				
				addcmd = null;
				
				sqlconn.Close();
				
			}
			else if(btnSaveUpdate.Text == "Update Relationship")
			{
				editcmd.CommandType = CommandType.StoredProcedure;
				editcmd.Parameters.Add("@uid",SqlDbType.Int);
				editcmd.Parameters.Add("@DSP",SqlDbType.VarChar);
				editcmd.Parameters.Add("@sdate",SqlDbType.DateTime);
				editcmd.Parameters.Add("@edate",SqlDbType.DateTime);
				editcmd.Parameters.Add("@per",SqlDbType.Int);
				
				editcmd.Parameters["@uid"].Value = ddlUser.SelectedValue.ToString();
				editcmd.Parameters["@DSP"].Value = ddlDSP.SelectedValue.ToString();
				editcmd.Parameters["@sdate"].Value = Convert.ToDateTime(txtSdate.Text.ToString());
				if (txtEdate.Text.ToString() != "")
				{
					editcmd.Parameters["@edate"].Value = Convert.ToDateTime(txtEdate.Text.ToString());
				}
				else
				{
					editcmd.Parameters["@edate"].Value = DBNull.Value;
				}
				if (chkPer.Checked == true){
					editcmd.Parameters["@per"].Value = 1;
				}
				else{
					editcmd.Parameters["@per"].Value = 0;
				}
				sqlconn.Open();
				
				editcmd.ExecuteNonQuery();
				
				editcmd = null;
				
				sqlconn.Close();
				ClearFrm();
			}
			dgDSPGrid.DataSource = Utility.GetSQLDataView("Select * from v_UserDSPs where UserID = " + Convert.ToString(ddlUser.SelectedValue));
			dgDSPGrid.DataBind();
		}
		protected void ClearFrm()
		{
			btnSaveUpdate.Text = "Save Relationship";
			btnCancel.Enabled = false;
			ddlUser.Enabled = true;
			ddlDSP.Enabled = true;
			ddlDSP.SelectedValue = "-1";
			txtSdate.Text = "";
			txtEdate.Text = "";
		}
		protected void Clear(object sender, EventArgs e)
		{
			ClearFrm();
		}
		#endregion
	}
}
