<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true"
    CodeBehind="TilakaTemplateSignFileDetail.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaTemplateSignFileDetail" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hfTemplateSignFileID" runat="server" />

    <table width="100%">
        <tr>
            <td class="label">Dokter</td>
            <td class="entry">
                <telerik:RadComboBox ID="cboParamedicID" runat="server" Width="400px"
                    AllowCustomText="false" EnableLoadOnDemand="true" Filter="Contains"
                    OnItemsRequested="cboParamedicID_ItemsRequested" />
            </td>
        </tr>
        <tr>
            <td class="label">Template No</td>
            <td class="entry"><asp:Label ID="lblTemplateNo" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">File Template</td>
            <td class="entry">
                <telerik:RadAsyncUpload ID="uplTemplate" runat="server" ControlObjectsVisibility="None"
                    AllowedFileExtensions=".pdf" MaxFileSize="20971520" />
                <asp:Label ID="lblFileName" runat="server" />
                <asp:HyperLink ID="lnkDownload" runat="server" Text="Download PDF"
                    Target="_blank" Visible="false" />
            </td>
        </tr>
        <tr>
            <td class="label">Ukuran File</td>
            <td class="entry"><asp:Label ID="lblFileSize" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Is Default</td>
            <td class="entry">
                <asp:CheckBox ID="chkIsDefault" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label">Is Active</td>
            <td class="entry">
                <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
            </td>
        </tr>
        <tr>
            <td class="label">Status</td>
            <td class="entry"><asp:Label ID="lblRegistered" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Tanggal Daftar</td>
            <td class="entry"><asp:Label ID="lblRegisteredDateTime" runat="server" /></td>
        </tr>
        <tr>
            <td class="label">Error</td>
            <td class="entry"><asp:Label ID="lblError" runat="server" /></td>
        </tr>
    </table>

    <br />
    <asp:Literal ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
