<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true"
    CodeBehind="TilakaSigningDetail.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaSigningDetail" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hfRequestID" runat="server" />

    <table class="layout">
        <tr><td class="label">Request ID</td><td><asp:Label ID="lblRequestID" runat="server" /></td><td width="20"></td><td></td></tr>
        <tr><td class="label">Casemix ID</td><td><asp:Label ID="lblCasemixFileID" runat="server" /></td><td width="20"></td><td></td></tr>
        <tr><td class="label">File</td><td>
            <asp:Label ID="lblFileName" runat="server" />
            <asp:HyperLink ID="lnkCasemix" runat="server" Text="[pratinjau]" Target="_blank" />
        </td><td width="20"></td><td></td></tr>
        <tr><td class="label">Template</td><td><asp:Label ID="lblTemplateNo" runat="server" /></td><td width="20"></td><td></td></tr>
        <tr><td class="label">Status</td><td><asp:Label ID="lblStatus" runat="server" /></td><td width="20"></td><td></td></tr>
        <tr><td class="label">Error</td><td><asp:Label ID="lblError" runat="server" /></td><td width="20"></td><td></td></tr>
        <tr><td class="label">Dokumen bertanda tangan</td><td>
            <asp:HyperLink ID="lnkSignedFile" runat="server" Text="Unduh PDF" Target="_blank" Visible="false" />
        </td><td width="20"></td><td></td></tr>
    </table>

    <telerik:RadGrid ID="grdSigner" runat="server" AllowPaging="false"
        OnNeedDataSource="grdSigner_NeedDataSource">
        <MasterTableView AutoGenerateColumns="false" DataKeyNames="QuickSignSignerID">
            <Columns>
                <telerik:GridBoundColumn DataField="Sequence" HeaderText="Seq" UniqueName="Sequence" />
                <telerik:GridBoundColumn DataField="UserIdentifier" HeaderText="Tilaka Name" UniqueName="UserIdentifier" />
                <telerik:GridBoundColumn DataField="Email" HeaderText="Email" UniqueName="Email" />
                <telerik:GridBoundColumn DataField="Status" HeaderText="Status" UniqueName="Status" />
                <telerik:GridBoundColumn DataField="NumSignatures" HeaderText="Signatures" UniqueName="NumSignatures" />
                <telerik:GridBoundColumn DataField="NumSignaturesDone" HeaderText="Done" UniqueName="NumSignaturesDone" />
                <telerik:GridTemplateColumn HeaderText="Auth" UniqueName="Auth">
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkAuth" runat="server" Text="Buka Auth" Target="_blank"
                            NavigateUrl='<%# Eval("AuthUrl") %>'
                            Visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("AuthUrl"))) %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="ErrorMessage" HeaderText="Error" UniqueName="ErrorMessage" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <br />
    <asp:Button ID="btnCheck" runat="server" Text="Cek Status" OnClick="btnCheck_OnClick" />
    <asp:Literal ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
