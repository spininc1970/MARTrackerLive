/*
 * Created by SharpDevelop.
 * User: DWHITTAKER
 * Date: 7/21/2014
 * Time: 3:41 PM
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
	/// Description of SecurityGroups
	/// </summary>
	public class SecurityGroups : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected DataGrid dgSgrpGrid;
		protected CheckBox chkCanDel;
		protected CheckBox chkCanMer;
		protected CheckBox chkDiffMer;
		protected CheckBox chkEditRel;
		protected CheckBox chkExpMO;
		protected CheckBox chkManU;
		protected CheckBox chkManGrp;
		protected Button btnSaveUpdate;
		protected Button btnCancel;
		protected TextBox txtSgroup;
		protected Panel PanAddUpdate;
		protected HiddenField hdfRid;
			
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Page Init & Exit (Open/Close DB connections here...)

		protected void PageInit(object sender, System.EventArgs e)
		{
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
			}
			SecurityChecks();
			dgSgrpGrid.DataSource = Utility.GetSQLDataView("Select * from SecurityGroups");
			dgSgrpGrid.DataBind();
			//------------------------------------------------------------------
		}
		protected void SecurityChecks(){
			if(Session["ManageGrps"].ToString() != "True"){
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
			this.btnSaveUpdate.Click += new System.EventHandler(AddUpdateGroup);
			this.btnCancel.Click += new EventHandler(ClearFields);
			//------------------------------------------------------------------
			//------------------------------------------------------------------
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data Manipulation
		protected void AddUpdateGroup(object sender, EventArgs e){
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand addcmd = new SqlCommand("CreateSGroup", sqlconn);
			SqlCommand upcmd = new SqlCommand("UpdateSGroup",sqlconn);
			
			if (btnSaveUpdate.Text == "Add Group"){
				addcmd.CommandType = CommandType.StoredProcedure;
				addcmd.Parameters.Add("@name",SqlDbType.VarChar);
				addcmd.Parameters.Add("@canDel",SqlDbType.Bit);
				addcmd.Parameters.Add("@canMer",SqlDbType.Bit);
				addcmd.Parameters.Add("@EditRel",SqlDbType.Bit);
				addcmd.Parameters.Add("@MerD",SqlDbType.Bit);
				addcmd.Parameters.Add("@CanEx",SqlDbType.Bit);
				addcmd.Parameters.Add("@ManU",SqlDbType.Bit);
				addcmd.Parameters.Add("@ManG",SqlDbType.Bit);
				addcmd.Parameters["@name"].Value = txtSgroup.Text;
				addcmd.Parameters["@canDel"].Value = chkCanDel.Checked;
				addcmd.Parameters["@canMer"].Value = chkCanMer.Checked;
				addcmd.Parameters["@EditRel"].Value = chkEditRel.Checked;
				addcmd.Parameters["@MerD"].Value = chkDiffMer.Checked;
				addcmd.Parameters["@CanEx"].Value = chkExpMO.Checked;
				addcmd.Parameters["@ManU"].Value = chkManU.Checked;
				addcmd.Parameters["@ManG"].Value = chkManGrp.Checked;
				
				sqlconn.Open();
				
				addcmd.ExecuteNonQuery();
				
				addcmd = null;
				
				sqlconn.Close();
			}
			else{
				upcmd.CommandType = CommandType.StoredProcedure;
				upcmd.Parameters.Add("@sgid",SqlDbType.Int);
				upcmd.Parameters.Add("@name",SqlDbType.VarChar);
				upcmd.Parameters.Add("@canDel",SqlDbType.Bit);
				upcmd.Parameters.Add("@canMer",SqlDbType.Bit);
				upcmd.Parameters.Add("@EditRel",SqlDbType.Bit);
				upcmd.Parameters.Add("@MerD",SqlDbType.Bit);
				upcmd.Parameters.Add("@CanEx",SqlDbType.Bit);
				upcmd.Parameters.Add("@ManU",SqlDbType.Bit);
				upcmd.Parameters.Add("@ManG",SqlDbType.Bit);
				upcmd.Parameters["@sgid"].Value = hdfRid.Value;
				upcmd.Parameters["@name"].Value = txtSgroup.Text;
				upcmd.Parameters["@canDel"].Value = chkCanDel.Checked;
				upcmd.Parameters["@canMer"].Value = chkCanMer.Checked;
				upcmd.Parameters["@EditRel"].Value = chkEditRel.Checked;
				upcmd.Parameters["@MerD"].Value = chkDiffMer.Checked;
				upcmd.Parameters["@CanEx"].Value = chkExpMO.Checked;
				upcmd.Parameters["@ManU"].Value = chkManU.Checked;
				upcmd.Parameters["@ManG"].Value = chkManGrp.Checked;
				
				sqlconn.Open();
				
				upcmd.ExecuteNonQuery();
				
				upcmd = null;
				
				sqlconn.Close();
			}
			dgSgrpGrid.DataSource = Utility.GetSQLDataView("Select * from SecurityGroups");
			dgSgrpGrid.DataBind();
			ClearFrm();
		}
		protected void EditGroup(object sender, DataGridCommandEventArgs e){
			hdfRid.Value = e.Item.Cells[0].Text.ToString();
			txtSgroup.Text = e.Item.Cells[1].Text.ToString();
			chkCanDel.Checked = Convert.ToBoolean(e.Item.Cells[2].Text);
			chkCanMer.Checked = Convert.ToBoolean(e.Item.Cells[3].Text);
			chkDiffMer.Checked = Convert.ToBoolean(e.Item.Cells[4].Text);
			chkEditRel.Checked = Convert.ToBoolean(e.Item.Cells[5].Text);
			chkExpMO.Checked = Convert.ToBoolean(e.Item.Cells[6].Text);
			chkManU.Checked = Convert.ToBoolean(e.Item.Cells[7].Text);
			chkManGrp.Checked = Convert.ToBoolean(e.Item.Cells[8].Text);
			btnSaveUpdate.Text = "Update Group";
			
		}
		protected void DeleteGroup(object sender, DataGridCommandEventArgs e){
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand delcmd = new SqlCommand("DeleteSgroup",sqlconn);
			
			delcmd.CommandType = CommandType.StoredProcedure;
			delcmd.Parameters.Add("@sgid",SqlDbType.Int);
			delcmd.Parameters["@sgid"].Value = Convert.ToInt32(e.Item.Cells[0].Text.ToString());
			
			sqlconn.Open();
			
			delcmd.ExecuteNonQuery();
			
			delcmd = null;
			
			sqlconn.Close();
			
			dgSgrpGrid.DataSource = Utility.GetSQLDataView("Select * from SecurityGroups");
			dgSgrpGrid.DataBind();
		}
		protected void ClearFrm(){
			txtSgroup.Text = "";
			chkCanDel.Checked = false;
			chkCanMer.Checked = false;
			chkDiffMer.Checked = false;
			chkEditRel.Checked = false;
			chkExpMO.Checked = false;
			chkManU.Checked = false;
			chkManGrp.Checked = false;
			hdfRid.Value = "";
			btnSaveUpdate.Text = "Add Group";
		}
		protected void ClearFields(object sender, EventArgs e){
			ClearFrm();
		}
		#endregion
	}

}
