<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaQuickSignUserList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaQuickSignUserList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="cc" Namespace="Temiang.Avicenna.CustomControl" Assembly="Temiang.Avicenna" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnFilterParamedic">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                    <telerik:AjaxUpdatedControl ControlID="lblMessage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>

    <cc:CollapsePanel ID="CollapsePanel1" runat="server" Title="Search Filter">
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                            <td class="label">Dokter</td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtParamedicName" runat="server" Width="300px" />
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterParamedic" runat="server"
                                    ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                            <td class="label">Status</td>
                            <td class="entry">
                                <telerik:RadComboBox ID="cboStatus" runat="server" Width="300px" AllowCustomText="false">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="(Semua)" Value="" />
                                        <telerik:RadComboBoxItem Text="Belum terdaftar" Value="0" />
                                        <telerik:RadComboBoxItem Text="Terdaftar" Value="1" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterStatus" runat="server"
                                    ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </cc:CollapsePanel>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="ParamedicID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="ParamedicID" HeaderText="ID" UniqueName="ParamedicID" />
                <telerik:GridBoundColumn DataField="ParamedicName" HeaderText="Dokter" UniqueName="ParamedicName" />
                <telerik:GridBoundColumn DataField="Email" HeaderText="Email" UniqueName="Email" />
                <telerik:GridBoundColumn DataField="TilakaName" HeaderText="Tilaka Name" UniqueName="TilakaName" />
                <telerik:GridBoundColumn DataField="CertificateStatus" HeaderText="Sertifikat Status" UniqueName="CertificateStatus" />
                <telerik:GridBoundColumn DataField="QuickSignStatus" HeaderText="Quick Sign" UniqueName="QuickSignStatus" />
                <telerik:GridBoundColumn DataField="QuickSignRegisteredDateTime" HeaderText="Register Time"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" UniqueName="QuickSignRegisteredDateTime" />
                <telerik:GridBoundColumn DataField="QuickSignLastErrorMessage" HeaderText="Error" UniqueName="QuickSignLastErrorMessage" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnRegister" runat="server" Text="Daftarkan Quick Sign"
                            CommandName="register" CommandArgument='<%# Eval("ParamedicID") %>'
                            Visible='<%# Eval("CanRegister") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <asp:Literal ID="lblMessage" runat="server" EnableViewState="false" />
</asp:Content>
