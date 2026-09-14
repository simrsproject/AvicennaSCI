<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master"
    CodeBehind="PpraHistoryPopup.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Ppra.PpraHistoryPopup" %>
<%@ Import Namespace="Temiang.Avicenna.Common" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="padding:8px;">
        <div style="background-color:#1a3c5e;color:#fff;padding:6px 10px;font-weight:bold;margin-bottom:8px;">
            <asp:Literal runat="server" ID="litHeader" />
        </div>
        <asp:Literal runat="server" ID="litHistory" />
    </div>

</asp:Content>
