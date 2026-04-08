<%@  Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master" AutoEventWireup="true"
    CodeBehind="ItemConsumptionPackage.aspx.cs" Inherits="Temiang.Avicenna.Module.Charges.ItemConsumptionPackage" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManagerProxy runat="server" ID="RadAjaxManagerProxy1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function edit(transNo, seqNo, itemID) {
                var oWnd = $find("<%= winCharges.ClientID %>");
                oWnd.setUrl('ItemConsumptionPackageDetail.aspx?trans=' + transNo + '&seq=' + seqNo + '&item=' + itemID + "&unit=" + '<%= Request.QueryString["unit"] %>');
                oWnd.show();
            }

            function onClientClose(oWnd, args) {
                if (oWnd.argument && oWnd.argument.rebind != null)
                    __doPostBack("<%= grdList.UniqueID %>", "rebind");
            }
            
        </script>

    </telerik:RadCodeBlock>
    <telerik:RadWindow runat="server" Animation="None" Width="600px" Height="200px" Behavior="Move, Close"
        ShowContentDuringLoad="False" VisibleStatusbar="false" Modal="true" OnClientClose="onClientClose"
        ID="winCharges">
    </telerik:RadWindow>
    <asp:HiddenField runat="server" ID="hdnPageId" />
    <telerik:RadGrid ID="grdList" runat="server" AutoGenerateColumns="False" GridLines="None"
        OnNeedDataSource="grdList_NeedDataSource" OnDeleteCommand="grdList_DeleteCommand"
        OnInsertCommand="grdList_InsertCommand" OnItemDataBound="grdList_ItemDataBound">
        <MasterTableView CommandItemDisplay="Top" DataKeyNames="TransactionNo, SequenceNo, DetailItemID">
                        <ColumnGroups>
                <telerik:GridColumnGroup HeaderText="Vaccine Drugs" Name="Vaccine" HeaderStyle-HorizontalAlign="Center">
                </telerik:GridColumnGroup>
                <telerik:GridColumnGroup HeaderText="Dosage" Name="Dosage" HeaderStyle-HorizontalAlign="Center">
                </telerik:GridColumnGroup>
                <telerik:GridColumnGroup HeaderText="Immunization" Name="Immunization" HeaderStyle-HorizontalAlign="Center">
                </telerik:GridColumnGroup>
            </ColumnGroups>

            <Columns>
                <telerik:GridTemplateColumn Groupable="false" HeaderStyle-HorizontalAlign="Center"
                    ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="30px" Visible="False">
                    <ItemTemplate>
                        <%# string.Format("<a href=\"#\" onclick=\"edit('{0}', '{1}', '{2}'); return false;\"><img src=\"../../../../Images/Toolbar/edit16.png\" border=\"0\" alt=\"Edit\" /></a>", DataBinder.Eval(Container.DataItem, "TransactionNo"), DataBinder.Eval(Container.DataItem, "SequenceNo"), DataBinder.Eval(Container.DataItem, "DetailItemID"))%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="TransactionNo" UniqueName="TransactionNo" SortExpression="TransactionNo"
                    Visible="false" />
                <telerik:GridBoundColumn DataField="SequenceNo" UniqueName="SequenceNo" SortExpression="SequenceNo"
                    Visible="false" />
                <telerik:GridBoundColumn DataField="DetailItemID" UniqueName="DetailItemID" SortExpression="DetailItemID"
                    HeaderText="Item ID" HeaderStyle-Width="100" HeaderStyle-HorizontalAlign="Left"
                    ItemStyle-HorizontalAlign="Left" />
                <telerik:GridBoundColumn DataField="ItemName" UniqueName="ItemName" SortExpression="ItemName"
                    HeaderText="Item Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <telerik:GridNumericColumn DataField="Qty" UniqueName="Qty" SortExpression="Qty"
                    HeaderText="Qty" HeaderStyle-Width="50" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <telerik:GridTemplateColumn HeaderText="Realization" UniqueName="QtyRealizationText"
                    HeaderStyle-HorizontalAlign="center">
                    <HeaderStyle Width="70px" />
                    <ItemTemplate>
                        <telerik:RadNumericTextBox runat="server" ID="txtQtyRealization" Width="60px" Value='<%# Convert.ToDouble(DataBinder.Eval(Container.DataItem, "QtyRealization")) %>' 
                            MaxValue='<%# Convert.ToDouble(DataBinder.Eval(Container.DataItem, "MaxValue")) %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="SRItemUnit" UniqueName="SRItemUnit" SortExpression="SRItemUnit"
                    HeaderText="Unit" HeaderStyle-Width="60" HeaderStyle-HorizontalAlign="Left"
                    ItemStyle-HorizontalAlign="Left" />
                                <telerik:GridNumericColumn HeaderStyle-Width="60px" DataField="QtyDosage" HeaderText="Qty"
                    UniqueName="QtyDosage" SortExpression="QtyDosage" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ColumnGroupName="Dosage" />
                <telerik:GridBoundColumn HeaderStyle-Width="60px" DataField="SRDosageUnit" HeaderText="Unit"
                    UniqueName="SRDosageUnit" SortExpression="SRDosageUnit" HeaderStyle-HorizontalAlign="Left"
                    ItemStyle-HorizontalAlign="Left" ColumnGroupName="Dosage" />
                <telerik:GridTemplateColumn HeaderText="Paramedic" UniqueName="ParamedicID"
                    HeaderStyle-HorizontalAlign="center" ColumnGroupName="Immunization">
                    <HeaderStyle Width="200px" />
                    <ItemTemplate>
                        <telerik:RadComboBox runat="server" ID="cboParamedicID" Width="200px" EmptyMessage="Select a Paramedic"
                            EnableLoadOnDemand="true" ShowMoreResultsBox="true" EnableVirtualScrolling="true">
                            <WebServiceSettings Method="Paramedics" Path="~/WebService/ComboBoxDataService.asmx" />
                            <ClientItemTemplate>
                            <div>
                                <ul class="details">
                                    <li class="bold">
                                        <span>#= Text # </span>
                                    </li>
                                    <li class="smaller">
                                        <span>#= Attributes.SpecialtyName # </span>
                                    </li>
                                </ul>
                            </div>
                            </ClientItemTemplate>
                        </telerik:RadComboBox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Reason" UniqueName="ImmReason"
                    HeaderStyle-HorizontalAlign="center" ColumnGroupName="Immunization">
                    <HeaderStyle Width="200px" />
                    <ItemTemplate>
                        <telerik:RadComboBox runat="server" ID="cboSRImmReason" Width="100%"></telerik:RadComboBox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Routine Timing" UniqueName="ImmTiming"
                    HeaderStyle-HorizontalAlign="center" ColumnGroupName="Immunization">
                    <HeaderStyle Width="200px" />
                    <ItemTemplate>
                        <telerik:RadComboBox runat="server" ID="cboSRImmTiming" Width="100%"></telerik:RadComboBox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Batch Number" UniqueName="BatchNumber"
                    HeaderStyle-HorizontalAlign="center" ColumnGroupName="Vaccine">
                    <HeaderStyle Width="150px" />
                    <ItemTemplate>
                        <telerik:RadTextBox runat="server" ID="txtBatchNumber" Width="100%" Text='<%# DataBinder.Eval(Container.DataItem, "BatchNumber") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Expiration Date" UniqueName="ExpirationDate"
                    HeaderStyle-HorizontalAlign="center" ColumnGroupName="Vaccine">
                    <HeaderStyle Width="120px" />
                    <ItemTemplate>
                        <telerik:RadDatePicker runat="server" ID="txtExpirationDate" Width="100%" SelectedDate='<%# DataBinder.Eval(Container.DataItem, "ExpirationDate") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridCheckBoxColumn DataField="IsPackage" UniqueName="IsPackage" SortExpression="IsPackage"
                    Visible="false" />
                <telerik:GridTemplateColumn HeaderText="Location" UniqueName="location"
                    HeaderStyle-HorizontalAlign="center">
                    <HeaderStyle Width="200px" />
                    <ItemTemplate>
                        <telerik:RadComboBox runat="server" ID="cboLocationID" Width="100%"></telerik:RadComboBox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn UniqueName="DeleteColumn" Text="Delete" CommandName="Delete"
                    ButtonType="ImageButton" ConfirmText="Delete this row?">
                    <HeaderStyle Width="30px" />
                    <ItemStyle HorizontalAlign="Center" CssClass="MyImageButton" />
                </telerik:GridButtonColumn>
            </Columns>
            <EditFormSettings UserControlName="ItemConsumptionPackageEntry.ascx" EditFormType="WebUserControl">
                <EditColumn UniqueName="ItemConsumptionPackageEntryEditCommand">
                </EditColumn>
            </EditFormSettings>
        </MasterTableView>
        <ClientSettings EnableRowHoverStyle="false">
            <Resizing AllowColumnResize="True" />
            <Selecting AllowRowSelect="True" />
        </ClientSettings>
    </telerik:RadGrid>
</asp:Content>
