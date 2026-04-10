<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="ImmunizationList.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Master.ImmunizationList" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadGrid ID="grdList" runat="server" OnNeedDataSource="grdList_NeedDataSource">
        <MasterTableView DataKeyNames="ImmunizationID">
            <Columns>
                <telerik:GridBoundColumn HeaderStyle-Width="100px" DataField="ImmunizationID" HeaderText="ID"
                    UniqueName="ImmunizationID" SortExpression="ImmunizationID" HeaderStyle-HorizontalAlign="Left"
                    ItemStyle-HorizontalAlign="Left" />
                <telerik:GridBoundColumn DataField="ImmunizationName" HeaderText="Immunization Name" UniqueName="ImmunizationName"
                    SortExpression="ImmunizationName" HeaderStyle-Width="200px" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                <telerik:GridBoundColumn HeaderStyle-Width="200px" DataField="MaxCount" HeaderText="Maximum Number of Immunizations"
                    UniqueName="MaxCount" SortExpression="MaxCount" HeaderStyle-HorizontalAlign="Left"
                    ItemStyle-HorizontalAlign="Left" />
                <telerik:GridBoundColumn DataField="IndexNo" HeaderText="Seq No"
                    UniqueName="IndexNo" SortExpression="IndexNo" HeaderStyle-HorizontalAlign="Left"
                    ItemStyle-HorizontalAlign="Left" />
                <telerik:GridTemplateColumn />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
