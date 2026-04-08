<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true"
    CodeBehind="ImmunizationDetail.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Master.ImmunizationDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script language="javascript" type="text/javascript">
        function cboSsBridgingID_ClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            context["tp"] = "cvxgroup";
        }
        function onSatuSehatItemClick(name) {
            var txt = $find("ctl00_ContentPlaceHolder1_grdAliasName_ctl00_ctl05_EditFormControl_txtServiceUnitAliasName");
            txt.set_value(name);
        }
    </script>
    <table width="100%">
        <tr>
            <td class="label">
                <asp:Label ID="lblImmunizationID" runat="server" Text="Immunization ID"></asp:Label>
            </td>
            <td class="entry">
                <telerik:RadTextBox ID="txtImmunizationID" runat="server" Width="100px" MaxLength="10" />
            </td>
            <td width="20">
                <asp:RequiredFieldValidator ID="rfvImmunizationID" runat="server" ErrorMessage="Immunization ID required."
                    ValidationGroup="entry" ControlToValidate="txtImmunizationID" SetFocusOnError="True"
                    Width="100%">
                    <asp:Image ID="Image1" runat="server" SkinID="rfvImage" />
                </asp:RequiredFieldValidator>
            </td>
            <td></td>
        </tr>
        <tr>
            <td class="label">
                <asp:Label ID="lblImmunizationName" runat="server" Text="Immunization Name"></asp:Label>
            </td>
            <td class="entry">
                <telerik:RadTextBox ID="txtImmunizationName" runat="server" Width="300px" MaxLength="100" />
            </td>
            <td width="20">
                <asp:RequiredFieldValidator ID="rfvImmunizationName" runat="server" ErrorMessage="Immunization Name required."
                    ValidationGroup="entry" ControlToValidate="txtImmunizationName" SetFocusOnError="True"
                    Width="100%">
                    <asp:Image ID="Image2" runat="server" SkinID="rfvImage" />
                </asp:RequiredFieldValidator>
            </td>
            <td></td>
        </tr>
                <tr>
            <td class="label">
                <asp:Label ID="Label1" runat="server" Text="Maximum Number of Immunizations"></asp:Label>
            </td>
            <td class="entry">
                <telerik:RadNumericTextBox ID="txtMaxCount" runat="server" Width="100px" NumberFormat-DecimalDigits="0"/>
            </td>
            <td width="20">
            </td>
            <td></td>
        </tr>
                <tr>
            <td class="label">
                <asp:Label ID="Label2" runat="server" Text="Sequence No"></asp:Label>
            </td>
            <td class="entry">
                <telerik:RadNumericTextBox ID="txtIndexNo" runat="server" Width="100px" NumberFormat-DecimalDigits="0"/>
            </td>
            <td width="20">

            </td>
            <td></td>
        </tr>
    </table>
    <telerik:RadTabStrip ID="tabStrip" runat="server" MultiPageID="multiPage" SelectedIndex="0">
        <Tabs>
            <telerik:RadTab runat="server" Text="Bridging & Integration" PageViewID="pgvAliasName" Selected="true"/>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="multiPage" runat="server" SelectedIndex="0" BorderStyle="Solid"
        BorderColor="gray">
        <telerik:RadPageView runat="server" ID="pgvAliasName">
            <telerik:RadGrid ID="grdAliasName" runat="server" OnNeedDataSource="grdAliasName_NeedDataSource"
                AutoGenerateColumns="False" GridLines="None" OnUpdateCommand="grdAliasName_UpdateCommand"
                OnDeleteCommand="grdAliasName_DeleteCommand" OnInsertCommand="grdAliasName_InsertCommand"
                AllowPaging="true">
                <HeaderContextMenu>
                </HeaderContextMenu>
                <MasterTableView CommandItemDisplay="None" DataKeyNames="ImmunizationID, SRBridgingType, BridgingID"
                    PageSize="15">
                    <CommandItemTemplate>
                        <asp:LinkButton ID="lbInsert" runat="server" CommandName="InitInsert" Visible='<%# !grdAliasName.MasterTableView.IsItemInserted %>'>
                            <img style="border: 0px; vertical-align: middle;" alt="" src="../../../../../Images/Toolbar/insert16.png" />
                            &nbsp;<asp:Label runat="server" ID="lblAddRow" Text="Add new Interaction"></asp:Label>
                        </asp:LinkButton>
                    </CommandItemTemplate>
                    <CommandItemStyle Height="29px" />
                    <Columns>
                        <telerik:GridEditCommandColumn ButtonType="ImageButton">
                            <HeaderStyle Width="35px" />
                            <ItemStyle CssClass="MyImageButton" />
                        </telerik:GridEditCommandColumn>
                        <telerik:GridBoundColumn HeaderStyle-Width="250px" DataField="BridgingTypeName" HeaderText="Bridging Type"
                            UniqueName="BridgingTypeName" SortExpression="BridgingTypeName" HeaderStyle-HorizontalAlign="Left"
                            ItemStyle-HorizontalAlign="Left" />
                        <telerik:GridBoundColumn HeaderStyle-Width="150px" DataField="BridgingID" HeaderText="Bridging ID"
                            UniqueName="BridgingID" SortExpression="BridgingID" HeaderStyle-HorizontalAlign="Left"
                            ItemStyle-HorizontalAlign="Left" />
                        <telerik:GridBoundColumn DataField="BridgingName" HeaderText="Bridging Name" UniqueName="BridgingName"
                            SortExpression="BridgingName" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                        <telerik:GridCheckBoxColumn HeaderStyle-Width="80px" DataField="IsActive" HeaderText="Active"
                            UniqueName="IsActive" SortExpression="IsActive" HeaderStyle-HorizontalAlign="Center"
                            ItemStyle-HorizontalAlign="Center" />
                        <telerik:GridButtonColumn UniqueName="DeleteColumn" Text="Delete" CommandName="Delete"
                            ButtonType="ImageButton" ConfirmText="Delete this row?">
                            <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" CssClass="MyImageButton" />
                        </telerik:GridButtonColumn>
                    </Columns>
                    <EditFormSettings UserControlName="~\Module\RADT\Master\ItemService\ItemAliasDetail.ascx" EditFormType="WebUserControl">
                        <EditColumn UniqueName="ItemAliasEditCommand">
                        </EditColumn>
                    </EditFormSettings>
                </MasterTableView>
                <FilterMenu>
                </FilterMenu>
                <ClientSettings EnableRowHoverStyle="true">
                    <Resizing AllowColumnResize="True" />
                    <Selecting AllowRowSelect="True" />
                </ClientSettings>
            </telerik:RadGrid>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</asp:Content>
