<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaQuickSignUserList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaQuickSignUserList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="layout">
        <tr>
            <td class="label">Dokter</td>
            <td><telerik:RadTextBox ID="txtParamedicName" runat="server" Width="250px" /></td>
            <td class="label">Status</td>
            <td>
                <telerik:RadComboBox ID="cboStatus" runat="server" Width="180px" AllowCustomText="false">
                    <Items>
                        <telerik:RadComboBoxItem Text="(Semua)" Value="" />
                        <telerik:RadComboBoxItem Text="Belum terdaftar" Value="0" />
                        <telerik:RadComboBoxItem Text="Terdaftar" Value="1" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_OnClick" /></td>
        </tr>
    </table>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="ParamedicID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="ParamedicID" HeaderText="ID" UniqueName="ParamedicID" />
                <telerik:GridBoundColumn DataField="ParamedicName" HeaderText="Dokter" UniqueName="ParamedicName" />
                <telerik:GridBoundColumn DataField="Email" HeaderText="Email" UniqueName="Email" />
                <telerik:GridBoundColumn DataField="TilakaName" HeaderText="Tilaka Name" UniqueName="TilakaName" />
                <telerik:GridBoundColumn DataField="CertificateStatus" HeaderText="Sertifikat" UniqueName="CertificateStatus" />
                <telerik:GridBoundColumn DataField="QuickSignStatus" HeaderText="Quick Sign" UniqueName="QuickSignStatus" />
                <telerik:GridBoundColumn DataField="QuickSignRegisteredDateTime" HeaderText="Terdaftar"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" UniqueName="QuickSignRegisteredDateTime" />
                <telerik:GridBoundColumn DataField="QuickSignLastErrorMessage" HeaderText="Error" UniqueName="QuickSignLastErrorMessage" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnRegister" runat="server" Text="Daftarkan Quick Sign"
                            CommandName="register" CommandArgument='<%# Eval("ParamedicID") %>'
                            Visible='<%# Eval("CanRegister") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
