<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDialog.Master" AutoEventWireup="true"
    CodeBehind="IntegratedNoteHist.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Emr.IntegratedNoteHist" %>

<%@ Import Namespace="Temiang.Avicenna.Common" %>
<%@ Import Namespace="Temiang.Avicenna.BusinessObject" %>
<%@ Import Namespace="Temiang.Avicenna.Module.RADT.EmrIp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadGrid ID="grdAssessment" runat="server" EnableViewState="False" Height="800px"
        OnNeedDataSource="grdAssessment_NeedDataSource"
        AutoGenerateColumns="False" GridLines="None">
        <MasterTableView DataKeyNames="RegistrationInfoMedicID"
            ShowHeader="True"
            ShowGroupFooter="True"
            HierarchyDefaultExpanded="True" AllowFilteringByColumn="True">
            <NestedViewTemplate>
                <table width="100%">
                    <tr>
                        <td style="width: 15px; vertical-align: top;">
                            <%# EmrIpDetail.IntegratedNoteVerifPrintEditLink(Container, IsUserParamedicDpjp())%>
                            <asp:LinkButton ID="lblDelete" runat="server" CommandName="Delete" ToolTip="Void"
                                CommandArgument='<%#string.Format("{0}_{1}", DataBinder.Eval(Container.DataItem, "RegistrationInfoMedicID"),DataBinder.Eval(Container.DataItem, "IsFromAskep"))%>'
                                Visible='<%#EmrIpDetail.IntegratedNoteDeleteable(Container) %>'
                                OnClientClick="javascript: if (!confirm('Are you sure void this ?')) return false;">
                                                            <img style="border: 0px; vertical-align: middle;" src="../../../Images/Toolbar/row_delete16.png" alt=""/>
                            </asp:LinkButton>

                            <asp:LinkButton ID="lblUnDelete" runat="server" CommandName="Delete" ToolTip="Unvoid"
                                CommandArgument='<%#string.Format("{0}_{1}", DataBinder.Eval(Container.DataItem, "RegistrationInfoMedicID"),DataBinder.Eval(Container.DataItem, "IsFromAskep"))%>'
                                Visible='<%#EmrIpDetail.IntegratedNoteUnDeleteable(Container) %>'
                                OnClientClick="javascript: if (!confirm('Are you sure unvoid this ?')) return false;">
                                                            <img style="border: 0px; vertical-align: middle;" src="../../../Images/Toolbar/refresh16.png" alt=""/>
                            </asp:LinkButton>
                        </td>

                        <td style="vertical-align: top;">
                            <%#EmrIpDetail.IntegratedNoteScript(Container)%>
                            <%#EmrIpDetail.AdditionalNoteScript(Container)%>
                        </td>
                    </tr>
                </table>
            </NestedViewTemplate>
            <CommandItemStyle Height="29px" />
            <Columns>
                <telerik:GridBoundColumn DataField="DatetimeInfoStr" UniqueName="DatetimeInfoStr" HeaderText="Time" HeaderStyle-Width="150px" ItemStyle-Font-Bold="True"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ParamedicName" UniqueName="ParamedicName" HeaderText="Physician" HeaderStyle-Width="170px"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CreatedByUserName" UniqueName="CreatedByUserName" HeaderText="Create By" HeaderStyle-Width="170px"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="SRUserType" UniqueName="SRUserType" HeaderText="PPA" HeaderStyle-Width="120px">
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="RegistrationNo" UniqueName="RegistrationNo" HeaderText="Registration No" HeaderStyle-Width="170px"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ServiceUnitName" UniqueName="ServiceUnitName" HeaderText="Service Unit"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AssessmentTypeName" UniqueName="AssessmentTypeName" HeaderText="Assessment" HeaderStyle-Width="170px"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="SRMedicalNotesInputType" UniqueName="SRMedicalNotesInputType" HeaderText="Notes Type" HeaderStyle-Width="120px"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IsInitialAssessment" UniqueName="IsInitialAssessment" Display="False"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn AllowFiltering="False" />
            </Columns>
        </MasterTableView>
        <ClientSettings EnableRowHoverStyle="False">
            <Selecting AllowRowSelect="false" />
            <Scrolling AllowScroll="True" UseStaticHeaders="True" />
        </ClientSettings>
        <GroupingSettings ShowUnGroupButton="False" />
    </telerik:RadGrid>

</asp:Content>
