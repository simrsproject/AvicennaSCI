using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Common;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Module.RADT.Emr.AssessmentCtl;
using Temiang.Avicenna.Module.RADT.EmrIp;
using Temiang.Dal.DynamicQuery;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Module.RADT.Emr
{
    public partial class IntegratedNoteHist : BasePageDialog
    {
        protected void grdAssessment_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var filterEntry = string.Empty;
            grdAssessment.DataSource = null;
            grdAssessment.DataSource = EmrIpDetail.RegistrationInfoMedicDataTable(RegistrationType, RegistrationNo, MergeRegistrations, PatientID, filterEntry, true);
        }
    }
}
