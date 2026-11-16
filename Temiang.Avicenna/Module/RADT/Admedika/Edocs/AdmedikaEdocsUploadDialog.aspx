<%@ Page Title="Upload Document" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master"
    AutoEventWireup="true" CodeBehind="AdmedikaEdocsUploadDialog.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaEdocsUploadDialog" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function closeUploadDialog() {
            GetRadWindow().close();
        }
    </script>
    <asp:HiddenField ID="hdnDocumentID" runat="server" Value="0" />
    <asp:Panel ID="pnlInfo" runat="server" GroupingText="Claim" Width="100%">
        <table width="100%">
            <tr>
                <td class="label">Card No</td>
                <td class="entry"><asp:Label ID="lblCardNo" runat="server" /></td>
            </tr>
            <tr>
                <td class="label">ClID</td>
                <td class="entry"><asp:Label ID="lblClID" runat="server" /></td>
            </tr>
            <tr>
                <td class="label">Patient Name</td>
                <td class="entry"><asp:Label ID="lblPatientName" runat="server" /></td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <asp:Panel ID="pnlUpload" runat="server" GroupingText="Upload Document" Width="100%">
        <table width="100%">
            <tr>
                <td class="label">File Type</td>
                <td class="entry">
                    <telerik:RadComboBox ID="cboDocType" runat="server" Width="100%"
                        MarkFirstMatch="true" Filter="Contains" />
                </td>
            </tr>
            <tr>
                <td class="label">Remarks</td>
                <td class="entry">
                    <telerik:RadTextBox ID="txtRemarks" runat="server" Width="100%"
                        TextMode="MultiLine" Height="80px" MaxLength="500" />
                </td>
            </tr>
            <tr>
                <td class="label">File</td>
                <td class="entry">
                    <telerik:RadAsyncUpload ID="uplFile" runat="server" Width="100%"
                        ControlObjectsVisibility="None" InitialFileInputsCount="1" MaxFileInputsCount="1"
                        AllowedFileExtensions=".pdf,.jpg,.jpeg,.png" MaxFileSize="10485760" />
                    <div style="font-size: 11px; color: #666;">
                        PDF / JPG / PNG, maksimal 10 MB
                    </div>
                </td>
            </tr>
            <tr>
                <td></td>
                <td class="entry" style="text-align: right; white-space: nowrap;">
                    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" />
                    &nbsp;
                    <asp:Button ID="btnUploadAndSend" runat="server" Text="Upload &amp; Send"
                        OnClick="btnUploadAndSend_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
