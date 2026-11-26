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
            <telerik:AjaxSetting AjaxControlID="btnFilterCertificateStatus">
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
                            <td class="label">Quick Sign Status</td>
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
                        <tr>
                            <td class="label">Sertifikat Status</td>
                            <td class="entry">
                                <telerik:RadComboBox ID="cboCertificateStatus" runat="server" Width="300px" AllowCustomText="false">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="(Semua)" Value="" />
                                        <telerik:RadComboBoxItem Text="0 - Belum Terdaftar" Value="0" />
                                        <telerik:RadComboBoxItem Text="1 - Menunggu KYC" Value="1" />
                                        <telerik:RadComboBoxItem Text="2 - Menunggu Verifikasi" Value="2" />
                                        <telerik:RadComboBoxItem Text="3 - Sertifikat Terbit" Value="3" />
                                        <telerik:RadComboBoxItem Text="4 - Aktif" Value="4" />
                                        <telerik:RadComboBoxItem Text="5 - KYC Gagal" Value="5" />
                                        <telerik:RadComboBoxItem Text="6 - Ditolak" Value="6" />
                                        <telerik:RadComboBoxItem Text="7 - Dicabut" Value="7" />
                                        <telerik:RadComboBoxItem Text="8 - Kedaluwarsa" Value="8" />
                                        <telerik:RadComboBoxItem Text="9 - UUID Terbit" Value="9" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterCertificateStatus" runat="server"
                                    ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </cc:CollapsePanel>

    <div style="text-align: right;">
        <asp:Literal ID="lblMessage" runat="server" EnableViewState="false" />
    </div>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="ParamedicID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="ParamedicID" HeaderText="ID" UniqueName="ParamedicID" />
                <telerik:GridBoundColumn DataField="ParamedicName" HeaderText="Dokter" UniqueName="ParamedicName" />
                <telerik:GridBoundColumn DataField="Email" HeaderText="Email" UniqueName="Email" />
                <telerik:GridBoundColumn DataField="TilakaName" HeaderText="Tilaka Name" UniqueName="TilakaName" />
                <telerik:GridTemplateColumn HeaderText="Sertifikat Status" UniqueName="CertificateStatusText">
                    <ItemTemplate>
                        <span style='<%# "color:" + Eval("CertificateStatusColor") + ";" %>'><%# Eval("CertificateStatusText") %></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Quick Sign" UniqueName="QuickSignStatus">
                    <ItemTemplate>
                        <span style='<%# "color:" + Eval("QuickSignStatusColor") + ";" %>'><%# Eval("QuickSignStatus") %></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
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
</asp:Content>
