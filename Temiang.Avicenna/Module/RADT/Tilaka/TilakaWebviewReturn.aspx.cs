using System;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaWebviewReturn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            try
            {
                ProcessReturn();
            }
            catch (Exception)
            {
                lblMessage.Text = "Gagal memproses hasil aktivasi Tilaka. Silakan gunakan tombol Cek Status di Avicenna.";
            }
        }

        private void ProcessReturn()
        {
            var requestId = Request.QueryString["request-id"];

            if (string.IsNullOrWhiteSpace(requestId))
            {
                lblMessage.Text = "Parameter request-id tidak ditemukan.";
                return;
            }

            var repository = new TilakaRegistrationRepository();
            var record = repository.FindByRequestId(requestId);
            if (record == null)
            {
                lblMessage.Text = "Data pendaftaran tidak ditemukan.";
                return;
            }

            var service = new Service();
            var name = record.TilakaName;
            if (string.IsNullOrWhiteSpace(name))
            {
                lblMessage.Text = "Nama Tilaka belum tersedia. Silakan gunakan tombol Cek Status di Avicenna.";
                return;
            }

            var certificate = service.CheckCertificateStatus(name);
            string message;
            if (!service.IsOk(certificate, out message))
            {
                lblMessage.Text = message;
                return;
            }

            record.TilakaName = name;
            record.CertificateStatus = certificate.Status;
            record.CertificateStatusInfo = certificate.Message == null ? null : certificate.Message.Info;
            record.LastErrorMessage = null;
            record.LastCheckedDateTime = DateTime.Now;
            record.LastCheckSource = TilakaConstant.LogSource.Callback;
            record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            repository.Save(record);

            lblMessage.Text = "Aktivasi selesai. Status: " + record.RegistrationState;
        }
    }
}
