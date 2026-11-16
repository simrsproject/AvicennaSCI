<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true"
    CodeBehind="AdmedikaEdocsDetail.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaEdocsDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function openUploadDialog() {
                var oWnd = $find("<%= winUpload.ClientID %>");
                oWnd.setUrl("AdmedikaEdocsUploadDialog.aspx?id=<%= ClaimTransactionID %>");
                oWnd.show();
            }

            function onUploadClientClose(oWnd, args) {
                var mode = (oWnd.argument && oWnd.argument.mode)
                    ? oWnd.argument.mode
                    : "rebind";

                __doPostBack("<%= grdDocuments.UniqueID %>", mode);
                if (oWnd.argument) oWnd.argument.mode = null;
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindow ID="winUpload" runat="server" Animation="None"
        Width="640px" Height="480px" ShowContentDuringLoad="False"
        VisibleStatusbar="False" Modal="true" Behavior="Close,Move,Resize,Maximize"
        OnClientClose="onUploadClientClose" />
    <div style="padding: 20px;">
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
                                <td class="label">Is Closed</td>
                                <td class="entry"><asp:Label ID="lblIsClosed" runat="server" /></td>
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
                                <td class="label">Patient Name (AdMedika)</td>
                                <td class="entry"><asp:Label ID="lblPatientName" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="label">Medical No</td>
                                <td class="entry"><asp:Label ID="lblMedicalNo" runat="server" /></td>
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
                                <td class="label">Registration Date</td>
                                <td class="entry"><asp:Label ID="lblRegistrationDate" runat="server" /></td>
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
                <td style="font-weight: bold; font-size: 13px;">Documents</td>
                <td style="text-align: right; white-space: nowrap;">
                    <asp:Button ID="btnUpload" runat="server" Text="Upload Document"
                        OnClientClick="openUploadDialog(); return false;" />
                </td>
            </tr>
        </table>
        <telerik:RadGrid ID="grdDocuments" runat="server" OnNeedDataSource="grdDocuments_NeedDataSource"
            OnItemCommand="grdDocuments_ItemCommand" OnItemDataBound="grdDocuments_ItemDataBound"
            AutoGenerateColumns="false" ShowGroupPanel="false" GridLines="None">
            <MasterTableView DataKeyNames="AdmedikaDocumentID" ClientDataKeyNames="AdmedikaDocumentID">
                <Columns>
                    <telerik:GridDateTimeColumn DataField="CreatedDateTime" HeaderText="Created Date"
                        UniqueName="CreatedDateTime" SortExpression="CreatedDateTime" DataType="System.DateTime"
                        DataFormatString="{0:MM/dd/yyyy HH:mm}" HeaderStyle-Width="130px" />
                    <telerik:GridTemplateColumn DataField="DocType" HeaderText="Kode &amp; Jenis File"
                        UniqueName="DocType" SortExpression="DocType" HeaderStyle-Width="250px">
                        <ItemTemplate>
                            <%# BuildDocTypeHtml(Container.DataItem) %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="FileName" HeaderText="File Name"
                        UniqueName="FileName" SortExpression="FileName" />
                    <telerik:GridBoundColumn DataField="CreatedByUserID" HeaderText="Upload By"
                        UniqueName="CreatedByUserID" SortExpression="CreatedByUserID" HeaderStyle-Width="110px" />
                    <telerik:GridDateTimeColumn DataField="SentDateTime" HeaderText="Sent Time"
                        UniqueName="SentDateTime" SortExpression="SentDateTime" DataType="System.DateTime"
                        DataFormatString="{0:MM/dd/yyyy HH:mm}" HeaderStyle-Width="130px" />
                    <telerik:GridBoundColumn DataField="SentByUserID" HeaderText="Sent By"
                        UniqueName="SentByUserID" SortExpression="SentByUserID" HeaderStyle-Width="110px" />
                    <telerik:GridBoundColumn DataField="ResponseMessage" HeaderText="Response Message"
                        UniqueName="ResponseMessage" SortExpression="ResponseMessage" />
                    <telerik:GridTemplateColumn DataField="IsSentSuccess" HeaderText="Status"
                        UniqueName="Status" SortExpression="IsSentSuccess" HeaderStyle-Width="90px"
                        ItemStyle-Font-Bold="true">
                        <ItemTemplate>
                            <%# GetStatusText(Container.DataItem) %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Action" HeaderText="Action"
                        HeaderStyle-Width="150px" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <a href='AdmedikaEdocsFile.ashx?id=<%# Eval("AdmedikaDocumentID") %>' target="_blank">Open File</a>
                            <asp:LinkButton ID="lnkSend" runat="server" CausesValidation="false"
                                CommandName="SendDocument"
                                CommandArgument='<%# Eval("AdmedikaDocumentID") %>'
                                Text='<%# GetSendText(Container.DataItem) %>'
                                Visible='<%# IsSendVisible(Container.DataItem) %>'
                                style="margin-left: 8px;" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="True" />
            </ClientSettings>
        </telerik:RadGrid>
    </div>
</asp:Content>
