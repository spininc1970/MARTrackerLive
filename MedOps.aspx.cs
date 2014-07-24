/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 12/16/2013
 * Time: 3:43 PM
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
using System.IO;

namespace SpinMARTracker
{
	/// <summary>
	/// Description of MedOps
	/// </summary>
	public class MedOps : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected DropDownList ddlEmp;
		protected Label lblJobT;
		protected Label lblNMed;
		protected Panel panEdit;
		protected Label lblDue;
		protected Label lblAction;
		protected TextBox txtCom;
		protected DropDownList ddlCBy;
		protected DataGrid dgMedGrid;
		protected Label lblIniT;
		protected Button btnCancel;
		protected Button btnSave;
		protected HiddenField sprdata;
		protected FileUpload fuFileUpload;
		protected DropDownList ddlMOType;
		protected TextBox txtComments;
		protected Button btnAdd;
		protected RequiredFieldValidator RequiredFieldValidator2;
		protected int mid;
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
			if(! IsPostBack)
			{
				ddlEmp.DataSource = Utility.GetSQLDataView("Select EMP_ID,Last_Name + ', ' + First_Name as 'EmpName' from v_ActiveDSPs order by last_name");
				ddlEmp.DataTextField = "EmpName";
				ddlEmp.DataValueField = "EMP_ID";
				ddlEmp.DataBind();
				
				ddlEmp.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlEmp.SelectedValue = "-1";
				
		 
				ddlCBy.DataSource = Utility.GetSQLDataView("Select EMP_ID,EmployeeName from v_Users order by EmployeeName");
				ddlCBy.DataTextField = "EmployeeName";
				ddlCBy.DataValueField = "EMP_ID";
				ddlCBy.DataBind();
				
				ddlCBy.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlCBy.SelectedValue = "-1";
				
				dgMedGrid.DataSource = Utility.GetSQLDataView("Select * from v_AllMedObs where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(dueDate as datetime) asc");
				dgMedGrid.DataBind();
				
				ddlMOType.DataSource = Utility.GetSQLDataView("Select MedObType,Type from MedObTypes where MedObType <> 1");
				ddlMOType.DataTextField = "Type";
				ddlMOType.DataValueField = "MedObType";
				ddlMOType.DataBind();
				
				ddlMOType.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlMOType.SelectedValue = "-1";
			}
			//------------------------------------------------------------------
			SecurityChecks();
		}
		protected void SecurityChecks(){
			if (Session["canDel"].ToString() != "True"){
				btnAdd.Visible = false;
			}
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
			this.ddlEmp.SelectedIndexChanged += new System.EventHandler(EmpSelected);
			this.dgMedGrid.ItemDataBound += new DataGridItemEventHandler(DSPMed_ItemDataBound);
			//------------------------------------------------------------------
		}
		protected void EmpSelected(object sender,EventArgs e)
		{
			lblJobT.Text = Convert.ToString(Utility.GetSqlScalar("Select Job_Title from employee where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "'"));
			lblNMed.Text = Convert.ToString(Utility.GetSqlScalar("select top 1 convert(varchar,MedObDue,101) as 'Due' from v_MedObDueDates Where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(MedObDue as datetime) desc"));
			lblIniT.Text = Convert.ToString(Utility.GetSqlScalar("Select IniTDate from InitialTraining where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "'"));
			dgMedGrid.DataSource = Utility.GetSQLDataView("Select * from v_AllMedObs where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(dueDate as datetime) asc");
			dgMedGrid.DataBind();
		}
		protected void DSPMed_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			string FExist;
			string url;
			int start = 0;
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
			{
				if(e.Item.Cells[10].Text.ToString() != "&nbsp;"){
					FExist = Convert.ToString(Utility.GetSqlScalar("Select MedOpID from MedObservations where MedOpID ="+ e.Item.Cells[10].Text.ToString()));
				}
				else{
					FExist = "";
				}
				if(e.Item.Cells[2].Text.ToString() == "&nbsp;")
				{
					LinkButton lbnCE = (LinkButton)e.Item.Cells[6].FindControl("lbnComEdit");
					lbnCE.Text = "Complete";
					LinkButton lbnDel = (LinkButton)e.Item.Cells[7].FindControl("lbnDelete");
					lbnDel.Visible = false;
				}
				if(FExist != ""){
					url = Convert.ToString(Utility.GetSqlScalar("Select [Path] from MedObservations where MedOpID = " + e.Item.Cells[10].Text.ToString()));
					url.Replace("\\","\\\\");
					while (url.IndexOf("\\",start + 1) > 0) {
						start = url.IndexOf("\\",start);
						url = url.Insert(start,"\\");
						start = start + 2;
					}
					LinkButton lbnCE = (LinkButton)e.Item.Cells[7].FindControl("lbnComEdit");
					lbnCE.Text = "View";
					lbnCE.Attributes.Add("onClick","JavaScript:ViewMed('" + url + "');return false;");
					LinkButton lbnDel = (LinkButton)e.Item.Cells[9].FindControl("lbnDelete");
					if (Session["canDel"].ToString() == "True"){
						if (e.Item.Cells[1].Text == Convert.ToString(Utility.GetSqlScalar("select top 1 convert(varchar,duedate,101) from medobservations where emp_id = '" + ddlEmp.SelectedValue.ToString() + "' and medobtype = 1 order by uploaddate desc"))){
							lbnDel.Visible = true;
							if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem){
								e.Item.Cells[9].Attributes.Add("onClick", "return ConfirmDeletion();");
							}
						}
						else if (e.Item.Cells[5].Text.ToString() != "6 Month Review"){
							lbnDel.Visible = true;
							if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem){
								e.Item.Cells[9].Attributes.Add("onClick", "return ConfirmDeletion();");
							}
						}
						else{
							lbnDel.Visible = false;
						}
					}
					else{
						lbnDel.Visible = false;
					}   
				}
			}
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region MedOp Related
		protected void AddMed(object sender, EventArgs e)
		{
			lblDue.Text = "";
			panEdit.Visible = true;
			btnSave.Text = "Add New Med Observation";
			btnAdd.Visible = false;	
		}
		protected void CompleteMed(object sender, EventArgs e)
		{
			int Mtype;
			string due;
			string edit;
			string tDate;
			string cDate;
			
			edit = ddlMOType.Enabled.ToString();
			
			tDate = Convert.ToString(Utility.GetSqlScalar("Select IniTDate from InitialTraining where EMP_ID =" + ddlEmp.SelectedValue));
			
			cDate = Convert.ToString(Utility.GetSqlScalar("Select ClassDate from InitialTraining where EMP_ID =" + ddlEmp.SelectedValue));
			
			if (tDate == "")
			{
				tDate = "1/1/1900";
			}
			
			if (cDate == ""){
				cDate = "1/1/1900";
			}
			
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand addcmd = new SqlCommand("CreateMedOb",sqlconn);
			if (ddlMOType.Enabled == false){
				Mtype = 0;
				if (cDate != "" && tDate == "")
				{
					Mtype = 2;
				}
				if (Convert.ToDateTime(cDate) > Convert.ToDateTime(tDate))
				{
					Mtype = 2;
				}
				if (Convert.ToDateTime(tDate) > Convert.ToDateTime(cDate))
				{
					Mtype = 1;						
				}
			}
			else{
				Mtype = Convert.ToInt32(ddlMOType.SelectedValue);
			}
			if (lblDue.Text == ""){
				due = Convert.ToString(DateTime.Today);
			}
			else{
				due = lblDue.Text;
			}
			if (btnSave.Text != "Update Med Observation"){
				addcmd.CommandType = CommandType.StoredProcedure;
				addcmd.Parameters.Add("@empid",SqlDbType.VarChar);
				addcmd.Parameters.Add("@mtype",SqlDbType.Int);
				addcmd.Parameters.Add("@ddate",SqlDbType.DateTime);
				addcmd.Parameters.Add("@com",SqlDbType.Text);
				addcmd.Parameters.Add("@uuser",SqlDbType.VarChar);
				addcmd.Parameters.Add("@medid",SqlDbType.Int).Direction = ParameterDirection.Output;
				
				addcmd.Parameters["@empid"].Value = ddlEmp.SelectedValue;
				addcmd.Parameters["@mtype"].Value = Mtype;
				
				addcmd.Parameters["@com"].Value = txtComments.Text;
				addcmd.Parameters["@uuser"].Value = Session["loguser"].ToString();
				
				if (Mtype == 2)
				{
					addcmd.Parameters["@ddate"].Value = DBNull.Value;
				}
				else
				{
					addcmd.Parameters["@ddate"].Value = due;
				}
				
				sqlconn.Open();
				
				addcmd.ExecuteNonQuery();
				
				mid = Convert.ToInt32(addcmd.Parameters["@medid"].Value);
				
				addcmd = null;
				
				sqlconn.Close();
				
				UploadFile(Mtype);
			}
			dgMedGrid.DataSource = Utility.GetSQLDataView("Select * from v_AllMedObs where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(dueDate as datetime) asc");
			dgMedGrid.DataBind();
			panEdit.Visible = false;
			if (Session["canDel"].ToString() != "True"){
				btnAdd.Visible = false;
			}
			else{
				btnAdd.Visible = true;
			}
			ddlMOType.Enabled = true;
			RequiredFieldValidator2.Enabled = true;
			ddlCBy.SelectedValue = "-1";
			ddlMOType.SelectedValue = "-1";
			lblDue.Text = "";
			txtComments.Text = "";
			Mtype = 0;
		}
		protected void CancelEdit(object sender, EventArgs e)
		{
			panEdit.Visible = false;			
			lblDue.Text = "";
			txtComments.Text = "";
			ddlCBy.SelectedValue = "-1";
			ddlMOType.Enabled = true;
			RequiredFieldValidator2.Enabled = true;
			ddlMOType.SelectedValue = "-1";
			btnSave.Text = "Update Med Observation";
			if (Session["canDel"].ToString() != "True"){
				btnAdd.Visible = false;
			}
			else{
				btnAdd.Visible = true;
			}
		}
		protected void UploadFile(int motype)
		{
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand upcmd = new SqlCommand("CreateMedObUpload",sqlconn);
			HttpFileCollection upfiles = Request.Files;
			string spath;
			string ddue;
			string fname;
			string file;
			HttpPostedFile UpFile = upfiles[0];	
			string uppath;
			string medt;
			string abbr;
			int CurMed;
			int MedTotal;
			
			if(fuFileUpload.HasFile){
				spath = "C:\\Inetpub\\SpinMARTracker\\MedObservations\\" + ddlEmp.SelectedValue + "\\";
			
				if(System.IO.Directory.Exists(spath) != true){
					System.IO.Directory.CreateDirectory(spath);
				}
				file = Convert.ToString(Utility.GetSqlScalar("Select Path from MedObservations where MedOPID = " + mid.ToString()));
				if(File.Exists(file)){
				   	File.Delete(file);
				}
				medt = Convert.ToString(Utility.GetSqlScalar("Select MedObType from MedObservations where MedOPID = " + mid.ToString()));
				
				abbr = Convert.ToString(Utility.GetSqlScalar("Select Shortcut from MedObTypes where MedObType = " + medt.ToString()));
				
				if(motype == 2){
					ddue = Convert.ToString(DateTime.Today.ToShortDateString());
				}
				else{
					ddue = lblDue.Text;
				}
				
				MedTotal = Convert.ToInt32(Utility.GetSqlScalar("Select Count(MedOPID) from MedObservations where EMP_ID = " + ddlEmp.SelectedValue.ToString()));
				
				MedTotal = MedTotal + 2;
				
				CurMed = MedTotal % 2;
				
				if (CurMed == 0){
					CurMed = 2;
				}
				
				fname = Convert.ToString(Session["loguser"].ToString() + abbr + ddue.Replace("/","") + CurMed + Path.GetExtension(UpFile.FileName));
			
				UpFile.SaveAs(spath + fname);
				
				uppath = "\\Smart\\MedObservations\\" + ddlEmp.SelectedValue.ToString() + "\\" + fname;
				
				upcmd.CommandType = CommandType.StoredProcedure;
				upcmd.Parameters.Add("@Medobid",SqlDbType.Int);
				upcmd.Parameters.Add("@path",SqlDbType.VarChar);
				
				upcmd.Parameters["@Medobid"].Value = mid;
				upcmd.Parameters["@path"].Value = uppath;
				
				sqlconn.Open();
				
				upcmd.ExecuteNonQuery();
				
				upcmd = null;
				
				sqlconn.Close();
				
			}
			else{
				
			}	
			
		}
		protected void DeleteMed(object sender,DataGridCommandEventArgs e)
		{
			string fpath;
			string delfile;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand delcmd = new SqlCommand("DeleteMedObservation",sqlconn);
			
			fpath = "C:\\Inetpub\\SpinMARTracker\\MedObservations\\" + Convert.ToString(e.Item.Cells[0].Text) + "\\";
			
			delfile = fpath + "\\" + Convert.ToString(e.Item.Cells[4].Text);
			
			if(File.Exists(delfile)){
				File.Delete(delfile);
			}
			
			delcmd.CommandType = CommandType.StoredProcedure;
			delcmd.Parameters.Add("@MedObID",SqlDbType.Int);
			
			delcmd.Parameters["@MedObID"].Value = Convert.ToInt32(e.Item.Cells[10].Text);
			
			sqlconn.Open();
			
			delcmd.ExecuteNonQuery();
			
			delcmd = null;
			
			sqlconn.Close();
			
			dgMedGrid.DataSource = Utility.GetSQLDataView("Select * from v_AllMedObs where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(dueDate as datetime) asc");
			dgMedGrid.DataBind();
		}
		protected void EditMed(object sender,DataGridCommandEventArgs e)
		{
			panEdit.Visible = true;
			lblDue.Text = e.Item.Cells[1].Text;
			if(e.Item.Cells[2].Text == "&nbsp;"){
				txtComments.Text = "";
				btnSave.Text = "Complete Med Observation";
				ddlMOType.Enabled = false;
				btnAdd.Visible = false;
				RequiredFieldValidator2.Enabled = false;
			}
			if(e.Item.Cells[3].Text == "&nbsp;"){
				ddlCBy.SelectedValue = "-1";
			}
			else{
				ddlCBy.SelectedValue = e.Item.Cells[3].Text;
			}
		}
		#endregion
	}
}
