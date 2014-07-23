/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 9/20/2013
 * Time: 1:34 PM
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
	/// Description of MainForm.
	/// </summary>
	public class Default : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected Panel panCompletMARs;
		protected Button btnComplete;
		protected Button btnBack;
		protected Button btnForward;
		protected Button btnGoToDate;
		protected DropDownList ddlMonth;
		protected DropDownList ddlYear;
		protected DataGrid dgNotifyGrid;
		protected Label lblMonth;
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Page Init & Exit (Open/Close DB connections here...)

		protected void PageInit(object sender, EventArgs e)
		{
			Utility.CheckLogin(Convert.ToBoolean(Session["login"]),this.Response);
		}
		//----------------------------------------------------------------------
		protected void PageExit(object sender, EventArgs e)
		{
		}

		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Page Load
		private void Page_Load(object sender, EventArgs e)
		{
			//------------------------------------------------------------------
			if(!IsPostBack)
			{
				ddlMonth.Items.Add(new ListItem("January", "1"));
				ddlMonth.Items.Add(new ListItem("February", "2"));
				ddlMonth.Items.Add(new ListItem("March", "3"));
				ddlMonth.Items.Add(new ListItem("April", "4"));
				ddlMonth.Items.Add(new ListItem("May", "5"));
				ddlMonth.Items.Add(new ListItem("June", "6"));
				ddlMonth.Items.Add(new ListItem("July", "7"));
				ddlMonth.Items.Add(new ListItem("August", "8"));
				ddlMonth.Items.Add(new ListItem("September", "9"));
				ddlMonth.Items.Add(new ListItem("October", "10"));
				ddlMonth.Items.Add(new ListItem("November", "11"));
				ddlMonth.Items.Add(new ListItem("December", "12"));
				
				ddlYear.Items.Add("2016");
				ddlYear.Items.Add("2015");
				ddlYear.Items.Add("2014");
				ddlYear.Items.Add("2013");
				ddlYear.Items.Add("2012");
				ddlYear.Items.Add("2011");
				ddlYear.Items.Add("2010");
				ddlYear.Items.Add("2009");
				ddlYear.Items.Add("2008");
				ddlYear.Items.Add("2007");
				ddlYear.Items.Add("2006");
				ddlYear.Items.Add("2004");
				ddlYear.Items.Add("2003");
				
				ddlMonth.SelectedValue = Convert.ToDateTime(Session["dtCDate"]).Month.ToString();
				ddlYear.SelectedValue = Convert.ToDateTime(Session["dtCDate"]).Year.ToString();
				lblMonth.Text = ddlMonth.SelectedItem.Text;
				NotifyFill();
			}
			

			//------------------------------------------------------------------
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region More...
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Initialize Component

		protected override void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		//----------------------------------------------------------------------
		private void InitializeComponent()
		{
			this.Load	+= new System.EventHandler(Page_Load);
			this.Init   += new System.EventHandler(PageInit);
			this.Unload += new System.EventHandler(PageExit);
			this.dgNotifyGrid.ItemDataBound += new DataGridItemEventHandler(_ItemDataBound);
		}
		protected void _ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			string lcom;
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
			{
				lcom = Convert.ToString(Utility.GetSqlScalar("select top 1 completeddate from CompletedMAR where EMP_ID = '" + e.Item.Cells[0].Text.ToString() + "' order by completeddate desc"));
				e.Item.Cells[0].Attributes.Add( "onClick", "return VerifyDue('" + lcom + "');");
			}
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Navigate
		protected void btnBack_Click(object sender, EventArgs e)
		{
			if(ddlMonth.SelectedValue == "1")
			{
				ddlMonth.SelectedValue = "12";
				ddlYear.SelectedValue = Convert.ToString(Convert.ToInt16(ddlYear.SelectedValue) - 1);
			}
			else
			{
				ddlMonth.SelectedValue = Convert.ToString(Convert.ToInt16(ddlMonth.SelectedValue) - 1);
			}
			Session["dtCDate"] = new DateTime(Convert.ToInt16(ddlYear.SelectedValue),Convert.ToInt16(ddlMonth.SelectedValue),1);
			lblMonth.Text = ddlMonth.SelectedItem.Text;
			NotifyFill();
		}
		protected void btnForward_Click(object sender, EventArgs e)
		{
			if(ddlMonth.SelectedValue == "12")
			{
				ddlMonth.SelectedValue = "1";
				ddlYear.SelectedValue = Convert.ToString(Convert.ToInt16(ddlYear.SelectedValue) + 1);
			}
			else
			{
				ddlMonth.SelectedValue = Convert.ToString(Convert.ToInt16(ddlMonth.SelectedValue) + 1);
			}
			Session["dtCDate"] = new DateTime(Convert.ToInt16(ddlYear.SelectedValue),Convert.ToInt16(ddlMonth.SelectedValue),1);
			lblMonth.Text = ddlMonth.SelectedItem.Text;
			NotifyFill();
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region DataFill
		protected void NotifyFill()
		{
			string sdate;
			string edate;
			string strcmd;
			
			sdate = ddlMonth.SelectedValue.ToString() + "/01/" + ddlYear.SelectedValue.ToString();
			edate = ddlMonth.SelectedValue.ToString() + "/" + DateTime.DaysInMonth(Convert.ToInt16(ddlYear.SelectedValue),Convert.ToInt16(ddlMonth.SelectedValue)).ToString() + "/" + ddlYear.SelectedValue.ToString();
		
			strcmd = "Select * from fn_Notifications('" + sdate + "','"  + edate +"') where EMP_ID in (select DSP_ID from v_ActiveUserDSPs where userid = " + Convert.ToString(Session["userid"]) + ")";
			
			dgNotifyGrid.DataSource = Utility.GetSQLDataView(strcmd);
			dgNotifyGrid.DataBind();
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data Manipulation
		protected void CompleteMAR(object sender, EventArgs e)
		{
			string useremp;
			//CheckBox chkCom;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand addcmd = new SqlCommand("CreateMARReview",sqlconn);
			
			useremp = Convert.ToString(Utility.GetSqlScalar("Select Emp_id from users where userid = " + Session["userid"].ToString()));
			
			addcmd.CommandType = CommandType.StoredProcedure;
			addcmd.Parameters.Add("@empid",SqlDbType.VarChar);
			addcmd.Parameters.Add("@cdate",SqlDbType.DateTime);
			addcmd.Parameters.Add("@ddate",SqlDbType.DateTime);
			addcmd.Parameters.Add("@cempid",SqlDbType.VarChar);
			addcmd.Parameters.Add("@modifiedby",SqlDbType.VarChar);
			sqlconn.Open();
			foreach (DataGridItem di in dgNotifyGrid.Items)
			{
				CheckBox chkCom = (CheckBox)di.Cells[0].FindControl("chkCompleted");
				if (chkCom.Checked)
				{				
					addcmd.Parameters["@empid"].Value = di.Cells[1].Text.ToString();
					addcmd.Parameters["@cdate"].Value = DateTime.Today;
					addcmd.Parameters["@ddate"].Value = Convert.ToDateTime(di.Cells[3].Text.ToString());
					addcmd.Parameters["@cempid"].Value = useremp;
					addcmd.Parameters["@modifiedby"].Value = Session["loguser"].ToString();
				
					addcmd.ExecuteNonQuery();
				}
			}
			addcmd = null;
			sqlconn.Close();
			
			NotifyFill();
		}
		#endregion
	}
}