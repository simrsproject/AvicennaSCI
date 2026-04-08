<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterDetail.Master" AutoEventWireup="true"
    CodeBehind="UserDokterAsuransiDetail.aspx.cs" Inherits="Temiang.Avicenna.Module.ControlPanel.Admin.UserDokterAsuransi.UserDokterAsuransiDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="UserDokterAsuransiDetail.js" type="text/javascript"></script>

    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td style="width: 50%; vertical-align: top">
                <table width="100%">
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblUserID" runat="server" Text="User ID"></asp:Label>
                        </td>
                        <td class="entry">
                            <telerik:RadTextBox ID="txtUserID" runat="server" Width="100px" MaxLength="40" />
                        </td>
                        <td width="20">
                            <asp:RequiredFieldValidator ID="rfvUserID" runat="server" ErrorMessage="User ID required."
                                ValidationGroup="entry" ControlToValidate="txtUserID" SetFocusOnError="True"
                                Width="100%">
                                <asp:Image ID="Image1" runat="server" SkinID="rfvImage" />
                            </asp:RequiredFieldValidator>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblUserName" runat="server" Text="User Name"></asp:Label>
                        </td>
                        <td class="entry">
                            <telerik:RadTextBox ID="txtUserName" runat="server" Width="300px" MaxLength="50" />
                        </td>
                        <td width="20">
                            <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ErrorMessage="User Name required."
                                ValidationGroup="entry" ControlToValidate="txtUserName" SetFocusOnError="True"
                                Width="100%">
                                <asp:Image ID="Image2" runat="server" SkinID="rfvImage" />
                            </asp:RequiredFieldValidator>
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblParamedicID" runat="server" Text="Physician ID"></asp:Label>
                        </td>
                        <td class="entry">
                            <telerik:RadComboBox ID="cboParamedicID" runat="server" Width="300px" AllowCustomText="true"
                                Filter="Contains" />
                        </td>
                        <td width="20"></td>
                        <td></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            //<![CDATA[
            
        //]]>
        </script>

    </telerik:RadCodeBlock>
</asp:Content>
