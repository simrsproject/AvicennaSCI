<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterCustom.Master" AutoEventWireup="true"
    CodeBehind="UserDokterAsuransi.aspx.cs"
    Inherits="Temiang.Avicenna.Module.ControlPanel.Admin.UserDokterAsuransi.UserDokterAsuransi" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script src="UserDokterAsuransi.js" type="text/javascript"></script>

    <telerik:RadAjaxLoadingPanel ID="ajxLoadingPanel" runat="server" Transparency="30">
        <img alt="Loading..."
            src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>'
            style="border:0px;margin-top:75px;" />
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxPanel ID="ajxPanel" runat="server" LoadingPanelID="ajxLoadingPanel">

        <asp:ValidationSummary ID="validationSummary"
            runat="server"
            ValidationGroup="entry"
            BackColor="#FFFFC0"
            BorderColor="#FFC080"
            BorderStyle="Solid"
            Font-Size="Small"
            EnableClientScript="true" />

        <asp:Panel ID="pnlInformation"
            runat="server"
            Visible="false"
            BorderColor="#FFC080"
            BorderStyle="Solid"
            BackColor="#FFFFC0">

            <table width="100%">
                <tr>
                    <td width="32">
                        <asp:Image ID="imgAttention"
                            runat="server"
                            ImageUrl="~/Images/AttentionLarge.png" />
                    </td>
                    <td>
                        &nbsp;&nbsp;
                        <asp:Label ID="lblInformation" runat="server" />
                    </td>
                </tr>
            </table>

        </asp:Panel>

        <br />

       <table width="100%" height="400px" style="vertical-align: middle;">
            <tr>
                <td valign="middle" align="center">

                    <table cellpadding="4" cellspacing="0">

                        <tr>
                            <td colspan="4"
                                align="center"
                                style="background-color: Black;
                                       color: White;
                                       font-weight: bold;
                                       padding: 5px;">
                                User Dokter Asuransi
                            </td>
                        </tr>

                        <tr>
                            <td class="label" style="width:130px;">
                                <asp:Label ID="lblUserID" runat="server" Text="User ID" />
                            </td>

                            <td class="entry" style="width:320px;">
                                <telerik:RadTextBox ID="txtUserID"
                                    runat="server"
                                    Width="300px"
                                    MaxLength="40"
                                    ReadOnly="true" />
                            </td>

                            <td width="20">
                                <asp:RequiredFieldValidator ID="rfvUserID"
                                    runat="server"
                                    ErrorMessage="User ID required."
                                    ValidationGroup="entry"
                                    ControlToValidate="txtUserID"
                                    SetFocusOnError="True">
                                    <asp:Image ID="Image1" runat="server" SkinID="rfvImage" />
                                </asp:RequiredFieldValidator>
                            </td>

                            <td></td>
                        </tr>

                        <tr>
                            <td class="label">
                                <asp:Label ID="lblUserName" runat="server" Text="User Name" />
                            </td>

                            <td class="entry">
                                <telerik:RadTextBox ID="txtUserName"
                                    runat="server"
                                    Width="300px"
                                    MaxLength="50"
                                    ReadOnly="true"/>
                            </td>

                            <td width="20">
                                <asp:RequiredFieldValidator ID="rfvUserName"
                                    runat="server"
                                    ErrorMessage="User Name required."
                                    ValidationGroup="entry"
                                    ControlToValidate="txtUserName"
                                    SetFocusOnError="True">
                                    <asp:Image ID="Image2" runat="server" SkinID="rfvImage" />
                                </asp:RequiredFieldValidator>
                            </td>

                            <td></td>
                        </tr>

                        <tr>
                            <td class="label">
                                <asp:Label ID="lblParamedicID" runat="server" Text="Physician ID" />
                            </td>

                            <td class="entry">
                                <telerik:RadComboBox ID="cboParamedicID"
                                    runat="server"
                                    Width="300px"
                                    AllowCustomText="true"
                                    Filter="Contains" />
                            </td>

                            <td></td>
                            <td></td>
                        </tr>

                        <tr>
                            <td></td>
                            <td style="padding-top:10px;">
                                <asp:Button ID="btnOk"
                                    runat="server"
                                    Text="Save"
                                    Width="100px"
                                    ValidationGroup="entry"
                                    OnClick="btnOk_Click" />
                            </td>
                            <td></td>
                            <td></td>
                        </tr>

                    </table>

                </td>
            </tr>
        </table>

    </telerik:RadAjaxPanel>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            //<![CDATA[

            // Javascript

            //]]>
        </script>
    </telerik:RadCodeBlock>

</asp:Content>