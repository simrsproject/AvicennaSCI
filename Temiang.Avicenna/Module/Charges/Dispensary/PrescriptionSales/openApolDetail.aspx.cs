using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.Charges.Dispensary.PrescriptionSales
{
    public partial class openApolDetail : BasePageDialog
    {
        public string PrescriptionNo => Request.QueryString["pno"];
        public string RegistrationNo => Request.QueryString["rno"];

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ButtonOk.Visible = false;
                ButtonCancel.Text = "Close";
                LoadHeader();
            }
        }

        private void LoadHeader()
        {
            var apol = new BpjsApol();
            apol.Query.Where(
                apol.Query.PrescriptionNo == PrescriptionNo,
                apol.Query.RegistrationNo == RegistrationNo
            );

            if (apol.Query.Load())
            {
                txtNOSJP.Text = apol.NOAPOTIK;
                txtNORESEP.Text = apol.NORESEP;
            }
        }

        private BpjsApol GetApolHeader()
        {
            var apol = new BpjsApol();

            apol.Query.Where(
                apol.Query.PrescriptionNo == PrescriptionNo,
                apol.Query.RegistrationNo == RegistrationNo
            );

            return apol.Query.Load() ? apol : null;
        }

        private BpjsApolDetail GetApolDetail(BpjsApol apol, string sequenceNo)
        {
            var det = new BpjsApolDetail();

            det.Query.Where(
                det.Query.PrescriptionNo == PrescriptionNo,
                det.Query.NOSJP == apol.NOAPOTIK,
                det.Query.SequenceNo == sequenceNo
            );

            return det.Query.Load() ? det : null;
        }

        private void UpdateDetailStatus(BpjsApolDetail det, string code, string message)
        {
            det.METADATACODE = code ?? "";
            det.METADATAMESSAGE = message ?? "";
            det.LastUpdateDateTime = DateTime.Now;
            det.LastUpdateByUserID = AppSession.UserLogin.UserID;
            det.Save();
        }

        private void ShowApolMessage(string message)
        {
            string encoded = HttpUtility.JavaScriptStringEncode(message ?? "");

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "apolMsg_" + Guid.NewGuid().ToString("N"),
                "alert('" + encoded + "');",
                true
            );
        }

        private static string ToApolNumber(object value)
        {
            return Convert.ToString(
                value,
                CultureInfo.InvariantCulture
            );
        }

        private static bool IsDeletedStatus(string code)
        {
            return (code ?? "").Trim().StartsWith(
                "DEL-",
                StringComparison.OrdinalIgnoreCase
            );
        }

        private static bool IsUncertainStatus(string code)
        {
            code = (code ?? "").Trim();

            return code == "0"
                || code == "504"
                || code.Equals("Catch", StringComparison.OrdinalIgnoreCase)
                || code.Equals("TIMEOUT", StringComparison.OrdinalIgnoreCase);
        }

        protected void grdApolDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var apolDet = new BpjsApolDetailCollection();

            var apol = GetApolHeader();

            if (apol != null)
            {
                apolDet.Query.Where(
                    apolDet.Query.PrescriptionNo == PrescriptionNo,
                    apolDet.Query.NOSJP == apol.NOAPOTIK
                );

                apolDet.Query.OrderBy(
                    apolDet.Query.SequenceNo.Ascending
                );

                apolDet.Query.Load();
            }

            grdApolDetail.DataSource = apolDet;
        }

        protected void grdApolDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;

            if (item == null)
                return;

            string code = Convert.ToString(
                DataBinder.Eval(item.DataItem, "MetadataCode")
            )?.Trim() ?? "";

            var statusCell = item["MetadataCode"];

            var btnResend = item.FindControl("btnResend") as LinkButton;
            var btnDelete = item.FindControl("btnDeleteApol") as LinkButton;

            bool isSent = code == "200";
            bool isDeleted = IsDeletedStatus(code);
            bool isUncertain = IsUncertainStatus(code);

            bool isKnownUnsent =
                code == "201" ||
                code == "404" ||
                code == "PENDING-NOMAP";

            if (btnResend != null)
            {
                btnResend.Visible =
                    !isSent &&
                    !isDeleted &&
                    !isUncertain &&
                    isKnownUnsent;
            }

            if (btnDelete != null)
            {
                btnDelete.Visible =
                    !isDeleted &&
                    !isUncertain &&
                    (isSent || isKnownUnsent);
            }

            switch (code)
            {
                case "200":
                    statusCell.Text = "SENT";
                    statusCell.Style["color"] = "green";
                    break;

                case "201":
                    statusCell.Text = "NOT SENT";
                    statusCell.Style["color"] = "orange";
                    break;

                case "404":
                    statusCell.Text = "FAILED";
                    statusCell.Style["color"] = "red";
                    break;

                case "DEL-200":
                    statusCell.Text = "DELETED APOL";
                    statusCell.Style["color"] = "gray";
                    break;

                case "DEL-LOCAL":
                    statusCell.Text = "DELETED LOCAL";
                    statusCell.Style["color"] = "gray";
                    break;

                default:
                    statusCell.Text = HttpUtility.HtmlEncode(
                        string.IsNullOrEmpty(code) ? "-" : code
                    );

                    break;
            }

            statusCell.Style["font-weight"] = "bold";
        }

        protected void grdApolDetail_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName != "ResendApol" &&
                e.CommandName != "DeleteApol")
                return;

            var item = e.Item as GridDataItem;

            if (item == null)
                return;

            string sequenceNo = Convert.ToString(
                item.GetDataKeyValue("SequenceNo")
            );

            if (string.IsNullOrWhiteSpace(sequenceNo))
            {
                ShowApolMessage("SequenceNo tidak ditemukan.");
                return;
            }

            try
            {
                if (e.CommandName == "ResendApol")
                {
                    ResendApolDetail(sequenceNo);
                }
                else if (e.CommandName == "DeleteApol")
                {
                    DeleteApolDetail(sequenceNo);
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                ShowApolMessage("APOL: " + ex.Message);
            }
            finally
            {
                grdApolDetail.Rebind();
            }
        }

        private void ResendApolDetail(string sequenceNo)
        {
            var apol = GetApolHeader();

            if (apol == null)
                throw new Exception("Header APOL tidak ditemukan.");

            var det = GetApolDetail(apol, sequenceNo);

            if (det == null)
                throw new Exception("Detail APOL tidak ditemukan.");

            string code = (det.METADATACODE ?? "").Trim();

            if (code == "200")
                throw new Exception("Obat sudah terkirim ke APOL.");

            if (IsDeletedStatus(code))
                throw new Exception("Obat sudah ditandai deleted.");

            if (IsUncertainStatus(code))
                throw new Exception(
                    "Status pengiriman belum pasti. Verifikasi ke BPJS dahulu."
                );

            if (code != "201" &&
                code != "404" &&
                code != "PENDING-NOMAP")
            {
                throw new Exception(
                    "Status " + code + " belum diizinkan untuk resend."
                );
            }

            // Sama seperti pengiriman awal, hanya resep approved.
            var prescription = new TransPrescription();

            if (!prescription.LoadByPrimaryKey(PrescriptionNo) ||
                !(prescription.IsApproval ?? false))
            {
                throw new Exception(
                    "Resep belum approved. Resend APOL dibatalkan."
                );
            }

            string nosjp = Convert.ToString(apol.NOAPOTIK)?.Trim();
            string noresep = Convert.ToString(apol.NORESEP)?.Trim();

            if (string.IsNullOrWhiteSpace(nosjp) ||
                string.IsNullOrWhiteSpace(noresep))
            {
                throw new Exception("NOSJP atau NORESEP belum tersedia.");
            }

            string kdobt = (det.KDOBT ?? "").Trim();
            string nmobat = (det.NMOBAT ?? "").Trim();
            string jnsrobt = (det.JNSROBT ?? "").Trim();

            if (string.IsNullOrWhiteSpace(kdobt) ||
                string.IsNullOrWhiteSpace(nmobat))
            {
                throw new Exception(
                    "Kode atau nama obat belum tersedia. " +
                    "Perbaiki mapping dan detail lokal dahulu."
                );
            }

            if (string.IsNullOrWhiteSpace(jnsrobt))
            {
                throw new Exception(
                    "Jenis obat JNSROBT belum tersedia."
                );
            }

            bool isCompound = jnsrobt.StartsWith(
                "R.",
                StringComparison.OrdinalIgnoreCase
            );

            if (!isCompound && jnsrobt != "N")
            {
                throw new Exception(
                    "Jenis obat tidak dikenali: " + jnsrobt
                );
            }

            var svc = new Temiang.Avicenna.Common.BPJS.Apotek.Service();

            string metaCode = "";
            string metaMsg = "";

            try
            {
                if (isCompound)
                {
                    var req =
                        new Temiang.Avicenna.Common.BPJS.Apotek.Obat.Racikan.Request.Root
                        {
                            Nosjp = nosjp,
                            Noresep = noresep,
                            Jnsrobt = jnsrobt,
                            Kdobt = kdobt,
                            Nmobat = nmobat,
                            Signa1obt = ToApolNumber(det.SIGNA1OBT),
                            Signa2obt = ToApolNumber(det.SIGNA2OBT),
                            Permintaan = ToApolNumber(det.PERMINTAAN),
                            Jmlobt = ToApolNumber(det.JMLOBT),
                            Jho = ToApolNumber(det.JHO),
                            Catkhsobt = Convert.ToString(det.CATKHSOBT)
                        };

                    var resp = svc.InsertRacikan(req);

                    metaCode = (resp?.Code ?? "").Trim();
                    metaMsg = (resp?.Message ?? "").Trim();
                }
                else
                {
                    var req =
                        new Temiang.Avicenna.Common.BPJS.Apotek.Obat.NonRacikan.Request.Root
                        {
                            Nosjp = nosjp,
                            Noresep = noresep,
                            Kdobt = kdobt,
                            Nmobat = nmobat,
                            Signa1obt = ToApolNumber(det.SIGNA1OBT),
                            Signa2obt = ToApolNumber(det.SIGNA2OBT),
                            Jmlobt = ToApolNumber(det.JMLOBT),
                            Jho = ToApolNumber(det.JHO),
                            Catkhsobt = Convert.ToString(det.CATKHSOBT)
                        };

                    var resp = svc.InsertNonRacikan(req);

                    metaCode = (resp?.Code ?? "").Trim();
                    metaMsg = (resp?.Message ?? "").Trim();
                }
            }
            catch (System.Net.WebException wex)
            {
                var http = wex.Response as System.Net.HttpWebResponse;

                int status = http != null
                    ? (int)http.StatusCode
                    : 0;

                UpdateDetailStatus(
                    det,
                    "VERIFY-REQUIRED",
                    "Pengiriman tidak dapat dipastikan. HTTP " +
                    status + ": " + wex.Message
                );

                throw new Exception(
                    "Pengiriman belum dapat dipastikan. " +
                    "Periksa status obat di APOL sebelum mengirim ulang."
                );
            }
            catch (Exception ex)
            {
                UpdateDetailStatus(
                    det,
                    "VERIFY-REQUIRED",
                    "Pengiriman tidak dapat dipastikan: " + ex.Message
                );

                throw;
            }

            if (string.IsNullOrWhiteSpace(metaCode))
            {
                UpdateDetailStatus(
                    det,
                    "VERIFY-REQUIRED",
                    "Response APOL tidak mempunyai status yang jelas."
                );

                throw new Exception(
                    "Response APOL tidak jelas. Verifikasi sebelum resend."
                );
            }

            UpdateDetailStatus(det, metaCode, metaMsg);

            if (metaCode == "200")
            {
                ShowApolMessage(
                    "Berhasil kirim ulang obat " + kdobt + " ke APOL."
                );
            }
            else
            {
                ShowApolMessage(
                    "APOL mengembalikan status " +
                    metaCode + ": " + metaMsg
                );
            }
        }

        private void DeleteApolDetail(string sequenceNo)
        {
            var apol = GetApolHeader();

            if (apol == null)
                throw new Exception("Header APOL tidak ditemukan.");

            var det = GetApolDetail(apol, sequenceNo);

            if (det == null)
                throw new Exception("Detail APOL tidak ditemukan.");

            string meta = (det.METADATACODE ?? "").Trim();

            if (IsDeletedStatus(meta))
                throw new Exception("Detail sudah dihapus.");

            if (IsUncertainStatus(meta) ||
                meta == "VERIFY-REQUIRED")
            {
                throw new Exception(
                    "Status obat belum pasti. Verifikasi ke BPJS dahulu."
                );
            }

            // Belum pernah berhasil dikirim.
            if (meta == "201" ||
                meta == "404" ||
                meta == "PENDING-NOMAP")
            {
                UpdateDetailStatus(
                    det,
                    "DEL-LOCAL",
                    "Dihapus lokal melalui APOL Detail."
                );

                ShowApolMessage("Detail ditandai deleted secara lokal.");
                return;
            }

            if (meta != "200")
            {
                throw new Exception(
                    "Status " + meta +
                    " belum diizinkan untuk penghapusan otomatis."
                );
            }

            // Sudah terkirim ke APOL, wajib hapus melalui API.
            string nosjp = Convert.ToString(apol.NOAPOTIK)?.Trim();
            string noresep = Convert.ToString(apol.NORESEP)?.Trim();
            string kdobt = (det.KDOBT ?? "").Trim();
            string jnsrobt = (det.JNSROBT ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nosjp) ||
                string.IsNullOrWhiteSpace(noresep) ||
                string.IsNullOrWhiteSpace(kdobt))
            {
                throw new Exception(
                    "NOSJP, NORESEP, atau Kode Obat tidak lengkap."
                );
            }

            if (string.IsNullOrWhiteSpace(jnsrobt))
            {
                throw new Exception("Jenis obat belum tersedia.");
            }

            string tipeObat;

            if (jnsrobt.StartsWith("R.", StringComparison.OrdinalIgnoreCase))
                tipeObat = "NSROBT";
            else if (jnsrobt == "N")
                tipeObat = "N";
            else
                throw new Exception("Jenis obat tidak dikenali.");

            var req =
                new Temiang.Avicenna.Common.BPJS.Apotek.PelayananObat.HapusPelayananObat.Request.Root
                {
                    Nosepapotek = nosjp,
                    Noresep = noresep,
                    Kodeobat = kdobt,
                    Tipeobat = tipeObat
                };

            var svc = new Temiang.Avicenna.Common.BPJS.Apotek.Service();

            try
            {
                var resp = svc.HapusPelayananObat(req);

                string code = (resp?.Metadata?.Code ?? "").Trim();
                string msg = (resp?.Metadata?.Message ?? "").Trim();

                if (code == "200")
                {
                    UpdateDetailStatus(
                        det,
                        "DEL-200",
                        string.IsNullOrWhiteSpace(msg)
                            ? "Deleted from APOL"
                            : msg
                    );

                    ShowApolMessage(
                        "Berhasil hapus obat " + kdobt + " dari APOL."
                    );
                }
                else if (string.IsNullOrWhiteSpace(code))
                {
                    UpdateDetailStatus(
                        det,
                        "VERIFY-DELETE",
                        "Respons penghapusan tidak jelas."
                    );

                    throw new Exception(
                        "Hasil penghapusan belum dapat dipastikan."
                    );
                }
                else
                {
                    UpdateDetailStatus(
                        det,
                        "DEL-" + code,
                        msg
                    );

                    ShowApolMessage(
                        "Gagal hapus di BPJS APOL. " +
                        code + ": " + msg
                    );
                }
            }
            catch (System.Net.WebException wex)
            {
                UpdateDetailStatus(
                    det,
                    "VERIFY-DELETE",
                    "Status penghapusan belum pasti: " + wex.Message
                );

                throw new Exception(
                    "Tidak dapat memastikan hasil hapus di BPJS. " +
                    "Lakukan verifikasi sebelum mengulang."
                );
            }
        }
    }
}