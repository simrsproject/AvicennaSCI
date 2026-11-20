<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaRegistrationList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaRegistrationList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="layout">
        <tr>
            <td class="label">Dokter</td>
            <td><telerik:RadTextBox ID="txtParamedicName" runat="server" Width="250px" /></td>
            <td class="label">Status</td>
            <td>
                <telerik:RadComboBox ID="cboState" runat="server" Width="200px" AllowCustomText="false">
                    <Items>
                        <telerik:RadComboBoxItem Text="(Semua)" Value="" />
                        <telerik:RadComboBoxItem Text="Belum terdaftar" Value="BelumTerdaftar" />
                        <telerik:RadComboBoxItem Text="Menunggu KYC" Value="MenungguKyc" />
                        <telerik:RadComboBoxItem Text="KYC gagal" Value="KycGagal" />
                        <telerik:RadComboBoxItem Text="Menunggu verifikasi" Value="MenungguVerifikasi" />
                        <telerik:RadComboBoxItem Text="Sertifikat terbit" Value="SertifikatTerbit" />
                        <telerik:RadComboBoxItem Text="Aktif" Value="Aktif" />
                        <telerik:RadComboBoxItem Text="Ditolak" Value="Ditolak" />
                        <telerik:RadComboBoxItem Text="Kedaluwarsa" Value="Kedaluwarsa" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_OnClick" /></td>
        </tr>
    </table>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="ParamedicID" ClientDataKeyNames="ParamedicID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="ParamedicID" HeaderText="ID" UniqueName="ParamedicID" />
                <telerik:GridBoundColumn DataField="ParamedicName" HeaderText="Dokter" UniqueName="ParamedicName" />
                <telerik:GridBoundColumn DataField="NamaKTP" HeaderText="Nama KTP" UniqueName="NamaKTP" />
                <telerik:GridBoundColumn DataField="StatusText" HeaderText="Status" UniqueName="StatusText" />
                <telerik:GridBoundColumn DataField="TilakaName" HeaderText="Tilaka Name" UniqueName="TilakaName" />
                <telerik:GridBoundColumn DataField="LastCheckedDateTime" HeaderText="Cek Terakhir"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" UniqueName="LastCheckedDateTime" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDetail" runat="server" Text="Detail"
                            CommandName="detail" CommandArgument='<%# Eval("ParamedicID") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <asp:Button ID="btnReconcile" runat="server" Text="Cek Status Massal" OnClick="btnReconcile_OnClick" />
    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
