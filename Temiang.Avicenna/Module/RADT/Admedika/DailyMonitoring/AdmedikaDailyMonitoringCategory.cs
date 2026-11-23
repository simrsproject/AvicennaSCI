using System.Collections.Generic;
using System.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public sealed class AdmedikaDailyMonitoringCategory
    {
        public int Code { get; private set; }
        public string Name { get; private set; }
        public string DefaultFormCode { get; private set; }

        private AdmedikaDailyMonitoringCategory(int code, string name, string defaultFormCode)
        {
            Code = code;
            Name = name;
            DefaultFormCode = defaultFormCode;
        }

        private static readonly AdmedikaDailyMonitoringCategory[] Items =
        {
            new AdmedikaDailyMonitoringCategory(1, "Plan / Kamar",
                AdmedikaConstant.DailyMonitoringFormID.DCM003C),
            new AdmedikaDailyMonitoringCategory(2, "JCL (Jaminan CL)",
                AdmedikaConstant.DailyMonitoringFormID.DCM004A),
            new AdmedikaDailyMonitoringCategory(4, "Bill Sementara",
                AdmedikaConstant.DailyMonitoringFormID.DCM006A),
            new AdmedikaDailyMonitoringCategory(5, "Dokter",
                AdmedikaConstant.DailyMonitoringFormID.DCM007A),
            new AdmedikaDailyMonitoringCategory(6, "eDoc / Petugas",
                AdmedikaConstant.DailyMonitoringFormID.DCM008A),
            new AdmedikaDailyMonitoringCategory(7, "Diagnosa",
                AdmedikaConstant.DailyMonitoringFormID.DCM012A),
            new AdmedikaDailyMonitoringCategory(8, "Anamnesa / Keluhan",
                AdmedikaConstant.DailyMonitoringFormID.DCM010A),
            new AdmedikaDailyMonitoringCategory(9, "Vital Sign",
                AdmedikaConstant.DailyMonitoringFormID.DCM011A),
            new AdmedikaDailyMonitoringCategory(10, "Lab / Penunjang",
                AdmedikaConstant.DailyMonitoringFormID.DCM011H),
            new AdmedikaDailyMonitoringCategory(11, "Indikasi RI",
                AdmedikaConstant.DailyMonitoringFormID.DCM014B),
            new AdmedikaDailyMonitoringCategory(12, "Obat",
                AdmedikaConstant.DailyMonitoringFormID.DCM013A),
            new AdmedikaDailyMonitoringCategory(15, "Approval",
                AdmedikaConstant.DailyMonitoringFormID.DCM002A)
        };

        public static IReadOnlyList<AdmedikaDailyMonitoringCategory> All
        {
            get { return Items; }
        }

        public static AdmedikaDailyMonitoringCategory Get(int code)
        {
            return Items.FirstOrDefault(item => item.Code == code);
        }

        public static string NameOf(int code)
        {
            var category = Get(code);
            return category == null ? code.ToString() : category.Name;
        }
    }
}
