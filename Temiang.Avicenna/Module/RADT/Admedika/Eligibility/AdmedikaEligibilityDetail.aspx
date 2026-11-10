<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true"
    CodeBehind="AdmedikaEligibilityDetail.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaEligibilityDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td width="50%" valign="top" style="padding-right: 8px;">
                <asp:Panel ID="pnlClaim" runat="server" GroupingText="Claim" Width="100%">
                    <table width="100%">
                        <tr>
                            <td class="label">ClID</td>
                            <td class="entry"><asp:Label ID="lblClID" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">ClStatus</td>
                            <td class="entry"><asp:Label ID="lblClStatus" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">ClDesc</td>
                            <td class="entry"><asp:Label ID="lblClDesc" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Created Date Time</td>
                            <td class="entry"><asp:Label ID="lblCreatedDateTime" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Is Closed</td>
                            <td class="entry"><asp:Label ID="lblIsClosed" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Discharge Date Time</td>
                            <td class="entry"><asp:Label ID="lblDischargeDateTime" runat="server" /></td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <asp:Panel ID="pnlCoverage" runat="server" GroupingText="Coverage &amp; Policy" Width="100%">
                    <table width="100%">
                        <tr>
                            <td class="label">CovID</td>
                            <td class="entry"><asp:Label ID="lblCovID" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">CovCode</td>
                            <td class="entry"><asp:Label ID="lblCovCode" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Plan Code</td>
                            <td class="entry"><asp:Label ID="lblPlanCode" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Policy No</td>
                            <td class="entry"><asp:Label ID="lblPolicyNo" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Payor Member ID</td>
                            <td class="entry"><asp:Label ID="lblPayorMemberID" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Policy Date</td>
                            <td class="entry"><asp:Label ID="lblPolicyDate" runat="server" /></td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
            <td width="50%" valign="top">
                <asp:Panel ID="pnlPatient" runat="server" GroupingText="Patient" Width="100%">
                    <table width="100%">
                        <tr>
                            <td class="label">Card No</td>
                            <td class="entry"><asp:Label ID="lblCardNo" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">PatientID</td>
                            <td class="entry"><asp:Label ID="lblPatientID" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Patient Name (Avicenna)</td>
                            <td class="entry"><asp:Label ID="lblPatientNameAvicenna" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Patient Name (AdMedika)</td>
                            <td class="entry"><asp:Label ID="lblPatientNameAdmedika" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Medical No</td>
                            <td class="entry"><asp:Label ID="lblMedicalNo" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Date of Birth</td>
                            <td class="entry"><asp:Label ID="lblDateOfBirth" runat="server" /></td>
                        </tr>
                    </table>
                </asp:Panel>
                <br />
                <asp:Panel ID="pnlRegistration" runat="server" GroupingText="Registration" Width="100%">
                    <table width="100%">
                        <tr>
                            <td class="label">Registration No</td>
                            <td class="entry"><asp:Label ID="lblRegistrationNo" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Guarantor ID / Name</td>
                            <td class="entry"><asp:Label ID="lblGuarantor" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label">Is Void</td>
                            <td class="entry"><asp:Label ID="lblIsVoid" runat="server" /></td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <br />
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td style="font-weight: bold; font-size: 13px;">Print Out LOA</td>
            <td style="text-align: right;">
                <asp:Button ID="btnRePrintLOA" runat="server" Text="Re-print LOA"
                    OnClick="btnRePrintLOA_Click" />
            </td>
        </tr>
    </table>
    <asp:Literal ID="litPrintOutLOA" runat="server" />
</asp:Content>
