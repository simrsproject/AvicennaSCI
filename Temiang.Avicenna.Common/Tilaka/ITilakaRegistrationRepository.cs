using System.Collections.Generic;

namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaRegistrationRepository
    {
        TilakaRegistrationRecord FindByRequestId(string requestId);
        TilakaRegistrationRecord FindLatest(string relatedType, string relatedId);
        IList<TilakaRegistrationRecord> FindPending(int max);
        void Save(TilakaRegistrationRecord record);
    }
}
