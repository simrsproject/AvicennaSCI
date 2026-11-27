<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master" AutoEventWireup="true"
    CodeBehind="TilakaRevokeDialog.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaRevokeDialog" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function openRevokeWindow() {
            window.open('', 'tilakaRevokeWindow');
            return true;
        }
    </script>
    <table width="100%">
        <tr>
            <td class="label">Dokter</td>
            <td class="entry"><asp:Label ID="lblParamedicName" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Tilaka Name</td>
            <td class="entry"><asp:Label ID="lblTilakaName" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">No. Sertifikat</td>
            <td class="entry"><asp:Label ID="lblCertificateSerialNumber" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Alasan Revoke</td>
            <td class="entry">
                <telerik:RadComboBox ID="cboReason" runat="server" Width="300px" />
            </td>
        </tr>
        <tr>
            <td class="label">Catatan</td>
            <td class="entry">
                <telerik:RadTextBox ID="txtNote" runat="server" Width="300px"
                    TextMode="MultiLine" Rows="4" MaxLength="500" />
            </td>
        </tr>
        <tr>
            <td></td>
            <td class="entry">
                <asp:CheckBox ID="chkConfirm" runat="server"
                    Text="Saya memahami sertifikat akan dicabut dan tidak dapat digunakan lagi." />
            </td>
        </tr>
    </table>
</asp:Content>
