<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master"
    AutoEventWireup="true"
    CodeBehind="openApolDetail.aspx.cs"
    Inherits="Temiang.Avicenna.Module.Charges.Dispensary.PrescriptionSales.openApolDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- HEADER INFO -->
    <table style="margin-bottom:10px;">
        <tr>
            <td class="label">No SJP</td>
            <td><asp:Label ID="lblNOSJP" runat="server" /></td>

            <td class="label" style="padding-left:20px;">No Resep</td>
            <td><asp:Label ID="lblNORESEP" runat="server" /></td>
        </tr>
    </table>

    <!-- GRID DETAIL -->
    <telerik:RadGrid ID="grdApolDetail"
        runat="server"
        AutoGenerateColumns="false"
        AllowSorting="true"
        ShowStatusBar="true"
        AllowPaging="false"
        OnNeedDataSource="grdApolDetail_NeedDataSource">

        <MasterTableView AutoGenerateColumns="false">

            <Columns>

                <telerik:GridBoundColumn
                    DataField="KDOBT"
                    HeaderText="Kode Obat"
                    UniqueName="KDOBT" />

                <telerik:GridBoundColumn
                    DataField="NMOBAT"
                    HeaderText="Nama Obat"
                    UniqueName="NMOBAT"
                    HeaderStyle-Width="200px" />

                <telerik:GridBoundColumn
                    DataField="SIGNA1OBT"
                    HeaderText="Signa 1"
                    UniqueName="SIGNA1OBT"
                    ItemStyle-HorizontalAlign="Right" />

                <telerik:GridBoundColumn
                    DataField="SIGNA2OBT"
                    HeaderText="Signa 2"
                    UniqueName="SIGNA2OBT"
                    ItemStyle-HorizontalAlign="Right" />

                <telerik:GridBoundColumn
                    DataField="JHO"
                    HeaderText="JHO"
                    UniqueName="JHO"
                    ItemStyle-HorizontalAlign="Right" />

                <telerik:GridBoundColumn
                    DataField="JMLOBT"
                    HeaderText="Jumlah Obat"
                    UniqueName="JMLOBT"
                    ItemStyle-HorizontalAlign="Right" />

                <telerik:GridBoundColumn
                    DataField="JNSROBT"
                    HeaderText="Jenis Racik"
                    UniqueName="JNSROBT" />

                <telerik:GridBoundColumn
                    DataField="CATKHSOBT"
                    HeaderText="Catatan"
                    UniqueName="CATKHSOBT"
                    HeaderStyle-Width="200px" />

                <telerik:GridBoundColumn
                    DataField="MetadataCode"
                    HeaderText="Status"
                    UniqueName="MetadataCode" />

            </Columns>

        </MasterTableView>
    </telerik:RadGrid>

</asp:Content>