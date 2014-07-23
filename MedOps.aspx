<%@ Page
	Language           = "C#"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.MedOps"
	ValidateRequest    = "false"
	EnableSessionState = "True"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
	<table>
		<tr><td><asp:hyperlink id="hlDef" runat="Server" NavigateUrl="Default.aspx" text="Back to Main"/><hr></tr></td>
		<tr><td><asp:hyperlink id="hlMARSS" runat="Server" NavigateUrl="DSPMar.aspx" text="View MAR Reviews by DSP"/><hr></tr></td>
	</table>	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
	<script language="JavaScript"> 
    	function ConfirmDeletion() 
     	{
     		return confirm('Are you sure you wish to delete this record?');
    	}
    	function ViewMed(url)
    	{
    		var url1 = url;
    		popupWindow = window.open(url1,'myWindow','toolbar=1,scrollbars=1,location=1,statusbar=1,menubar=1,resizable=1,width=1000,height=1000');
    	}
   	</script>
	<Table border = 0 width = "40%">
		<tr>
			<td>Employee: <asp:dropdownlist id="ddlEmp" runat="Server" autopostback="True"/></td>
		</tr>
		<tr>
			<td>Job Title: <asp:label id="lblJobT" runat="Server"/></td>
		</tr>
		<tr>
			<td>Initial Training: <asp:label id="lblIniT" runat="Server"/></td>
		</tr>
		<tr>
			<td>Next Med Observation Due: <b><asp:label id="lblNMed" runat="Server"/></b></td>
		</tr>
		<tr>
			<td>
				<asp:panel id="panEdit" runat="Server" visible="False">
					<table style="border:2px #4A004A solid"  border = 0 width = 1000>
						<tr>
							<td colspan = 6><center><h2><asp:label id="lblAction" runat="Server"/> Selected Med Observation</h2></center></td>
						</tr>
						<tr>
							<td>Due:</td>
							<td colspan = 5><b><asp:label id="lblDue" runat="Server"/></b></td>
						</tr>
						<tr>
							<td width = 150>Uploaded By:</td>
							<td><asp:dropdownlist id="ddlCBy" runat="Server"/></td>
							<td><asp:hiddenfield id="sprdata" runat="server"/><asp:RequiredFieldValidator id="RequiredFieldValidator1" validationgroup = "Add" ControlToValidate="ddlCBy" InitialValue = "-1" Text="Employee Required!" CSSClass = "Validator" runat="server"/></td>
						</tr>
						<tr>
							<td>Type:</td>
							<td><asp:dropdownlist id="ddlMOType" runat="Server" /><asp:RequiredFieldValidator id="RequiredFieldValidator2" validationgroup = "Add" ControlToValidate="ddlMOType" InitialValue = "-1" Text="Med Ob Type Required!" CSSClass = "Validator" runat="server"/></td>
							<td>Comments:</td>
							<td><asp:textbox id="txtComments" runat="Server" width = 250/></td>
						</tr>	
						<tr>
							<td>File:</td>
							<td colspan = 6><asp:FileUpload id = "fuFileUpload" text="Choose File" runat = "server" width = 600 /></td>
							<td><asp:RequiredFieldValidator id="RequiredFieldValidator3" runat="server" ErrorMessage="File Required" ControlToValidate="fuFileUpload" validationgroup = "Add" CSSClass = "Validator"/><asp:RegularExpressionValidator id="RegularExpressionValidator1" runat="server" ErrorMessage="File Not PDF" ValidationExpression ="^.+(.pdf|.PDF)$" ControlToValidate="fuFileUpload" validationgroup = "Add" CSSClass = "Validator"/>
						</tr>
						<tr >
							<td colspan= 3><asp:button id="btnSave" onClick="CompleteMed" runat="Server" Text="Update Med Observation" causesvalidation = "True" validationgroup = "Add"/></td>
							<td colspan= 3><asp:button id="btnCancel" OnClick="CancelEdit" runat="Server" Text="Cancel" causesvalidation = "False"/></td>
						</tr>
					</table>
				</asp:panel>
			</td>
		</tr>
		<tr>
			<td>
				<asp:button id="btnAdd" onClick = "AddMed" runat="Server" text="Add New" />
			</td>
		</tr>
	</table>
	<table>
		<tr>
			<td>
				<table border = 1 bordercolor = "Black" width = "100%">
					<tr>
						<td align = "center" bgcolor="#4A004A;">
							<font size = medium color = White><b>Med Observations</b></font>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan=2>
					<asp:datagrid id="dgMedGrid"
						autogeneratecolumns = "False"
						backcolor = "black"
						runat="server"
						gridlines="vertical"
						AutoPostBack="False"
						onEditCommand="EditMed"
						onDeleteCommand="DeleteMed">
					<headerstyle CssClass="GridHeader"/>
					<columns>
						<asp:boundcolumn headertext = "EMP ID" datafield="EMP_ID" visible="False">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Med Ob Due Date" datafield="DueDate">
							<itemstyle CssClass="GridFColumn"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Upload Date" datafield="UploadDate">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Upload User" datafield="UploadUser" visible="False">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Path" datafield="Path">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Type" datafield="Type">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Comments" datafield="Comments">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:boundcolumn headertext = "Status" datafield="Status">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
						<asp:TemplateColumn HeaderText="">
							<ItemTemplate>
								<asp:linkbutton id="lbnComEdit" runat="server" CommandName="Edit" causesvalidation = "False">Edit</asp:linkbutton>
							</ItemTemplate>
							<Itemstyle HorizontalAlign="Center" CssClass="GridColumns"/>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="">
							<ItemTemplate>
								<asp:linkbutton id="lbnDelete" runat="server" CommandName="Delete" causesvalidation = "False">Delete</asp:linkbutton>
							</ItemTemplate>
							<Itemstyle HorizontalAlign="Center" CssClass="GridColumns"/>
						</asp:TemplateColumn>
						<asp:boundcolumn headertext = "MedOpID" datafield="MedOpID" visible = "False">
							<itemstyle CssClass="GridColumns"/>
						</asp:boundcolumn>
					</columns>
				</asp:datagrid>			
			</td>
		</tr>
	</table>
</asp:Content>