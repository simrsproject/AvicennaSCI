<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemImmunizationDetail.ascx.cs"
    Inherits="Temiang.Avicenna.Module.Inventory.Master.ItemImmunizationDetail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:ValidationSummary ID="vsumItemBalance" runat="server" ValidationGroup="ItemMedic" />
<asp:CustomValidator ID="customValidator" runat="server" ValidationGroup="ItemMedic"
    ErrorMessage="" OnServerValidate="customValidator_ServerValidate">&nbsp;</asp:CustomValidator>
<table width="100%">
    <tr>
        <td class="label">
            <asp:Label ID="lblImmunizationID" runat="server" Text="Immunization" />
        </td>
        <td class="entry">
            <telerik:RadComboBox runat="server" ID="cboImmunizationID" Width="300px" EnableLoadOnDemand="true"
                HighlightTemplatedItems="true" AutoPostBack="true" OnItemDataBound="cboImmunizationID_ItemDataBound"
                OnItemsRequested="cboImmunizationID_ItemsRequested" />
        </td>
        <td width="20px">
            <asp:RequiredFieldValidator ID="rfvImmunizationID" runat="server" ErrorMessage="Immunization required."
                ValidationGroup="ItemMedic" ControlToValidate="cboImmunizationID" SetFocusOnError="True"
                Width="100%">
                <asp:Image ID="Image6" runat="server" SkinID="rfvImage" />
            </asp:RequiredFieldValidator>
        </td>
        <td />
    </tr>
    <tr>
        <td align="right" colspan="2" style="height: 26px">
            <asp:Button ID="btnUpdate" Text="Update" runat="server" CommandName="Update" ValidationGroup="ItemMedic"
                Visible='<%# !(DataItem is GridInsertionObject) %>' />
            <asp:Button ID="btnInsert" Text="Insert" runat="server" CommandName="PerformInsert"
                ValidationGroup="ItemMedic" Visible='<%# DataItem is GridInsertionObject %>' />
            &nbsp;
            <asp:Button ID="btnCancel" Text="Cancel" runat="server" CausesValidation="False"
                CommandName="Cancel" />
        </td>
    </tr>
</table>
