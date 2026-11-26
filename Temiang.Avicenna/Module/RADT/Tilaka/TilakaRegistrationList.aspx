<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterList.Master" AutoEventWireup="true"
    CodeBehind="TilakaRegistrationList.aspx.cs"
    Inherits="Temiang.Avicenna.Module.RADT.Tilaka.TilakaRegistrationList" %>
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
            <telerik:AjaxSetting AjaxControlID="btnFilterState">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnReconcile">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
                    <telerik:AjaxUpdatedControl ControlID="lblMessage" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" LoadingPanelID="fw_ajxLoadingPanel" />
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
                            <td class="label">Dokter (ID/Nama)</td>
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
                            <td class="label">Sertifikat Status</td>
                            <td class="entry">
                                <telerik:RadComboBox ID="cboState" runat="server" Width="300px" AllowCustomText="false">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="(Semua)" Value="" />
                                        <telerik:RadComboBoxItem Text="0 - Belum Terdaftar" Value="BelumTerdaftar" />
                                        <telerik:RadComboBoxItem Text="1 - Menunggu KYC" Value="MenungguKyc" />
                                        <telerik:RadComboBoxItem Text="2 - Menunggu Verifikasi" Value="MenungguVerifikasi" />
                                        <telerik:RadComboBoxItem Text="3 - Sertifikat Terbit" Value="SertifikatTerbit" />
                                        <telerik:RadComboBoxItem Text="4 - Aktif" Value="Aktif" />
                                        <telerik:RadComboBoxItem Text="5 - KYC Gagal" Value="KycGagal" />
                                        <telerik:RadComboBoxItem Text="6 - Ditolak" Value="Ditolak" />
                                        <telerik:RadComboBoxItem Text="7 - Dicabut" Value="Dicabut" />
                                        <telerik:RadComboBoxItem Text="8 - Kedaluwarsa" Value="Kedaluwarsa" />
                                        <telerik:RadComboBoxItem Text="9 - UUID Terbit" Value="UuidTerbit" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td style="text-align: left">
                                <asp:ImageButton ID="btnFilterState" runat="server"
                                    ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </cc:CollapsePanel>

    <table width="100%" cellpadding="0" cellspacing="10">
        <tr>
            <td style="padding: 4px 0 6px 0;">
                <asp:Button ID="btnReconcile" runat="server" Text="Cek Status Massal"
                    OnClick="btnReconcile_OnClick" />
                <asp:Literal ID="lblMessage" runat="server" EnableViewState="false" />
            </td>
        </tr>
    </table>

    <telerik:RadGrid ID="grdList" runat="server" AllowPaging="True" PageSize="20"
        OnNeedDataSource="grdList_NeedDataSource" OnItemCommand="grdList_ItemCommand">
        <MasterTableView DataKeyNames="ParamedicID" ClientDataKeyNames="ParamedicID" AutoGenerateColumns="false">
            <Columns>
                <telerik:GridBoundColumn DataField="ParamedicID" HeaderText="ID" UniqueName="ParamedicID" />
                <telerik:GridBoundColumn DataField="ParamedicName" HeaderText="Dokter" UniqueName="ParamedicName" />
                <telerik:GridBoundColumn DataField="NamaKTP" HeaderText="Nama KTP" UniqueName="NamaKTP" />
                <telerik:GridTemplateColumn HeaderText="Sertifikat Status" UniqueName="StatusText">
                    <ItemTemplate>
                        <span style='<%# "color:" + Eval("StatusColor") + ";" %>'><%# Eval("StatusText") %></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="TilakaName" HeaderText="Tilaka Name" UniqueName="TilakaName" />
                <telerik:GridBoundColumn DataField="LastCheckedDateTime" HeaderText="Cek Terakhir"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" UniqueName="LastCheckedDateTime" />
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnDetail" runat="server" Text="Detail"
                            CommandName="detail" CommandArgument='<%# Eval("ParamedicID") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
