﻿<%@ Page
	Language           = "VB"
	AutoEventWireup    = "false"
	Inherits           = "SpinMARTracker.MergeRecords"
	ValidateRequest    = "false"
	EnableSessionState = "True"
	MasterPageFile="~/SiteMaster.master"
%>
<%@ MasterType VirtualPath="~/SiteMaster.master" %>
<%@ Register TagPrefix="CWC" TagName="ddlSearch" src="ddlSearch.ascx" %> 
	<asp:Content ID="Content1" ContentPlaceHolderID="Links" Runat="Server">
		<table>
			<tr><td><a href="Admin.aspx">Back to Admin Screen</a></tr></td>
		</table>
	</asp:Content>
	<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
		<script language="JavaScript"> 
       		function ConfirmMerge() 
           	{
           		var cResult;
           	
             	if (document.getElementById("Main_hdNames").value == "No") {
             		cResult = confirm("Names do not match in Source Employee and Merge to Employee proceeding with this Merge could be dangerous.  Do you still wish to continue?");
             	}
             	else{
             		cResult = true;
             	}
             	
             	if (cResult == true){
             	return confirm('This merge will move all records in the Source Employee field to the Merge to Employee field and delete the employee in the Source Employee field.  Do you still wish to continue?');
             	}
             	else{
             		return false;
             	}
           	}
   		</script>
		<table width = 80% border = 1>
			<asp:hiddenfield id="hdNames" runat="Server"/>
			<tr>
				<td>
					<asp:UpdatePanel id = "UpPan1" runat = "Server" UpdateMode = "Conditional">
						<ContentTemplate>
							<table width = 100%>
								<tr>
									<td><b>Source Employee:</b><CWC:ddlSearch id="cwcSourceEmp" runat="Server"/><asp:RequiredFieldValidator id="RequiredFieldValidator1" validationgroup = "Add" ControlToValidate="cwcSourceEmp$txtSearch" Text="Source Employee Required!" CSSClass = "Validator" EnableClientScript="true" runat="server"/></td>
								</tr>
								<tr>
									<td align = "Center" bgcolor = "#4A004A"><h3><font color="white">Source Employee Info:</font></h3></td>
								</tr>
								<tr border = 1>
									<td align = "center">
										<asp:datagrid id="dgSourceEmp" 
											autogeneratecolumns = "False" 
											backcolor = "black" 
											runat="server" 
											gridlines="vertical"
											AutoPostBack = "False" width = 380>
											<headerstyle CssClass="GridHeader"/>
											<columns>
												<asp:boundcolumn headertext="Info" datafield="Info">
													<Itemstyle cssClass="GridFColumn"/>
												</asp:boundcolumn>
												<asp:boundcolumn headertext="Data" datafield="Data">
													<Itemstyle cssClass="GridColumns"/>
												</asp:boundcolumn>
												<asp:boundcolumn headertext="ID" datafield="ID" visible = "false">
													<Itemstyle cssClass="GridColumns"/>
												</asp:boundcolumn>
												<asp:templatecolumn HeaderText="Keep" visible="True">
					  								<itemtemplate>
														<asp:RadioButton id="rdKeep" runat="server" Text=""/>
													</itemtemplate>
												</asp:templatecolumn>
												<asp:templatecolumn HeaderText="Delete" visible="True">
					  								<itemtemplate>
														<asp:RadioButton id="rdDel" runat="server" Text=""/>
													</itemtemplate>
												</asp:templatecolumn>
											</columns>
										</asp:datagrid>
									</td>
								</tr>
							</table>
						</ContentTemplate>
						<Triggers>
							
						</Triggers>
					</asp:UpdatePanel>
				</td>
				<td align = "Center">
					<asp:UpdatePanel id="UpPan3" runat="Server" UpdateMode = "Conditional">
						<ContentTemplate>
							<asp:button id="btnMerge" runat="Server" text="Merge Records" OnClientClick = "return ConfirmMerge();" causevalidation = "True" validationgroup = "Add"/>
						</ContentTemplate>
					</asp:UpdatePanel>
				</td>
				<td>
					<asp:UpdatePanel id = "UpPan2" runat = "Server" UpdateMode = "Conditional">
						<ContentTemplate>
							<table width = 100%>
								<tr>
									<td><b>Merge to Employee:</b><CWC:ddlSearch id="cwcMergeEmp" runat="Server"/><asp:RequiredFieldValidator id="RequiredFieldValidator2" EnableClientScript="true" validationgroup = "Add" ControlToValidate="cwcMergeEmp$txtSearch" Text="Merge to Employee Required!" CSSClass = "Validator" runat="server"/></td>
								</tr>
								<tr>
									<td align = "Center" bgcolor = "#4A004A"><h3><font color="white">Merge to Employee Info:</font></h3></td>
								</tr>
								<tr>
									<td align = "center">
										<asp:datagrid id="dgMergeEmp" 
											autogeneratecolumns = "False" 
											backcolor = "black" 
											runat="server" 
											gridlines="vertical"
											AutoPostBack = "False" width = 380>
											<headerstyle CssClass="GridHeader"/>
											<columns>
												<asp:boundcolumn headertext="Info" datafield="Info">
													<Itemstyle cssClass="GridFColumn"/>
												</asp:boundcolumn>
												<asp:boundcolumn headertext="Data" datafield="Data">
													<Itemstyle cssClass="GridColumns"/>
												</asp:boundcolumn>
												<asp:boundcolumn headertext="ID" datafield="ID" visible = "false">
													<Itemstyle cssClass="GridColumns"/>
												</asp:boundcolumn>
												<asp:templatecolumn HeaderText="Delete" visible="True">
					  								<itemtemplate>
														<asp:CheckBox id="chkDelM" runat="server" Text=""/>
													</itemtemplate>
												</asp:templatecolumn>
											</columns>
										</asp:datagrid>
									</td>
								</tr>
							</table>
						</ContentTemplate>
						<Triggers>
							
						</Triggers>
					</asp:UpdatePanel>
				</td>
			</tr>
		</table>
	</asp:Content>