<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.Users"
	ValidateRequest    = "false"
	EnableSessionState = "True"
	maintainScrollPositionOnPostBack = "False"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
	<table>
		<tr><td><asp:hyperlink id="hlBAdmin" runat="Server" NavigateUrl="Admin.aspx" text="Back to Admin screen"/><hr></td></tr>
		<tr><td><asp:hyperlink id="hlMAGrps" runat="Server" NavigateUrl="SecurityGroups.aspx" text="Add/Edit Security Groups"/><hr></td></tr>
	</table>	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
	<script language="JavaScript"> 
    	function ConfirmDeletion() 
        {
        	return confirm('Are you sure you wish to delete this record?');
        }
   	</script>
	<table>
		<tr>
			<td>Employee:</td>
			<td><asp:dropdownlist id="ddlEmp" runat="Server" AutoPostBack = "True"/></td>
			<td></td>
		</tr>
		<tr>
			<td>LDAP Login Name:</td>
			<td><asp:textbox id="txtLDAP" runat="Server"/></td>
			<td></td>
		</tr>
		<tr>
			<td>Security Group:</td>
			<td><asp:dropdownlist id="ddlSgroup" runat="Server"/></td>
			<td></td>
		</tr>
		<tr>
			<td>Active:</td>
			<td><asp:checkbox id="chkActive" runat="Server" enabled="False" checked="true"/></td>
			<td></td>
		</tr>
		<tr>
			<td><asp:button id="btnSaveUpdate" runat="Server" text="Add User" enabled="false" onClick="AddEmp"/></td>
			<td><asp:button id="btnCancel" runat="Server" text="Cancel" enabled="false" onClick="Clear"/></td>
		</tr>
		<tr>
			<td colspan="3">
				<table border = 1 bordercolor = "Black" width =100% >
					<tr>
						<td align = "center" bgcolor="#4A004A;">
							<font size = medium color = White><b>Users</b></font>
						</td>
					</tr>
				</table>
			<td>
		</tr>
		<tr>
			<td align = "center" colspan=3>
				<asp:datagrid id="dgEmpGrid"
						autogeneratecolumns = "False"
						backcolor = "black"
						runat="server"
						gridlines="vertical"
						onDeleteCommand="RemoveEmp"
						onEditCommand="EditEmp"
						AutoPostBack="False">
					<headerstyle CssClass="GridHeader"/>
					<columns>
						<asp:boundcolumn headertext = "User ID" datafield="UserID" visible="False">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "EMP ID" datafield="EMP_ID" visible="False">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Employee Name" datafield="EmployeeName">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "LDAP Login Name" datafield="LDAPLogin">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Gid" datafield="SGroupID" visible="False">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext="Security Group" datafield="GroupName">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Active" datafield="Active">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:TemplateColumn HeaderText="">
							<ItemTemplate>
								<asp:linkbutton id="lbnEdit" runat="server" CommandName="Edit">Edit</asp:linkbutton>
							</ItemTemplate>
							<Itemstyle HorizontalAlign="Center" CssClass="GridColumns"/>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="">
							<ItemTemplate>
								<asp:linkbutton id="lbnDelete" runat="server" CommandName="Delete">Delete</asp:linkbutton>
							</ItemTemplate>
							<Itemstyle HorizontalAlign="Center" CssClass="GridColumns"/>
						</asp:TemplateColumn>
					</columns>
				</asp:datagrid>					
			</td>
		</tr>
	</table>
</asp:Content>

