using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;
using AdmedikaService = Temiang.Avicenna.Common.Admedika.Service;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    internal static class AdmedikaEdocsHelper
    {
        public const int MaxFileSizeBytes = 10 * 1024 * 1024;

        public static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };

        public static string GetRootPath()
        {
            return new AdmedikaConfig().UploadDocumentPath ?? string.Empty;
        }

        public static string GetVirtualDocUrl()
        {
            var url = new AdmedikaConfig().VirtualDocUrl ?? string.Empty;
            return url.TrimEnd('/');
        }

        /// <summary>
        /// URL preview file melalui virtual directory yang menunjuk ke folder upload.
        /// </summary>
        public static string BuildFileUrl(AdmedikaClaimTransaction claim, string fileName)
        {
            var baseUrl = GetVirtualDocUrl();
            if (string.IsNullOrWhiteSpace(baseUrl) || claim == null) return string.Empty;

            var cardNo = SanitizeFolderName(claim.CardNo);
            var clID = SanitizeFolderName(claim.ClID);

            return string.Format("{0}/{1}/{2}/{3}",
                baseUrl,
                Uri.EscapeDataString(string.IsNullOrEmpty(cardNo) ? "UNKNOWN" : cardNo),
                Uri.EscapeDataString(string.IsNullOrEmpty(clID) ? "UNKNOWN" : clID),
                Uri.EscapeDataString(SanitizeFileName(fileName)));
        }

        public static string RemoveSpaces(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : Regex.Replace(value, @"\s+", string.Empty);
        }

        public static string SanitizeFolderName(string value)
        {
            var name = RemoveSpaces(value);

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                name = name.Replace(invalidChar.ToString(), string.Empty);

            return name.Trim('.', ' ');
        }

        public static string GetDocumentFolder(AdmedikaClaimTransaction claim)
        {
            var cardNo = SanitizeFolderName(claim.CardNo);
            var clID = SanitizeFolderName(claim.ClID);

            return Path.Combine(
                GetRootPath(),
                string.IsNullOrEmpty(cardNo) ? "UNKNOWN" : cardNo,
                string.IsNullOrEmpty(clID) ? "UNKNOWN" : clID);
        }

        public static string SanitizeFileName(string fileName)
        {
            var name = Path.GetFileName(fileName ?? string.Empty);
            name = RemoveSpaces(name);

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                name = name.Replace(invalidChar.ToString(), string.Empty);

            name = name.Trim('.', ' ');

            return string.IsNullOrEmpty(name) ? "document" : name;
        }

        public static string EnsureUniqueFileName(string folder, string fileName)
        {
            if (!Directory.Exists(folder)) return fileName;

            var baseName = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var candidate = fileName;
            var index = 1;

            while (File.Exists(Path.Combine(folder, candidate)))
            {
                candidate = string.Format("{0}_{1}{2}", baseName, index, extension);
                index++;
            }

            return candidate;
        }

        public static bool IsAllowedExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }

        public static string GetContentType(string fileName)
        {
            switch (Path.GetExtension(fileName ?? string.Empty).ToLowerInvariant())
            {
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }

        public static string GetDocumentFilePath(AdmedikaDocument document, AdmedikaClaimTransaction claim)
        {
            if (document == null || claim == null) return string.Empty;

            return Path.Combine(GetDocumentFolder(claim), Path.GetFileName(document.FileName));
        }

        /// <summary>
        /// Mengirim dokumen ke AdMedika dan memperbarui status pada baris AdmedikaDocument.
        /// </summary>
        public static string Send(AdmedikaDocument document, AdmedikaClaimTransaction claim, out bool success)
        {
            success = false;

            if (document == null) return "Dokumen tidak ditemukan.";
            if (claim == null) return "Data klaim tidak ditemukan.";
            if (string.IsNullOrWhiteSpace(claim.CardNo) || string.IsNullOrWhiteSpace(claim.ClID))
                return "Card No / ClID klaim tidak lengkap.";

            var filePath = GetDocumentFilePath(document, claim);
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return "File dokumen tidak ditemukan di folder upload.";

            try
            {
                var file = new AdmedikaFile
                {
                    FileName = Path.GetFileName(document.FileName),
                    ContentType = GetContentType(document.FileName),
                    Content = File.ReadAllBytes(filePath)
                };

                var service = new AdmedikaService();
                var result = service.EdocsUpload(
                    claim.CardNo,
                    claim.ClID,
                    document.DocType,
                    string.Empty,
                    claim.PatientName,
                    AppSession.UserLogin.UserID,
                    new[] { file });

                string message;
                success = service.IsOk(result, out message);

                document.IsSentSuccess = success;
                document.SentDateTime = DateTime.Now;
                document.SentByUserID = AppSession.UserLogin.UserID;
                document.ResponseMessage = Truncate(
                    success
                        ? (string.IsNullOrWhiteSpace(result.StatusMsg) ? "Success" : result.StatusMsg)
                        : message,
                    500);
                document.LastUpdateDateTime = DateTime.Now;
                document.LastUpdateByUserID = AppSession.UserLogin.UserID;
                document.Save();

                return success ? "Dokumen berhasil dikirim." : message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
            return value.Substring(0, maxLength);
        }
    }
}
