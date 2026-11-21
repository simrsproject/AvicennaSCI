<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaTemplateSignFileList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaTemplateSignFileList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="layout">
        <tr>
            <td class="label">Dokter</td>
            <td><telerik:RadTextBox ID="txtParamedicID" runat="server" Width="150px" /></td>
            <td><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_OnClick" /></td>
        </tr>
        <tr>
            <td class="label">Upload template</td>
            <td>
                <telerik:RadAsyncUpload ID="uplTemplate" runat="server" ControlObjectsVisibility="None"
                    AllowedFileExtensions=".pdf" MaxFileSize="20971520" />
            </td>
            <td><asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_OnClick" /></td>
        </tr>
    </table>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="TemplateSignFileID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="TemplateNo" HeaderText="Template No" UniqueName="TemplateNo" />
                <telerik:GridBoundColumn DataField="FileName" HeaderText="File" UniqueName="FileName" />
                <telerik:GridBoundColumn DataField="IsDefault" HeaderText="Default" UniqueName="IsDefault" />
                <telerik:GridBoundColumn DataField="IsRegistered" HeaderText="Terdaftar" UniqueName="IsRegistered" />
                <telerik:GridBoundColumn DataField="RegisteredDateTime" HeaderText="Tanggal Daftar"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" UniqueName="RegisteredDateTime" />
                <telerik:GridBoundColumn DataField="LastErrorMessage" HeaderText="Error" UniqueName="LastErrorMessage" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnRegister" runat="server" Text="Daftarkan"
                            CommandName="register" CommandArgument='<%# Eval("TemplateSignFileID") %>' />
                        <asp:LinkButton ID="btnDefault" runat="server" Text="Set Default"
                            CommandName="default" CommandArgument='<%# Eval("TemplateSignFileID") %>' />
                        <asp:LinkButton ID="btnDeactivate" runat="server" Text="Nonaktifkan"
                            CommandName="deactivate" CommandArgument='<%# Eval("TemplateSignFileID") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
