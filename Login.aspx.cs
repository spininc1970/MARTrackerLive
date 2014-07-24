/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 9/23/2013
 * Time: 9:18 AM
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
using System.DirectoryServices;


namespace SpinMARTracker
{
	/// <summary>
	/// Description of Login
	/// </summary>
	public class Login : Page
	{	
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Data
		protected TextBox txtUsername;
		protected TextBox txtPassword;
		protected Button btnLogin;
		protected Label lblMsg;

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
				Session["login"] = false;				
				Session.Remove("username");
				Session.Remove("logssn");
			}
			//------------------------------------------------------------------
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		#region Add more events here...
		public void MARLogin(object sender, EventArgs e)
		{
			string user = txtUsername.Text.ToString().Trim().ToLower();
			string active;
			
			if(Auth(user,txtPassword.Text.ToString()))
			{
				active = Convert.ToString(Utility.GetSqlScalar("Select Active from Users where LDAPLogin = '" + user + "'"));
				if (active == "True")
				{
					Session["userid"] = Convert.ToString(Utility.GetSqlScalar("Select UserID from Users where LDAPLogin = '" + user + "'"));
					Session["login"] = true;
					Session["loguser"] = user.ToString();
					LookUpGroup();
			   		Response.Redirect("Default.aspx");
				}
				else
				{
					lblMsg.Text = "You do not have rights to this system please contact your Manager";
				}

			}
			else
			{
				lblMsg.Text = "Invalid username or password please try again";
			}
		}
		private bool Auth(string un, string pword)
		{
			SearchResult SR;
			try
			{
				SR = LDAPLookup(un,pword);
				Session["logEmail"] = SR.GetDirectoryEntry().Properties["mail"].Value.ToString();
				return true;
			}
			catch(Exception ex)
			{
				return false;
			}
		}
		private SearchResult LDAPLookup(string un, string pword)
		{
			DirectoryEntry DE = new DirectoryEntry("LDAP://spindc1.spininc.local:389/DC=spininc,DC=local", un, pword);
			DirectorySearcher Search = new DirectorySearcher(DE);
			
			Search.Filter =("(sAMAccountName=" + un + ")");
			return Search.FindOne();
		}
		private void LookUpGroup(){
			SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
			SqlDataReader drGroup;
			SqlCommand cmdRead = new SqlCommand();
			string sgroup;
			string test;
			
			sgroup = Convert.ToString(Utility.GetSqlScalar("select sgroupid from users where userid='" + Session["userid"].ToString() + "'"));
			
			cmdRead.Connection = sqlconn;
			cmdRead.CommandText = "select CanDelete,CanMerge,EditRelationships,MergeDiff,CanExport,ManageUsers,ManageGroups from securitygroups where SGROUPID = " + sgroup;
			
			sqlconn.Open();
			
			drGroup = cmdRead.ExecuteReader();
			while(drGroup.Read()){
				Session["canDel"] = drGroup.GetBoolean(0).ToString();
				Session["CanMerge"] = drGroup.GetBoolean(1).ToString();
				Session["EditRel"] = drGroup.GetBoolean(2).ToString();
				Session["MergeD"] = drGroup.GetBoolean(3).ToString();
				Session["CanExport"] = drGroup.GetBoolean(4).ToString();
				Session["ManageU"] = drGroup.GetBoolean(5).ToString();
				Session["ManageGrps"] = drGroup.GetBoolean(6).ToString();
			}
			cmdRead = null;
			drGroup.Close();
			sqlconn.Close();

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
			this.btnLogin.Click += new System.EventHandler(MARLogin);
			//------------------------------------------------------------------
		}
		#endregion
		//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
	}
}
