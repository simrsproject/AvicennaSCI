<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true"
    CodeBehind="AdmedikaDailyMonitoringDetail.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaDailyMonitoringDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
                                <td class="label">Tanggal Masuk</td>
                                <td class="entry"><asp:Label ID="lblRegistrationDate" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="label">Ruang / Kelas / Bed</td>
                                <td class="entry"><asp:Label ID="lblRoom" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="label">Discharge Date</td>
                                <td class="entry"><asp:Label ID="lblDischargeDate" runat="server" /></td>
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
                                <td class="label">Patient Name</td>
                                <td class="entry"><asp:Label ID="lblPatientName" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="label">Medical No</td>
                                <td class="entry"><asp:Label ID="lblMedicalNo" runat="server" /></td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="pnlSummary" runat="server" GroupingText="Monitoring" Width="100%">
                        <table width="100%">
                            <tr>
                                <td class="label">Hari Monitoring</td>
                                <td class="entry"><asp:Label ID="lblDayInfo" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="label">Status Pasien</td>
                                <td class="entry"><asp:Label ID="lblPatientStatus" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="label">Diagnosa Terakhir</td>
                                <td class="entry"><asp:Label ID="lblLastDiagnosis" runat="server" /></td>
                            </tr>
                            <tr>
                                <td class="label">Status Kirim</td>
                                <td class="entry"><asp:Label ID="lblSendStatus" runat="server" /></td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>

        <br />
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td class="label">Tanggal Monitoring</td>
                <td class="entry">
                    <telerik:RadDatePicker ID="txtMonitoringDate" runat="server" Width="120px" />
                    <asp:Button ID="btnShow" runat="server" Text="Tampilkan" OnClick="btnShow_Click" />
                </td>
                <td style="text-align: right; white-space: nowrap;">
                    <asp:Button ID="btnPull" runat="server" Text="Tarik Data" OnClick="btnPull_Click" />
                    &nbsp;
                    <telerik:RadComboBox ID="cboCategory" runat="server" Width="180px" />
                    <asp:Button ID="btnAddEntry" runat="server" Text="Tambah Entri" OnClick="btnAddEntry_Click" />
                    &nbsp;
                    <asp:Button ID="btnSave" runat="server" Text="Simpan" OnClick="btnSave_Click" />
                    &nbsp;
                    <asp:Button ID="btnSend" runat="server" Text="Kirim Monitoring" OnClick="btnSend_Click" />
                </td>
            </tr>
        </table>

        <br />
        <asp:Repeater ID="rptEntries" runat="server" OnItemCommand="rptEntries_ItemCommand"
            OnItemDataBound="rptEntries_ItemDataBound">
            <HeaderTemplate>
                <table width="100%" cellpadding="4" cellspacing="0" border="1" style="border-collapse: collapse;">
                    <tr style="background-color: #f2f2f2; font-weight: bold;">
                        <td style="width: 180px;">Kategori</td>
                        <td>Keterangan</td>
                        <td style="width: 130px;">Jam</td>
                        <td style="width: 90px;">Sumber</td>
                        <td style="width: 150px;">Status</td>
                        <td style="width: 60px;">Aksi</td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr>
                        <td><%# Eval("CategoryName") %></td>
                        <td>
                            <asp:HiddenField ID="hdnItemID" runat="server" Value='<%# Eval("AdmedikaDailyMonitoringItemID") %>' />
                            <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine" Width="100%"
                                Height="50px" Text='<%# Eval("Remarks") %>' />
                        </td>
                        <td><%# Eval("EntryTime") %></td>
                        <td><%# Eval("SourceType") %></td>
                        <td><%# Eval("Status") %></td>
                        <td style="text-align: center;">
                            <asp:LinkButton ID="lnkDelete" runat="server" Text="Hapus" CausesValidation="false"
                                CommandName="DeleteItem"
                                CommandArgument='<%# Eval("AdmedikaDailyMonitoringItemID") %>'
                                Visible='<%# !Convert.ToBoolean(Eval("IsSent")) %>'
                                OnClientClick="return confirm('Hapus entri ini?');" />
                        </td>
                    </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <br />
        <div style="font-weight: bold; font-size: 13px;">Riwayat Pengiriman</div>
        <telerik:RadGrid ID="grdHistory" runat="server" OnItemCommand="grdHistory_ItemCommand"
            AutoGenerateColumns="false" ShowGroupPanel="false" GridLines="None">
            <MasterTableView DataKeyNames="AdmedikaDailyMonitoringID">
                <Columns>
                    <telerik:GridDateTimeColumn DataField="MonitoringDate" HeaderText="Tanggal"
                        UniqueName="MonitoringDate" DataType="System.DateTime"
                        DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-Width="100px" />
                    <telerik:GridDateTimeColumn DataField="SentDateTime" HeaderText="Dikirim"
                        UniqueName="SentDateTime" DataType="System.DateTime"
                        DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Width="130px" />
                    <telerik:GridBoundColumn DataField="OkCount" HeaderText="Ok"
                        UniqueName="OkCount" HeaderStyle-Width="60px" />
                    <telerik:GridBoundColumn DataField="ErCount" HeaderText="Er"
                        UniqueName="ErCount" HeaderStyle-Width="60px" />
                    <telerik:GridBoundColumn DataField="Message" HeaderText="Pesan"
                        UniqueName="Message" />
                    <telerik:GridTemplateColumn UniqueName="Action" HeaderText="Action"
                        HeaderStyle-Width="80px" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelect" runat="server" Text="Pilih" CausesValidation="false"
                                CommandName="SelectDate"
                                CommandArgument='<%# Convert.ToDateTime(Eval("MonitoringDate")).ToString("yyyy-MM-dd") %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
</asp:Content>
