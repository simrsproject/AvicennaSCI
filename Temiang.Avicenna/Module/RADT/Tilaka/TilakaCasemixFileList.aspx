<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaCasemixFileList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaCasemixFileList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="cc" Namespace="Temiang.Avicenna.CustomControl" Assembly="Temiang.Avicenna" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnFilterRegistrationNo">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterParamedic">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                    <telerik:AjaxUpdatedControl ControlID="lblMessage" />
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
                            <td class="label">Registration No</td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtRegistrationNo" runat="server" Width="300px" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterRegistrationNo" runat="server"
                                    ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                            <td class="label">Dokter (ID/Nama)</td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtParamedicID" runat="server" Width="300px" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterParamedic" runat="server"
                                    ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </cc:CollapsePanel>

    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="CasemixFileID" ClientDataKeyNames="CasemixFileID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="RegistrationNo" HeaderText="Registration No" UniqueName="RegistrationNo" />
                <telerik:GridBoundColumn DataField="ParamedicID" HeaderText="ID Dokter" UniqueName="ParamedicID" />
                <telerik:GridBoundColumn DataField="ParamedicName" HeaderText="Nama Dokter" UniqueName="ParamedicName" />
                <telerik:GridBoundColumn DataField="FileName" HeaderText="File Name" UniqueName="FileName" />
                <telerik:GridBoundColumn DataField="FilePath" HeaderText="File Path" UniqueName="FilePath" />
                <telerik:GridBoundColumn DataField="DownloadPath" HeaderText="Download Path" UniqueName="DownloadPath" />
                <telerik:GridBoundColumn DataField="RequestStatus" HeaderText="Status" UniqueName="RequestStatus" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnProcess" runat="server" Text="Process Signature"
                            CommandName="process" CommandArgument='<%# Eval("CasemixFileID") %>'
                            Enabled='<%# Eval("CanProcess") %>'
                            OnClientClick="return confirm('Proses tanda tangan untuk file ini? Pastikan dokter dan template sudah benar.');"
                            ToolTip='<%# Convert.ToBoolean(Eval("CanProcess")) ? "" : "Dokter tidak menggunakan tanda tangan elektronik atau request masih berjalan." %>' />
                        <asp:LinkButton ID="btnStatus" runat="server" Text="Status"
                            CommandName="status" CommandArgument='<%# Eval("RequestID") %>'
                            Visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("RequestID"))) %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
