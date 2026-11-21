<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaSigningList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaSigningList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="layout">
        <tr>
            <td class="label">Request ID</td>
            <td><telerik:RadTextBox ID="txtRequestID" runat="server" Width="250px" /></td>
            <td class="label">Status</td>
            <td>
                <telerik:RadComboBox ID="cboStatus" runat="server" Width="180px" AllowCustomText="false">
                    <Items>
                        <telerik:RadComboBoxItem Text="(Semua)" Value="" />
                        <telerik:RadComboBoxItem Text="Requested" Value="Requested" />
                        <telerik:RadComboBoxItem Text="InProgress" Value="InProgress" />
                        <telerik:RadComboBoxItem Text="Completed" Value="Completed" />
                        <telerik:RadComboBoxItem Text="Failed" Value="Failed" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_OnClick" /></td>
        </tr>
    </table>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="RequestID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="RequestID" HeaderText="Request ID" UniqueName="RequestID" />
                <telerik:GridBoundColumn DataField="CasemixFileID" HeaderText="Casemix ID" UniqueName="CasemixFileID" />
                <telerik:GridBoundColumn DataField="FileName" HeaderText="File" UniqueName="FileName" />
                <telerik:GridBoundColumn DataField="TemplateNo" HeaderText="Template" UniqueName="TemplateNo" />
                <telerik:GridBoundColumn DataField="Status" HeaderText="Status" UniqueName="Status" />
                <telerik:GridBoundColumn DataField="RequestDateTime" HeaderText="Tanggal"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" UniqueName="RequestDateTime" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDetail" runat="server" Text="Detail"
                            CommandName="detail" CommandArgument='<%# Eval("RequestID") %>' />
                        <asp:HyperLink ID="lnkDownload" runat="server" Text="Unduh" Target="_blank"
                            NavigateUrl='<%# "TilakaSignedFileHandler.ashx?id=" + Server.UrlEncode(Convert.ToString(Eval("RequestID"))) %>'
                            Visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("SignedFilePath"))) %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
