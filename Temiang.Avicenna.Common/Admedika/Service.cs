using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class Service
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        private readonly AdmedikaConfig _config;
        private readonly IAdmedikaTransport _transport;
        private readonly IAdmedikaLogger _logger;

        public Service()
        {
            _config = new AdmedikaConfig();
            _transport = new HttpAdmedikaTransport();
            _logger = new WebServiceApiLogger(_config);
        }

        public Service(AdmedikaConfig config, IAdmedikaTransport transport, IAdmedikaLogger logger)
        {
            _config = config ?? new AdmedikaConfig();
            _transport = transport ?? new HttpAdmedikaTransport();
            _logger = logger ?? new NullAdmedikaLogger();
        }

        public AdmedikaConfig Config
        {
            get { return _config; }
        }

        /// <summary>
        /// Menentukan apakah respons AdMedika dianggap sukses.
        /// TechnicalError diperiksa lebih dulu, lalu IsSuccess.
        /// </summary>
        public bool IsOk(AdmedikaResponse result, out string message)
        {
            message = null;

            if (result == null)
            {
                message = "Respons AdMedika kosong.";
                return false;
            }

            if (!string.IsNullOrEmpty(result.TechnicalError))
            {
                message = result.TechnicalError;
                return false;
            }

            if (!result.IsSuccess)
            {
                var code = result.ErrorCode ?? result.StatusCode;
                var description = result.ErrorDesc ?? result.StatusMsg;

                message = string.IsNullOrEmpty(code)
                    ? description
                    : string.Format("[{0}] {1}", code, description);

                if (string.IsNullOrEmpty(message))
                    message = "Permintaan AdMedika gagal.";

                return false;
            }

            return true;
        }

        public HelloWorldResponse HelloWorld()
        {
            return Send<HelloWorldResponse>(
                AdmedikaConstant.ServiceID.HelloWorld,
                new HelloWorldTxnData
                {
                    Request = new HelloWorldRequest { CustomerID = _config.CustomerID }
                });
        }

        private bool RequireCoverageId(string covID, AdmedikaResponse response, out string normalized)
        {
            normalized = covID == null ? null : covID.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                response.ErrorCode = "1999";
                response.ErrorDesc = "covID cannot be empty.";
                response.StatusMsg = response.ErrorDesc;
                return false;
            }

            if (!AdmedikaCoverageType.IsValid(normalized))
            {
                response.ErrorCode = "1820";
                response.ErrorDesc = "Coverage code is invalid. Allowed coverage ID: "
                                     + AdmedikaCoverageType.AllowedText() + ".";
                response.StatusMsg = response.ErrorDesc;
                return false;
            }

            return true;
        }

        public CheckIcdExclusionResponse CheckIcdExclusion(string cardNo, string covID, string diagnosisCodeList)
        {
            var response = new CheckIcdExclusionResponse();
            string normalizedCovID;
            if (!RequireCoverageId(covID, response, out normalizedCovID)) return response;

            return Send<CheckIcdExclusionResponse>(
                AdmedikaConstant.ServiceID.CheckIcdExclusion,
                new CheckIcdExclusionTxnData
                {
                    Request = new CheckIcdExclusionRequest
                    {
                        CheckIcdExclusion = new CheckIcdExclusionPayload
                        {
                            CardNo = cardNo,
                            CovID = normalizedCovID,
                            DiagnosisCodeList = diagnosisCodeList
                        }
                    }
                });
        }

        private bool RequireTerminalId(AdmedikaResponse response)
        {
            if (!string.IsNullOrWhiteSpace(_config.TerminalID)) return true;

            response.TechnicalError = "AdMedika terminalID belum dikonfigurasi (AdmedikaTerminalID).";
            response.StatusMsg = response.TechnicalError;
            return false;
        }

        public EligibilityResponse Eligibility(string cardNo, string covID, string diagnosisCodeList,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks)
        {
            var response = new EligibilityResponse();
            if (!RequireTerminalId(response)) return response;

            string normalizedCovID;
            if (!RequireCoverageId(covID, response, out normalizedCovID)) return response;

            return Send<EligibilityResponse>(
                AdmedikaConstant.ServiceID.Eligibility,
                new EligibilityTxnData
                {
                    Request = new EligibilityRequest
                    {
                        Eligibility = new EligibilityPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo,
                            CovID = normalizedCovID,
                            DiagnosisCodeList = diagnosisCodeList,
                            ProviderTransID = providerTransID,
                            NationalID = nationalID,
                            FamilyCardID = familyCardID,
                            PhysicianName = physicianName,
                            AccidentFlag = accidentFlag,
                            SurgicalFlag = surgicalFlag,
                            RoomType = roomType,
                            RoomPrice = roomPrice,
                            Remarks = remarks
                        }
                    }
                });
        }

        public GetEntitlementResponse GetEntitlement(string cardNo)
        {
            var response = new GetEntitlementResponse();
            if (!RequireTerminalId(response)) return response;

            return Send<GetEntitlementResponse>(
                AdmedikaConstant.ServiceID.GetEntitlement,
                new GetEntitlementTxnData
                {
                    Request = new GetEntitlementRequest
                    {
                        GetEntitlement = new GetEntitlementPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo
                        }
                    }
                });
        }

        public DischargeOpResponse DischargeOp(string cardNo, string diagnosisCodeList, string mcDays,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks,
            DischargeEntitlementInput[] entitlement)
        {
            var response = new DischargeOpResponse();
            if (!RequireTerminalId(response)) return response;

            if (!ValidateAndNormalizeDischarge(response, ref cardNo, ref diagnosisCodeList, ref mcDays,
                ref accidentFlag, ref surgicalFlag, ref remarks, entitlement))
                return response;

            return Send<DischargeOpResponse>(
                AdmedikaConstant.ServiceID.DischargeOp,
                new DischargeOpTxnData
                {
                    Request = new DischargeOpRequest
                    {
                        Discharge = new DischargeOpPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo,
                            DiagnosisCodeList = diagnosisCodeList,
                            McDays = mcDays,
                            ProviderTransID = providerTransID,
                            NationalID = nationalID,
                            FamilyCardID = familyCardID,
                            PhysicianName = physicianName,
                            AccidentFlag = accidentFlag,
                            SurgicalFlag = surgicalFlag,
                            RoomType = roomType,
                            RoomPrice = roomPrice,
                            Remarks = remarks,
                            Entitlement = entitlement
                        }
                    }
                });
        }

        private static bool ValidateAndNormalizeDischarge(AdmedikaResponse response,
            ref string cardNo, ref string diagnosisCodeList, ref string mcDays,
            ref string accidentFlag, ref string surgicalFlag, ref string remarks,
            DischargeEntitlementInput[] entitlement)
        {
            cardNo = Normalize(cardNo);
            diagnosisCodeList = Normalize(diagnosisCodeList);
            mcDays = Normalize(mcDays);
            accidentFlag = NormalizeFlag(accidentFlag);
            surgicalFlag = NormalizeFlag(surgicalFlag);
            remarks = Normalize(remarks);

            if (string.IsNullOrEmpty(cardNo))
                return SetValidationError(response, "cardNo cannot be empty.");

            if (cardNo.Length != 16 || !ContainsDigitsOnly(cardNo))
                return SetValidationError(response, "cardNo must contain exactly 16 digits.", "1997");

            if (string.IsNullOrEmpty(diagnosisCodeList))
                return SetValidationError(response, "diagnosisCodeList cannot be empty.", "1830");

            int days;
            if (!string.IsNullOrEmpty(mcDays) &&
                (!int.TryParse(mcDays, NumberStyles.None, CultureInfo.InvariantCulture, out days) ||
                 days < 0 || days > 99))
                return SetValidationError(response, "mcDays must be a number from 0 to 99.", "1997");

            if (!IsOptionalFlagValid(accidentFlag))
                return SetValidationError(response, "accidentFlag must be Y or N.", "1997");

            if (!IsOptionalFlagValid(surgicalFlag))
                return SetValidationError(response, "surgicalFlag must be Y or N.", "1997");

            if (!string.IsNullOrEmpty(remarks) && remarks.Length > 500)
                return SetValidationError(response, "remarks cannot exceed 500 characters.", "1997");

            if (entitlement == null || entitlement.Length == 0)
                return SetValidationError(response, "entitlement cannot be empty.");

            var benefitIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var benefit in entitlement)
            {
                if (benefit == null)
                    return SetValidationError(response, "entitlement contains an empty benefit.", "1997");

                benefit.BenID = Normalize(benefit.BenID);
                if (string.IsNullOrEmpty(benefit.BenID))
                    return SetValidationError(response, "benID cannot be empty.");

                if (benefit.BenID.Length > 2)
                    return SetValidationError(response, "benID cannot exceed 2 characters.", "1997");

                if (!benefitIds.Add(benefit.BenID))
                    return SetValidationError(response, "Duplicate benID " + benefit.BenID + ".", "1997");

                if (benefit.BenItemList == null || benefit.BenItemList.Length == 0)
                    return SetValidationError(response,
                        "benItemList cannot be empty for benID " + benefit.BenID + ".");

                decimal benefitAmount;
                if (!TryParseAmount(benefit.BenAmount, out benefitAmount) || benefitAmount < 0)
                    return SetValidationError(response,
                        "Invalid benAmount for benID " + benefit.BenID + ".", "1997");

                decimal itemTotal = 0;
                foreach (var item in benefit.BenItemList)
                {
                    if (item == null)
                        return SetValidationError(response,
                            "benItemList contains an empty item for benID " + benefit.BenID + ".", "1997");

                    item.Code = Normalize(item.Code);
                    item.Name = Normalize(item.Name);
                    item.DateIssue = Normalize(item.DateIssue);
                    item.Qty = Normalize(item.Qty);

                    if (string.IsNullOrEmpty(item.Code) || item.Code.Length > 50)
                        return SetValidationError(response,
                            "Item code is required and cannot exceed 50 characters.", "1997");

                    if (string.IsNullOrEmpty(item.Name) || item.Name.Length > 50)
                        return SetValidationError(response,
                            "Item name is required and cannot exceed 50 characters.", "1997");

                    DateTime issueDate;
                    if (!string.IsNullOrEmpty(item.DateIssue) &&
                        !DateTime.TryParseExact(item.DateIssue, "ddMMyyyy", CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out issueDate))
                        return SetValidationError(response,
                            "dateIssue must use DDMMYYYY format.", "1829");

                    decimal quantity;
                    if (!string.IsNullOrEmpty(item.Qty) &&
                        (!decimal.TryParse(item.Qty, NumberStyles.Number, CultureInfo.InvariantCulture,
                             out quantity) || quantity < 0 || quantity > 999))
                        return SetValidationError(response,
                            "qty must be a number from 0 to 999.", "1997");

                    decimal price;
                    if (!TryParseAmount(item.TotPrice, out price) || price < 0)
                        return SetValidationError(response,
                            "Invalid totPrice for item " + item.Code + ".", "1997");

                    item.TotPrice = FormatAmount(price);
                    itemTotal += price;
                }

                if (benefitAmount != itemTotal)
                    return SetValidationError(response,
                        string.Format(CultureInfo.InvariantCulture,
                            "benAmount for benID {0} ({1}) does not match item total ({2}).",
                            benefit.BenID, FormatAmount(benefitAmount), FormatAmount(itemTotal)), "1809");

                benefit.BenAmount = FormatAmount(benefitAmount);
            }

            return true;
        }

        private static bool SetValidationError(AdmedikaResponse response, string message,
            string errorCode = "1999")
        {
            response.ErrorCode = errorCode;
            response.ErrorDesc = message;
            response.StatusMsg = message;
            return false;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string NormalizeFlag(string value)
        {
            var normalized = Normalize(value);
            return normalized == null ? null : normalized.ToUpperInvariant();
        }

        private static bool IsOptionalFlagValid(string value)
        {
            return string.IsNullOrEmpty(value) || value == "Y" || value == "N";
        }

        private static bool ContainsDigitsOnly(string value)
        {
            foreach (var character in value)
            {
                if (character < '0' || character > '9') return false;
            }

            return true;
        }

        private static bool TryParseAmount(string value, out decimal amount)
        {
            return decimal.TryParse(Normalize(value), NumberStyles.Number,
                CultureInfo.InvariantCulture, out amount);
        }

        private static string FormatAmount(decimal value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public CheckMemberClaimTxnStatusResponse CheckMemberClaimTxnStatus(string cardNo, string clID)
        {
            var response = new CheckMemberClaimTxnStatusResponse();
            if (!RequireTerminalId(response)) return response;

            return Send<CheckMemberClaimTxnStatusResponse>(
                AdmedikaConstant.ServiceID.CheckMemberClaimTxnStatus,
                new CheckMemberClaimTxnStatusTxnData
                {
                    Request = new CheckMemberClaimTxnStatusRequest
                    {
                        CheckMemberClaimTxnStatus = new CheckMemberClaimTxnStatusPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo,
                            ClID = clID
                        }
                    }
                });
        }

        public GetMemberEnrolledPlanTcResponse GetMemberEnrolledPlanTc(string cardNo, string covID,
            string searchForTermCondition)
        {
            var response = new GetMemberEnrolledPlanTcResponse();
            string normalizedCovID;
            if (!RequireCoverageId(covID, response, out normalizedCovID)) return response;

            return Send<GetMemberEnrolledPlanTcResponse>(
                AdmedikaConstant.ServiceID.GetMemberEnrolledPlanTc,
                new GetMemberEnrolledPlanTcTxnData
                {
                    Request = new GetMemberEnrolledPlanTcRequest
                    {
                        GetMemberEnrolledPlanTC = new GetMemberEnrolledPlanTcPayload
                        {
                            CardNo = cardNo,
                            CovID = normalizedCovID,
                            SearchForTermCondition = searchForTermCondition
                        }
                    }
                });
        }

        public CancelOpenClaimsTxnResponse CancelOpenClaimsTxn(string cardNo, string remarks)
        {
            var response = new CancelOpenClaimsTxnResponse();
            if (!RequireTerminalId(response)) return response;

            return Send<CancelOpenClaimsTxnResponse>(
                AdmedikaConstant.ServiceID.CancelOpenClaimsTxn,
                new CancelOpenClaimsTxnTxnData
                {
                    Request = new CancelOpenClaimsTxnRequest
                    {
                        CancelOpenClaimTxn = new CancelOpenClaimsTxnPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo,
                            Remarks = remarks
                        }
                    }
                });
        }

        public DailyMonitoringResponse DailyMonitoring(string cardNo, string providerTransID, string clID,
            string dmUser, DailyMonitoringForm[] dmForms)
        {
            var response = new DailyMonitoringResponse();
            if (!RequireTerminalId(response)) return response;

            return Send<DailyMonitoringResponse>(
                AdmedikaConstant.ServiceID.DailyMonitoring,
                new DailyMonitoringTxnData
                {
                    Request = new DailyMonitoringRequest
                    {
                        DailyMonitoring = new DailyMonitoringPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo,
                            ProviderTransID = providerTransID,
                            ClID = clID,
                            DmUser = dmUser,
                            DmForms = dmForms
                        }
                    }
                });
        }

        public PreRegisterEbillEnquiryResponse PreRegisterEbillEnquiry(string payorCode, string coverage,
            string dateFrom, string dateTo)
        {
            var response = new PreRegisterEbillEnquiryResponse();
            if (!RequireTerminalId(response)) return response;

            if (!ValidatePreRegisterEbillPeriod(response, ref payorCode, ref coverage, ref dateFrom, ref dateTo))
                return response;

            return Send<PreRegisterEbillEnquiryResponse>(
                AdmedikaConstant.ServiceID.PreRegisterEbillEnquiry,
                new PreRegisterEbillEnquiryTxnData
                {
                    Request = new PreRegisterEbillEnquiryRequest
                    {
                        PreRegisterEbillEnquiry = new PreRegisterEbillEnquiryPayload
                        {
                            TerminalID = _config.TerminalID,
                            PayorCode = payorCode,
                            Coverage = coverage,
                            DateFrom = dateFrom,
                            DateTo = dateTo
                        }
                    }
                });
        }

        public PreRegisterEbillResponse PreRegisterEbill(PreRegisterEbillPayload request,
            AdmedikaFile[] files)
        {
            var response = new PreRegisterEbillResponse();
            if (!RequireTerminalId(response)) return response;

            if (request == null)
            {
                SetValidationError(response, "preRegisterEbill cannot be empty.");
                return response;
            }

            var payorCode = request.PayorCode;
            var coverage = request.Coverage;
            var dateFrom = request.DateFrom;
            var dateTo = request.DateTo;

            if (!ValidatePreRegisterEbillPeriod(response, ref payorCode, ref coverage, ref dateFrom, ref dateTo))
                return response;

            request.PayorCode = payorCode;
            request.Coverage = coverage;
            request.DateFrom = dateFrom;
            request.DateTo = dateTo;

            request.InvoiceNo = Normalize(request.InvoiceNo);
            request.InvoiceDate = Normalize(request.InvoiceDate);
            request.PreparedBy = Normalize(request.PreparedBy);
            request.TotAmountDue = Normalize(request.TotAmountDue);

            if (string.IsNullOrEmpty(request.InvoiceNo))
                return ValidationFailed(response, "invoiceNo cannot be empty.");
            if (!IsValidDdMmYyyy(request.InvoiceDate))
                return ValidationFailed(response, "invoiceDate must use DDMMYYYY format.", "1829");
            if (string.IsNullOrEmpty(request.PreparedBy))
                return ValidationFailed(response, "preparedBy cannot be empty.");
            if (request.PreparedBy.Length > 25)
                return ValidationFailed(response, "preparedBy cannot exceed 25 characters.", "1997");
            if (string.IsNullOrEmpty(request.TotAmountDue))
                return ValidationFailed(response, "totAmountDue cannot be empty.");

            var selectedClaims = request.SelectedClaims ?? new PreRegisterEbillSelectedClaim[0];
            if (selectedClaims.Length == 0)
                return ValidationFailed(response, "selectedClaims cannot be empty.");

            for (var i = 0; i < selectedClaims.Length; i++)
            {
                if (selectedClaims[i] == null)
                    return ValidationFailed(response, "selectedClaims cannot contain empty item.");

                selectedClaims[i].ClID = Normalize(selectedClaims[i].ClID);
                if (string.IsNullOrEmpty(selectedClaims[i].ClID))
                    return ValidationFailed(response, "selectedClaims clID cannot be empty.");
            }

            request.SelectedClaims = selectedClaims;
            request.TotSelectedClaims = selectedClaims.Length.ToString(CultureInfo.InvariantCulture);
            request.TerminalID = _config.TerminalID;

            return SendMultipart<PreRegisterEbillResponse>(
                AdmedikaConstant.ServiceID.PreRegisterEbill,
                new PreRegisterEbillTxnData
                {
                    Request = new PreRegisterEbillRequest
                    {
                        PreRegisterEbill = request
                    }
                },
                files);
        }

        public FinalBillResponse FinalBill(FinalBillPayload bill)
        {
            var response = new FinalBillResponse();
            if (!RequireTerminalId(response)) return response;

            if (bill == null)
                return ValidationFailed(response, "finalBill cannot be empty.");

            bill.CardNo = Normalize(bill.CardNo);
            bill.FinalFlag = NormalizeFlag(bill.FinalFlag);
            bill.DateAdmission = Normalize(bill.DateAdmission);
            bill.DateDischarge = Normalize(bill.DateDischarge);
            bill.InvoiceNo = Normalize(bill.InvoiceNo);
            bill.InvoiceDate = Normalize(bill.InvoiceDate);
            bill.PreparedBy = Normalize(bill.PreparedBy);
            bill.Remarks = Normalize(bill.Remarks);
            bill.AccidentFlag = NormalizeFlag(bill.AccidentFlag);
            bill.SurgicalFlag = NormalizeFlag(bill.SurgicalFlag);
            bill.TotAmountDue = Normalize(bill.TotAmountDue);

            if (string.IsNullOrEmpty(bill.CardNo))
                return ValidationFailed(response, "cardNo cannot be empty.");
            if (bill.CardNo.Length != 16 || !ContainsDigitsOnly(bill.CardNo))
                return ValidationFailed(response, "cardNo must contain exactly 16 digits.", "1997");
            if (bill.FinalFlag != "Y" && bill.FinalFlag != "N")
                return ValidationFailed(response, "finalFlag must be Y or N.", "1997");
            if (!IsValidDdMmYyyyTime(bill.DateAdmission))
                return ValidationFailed(response, "dateAdmission must use DDMMYYYY HH24MISS format.", "1814");

            if (bill.FinalFlag == "Y")
            {
                if (!IsValidDdMmYyyyTime(bill.DateDischarge))
                    return ValidationFailed(response,
                        "dateDischarge is required in DDMMYYYY HH24MISS format when finalFlag is Y.", "1815");
            }
            else if (!string.IsNullOrEmpty(bill.DateDischarge) && !IsValidDdMmYyyyTime(bill.DateDischarge))
            {
                return ValidationFailed(response, "dateDischarge must use DDMMYYYY HH24MISS format.", "1815");
            }

            if (!string.IsNullOrEmpty(bill.InvoiceDate) && !IsValidDdMmYyyy(bill.InvoiceDate))
                return ValidationFailed(response, "invoiceDate must use DDMMYYYY format.", "1829");
            if (string.IsNullOrEmpty(bill.PreparedBy))
                return ValidationFailed(response, "preparedBy cannot be empty.");
            if (bill.PreparedBy.Length > 25)
                return ValidationFailed(response, "preparedBy cannot exceed 25 characters.", "1997");
            if (!string.IsNullOrEmpty(bill.Remarks) && bill.Remarks.Length > 500)
                return ValidationFailed(response, "remarks cannot exceed 500 characters.", "1997");
            if (string.IsNullOrEmpty(bill.TotAmountDue))
                return ValidationFailed(response, "totAmountDue cannot be empty.");
            if (!IsOptionalFlagValid(bill.AccidentFlag))
                return ValidationFailed(response, "accidentFlag must be Y or N.", "1997");
            if (!IsOptionalFlagValid(bill.SurgicalFlag))
                return ValidationFailed(response, "surgicalFlag must be Y or N.", "1997");

            if (bill.ItemizedBill == null || bill.ItemizedBill.Length == 0)
                return ValidationFailed(response, "itemizedBill cannot be empty.");

            var totItems = 0;

            foreach (var group in bill.ItemizedBill)
            {
                if (group == null)
                    return ValidationFailed(response, "itemizedBill cannot contain empty group.");

                group.GroupCode = Normalize(group.GroupCode);
                group.GroupName = Normalize(group.GroupName);

                if (string.IsNullOrEmpty(group.GroupCode))
                    return ValidationFailed(response, "groupCode cannot be empty.");
                if (string.IsNullOrEmpty(group.GroupName))
                    return ValidationFailed(response, "groupName cannot be empty.");
                if (group.ItemList == null || group.ItemList.Length == 0)
                    return ValidationFailed(response, "itemList cannot be empty for group " + group.GroupCode + ".");

                foreach (var item in group.ItemList)
                {
                    if (item == null)
                        return ValidationFailed(response, "itemList cannot contain empty item.");

                    item.Code = Normalize(item.Code);
                    item.Name = Normalize(item.Name);
                    item.DateIssue = Normalize(item.DateIssue);
                    item.Qty = Normalize(item.Qty);
                    item.TotPrice = Normalize(item.TotPrice);
                    item.Discount = Normalize(item.Discount);
                    item.NonMedicalFlag = NormalizeFlag(item.NonMedicalFlag);
                    item.BenID = Normalize(item.BenID);
                    item.Remarks = Normalize(item.Remarks);

                    if (string.IsNullOrEmpty(item.Code))
                        return ValidationFailed(response, "item code cannot be empty.");
                    if (string.IsNullOrEmpty(item.Name))
                        return ValidationFailed(response, "item name cannot be empty.");
                    if (string.IsNullOrEmpty(item.Qty))
                        return ValidationFailed(response, "item qty cannot be empty.");
                    if (string.IsNullOrEmpty(item.TotPrice))
                        return ValidationFailed(response, "item totPrice cannot be empty.");
                    if (!IsOptionalFlagValid(item.NonMedicalFlag))
                        return ValidationFailed(response, "item nonMedicalFlag must be Y or N.", "1997");
                    if (!string.IsNullOrEmpty(item.Remarks) && item.Remarks.Length > 500)
                        return ValidationFailed(response, "item remarks cannot exceed 500 characters.", "1997");

                    totItems++;
                }
            }

            bill.TerminalID = _config.TerminalID;
            bill.TotItems = totItems.ToString(CultureInfo.InvariantCulture);

            return Send<FinalBillResponse>(
                AdmedikaConstant.ServiceID.FinalBill,
                new FinalBillTxnData
                {
                    Request = new FinalBillRequest
                    {
                        FinalBill = bill
                    }
                });
        }

        public DischargeIpResponse DischargeIp(string cardNo, string clID)
        {
            var response = new DischargeIpResponse();
            if (!RequireTerminalId(response)) return response;

            if (!ValidateCardAndClaim(response, ref cardNo, ref clID)) return response;

            return Send<DischargeIpResponse>(
                AdmedikaConstant.ServiceID.DischargeIp,
                new DischargeIpTxnData
                {
                    Request = new DischargeIpRequest
                    {
                        Discharge = new DischargeIpPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo,
                            ClID = clID
                        }
                    }
                });
        }

        public PreAssessmentStatusResponse PreAssessmentStatus(string cardNo, string clID)
        {
            var response = new PreAssessmentStatusResponse();
            if (!RequireTerminalId(response)) return response;

            if (!ValidateCardAndClaim(response, ref cardNo, ref clID)) return response;

            return Send<PreAssessmentStatusResponse>(
                AdmedikaConstant.ServiceID.PreAssessmentStatus,
                new PreAssessmentStatusTxnData
                {
                    Request = new PreAssessmentStatusRequest
                    {
                        PreAssessmentStatus = new PreAssessmentStatusPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo,
                            ClID = clID
                        }
                    }
                });
        }

        private bool ValidateCardAndClaim(AdmedikaResponse response, ref string cardNo, ref string clID)
        {
            cardNo = Normalize(cardNo);
            clID = Normalize(clID);

            if (string.IsNullOrEmpty(cardNo))
                return SetValidationError(response, "cardNo cannot be empty.");
            if (cardNo.Length != 16 || !ContainsDigitsOnly(cardNo))
                return SetValidationError(response, "cardNo must contain exactly 16 digits.", "1997");
            if (string.IsNullOrEmpty(clID))
                return SetValidationError(response, "clID cannot be empty.");

            return true;
        }

        private static bool ValidatePreRegisterEbillPeriod(AdmedikaResponse response,
            ref string payorCode, ref string coverage, ref string dateFrom, ref string dateTo)
        {
            payorCode = Normalize(payorCode);
            coverage = NormalizeFlag(coverage);
            dateFrom = Normalize(dateFrom);
            dateTo = Normalize(dateTo);

            if (string.IsNullOrEmpty(payorCode))
                return SetValidationError(response, "payorCode cannot be empty.");
            if (coverage != "RJ" && coverage != "RI")
                return SetValidationError(response, "coverage must be RJ or RI.", "1997");
            if (!IsValidDdMmYyyy(dateFrom))
                return SetValidationError(response, "dateFrom must use DDMMYYYY format.", "1829");
            if (!IsValidDdMmYyyy(dateTo))
                return SetValidationError(response, "dateTo must use DDMMYYYY format.", "1829");

            return true;
        }

        private static bool IsValidDdMmYyyy(string value)
        {
            DateTime parsed;
            return !string.IsNullOrEmpty(value) &&
                   DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out parsed);
        }

        private static bool IsValidDdMmYyyyTime(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            DateTime parsed;
            return DateTime.TryParseExact(value, "ddMMyyyy HHmmss", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out parsed)
                   || DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out parsed);
        }

        private static TResponse ValidationFailed<TResponse>(TResponse response, string message,
            string errorCode = "1999")
            where TResponse : AdmedikaResponse
        {
            SetValidationError(response, message, errorCode);
            return response;
        }

        private bool RequireDocType(string docType, AdmedikaResponse response, out string normalized)
        {
            normalized = docType == null ? null : docType.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                response.ErrorCode = "1999";
                response.ErrorDesc = "docType cannot be empty.";
                response.StatusMsg = response.ErrorDesc;
                return false;
            }

            var type = AdmedikaDocType.Get(normalized);
            if (type == null)
            {
                response.ErrorCode = "1997";
                response.ErrorDesc = "Invalid docType. Allowed: "
                                     + AdmedikaDocType.AllowedText() + ".";
                response.StatusMsg = response.ErrorDesc;
                return false;
            }

            normalized = type.Code;
            return true;
        }

        public EdocsUploadResponse EdocsUpload(string cardNo, string clID, string docType, string remarks,
            string patientName, string userBy, AdmedikaFile[] files)
        {
            var response = new EdocsUploadResponse();
            string normalizedDocType;
            if (!RequireDocType(docType, response, out normalizedDocType)) return response;

            return SendMultipart<EdocsUploadResponse>(
                AdmedikaConstant.ServiceID.EdocsUpload,
                new EdocsUploadTxnData
                {
                    Request = new EdocsUploadRequest
                    {
                        EdocsUpload = new EdocsUploadPayload
                        {
                            CardNo = cardNo,
                            ClID = clID,
                            DocType = normalizedDocType,
                            Remarks = remarks,
                            PatientName = patientName,
                            UserBy = userBy
                        }
                    }
                },
                files);
        }

        public InvoiceTrackingResponse InvoiceTracking(string invoiceNo, string dateName, string dateFrom,
            string dateTo)
        {
            var response = new InvoiceTrackingResponse();
            if (!RequireTerminalId(response)) return response;

            return Send<InvoiceTrackingResponse>(
                AdmedikaConstant.ServiceID.InvoiceTracking,
                new InvoiceTrackingTxnData
                {
                    Request = new InvoiceTrackingRequest
                    {
                        InvoiceTracking = new InvoiceTrackingPayload
                        {
                            TerminalID = _config.TerminalID,
                            InvoiceNo = invoiceNo,
                            DateName = dateName,
                            DateFrom = dateFrom,
                            DateTo = dateTo
                        }
                    }
                });
        }

        public InvoiceTrackingDetailsResponse InvoiceTrackingDetails(string payorCode, string invoiceNo)
        {
            var response = new InvoiceTrackingDetailsResponse();
            if (!RequireTerminalId(response)) return response;

            payorCode = Normalize(payorCode);
            invoiceNo = Normalize(invoiceNo);

            if (string.IsNullOrEmpty(payorCode))
                return ValidationFailed(response, "payorCode cannot be empty.");
            if (string.IsNullOrEmpty(invoiceNo))
                return ValidationFailed(response, "invoiceNo cannot be empty.");

            return Send<InvoiceTrackingDetailsResponse>(
                AdmedikaConstant.ServiceID.InvoiceTrackingDetails,
                new InvoiceTrackingDetailsTxnData
                {
                    Request = new InvoiceTrackingDetailsRequest
                    {
                        InvoiceTrackingDetails = new InvoiceTrackingDetailsPayload
                        {
                            TerminalID = _config.TerminalID,
                            PayorCode = payorCode,
                            InvoiceNo = invoiceNo
                        }
                    }
                });
        }

        public RePrintOutResponse RePrintOut(string clID, string rePrintType)
        {
            return Send<RePrintOutResponse>(
                AdmedikaConstant.ServiceID.RePrintOut,
                new RePrintOutTxnData
                {
                    Request = new RePrintOutRequest
                    {
                        RePrintOut = new RePrintOutPayload
                        {
                            ClID = clID,
                            RePrintType = rePrintType
                        }
                    }
                });
        }

        private TResponse Send<TResponse>(string serviceId, object txnData)
            where TResponse : AdmedikaResponse, new()
        {
            var body = BuildBody(serviceId, txnData);
            return SendCore<TResponse>(serviceId, body,
                () => _transport.Post(_config.ServiceUrl, body, _config.TimeoutSeconds));
        }

        private TResponse SendMultipart<TResponse>(string serviceId, object txnData, AdmedikaFile[] files)
            where TResponse : AdmedikaResponse, new()
        {
            var body = BuildBody(serviceId, txnData);
            return SendCore<TResponse>(serviceId, body,
                () => _transport.PostMultipart(_config.ApiServiceUrl, body, files, _config.TimeoutSeconds));
        }

        public static string FormatRequestDateTime(DateTime utcNow, string timeZoneId)
        {
            var value = utcNow;

            if (!string.IsNullOrWhiteSpace(timeZoneId))
            {
                try
                {
                    var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                    value = TimeZoneInfo.ConvertTimeFromUtc(utcNow, zone);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }

            return value.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        private string BuildBody(string serviceId, object txnData)
        {
            var requestDateTime = FormatRequestDateTime(DateTime.UtcNow, _config.TimeZoneId);
            var requestId = RequestIdGenerator.Next();

            var request = new AdmedikaRequest<object>
            {
                TokenAuth = TokenGenerator.CreateToken(
                    _config.CustomerID, _config.SecurityWord, requestDateTime, requestId, serviceId),
                ServiceID = serviceId,
                CustomerID = _config.CustomerID,
                RequestID = requestId,
                TxnData = txnData,
                TxnRequestDateTime = requestDateTime
            };

            return JsonConvert.SerializeObject(new { input = request }, SerializerSettings);
        }

        private TResponse SendCore<TResponse>(string serviceId, string body, Func<string> send)
            where TResponse : AdmedikaResponse, new()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var raw = send();
                stopwatch.Stop();

                var response = ResponseParser.Parse<TResponse>(raw);
                WriteLog(serviceId, body, raw, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (AdmedikaTransportException ex)
            {
                stopwatch.Stop();

                var failed = string.IsNullOrWhiteSpace(ex.ResponseBody)
                    ? new TResponse()
                    : ResponseParser.Parse<TResponse>(ex.ResponseBody);

                if (string.IsNullOrEmpty(failed.TechnicalError)) failed.TechnicalError = ex.Message;
                if (string.IsNullOrEmpty(failed.StatusMsg)) failed.StatusMsg = ex.Message;

                WriteLog(serviceId, body, ex.ResponseBody, stopwatch.ElapsedMilliseconds);
                return failed;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                var failed = new TResponse
                {
                    TechnicalError = ex.Message,
                    StatusMsg = ex.Message
                };

                WriteLog(serviceId, body, null, stopwatch.ElapsedMilliseconds);
                return failed;
            }
        }

        private void WriteLog(string serviceId, string requestBody, string responseBody, long elapsedMs)
        {
            if (_logger == null) return;

            try
            {
                _logger.Log(serviceId, requestBody, responseBody, elapsedMs);
            }
            catch (Exception)
            {
                // Logging tidak boleh menggagalkan transaksi.
            }
        }
    }
}
