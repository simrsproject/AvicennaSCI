namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaCertificateStatusRules
    {
        public static bool CanApply(int? current, int next)
        {
            if (next < TilakaConstant.CertificateStatus.None || next > TilakaConstant.CertificateStatus.Rejected) return false;
            if (!current.HasValue || current.Value <= TilakaConstant.CertificateStatus.None) return true;
            if (current.Value == TilakaConstant.CertificateStatus.Active)
                return next == TilakaConstant.CertificateStatus.Active
                    || next == TilakaConstant.CertificateStatus.None
                    || next == TilakaConstant.CertificateStatus.Rejected;

            if (current.Value == TilakaConstant.CertificateStatus.Rejected)
                return next == TilakaConstant.CertificateStatus.Rejected;

            return next >= current.Value;
        }
    }
}
