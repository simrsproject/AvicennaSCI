<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterCustomNoMenu.Master" AutoEventWireup="true"
    CodeBehind="TilakaWebviewReturn.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaWebviewReturn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="padding: 24px; font-family: Arial, sans-serif;">
        <h3>Tilaka</h3>
        <asp:Label ID="lblMessage" runat="server" />
        <p><a href="javascript:window.close();">Tutup halaman ini</a></p>
    </div>
</asp:Content>
