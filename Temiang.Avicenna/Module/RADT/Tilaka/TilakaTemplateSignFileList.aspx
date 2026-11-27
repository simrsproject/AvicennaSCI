<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaTemplateSignFileList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaTemplateSignFileList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="cc" Namespace="Temiang.Avicenna.CustomControl" Assembly="Temiang.Avicenna" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
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
                            <td class="label">Dokter (ID/Nama)</td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtParamedic" runat="server" Width="300px" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterParamedic" runat="server"
                                    ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="50%" style="vertical-align: top"></td>
            </tr>
        </table>
    </cc:CollapsePanel>

    <div style="text-align: right;">
        <asp:Literal ID="lblMessage" runat="server" EnableViewState="false" />
    </div>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="TemplateSignFileID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="TemplateNo" HeaderText="Template No" UniqueName="TemplateNo" />
                <telerik:GridBoundColumn DataField="ParamedicID" HeaderText="ID Dokter" UniqueName="ParamedicID" />
                <telerik:GridBoundColumn DataField="ParamedicName" HeaderText="Nama Dokter" UniqueName="ParamedicName" />
                <telerik:GridBoundColumn DataField="FileName" HeaderText="File" UniqueName="FileName" />
                <telerik:GridCheckBoxColumn DataField="IsDefault" HeaderText="Default" UniqueName="IsDefault" />
                <telerik:GridCheckBoxColumn DataField="IsActive" HeaderText="Active" UniqueName="IsActive" />
                <telerik:GridCheckBoxColumn DataField="IsRegistered" HeaderText="Terdaftar" UniqueName="IsRegistered" />
                <telerik:GridBoundColumn DataField="RegisteredDateTime" HeaderText="Tanggal Daftar"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" UniqueName="RegisteredDateTime" />
                <telerik:GridBoundColumn DataField="LastErrorMessage" HeaderText="Error" UniqueName="LastErrorMessage" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnRegister" runat="server" Text="Register"
                            CommandName="register" CommandArgument='<%# Eval("TemplateSignFileID") %>'
                            Visible='<%# Eval("CanRegister") %>' />
                        <asp:ImageButton ID="btnDelete" runat="server"
                            ImageUrl="~/Images/Toolbar/delete16.png"
                            CommandName="delete"
                            CommandArgument='<%# Eval("TemplateSignFileID") %>'
                            Visible='<%# Eval("CanDelete") %>'
                            OnClientClick="return confirm('Hapus template ini? Data yang dihapus tidak dapat dikembalikan.');"
                            ToolTip="Delete"
                            Style="margin-left: 20px; vertical-align: middle;" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
