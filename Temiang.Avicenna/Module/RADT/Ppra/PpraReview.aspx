<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master"
    CodeBehind="PpraReview.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Ppra.PpraReview" %>
<%@ Import Namespace="Temiang.Avicenna.Common" %>
<%@ Import Namespace="Temiang.Avicenna.BusinessObject" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/CustomControl/SoapInfoCtl.ascx" TagPrefix="cc" TagName="SoapInfoCtl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <telerik:RadCodeBlock runat="server" ID="RadCodeBlock1">
        <script type="text/javascript">
            function approveNonPpabPrescription(prescNo) {
                if (confirm("Approve resep Non PPAB ini dan kirim ke Farmasi?")) {
                    document.getElementById('<%= hdnPrescNo.ClientID %>').value = prescNo;
                    document.getElementById('<%= hdnAction.ClientID %>').value = 'approve';
                    document.getElementById('<%= btnAction.ClientID %>').click();
                }
            }
            function rejectNonPpabPrescription(prescNo) {
                var defaultReason = "Diagnosa infeksi termasuk dalam PPAB, silahkan Revisi RASAL / RASLAN / RASPRAJA / RASPATUR / PROFILAKSIS";
                var reason = prompt("Alasan penolakan resep Non PPAB:", defaultReason);
                if (reason == null) return;
                reason = reason.replace(/^\s+|\s+$/g, "");
                if (reason.length == 0) { alert("Alasan penolakan wajib diisi."); return; }
                document.getElementById('<%= hdnPrescNo.ClientID %>').value = prescNo;
                document.getElementById('<%= hdnAction.ClientID %>').value = 'reject|' + encodeURIComponent(reason);
                document.getElementById('<%= btnAction.ClientID %>').click();
            }
        </script>
    </telerik:RadCodeBlock>

    <%-- Patient Info Header --%>
    <div style="background-color:#1a3c5e;color:#fff;padding:8px 12px;font-size:13pt;font-weight:bold;margin-bottom:8px;">
        <asp:Literal runat="server" ID="litPatientHeader" />
    </div>

    <table width="100%" cellpadding="0" cellspacing="4" style="margin-bottom:8px;">
        <tr>
            <td width="50%" valign="top">
                <table width="100%" class="label-table">
                    <tr>
                        <td class="label" width="130px">MRN</td>
                        <td><asp:Literal runat="server" ID="litMedicalNo" /></td>
                    </tr>
                    <tr>
                        <td class="label">Reg No</td>
                        <td><asp:Literal runat="server" ID="litRegistrationNo" /></td>
                    </tr>
                    <tr>
                        <td class="label">Reg Date</td>
                        <td><asp:Literal runat="server" ID="litRegistrationDate" /></td>
                    </tr>
                    <tr>
                        <td class="label">Physician</td>
                        <td><asp:Literal runat="server" ID="litParamedicName" /></td>
                    </tr>
                    <tr>
                        <td class="label">Guarantor</td>
                        <td><asp:Literal runat="server" ID="litGuarantorName" /></td>
                    </tr>
                </table>
            </td>
            <td width="50%" valign="top">
                <table width="100%" class="label-table">
                    <tr>
                        <td class="label" width="130px">Service Unit</td>
                        <td><asp:Literal runat="server" ID="litServiceUnit" /></td>
                    </tr>
                    <tr>
                        <td class="label">Room / Bed</td>
                        <td><asp:Literal runat="server" ID="litRoomBed" /></td>
                    </tr>
                    <tr>
                        <td class="label">Gender</td>
                        <td><asp:Literal runat="server" ID="litGender" /></td>
                    </tr>
                    <tr>
                        <td class="label">DOB / Age</td>
                        <td><asp:Literal runat="server" ID="litDobAge" /></td>
                    </tr>
                    <tr>
                        <td class="label">Drug Allergies</td>
                        <td><asp:Literal runat="server" ID="litAllergies" /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <%-- Section: Pending Non PPAB Prescriptions --%>
    <fieldset style="margin-bottom:8px;">
        <legend><strong>Antibiotic Non PPAB - Pending PPRA Review</strong></legend>
        <asp:Literal runat="server" ID="litPendingPrescriptions" />
    </fieldset>

    <%-- Section: RASPRO Form --%>
    <fieldset style="margin-bottom:8px;">
        <legend><strong>Form PPRA (RASPRO/RASLAN/RASPRAJA)</strong></legend>
        <asp:Literal runat="server" ID="litRasproForm" />
    </fieldset>

    <%-- Section: SOAP History --%>
    <fieldset style="margin-bottom:8px;">
        <legend><strong>SOAP History</strong></legend>
        <cc:SoapInfoCtl runat="server" ID="soapInfoCtl" />
    </fieldset>

    <%-- Section: Exam Order — Lab --%>
    <fieldset style="margin-bottom:8px;">
        <legend><strong>Lab Results</strong></legend>
        <telerik:RadGrid ID="grdLab" runat="server" AutoGenerateColumns="false"
            OnNeedDataSource="grdLab_NeedDataSource" GridLines="None"
            AllowPaging="false">
            <MasterTableView DataKeyNames="TransactionNo">
                <NoRecordsTemplate>
                    <div style="padding:6px;color:#888;">No lab orders found.</div>
                </NoRecordsTemplate>
                <Columns>
                    <telerik:GridBoundColumn DataField="TransactionNo" HeaderText="Order No" UniqueName="TransactionNo">
                        <HeaderStyle Width="130px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="TransactionDate" HeaderText="Date" UniqueName="TransactionDate" DataFormatString="{0:dd-MMM-yyyy}">
                        <HeaderStyle Width="100px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ItemName" HeaderText="Exam Item" UniqueName="ItemName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ResultValue" HeaderText="Result" UniqueName="ResultValue">
                        <HeaderStyle Width="150px" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </fieldset>

    <%-- Section: Exam Order — Radiology --%>
    <fieldset style="margin-bottom:8px;">
        <legend><strong>Radiology Results</strong></legend>
        <telerik:RadGrid ID="grdRad" runat="server" AutoGenerateColumns="false"
            OnNeedDataSource="grdRad_NeedDataSource" GridLines="None"
            AllowPaging="false">
            <MasterTableView DataKeyNames="TransactionNo">
                <NoRecordsTemplate>
                    <div style="padding:6px;color:#888;">No radiology orders found.</div>
                </NoRecordsTemplate>
                <Columns>
                    <telerik:GridBoundColumn DataField="TransactionNo" HeaderText="Order No" UniqueName="TransactionNo">
                        <HeaderStyle Width="130px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="TransactionDate" HeaderText="Date" UniqueName="TransactionDate" DataFormatString="{0:dd-MMM-yyyy}">
                        <HeaderStyle Width="100px" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ItemName" HeaderText="Exam Item" UniqueName="ItemName">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ResultValue" HeaderText="Result" UniqueName="ResultValue">
                        <HeaderStyle Width="150px" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </fieldset>

    <%-- Hidden postback controls --%>
    <asp:HiddenField runat="server" ID="hdnPrescNo" />
    <asp:HiddenField runat="server" ID="hdnAction" />
    <asp:Button runat="server" ID="btnAction" Style="display:none;" OnClick="btnAction_Click" />

</asp:Content>
