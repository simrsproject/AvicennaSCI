<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true" CodeBehind="RujukanSatuSehatDetail.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Bpjs.RujukanSatuSehatDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../../../../JavaScript/jquery.js"></script>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function OnClientItemsRequestingHandler(sender, eventArgs) {
                if (sender.get_text().length < 3) eventArgs.set_cancel(true);
                else {
                    var context = eventArgs.get_context();
                    context["filter"] = eventArgs.get_text();
                }
            }

            function openWinSearchSep() {
                var jp = $find("<%= txtNoSep.ClientID %>");
                if (jp.get_value() == '') {
                    alert('No SEP pasien belum diisi.');
                    return;
                }

                var oWnd = $find("<%= winRujukan.ClientID %>");
                oWnd.setUrl("../../Registration/BPJSRegistrationInfo.aspx?type=ruj&sep=" + jp.get_value());
                oWnd.set_title('Avicenna - BPJS Info');
                oWnd.set_width($(window).width());
                oWnd.show();
            }

            function onClientClose(oWnd, args) {
                if (oWnd.argument && oWnd.argument.sep != null) {
                    if (oWnd.argument.sep != '') {
                        var jp = $find("<%= txtNoSep.ClientID %>");
                        jp.set_value = oWnd.argument.sep;
                        __doPostBack("<%= txtNoSep.UniqueID %>", "rebind");
                    }
                }
            }
        </script>

    </telerik:RadCodeBlock>
    <telerik:RadWindow runat="server" Animation="None" Behavior="Close, Move" ShowContentDuringLoad="False"
        Width="750px" Height="420px" VisibleStatusbar="False" Modal="true" ID="winRujukan"
        OnClientClose="onClientClose" />
    <table>
        <tr>
            <td class="label">No Rujukan</td>
            <td class="entry">
                <telerik:RadTextBox ID="txtNoRujukan" runat="server" ReadOnly="true" Width="300px" />
            </td>
            <td width="20px" />
            <td />
        </tr>
        <tr>
            <td class="label">No Rujukan SatuSehat</td>
            <td class="entry">
                <telerik:RadTextBox ID="txtNoRujukanSS" runat="server" ReadOnly="true" Width="300px" />
            </td>
            <td width="20px" />
            <td />
        </tr>
        <tr>
            <td class="label">No SEP</td>
            <td class="entry">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>

                            <telerik:RadTextBox ID="txtNoSep" runat="server" Width="300px" />
                        </td>
                        <td>&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:ImageButton ID="btnCariPasien" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                OnClientClick="openWinSearchSep(); return false;" ToolTip="Cari Sep" />
                        </td>
                    </tr>
                </table>
            </td>
            <td width="20px">
                <asp:RequiredFieldValidator ID="rfvNoSep" runat="server" ErrorMessage="No SEP required."
                    ValidationGroup="entry" ControlToValidate="txtNoSep" SetFocusOnError="True" Width="100%">
                    <asp:Image ID="Image1" runat="server" SkinID="rfvImage" />
                </asp:RequiredFieldValidator>
            </td>
            <td />
        </tr>
        <tr>
            <td class="label">Tgl. SEP</td>
            <td class="entry">
                <telerik:RadDatePicker ID="txtTglSep" runat="server" Width="100px" DateInput-ReadOnly="true" DatePopupButton-Enabled="false" />
            </td>
            <td width="20px" />
            <td />
        </tr>
        <tr>
            <td class="label">Tgl. Rujukan</td>
            <td class="entry">
                <telerik:RadDatePicker ID="txtTglRujukan" runat="server" Width="100px" />
            </td>
            <td width="20px">
                <asp:RequiredFieldValidator ID="rfvTglRujukan" runat="server" ErrorMessage="Tgl rujukan required."
                    ValidationGroup="entry" ControlToValidate="txtTglRujukan" SetFocusOnError="True" Width="100%">
                    <asp:Image ID="Image2" runat="server" SkinID="rfvImage" />
                </asp:RequiredFieldValidator>
            </td>
            <td />
        </tr>
        <tr>
            <td class="label">Tgl. Rencana</td>
            <td class="entry">
                <telerik:RadDatePicker ID="txtTglRencanaKunjungan" runat="server" Width="100px" />
            </td>
            <td width="20px">
                <asp:RequiredFieldValidator ID="rfvTglRencanaKunjungan" runat="server" ErrorMessage="Tgl rencana required."
                    ValidationGroup="entry" ControlToValidate="txtTglRencanaKunjungan" SetFocusOnError="True" Width="100%">
                    <asp:Image ID="Image8" runat="server" SkinID="rfvImage" />
                </asp:RequiredFieldValidator>
            </td>
            <td />
        </tr>
        <tr>
            <td class="label">No Peserta</td>
            <td class="entry">
                <telerik:RadTextBox ID="txtNoPeserta" runat="server" ReadOnly="true" Width="300px" />
            </td>
            <td width="20px" />
            <td />
        </tr>
        <tr>
            <td class="label">Nama Peserta</td>
            <td class="entry">
                <telerik:RadTextBox ID="txtNamaPeserta" runat="server" ReadOnly="true" Width="300px" />
            </td>
            <td width="20px" />
            <td />
        </tr>
        <tr>
            <td class="label">Diagnosa Dirujuk
            </td>
            <td class="entry">
                <telerik:RadComboBox
                    ID="cboDiagnosa"
                    runat="server"
                    Width="304px"
                    AllowCustomText="true"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="cboDiagnosa_SelectedIndexChanged"
                    OnItemDataBound="cboDiagnosaSep_ItemDataBound"
                    EnableLoadOnDemand="true"
                    OnClientItemsRequesting="OnClientItemsRequestingHandler">
                    <WebServiceSettings Path="../../../../WebService/Sep.asmx" Method="GetDiagnosa" />
                </telerik:RadComboBox>
            </td>
            <td width="20px">
                <asp:RequiredFieldValidator ID="rfvDiagnosa" runat="server" ErrorMessage="Diagnosa dirujuk required."
                    ValidationGroup="entry" ControlToValidate="cboDiagnosa" SetFocusOnError="True" Width="100%">
                    <asp:Image ID="Image7" runat="server" SkinID="rfvImage" />
                </asp:RequiredFieldValidator>
            </td>
            <td />
        </tr>
        <tr>
            <td class="label">Kriteria Rujukan</td>
            <td class="entry">
                <asp:Repeater ID="rptKriteria" runat="server">
                    <ItemTemplate>

                        <asp:HiddenField ID="hfLinkId"
                            runat="server"
                            Value='<%# Eval("LinkId") %>' />
                        <asp:Label ID="lblText"
                            runat="server"
                            Text='<%# Eval("Text") %>'
                            Style="display:none;" />

                        <telerik:RadComboBox
                            ID="cboKriteria"
                            runat="server"
                            Width="304px"
                            AllowCustomText="true"
                            EnableLoadOnDemand="true"
                            OnItemDataBound="cboDiagnosaSep_ItemDataBound"
                            OnClientItemsRequesting="OnClientItemsRequestingHandler">

                            <WebServiceSettings
                                Path="../../../../WebService/Sep.asmx"
                                Method="GetProcedureIdrg" />

                        </telerik:RadComboBox>

                    </ItemTemplate>
                </asp:Repeater>
                <div style="display: flex;; margin-top: 10px; align-items: center; vertical-align:middle;">
                    <asp:Repeater ID="rptBoolean" runat="server">
                        <ItemTemplate>

                            <asp:HiddenField ID="hfLinkId"
                                runat="server"
                                Value='<%# Eval("LinkId") %>' />

                            <asp:CheckBox ID="chkBoolean"
                                runat="server"
                                Text='<%# Eval("Text") %>' />

                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </td>
        </tr>
        <tr>
            <td class="label">Provinsi</td>
            <td class="entry">
                <telerik:RadComboBox
                    ID="cboProvinsi"
                    runat="server"
                    Width="300px"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="cboProvinsi_SelectedIndexChanged">
                </telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td class="label">Kabupaten / Kota</td>
            <td class="entry">
                <telerik:RadComboBox
                    ID="cboKabupaten"
                    runat="server"
                    Width="300px">
                </telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td class="label">Spesialistik</td>
            <td class="entry">
                <telerik:RadComboBox
                    ID="cboSpesialistik"
                    runat="server"
                    Width="300px"
                    EmptyMessage="-- pilih spesialistik --"
                    Filter="Contains"
                    MarkFirstMatch="true"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="cboSpesialistik_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td class="label">Faskes Rujukan</td>
            <td class="entry">
                <telerik:RadComboBox
                    ID="cboFaskesRujukan"
                    runat="server"
                    Width="300px"
                    Filter="Contains"
                    MarkFirstMatch="true" />
            </td>
        </tr>
        <tr>
            <td class="label">Jenis Pelayanan
            </td>
            <td class="entry">
                <telerik:RadComboBox ID="cboPelayanan" runat="server" Width="304px">
                    <Items>
                        <telerik:RadComboBoxItem Value="" Text="" />
                        <telerik:RadComboBoxItem Value="1" Text="Rawat Inap" />
                        <telerik:RadComboBoxItem Value="2" Text="Rawat Jalan" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td width="20px">
                <asp:RequiredFieldValidator ID="rfvPelayanan" runat="server" ErrorMessage="Jenis pelayanan required."
                    ValidationGroup="entry" ControlToValidate="cboPelayanan" SetFocusOnError="True" Width="100%">
                    <asp:Image ID="Image3" runat="server" SkinID="rfvImage" />
                </asp:RequiredFieldValidator>
            </td>
            <td />
        </tr>
        <tr>
            <td class="label">Tipe Rujukan
            </td>
            <td class="entry">
                <telerik:RadComboBox ID="cboTipeRujukan" runat="server" Width="304px">
                    <Items>
                        <telerik:RadComboBoxItem Value="" Text="" />
                        <telerik:RadComboBoxItem Value="0" Text="Penuh" />
                        <telerik:RadComboBoxItem Value="1" Text="Partial" />
                        <telerik:RadComboBoxItem Value="2" Text="Rujuk Balik" />
                    </Items>
                </telerik:RadComboBox>
            </td>
            <td width="20px">
                <asp:RequiredFieldValidator ID="rfvTipeRujukan" runat="server" ErrorMessage="Tipe rujukan required."
                    ValidationGroup="entry" ControlToValidate="cboTipeRujukan" SetFocusOnError="True" Width="100%">
                    <asp:Image ID="Image4" runat="server" SkinID="rfvImage" />
                </asp:RequiredFieldValidator>
            </td>
            <td />
        </tr>
        <tr>
            <td class="label">Poli Dirujuk</td>
            <td class="entry">
                <telerik:RadComboBox ID="cboPoliDirujuk" runat="server" Width="304px" AllowCustomText="true"
                    OnItemDataBound="cboPoliDirujuk_ItemDataBound" EnableLoadOnDemand="true" OnClientItemsRequesting="OnClientItemsRequestingHandler">
                    <WebServiceSettings Path="../../../../WebService/Sep.asmx" Method="GetPoliDirujuk" />
                </telerik:RadComboBox>
            </td>
            <td width="20px"></td>
            <td />
        </tr>
        <tr>
            <td class="label">Catatan
            </td>
            <td class="entry">
                <telerik:RadTextBox ID="txtCatatan" runat="server" Width="300px" TextMode="MultiLine" />
            </td>
            <td width="20px" />
            <td />
        </tr>
        <tr>
            <td class="label">Instruksi Pasien
            </td>
            <td class="entry">
                <telerik:RadTextBox ID="txtInstruksiPasien" runat="server" Width="300px" TextMode="MultiLine" />
            </td>
            <td width="20px" />
            <td />
        </tr>
                <tr>
            <td class="label">Keterangan Rujukan
            </td>
            <td class="entry">
                <telerik:RadTextBox ID="txtKeteranganRujukan" runat="server" Width="300px" TextMode="MultiLine" />
            </td>
            <td width="20px" />
            <td />
        </tr>
    </table>
</asp:Content>
