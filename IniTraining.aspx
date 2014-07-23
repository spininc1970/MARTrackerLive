<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.IniTraining"
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
	<table>
		<tr>
			<td>Employee:</td>
			<td><asp:dropdownlist id="ddlEmp" runat="Server" Autopostback="True"/></td>
		</tr>
		<tr>
			<td>Classroom Completion Date:</td>
			<td><asp:textbox id="txtCCDate" runat="Server"/><asp:RegularExpressionValidator runat=server ControlToValidate="txtCCDate" ErrorMessage="Date Invalid" ValidationExpression="^((((0[13578])|([13578])|(1[02]))[\/](([1-9])|([0-2][0-9])|(3[01])))|(((0[469])|([469])|(11))[\/](([1-9])|([0-2][0-9])|(30)))|((2|02)[\/](([1-9])|([0-2][0-9]))))[\/]\d{4}$|^\d{4}$" CSSClass = "Validator"/></td>
		<tr>
		<tr>
			<td>Medication Administration Completion Date:</td>
			<td><asp:textbox id="txtIniTrain" runat="Server"/><asp:RegularExpressionValidator runat=server ControlToValidate="txtIniTrain" ErrorMessage="Date Invalid" ValidationExpression="^((((0[13578])|([13578])|(1[02]))[\/](([1-9])|([0-2][0-9])|(3[01])))|(((0[469])|([469])|(11))[\/](([1-9])|([0-2][0-9])|(30)))|((2|02)[\/](([1-9])|([0-2][0-9]))))[\/]\d{4}$|^\d{4}$" CSSClass = "Validator"/></td>
		</tr>
		<tr>
			<td><asp:button id="btnSaveUpdate" runat="Server" text="Save" causesvalidation = "True" validationgroup = "Add"/></td>
		</tr>
	</table>
</asp:Content>
