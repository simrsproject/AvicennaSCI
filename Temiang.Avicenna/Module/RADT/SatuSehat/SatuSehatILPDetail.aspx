<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDialogEntry.Master" AutoEventWireup="true"
    CodeBehind="SatuSehatILPDetail.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Emr.SatuSehatILPDetail" %>

<%@ Import Namespace="Temiang.Avicenna.Common" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock runat="server" ID="cb">
        <script type="text/javascript" language="javascript">
            function openSatuSehatRespondData(regNo, templateId, testNo) {
                var oWnd = $find("<%= winInfo.ClientID %>");
                var url = 'SatuSehatRespondData.aspx?regno=' + regNo + '&templateId=' + templateId + '&testNo=' + testNo;
                oWnd.setUrl(url);
                oWnd.setSize(1040, 600);
                oWnd.center();
                oWnd.show();
            }
        </script>
        <style>
            td.algn {
                padding-top: 10px;
                vertical-align: top;
            }
            .ColumnSign {
                float: left;
            }
            /* Clear floats after the columns */
            .RowSign:after {
                content: "";
                display: table;
                clear: both;
            }
        </style>
        <script type="text/javascript" language="javascript">

        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindow ID="winInfo" Width="1000px" Height="900px" runat="server" Modal="true" VisibleStatusbar="false"
        DestroyOnClose="false" Behavior="Close, Move" ReloadOnShow="True" ShowContentDuringLoad="false">
    </telerik:RadWindow>
    <fieldset>
        <legend>Registration Information</legend>
        <table width="100%">
            <tr>
                <td style="width:33%; vertical-align:top;">
                    <table width="100%">
                        <tr>
                            <td class="label">Registration No</td>
                            <td class="entry"><label id="lblRegNo" runat="server"></label></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">Registration Date</td>
                            <td class="entry"><label id="lblRegDate" runat="server"></label></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">Service Unit</td>
                            <td class="entry"><label id="lblServiceUnit" runat="server"></label></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">Physician</td>
                            <td class="entry"><label id="lblPhysician" runat="server"></label></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td style="width:33%; vertical-align:top;">
                    <table width="100%">
                        <tr>
                            <td class="label">Medical No</td>
                            <td class="entry"><label id="lblMRN" runat="server"></label></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">Patient Name</td>
                            <td class="entry"><label id="lblPatientName" runat="server"></label></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">Birth Date</td>
                            <td class="entry"><label id="lblBirthDate" runat="server"></label></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">SSN</td>
                            <td class="entry"><label id="lblSsn" runat="server"></label></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td style="width:34%; vertical-align:top;">
                    <table width="100%">
                        <tr>
                            <td class="label">Satu Sehat Patient ID</td>
                            <td class="entry"><label id="lblSSPatientID" runat="server"></label></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="label">Satu Sehat Encounter ID</td>
                            <td class="entry"><label id="lblSSEncID" runat="server"></label></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                &nbsp;
                            </td>
                        </tr>
                       <tr>
                            <td colspan="3">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnLoad" runat="server" Text="Populate Data" OnClick="btnLoad_Click" />
                                        </td>
                                        <td style="padding-left: 8px;">
                                            <asp:Button ID="btnClear" runat="server" Text="Clear and Populate Data" OnClick="btnClear_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Button ID="btnSend" runat="server" Text="Kirim Data" OnClick="btnSend_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <legend>Form Template <label id="lblFormName" runat="server"></label></legend>
        <table id="tblInput" runat="server" width="100%" border="0"></table>
    </fieldset>
</asp:Content>
