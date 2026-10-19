<%@ Import Namespace="Temiang.Avicenna.Common" %>

<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterBootstrap.Master" AutoEventWireup="true"
    CodeBehind="BedMonitoringDisplayMjkn.aspx.cs"
    Inherits="Temiang.Avicenna.Module.Kiosk.BedMonitoringDisplayMjkn" EnableTheming="False" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        body {
            background-color: #1a1a24;
            color: #ffffff;
            overflow-x: hidden;
        }

        .content-header {
            padding: 8px 15px;
        }

        .kiosk-title {
            font-size: 2rem;
            font-weight: 700;
            color: #ffffff;
            margin: 0;
            letter-spacing: 1px;
            display: flex;
            align-items: center;
        }

        .kiosk-title i {
            margin-right: 12px;
            color: #17a2b8;
        }

        .kiosk-clock {
            font-size: 1.6rem;
            font-weight: 600;
            color: #ffc107;
            text-align: right;
        }

        .panel-box {
            margin-bottom: 16px;
        }

        .card-panel {
            background: #242533;
            border: 1px solid #3d3e52;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.4);
            margin-bottom: 14px;
            overflow: hidden;
        }

        .card-panel-header {
            padding: 10px 16px;
            font-weight: 700;
            font-size: 1.45rem;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-bottom: 2px solid rgba(255, 255, 255, 0.1);
        }

        /* Distinct color accents for class categories */
        .hdr-kl1 { background: linear-gradient(90deg, #0d6efd, #0b5ed7); color: #fff; }
        .hdr-kl2 { background: linear-gradient(90deg, #0dcaf0, #0aa2c0); color: #fff; }
        .hdr-kl3 { background: linear-gradient(90deg, #198754, #146c43); color: #fff; }
        .hdr-vip { background: linear-gradient(90deg, #fd7e14, #d35400); color: #fff; }
        .hdr-vvp { background: linear-gradient(90deg, #6f42c1, #59359a); color: #fff; }
        .hdr-int { background: linear-gradient(90deg, #dc3545, #b02a37); color: #fff; }

        .page-badge {
            font-size: 0.95rem;
            font-weight: 600;
            background: rgba(0, 0, 0, 0.35);
            padding: 3px 10px;
            border-radius: 12px;
            color: #fff;
        }

        .tbl-kiosk {
            width: 100%;
            margin-bottom: 0;
            border-collapse: collapse;
        }

        .tbl-kiosk thead th {
            background-color: #171822;
            color: #cbd5e1;
            font-size: 1.15rem;
            font-weight: 700;
            padding: 8px 10px;
            border-bottom: 2px solid #3d3e52;
            text-transform: uppercase;
        }

        .tbl-kiosk tbody tr {
            border-bottom: 1px solid #333446;
            transition: background-color 0.4s ease;
        }

        .tbl-kiosk tbody tr:nth-child(even) {
            background-color: rgba(255, 255, 255, 0.03);
        }

        .tbl-kiosk tbody td {
            padding: 7px 10px;
            font-size: 1.3rem;
            font-weight: 600;
            vertical-align: middle;
            color: #f8fafc;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .col-name {
            width: 58%;
            text-align: left;
        }

        .col-occupied {
            width: 21%;
            text-align: center;
            color: #fdba74;
            font-size: 1.4rem !important;
            font-weight: 700 !important;
        }

        .col-ready {
            width: 21%;
            text-align: center;
            color: #86efac;
            font-size: 1.45rem !important;
            font-weight: 800 !important;
        }

        .row-full {
            background-color: #7f1d1d !important;
            color: #fee2e2 !important;
        }

        .row-full td.col-ready {
            color: #fca5a5 !important;
        }

        .row-empty-slot td {
            color: transparent !important;
            user-select: none;
        }

        .table-container {
            min-height: 275px;
        }

        .fade-in-rows {
            animation: fadeIn 0.45s ease-in-out;
        }

        @keyframes fadeIn {
            from { opacity: 0.25; transform: translateY(3px); }
            to { opacity: 1; transform: translateY(0); }
        }
    </style>

    <div class="row pt-2 pb-2 align-items-center">
        <div class="col-8">
            <h1 class="kiosk-title">
                <i class="fas fa-procedures"></i>
                <span id="brandTitle"><%= GetBrandName() %></span> - INFORMASI KETERSEDIAAN TEMPAT TIDUR
            </h1>
        </div>
        <div class="col-4 kiosk-clock" id="liveClock">
            --:--:--
        </div>
    </div>

    <!-- Grid 2 Baris x 3 Kolom -->
    <div class="row">
        <!-- BARIS 1: KELAS I, KELAS II, KELAS III -->
        <div class="col-lg-4 col-md-6 col-12 panel-box">
            <div class="card-panel">
                <div class="card-panel-header hdr-kl1">
                    <span><i class="fas fa-bed mr-2"></i>KELAS I</span>
                    <span class="page-badge" id="badge-KL1">Hal 1/1</span>
                </div>
                <div class="table-container">
                    <table class="tbl-kiosk">
                        <thead>
                            <tr>
                                <th class="col-name">NAMA RUANGAN</th>
                                <th class="col-occupied">OCCUPIED</th>
                                <th class="col-ready">READY</th>
                            </tr>
                        </thead>
                        <tbody id="tbody-KL1" class="fade-in-rows"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div class="col-lg-4 col-md-6 col-12 panel-box">
            <div class="card-panel">
                <div class="card-panel-header hdr-kl2">
                    <span><i class="fas fa-bed mr-2"></i>KELAS II</span>
                    <span class="page-badge" id="badge-KL2">Hal 1/1</span>
                </div>
                <div class="table-container">
                    <table class="tbl-kiosk">
                        <thead>
                            <tr>
                                <th class="col-name">NAMA RUANGAN</th>
                                <th class="col-occupied">OCCUPIED</th>
                                <th class="col-ready">READY</th>
                            </tr>
                        </thead>
                        <tbody id="tbody-KL2" class="fade-in-rows"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div class="col-lg-4 col-md-6 col-12 panel-box">
            <div class="card-panel">
                <div class="card-panel-header hdr-kl3">
                    <span><i class="fas fa-bed mr-2"></i>KELAS III</span>
                    <span class="page-badge" id="badge-KL3">Hal 1/1</span>
                </div>
                <div class="table-container">
                    <table class="tbl-kiosk">
                        <thead>
                            <tr>
                                <th class="col-name">NAMA RUANGAN</th>
                                <th class="col-occupied">OCCUPIED</th>
                                <th class="col-ready">READY</th>
                            </tr>
                        </thead>
                        <tbody id="tbody-KL3" class="fade-in-rows"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- BARIS 2: KELAS VIP, KELAS VVIP, INTENSIVE CARE -->
        <div class="col-lg-4 col-md-6 col-12 panel-box">
            <div class="card-panel">
                <div class="card-panel-header hdr-vip">
                    <span><i class="fas fa-star mr-2"></i>KELAS VIP</span>
                    <span class="page-badge" id="badge-VIP">Hal 1/1</span>
                </div>
                <div class="table-container">
                    <table class="tbl-kiosk">
                        <thead>
                            <tr>
                                <th class="col-name">NAMA RUANGAN</th>
                                <th class="col-occupied">OCCUPIED</th>
                                <th class="col-ready">READY</th>
                            </tr>
                        </thead>
                        <tbody id="tbody-VIP" class="fade-in-rows"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div class="col-lg-4 col-md-6 col-12 panel-box">
            <div class="card-panel">
                <div class="card-panel-header hdr-vvp">
                    <span><i class="fas fa-crown mr-2"></i>KELAS VVIP</span>
                    <span class="page-badge" id="badge-VVP">Hal 1/1</span>
                </div>
                <div class="table-container">
                    <table class="tbl-kiosk">
                        <thead>
                            <tr>
                                <th class="col-name">NAMA RUANGAN</th>
                                <th class="col-occupied">OCCUPIED</th>
                                <th class="col-ready">READY</th>
                            </tr>
                        </thead>
                        <tbody id="tbody-VVP" class="fade-in-rows"></tbody>
                    </table>
                </div>
            </div>
        </div>

        <div class="col-lg-4 col-md-6 col-12 panel-box">
            <div class="card-panel">
                <div class="card-panel-header hdr-int">
                    <span><i class="fas fa-heartbeat mr-2"></i>INTENSIVE CARE</span>
                    <span class="page-badge" id="badge-INT">Hal 1/1</span>
                </div>
                <div class="table-container">
                    <table class="tbl-kiosk">
                        <thead>
                            <tr>
                                <th class="col-name">NAMA RUANGAN</th>
                                <th class="col-occupied">OCCUPIED</th>
                                <th class="col-ready">READY</th>
                            </tr>
                        </thead>
                        <tbody id="tbody-INT" class="fade-in-rows"></tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <form id="Form1" runat="server">
    </form>

    <script type="text/javascript">
        var PAGE_SIZE = 6;
        var ROTATE_INTERVAL_MS = 8000;
        var POLL_INTERVAL_MS = 30000;

        var categories = [
            {
                id: 'KL1',
                title: 'KELAS I',
                filter: function (d) { return d.BpjsClassID === 'KL1'; },
                page: 0,
                items: []
            },
            {
                id: 'KL2',
                title: 'KELAS II',
                filter: function (d) { return d.BpjsClassID === 'KL2'; },
                page: 0,
                items: []
            },
            {
                id: 'KL3',
                title: 'KELAS III',
                filter: function (d) { return d.BpjsClassID === 'KL3'; },
                page: 0,
                items: []
            },
            {
                id: 'VIP',
                title: 'KELAS VIP',
                filter: function (d) { return d.BpjsClassID === 'VIP'; },
                page: 0,
                items: []
            },
            {
                id: 'VVP',
                title: 'KELAS VVIP',
                filter: function (d) { return d.BpjsClassID === 'VVP'; },
                page: 0,
                items: []
            },
            {
                id: 'INT',
                title: 'INTENSIVE CARE',
                filter: function (d) {
                    return ['ICU', 'HCU', 'NIC', 'PIC', 'NICU', 'PICU', 'IMCA', 'IMC'].indexOf(d.BpjsClassID) !== -1;
                },
                page: 0,
                items: []
            }
        ];

        $(document).ready(function () {
            $.ajaxSetup({
                beforeSend: function (xhr) {
                    if (xhr.overrideMimeType) {
                        xhr.overrideMimeType("application/json");
                    }
                }
            });

            // Collapse sidebar untuk tampilan penuh monitor Kiosk
            $('body').removeClass("sidebar-mini");
            $('body').addClass("sidebar-collapse");

            updateClock();
            setInterval(updateClock, 1000);

            // Fetch data awal
            fetchBedData();

            // Setup polling data latar belakang
            setInterval(fetchBedData, POLL_INTERVAL_MS);

            // Setup perputaran rotasi halaman per panel
            setInterval(rotatePages, ROTATE_INTERVAL_MS);
        });

        function updateClock() {
            var now = new Date();
            var options = { weekday: 'long', year: 'numeric', month: 'short', day: 'numeric' };
            var dateStr = now.toLocaleDateString('id-ID', options);
            var timeStr = now.toLocaleTimeString('id-ID', { hour12: false });
            $('#liveClock').html('<i class="far fa-clock mr-2"></i>' + dateStr + ' ' + timeStr);
        }

        var _isFetching = false;
        function fetchBedData() {
            if (_isFetching) return;
            _isFetching = true;

            $.ajax({
                url: "<%=Helper.UrlRoot() %>/WebService/KioskQueue.asmx/BedMonitoringMjknGetList",
                type: "POST",
                dataType: "json",
                data: {},
                success: function (response) {
                    _isFetching = false;
                    var resData = response;
                    if (resData && resData.d !== undefined && resData.d !== null) {
                        resData = resData.d;
                    }
                    if (typeof resData === "string") {
                        try {
                            var cleanJson = resData.trim();
                            if (cleanJson.indexOf('}{') !== -1) {
                                cleanJson = cleanJson.substring(0, cleanJson.indexOf('}{') + 1);
                            }
                            resData = JSON.parse(cleanJson);
                        } catch (e) {
                            console.error("JSON parse error:", e, resData);
                        }
                    }

                    var rawList = [];
                    if (resData && resData.status === 'ERR') {
                        console.error("Server returned error:", resData.errorCode, resData.data);
                        return;
                    } else if (resData && resData.data && $.isArray(resData.data)) {
                        rawList = resData.data;
                    } else if ($.isArray(resData)) {
                        rawList = resData;
                    } else if (resData && typeof resData === 'object') {
                        for (var key in resData) {
                            if ($.isArray(resData[key])) {
                                rawList = resData[key];
                                break;
                            }
                        }
                    }

                    console.log("BedMonitoringMjkn loaded items:", rawList.length);
                    processData(rawList);
                },
                error: function (xhr, status, error) {
                    _isFetching = false;
                    console.error("fetchBedData error:", status, error, xhr.responseText);
                    if (xhr && xhr.responseText) {
                        try {
                            var text = xhr.responseText.trim();
                            if (text.indexOf('}{') !== -1) {
                                text = text.substring(0, text.indexOf('}{') + 1);
                            }
                            var parsed = JSON.parse(text);
                            if (parsed && parsed.data && $.isArray(parsed.data)) {
                                processData(parsed.data);
                            }
                        } catch (err) {
                            console.error("Fallback parse failed:", err);
                        }
                    }
                }
            });
        }

        function processData(list) {
            if (!list || !$.isArray(list)) return;

            // Normalkan data kamar
            var normalized = [];
            for (var i = 0; i < list.length; i++) {
                var row = list[i];
                var bpjsClass = (row.BpjsClassID || row.bpjsclassid || row.BPJSCLASSID || '').toString().trim().toUpperCase();
                var roomName = (row.ServiceUnitName || row.serviceunitname || row.SERVICEUNITNAME || row.RoomID || row.roomid || '-').toString().trim();
                var cap = parseInt(row.Capacity !== undefined ? row.Capacity : (row.capacity !== undefined ? row.capacity : 0), 10);
                if (isNaN(cap)) cap = 0;
                var avail = parseInt(row.Available !== undefined ? row.Available : (row.available !== undefined ? row.available : 0), 10);
                if (isNaN(avail)) avail = 0;

                var occ = cap - avail;
                if (occ < 0) occ = 0;

                normalized.push({
                    BpjsClassID: bpjsClass,
                    RoomID: row.RoomID || row.roomid || '',
                    ServiceUnitName: roomName,
                    Capacity: cap,
                    Occupied: occ,
                    Ready: avail
                });
            }

            // Pisahkan data per kategori
            for (var c = 0; c < categories.length; c++) {
                var cat = categories[c];
                var filtered = [];
                for (var j = 0; j < normalized.length; j++) {
                    if (cat.filter(normalized[j])) {
                        filtered.push(normalized[j]);
                    }
                }
                cat.items = filtered;

                var totalPages = Math.ceil(cat.items.length / PAGE_SIZE) || 1;
                if (cat.page >= totalPages) {
                    cat.page = 0;
                }

                renderCategory(cat);
            }
        }

        function rotatePages() {
            for (var c = 0; c < categories.length; c++) {
                var cat = categories[c];
                var totalPages = Math.ceil(cat.items.length / PAGE_SIZE) || 1;
                if (totalPages > 1) {
                    cat.page = (cat.page + 1) % totalPages;
                    renderCategory(cat);
                }
            }
        }

        function renderCategory(cat) {
            var tbody = $('#tbody-' + cat.id);
            var badge = $('#badge-' + cat.id);

            var totalPages = Math.ceil(cat.items.length / PAGE_SIZE) || 1;
            var curPage = cat.page + 1;
            badge.text('Hal ' + curPage + '/' + totalPages);

            var startIndex = cat.page * PAGE_SIZE;
            var pageItems = cat.items.slice(startIndex, startIndex + PAGE_SIZE);

            var html = '';
            for (var i = 0; i < pageItems.length; i++) {
                var item = pageItems[i];
                var rowClass = (item.Ready === 0) ? 'row-full' : '';

                html += '<tr class="' + rowClass + '">';
                html += '<td class="col-name" title="' + escapeHtml(item.ServiceUnitName) + '">' + escapeHtml(item.ServiceUnitName) + '</td>';
                html += '<td class="col-occupied">' + item.Occupied + '</td>';
                html += '<td class="col-ready">' + item.Ready + '</td>';
                html += '</tr>';
            }

            // Jika item kurang dari PAGE_SIZE, isi slot kosong transparan agar tinggi tabel tetap stabil
            var emptySlots = PAGE_SIZE - pageItems.length;
            for (var k = 0; k < emptySlots; k++) {
                html += '<tr class="row-empty-slot">';
                html += '<td class="col-name">-</td>';
                html += '<td class="col-occupied">-</td>';
                html += '<td class="col-ready">-</td>';
                html += '</tr>';
            }

            tbody.removeClass('fade-in-rows');
            // Trigger reflow untuk me-restart animasi CSS
            void tbody[0].offsetWidth;
            tbody.addClass('fade-in-rows');
            tbody.html(html);
        }

        function escapeHtml(text) {
            if (!text) return '';
            return $('<div>').text(text).html();
        }
    </script>
</asp:Content>
