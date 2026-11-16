<%@ Page Title="Select Patient" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master"
    AutoEventWireup="true" CodeBehind="AdmedikaPatientDialogList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaPatientDialogList" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnSearchGuarantorCardNo">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdPatient" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSearchPatientName">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdPatient" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdPatient">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdPatient" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td width="50%" style="vertical-align: top">
                <table width="100%">
                    <tr>
                        <td class="label">
                            Guarantor Card No
                        </td>
                        <td class="entry">
                            <telerik:RadTextBox ID="txtGuarantorCardNo" runat="server" Width="300px" MaxLength="32" />
                        </td>
                        <td style="text-align: left">
                            <asp:ImageButton ID="btnSearchGuarantorCardNo" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                OnClick="btnSearch_Click" ToolTip="Search" />
                        </td>
                    </tr>
                </table>
            </td>
            <td width="50%" style="vertical-align: top">
                <table width="100%">
                    <tr>
                        <td class="label">
                            Patient Name
                        </td>
                        <td class="entry">
                            <telerik:RadTextBox ID="txtPatientName" runat="server" Width="300px" MaxLength="150" />
                        </td>
                        <td style="text-align: left">
                            <asp:ImageButton ID="btnSearchPatientName" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                OnClick="btnSearch_Click" ToolTip="Search" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <telerik:RadGrid ID="grdPatient" runat="server" OnNeedDataSource="grdPatient_NeedDataSource"
        AutoGenerateColumns="false" ShowGroupPanel="false" AllowPaging="true" PageSize="15"
        AllowSorting="true" GridLines="None">
        <MasterTableView DataKeyNames="PatientID" ClientDataKeyNames="PatientID">
            <Columns>
                <telerik:GridBoundColumn DataField="PatientID" HeaderText="PatientID"
                    UniqueName="PatientID" SortExpression="PatientID" HeaderStyle-Width="100px" />
                <telerik:GridBoundColumn DataField="PatientName" HeaderText="Patient Name"
                    UniqueName="PatientName" SortExpression="PatientName" />
                <telerik:GridBoundColumn DataField="MedicalNo" HeaderText="Medical No"
                    UniqueName="MedicalNo" SortExpression="MedicalNo" HeaderStyle-Width="100px" />
                <telerik:GridBoundColumn DataField="OldMedicalNo" HeaderText="Old Medical No"
                    UniqueName="OldMedicalNo" SortExpression="OldMedicalNo" HeaderStyle-Width="100px" />
                <telerik:GridBoundColumn DataField="SSN" HeaderText="SSN"
                    UniqueName="SSN" SortExpression="SSN" HeaderStyle-Width="100px" />
                <telerik:GridBoundColumn DataField="Gender" HeaderText="Gender"
                    UniqueName="Gender" SortExpression="Gender" HeaderStyle-Width="70px" />
                <telerik:GridDateTimeColumn DataField="DateOfBirth" HeaderText="Date of Birth"
                    UniqueName="DateOfBirth" SortExpression="DateOfBirth" DataType="System.DateTime"
                    DataFormatString="{0:MM/dd/yyyy}" HeaderStyle-Width="90px" />
                <telerik:GridBoundColumn DataField="Address" HeaderText="Address"
                    UniqueName="Address" SortExpression="Address" />
                <telerik:GridBoundColumn DataField="PhoneNo" HeaderText="Phone No"
                    UniqueName="PhoneNo" SortExpression="PhoneNo" HeaderStyle-Width="100px" />
                <telerik:GridBoundColumn DataField="MobilePhoneNo" HeaderText="Mobile Phone No"
                    UniqueName="MobilePhoneNo" SortExpression="MobilePhoneNo" HeaderStyle-Width="110px" />
            </Columns>
        </MasterTableView>
        <ClientSettings EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
        </ClientSettings>
    </telerik:RadGrid>
</asp:Content>
