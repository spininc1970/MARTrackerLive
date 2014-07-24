/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 9/23/2013
 * Time: 9:15 AM
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
	/// Description of Users
	/// </summary>
	public class Users : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected DropDownList ddlEmp;
		protected TextBox txtLDAP;
		protected CheckBox chkActive;
		protected DataGrid dgEmpGrid;
		protected Button btnSaveUpdate;
		protected Button btnCancel;
		protected DropDownList ddlSgroup;
		protected HyperLink hlMAGrps;
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
				ddlEmp.DataSource = Utility.GetSQLDataView("Select EMP_ID,Last_Name + ', ' + First_Name as 'EmpName' from Employee order by last_name");
				ddlEmp.DataTextField = "EmpName";
				ddlEmp.DataValueField = "EMP_ID";
				ddlEmp.DataBind();
				
				ddlEmp.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlEmp.SelectedValue = "-1";
				
				ddlSgroup.DataSource = Utility.GetSQLDataView("Select SGroupID,GroupName from SecurityGroups");
				ddlSgroup.DataTextField = "GroupName";
				ddlSgroup.DataValueField = "SGroupID";
				ddlSgroup.DataBind();
				
				ddlSgroup.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlSgroup.SelectedValue = "-1";
			}
			SecurityChecks();
			if(ddlEmp.SelectedValue != "-1")
			{
				btnSaveUpdate.Enabled = true;
			}
			else
			{
				btnSaveUpdate.Enabled = false;
			}
			dgEmpGrid.DataSource = Utility.GetSQLDataView("Select * from v_Users order by EmployeeName asc");
			dgEmpGrid.DataBind();
			//------------------------------------------------------------------
		}
		protected void SecurityChecks(){
			if (Session["ManageU"].ToString() != "True"){
				Response.Redirect("Default.aspx");
			}
			if (Session["ManageGrps"].ToString() != "True"){
				hlMAGrps.Visible = false;
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
			this.dgEmpGrid.ItemDataBound += new DataGridItemEventHandler(_ItemDataBound);
			//------------------------------------------------------------------
		}
		protected void _ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
			{
				e.Item.Cells[6].Attributes.Add( "onClick", "return ConfirmDeletion();");
			}
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region DataManipulation
		protected void RemoveEmp(object sender, DataGridCommandEventArgs e)
		{
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand delcmd = new SqlCommand("RemoveUser",sqlconn);
			
			delcmd.CommandType = CommandType.StoredProcedure;
			delcmd.Parameters.Add("@userid",SqlDbType.Int);
			
			delcmd.Parameters["@userid"].Value = Convert.ToInt32(e.Item.Cells[0].Text);
			
			sqlconn.Open();
			
			delcmd.ExecuteNonQuery();
			
			delcmd = null;
			
			sqlconn.Close();
			
			dgEmpGrid.DataSource = Utility.GetSQLDataView("Select * from v_Users order by EmployeeName asc");
			dgEmpGrid.DataBind();
			
			ClearFrm();
		}
		protected void AddEmp(object sender, EventArgs e)
		{
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand addcmd = new SqlCommand("CreateUser",sqlconn);
			SqlCommand editcmd = new SqlCommand("UpdateUser",sqlconn);
			
			if(btnSaveUpdate.Text == "Add User")
			{
				addcmd.CommandType = CommandType.StoredProcedure;
				addcmd.Parameters.Add("@empid",SqlDbType.VarChar);
				addcmd.Parameters.Add("@ldap",SqlDbType.VarChar);
				addcmd.Parameters.Add("@sgid",SqlDbType.Int);
			
				addcmd.Parameters["@empid"].Value = ddlEmp.SelectedValue.ToString();
				addcmd.Parameters["@ldap"].Value = txtLDAP.Text.ToString();
				addcmd.Parameters["@sgid"].Value = ddlSgroup.SelectedValue;
			
				sqlconn.Open();
			
				addcmd.ExecuteNonQuery();
			
				addcmd = null;
			
				sqlconn.Close();
			
				Response.Write("<script>alert('User Added')</script>");
			}
			else if(btnSaveUpdate.Text == "Update Employee")
			{
				editcmd.CommandType = CommandType.StoredProcedure;
			
				editcmd.Parameters.Add("@empid",SqlDbType.VarChar);
				editcmd.Parameters.Add("@ldap",SqlDbType.VarChar);
				editcmd.Parameters.Add("@Active",SqlDbType.Bit);
				editcmd.Parameters.Add("@sgid",SqlDbType.Int);
				
				editcmd.Parameters["@empid"].Value = ddlEmp.SelectedValue.ToString();
				editcmd.Parameters["@ldap"].Value = txtLDAP.Text.ToString();
				if(chkActive.Checked == true)
				{
					editcmd.Parameters["@Active"].Value = 1;
				}
				else
				{
					editcmd.Parameters["@Active"].Value = 0;
				}
				
				editcmd.Parameters["@sgid"].Value = ddlSgroup.SelectedValue;
				
				sqlconn.Open();
				
				editcmd.ExecuteNonQuery();
				
				editcmd = null;
				
				sqlconn.Close();
				
				Response.Write("<script>alert('User Updated')</script>");
				
			}
			dgEmpGrid.DataSource = Utility.GetSQLDataView("Select * from v_Users order by EmployeeName asc");
			dgEmpGrid.DataBind();
			ClearFrm();
			
		}
		protected void EditEmp(object sender, DataGridCommandEventArgs e)
		{
			ddlEmp.SelectedValue = e.Item.Cells[1].Text.ToString();
			if (e.Item.Cells[3].Text == "&nbsp;")
			{
				txtLDAP.Text = "";
			}
			else
			{
				txtLDAP.Text = e.Item.Cells[3].Text.ToString();
			}
			chkActive.Enabled = true;
			chkActive.Checked = Convert.ToBoolean(e.Item.Cells[6].Text.ToString());
			ddlSgroup.SelectedValue = e.Item.Cells[4].Text.ToString();
			btnSaveUpdate.Text = "Update Employee";
			btnSaveUpdate.Enabled=true;
			ddlEmp.Enabled= false;
			btnCancel.Enabled=true;
		}
		protected void ClearFrm()
		{
			btnSaveUpdate.Text = "Add User";
			ddlEmp.Enabled= true;
			btnCancel.Enabled=false;
			txtLDAP.Text = "";
			chkActive.Enabled = false;
			chkActive.Checked = true;
			ddlSgroup.SelectedValue = "-1";
			ddlEmp.SelectedValue = "-1";
		}
		protected void Clear(object sender, EventArgs e)
		{
			ClearFrm();
		}
		#endregion
	}
}
