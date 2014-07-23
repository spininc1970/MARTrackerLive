<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.Config"
	ValidateRequest    = "false"
	EnableSessionState = "True"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
	<table>
		<tr><td><asp:hyperlink id="hlIniT" runat="Server" NavigateUrl="IniTraining.aspx" text="Add Initial Training date to DSP"/><hr></td></tr>	
		<tr><td><asp:hyperlink id="hlRel" runat="Server" NavigateUrl="Relationships.aspx" text="Link DSP to User"/><hr></td></tr>
	</table>	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
</asp:Content>