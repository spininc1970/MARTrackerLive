/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 5/1/2014
 * Time: 11:30 AM
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
	/// Description of MergeRecords
	/// </summary>
	public class MergeRecords : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected ddlSearch cwcSourceEmp;
		protected ddlSearch cwcMergeEmp;
		protected DataGrid dgSourceEmp;
		protected DataGrid dgMergeEmp;
		protected Button btnMerge;
		protected UpdatePanel UpPan1;
		protected UpdatePanel UpPan2;
		protected UpdatePanel UpPan3;
		protected HiddenField hdNames;
		protected bool Minid;
		protected bool Sinid;
		protected bool bkeepdates;
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
			SecurityChecks();
			if(! IsPostBack)
			{
				cwcSourceEmp.CtrlDataSource = "select emp_id,last_name + ', ' + first_name + ' (' + isnull(socialsecurityno,emp_id) + ', ' + dept_name + ' ' + job_title + ')' as empinfo from Employee order by last_name, First_Name";
				cwcSourceEmp.CtrlTField = "EmpInfo";
				cwcSourceEmp.CtrlVField = "EMP_ID";
				cwcSourceEmp.CtrlBind();
				
				cwcSourceEmp.ConWidth = 380;
				
				cwcMergeEmp.CtrlDataSource = "select emp_id,last_name + ', ' + first_name + ' (' + isnull(socialsecurityno,emp_id) + ', ' + dept_name + ' ' + job_title + ')' as empinfo from Employee order by last_name, First_Name";
				cwcMergeEmp.CtrlTField = "EmpInfo";
				cwcMergeEmp.CtrlVField = "EMP_ID";
				cwcMergeEmp.CtrlBind();
				
				cwcMergeEmp.ConWidth = 380;
			}

			//------------------------------------------------------------------
		}
		protected void SecurityChecks(){
			if (Session["CanMerge"].ToString() != "True"){
				Response.Redirect("Default.aspx");
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
			this.cwcSourceEmp.IndChange += new EventHandler<EvArgs>(SourceChange);
			this.cwcMergeEmp.IndChange += new EventHandler<EvArgs>(MergeChange);
			this.dgSourceEmp.ItemDataBound += new DataGridItemEventHandler(SourceBound);
			this.btnMerge.Click += new System.EventHandler(MergeRecord);
			//------------------------------------------------------------------
			//------------------------------------------------------------------
		}
		protected void SourceBound(object sender, DataGridItemEventArgs e){
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem){
				RadioButton rd = e.Item.Cells[3].FindControl("rdKeep") as RadioButton;
				RadioButton rd1 = e.Item.Cells[4].FindControl("rdDel") as RadioButton;
				
				rd.GroupName = e.Item.Cells[2].ToString();
				rd1.GroupName = rd.GroupName.ToString();
			}
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region datamanipulation
		protected void SourceChange(object sender, System.EventArgs e){
			dgSourceEmp.DataSource = Utility.GetSQLDataView("select Data, Info,ID from v_GenEmpInfo where emp_id = " + cwcSourceEmp.CtrlSelValue.ToString());
			dgSourceEmp.DataBind();
			CheckNames();
			UpPan1.Update();
		}
		protected void MergeChange(object sender, System.EventArgs e){
			dgMergeEmp.DataSource = Utility.GetSQLDataView("select Data, Info,ID from v_GenEmpInfo where emp_id = " + cwcMergeEmp.CtrlSelValue.ToString());
			dgMergeEmp.DataBind();
			CheckNames();
			UpPan2.Update();
		}
		protected void CheckNames(){
			string SName;
			string MName;
			
			SName = Convert.ToString(Utility.GetSqlScalar("Select Last_Name + ' ' + First_Name from employee where emp_id = " + cwcSourceEmp.CtrlSelValue.ToString()));
			MName = Convert.ToString(Utility.GetSqlScalar("Select Last_Name + ' ' + First_Name from employee where emp_id = " + cwcMergeEmp.CtrlSelValue.ToString()));
			
			if (SName != MName){
				hdNames.Value = "No";
				if(Session["MergeD"].ToString() == "True"){
					btnMerge.Enabled = true;
				}
				else{
					btnMerge.Enabled = false;
				}
				UpPan3.Update();
			}
			else{
				hdNames.Value = "Yes";
				btnMerge.Enabled = true;
				UpPan3.Update();
			}
		}
		protected void MergeRecord(object sender, System.EventArgs e){
			RadioButton rddel;
			RadioButton rdkeep;
			CheckBox chk;
			bool AllSel;
			if(cwcSourceEmp.CtrlSelValue == "-1" || cwcMergeEmp.CtrlSelValue == "-1"){
				Response.Write("<script>alert('You must select both a Source and Merge to Employee')</script>");
			}
			else{
				AllSel = ValAllSelected();
				if (AllSel == false){
					Response.Write("<script>alert('You must choose to either keep or delete every item in the Source Employee table')</script>");
				}
				else{
					if(ValIniTraining() == "Run"){
						DelFromMerge();
						DelFromSource();
						MoveToMerge();
					}
				}
			}
		}
		private bool ValAllSelected(){
			RadioButton rddel;
			RadioButton rdkeep;
			bool Sel;
			string test;
			Sel = true;
			foreach(DataGridItem i in dgSourceEmp.Items){
				rdkeep = (RadioButton)i.Cells[3].FindControl("rdKeep");
				rddel = (RadioButton)i.Cells[4].FindControl("rdDel");
				
				if (rdkeep.Checked != true && rddel.Checked != true){
					Sel = false;
				}
			}
			return Sel;
		}
		private string ValIniTraining(){
			RadioButton rddel;
			CheckBox chkDel;
			foreach(DataGridItem i in dgSourceEmp.Items){
				rddel = (RadioButton)i.Cells[4].FindControl("rdDel");
				
				
				if (i.Cells[0].Text == "Initial Training Dates"){
					if(rddel.Checked == true){
						Sinid = false;	
					}
					else{
						Sinid = true;
					}
				}
			}
			foreach (DataGridItem m in dgMergeEmp.Items) {
				chkDel = (CheckBox) m.Cells[3].FindControl("chkDelM");
				if (m.Cells[0].Text == "Initial Training Dates"){
					if(chkDel.Checked == true){
						Minid = false;
					}
					else{
						Minid = true;
					}
				}
			}
			
			if(Sinid == true && Minid == true){
				Response.Write("<script>alert('You may only keep one set initial training date not both.  Please choose one to delete')</script>");
				return "No";
			}
			else if(Sinid == false && Minid == false){
				Response.Write("<script>alert('You cannot delete both sets initial training dates please choose one to keep.')</script>");
				return "No";
			}
			else{
				return "Run";
			}
		}
		private void DelFromMerge(){
			CheckBox chkdelc;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand delmar = new SqlCommand("DeleteMARReview",sqlconn);
			SqlCommand delumar = new SqlCommand("DeleteUploadedMAR",sqlconn);
			SqlCommand delmed = new SqlCommand("DeleteMedObservation",sqlconn);
			string type;
			
			delmar.CommandType = CommandType.StoredProcedure;
			delmar.Parameters.Add("@CMID",SqlDbType.Int);
			
			
			delumar.CommandType = CommandType.StoredProcedure;
			delumar.Parameters.Add("@CMID",SqlDbType.Int);
			
			delmed.CommandType = CommandType.StoredProcedure;
			delmed.Parameters.Add("@MedObId",SqlDbType.Int);
			
			sqlconn.Open();
			foreach (DataGridItem i in dgMergeEmp.Items){
				chkdelc = (CheckBox)i.Cells[3].FindControl("chkDelM");
				
				if(chkdelc.Checked==true){
					type = i.Cells[0].Text.ToString();
					
					if (type != "Initial Training Dates"){
						ManipulateFiles(i.Cells[2].Text.ToString(),type,"Delete");
					}
		
					if(type == "MAR Review"){
						
						
						delmar.Parameters["@CMID"].Value = Convert.ToInt32(i.Cells[2].Text.ToString());
						
						
						delumar.Parameters["@CMID"].Value = Convert.ToInt32(i.Cells[2].Text.ToString());
						
						
						
						delmar.ExecuteNonQuery();
						delumar.ExecuteNonQuery();

						

					
					}
					else if (type == "Med Observation"){
						delmed.Parameters["@MedObId"].Value = Convert.ToInt32(i.Cells[2].Text.ToString());

						
						delmed.ExecuteNonQuery();
						
					}
					else if(type == "Initial Training Dates"){
						bkeepdates = false;
					}
				}
			}
			delmar = null;
			delumar = null;
			delmed = null;
			sqlconn.Close();
		}
		private void DelFromSource(){
			RadioButton rdDelI;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand delmar = new SqlCommand("DeleteMARReview",sqlconn);
			SqlCommand delumar = new SqlCommand("DeleteUploadedMAR",sqlconn);
			SqlCommand delmed = new SqlCommand("DeleteMedObservation",sqlconn);
			string type;
			
			delmar.CommandType = CommandType.StoredProcedure;
			delmar.Parameters.Add("@CMID",SqlDbType.Int);
			
			delumar.CommandType = CommandType.StoredProcedure;
			delumar.Parameters.Add("@CMID",SqlDbType.Int);
			
			delmed.Parameters.Add("@MedObId",SqlDbType.Int);
			delmed.CommandType = CommandType.StoredProcedure;
			
			sqlconn.Open();
			foreach (DataGridItem i in dgSourceEmp.Items) {
				rdDelI = (RadioButton)i.Cells[4].FindControl("rdDel");
				
				if(rdDelI.Checked == true){
					type = i.Cells[0].Text.ToString();
					
					if (type != "Initial Training Dates"){
						ManipulateFiles(i.Cells[2].Text.ToString(),type,"Delete");
					}
					
					if(type == "MAR Review"){
						
						
						
						delmar.Parameters["@CMID"].Value = Convert.ToInt32(i.Cells[2].Text.ToString());
						
						delumar.Parameters["@CMID"].Value = Convert.ToInt32(i.Cells[2].Text.ToString());
						
						delmar.ExecuteNonQuery();
						delumar.ExecuteNonQuery();
					
					}
					else if (type == "Med Observation"){
						
						delmed.Parameters["@MedObId"].Value = Convert.ToInt32(i.Cells[2].Text.ToString());
						
						delmed.ExecuteNonQuery();
						
					}
					else if(type == "Initial Training Dates"){
						bkeepdates = true;
					}
				}
			}			
			delmar = null;
			delumar = null;
			delmed = null;			
			
			sqlconn.Close();
		}
		private void MoveToMerge(){
			int mout;
			RadioButton rdKeepI;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand cmdEMer = new SqlCommand("EmpMerge",sqlconn);
			
			foreach (DataGridItem i in dgSourceEmp.Items) {
				rdKeepI = (RadioButton)i.Cells[3].FindControl("rdKeep");
				
				if(rdKeepI.Checked == true && i.Cells[0].Text.ToString() != "Initial Training Dates"){
					ManipulateFiles(i.Cells[2].Text.ToString(),i.Cells[0].Text.ToString(),"Move");
				}
			}			
			
			cmdEMer.CommandType = CommandType.StoredProcedure;
			cmdEMer.Parameters.Add("@sempid",SqlDbType.Int);
			cmdEMer.Parameters.Add("@mempid",SqlDbType.Int);
			cmdEMer.Parameters.Add("@kd",SqlDbType.Bit);
			cmdEMer.Parameters.Add("@merged",SqlDbType.Int);
			cmdEMer.Parameters["@merged"].Direction = ParameterDirection.Output;
			
			cmdEMer.Parameters["@sempid"].Value = cwcSourceEmp.CtrlSelValue;
			cmdEMer.Parameters["@mempid"].Value = cwcMergeEmp.CtrlSelValue;
			cmdEMer.Parameters["@kd"].Value = bkeepdates;
			
			sqlconn.Open();
			
			cmdEMer.ExecuteNonQuery();
			
			mout = Convert.ToInt32(cmdEMer.Parameters["@merged"].Value);
			
			cmdEMer = null;
			
			sqlconn.Close();
			
			if(mout == 1){
				Response.Write("<script>alert('Merge Successful')</script>");
			}
			else{
				Response.Write("<script>alert('Merge Failed')</script>");
			}
			
		}
		private void ManipulateFiles(string FileID,string Type,string Action){
			string fpath;
			string mpath; 
			string npath;
			string sqlpath;
			string aFile;
			string mFile;
			string strSQL = "";
			string sqlid = "";
			string sqlTable = "";
			string test;
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlCommand cmdMfile = new SqlCommand();
			
			mpath = "C:\\Inetpub\\SpinMARTracker";
			
			
			if(Type == "MAR Review"){
				strSQL = "select path from marreviews where CMARID = " + FileID;
				sqlTable = "MarReviews";
				sqlid = "CMARID";
			}
			else if(Type == "Med Observation"){
				strSQL = "select path from MedObservations where MedOPID = " + FileID;
				sqlTable = "MedObservations";
				sqlid = "MedOpID";
			}
			
			sqlpath = Convert.ToString(Utility.GetSqlScalar(strSQL));
			
			fpath = sqlpath.Substring(7,sqlpath.Length - 7);
			
			aFile = mpath + "\\" + fpath; 
			
			mFile = aFile.Replace(cwcSourceEmp.CtrlSelValue.ToString(),cwcMergeEmp.CtrlSelValue.ToString());
			
			cmdMfile.Connection = sqlconn;
			
			if(Action == "Move"){
				npath = sqlpath.Replace(cwcSourceEmp.CtrlSelValue.ToString(),cwcMergeEmp.CtrlSelValue.ToString());
				if(File.Exists(aFile)){
					if(!File.Exists(mFile)){
					   	File.Move(aFile,mFile);
					   }
				}
				
				strSQL = "update " + sqlTable + " set path = '" + npath + "' where " + sqlid + " = " + FileID;
				cmdMfile.CommandText = strSQL;
				sqlconn.Open();
				cmdMfile.ExecuteNonQuery();
				
				cmdMfile = null;
				
				sqlconn.Close();
			}
			else if(Action == "Delete"){
				if(File.Exists(aFile)){
					File.Delete(aFile);
				}
			}
			
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
	}
}
