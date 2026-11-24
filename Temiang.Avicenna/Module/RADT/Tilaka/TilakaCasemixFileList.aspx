<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaCasemixFileList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaCasemixFileList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="layout">
        <tr>
            <td class="label">Registration No</td>
            <td><telerik:RadTextBox ID="txtRegistrationNo" runat="server" Width="200px" /></td>
            <td class="label">Dokter</td>
            <td><telerik:RadTextBox ID="txtParamedicID" runat="server" Width="150px" /></td>
            <td><asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_OnClick" /></td>
        </tr>
    </table>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="CasemixFileID" ClientDataKeyNames="CasemixFileID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="RegistrationNo" HeaderText="Registration No" UniqueName="RegistrationNo" />
                <telerik:GridBoundColumn DataField="ParamedicID" HeaderText="Dokter" UniqueName="ParamedicID" />
                <telerik:GridBoundColumn DataField="FileName" HeaderText="File Name" UniqueName="FileName" />
                <telerik:GridBoundColumn DataField="FilePath" HeaderText="File Path" UniqueName="FilePath" />
                <telerik:GridBoundColumn DataField="RequestStatus" HeaderText="Status" UniqueName="RequestStatus" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnProcess" runat="server" Text="Process Signature"
                            CommandName="process" CommandArgument='<%# Eval("CasemixFileID") %>'
                            Enabled='<%# Eval("CanProcess") %>'
                            ToolTip='<%# Convert.ToBoolean(Eval("CanProcess")) ? "" : "Dokter tidak menggunakan tanda tangan elektronik atau request masih berjalan." %>' />
                        <asp:LinkButton ID="btnStatus" runat="server" Text="Status"
                            CommandName="status" CommandArgument='<%# Eval("RequestID") %>'
                            Visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("RequestID"))) %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
