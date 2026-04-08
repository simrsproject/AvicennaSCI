<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemConsumptionPackageEntry.ascx.cs"
    Inherits="Temiang.Avicenna.Module.Charges.ItemConsumptionPackageEntry" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:ValidationSummary ID="vsumTransChargesItemConsumption" runat="server" ValidationGroup="TransChargesItemConsumption" />
<asp:CustomValidator ID="customValidator" runat="server" ValidationGroup="TransChargesItemConsumption"
    ErrorMessage="" OnServerValidate="customValidator_ServerValidate">&nbsp;</asp:CustomValidator>
<asp:HiddenField runat="server" ID="hdnIsVaccine" />
<table width="100%">
    <tr>
        <td style="width: 50%; vertical-align: top">
            <table style="width: 100%">
                <tr>
                    <td class="label">
                        <asp:Label ID="lblDetailItemID" runat="server" Text="Item ID"></asp:Label>
                    </td>
                    <td class="entry">
                        <telerik:RadComboBox runat="server" ID="cboDetailItemID" Width="300px" EnableLoadOnDemand="true"
                            HighlightTemplatedItems="true" AutoPostBack="true" MarkFirstMatch="False" OnItemDataBound="cboDetailItemID_ItemDataBound"
                            OnItemsRequested="cboDetailItemID_ItemsRequested" OnSelectedIndexChanged="cboDetailItemID_SelectedIndexChanged">
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "ItemName") %>
                    &nbsp;<b>(<%# DataBinder.Eval(Container.DataItem, "ItemID")%>) </b>
                                <%# Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "Balance")) > 0 ? DataBinder.Eval(Container.DataItem, "Balance", "<br />Stock : {0:n2}") : string.Empty%>
                            </ItemTemplate>
                            <FooterTemplate>
                                Note : Show max 20 items
                            </FooterTemplate>
                        </telerik:RadComboBox>
                    </td>
                    <td width="20px">
                        <asp:RequiredFieldValidator ID="rfvDetailItemID" runat="server" ErrorMessage="Detail Item ID required."
                            ControlToValidate="cboDetailItemID" SetFocusOnError="True" ValidationGroup="TransChargesItemConsumption"
                            Width="100%">
                            <asp:Image ID="Image1" runat="server" SkinID="rfvImage" />
                        </asp:RequiredFieldValidator>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:Label ID="lblQuantity" runat="server" Text="Quantity"></asp:Label>
                    </td>
                    <td class="entry">
                        <telerik:RadNumericTextBox ID="txtQty" runat="server" Width="100px" />
                        <telerik:RadTextBox ID="txtSRItemUnit" runat="server" Width="100px" ReadOnly="true" />
                    </td>
                    <td width="20px">
                        <asp:RequiredFieldValidator ID="rfvQuantity" runat="server" ErrorMessage="Quantity required."
                            ControlToValidate="txtQty" SetFocusOnError="True" ValidationGroup="TransChargesItemConsumption"
                            Width="100%">
                            <asp:Image ID="Image2" runat="server" SkinID="rfvImage" />
                        </asp:RequiredFieldValidator>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td align="right" colspan="2" style="height: 26px">
                        <asp:Button ID="btnUpdate" Text="Update" runat="server" CommandName="Update" ValidationGroup="TransChargesItemConsumption"
                            Visible='<%# !(DataItem is GridInsertionObject) %>'></asp:Button>
                        <asp:Button ID="btnInsert" Text="Insert" runat="server" CommandName="PerformInsert"
                            ValidationGroup="TransChargesItemConsumption" Visible='<%# DataItem is GridInsertionObject %>'></asp:Button>
                        &nbsp;
                        <asp:Button ID="btnCancel" Text="Cancel" runat="server" CausesValidation="False"
                            CommandName="Cancel"></asp:Button>
                    </td>
                </tr>
            </table>
        </td>
        <td style="width: 50%; vertical-align: top">
            <fieldset runat="server" id="divVaccineInf" visible="false">
                <legend>Immunization Information</legend>
                <table style="width: 100%">
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblDosage" runat="server" Text="Dosage & Unit"></asp:Label>
                        </td>
                        <td class="entry">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <telerik:RadNumericTextBox ID="txtQtyDosage" runat="server" Width="100px" MaxLength="5"
                                            MinValue="0" />
                                    </td>
                                    <td>&nbsp;
                                    </td>
                                    <td>
                                        <telerik:RadComboBox ID="cboSRDosageUnit" runat="server" Width="197px" EnableLoadOnDemand="true"
                                            MarkFirstMatch="true" HighlightTemplatedItems="true" AutoPostBack="false" OnItemDataBound="cboStandardReferenceItem_ItemDataBound"
                                            OnItemsRequested="cboSRDosageUnit_ItemsRequested">
                                            <ItemTemplate>
                                                <%# DataBinder.Eval(Container.DataItem, "ItemID")%>
                                                &nbsp;-&nbsp;
                                                <%# DataBinder.Eval(Container.DataItem, "ItemName")%>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                Note : Show max 10 items
                                            </FooterTemplate>
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td width="20px"></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="Label1" runat="server" Text="Drug Batch Number"></asp:Label>
                        </td>
                        <td class="entry">
                            <telerik:RadTextBox ID="txtBatchNumber" runat="server" Width="150px" />
                        </td>
                        <td width="20px"></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="Label4" runat="server" Text="Expiration Date"></asp:Label>
                        </td>
                        <td class="entry">
                            <telerik:RadDatePicker ID="txtExpirationDate" runat="server" Width="150px" />
                        </td>
                        <td width="20px"></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="Label2" runat="server" Text="Immunization Reason"></asp:Label>
                        </td>
                        <td class="entry">
                            <telerik:RadComboBox ID="cboSRImmReason" runat="server" Width="300px"></telerik:RadComboBox>
                        </td>
                        <td width="20px"></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="Label3" runat="server" Text="Immunization Routine Timing"></asp:Label>
                        </td>
                        <td class="entry">
                            <telerik:RadComboBox ID="cboSRImmTiming" runat="server" Width="300px"></telerik:RadComboBox>
                        </td>
                        <td width="20px"></td>
                        <td></td>
                    </tr>
                </table>
            </fieldset>
        </td>
    </tr>
</table>
