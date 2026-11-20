<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true"
    CodeBehind="TilakaRegistrationDetail.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaRegistrationDetail" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function openWebview() {
                var url = document.getElementById('<%= hfWebviewUrl.ClientID %>').value;
                if (url) window.open(url, '_blank');
                return false;
            }
        </script>
    </telerik:RadCodeBlock>

    <asp:HiddenField ID="hfWebviewUrl" runat="server" />
    <asp:HiddenField ID="hfParamedicID" runat="server" />

    <table width="100%">
        <tr>
            <td class="label">Dokter</td>
            <td class="entry"><asp:Label ID="lblParamedicName" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Nama sesuai KTP</td>
            <td class="entry"><asp:Label ID="lblNamaKTP" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">NIK (SSN)</td>
            <td class="entry"><asp:Label ID="lblSsn" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Email</td>
            <td class="entry"><asp:Label ID="lblEmail" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Status</td>
            <td class="entry"><asp:Label ID="lblStatus" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Tilaka Name</td>
            <td class="entry"><asp:Label ID="lblTilakaName" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Foto KTP</td>
            <td class="entry">
                <telerik:RadAsyncUpload ID="uplKtp" runat="server" ControlObjectsVisibility="None"
                    AllowedFileExtensions=".jpg,.jpeg,.png" MaxFileSize="2097152" />
                <asp:Image ID="imgKtp" runat="server" Width="320" Visible="false" />
            </td>
        </tr>
        <tr>
            <td class="label">Persetujuan TnC</td>
            <td class="entry">
                <asp:CheckBox ID="chkConsent" runat="server"
                    Text="Dokter telah membaca dan menyetujui TnC Tilaka." />
                <asp:HyperLink ID="lnkConsent" runat="server" Target="_blank" Text="Lihat TnC" />
            </td>
        </tr>
    </table>

    <br />
    <asp:Button ID="btnDaftar" runat="server" Text="Daftar" OnClick="btnDaftar_OnClick" />
    <asp:Button ID="btnKirimData" runat="server" Text="Kirim Data KYC" OnClick="btnKirimData_OnClick" />
    <asp:Button ID="btnWebview" runat="server" Text="Buka Webview" OnClientClick="return openWebview();" />
    <asp:Button ID="btnCekStatus" runat="server" Text="Cek Status" OnClick="btnCekStatus_OnClick" />
    <br />
    <br />
    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
