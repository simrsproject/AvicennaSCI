<%@ Page Title="Discharge OutPatient" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master"
    AutoEventWireup="true" CodeBehind="AdmedikaDischargeDialog.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaDischargeDialog" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="rcbDischargeScript" runat="server">
    <script type="text/javascript">
        function closeDischargeDialog() {
            GetRadWindow().close();
        }

        function confirmDischarge() {
            if (typeof Page_ClientValidate === 'function' && !Page_ClientValidate('discharge')) {
                var diagnosis = document.getElementById('<%= txtDiagnosisCodeList.ClientID %>');
                if (diagnosis) {
                    diagnosis.scrollIntoView({ block: 'center' });
                    diagnosis.focus();
                }
                return false;
            }
            return confirm('Submit discharge ke AdMedika? Pastikan mapping benefit dan billing sudah final.');
        }

        function printDischargeLOC() {
            window.open('AdmedikaPrintOut.aspx?id=<%= ClaimTransactionID %>&type=D', '_blank',
                'width=760,height=700,scrollbars=yes,resizable=yes');
            return false;
        }
    </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnRefreshEntitlement">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlDischargeContent" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnProcessDischarge">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlDischargeContent" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>

    <asp:Panel ID="pnlDischargeContent" runat="server">
    <asp:Panel ID="pnlRequest" runat="server">
        <asp:Panel ID="pnlClaim" runat="server" GroupingText="Patient &amp; Claim" Width="100%">
            <table width="100%">
                <tr>
                    <td class="label" style="width:14%;">Registration No</td>
                    <td class="entry" style="width:19%;"><asp:Label ID="lblRegistrationNo" runat="server" /></td>
                    <td class="label" style="width:14%;">Patient</td>
                    <td class="entry" style="width:19%;"><asp:Label ID="lblPatientName" runat="server" /></td>
                    <td class="label" style="width:14%;">Card No</td>
                    <td class="entry"><asp:Label ID="lblCardNo" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label">Claim ID</td>
                    <td class="entry"><asp:Label ID="lblClaimID" runat="server" /></td>
                    <td class="label">Claim Status</td>
                    <td class="entry"><asp:Label ID="lblClaimStatus" runat="server" /></td>
                    <td class="label">Physician</td>
                    <td class="entry"><asp:Label ID="lblPhysician" runat="server" /></td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel ID="pnlMedical" runat="server" GroupingText="Medical Information" Width="100%"
            Style="margin-top:8px;">
            <asp:Label ID="lblStoredMedicalNote" runat="server" Visible="false"
                Text="Detail izin sakit, kecelakaan, operasi dan remarks belum tersedia untuk claim lama ini. Diagnosis ditampilkan dari data claim tersimpan."
                Style="display:block;padding:6px;color:#666;" />
            <asp:ValidationSummary ID="validationSummary" runat="server" ValidationGroup="discharge" />
            <table width="100%">
                <tr>
                    <td class="label" style="width:14%;">ICD-10 <span style="color:red;">*</span></td>
                    <td class="entry" style="width:36%;">
                        <telerik:RadTextBox ID="txtDiagnosisCodeList" runat="server" Width="100%" MaxLength="500" />
                        <asp:RequiredFieldValidator ID="rfvDiagnosis" runat="server"
                            ControlToValidate="txtDiagnosisCodeList" ErrorMessage="Diagnosis ICD-10 wajib diisi."
                            ForeColor="Red" ValidationGroup="discharge" Display="Dynamic" />
                        <div style="font-size:11px;color:#666;">Pisahkan beberapa diagnosis dengan koma.</div>
                    </td>
                    <td class="label" style="width:14%;">Izin Sakit</td>
                    <td class="entry">
                        <telerik:RadNumericTextBox ID="txtMcDays" runat="server" Width="80px"
                            MinValue="0" MaxValue="99" NumberFormat-DecimalDigits="0" Value="0" /> hari
                    </td>
                </tr>
                <tr>
                    <td class="label">Kecelakaan</td>
                    <td class="entry">
                        <asp:RadioButtonList ID="rblAccidentFlag" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Tidak" Value="N" Selected="True" />
                            <asp:ListItem Text="Ya" Value="Y" />
                        </asp:RadioButtonList>
                    </td>
                    <td class="label">Operasi</td>
                    <td class="entry">
                        <asp:RadioButtonList ID="rblSurgicalFlag" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Tidak" Value="N" Selected="True" />
                            <asp:ListItem Text="Ya" Value="Y" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="label">Remarks</td>
                    <td class="entry" colspan="3">
                        <telerik:RadTextBox ID="txtRemarks" runat="server" Width="100%" Height="55px"
                            TextMode="MultiLine" MaxLength="500" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel ID="pnlEntitlement" runat="server" GroupingText="Entitlement" Width="100%"
            Style="margin-top:8px;">
            <div style="padding:4px 0;">
                <asp:Button ID="btnRefreshEntitlement" runat="server" Text="Refresh Benefit &amp; Billing"
                    OnClick="btnRefreshEntitlement_Click" CausesValidation="false" />
                <asp:Label ID="lblEntitlementMessage" runat="server" Style="margin-left:8px;" />
            </div>
            <asp:Literal ID="litEntitlement" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlBilling" runat="server" GroupingText="Billing Item &amp; Benefit Mapping"
            Width="100%" Style="margin-top:8px;">
            <telerik:RadGrid ID="grdBillingItems" runat="server" AutoGenerateColumns="false"
                OnNeedDataSource="grdBillingItems_NeedDataSource"
                OnItemDataBound="grdBillingItems_ItemDataBound" ShowFooter="true" GridLines="None">
                <MasterTableView DataKeyNames="SourceType,TransactionNo,SequenceNo,ItemID,ItemName,Quantity,TotalPrice,IssueDate">
                    <Columns>
                        <telerik:GridBoundColumn DataField="SourceType" HeaderText="Type" HeaderStyle-Width="90px" />
                        <telerik:GridDateTimeColumn DataField="IssueDate" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}"
                            HeaderStyle-Width="90px" />
                        <telerik:GridBoundColumn DataField="ItemID" HeaderText="Item ID" HeaderStyle-Width="120px" />
                        <telerik:GridBoundColumn DataField="ItemName" HeaderText="Item Name" />
                        <telerik:GridNumericColumn DataField="Quantity" HeaderText="Qty" DataFormatString="{0:n2}"
                            HeaderStyle-Width="70px" ItemStyle-HorizontalAlign="Right" />
                        <telerik:GridNumericColumn DataField="TotalPrice" HeaderText="Total" DataFormatString="{0:n2}"
                            Aggregate="Sum" FooterAggregateFormatString="{0:n2}" HeaderStyle-Width="110px"
                            ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />
                        <telerik:GridTemplateColumn HeaderText="AdMedika Benefit" HeaderStyle-Width="250px">
                            <ItemTemplate>
                                <telerik:RadComboBox ID="cboBenefit" runat="server" Width="240px"
                                    AllowCustomText="true" Filter="Contains" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings EnableRowHoverStyle="true" />
            </telerik:RadGrid>
            <div id="pnlMappingHint" runat="server" style="padding:8px 0;color:#666;">
                Pilih benefit untuk setiap item. Item resep otomatis memilih benefit obat (41) jika tersedia.
            </div>
            <asp:Label ID="lblStoredBillingNote" runat="server" Visible="false"
                Style="display:block;padding:6px;color:#666;" />
        </asp:Panel>

        <div style="padding:10px 8px;text-align:right;">
            <asp:Button ID="btnProcessDischarge" runat="server" Text="Process Discharge"
                OnClick="btnProcessDischarge_Click" ValidationGroup="discharge"
                OnClientClick="if (!confirmDischarge()) return false;" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlResponse" runat="server" GroupingText="Discharge Response" Width="100%"
        Style="margin-top:8px;">
        <asp:Label ID="lblProcessMessage" runat="server" Visible="false" />
        <asp:Literal ID="litResponse" runat="server" />
        <div style="padding-top:8px;text-align:right;">
            <asp:Button ID="btnPrintLOC" runat="server" Text="Print LOC" Enabled="false"
                CausesValidation="false" OnClientClick="return printDischargeLOC();" />
        </div>
    </asp:Panel>
    </asp:Panel>
</asp:Content>
