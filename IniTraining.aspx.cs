/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 9/20/2013
 * Time: 3:39 PM
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
	/// Description of IniTraining
	/// </summary>
	public class IniTraining : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected DropDownList ddlEmp;
		protected TextBox txtIniTrain;
		protected Button btnSaveUpdate;
		protected TextBox txtCCDate;
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
				ddlEmp.DataSource = Utility.GetSQLDataView("Select EMP_ID,Last_Name + ', ' + First_Name as 'EmpName' from v_ActiveDSPs order by Last_Name");
				ddlEmp.DataValueField = "EMP_ID";
				ddlEmp.DataTextField="EmpName";
				ddlEmp.DataBind();
				
				ddlEmp.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlEmp.SelectedValue="-1";
			}
			SecurityChecks();
			//------------------------------------------------------------------
		}
		protected void SecurityChecks(){
			if (Session["canDel"].ToString() != "True"){
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
			this.ddlEmp.SelectedIndexChanged += new System.EventHandler(IndChange);
			this.btnSaveUpdate.Click += new System.EventHandler(DateAdd);
			//------------------------------------------------------------------
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region DataFill
		private void IndChange(object sender, EventArgs e)
		{
			string tDate;
			string cDate;
			
			tDate = Convert.ToString(Utility.GetSqlScalar("Select IniTDate from InitialTraining where EMP_ID =" + ddlEmp.SelectedValue));
			cDate = Convert.ToString(Utility.GetSqlScalar("Select ClassDate from InitialTraining where EMP_ID =" + ddlEmp.SelectedValue));
			
			if (tDate != "")
			{
				tDate = Convert.ToDateTime(tDate).ToShortDateString();
			}
			if (cDate != "")
			{
				cDate = Convert.ToDateTime(cDate).ToShortDateString();
			}
			txtIniTrain.Text = Convert.ToString(tDate);
			txtCCDate.Text = Convert.ToString(cDate);
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data Manipulation
		private void DateAdd(object sender, EventArgs e)
		{
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand addcmd = new SqlCommand("CreateTrainingDate",sqlconn);
			
			addcmd.CommandType = CommandType.StoredProcedure;
			addcmd.Parameters.Add("@empid",SqlDbType.VarChar);
			addcmd.Parameters.Add("@tdate",SqlDbType.DateTime);
			addcmd.Parameters.Add("@moduser",SqlDbType.VarChar);
			addcmd.Parameters.Add("@cdate",SqlDbType.DateTime);
			
			addcmd.Parameters["@empid"].Value = ddlEmp.SelectedValue;
			addcmd.Parameters["@moduser"].Value = Convert.ToString(Session["loguser"]);
			
			if (txtIniTrain.Text != "")
			{
				addcmd.Parameters["@tdate"].Value = txtIniTrain.Text;
			}
			else 
			{
				addcmd.Parameters["@tdate"].Value = DBNull.Value;
			}
			
			if (txtCCDate.Text != "")
			{
				addcmd.Parameters["@cdate"].Value = txtCCDate.Text;
			}
			else 
			{
				addcmd.Parameters["@cdate"].Value = DBNull.Value;
			}
			
			sqlconn.Open();
			
			addcmd.ExecuteNonQuery();
			
			addcmd = null;
			
			sqlconn.Close();
			
			Response.Write("<script>alert('Date(s) Updated')</script>");
		}
		#endregion
	}
}
