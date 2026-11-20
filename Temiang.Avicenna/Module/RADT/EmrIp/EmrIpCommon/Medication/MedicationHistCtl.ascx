<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MedicationHistCtl.ascx.cs" Inherits="Temiang.Avicenna.Module.RADT.EmrIp.EmrIpCommon.Medication.MedicationHistCtl" %>
<%@ Import Namespace="Temiang.Avicenna.Common" %>
<style type="text/css">

/* =================================
   MEDICATION KARDEX - COMPACT
================================= */

.schedule-label-content {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
}

.schedule-edit-button {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    margin-left: 3px;
    line-height: 0;
}

.schedule-edit-button img {
    width: 16px;
    height: 16px;
    object-fit: contain;
}

.medused-label,
.medused {
    width: 100% !important;
    border-collapse: collapse !important;
    border-spacing: 0 !important;
    table-layout: fixed !important;
    margin: 0 !important;
    padding: 0 !important;
    font-family: Arial, sans-serif;
    font-size: 11px;
}

/* Semua cell */
.medused-label th,
.medused-label td,
.medused th,
.medused td {
    border: 1px solid #aaa !important;
    box-sizing: border-box;
    padding: 2px 3px !important;
    vertical-align: middle;
    line-height: 14px;
}

/* Tinggi setiap baris */
.medused-label .row-date,
.medused .row-date {
    height: 20px !important;
}

.medused-label .row-schedule,
.medused .row-schedule {
    height: 22px !important;
}

.medused-label .row-verif,
.medused .row-verif {
    height: 22px !important;
}

.medused-label .row-realization,
.medused .row-realization {
    height: 28px !important;
}

.medused-label .row-balance-receive,
.medused .row-balance-receive {
    height: 20px !important;
}

.medused-label .row-balance-used,
.medused .row-balance-used {
    height: 20px !important;
}

.medused-label .row-high-alert,
.medused .row-high-alert {
    height: 28px !important;
}

.medused-label .row-notes,
.medused .row-notes {
    height: 45px !important;
}

/* Header tanggal */
.medused-label .row-date th,
.medused .row-date th {
    background: #4caf50 !important;
    color: white !important;
    text-align: center;
    font-weight: bold;
    padding: 2px !important;
}

/* Label */
.medused-label td {
    background: #f1f1f1;
    color: #333;
    text-align: left;
    white-space: normal;
}

/* Data */
.medused td {
    background: white;
    text-align: center;
    white-space: nowrap;
    overflow: hidden;
}

/* Ikon */
.medused img {
    max-width: 16px;
    max-height: 16px;
    vertical-align: middle;
}

/* Kolom label */
.medused-label {
    min-width: 112px;
}

/* Item dan Consume Method */
.medication-info {
    min-height: 205px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    padding: 4px;
    box-sizing: border-box;
}

.medication-info-footer {
    font-size: 11px;
    font-style: italic;
    padding-bottom: 4px;
}

/* PENTING: Hilangkan padding bawaan RadGrid */
.RadGrid .rgRow td.medication-day-cell,
.RadGrid .rgAltRow td.medication-day-cell {
    padding: 0 !important;
    margin: 0 !important;
    vertical-align: top !important;
}

.RadGrid .rgRow td.medication-label-cell,
.RadGrid .rgAltRow td.medication-label-cell {
    padding: 0 !important;
    vertical-align: top !important;
}

/* Tabel tanggal memenuhi seluruh kolom */
.medication-day-cell .medused {
    width: 100% !important;
    margin: 0 !important;
}

/* Notes */
.medused .row-notes td {
    text-align: center;
    vertical-align: middle;
    white-space: nowrap !important;
    overflow: hidden;
    text-overflow: ellipsis;
    overflow-wrap: normal !important;
    word-break: normal !important;
    font-size: 10px;
    padding: 3px 1px !important;
}

/* Nama Service Unit */
.medused .row-notes td strong {
    font-size: 10px;
    white-space: nowrap;
}

