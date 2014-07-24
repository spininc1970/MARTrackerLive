<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.SecurityGroups"
	ValidateRequest    = "false"
	EnableSessionState = "true"
	maintainScrollPositionOnPostBack = "False"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
	<table>
		<tr><td><asp:hyperlink id="hlUsers" runat="Server" NavigateUrl="Users.aspx" text="Back To Users Screen"/><hr></td></tr>
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
			<td>
				<asp:panel id="PanUpAdd" runat="Server">
					<table border = 1>
						<tr>
							<td>Security Group: <asp:textbox id="txtSgroup" runat="Server"/></td>
							<td><asp:hiddenfield id="hdfRid" runat="Server"/></td>
							<td></td>
						</tr>
						<tr>
							<td>Access to Delete(Also Controls access to Admin and Configuration Screens):</td>
							<td><asp:checkBox id="chkCanDel" runat="Server"/></td>
							<td>
								<table border = 1>
									<tr>
										<td>Access To Merge Users:</td>
										<td><asp:checkbox id="chkCanMer" runat="Server"/></td>
									</tr>
									<tr>
										<td>Can Merge users with different names:</td>
										<td><asp:checkbox id="chkDiffMer" runat="Server"/></td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td>Access to Edit/Add Relationships:</td>
							<td><asp:checkbox id="chkEditRel" runat="Server"/></td>
							<td>Access to Export Med Observations:</td>
							<td><asp:checkbox id="chkExpMO" runat="Server"/></td>
						</tr>
						<tr>
							<td>Access to Manage Users: </td>
							<td><asp:checkbox id="chkManU" runat="Server"/></td>
							<td>Access to Manage Security Groups:</td>
							<td><asp:checkbox id="chkManGrp" runat="Server"/></td>
						</tr>
						<tr>
							<td><asp:button id="btnSaveUpdate" runat="Server" text="Add Group"/></td>
							<td><asp:button id="btnCancel" runat="Server" text="Cancel"/></td>
						</tr>
					</table>
				</asp:panel>
			</td>
		</tr>
		<tr>
			<td colspan="3">
				<table border = 1 bordercolor = "Black" width =100% >
					<tr>
						<td align = "center" bgcolor="#4A004A;">
							<font size = medium color = White><b>Security Groups</b></font>
						</td>
					</tr>
				</table>
			<td>
		</tr>
		<tr>
			<td align = "center" colspan=3>
				<asp:datagrid id="dgSgrpGrid"
						autogeneratecolumns = "False"
						backcolor = "black"
						runat="server"
						gridlines="vertical"
						onDeleteCommand="DeleteGroup"
						onEditCommand="EditGroup"
						AutoPostBack="False">
					<headerstyle CssClass="GridHeader"/>
					<columns>
						<asp:boundcolumn headertext = "SGroupID" datafield="SGroupID" visible="False">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Security Group" datafield="GroupName" visible="True">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Can Delete" datafield="CanDelete">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Can Merge" datafield="CanMerge">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Merge Different" datafield="MergeDiff">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext="Edit Relationships" datafield="EditRelationships">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Can Export" datafield="CanExport">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Manage Users" datafield="ManageUsers">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Manage Groups" datafield="ManageGroups">
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