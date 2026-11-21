<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaAuthIntervalList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaAuthIntervalList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="layout">
        <tr>
            <td class="label">Email</td>
            <td><telerik:RadTextBox ID="txtEmail" runat="server" Width="250px" /></td>
            <td class="label">Status</td>
            <td>
                <telerik:RadComboBox ID="cboAuthStatus" runat="server" Width="160px" AllowCustomText="false">
                    <Items>
                        <telerik:RadComboBoxItem Text="(Semua)" Value="" />
                        <telerik:RadComboBoxItem Text="active" Value="active" />
                        <telerik:RadComboBoxItem Text="expired" Value="expired" />
                        <telerik:RadComboBoxItem Text="not_activated" Value="not_activated" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td><asp:Button ID="btnSearch" runat="server" Text="Refresh" OnClick="btnSearch_OnClick" /></td>
        </tr>
    </table>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="False"
        OnNeedDataSource="grdList_NeedDataSource">
        <MasterTableView AutoGenerateColumns="false" DataKeyNames="Email">
            <Columns>
                <telerik:GridBoundColumn DataField="Email" HeaderText="Email" UniqueName="Email" />
                <telerik:GridBoundColumn DataField="TilakaName" HeaderText="Tilaka Name" UniqueName="TilakaName" />
                <telerik:GridBoundColumn DataField="RegisteredDate" HeaderText="Terdaftar" UniqueName="RegisteredDate" />
                <telerik:GridBoundColumn DataField="AuthIntervalDuration" HeaderText="Durasi (hari)" UniqueName="AuthIntervalDuration" />
                <telerik:GridBoundColumn DataField="AuthActivatedDate" HeaderText="Aktivasi" UniqueName="AuthActivatedDate" />
                <telerik:GridBoundColumn DataField="AuthExpiredDate" HeaderText="Kedaluwarsa" UniqueName="AuthExpiredDate" />
                <telerik:GridBoundColumn DataField="AuthStatus" HeaderText="Status" UniqueName="AuthStatus" />
                <telerik:GridBoundColumn DataField="TemplateNumbers" HeaderText="Template" UniqueName="TemplateNumbers" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
