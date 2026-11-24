
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterCustom.Master" AutoEventWireup="true" CodeBehind="AppointmentCancelList.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.OutPatient.AppointmentCancel" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdAppointmentList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdAppointmentList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilter">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdAppointmentList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboServiceUnitID">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="cboParamedicID" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>

    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td width="50%" valign="top">
                <table width="100%">
                    <tr>
                        <td class="label">
                            Appointment Date
                        </td>
                        <td class="entry">
                            <telerik:RadDatePicker ID="txtDate" runat="server" Width="150px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            Medical No
                        </td>
                        <td class="entry">
                            <telerik:RadTextBox ID="txtMedicalNo" runat="server"
                                Width="250px" MaxLength="20" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            Patient Name
                        </td>
                        <td class="entry">
                            <telerik:RadTextBox ID="txtPatientName" runat="server"
                                Width="250px" MaxLength="50" />
                        </td>
                    </tr>
                </table>
            </td>

            <td width="50%" valign="top">
                <table width="100%">
                    <tr>
                        <td class="label">
                            Service Unit
                        </td>
                        <td class="entry">
                            <telerik:RadComboBox ID="cboServiceUnitID" runat="server"
                                Width="250px"
                                AllowCustomText="true"
                                Filter="Contains"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="cboServiceUnitID_SelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            Physician
                        </td>
                        <td class="entry">
                            <telerik:RadComboBox ID="cboParamedicID" runat="server"
                                Width="250px"
                                EnableLoadOnDemand="true"
                                MarkFirstMatch="true"
                                HighlightTemplatedItems="true"
                                OnItemDataBound="cboParamedicID_ItemDataBound"
                                OnItemsRequested="cboParamedicID_ItemsRequested">
                                <FooterTemplate>
                                    Note : Show max 10 items
                                </FooterTemplate>
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label"></td>
                        <td class="entry">
                            <asp:ImageButton ID="btnFilter" runat="server"
                                ImageUrl="~/Images/Toolbar/search16.png"
                                OnClick="btnFilter_Click"
                                ToolTip="Search" />
                            &nbsp;Search Appointment
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <br />

    <telerik:RadGrid ID="grdAppointmentList" runat="server"
        OnNeedDataSource="grdAppointmentList_NeedDataSource"
        OnItemCommand="grdAppointmentList_ItemCommand"
        AutoGenerateColumns="False"
        AllowPaging="True"
        PageSize="15"
        AllowMultiRowSelection="true">

        <MasterTableView DataKeyNames="AppointmentNo">

            <GroupHeaderTemplate>
                <asp:CheckBox ID="chkSelectGroup"
                    runat="server"
                    Text=" Select All Appointment"
                    onclick="selectAppointmentGroup(this);" />

                <strong>
                    <%# ((GridGroupHeaderItem)Container).AggregatesValues["GROUPGRID"] %>
                </strong>
            </GroupHeaderTemplate>

            <GroupByExpressions>
                <telerik:GridGroupByExpression>
                    <SelectFields>
                        <telerik:GridGroupByField
                            FieldName="GROUPGRID"
                            HeaderText="Physician" />
                    </SelectFields>
                    <GroupByFields>
                        <telerik:GridGroupByField
                            FieldName="GROUPGRID"
                            SortOrder="Ascending" />
                    </GroupByFields>
                </telerik:GridGroupByExpression>
            </GroupByExpressions>

            <Columns>

                <telerik:GridBoundColumn
                    DataField="AppointmentQue"
                    HeaderText="Que No"
                    UniqueName="AppointmentQue"
                    HeaderStyle-Width="60px"
                    ItemStyle-HorizontalAlign="Center" />

                <telerik:GridBoundColumn
                    DataField="AppointmentNo"
                    HeaderText="Appointment No"
                    UniqueName="AppointmentNo"
                    HeaderStyle-Width="120px" />

                <telerik:GridDateTimeColumn
                    DataField="AppointmentDate"
                    HeaderText="Date"
                    UniqueName="AppointmentDate"
                    HeaderStyle-Width="90px"
                    DataFormatString="{0:dd/MM/yyyy}" />

                <telerik:GridBoundColumn
                    DataField="AppointmentTime"
                    HeaderText="Time"
                    UniqueName="AppointmentTime"
                    HeaderStyle-Width="70px" />

                <telerik:GridBoundColumn
                    DataField="VisitTypeName"
                    HeaderText="Visit Type"
                    UniqueName="VisitTypeName" />

                <telerik:GridBoundColumn
                    DataField="AppointmentStatus"
                    HeaderText="Status"
                    UniqueName="AppointmentStatus"
                    HeaderStyle-Width="100px" />

                <telerik:GridBoundColumn
                    DataField="AppointmentType"
                    HeaderText="Appt. Type"
                    UniqueName="AppointmentType" />

                <telerik:GridBoundColumn
                    DataField="MedicalNo"
                    HeaderText="Medical No"
                    UniqueName="MedicalNo"
                    HeaderStyle-Width="100px" />

                <telerik:GridTemplateColumn
                    DataField="PatientName"
                    HeaderText="Patient Name"
                    UniqueName="PatientName"
                    HeaderStyle-Width="200px">
                    <ItemTemplate>
                        <%# string.Format("{0} {1}",
                            DataBinder.Eval(Container.DataItem, "SalutationName"),
                            DataBinder.Eval(Container.DataItem, "PatientName")) %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn
                    DataField="PhoneNo"
                    HeaderText="Phone No"
                    UniqueName="PhoneNo" />

                <telerik:GridBoundColumn
                    DataField="MobilePhoneNo"
                    HeaderText="Mobile No"
                    UniqueName="MobilePhoneNo" />

                <telerik:GridBoundColumn
                    DataField="GuarantorName"
                    HeaderText="Guarantor"
                    UniqueName="GuarantorName" />

                <telerik:GridBoundColumn
                    DataField="Notes"
                    HeaderText="Notes"
                    UniqueName="Notes" />

                <telerik:GridBoundColumn
                    DataField="LastCreateByUserID"
                    HeaderText="Created By"
                    UniqueName="LastCreateByUserID" />

                <telerik:GridTemplateColumn
                    HeaderText="Cancel"
                    UniqueName="Cancel"
                    HeaderStyle-Width="60px"
                    ItemStyle-HorizontalAlign="Center"
                    HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:ImageButton ID="btnCancelAppointment"
                            runat="server"
                            ImageUrl="~/Images/Toolbar/cancel16.png"
                            ToolTip="Cancel Appointment"
                            CommandName="CancelAppointment"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "AppointmentNo") %>'
                            OnClientClick="return confirm('Anda Yakin ingin cancel Appointment berikut?');" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

            </Columns>
        </MasterTableView>

        <ClientSettings EnableRowHoverStyle="true">
            <Resizing AllowColumnResize="True" />
            <Selecting AllowRowSelect="True" />
        </ClientSettings>

    </telerik:RadGrid>

    <br />

    <table width="100%">
        <tr>
            <td align="right">
                <asp:Button
                    ID="btnCancelSelected"
                    runat="server"
                    Text="Cancel Selected"
                    OnClick="btnCancelSelected_Click"
                    OnClientClick="return confirm('Yakin ingin membatalkan Appointment yang dipilih?');" />
            </td>
        </tr>
    </table>

    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server"
        Width="100%" LoadingPanelID="fw_ajxLoadingPanel"
        EnableEmbeddedScripts="false">
    </telerik:RadAjaxPanel>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            function selectAppointmentGroup(chk) {

                var grid = $find("<%= grdAppointmentList.ClientID %>");

                if (!grid) return;

                var masterTable = grid.get_masterTableView();
                var dataItems = masterTable.get_dataItems();

                var groupHeader = chk.closest("tr");

                if (!groupHeader) return;

                var currentRow = groupHeader.nextElementSibling;

                while (currentRow) {

                    if (currentRow.className.indexOf("rgGroupHeader") >= 0) {
                        break;
                    }

                    for (var i = 0; i < dataItems.length; i++) {

                        var dataRow = dataItems[i].get_element();

                        if (dataRow === currentRow) {
                            dataItems[i].set_selected(chk.checked);
                            break;
                        }
                    }

                    currentRow = currentRow.nextElementSibling;
                }
            }

        </script>
    </telerik:RadCodeBlock>

</asp:Content>
