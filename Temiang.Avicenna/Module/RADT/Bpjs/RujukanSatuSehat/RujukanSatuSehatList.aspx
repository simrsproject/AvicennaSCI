<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true" CodeBehind="RujukanSatuSehatList.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Bpjs.RujukanSatuSehatList" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadGrid ID="grdList" runat="server" OnNeedDataSource="grdList_NeedDataSource" OnItemDataBound="grdList_ItemDataBound"
        AutoGenerateColumns="False" ShowGroupPanel="false" AllowPaging="True" PageSize="15"
        AllowSorting="True" GridLines="None">
        <MasterTableView DataKeyNames="noSep,NoRujukan,noRujukanSatuSehat" ClientDataKeyNames="noSep">
            <Columns>
                <telerik:GridDateTimeColumn HeaderStyle-Width="120px" DataField="NoRujukan" HeaderText="No Rujukan"
                    HeaderStyle-HorizontalAlign="Center" UniqueName="NoRujukan" SortExpression="NoRujukan"
                    ItemStyle-HorizontalAlign="Center" />
                <telerik:GridDateTimeColumn HeaderStyle-Width="120px" DataField="noRujukanSatuSehat" HeaderText="No Rujukan SS"
                    HeaderStyle-HorizontalAlign="Center" UniqueName="noRujukanSatuSehat" SortExpression="NoRujukanSS"
                    ItemStyle-HorizontalAlign="Center" />
                <telerik:GridDateTimeColumn HeaderStyle-Width="120px" DataField="noSep" HeaderText="No SEP"
                    HeaderStyle-HorizontalAlign="Center" UniqueName="noSep" SortExpression="noSep"
                    ItemStyle-HorizontalAlign="Center" />
                <telerik:GridDateTimeColumn HeaderStyle-Width="80px" DataField="tglRujukan" HeaderText="Tgl Rujukan"
                    HeaderStyle-HorizontalAlign="Center" UniqueName="tglRujukan" SortExpression="tglRujukan"
                    ItemStyle-HorizontalAlign="Center" />
                <telerik:GridDateTimeColumn HeaderStyle-Width="80px" DataField="tglRencana" HeaderText="Tgl Rencana"
                    HeaderStyle-HorizontalAlign="Center" UniqueName="tglRencana" SortExpression="tglRencana"
                    ItemStyle-HorizontalAlign="Center" />
                <telerik:GridBoundColumn HeaderStyle-Width="110px" DataField="NomorKartu" HeaderText="No Kartu"
                    UniqueName="NomorKartu" SortExpression="NomorKartu" HeaderStyle-HorizontalAlign="Center"
                    ItemStyle-HorizontalAlign="Center" />
                <telerik:GridBoundColumn DataField="NamaPasienJK" HeaderText="Nama Pasien (JK)" UniqueName="NamaPasienJK"
                    SortExpression="NamaPasienJK" />
                <telerik:GridBoundColumn DataField="TypeOfService" HeaderText="Jenis Pelayanan" UniqueName="TypeOfService"
                    SortExpression="TypeOfService" />
                <telerik:GridBoundColumn DataField="namaPoliRujukan" HeaderText="Poli Tujuan" UniqueName="namaPoliRujukan"
                    SortExpression="namaPoliRujukan" />
                <telerik:GridBoundColumn DataField="DiagnoseName" HeaderText="Diagnosa Awal" UniqueName="DiagnoseName"
                    SortExpression="DiagnoseName" />
                <telerik:GridBoundColumn DataField="catatan" HeaderText="Catatan" UniqueName="catatan"
                    SortExpression="catatan" />
            </Columns>
            <NestedViewTemplate>
                <fieldset style="padding-left: 20px; padding-bottom: 10px;">
                    <legend>Satu Sehat Rujukan</legend>

                    <table style="width: 100%">
                        <tr>
                            <td width="200"><b>Kode Faskes</b></td>
                            <td><%# Eval("kodeFaskesSatuSehat") %></td>
                        </tr>
                        <tr>
                            <td><b>ID Pasien SatuSehat</b></td>
                            <td><%# Eval("idPasienSatuSehat") %></td>
                        </tr>
                        <tr>
                            <td><b>PPK Tujuan</b></td>
                            <td><%# Eval("kdppkSatuSehatTujuanRujukan") %></td>
                        </tr>
                        <tr>
                            <td><b>Kode Dokter</b></td>
                            <td><%# Eval("kdDokterSatuSehat") %></td>
                        </tr>
                        <tr>
                            <td><b>Encounter</b></td>
                            <td><%# Eval("EncounterReference") %></td>
                        </tr>
                        <tr>
                            <td><b>Instruction</b></td>
                            <td><%# Eval("patientInstruction") %></td>
                        </tr>
                        <tr>
                            <td><b>Keterangan</b></td>
                            <td><%# Eval("keteranganRujukan") %></td>
                        </tr>
                    </table>

                    <br />

                    <telerik:RadGrid ID="grdKriteria" runat="server"
                        AutoGenerateColumns="False" GridLines="None">
                        <MasterTableView DataKeyNames="linkId">
                            <Columns>

                                <telerik:GridBoundColumn DataField="linkId"
                                    HeaderText="Link ID"
                                    UniqueName="linkId"
                                    HeaderStyle-Width="100px" />

                                <telerik:GridBoundColumn DataField="text"
                                    HeaderText="Kriteria"
                                    UniqueName="text"
                                    HeaderStyle-Width="300px" />

                                <telerik:GridBoundColumn DataField="answer"
                                    HeaderText="Answer"
                                    UniqueName="answer" />

                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>

                </fieldset>
            </NestedViewTemplate>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
