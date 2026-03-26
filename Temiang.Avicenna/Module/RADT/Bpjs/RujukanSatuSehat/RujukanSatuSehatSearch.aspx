<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master" AutoEventWireup="true" CodeBehind="RujukanSatuSehatSearch.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Bpjs.RujukanSatuSehatSearch" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td class="label">
                <asp:Label ID="lblSEPNo" runat="server" Text="No SEP" Width="100px"></asp:Label>
            </td>
            <td class="filter" />
            <td class="entry">
                <telerik:RadTextBox ID="txtNoSep" runat="server" Width="300px" />
            </td>
            <td width="20px" />
            <td />
        </tr>
        <tr>
            <td class="label">
                No Rujukan
            </td>
            <td class="filter" />
            <td class="entry">
                <telerik:RadTextBox ID="txtNoRujukan" runat="server" Width="300px" />
            </td>
            <td width="20px" />
            <td />
        </tr>
        <tr>
            <td class="label">
                Tanggal Rujukan
            </td>
            <td class="filter" />
            <td class="entry">
                <telerik:RadDatePicker ID="txtTanggalRujukan" runat="server" Width="100px" />
            </td>
            <td width="20px" />
            <td />
        </tr>
    </table>
</asp:Content>
