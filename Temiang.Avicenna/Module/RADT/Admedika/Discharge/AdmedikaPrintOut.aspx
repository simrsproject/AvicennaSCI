<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdmedikaPrintOut.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Admedika.AdmedikaPrintOut" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>AdMedika Print Out</title>
    <style type="text/css">
        body { font-family: Arial, sans-serif; color: #111; margin: 0; background: #eee; }
        .toolbar { padding: 10px; text-align: center; }
        .sheet { width: 420px; margin: 0 auto 20px; padding: 20px; background: #fff; border: 1px solid #bbb; }
        .logo { display: block; width: 180px; height: auto; margin: 0 auto 12px; }
        .title { text-align: center; font-weight: bold; margin-bottom: 10px; }
        .meta { width: 100%; font-size: 12px; border-collapse: collapse; margin-bottom: 10px; }
        .meta td { padding: 2px; vertical-align: top; }
        .meta td:first-child { width: 105px; font-weight: bold; }
        pre { font-family: "Courier New", monospace; font-size: 12px; line-height: 1.35;
              white-space: pre-wrap; word-wrap: break-word; border-top: 1px dashed #777;
              padding-top: 10px; margin: 0; }
        @media print {
            body { background: #fff; }
            .toolbar { display: none; }
            .sheet { border: 0; margin: 0 auto; padding: 0; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="toolbar">
            <button type="button" onclick="window.print();">Print</button>
            <button type="button" onclick="window.close();">Close</button>
        </div>
        <div class="sheet">
            <img class="logo" src="../../../../Images/Admedika/logo.png" alt="AdMedika" />
            <div class="title"><asp:Label ID="lblTitle" runat="server" /></div>
            <table class="meta">
                <tr><td>Reference ID</td><td>: <asp:Label ID="lblClaimID" runat="server" /></td></tr>
                <tr><td>Registration No</td><td>: <asp:Label ID="lblRegistrationNo" runat="server" /></td></tr>
                <tr><td>Patient Name</td><td>: <asp:Label ID="lblPatientName" runat="server" /></td></tr>
                <tr><td>Card No</td><td>: <asp:Label ID="lblCardNo" runat="server" /></td></tr>
            </table>
            <asp:Literal ID="litPrintOut" runat="server" />
        </div>
    </form>
</body>
</html>
