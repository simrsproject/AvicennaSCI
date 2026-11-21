using System;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaUserProfileProvider
    {
        string GetEmail(string paramedicID);
        string GetSignatureImageBase64(string paramedicID);
    }

    public class TilakaUserProfileProvider : ITilakaUserProfileProvider
    {
        public string GetEmail(string paramedicID)
        {
            if (string.IsNullOrWhiteSpace(paramedicID)) return null;
            var paramedic = new Paramedic();
            return paramedic.LoadByPrimaryKey(paramedicID) ? paramedic.Email : null;
        }

        public string GetSignatureImageBase64(string paramedicID)
        {
            if (string.IsNullOrWhiteSpace(paramedicID)) return null;

            var query = new AppUserQuery();
            query.Where(query.ParamedicID == paramedicID);
            query.es.Top = 1;

            var user = new AppUser();
            if (!user.Load(query)) return null;
            if (user.SignatureImage == null || user.SignatureImage.Length == 0) return null;

            return Convert.ToBase64String(user.SignatureImage);
        }
    }
}
