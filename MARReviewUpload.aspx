<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.MARReviewUpload"
	ValidateRequest    = "false"
	EnableSessionState = "True"
	maintainScrollPositionOnPostBack = "False"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
	<table>
		<tr><td><asp:hyperlink id="hlDef" runat="Server" NavigateUrl="Default.aspx" text="Back to Main"/><hr></tr></td>
	</table>	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
	<Table border = 0 width = "60%">
		<tr>
			<td>Emloyee: <asp:dropdownlist id="ddlEmp" runat="Server" autopostback="True"/></td>
		</tr>
		<tr>
			<td>Due Dates: <asp:dropdownlist id="ddlDue" runat="Server" autopostback="True"/></td>
		</tr>
		<tr>
			<td>Existing File: <asp:Label id="lblFile" runat="Server"/></td>
		</tr>
		<tr>
			<td>New File: <asp:FileUpload id = "fuFileUpload" text="Choose File" runat = "server" width = 600 enabled = "false"/></td>
			<td><asp:button id="btnUploadFile" text="Upload" runat="server" enabled="False"/></td>
		</tr>
		<tr>
			<td>
				<table border = 1 bordercolor = "Black" width = "100%">
					<tr>
						<td align = "center" bgcolor="#4A004A;">
							<font size = medium color = White><b>MAR Reviews by Due Date</b></font>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td align="center">
				<div class="Due" id="MARRev" runat="server">
				</div>
			</td>
		</tr>
	</table>
</asp:Content>
