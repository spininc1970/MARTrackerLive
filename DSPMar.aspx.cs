/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 9/25/2013
 * Time: 3:53 PM
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
	/// Description of DSPMar
	/// </summary>
	public class DSPMar : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected DropDownList ddlEmp;
		protected Label lblJobT;
		protected Label lblNMar;
		protected Panel panEdit;
		protected Label lblDue;
		protected Label lblAction;
		protected TextBox txtCom;
		protected DropDownList ddlCBy;
		protected DataGrid dgMARGrid;
		protected Label lblIniT;
		protected Button btnCancel;
		protected Button btnSave;
		protected HiddenField sprdata;
		protected FileUpload fuFileUpload;
		protected string cmarid;
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
				
				dgMARGrid.DataSource = Utility.GetSQLDataView("Select * from v_AllMars where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(dueDate as datetime) asc");
				dgMARGrid.DataBind();
			}
			//------------------------------------------------------------------
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
			this.dgMARGrid.ItemDataBound += new DataGridItemEventHandler(DSPMar_ItemDataBound);

			//------------------------------------------------------------------
		}
		protected void EmpSelected(object sender,EventArgs e)
		{
			lblJobT.Text = Convert.ToString(Utility.GetSqlScalar("Select Job_Title from employee where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "'"));
			lblNMar.Text = Convert.ToString(Utility.GetSqlScalar("select top 1 convert(varchar,MarDue,101) as 'Due' from v_MarDueDates Where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(MARDue as datetime) desc"));
			lblIniT.Text = Convert.ToString(Utility.GetSqlScalar("Select IniTDate from InitialTraining where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "'"));
			dgMARGrid.DataSource = Utility.GetSQLDataView("Select * from v_AllMars where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(dueDate as datetime) asc");
			dgMARGrid.DataBind();
		}
		protected void DSPMar_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			string FExist;
			string url;
			int start = 0;
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
			{
				if(e.Item.Cells[9].Text.ToString() != "&nbsp;"){
					FExist = Convert.ToString(Utility.GetSqlScalar("Select UMARID from MARReviews where CMARID ="+ e.Item.Cells[9].Text.ToString()));
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
					url = Convert.ToString(Utility.GetSqlScalar("Select Path from MARReviews where CMARID = " + e.Item.Cells[9].Text.ToString()));
					url.Replace("\\","\\\\");
					while (url.IndexOf("\\",start + 1) > 0) {
						start = url.IndexOf("\\",start);
						url = url.Insert(start,"\\");
						start = start + 2;
					}
					LinkButton lbnCE = (LinkButton)e.Item.Cells[6].FindControl("lbnComEdit");
					lbnCE.Text = "View";
					lbnCE.Attributes.Add("onClick","JavaScript:ViewMAR('" + url + "');return false;");
					LinkButton lbnDel = (LinkButton)e.Item.Cells[7].FindControl("lbnDelete");
					if (Session["canDel"].ToString() == "True"){
						if (e.Item.Cells[1].Text == Convert.ToString(Utility.GetSqlScalar("select top 1 convert(varchar,duedate,101) from CompletedMAR where emp_id = '" + ddlEmp.SelectedValue.ToString() + "'order by completeddate desc"))){
							lbnDel.Visible = true;
							if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem){
								e.Item.Cells[8].Attributes.Add("onClick", "return ConfirmDeletion();");
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
		#region Data Manipulation
		protected void EditMar(object sender,DataGridCommandEventArgs e)
		{
			panEdit.Visible = true;
			lblDue.Text = e.Item.Cells[1].Text;
			if(e.Item.Cells[2].Text == "&nbsp;"){
				txtCom.Text = "";
				btnSave.Text = "Complete Mar Review";
			}
			else{
				txtCom.Text = e.Item.Cells[2].Text;
				btnSave.Text = "Update Mar Review";
				sprdata.Value = e.Item.Cells[9].Text;
			}
			if(e.Item.Cells[3].Text == "&nbsp;"){
				ddlCBy.SelectedValue = "-1";
			}
			else{
				ddlCBy.SelectedValue = e.Item.Cells[3].Text;
			}
			
		}
		protected void DeleteMar(object sender,DataGridCommandEventArgs e)
		{
			string fpath;
			string delfile;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand delcmd = new SqlCommand("DeleteMARReview",sqlconn);
			SqlCommand rucmd = new SqlCommand("DeleteUploadedMAR",sqlconn);
			
			cmarid = Convert.ToString(Utility.GetSqlScalar("select cmarid from completedmar where CMARID = " + e.Item.Cells[9].Text));
			
			fpath = "C:\\Inetpub\\SpinMARTracker\\MARReviews\\" + Convert.ToString(e.Item.Cells[0].Text) + "\\";
			
			delfile = fpath + "\\" + Convert.ToString(e.Item.Cells[5].Text);
			if (File.Exists(delfile)){
				File.Delete(delfile);
			}
			
			sqlconn.Open();
			delcmd.CommandType = CommandType.StoredProcedure;
			
			delcmd.Parameters.Add("@CMID",SqlDbType.Int);
			delcmd.Parameters["@CMID"].Value = Convert.ToInt32(e.Item.Cells[9].Text);
			
			delcmd.ExecuteNonQuery();
			delcmd = null;
			
			rucmd.CommandType = CommandType.StoredProcedure;
			
			rucmd.Parameters.Add("@CMID",SqlDbType.Int);
			rucmd.Parameters["@CMID"].Value = Convert.ToInt32(cmarid);
			
			rucmd.ExecuteNonQuery();
			rucmd = null;
			
			
			sqlconn.Close();
			
			
			dgMARGrid.DataSource = Utility.GetSQLDataView("Select * from v_AllMars where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(dueDate as datetime) asc");
			dgMARGrid.DataBind();
			
			lblDue.Text = "";
			txtCom.Text = "";
			ddlCBy.SelectedValue = "-1";
			panEdit.Visible = false;
			
		}
		protected void CompleteMar(object sender, EventArgs e)
		{
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand addcmd = new SqlCommand("CreateMARReview",sqlconn);
			if(btnSave.Text == "Complete Mar Review"){
				addcmd.CommandType = CommandType.StoredProcedure;
				addcmd.Parameters.Add("@empid",SqlDbType.VarChar);
				addcmd.Parameters.Add("@cdate",SqlDbType.DateTime);
				addcmd.Parameters.Add("@ddate",SqlDbType.DateTime);
				addcmd.Parameters.Add("@cempid",SqlDbType.VarChar);
				addcmd.Parameters.Add("@modifiedby",SqlDbType.VarChar);
				sqlconn.Open();
				
				addcmd.Parameters["@empid"].Value = ddlEmp.SelectedValue;
				addcmd.Parameters["@cdate"].Value = Convert.ToDateTime(txtCom.Text);
				addcmd.Parameters["@ddate"].Value = Convert.ToDateTime(lblDue.Text);
				addcmd.Parameters["@cempid"].Value = ddlCBy.SelectedValue;
				addcmd.Parameters["@modifiedby"].Value = Session["loguser"].ToString();
				
				addcmd.ExecuteNonQuery();
				
				addcmd = null;
				
				cmarid = Convert.ToString(Utility.GetSqlScalar("select cmarid from completedmar where emp_id = " + ddlEmp.SelectedValue + " and completeddate = '"+ txtCom.Text.ToString() +"' and duedate = '"+ lblDue.Text.ToString() +"'"));
				
				UploadFile();
				
				
			}
			else{
				
			}
			sqlconn.Close();
			
			dgMARGrid.DataSource = Utility.GetSQLDataView("Select * from v_AllMars where EMP_ID = '" + ddlEmp.SelectedValue.ToString() + "' order by Cast(dueDate as datetime) asc");
			dgMARGrid.DataBind();
			
			lblDue.Text = "";
			txtCom.Text = "";
			ddlCBy.SelectedValue = "-1";
			panEdit.Visible = false;
		}
		protected void CancelEdit(object sender, EventArgs e)
		{
			lblDue.Text = "";
			txtCom.Text = "";
			ddlCBy.SelectedValue = "-1";
			panEdit.Visible = false;
		}
		protected void UploadFile(){
			HttpFileCollection upfiles = Request.Files;
			string spath;
			string uname;
			string fname;
			string file;
			int MarTotal;
			int CurMar;
			HttpPostedFile UpFile = upfiles[0];	
			
			if(fuFileUpload.HasFile){
				spath = "C:\\Inetpub\\SpinMARTracker\\MARReviews\\" + ddlEmp.SelectedValue + "\\";
			
				if(System.IO.Directory.Exists(spath) != true){
					System.IO.Directory.CreateDirectory(spath);
				}
				file = Convert.ToString(Utility.GetSqlScalar("Select Path from MARReviews where CMARID = " + cmarid.ToString()));
				if(File.Exists(file)){
				   	File.Delete(file);
				}
				
				MarTotal = Convert.ToInt32(Utility.GetSqlScalar("Select Count(CMARID) from CompletedMAR where EMP_ID = " + ddlEmp.SelectedValue.ToString()));
				
				MarTotal = MarTotal + 4;
				
				CurMar = MarTotal % 4;
				
				if(CurMar == 0){
					CurMar = 4;
				}
				
				fname = Convert.ToString(Session["loguser"].ToString() + lblDue.Text.Replace("/","") + CurMar + Path.GetExtension(UpFile.FileName));
			
				UpFile.SaveAs(spath + fname);
				
				CreateMARUpload(fname,"Add");
			}
			else{
				
			}	
		}
		private void CreateMARUpload(string path,string type){
			string uppath;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand cmd = new SqlCommand("",sqlconn);
			
			if(type == "Add"){
				cmd.CommandText = "CreateMARReviewUpload";
			}
			else{
				cmd.CommandText = "UpdateMARReviewUpload";
			}
			
			uppath = "\\Smart\\MARReviews\\" + ddlEmp.SelectedValue.ToString() + "\\" +path;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add("@cmarid",SqlDbType.Int);
			cmd.Parameters.Add("@path",SqlDbType.VarChar);
			cmd.Parameters.Add("@uuser",SqlDbType.VarChar);
			sqlconn.Open();
			
			cmd.Parameters["@cmarid"].Value = cmarid;
			cmd.Parameters["@path"].Value = uppath;
			cmd.Parameters["@uuser"].Value = Session["loguser"].ToString();
			
			cmd.ExecuteNonQuery();
			
			cmd = null;
			
			sqlconn.Close();
		}
		#endregion
	}
}
