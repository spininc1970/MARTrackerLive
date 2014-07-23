/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 10/15/2013
 * Time: 11:22 AM
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
	/// Description of MARReviewUpload
	/// </summary>
	public class MARReviewUpload : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected DropDownList ddlEmp;
		protected DropDownList ddlDue;
		protected Label lblFile;
		protected FileUpload fuFileUpload;
		protected Button btnUploadFile;	
		protected HtmlGenericControl MARRev;
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
				
				ddlDue.Items.Insert(0,new ListItem("None Selected","-1"));
				ddlDue.SelectedValue = "-1";	
			}
			FillMAR();
			if(fuFileUpload.HasFile){
				btnUploadFile.Enabled = true;
			}
			//------------------------------------------------------------------
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
			this.ddlEmp.SelectedIndexChanged += new System.EventHandler(EmpChange);
			this.ddlDue.SelectedIndexChanged += new System.EventHandler(DueChange);
			this.btnUploadFile.Click += new System.EventHandler(UploadFile);
			//------------------------------------------------------------------
			//------------------------------------------------------------------
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region DataFill
		private void EmpChange(object sender,EventArgs e)
		{

			ddlDue.DataSource = Utility.GetSQLDataView("select CMARID, DueDate from v_allMars where Emp_Id = '" + ddlEmp.SelectedValue + "' and CMARID is not null Order by cast(DueDate as datetime) desc");			
			ddlDue.DataValueField = "CMARID";
			ddlDue.DataTextField = "DueDate";
			ddlDue.DataBind();
			
			ddlDue.Items.Insert(0,new ListItem("None Selected","-1"));
			ddlDue.SelectedValue = "-1";
			
			FillMAR();
		}
		private void DueChange(object sender, EventArgs e)
		{
			string fname;
			int len;
			int start;
			if(ddlDue.SelectedValue != "-1"){
				fuFileUpload.Enabled = true;
				btnUploadFile.Enabled = true;
			}
			else{
				fuFileUpload.Enabled= false;
				btnUploadFile.Enabled = false;
			}
			fname = Convert.ToString(Utility.GetSqlScalar("Select Path from MARReviews where CMARID = " + ddlDue.SelectedValue.ToString()));
			start = fname.LastIndexOf("\\");
			if(start == -1){
				start = 0;
			}
			len = fname.Length - start;
			                          
			lblFile.Text = fname.Substring(start,len);
		}
		private void FillMAR(){
			string fname;
			int len;
			int start;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand rcmd = new SqlCommand("select DueDate,CMARID from v_allMars where Emp_Id = '" + ddlEmp.SelectedValue + "' and CMARID in (select CMARID from MARReviews) Order by cast(DueDate as datetime) desc",sqlconn);
			string path;
			SqlDataReader drDue;
			
			sqlconn.Open();
			drDue = rcmd.ExecuteReader();
			
			MARRev.InnerHtml = "<ul>";
			
			while (drDue.Read()) {
				path = Convert.ToString(Utility.GetSqlScalar("Select Path from MARReviews where CMARID = " + Convert.ToString(drDue.GetSqlInt32(1))));
				MARRev.InnerHtml = MARRev.InnerHtml + "<li><a href='"+ path + "'>" + drDue.GetString(0) + "</a></li>";
				//MARRev.InnerHtml = drDue.GetString(0);
			}
			
			drDue = null;
			sqlconn.Close();
			
			MARRev.InnerHtml = MARRev.InnerHtml + "</ul>";
			fname = Convert.ToString(Utility.GetSqlScalar("Select Path from MARReviews where CMARID = " + ddlDue.SelectedValue.ToString()));
			start = fname.LastIndexOf("\\");
			if(start == -1){
				start = 0;
			}
			len = fname.Length - start;
			                          
			lblFile.Text = fname.Substring(start,len);
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region UploadRelated
		private void UploadFile(object sender,EventArgs e){
			HttpFileCollection upfiles = Request.Files;
			string spath;
			string uname;
			string fname;
			HttpPostedFile UpFile = upfiles[0];	
			
			if(fuFileUpload.HasFile){
				spath = "C:\\Inetpub\\SpinMARTracker\\MARReviews\\" + ddlEmp.SelectedValue + "\\";
			
				if(System.IO.Directory.Exists(spath) != true){
					System.IO.Directory.CreateDirectory(spath);
				}
				if(File.Exists(lblFile.Text)){
				   	File.Delete(lblFile.Text);
				}
				
				
				
				fname = Convert.ToString(Session["loguser"].ToString() + ddlDue.SelectedItem.Text.Replace("/","") + Path.GetExtension(UpFile.FileName));
			
				UpFile.SaveAs(spath + fname);
				
				if(lblFile.Text == ""){
					CreateMARUpload(fname,"Add");
				}
				else{
					CreateMARUpload(fname,"Update");
				}
			}
			else{
				
			}
			
			
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region DataManipulation
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
			
			cmd.Parameters["@cmarid"].Value = ddlDue.SelectedValue;
			cmd.Parameters["@path"].Value = uppath;
			cmd.Parameters["@uuser"].Value = Session["loguser"].ToString();
			
			cmd.ExecuteNonQuery();
			
			cmd = null;
			
			sqlconn.Close();
			
			FillMAR();
		}
		#endregion
	}
}
