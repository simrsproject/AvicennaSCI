<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master" AutoEventWireup="true" CodeBehind="openApolDetail.aspx.cs"
    Inherits="Temiang.Avicenna.Module.Charges.Dispensary.PrescriptionSales.openApolDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="margin-bottom: 10px; margin-top: 10px;">
        <tr>
            <td>
                <asp:Label ID="lblNOSJP" runat="server" Text="No SJP" />
            </td>
            <td>
                <asp:TextBox ID="txtNOSJP" runat="server" ReadOnly="true" Width="150px" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblNORESEP" runat="server" Text="No Resep" />
            </td>
            <td>
                <asp:TextBox ID="txtNORESEP" runat="server" ReadOnly="true" Width="150px" />
            </td>
        </tr>
    </table>
    <telerik:RadGrid ID="grdApolDetail" runat="server" AutoGenerateColumns="false" AllowSorting="true" ShowStatusBar="true" AllowPaging="false"
        OnNeedDataSource="grdApolDetail_NeedDataSource" OnItemDataBound="grdApolDetail_ItemDataBound" OnItemCommand="grdApolDetail_ItemCommand">
        <MasterTableView AutoGenerateColumns="false" DataKeyNames="SequenceNo">
            <Columns>
                <telerik:GridBoundColumn DataField="KDOBT" HeaderText="Kode Obat" UniqueName="KDOBT" />
                <telerik:GridBoundColumn DataField="NMOBAT" HeaderText="Nama Obat" UniqueName="NMOBAT" HeaderStyle-Width="200px" />
                <telerik:GridBoundColumn DataField="JNSROBT" HeaderText="Jenis Racik" UniqueName="JNSROBT" />
                <telerik:GridBoundColumn DataField="PERMINTAAN" HeaderText="Permintaan" UniqueName="PERMINTAAN" />
                <telerik:GridBoundColumn DataField="SIGNA1OBT" HeaderText="Signa 1" UniqueName="SIGNA1OBT"  />
                <telerik:GridBoundColumn DataField="SIGNA2OBT" HeaderText="Signa 2" UniqueName="SIGNA2OBT"  />
                <telerik:GridBoundColumn DataField="JHO" HeaderText="Harian Obat" UniqueName="JHO"  />
                <telerik:GridBoundColumn DataField="JMLOBT" HeaderText="Jumlah Obat" UniqueName="JMLOBT"  />
                <telerik:GridBoundColumn DataField="MetadataCode" HeaderText="Status" UniqueName="MetadataCode" />
                <telerik:GridBoundColumn DataField="MetadataMessage" HeaderText="Message" UniqueName="MetadataMessage" />
                <telerik:GridTemplateColumn HeaderText="Resend" UniqueName="ResendColumn">
                <HeaderStyle Width="55px" />
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:LinkButton
                        ID="btnResend"
                        runat="server"
                        CommandName="ResendApol"
                        ToolTip="Kirim ulang obat ke APOL"
                        OnClientClick="return confirm('Kirim ulang obat ini ke BPJS APOL?');"
                        CausesValidation="false"
                        Style="text-decoration:none; font-size:18px;">
                        &#8635;
                    </asp:LinkButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Delete" UniqueName="DeleteColumn">
                <HeaderStyle Width="50px" />
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:LinkButton
                        ID="btnDeleteApol"
                        runat="server"
                        CommandName="DeleteApol"
                        ToolTip="Hapus detail APOL"
                        OnClientClick="return confirm('Yakin ingin menghapus obat ini dari APOL?');"
                        CausesValidation="false"
                        Style="color:#c62828; text-decoration:none; font-size:20px; font-weight:bold;">
                        &#215;
                    </asp:LinkButton>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
