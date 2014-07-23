/*
 * Created by SharpDevelop.
 * User: dwhittaker
 * Date: 9/20/2013
 * Time: 1:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Configuration;
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
	/// Description of Utility.
	/// </summary>
	public class Utility
	{
		public static DataView GetSQLDataView(string SQLCmd)
		{
			string connStr = System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"];
			SqlConnection sqlconn = new SqlConnection(connStr);
			SqlDataAdapter daData = new SqlDataAdapter("",connStr);
			DataSet dsData = new DataSet("Data");
			DataView dvData = new DataView();
			
			dsData.Clear();
			
			daData.SelectCommand.Connection.ConnectionString = connStr;
			
			daData.SelectCommand.CommandText = SQLCmd;
			sqlconn.Open();
			daData.Fill(dsData);
			dvData.Table = dsData.Tables[0];
			sqlconn.Close();
			return dvData;
		}
		public static object GetSqlScalar(string SQLCmd)
		{
			string connStr = System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"];
			object result;
			SqlConnection sqlconn = new SqlConnection(connStr);
			SqlCommand cmd = new SqlCommand(SQLCmd,sqlconn);
			
			sqlconn.Open();
			
			result = cmd.ExecuteScalar();
			
			cmd = null;
			
			sqlconn.Close();
			
			return result;
		}
		public static void CheckLogin(bool log, HttpResponse p)
		{
			if (log == false)
			{
				p.Redirect("Login.aspx");
			}
		}
	}
}
