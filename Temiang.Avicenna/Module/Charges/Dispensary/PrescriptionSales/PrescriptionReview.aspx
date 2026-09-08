<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterDialogEntry.Master"
    AutoEventWireup="true" CodeBehind="PrescriptionReview.aspx.cs" Inherits="Temiang.Avicenna.Module.Charges.Dispensary.PrescriptionSales.PrescriptionReview" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hdfReturnValue" />
    <fieldset>
        <legend><asp:Label runat="server" ID="lblReview" Text="Review"></asp:Label></legend>

        <!-- BUTTON CHECK ALL / UNCHECK ALL -->
        <div style="margin-bottom: 8px;">
            <asp:Button
                ID="btnCheckAll"
                runat="server"
                Text="Check All"
                CausesValidation="false"
                UseSubmitBehavior="false"
                OnClientClick="setAllReviewCheckboxes(true); return false;" />

            <asp:Button
                ID="btnUncheckAll"
                runat="server"
                Text="Uncheck All"
                CausesValidation="false"
                UseSubmitBehavior="false"
                OnClientClick="setAllReviewCheckboxes(false); return false;" />
        </div>

        <telerik:RadGrid ID="grdPrescriptionReview" Width="100%" runat="server" RenderMode="Lightweight"
            AutoGenerateColumns="False" EnableViewState="true" AllowMultiRowSelection="True"
            OnItemDataBound="grdPrescriptionReview_ItemDataBound" OnNeedDataSource="grdPrescriptionReview_NeedDataSource">
            <MasterTableView DataKeyNames="ItemID" Width="100%">
                <Columns>
                    <telerik:GridBoundColumn DataField="ItemName" UniqueName="ItemName" HeaderText="Review"
                        HeaderStyle-Width="200px" />
                    <telerik:GridTemplateColumn HeaderText="Prescription" UniqueName="IsPrescriptionReviewEdit"
                        HeaderStyle-Width="50px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chkIsPrescriptionReview" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridCheckBoxColumn HeaderText="Prescription" DataField="IsPrescriptionReview"
                        UniqueName="IsPrescriptionReview" HeaderStyle-Width="50px" Display="False" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridTemplateColumn HeaderText="Drug" UniqueName="IsDrugReviewEdit" HeaderStyle-Width="50px"
                        HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox runat="server" ID="chkIsDrugReview" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridCheckBoxColumn HeaderText="Drug" DataField="IsDrugReview" UniqueName="IsDrugReview"
                        HeaderStyle-Width="50px" Display="False" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center" />
                    <telerik:GridTemplateColumn HeaderText="Follow-Up" UniqueName="NoteEdit">
                        <ItemTemplate>
                            <telerik:RadTextBox ID="txtNote" runat="server" Width="100%">
                            </telerik:RadTextBox>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Note" UniqueName="Note" HeaderText="Follow-Up" />
                </Columns>
            </MasterTableView>
            <FilterMenu>
            </FilterMenu>
            <ClientSettings EnableRowHoverStyle="False" AllowGroupExpandCollapse="False">
                <Resizing AllowColumnResize="False" />
                <Scrolling UseStaticHeaders="True" ScrollHeight=""></Scrolling>
            </ClientSettings>
        </telerik:RadGrid>
    </fieldset>
    <!-- JAVASCRIPT -->
        <script type="text/javascript">

            function setAllReviewCheckboxes(checked) {

                var grid = $find("<%= grdPrescriptionReview.ClientID %>");

                if (!grid)
                    return;

                var rows = grid.get_masterTableView().get_dataItems();

                for (var i = 0; i < rows.length; i++) {

                    var rowElement = rows[i].get_element();

                    // ==========================================
                    // PRESCRIPTION
                    // ==========================================
                    var prescriptionCheckboxes = rowElement.querySelectorAll(
                        "input[type='checkbox'][id*='chkIsPrescriptionReview']"
                    );

                    for (var p = 0; p < prescriptionCheckboxes.length; p++) {

                        if (!prescriptionCheckboxes[p].disabled) {
                            prescriptionCheckboxes[p].checked = checked;
                        }
                    }


                    // ==========================================
                    // DRUG
                    // ==========================================
                    var drugCheckboxes = rowElement.querySelectorAll(
                        "input[type='checkbox'][id*='chkIsDrugReview']"
                    );

                    for (var d = 0; d < drugCheckboxes.length; d++) {

                        if (!drugCheckboxes[d].disabled) {
                            drugCheckboxes[d].checked = checked;
                        }
                    }
                }
            }

    </script>
</asp:Content>
