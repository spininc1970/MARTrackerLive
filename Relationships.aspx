<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.Relationships"
	ValidateRequest    = "false"
	EnableSessionState = "True"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
	<table>
		<tr><td><asp:hyperlink id="hlConfig" runat="Server" NavigateUrl="Config.aspx" text="Back to configuration"/><hr></td></tr>	
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
			<td>User:</td>
			<td><asp:dropdownlist id="ddlUser" runat="Server" Autopostback="True"/></td>
		</tr>
		<tr>
			<td>DSP:</td>
			<td><asp:dropdownlist id="ddlDSP" runat="Server" enabled="False"/></td>
		</tr>
		<tr>
			<td>Persist</td>
			<td><asp:checkbox id="chkPer" runat = "Server" enabled="False"/></td>
		</tr>
		<tr>
			<td>Start Date:</td>
			<td><asp:textbox id="txtSdate" runat="Server" enabled= "false"/></td>
			<td>End Date:</td>
			<td><asp:textbox id="txtEdate" runat="Server" enabled="False"/></td>
		</tr>
		<tr>
			<td><asp:button id="btnSaveUpdate" runat="Server" Text="Save Relationship" enabled="false" onClick="AddDSP"/></td>
			<td><asp:button id="btnCancel" runat="Server" text="Cancel" enabled="False" onClick ="Clear"/></td>
		</tr>
		<tr>
			<td colspan="4">
				<table border = 1 bordercolor = "Black" width =100% >
					<tr>
						<td align = "center" bgcolor="#4A004A;">
							<font size = medium color = White><b>DSPs</b></font>
						</td>
					</tr>
				</table>
			<td>
		</tr>
		<tr>
			<td align = "center" colspan=4>
				<asp:datagrid id="dgDSPGrid"
						autogeneratecolumns = "False"
						backcolor = "black"
						runat="server"
						gridlines="vertical"
						onDeleteCommand="RemoveDSP"
						onEditCommand="EditDSP"
						AutoPostBack="False">
					<headerstyle CssClass="GridHeader"/>
					<columns>
						<asp:boundcolumn headertext = "Relationship ID" datafield="RelationshipID" visible="False">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "User ID" datafield="UserID" visible="False">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "DSP ID" datafield="DSP_ID" visible="False">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "DSP Name" datafield="DSPName">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Relationship Start Date" datafield="StartDate">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Relationship End Date" datafield="EndDate">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Persist" datafield="Persist">
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