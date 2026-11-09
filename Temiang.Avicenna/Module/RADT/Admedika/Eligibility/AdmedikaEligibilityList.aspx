<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="AdmedikaEligibilityList.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaEligibilityList" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="cc" Namespace="Temiang.Avicenna.CustomControl" Assembly="Temiang.Avicenna" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function openNewEligibility() {
                var oWnd = $find("<%= winEligibility.ClientID %>");
                oWnd.argument = new Object();
                oWnd.setUrl("AdmedikaEligibilityDialog.aspx");
                oWnd.show();
            }

            function onEligibilityClientClose(oWnd, args) {
                var mode = (oWnd.argument && oWnd.argument.mode)
                    ? oWnd.argument.mode
                    : "rebind";

                __doPostBack("<%= grdList.UniqueID %>", mode);
                if (oWnd.argument) oWnd.argument.mode = null;
            }

            function openSelectPatient(id, cardNo, patientName) {
                var oWnd = $find("<%= winPatient.ClientID %>");
                oWnd.setUrl("AdmedikaPatientDialogList.aspx?id=" + encodeURIComponent(id) +
                    "&cardNo=" + encodeURIComponent(cardNo) +
                    "&patientName=" + encodeURIComponent(patientName));
                oWnd.show();
            }

            function onPatientClientClose(oWnd, args) {
                if (oWnd.argument && oWnd.argument.mode) {
                    __doPostBack("<%= grdList.UniqueID %>", oWnd.argument.mode);
                    oWnd.argument.mode = null;
                }
            }

            function openRegistration(patientID, registrationType, claimID) {
                var oWnd = $find("<%= winRegistration.ClientID %>");

                var url = "<%= Page.ResolveUrl("~/Module/RADT/Registration/RegistrationDetail.aspx") %>?md=new"
                    + "&id=" + encodeURIComponent(patientID)
                    + "&rt=" + encodeURIComponent(registrationType)
                    + "&type=admedika"
                    + "&ClaimID=" + encodeURIComponent(claimID)
                    + "&norm=";

                oWnd.setUrl(url);
                oWnd.show();
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindow ID="winEligibility" runat="server" Animation="None" Behavior="Close,Move"
        ShowContentDuringLoad="False" Width="1150px" Height="700px" VisibleStatusbar="False"
        Modal="true" OnClientClose="onEligibilityClientClose" />
    <telerik:RadWindow ID="winPatient" runat="server" Animation="None" Behavior="Close,Move,Resize,Maximize"
        ShowContentDuringLoad="False" Width="1150px" Height="600px" VisibleStatusbar="False"
        Modal="true" OnClientClose="onPatientClientClose" />
    <telerik:RadWindow ID="winPrint" Animation="None" Width="1000px" Height="500px" runat="server"
        ShowContentDuringLoad="false" Behavior="Close,Resize,Maximize" VisibleStatusbar="false"
        Modal="true">
    </telerik:RadWindow>
    <telerik:RadWindow ID="winRegistration" runat="server" Animation="None"
        ShowContentDuringLoad="False" Width="1200px" Height="640px"
        VisibleStatusbar="False" Modal="true" Behavior="Close,Move,Resize,Maximize" />
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnFilterDate">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterRegistration">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterPatient">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterCard">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterClID">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <cc:CollapsePanel ID="CollapsePanel1" runat="server" Title="Search Filter">
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                            <td class="label">
                                Created Date
                            </td>
                            <td class="entry">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <telerik:RadDatePicker ID="txtCreatedDate" runat="server" Width="100px" />
                                        </td>
                                        <td>
                                        &nbsp;-&nbsp;
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="txtCreatedDateTo" runat="server" Width="100px" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterDate" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Registration No
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtRegistrationNo" runat="server" Width="300px" MaxLength="40" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterRegistration" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                ClID (Claim ID)
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtClID" runat="server" Width="300px" MaxLength="20" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterClID" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                            <td class="label">
                                Patient Name
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtPatientName" runat="server" Width="300px" MaxLength="150" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterPatient" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Card No
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtCardNo" runat="server" Width="300px" MaxLength="32" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterCard" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </cc:CollapsePanel>
    <table width="100%" cellpadding="0" cellspacing="10">
        <tr>
            <td style="padding: 4px 0 6px 0;">
                <asp:Button ID="btnNewEligibility" runat="server" Text="New Eligibility"
                    OnClientClick="openNewEligibility(); return false;" />
            </td>
        </tr>
    </table>
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" Width="100%" LoadingPanelID="fw_ajxLoadingPanel"
        EnableEmbeddedScripts="false" HorizontalAlign="NotSet">
        <telerik:RadGrid ID="grdList" runat="server" OnNeedDataSource="grdList_NeedDataSource"
            OnItemDataBound="grdList_ItemDataBound" OnItemCommand="grdList_ItemCommand"
            AutoGenerateColumns="false" ShowGroupPanel="false" GridLines="None">
            <MasterTableView DataKeyNames="AdmedikaClaimTransactionID" ClientDataKeyNames="AdmedikaClaimTransactionID">
            <Columns>
                <telerik:GridDateTimeColumn DataField="CreatedDateTime" HeaderText="Created Date Time"
                    UniqueName="CreatedDateTime" SortExpression="CreatedDateTime" DataType="System.DateTime"
                    DataFormatString="{0:MM/dd/yyyy HH:mm}" HeaderStyle-Width="140px" />
                <telerik:GridBoundColumn DataField="RegistrationNo" HeaderText="Registration No"
                    UniqueName="RegistrationNo" SortExpression="RegistrationNo" HeaderStyle-Width="150px"
                    ItemStyle-Font-Bold="true" />
                <telerik:GridTemplateColumn DataField="PatientID" HeaderText="PatientID"
                    UniqueName="PatientID" SortExpression="PatientID" HeaderStyle-Width="200px">
                    <ItemTemplate>
                        <%# BuildPatientIdHtml(Container.DataItem) %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="PatientName" HeaderText="Patient Name"
                    UniqueName="PatientName" SortExpression="PatientName" />
                <telerik:GridBoundColumn DataField="MedicalNo" HeaderText="Medical No"
                    UniqueName="MedicalNo" SortExpression="MedicalNo" HeaderStyle-Width="110px" />
                <telerik:GridBoundColumn DataField="CardNo" HeaderText="Card No"
                    UniqueName="CardNo" SortExpression="CardNo" HeaderStyle-Width="150px"
                    ItemStyle-Font-Bold="true" />
                <telerik:GridBoundColumn DataField="ClID" HeaderText="ClID (Claim ID)"
                    UniqueName="ClID" SortExpression="ClID" HeaderStyle-Width="110px"
                    ItemStyle-Font-Bold="true" />
                <telerik:GridBoundColumn DataField="CovID" HeaderText="CovID"
                    UniqueName="CovID" SortExpression="CovID" HeaderStyle-Width="70px" />
                <telerik:GridTemplateColumn DataField="CovCode" HeaderText="CovCode"
                    UniqueName="CovCode" SortExpression="CovCode" HeaderStyle-Width="150px">
                    <ItemTemplate>
                        <%# BuildCovCodeHtml(Container.DataItem) %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="ClStatus" HeaderText="ClStatus"
                    UniqueName="ClStatus" SortExpression="ClStatus" HeaderStyle-Width="70px" />
                <telerik:GridBoundColumn DataField="ClDesc" HeaderText="ClDesc"
                    UniqueName="ClDesc" SortExpression="ClDesc" />
                <telerik:GridCheckBoxColumn DataField="IsClosed" HeaderText="Is Closed"
                    UniqueName="IsClosed" SortExpression="IsClosed" HeaderStyle-Width="50px"
                    ItemStyle-HorizontalAlign="Center" />
                <telerik:GridTemplateColumn UniqueName="Action" HeaderText="Action"
                    HeaderStyle-Width="320px" HeaderStyle-HorizontalAlign="Center"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# BuildActionHtml(Container.DataItem) %>
                        <asp:LinkButton ID="lnkPrintLOA" runat="server"
                            CommandName="PrintLOA"
                            CommandArgument='<%# Eval("ClID") %>'
                            Text="LOA"
                            CausesValidation="false"
                            style="background: url(../../../../Images/Toolbar/print16.png) no-repeat left center; padding-left: 20px; text-decoration: none;" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
        </ClientSettings>
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
</asp:Content>
