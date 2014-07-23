<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.Default"
	ValidateRequest    = "false"
	EnableSessionState = "True"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
	<table>
		<tr><td><asp:hyperlink id="hlDSPMAR" runat="Server" NavigateUrl="DSPMAR.aspx" text="View MAR Reviews by DSP"/><hr></tr></td>
		<tr><td><asp:hyperlink id="hlMEDOPS" runat="Server" NavigateUrl="MedOps.aspx" text="View Med Observations by DSP"/><hr></tr></td>
	</table>	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
	<script language="JavaScript">
		function VerifyDue(ldate)
		{
			if (ldate!='')
			{
				var due = new Date(ldate);
				var cdate = new Date();
				var lcmon = due.getMonth();
				var cmonth = cdate.getMonth();
				if (lcmon = cmonth)
				{
					return confirm('You are attempting to mark an MAR Review completed in the same month the last MAR Review was completed is this correct?');
				}
			}
		}
	</script>
	<table width = 100%>
		<tr>
			<td>
				<table style="border:2px #4A004A solid" width = 100% cellpadding = 10>
					<tr>
						<td><center><h1>Items Due as of <asp:label id="lblMonth" runat="Server"/></h1></center></td>
					</tr>
				</table>
				<asp:panel id = "panCompletMARs" runat = "server" visible = "false">
					<h2>Click to mark check MAR Reviews as Completed</h2>
					<asp:button id = "btnComplete" Text = "Complete" runat="server" onClick="CompleteMAR"/> (select MAR Reviews with the "Completed" checkboxes below).</p>
				</asp:panel>
				<table style="border:2px #4A004A solid" width = 100% cellpadding = 10>
					<tr style="border:2px #4A004A solid">
						<td width = 37%> </td>
						<td width = 25%>
							<table width = 100%>
								<tr>
									<td align= "right"><asp:button text="&#60;" id="btnBack" OnClick="btnBack_Click" runat="server"/></td>
									<td><center><b>
										<asp:dropdownlist id="ddlMonth" runat="server" />
										<asp:dropdownlist id="ddlYear" runat="server" />
										<asp:button id="btnGoToDate" runat="server" text="Go to date" />
									</b></center></td>
									<td align = "left"><asp:button text="&#62;" id="btnForward" OnClick="btnForward_Click" runat="server"/></td>
								</tr>
							</table>
						</td>
						<td width = 37%> </td>
					</tr>
					<tr border = 1>
						<td colspan = 3 align = Center>
							<asp:datagrid id="dgNotifyGrid"
								autogeneratecolumns = "False"
								backcolor = "black"
								runat="server"
								gridlines="vertical"
								AutoPostBack="False">
							<headerstyle CssClass="GridHeader"/>
								<columns>
									<asp:boundcolumn headertext = "EMP_ID" datafield="EMP_ID" visible="False">
										<itemstyle CssClass="GridFColumn"/>
									</asp:boundcolumn>
									<asp:boundcolumn headertext = "DSP Name" datafield="EmployeeName">
										<itemstyle CssClass="GridFColumn"/>
									</asp:boundcolumn>
									<asp:boundcolumn headertext = "Inital Training Date" datafield="IniTDate">
										<itemstyle CssClass="GridColumns"/>
									</asp:boundcolumn>
									<asp:boundcolumn headertext = "Due Date" datafield="Due">
										<itemstyle CssClass="GridColumns"/>
									</asp:boundcolumn>
									<asp:boundcolumn headertext = "Status" datafield="Status">
										<itemstyle CssClass="GridFColumn"/>
									</asp:boundcolumn>
								</columns>
							</asp:datagrid>
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>		
</asp:Content>
