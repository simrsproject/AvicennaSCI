<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="AdmedikaEdocsList.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaEdocsList" %>

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
            <telerik:AjaxSetting AjaxControlID="btnFilterClID">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterCard">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterPatient">
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
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" Width="100%" LoadingPanelID="fw_ajxLoadingPanel"
        EnableEmbeddedScripts="false" HorizontalAlign="NotSet">
        <telerik:RadGrid ID="grdList" runat="server" OnNeedDataSource="grdList_NeedDataSource"
            OnDetailTableDataBind="grdList_DetailTableDataBind"
            AutoGenerateColumns="false" ShowGroupPanel="false" GridLines="None">
            <MasterTableView DataKeyNames="AdmedikaClaimTransactionID" ClientDataKeyNames="AdmedikaClaimTransactionID">
                <Columns>
                    <telerik:GridBoundColumn DataField="RegistrationNo" HeaderText="Registration No"
                        UniqueName="RegistrationNo" SortExpression="RegistrationNo" HeaderStyle-Width="150px"
                        ItemStyle-Font-Bold="true" />
                    <telerik:GridDateTimeColumn DataField="RegistrationDate" HeaderText="Registration Date"
                        UniqueName="RegistrationDate" SortExpression="RegistrationDate" DataType="System.DateTime"
                        DataFormatString="{0:MM/dd/yyyy}" HeaderStyle-Width="120px" />
                    <telerik:GridBoundColumn DataField="CardNo" HeaderText="Card No"
                        UniqueName="CardNo" SortExpression="CardNo" HeaderStyle-Width="150px"
                        ItemStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="ClID" HeaderText="Claim ID"
                        UniqueName="ClID" SortExpression="ClID" HeaderStyle-Width="110px"
                        ItemStyle-Font-Bold="true" />
                    <telerik:GridBoundColumn DataField="PatientName" HeaderText="Patient Name"
                        UniqueName="PatientName" SortExpression="PatientName" />
                    <telerik:GridBoundColumn DataField="JumlahFile" HeaderText="Jumlah File"
                        UniqueName="JumlahFile" SortExpression="JumlahFile" HeaderStyle-Width="90px"
                        HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
                </Columns>
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="AdmedikaDocumentID" Name="grdDocuments"
                        AutoGenerateColumns="False" AllowPaging="true" PageSize="10">
                        <Columns>
                            <telerik:GridDateTimeColumn DataField="CreatedDateTime" HeaderText="Created Date"
                                UniqueName="CreatedDateTime" SortExpression="CreatedDateTime" DataType="System.DateTime"
                                DataFormatString="{0:MM/dd/yyyy HH:mm}" HeaderStyle-Width="130px" />
                            <telerik:GridTemplateColumn DataField="DocType" HeaderText="Kode &amp; Jenis File"
                                UniqueName="DocType" SortExpression="DocType" HeaderStyle-Width="250px">
                                <ItemTemplate>
                                    <%# BuildDocTypeHtml(Container.DataItem) %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="FileName" HeaderText="File Name"
                                UniqueName="FileName" SortExpression="FileName" />
                            <telerik:GridBoundColumn DataField="CreatedByUserID" HeaderText="Upload By"
                                UniqueName="CreatedByUserID" SortExpression="CreatedByUserID" HeaderStyle-Width="110px" />
                            <telerik:GridDateTimeColumn DataField="SentDateTime" HeaderText="Sent Time"
                                UniqueName="SentDateTime" SortExpression="SentDateTime" DataType="System.DateTime"
                                DataFormatString="{0:MM/dd/yyyy HH:mm}" HeaderStyle-Width="130px" />
                            <telerik:GridBoundColumn DataField="SentByUserID" HeaderText="Sent By"
                                UniqueName="SentByUserID" SortExpression="SentByUserID" HeaderStyle-Width="110px" />
                            <telerik:GridBoundColumn DataField="ResponseMessage" HeaderText="Response Message"
                                UniqueName="ResponseMessage" SortExpression="ResponseMessage" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
            </MasterTableView>
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="True" />
            </ClientSettings>
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
</asp:Content>
