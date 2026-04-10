<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master" AutoEventWireup="true"
    CodeBehind="ImmunizationHist.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Emr.ImmunizationHist" %>

<%@ Import Namespace="Temiang.Avicenna.Common" %>
<%@ Register Src="~/Module/RADT/Cpoe/EmrCommon/Assessment/AssessmentCtl/Initial/Kid/ImunizationHistCtl.ascx" TagPrefix="uc1" TagName="ImunizationHistCtl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock ID="radCodeBlock" runat="server">
        <script type="text/javascript" language="javascript">
            function showResumeMedis(regno, patientID, isRichTextMode) {
                var url = '<%= Helper.UrlRoot() %>/Module/RADT/EmrIp/EmrIpCommon/ResumeMedis/ResumeMedisInPatientEntry.aspx?mod=view&editable=false&regno=' + regno + '&fregno=&patid=' + patientID + '&parid=';
                if (isRichTextMode=== true)
                    url = '<%= Helper.UrlRoot() %>/Module/RADT/EmrIp/EmrIpCommon/ResumeMedis/ResumeMedisRichTextInPatientEntry.aspx?mod=view&editable=false&regno=' + regno + '&fregno=&patid=' + patientID + '&parid=';

                openWindow(url, 1000, 600);
            }
            function openWindow(url, width, height) {
                var oWnd;
                oWnd = radopen(url, 'winDialog');
                oWnd.setSize(width, height);
                oWnd.center();

                // Cek position
                var pos = oWnd.getWindowBounds();
                if (pos.y < 0)
                    oWnd.moveTo(pos.x, 0);
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindowManager ID="radWindowManager" runat="server" Style="z-index: 7001"
        Modal="true" VisibleStatusbar="false" DestroyOnClose="false" Behavior="Close,Move"
        ReloadOnShow="True" ShowContentDuringLoad="false">
        <Windows>
            <telerik:RadWindow ID="winDialog" Width="900px" Height="600px" runat="server"
                ShowContentDuringLoad="false" Behaviors="Close,Move" Modal="True">
            </telerik:RadWindow>

        </Windows>
    </telerik:RadWindowManager>
    <uc1:ImunizationHistCtl runat="server" ID="immunizationHistCtl" />
</asp:Content>
