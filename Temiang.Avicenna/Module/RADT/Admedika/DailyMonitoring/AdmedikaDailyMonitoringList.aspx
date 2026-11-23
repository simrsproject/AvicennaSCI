<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="AdmedikaDailyMonitoringList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaDailyMonitoringList" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="cc" Namespace="Temiang.Avicenna.CustomControl" Assembly="Temiang.Avicenna" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnFilterRegistrationDate">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterRegistration">
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
            <telerik:AjaxSetting AjaxControlID="btnFilterPatient">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterFlag">
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
                                Registration Date
                            </td>
                            <td class="entry">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <telerik:RadDatePicker ID="txtRegistrationDate" runat="server" Width="100px" />
                                        </td>
                                        <td>
                                        &nbsp;-&nbsp;
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="txtRegistrationDateTo" runat="server" Width="100px" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterRegistrationDate" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
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
                    </table>
                </td>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
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
                        <tr>
                            <td class="label">
                                Claim ID
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtClID" runat="server" Width="300px" MaxLength="20" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterClID" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Filter
                            </td>
                            <td class="entry">
                                <asp:CheckBox ID="chkNotMonitored" runat="server" Text="Belum pernah monitoring" />
                                <br />
                                <asp:CheckBox ID="chkIncludeClosed" runat="server" Text="Termasuk klaim closed" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterFlag" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </cc:CollapsePanel>
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" Width="100%" LoadingPanelID="fw_ajxLoadingPanel"
        EnableEmbeddedScripts="false" HorizontalAlign="NotSet">
        <telerik:RadGrid ID="grdList" runat="server" OnNeedDataSource="grdList_NeedDataSource"
            OnItemDataBound="grdList_ItemDataBound"
            AutoGenerateColumns="false" ShowGroupPanel="false" GridLines="None">
            <MasterTableView DataKeyNames="AdmedikaClaimTransactionID" ClientDataKeyNames="AdmedikaClaimTransactionID">
                <Columns>
                    <telerik:GridBoundColumn DataField="RegistrationNo" HeaderText="Registration No"
                        UniqueName="RegistrationNo" SortExpression="RegistrationNo" HeaderStyle-Width="150px"
                        ItemStyle-Font-Bold="true" />
                    <telerik:GridTemplateColumn DataField="RegistrationDate" HeaderText="Registration Date"
                        UniqueName="RegistrationDate" SortExpression="RegistrationDate" HeaderStyle-Width="130px">
                        <ItemTemplate>
                            <%# BuildRegistrationDateHtml(Container.DataItem) %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="CardNo" HeaderText="Card No"
                        UniqueName="CardNo" SortExpression="CardNo" HeaderStyle-Width="150px" />
                    <telerik:GridBoundColumn DataField="ClID" HeaderText="Claim ID"
                        UniqueName="ClID" SortExpression="ClID" HeaderStyle-Width="110px"
                        ItemStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="PatientName" HeaderText="Patient Name"
                        UniqueName="PatientName" SortExpression="PatientName" />
                    <telerik:GridBoundColumn DataField="RoomName" HeaderText="Room"
                        UniqueName="RoomName" SortExpression="RoomName" HeaderStyle-Width="150px" />
                    <telerik:GridTemplateColumn HeaderText="LOS" UniqueName="LOS"
                        HeaderStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# BuildLosHtml(Container.DataItem) %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Status Pasien" UniqueName="StatusPasien"
                        HeaderStyle-Width="110px">
                        <ItemTemplate>
                            <%# BuildPatientStatusHtml(Container.DataItem) %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="LastDiagnosis" HeaderText="Diagnosa Terakhir"
                        UniqueName="LastDiagnosis" SortExpression="LastDiagnosis" />
                    <telerik:GridTemplateColumn DataField="LastMonitoringDate" HeaderText="Last Monitoring"
                        UniqueName="LastMonitoringDate" SortExpression="LastMonitoringDate" HeaderStyle-Width="120px">
                        <ItemTemplate>
                            <%# BuildLastMonitoringHtml(Container.DataItem) %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Action" HeaderText="Action"
                        HeaderStyle-Width="100px" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <a href='AdmedikaDailyMonitoringDetail.aspx?id=<%# Eval("AdmedikaClaimTransactionID") %>'>Monitoring</a>
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
