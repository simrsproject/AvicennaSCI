<%@ Page Title="New Eligibility" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master"
    AutoEventWireup="true" CodeBehind="AdmedikaEligibilityDialog.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaEligibilityDialog" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function closeEligibilityDialog() {
            GetRadWindow().close();
        }
    </script>
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnCheckBenefit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" LoadingPanelID="fw_ajxLoadingPanel" />
                    <telerik:AjaxUpdatedControl ControlID="pnlBenefit" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnProcessEligibility">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlMain" LoadingPanelID="fw_ajxLoadingPanel" />
                    <telerik:AjaxUpdatedControl ControlID="pnlEligibilityResponse" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td width="42%" valign="top" style="padding-right: 8px;">
                <asp:Panel ID="pnlMain" runat="server" GroupingText="Eligibility Request" Width="100%">
                    <div style="background-color: #0d6efd; color: #ffffff; padding: 4px 8px;
                        margin-bottom: 8px; font-weight: bold;">
                        Terminal ID : <asp:Label ID="lblTerminalID" runat="server" />
                    </div>
                    <asp:ValidationSummary ID="validationSummary" runat="server" ValidationGroup="eligibility" />
                    <table width="100%">
                        <tr>
                            <td class="label">
                                Card No
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtCardNo" runat="server" Width="100%" MaxLength="32" />
                                <asp:RequiredFieldValidator ID="rfvCardNo" runat="server" ControlToValidate="txtCardNo"
                                    ErrorMessage="Card No required." ValidationGroup="eligibility" Display="None" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Coverage
                            </td>
                            <td class="entry">
                                <asp:RadioButtonList ID="rblCoverageType" runat="server" RepeatDirection="Vertical"
                                    RepeatColumns="2" ValidationGroup="eligibility" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="entry">
                                <asp:Button ID="btnCheckBenefit" runat="server" Text="Cek Manfaat &amp; TC"
                                    OnClick="btnCheckBenefit_Click" ValidationGroup="eligibility" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                ICD 10
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtDiagnosisCodeList" runat="server" Width="100%" MaxLength="500" />
                                <div style="font-size: 11px; color: #666;">
                                    Pisahkan dengan koma
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Remarks / Catatan
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtRemarks" runat="server" Width="100%" Height="80px"
                                    TextMode="MultiLine" MaxLength="255" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="entry">
                                <asp:Button ID="btnProcessEligibility" runat="server" Text="Proses Eligibility"
                                    OnClick="btnProcessEligibility_Click" ValidationGroup="eligibility" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
            <td width="58%" valign="top">
                <asp:Panel ID="pnlBenefit" runat="server" GroupingText="Manfaat &amp; Term Condition"
                    Width="100%">
                    <div style="min-height: 150px;">
                        <asp:Literal ID="litBenefit" runat="server" />
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlEligibilityResponse" runat="server" GroupingText="Response Eligibility"
                    Width="100%" Style="margin-top: 8px;">
                    <div style="min-height: 150px;">
                        <asp:Literal ID="litEligibilityResponse" runat="server" />
                    </div>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