/* Keterangan notes */
.medused-note {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.medused-label {
    width: 105px !important;
    min-width: 105px;
}

.medused {
    width: 100% !important;
}

.medused th,
.medused td {
    padding: 2px !important;
}



/* =====================================
   ITEM & CONSUME METHOD DESIGN
===================================== */

/* Header */
.RadGrid .rgHeader.medication-grid-header {
    background: #d9d9d9 !important;
    color: #000 !important;
    border: 1px solid #222 !important;
    text-align: center !important;
    font-weight: normal !important;
    height: 52px !important;
}

/* Cell */
.RadGrid td.medication-grid-cell {
    border: 1px solid #222 !important;
    padding: 0 !important;
    vertical-align: top !important;
}

/* Container utama */
.medication-info {
    height: 198px;
    min-height: 198px;
    display: flex;
    flex-direction: column;
    padding: 0;
    box-sizing: border-box;
}

/* Item Description - Auto Wrap */
.medication-item-main {
    flex: 1;
    min-width: 0;
    display: flex;
    align-items: center;
    justify-content: center;

    text-align: center;
    white-space: normal !important;
    overflow-wrap: anywhere;
    word-break: normal;

    line-height: 18px;
    padding: 10px;
    box-sizing: border-box;
}

/* Paksa elemen di dalam deskripsi mengikuti wrapping */
.medication-item-main * {
    white-space: normal !important;
    overflow-wrap: anywhere !important;
    word-break: normal;
}

/* Footer */

/* Footer Setup By & Advised By */
.medication-info-footer {
    width: 100%;
    height: auto;
    min-height: 27px;

    display: block;
    box-sizing: border-box;

    font-size: 11px;
    font-style: italic;
    line-height: 14px;

    padding: 4px;

    white-space: normal !important;
    overflow-wrap: anywhere;
    word-break: normal;
}

/* Consume Method Content */
.medication-consume-content {
    position: relative;
    height: 171px;
    min-height: 171px;
    flex: none;
    padding: 0;
    box-sizing: border-box;
}

/* Teks Consume Method */
.medication-consume-text {
    position: absolute;
    top: 50%;
    left: 0;
    width: 100%;
    transform: translateY(-50%);
    text-align: center;
    line-height: 17px;
    padding: 0 5px;
    box-sizing: border-box;
}

/* Tombol Pensil - kiri bawah */
.medication-edit-action {
    position: absolute;
    left: 5px;
    bottom: 8px;
    z-index: 5;
    margin: 0;
    padding: 0;
}

.medication-edit-action a {
    display: inline-block;
    padding: 0;
    border: none;
    background: transparent;
    line-height: 0;
}

.medication-edit-action img {
    width: 16px;
    height: 16px;
    object-fit: contain;
    vertical-align: middle;
}

/* Hilangkan background hover */
.medication-edit-action a:hover {
    background: transparent;
}

</style>
<telerik:RadGrid ID="grdMedicationHist" runat="server" Width="100%"
    AutoGenerateColumns="False" GridLines="None" AllowPaging="False">
    <MasterTableView>
       
<Columns>

    <%-- ITEM DESCRIPTION --%>
    <telerik:GridTemplateColumn
        UniqueName="ItemDescription"
        HeaderText="Item"
        HeaderStyle-Width="240px"
        ItemStyle-Width="240px"
        HeaderStyle-CssClass="medication-grid-header"
        ItemStyle-CssClass="medication-grid-cell"
        ItemStyle-VerticalAlign="Top">

        <ItemTemplate>
            <div class="medication-info">

                <div class="medication-item-main">
                    <%# Temiang.Avicenna.BusinessObject.MedicationReceive.PrescriptionItemDescription(
                        Eval("RefTransactionNo"),
                        Eval("RefSequenceNo"),
                        Eval("ItemDescription"),
                        false,
                        Eval("SRMedicationRoute")
                    ) %>
                </div>

                <div class="medication-info-footer">
                    Setup by: <%# Eval("SetupByName") %>
                </div>

            </div>
        </ItemTemplate>

    </telerik:GridTemplateColumn>


    <%-- CONSUME METHOD --%>
    <telerik:GridTemplateColumn
        UniqueName="ConsumeMethod"
        HeaderText="Consume Method"
        HeaderStyle-Width="160px"
        ItemStyle-Width="160px"
        ItemStyle-CssClass="medication-grid-cell"
        HeaderStyle-CssClass="medication-grid-header"
        HeaderStyle-HorizontalAlign="Center"
        ItemStyle-VerticalAlign="Top">

        
        <ItemTemplate>

            <div class="medication-info">

                <div class="medication-consume-content">

                    <%-- CONSUME METHOD TEXT --%>
                    <div class="medication-consume-text">

                        <%# Eval("SRConsumeMethodName") %>
                        @<%# Eval("ConsumeQtyInString") %>
                        <%# Eval("SRConsumeUnit") %>

                        <%# Eval("SRMedicationConsumeName") == DBNull.Value
                            ? string.Empty
                            : ", " + Eval("SRMedicationConsumeName") %>

                        <br /><br />

                        <%# MedicationChangeConsumeMethodHtml(
                            Eval("MedicationReceiveNo"),
                            Eval("SRConsumeMethodName"),
                            Eval("PatientID"),
                            Eval("BalanceQty"),
                            Eval("IsAntibiotic"),
                            Eval("IsVoid"),
                            Eval("IsContinue")
                        ) %>

                    </div>

                    <%-- BUTTON EDIT DI KIRI BAWAH --%>
                    <div class="medication-edit-action">

                        <%# MedicationEditHtml(
                            Eval("MedicationReceiveNo"),
                            Eval("PatientID")
                        ) %>

                    </div>

                </div>

                <%-- ADVISED BY --%>
                <div class="medication-info-footer">
                    Advised by: <%# Eval("AdvisedByName") %>
                </div>

            </div>

        </ItemTemplate>


    </telerik:GridTemplateColumn>

    <%-- ROW LABELS --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationRowLabels"
        HeaderText=""
        HeaderStyle-Width="112px"
        ItemStyle-Width="112px"
        ItemStyle-CssClass="medication-label-cell"
        ItemStyle-VerticalAlign="Top">

        <ItemTemplate>
            <table class="medused-label">
                <tbody>
                    <tr class="row-date">
                        <th>Date</th>
                    </tr>
                    <tr class="row-schedule">
                        <td>
                            <div class="schedule-label-content">
                                <span>Schedule Time</span>

                                <a href="<%# MedicationSetupStatusUrl %>"
                                   onclick="window.parent.openMedicationSetupStatusFromHistory(this.href); return false;"
                                   title="Medication Setup Status"
                                   class="schedule-edit-button">
                                    <img src="<%# Helper.UrlRoot() %>/Images/Toolbar/edit16.png" alt="Edit" />
                                </a>
                            </div>
                        </td>
                    </tr>
                    <tr class="row-verif">
                        <td>Verif</td>
                    </tr>
                    <tr class="row-realization">
                        <td>Realization</td>
                    </tr>
                    <tr class="row-balance-receive">
                        <td>Balance Receive</td>
                    </tr>
                    <tr class="row-balance-used">
                        <td>Balance after used</td>
                    </tr>
                    <tr class="row-high-alert">
                        <td>High Alert Double Check</td>
                    </tr>
                    <tr class="row-notes">
                        <td>Notes</td>
                    </tr>
                </tbody>
            </table>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 10 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay10"
        HeaderText="10"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 1) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 09 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay09"
        HeaderText="09"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 2) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 08 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay08"
        HeaderText="08"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 3) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 07 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay07"
        HeaderText="07"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 4) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 06 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay06"
        HeaderText="06"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 5) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 05 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay05"
        HeaderText="05"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 6) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 04 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay04"
        HeaderText="04"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 7) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 03 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay03"
        HeaderText="03"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 8) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 02 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay02"
        HeaderText="02"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 9) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>


    <%-- DAY 01 --%>
    <telerik:GridTemplateColumn
        UniqueName="MedicationDay01"
        HeaderText="01"
        HeaderStyle-Width="220px"
        ItemStyle-Width="220px"
        ItemStyle-CssClass="medication-day-cell"
        HeaderStyle-HorizontalAlign="Center">

        <ItemTemplate>
            <%# MedicationRealizationHtml(Container, 10) %>
        </ItemTemplate>
    </telerik:GridTemplateColumn>

</Columns>

    </MasterTableView>
    <ClientSettings EnableRowHoverStyle="False">
        <Selecting AllowRowSelect="False" />
        <Scrolling
            AllowScroll="True"
            UseStaticHeaders="True"
            SaveScrollPosition="true"
            FrozenColumnsCount="2">
        </Scrolling>
    </ClientSettings>
</telerik:RadGrid>