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

            function openChangeMfa() {
                var url = document.getElementById('<%= hfChangeMfaUrl.ClientID %>').value;
                if (url) window.open(url, '_blank');
                return false;
            }

            function openConsent() {
                var panel = document.getElementById('<%= pnlConsent.ClientID %>');
                if (panel) panel.style.display = 'block';

                var oWnd = $find("<%= winConsent.ClientID %>");
                if (oWnd) oWnd.show();
                return false;
            }
        </script>
    </telerik:RadCodeBlock>

    <telerik:RadWindow ID="winConsent" runat="server" Animation="None"
        Behavior="Close,Move,Resize" Width="800px" Height="600px"
        Modal="true" VisibleStatusbar="False" Title="Persetujuan Tilaka"
        NavigateUrl="TilakaConsentHandler.ashx" />

    <asp:HiddenField ID="hfWebviewUrl" runat="server" />
    <asp:HiddenField ID="hfParamedicID" runat="server" />
    <asp:HiddenField ID="hfChangeMfaUrl" runat="server" />

    <table width="100%">
        <tr>
            <td class="label">Dokter</td>
            <td class="entry"><asp:Label ID="lblParamedicName" runat="server" /></td>
        <td width="20"></td><td></td></tr>
        <tr>
            <td class="label">Nama sesuai KTP</td>
            <td class="entry"><asp:Label ID="lblNamaKTP" runat="server" /></td>
        <td width="20"></td><td></td></tr>
        <tr>
            <td class="label">NIK (SSN)</td>
            <td class="entry"><asp:Label ID="lblSsn" runat="server" /></td>
        <td width="20"></td><td></td></tr>
        <tr>
            <td class="label">Email</td>
            <td class="entry"><asp:Label ID="lblEmail" runat="server" /></td>
        <td width="20"></td><td></td></tr>
        <tr>
            <td class="label">Status</td>
            <td class="entry"><asp:Label ID="lblStatus" runat="server" /></td>
        <td width="20"></td><td></td></tr>
        <tr>
            <td class="label">Tilaka Name</td>
            <td class="entry"><asp:Label ID="lblTilakaName" runat="server" /></td>
        <td width="20"></td><td></td></tr>
        <tr>
            <td class="label">Foto KTP</td>
            <td class="entry">
                <telerik:RadAsyncUpload ID="uplKtp" runat="server" ControlObjectsVisibility="None"
                    AllowedFileExtensions=".jpg,.jpeg,.png" MaxFileSize="2097152" />
                <span style="margin-left: 8px; color: #666; font-size: 11px;">
                    Maks. upload 2 MB (JPG/PNG)
                </span>
                <asp:Image ID="imgKtp" runat="server" Width="320" Visible="false" />
            </td>
        <td width="20"></td><td></td></tr>
        <tr>
            <td class="label">Persetujuan TnC</td>
            <td class="entry">
                <asp:Panel ID="pnlConsent" runat="server" Style="display: none;">
                    <asp:CheckBox ID="chkConsent" runat="server"
                        Text="Dokter telah membaca dan menyetujui TnC Tilaka." />
                </asp:Panel>
                &nbsp;
                <asp:LinkButton ID="btnReadConsent" runat="server" Text="Baca Persetujuan"
                    OnClientClick="return openConsent();" CausesValidation="false"
                    Style="text-decoration: underline; color: #0b5cab; cursor: pointer;" />
            </td>
        <td width="20"></td><td></td></tr>
    </table>

    <br />
    <div style="margin-left: 10px;">
        <asp:Button ID="btnDaftar" runat="server" Text="Daftar" OnClick="btnDaftar_OnClick" />
        <asp:Button ID="btnReEnroll" runat="server" Text="Re-enroll Sertifikat" OnClick="btnReEnroll_OnClick" />
        <asp:Button ID="btnKirimData" runat="server" Text="Kirim Data KYC" OnClick="btnKirimData_OnClick" />
        <asp:Button ID="btnWebview" runat="server" Text="Buka Webview" OnClientClick="return openWebview();" />
        <asp:Button ID="btnCekStatus" runat="server" Text="Cek Status" OnClick="btnCekStatus_OnClick" />
        <asp:Button ID="btnChangeMfa" runat="server" Text="Ganti MFA"
            OnClientClick="return openChangeMfa();" />
    </div>
    <br />
    <br />
    <asp:Literal ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
