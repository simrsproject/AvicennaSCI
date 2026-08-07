<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvoiceNoCtl.ascx.cs" 
    Inherits="Temiang.Avicenna.Module.Reports.OptionControl.InvoiceNoCtl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<table style="width: 90%">
    <tr>
        <td style="width: 5px">
        </td>
        <td style="width: 100px">
            <asp:Label ID="lblCaption" runat="server" Text="Invoice No" />
        </td>
        <td>
            <telerik:RadTextBox runat="server" ID="txtInvoiceNo" Width="250px">
            </telerik:RadTextBox>
        </td>
    </tr>
</table>
