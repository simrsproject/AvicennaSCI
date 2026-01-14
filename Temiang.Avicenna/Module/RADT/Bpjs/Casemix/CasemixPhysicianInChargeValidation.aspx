<%@ Page Language="C#" MasterPageFile="~/MasterPage/MasterCustom.Master" AutoEventWireup="true"
    CodeBehind="CasemixPhysicianInChargeValidation.aspx.cs" Inherits="Temiang.Avicenna.Module.RADT.Bpjs.CasemixPhysicianInChargeValidation" %>

<%@ Import Namespace="Temiang.Avicenna.BusinessObject" %>
<%@ Import Namespace="Temiang.Avicenna.Common" %>
<%@ Register TagPrefix="cc" Namespace="Temiang.Avicenna.CustomControl" Assembly="Temiang.Avicenna" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript" src="../../../../JavaScript/jQuery.js"></script>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script language="javascript" type="text/javascript">
            function openWinProcess(regNo, fregNo, isDpjp, regType, mode) {
                const isAllowed = String(isDpjp).toLowerCase() === "true";
                if (!isAllowed) {
                    alert("This menu can only be accessed by the paramedic team.");
                    return false;
                }
                entryResumeMedis(regNo, fregNo, '', regType, mode);
            }
            function entryResumeMedis(regno, fregno, parid, regType, mode) {
                var url = "";
                if (regType == "IPR") {
                        url = '<%= Helper.UrlRoot() %>/Module/RADT/EmrIp/EmrIpCommon/ResumeMedis/ResumeMedisRichTextInPatientEntry.aspx?mod=view&regno=' + regno + '&fregno=' + fregno + '&patid=<%= PatientID %>&parid=' + parid;
                }
                else if (regType == "OPR") {
                        url = '<%= Helper.UrlRoot() %>/Module/RADT/EmrIp/EmrIpCommon/ResumeMedis/ResumeMedisRichTextOutPatientEntry.aspx?mod=view&regno=' + regno + '&fregno=' + fregno + '&patid=<%= PatientID %>&parid=' + parid;
                }
                else if (regType == "EMR") {
                        url = '<%= Helper.UrlRoot() %>/Module/RADT/EmrIp/EmrIpCommon/ResumeMedis/ResumeMedisRichTextOutPatientEntry.aspx?mod=view&regno=' + regno + '&fregno=' + fregno + '&patid=<%= PatientID %>&parid=' + parid;
                }
                //console.log(url);
                if (url != "") {
                    if (mode == "cmx")
                        url = url + "&toValidate=1";

                    var oWnd = $find("<%= winRegInfo.ClientID %>");
                    oWnd.setUrl(url);
                    oWnd.show();
                    oWnd.maximize();
                }
            }
            function onClientClose(oWnd, args) {
                if (oWnd.argument && oWnd.argument.mode != null) {
                    __doPostBack("<%= grdList.UniqueID %>", "rebind");
                }
            }
            <%--function openPaymentReceiveWithCheckPayment(patientId, regNo, pc, cmno) {
                $.ajax({
                    type: "POST",
                    url: "../../../../WebService/BillingChargeService.asmx/RemainingAmountConfirm",
                    data: "{'patientId':'" + patientId + "','beforeRegNo':'" + regNo + "'}", // if ur method take parameters
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        var result = response.d;
                        if (result != '') {
                            alert('This patient has remain amount: ' + result);
                        }
                        continuePaymentReceive(regNo, pc, cmno);
                    },
                    dataType: "json",
                    failure: function (response) {
                        var result = response.d;
                        alert(result);
                    }
                });
            }


            
            function viewHistory(regNo) {
                var oWnd = $find("<%= winHistory.ClientID %>");
                oWnd.setUrl('PaymentReceiveHistory.aspx?type=1&rno=' + regNo);

                oWnd.set_width(document.body.offsetWidth);
                oWnd.show();
            }
            function rowDelete(payNo) {
                __doPostBack("<%= grdList.UniqueID %>", 'delete|' + payNo);
            }
            function openWinRegistrationInfo(regNo) {
                var oWnd = $find("<%= winRegInfo.ClientID %>");
                var lblToBeUpdate = "noti_" + regNo;

                oWnd.setUrl('<%=Page.ResolveUrl("~/Module/RADT/RegistrationInfo/RegistrationInfoList.aspx?regNo=' + regNo + '&lblRegistrationInfo=' + lblToBeUpdate + '")%>');
                oWnd.show();
            }

            function openWinQuestionFormCheckList(regNo) {
                var oWnd = $find("<%= winDocsOption.ClientID %>");
                var lblToBeUpdate = "noti2_" + regNo;

                oWnd.setUrl('<%=Page.ResolveUrl("~/Module/RADT/Registration/RegistrationDocumentCheckList.aspx?regNo=' + regNo + '&lblRegistrationInfo=' + lblToBeUpdate + '")%>');
                oWnd.set_title('Document Checklist');
                oWnd.show();
            }--%>
        </script>

    </telerik:RadCodeBlock>
    <%--<telerik:RadWindow runat="server" Animation="None" Width="800px" Height="550px" Behavior="Move, Close"
        ShowContentDuringLoad="False" VisibleStatusbar="false" Modal="true" ID="winHistory">
    </telerik:RadWindow>
    <telerik:RadWindow ID="winRegInfo" Animation="None" Width="900px" Height="500px"
        runat="server" ShowContentDuringLoad="false" Behavior="Close" VisibleStatusbar="false"
        Modal="true" />
    <telerik:RadWindow runat="server" Animation="None" Width="500px" Height="400px" Behavior="Close, Move"
        ShowContentDuringLoad="False" VisibleStatusbar="False" Modal="true" Title="Document Checklist"
        ID="winDocsOption">
    </telerik:RadWindow>--%>
    <telerik:RadWindow ID="winRegInfo" Animation="None" Width="900px" Height="500px"
    runat="server" ShowContentDuringLoad="false" Behavior="Close" VisibleStatusbar="false"
    Modal="true" />
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnFilterDate">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterRegistration">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
           <%-- <telerik:AjaxSetting AjaxControlID="btnFilterPaymenNo">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" />
                </UpdatedControls>
            </telerik:AjaxSetting>--%>
            <telerik:AjaxSetting AjaxControlID="txtServiceUnitID">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblServiceUnitName" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterServiceUnit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterParamedic">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnFilterName">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManagerProxy>
    <cc:CollapsePanel2 ID="CollapsePanel" runat="server" Title="Search Filter">
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                            <td class="label">
                                <asp:Label ID="Label1" runat="server" Text="Registration Date"></asp:Label>
                            </td>
                            <td class="entry">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <telerik:RadDatePicker ID="txtOrderDate1" runat="server" Width="100px" />
                                        </td>
                                        <td>&nbsp;-&nbsp;
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="txtOrderDate2" runat="server" Width="100px" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td width="20">
                                <asp:ImageButton ID="btnFilterDate" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                            <td style="text-align: right"></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Label ID="lblRegistrationNo" runat="server" Text="Registration No / Medical No"></asp:Label>
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtRegistrationNo" runat="server" Width="300px" MaxLength="20" />
                            </td>
                            <td width="20">
                                <asp:ImageButton ID="btnFilterRegistration" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                            <td style="text-align: right"></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Label ID="lblPatientName" runat="server" Text="Patient Name"></asp:Label>
                            </td>
                            <td class="entry">
                                <telerik:RadTextBox ID="txtPatientName" runat="server" Width="300px" MaxLength="150" />
                            </td>
                            <td width="20">
                                <asp:ImageButton ID="btnFilterName" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                            <td style="text-align: right"></td>
                        </tr>
                    </table>
                </td>
                <td width="50%" style="vertical-align: top">
                    <table width="100%">
                        <tr>
                            <td class="label">
                                <asp:Label ID="lblFromServiceUnitID" runat="server" Text="Service Unit"></asp:Label>
                            </td>
                            <td class="entry">
                                <telerik:RadComboBox runat="server" ID="cboServiceUnitID" Width="300px" AllowCustomText="true"
                                    OnSelectedIndexChanged="cboServiceUnitID_SelectedIndexChanged" Filter="Contains" AutoPostBack="true">
                                </telerik:RadComboBox>
                            </td>
                            <td width="20">
                                <asp:ImageButton ID="btnFilterServiceUnit" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                            <td style="text-align: right"></td>
                        </tr>
                        <tr runat="server">
                            <td class="label">
                                <asp:Label ID="lblParamedicID" runat="server" Text="Physician"></asp:Label>
                            </td>
                            <td class="entry300">
                                <telerik:RadComboBox runat="server" ID="cboParamedicID" Width="300px" AllowCustomText="true"
                                    Filter="Contains">
                                </telerik:RadComboBox>
                            </td>
                            <td width="20">
                                <asp:ImageButton ID="btnFilterParamedic" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                            <td style="text-align: right"></td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Label ID="lblGuarantorID" runat="server" Text="Guarantor"></asp:Label>
                            </td>
                            <td class="entry">
                                <telerik:RadComboBox runat="server" ID="cboGuarantorID" Width="300px" AllowCustomText="true"
                                    Filter="Contains">
                                </telerik:RadComboBox>
                            </td>
                            <td width="20">
                                <asp:ImageButton ID="btnFilterGuarantor" runat="server" ImageUrl="~/Images/Toolbar/search16.png"
                                    OnClick="btnFilter_Click" ToolTip="Search" />
                            </td>
                            <td style="text-align: right"></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </cc:CollapsePanel2>
    <telerik:RadGrid ID="grdList" runat="server" OnNeedDataSource="grdList_NeedDataSource"
        GridLines="None" AutoGenerateColumns="false" AllowPaging="true" PageSize="15">
        <MasterTableView DataKeyNames="RegistrationNo" ClientDataKeyNames="RegistrationNo"
            GroupLoadMode="Client">
             <GroupHeaderTemplate>
                 <%#string.Format("Physician: <b>{0}</b>", Eval("ParamedicName") )%>
             </GroupHeaderTemplate>
             <GroupByExpressions>
                 <telerik:GridGroupByExpression>
                     <SelectFields>
                         <telerik:GridGroupByField FieldName="Group" HeaderText="Service Unit " />
                         <telerik:GridGroupByField FieldName="ParamedicName" HeaderText="Reg To Physician " />
                     </SelectFields>
                     <GroupByFields>
                         <telerik:GridGroupByField FieldName="Group" SortOrder="Ascending" />
                         <telerik:GridGroupByField FieldName="ParamedicName" SortOrder="Ascending" />
                     </GroupByFields>
                 </telerik:GridGroupByExpression>
             </GroupByExpressions>
            <Columns>
                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="MDS Casemix">
                    <ItemTemplate>
                        <%#string.Format("<a href=\"#\" onclick=\"openWinProcess('{0}', '{1}','{2}','{3}','{4}'); return false;\"> <img src=\"../../../../Images/Toolbar/ordering16.png\" border=\"0\" alt=\"New\" /></a>",
                                                          DataBinder.Eval(Container.DataItem, "RegistrationNo"),DataBinder.Eval(Container.DataItem, "FromRegistrationNo"),DataBinder.Eval(Container.DataItem, "IsParamedicTeam"),DataBinder.Eval(Container.DataItem, "SRRegistrationType"),"cmx")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="60px" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="MDS EMR">
                    <ItemTemplate>
                        <%#string.Format("<a href=\"#\" onclick=\"openWinProcess('{0}', '{1}','{2}','{3}','{4}'); return false;\"> <img src=\"../../../../Images/Toolbar/ordering16.png\" border=\"0\" alt=\"New\" /></a>",
                                                          DataBinder.Eval(Container.DataItem, "RegistrationNo"),DataBinder.Eval(Container.DataItem, "FromRegistrationNo"),DataBinder.Eval(Container.DataItem, "IsParamedicTeam"),DataBinder.Eval(Container.DataItem, "SRRegistrationType"),"emr")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="60px" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridTemplateColumn>
                <telerik:GridDateTimeColumn DataField="RegistrationDate" HeaderText="Reg. Date" UniqueName="RegistrationDate"
                    SortExpression="RegistrationDate">
                    <HeaderStyle HorizontalAlign="Center" Width="80px" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="RegistrationNo" HeaderText="Registration No"
                    UniqueName="RegistrationNo" SortExpression="RegistrationNo">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="MedicalNo" HeaderText="Medical No" UniqueName="MedicalNo"
                    SortExpression="MedicalNo">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderStyle-Width="40px" DataField="SalutationName" HeaderText=""
                    UniqueName="SalutationName" SortExpression="SalutationName" HeaderStyle-HorizontalAlign="Center"
                    ItemStyle-HorizontalAlign="Center" Visible="false" />
                <telerik:GridTemplateColumn HeaderStyle-Width="250px" DataField="PatientName" HeaderText="Patient Name" UniqueName="PatientName"
                    SortExpression="PatientName">
                    <ItemTemplate>
                        <%# string.Format("{0} {1}", DataBinder.Eval(Container.DataItem, "SalutationName"), DataBinder.Eval(Container.DataItem, "PatientName"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Sex" HeaderText="Gender" UniqueName="Sex" SortExpression="Sex">
                    <HeaderStyle HorizontalAlign="Center" Width="60px" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ParamedicName" HeaderText="Physician" UniqueName="ParamedicName"
                    SortExpression="ParamedicName">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="GuarantorName" HeaderText="Guarantor" UniqueName="GuarantorName"
                    SortExpression="GuarantorName">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ServiceUnitName" HeaderText="Service Unit" UniqueName="ServiceUnitName"
                    SortExpression="ServiceUnitName">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RoomName" HeaderText="Room" UniqueName="RoomName"
                    SortExpression="RoomName" Visible="False">
                    <HeaderStyle HorizontalAlign="Left" Width="150px" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
            </Columns>
            <ExpandCollapseColumn Visible="True" />
        </MasterTableView>
        <FilterMenu>
        </FilterMenu>
        <ClientSettings EnableRowHoverStyle="true">
            <Resizing AllowColumnResize="True" />
            <Selecting AllowRowSelect="True" />
        </ClientSettings>
    </telerik:RadGrid>
</asp:Content>
