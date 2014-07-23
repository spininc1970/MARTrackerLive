<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.Admin"
	ValidateRequest    = "false"
	EnableSessionState = "True"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
	<table>
		<tr><td><asp:hyperlink id="hlUsers" runat="Server" NavigateUrl="Users.aspx" text="Manage Users"/><hr></tr></td>
		<tr><td><asp:hyperlink id="hlMerge" runat="Server" NavigateUrl="MergeRecords.aspx" text="Merge Records"/><hr></tr></td>
	</table>	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
</asp:Content>