<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="AdmedikaDischargeList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaDischargeList" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="cc" Namespace="Temiang.Avicenna.CustomControl" Assembly="Temiang.Avicenna" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function openDischarge(id) {
                var oWnd = $find("<%= winDischarge.ClientID %>");
                oWnd.argument = new Object();
                oWnd.setUrl("AdmedikaDischargeDialog.aspx?id=" + encodeURIComponent(id));
                oWnd.show();
            }

            function onDischargeClose(oWnd, args) {
                if (oWnd.argument && oWnd.argument.mode === "rebind") {
                    __doPostBack("<%= grdList.UniqueID %>", "rebind");
                    oWnd.argument.mode = null;
                }
            }

            function printLOC(id) {
                window.open('AdmedikaPrintOut.aspx?id=' + encodeURIComponent(id) + '&type=D', '_blank',
                    'width=760,height=700,scrollbars=yes,resizable=yes');
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindow ID="winDischarge" runat="server" Animation="None"
        Behavior="Close,Move,Resize,Maximize" ShowContentDuringLoad="false"
        Width="1200px" Height="720px" VisibleStatusbar="false" Modal="true"
        OnClientClose="onDischargeClose" />

    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnFilterDate">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterPatient">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterRegistration">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterClaim">
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

    <asp:Panel ID="pnlUnavailable" runat="server" Visible="false"
        Style="margin:12px 0;padding:12px;border:1px solid #f0ad4e;background:#fcf8e3;color:#8a6d3b;">
        Discharge AdMedika rawat inap belum tersedia pada halaman ini.
    </asp:Panel>

    <cc:CollapsePanel ID="CollapsePanel1" runat="server" Title="Search Filter">
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                <td class="label">Registration Date</td>
                <td class="entry">
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td><telerik:RadDatePicker ID="txtDateFrom" runat="server" Width="100px" /></td>
                            <td>&nbsp;-&nbsp;</td>
                            <td><telerik:RadDatePicker ID="txtDateTo" runat="server" Width="100px" /></td>
                        </tr>
                    </table>
                </td>
                <td style="text-align: left"><asp:ImageButton ID="btnFilterDate" runat="server" ImageUrl="~/Images/Toolbar/search16.png" OnClick="btnFilter_Click" ToolTip="Search" /></td>
            </tr>
            <tr>
                <td class="label">Patient / Medical No</td>
                <td class="entry"><telerik:RadTextBox ID="txtPatient" runat="server" Width="300px" /></td>
                <td style="text-align: left"><asp:ImageButton ID="btnFilterPatient" runat="server" ImageUrl="~/Images/Toolbar/search16.png" OnClick="btnFilter_Click" ToolTip="Search" /></td>
            </tr>
            <tr>
                <td class="label">Claim Status</td>
                <td class="entry"><telerik:RadComboBox ID="cboClaimStatus" runat="server" Width="300px"
                    AllowCustomText="true" Filter="Contains" /></td>
                <td style="text-align: left"><asp:ImageButton ID="btnFilterStatus" runat="server" ImageUrl="~/Images/Toolbar/search16.png" OnClick="btnFilter_Click" ToolTip="Search" /></td>
            </tr>
                    </table>
                </td>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                <td class="label">Registration No</td>
                <td class="entry"><telerik:RadTextBox ID="txtRegistrationNo" runat="server" Width="300px" /></td>
                <td style="text-align: left"><asp:ImageButton ID="btnFilterRegistration" runat="server" ImageUrl="~/Images/Toolbar/search16.png" OnClick="btnFilter_Click" ToolTip="Search" /></td>
            </tr>
            <tr>
                <td class="label">Card / Claim ID</td>
                <td class="entry"><telerik:RadTextBox ID="txtClaim" runat="server" Width="300px" /></td>
                <td style="text-align: left"><asp:ImageButton ID="btnFilterClaim" runat="server" ImageUrl="~/Images/Toolbar/search16.png" OnClick="btnFilter_Click" ToolTip="Search" /></td>
            </tr>
                    </table>
                </td>
            </tr>
        </table>
    </cc:CollapsePanel>

    <telerik:RadGrid ID="grdList" runat="server" AutoGenerateColumns="false" GridLines="None"
        OnNeedDataSource="grdList_NeedDataSource" AllowPaging="true" PageSize="20">
        <MasterTableView DataKeyNames="AdmedikaClaimTransactionID">
            <Columns>
                <telerik:GridDateTimeColumn DataField="RegistrationDate" HeaderText="Reg. Date"
                    DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Width="125px" />
                <telerik:GridBoundColumn DataField="RegistrationNo" HeaderText="Registration No"
                    HeaderStyle-Width="140px" ItemStyle-Font-Bold="true" />
                <telerik:GridBoundColumn DataField="MedicalNo" HeaderText="Medical No" HeaderStyle-Width="100px" />
                <telerik:GridBoundColumn DataField="PatientName" HeaderText="Patient Name" />
                <telerik:GridBoundColumn DataField="GuarantorName" HeaderText="Guarantor" />
                <telerik:GridBoundColumn DataField="CardNo" HeaderText="Card No" HeaderStyle-Width="135px" />
                <telerik:GridBoundColumn DataField="ClID" HeaderText="Claim ID" HeaderStyle-Width="100px" />
                <telerik:GridTemplateColumn HeaderText="Claim Status" HeaderStyle-Width="160px">
                    <ItemTemplate><%# BuildStatusHtml(Container.DataItem) %></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridNumericColumn DataField="TotAmtApproved" HeaderText="Approved"
                    DataFormatString="{0:n2}" HeaderStyle-Width="105px" ItemStyle-HorizontalAlign="Right" />
                <telerik:GridNumericColumn DataField="TotAmtNotApproved" HeaderText="Patient Excess"
                    DataFormatString="{0:n2}" HeaderStyle-Width="105px" ItemStyle-HorizontalAlign="Right" />
                <telerik:GridTemplateColumn HeaderText="Action" HeaderStyle-Width="165px"
                    HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate><%# BuildActionHtml(Container.DataItem) %></ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings EnableRowHoverStyle="true" />
    </telerik:RadGrid>
</asp:Content>
