using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Temiang.Dal.DynamicQuery;
using System.Data;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.BusinessObject
{
    public partial class AppointmentQueueing
    {
        private bool _isGuarantorPrefix = AppParameter.GetParameterValue(AppParameter.ParameterItem.IsUsingGuarantorPrefixForQueueCodeKioskV2) == "Yes";
        private bool _isQueueByPhysician = AppParameter.GetParameterValue(AppParameter.ParameterItem.IsUsingQueueCodeByPhysicianKioskV2) == "Yes";

        private string GetFormattedNo(string SRQueueingType, ServiceUnit su, Paramedic par, int queNo)
        {
            var asri = new AppStandardReferenceItem();
            asri.LoadByPrimaryKey("QueueingType", SRQueueingType);

            if (_isGuarantorPrefix)
                if (_isQueueByPhysician)
                    return string.Format("{0}{1}{2}-{3}", asri.CustomField, su.QueueCode, par.ParamedicQueueCode, queNo.ToString().PadLeft(3, '0'));
                else
                    return string.Format("{0}{1}-{2}", asri.CustomField, su.QueueCode, queNo.ToString().PadLeft(3, '0'));
            else if (_isQueueByPhysician)
                return string.Format("{0}{1}-{2}", su.QueueCode, par.ParamedicQueueCode, queNo.ToString().PadLeft(3, '0'));
            else
                return string.Format("{0}-{1}", su.QueueCode, queNo.ToString().PadLeft(3, '0'));
        }

        private bool ValidatePoliQueueing(ServiceUnit su, bool IsFromApiAntrianV2)
        {
            if (string.IsNullOrEmpty(su.SrqueueinglocationPoli))
            {
                if (IsFromApiAntrianV2)
                {
                    throw new Exception(string.Format("Invalid service unit queueing location in service unit {0}", su.ServiceUnitName));
                }
                else
                {
                    return false;
                }
            }
            if (string.IsNullOrEmpty(su.QueueCode))
            {
                if (IsFromApiAntrianV2)
                {
                    throw new Exception(string.Format("Invalid queueing code in service unit {0}", su.QueueCode));
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool SetQueForReg(Appointment apt, string SRQueueingType, ServiceUnit su, string UserID, bool IsFromApiAntrianV2) {
            // validasi 
            if (string.IsNullOrEmpty(su.SrqueueinglocationReg)) {
                if (IsFromApiAntrianV2) { 
                    throw new Exception(string.Format("Invalid registration queueing location in service unit {0}", su.ServiceUnitName)); 
                }
                else {
                    return false;
                }
            }
            if (string.IsNullOrEmpty(su.QueueCode))
            {
                if (IsFromApiAntrianV2)
                {
                    throw new Exception(string.Format("Invalid queueing code in service unit {0}", su.QueueCode));
                }
                else {
                    return false;
                }       
            }

            var par = new Paramedic();
            par.LoadByPrimaryKey(apt.ParamedicID);

            this.AppointmentNo = apt.AppointmentNo;
            this.SRQueueingLocation = su.SrqueueinglocationReg;
            this.SRQueueingGroup = "01";
            this.SRQueueingType = SRQueueingType;

            this.FormattedNo = GetFormattedNo(SRQueueingType, su, par, apt.AppointmentQue.Value);

            this.QueueingDate = apt.AppointmentDate;
            this.SRKioskQueueStatus = "01";
            this.ServiceUnitID = su.ServiceUnitID;

            var cDate = (new DateTime()).NowAtSqlServer();

            this.CreateDateTime = cDate;
            this.CreateByUserID = UserID;
            this.LastUpdateDateTime = cDate;
            this.LastUpdateByUserID = UserID;
            this.ParamedicID = apt.ParamedicID;

            return true;
        }

        public bool SetQueForPoli(string apptNo, string UserID) {
            var regQueColl = new AppointmentQueueingCollection();
            regQueColl.Query.Where(regQueColl.Query.AppointmentNo == apptNo);
            if (regQueColl.LoadAll())
            {
                var query = new AppointmentQueueingQuery("aq");
                query.Where(query.AppointmentNo == apptNo, query.SRQueueingGroup == "02");
                var dtb = query.LoadDataTable();

                if (dtb.Rows.Count == 1) return false;

                var regQue = regQueColl.First();

                var su = new ServiceUnit();
                su.LoadByPrimaryKey(regQue.ServiceUnitID);

                this.AppointmentNo = regQue.AppointmentNo;
                this.SRQueueingLocation = su.SrqueueinglocationPoli;
                this.SRQueueingGroup = "02";
                this.SRQueueingType = regQue.SRQueueingType;
                this.FormattedNo = regQue.FormattedNo;
                this.QueueingDate = regQue.QueueingDate;
                this.SRKioskQueueStatus = "01";
                this.ServiceUnitID = regQue.ServiceUnitID;

                var cDate = (new DateTime()).NowAtSqlServer();

                this.CreateDateTime = cDate;
                this.CreateByUserID = UserID;
                this.LastUpdateDateTime = cDate;
                this.LastUpdateByUserID = UserID;
                this.ParamedicID = regQue.ParamedicID;

                return true;
            }
            else {
                return false;
            }
        }

        public bool SetQueForPoliByReg(Registration reg, string SRQueueingType, ServiceUnit su, string UserID, bool IsFromApiAntrianV2)
        {
            if (!ValidatePoliQueueing(su, IsFromApiAntrianV2))
                return false;

            var par = new Paramedic();
            par.LoadByPrimaryKey(reg.ParamedicID);

            var formattedNo = GetFormattedNo(SRQueueingType, su, par, reg.RegistrationQue.Value);

            var regQueColl = new AppointmentQueueingCollection();
            regQueColl.Query.Where(regQueColl.Query.AppointmentNo == reg.RegistrationNo);
            if (regQueColl.LoadAll())
            {
                var poliQue = regQueColl.FirstOrDefault(x => x.SRQueueingGroup == "02");
                if (poliQue == null)
                    return false;

                if (poliQue.FormattedNo == formattedNo &&
                    poliQue.SRQueueingLocation == su.SrqueueinglocationPoli &&
                    poliQue.SRQueueingType == SRQueueingType &&
                    poliQue.QueueingDate == reg.RegistrationDate &&
                    poliQue.ServiceUnitID == su.ServiceUnitID &&
                    poliQue.ParamedicID == reg.ParamedicID)
                    return false;

                this.LoadByPrimaryKey(poliQue.Id.Value);
                this.SRQueueingLocation = su.SrqueueinglocationPoli;
                this.SRQueueingType = SRQueueingType;
                this.FormattedNo = formattedNo;
                this.QueueingDate = reg.RegistrationDate;
                this.ServiceUnitID = su.ServiceUnitID;
                this.LastUpdateDateTime = (new DateTime()).NowAtSqlServer();
                this.LastUpdateByUserID = UserID;
                this.ParamedicID = reg.ParamedicID;

                return true;
            }

            this.AppointmentNo = reg.RegistrationNo;
            this.SRQueueingLocation = su.SrqueueinglocationPoli;
            this.SRQueueingGroup = "02";
            this.SRQueueingType = SRQueueingType;
            this.FormattedNo = formattedNo;

            this.QueueingDate = reg.RegistrationDate;
            this.SRKioskQueueStatus = "01";
            this.ServiceUnitID = su.ServiceUnitID;

            var createDate = (new DateTime()).NowAtSqlServer();

            this.CreateDateTime = createDate;
            this.CreateByUserID = UserID;
            this.LastUpdateDateTime = createDate;
            this.LastUpdateByUserID = UserID;
            this.ParamedicID = reg.ParamedicID;

            return true;
        }
        public bool UpdateQueForReg(Appointment apt, AppointmentQueueing apQue, string SRQueueingType, ServiceUnit su, string UserID, bool IsFromApiAntrianV2)
        {
            // validasi 
            if (string.IsNullOrEmpty(su.SrqueueinglocationReg))
            {
                if (IsFromApiAntrianV2)
                {
                    throw new Exception(string.Format("Invalid registration queueing location in service unit {0}", su.ServiceUnitName));
                }
                else
                {
                    return false;
                }
            }
            if (string.IsNullOrEmpty(su.QueueCode))
            {
                if (IsFromApiAntrianV2)
                {
                    throw new Exception(string.Format("Invalid queueing code in service unit {0}", su.QueueCode));
                }
                else
                {
                    return false;
                }
            }

            var par = new Paramedic();
            par.LoadByPrimaryKey(apt.ParamedicID);

            //this.AppointmentNo = apt.AppointmentNo;
            apQue.SRQueueingLocation = su.SrqueueinglocationReg;
            apQue.SRQueueingGroup = "01";
            apQue.SRQueueingType = SRQueueingType;

            apQue.FormattedNo = GetFormattedNo(SRQueueingType, su, par, apt.AppointmentQue.Value);

            apQue.QueueingDate = apt.AppointmentDate;
            apQue.SRKioskQueueStatus = "01";
            apQue.ServiceUnitID = su.ServiceUnitID;

            var cDate = (new DateTime()).NowAtSqlServer();

            apQue.CreateDateTime = cDate;
            apQue.CreateByUserID = UserID;
            apQue.LastUpdateDateTime = cDate;
            apQue.LastUpdateByUserID = UserID;
            apQue.ParamedicID = apt.ParamedicID;

            return true;
        }
    }

    public partial class AppointmentQueueingCollection {
        public bool LoadByAppointmentNo(string AppointmentNo) {
            this.Query.Where(this.Query.AppointmentNo == AppointmentNo);
            return this.LoadAll();
        }

        public DataTable GetLatestCalled(DateTime ApptDate) {
            string cmd = "sp_ApptQueueingGetLatestCalled";
            var pars = new esParameters();
            var pDate = new esParameter("p_Date", ApptDate, esParameterDirection.Input, DbType.DateTime, 0);
            pars.Add(pDate);
            return FillDataTable(esQueryType.StoredProcedure, cmd, pars);
        }

        public DataTable GetLastCalledRegistration(DateTime Date)
        {
            string cmd = "sp_ApptQueueingGetLastCalledRegistration";
            var pars = new esParameters();
            var pDate = new esParameter("p_Date", Date, esParameterDirection.Input, DbType.DateTime, 0);
            pars.Add(pDate);
            return FillDataTable(esQueryType.StoredProcedure, cmd, pars);
        }

        public DataTable GetLastCalledPolyclinic(DateTime ApptDate)
        {
            string cmd = "sp_ApptQueueingGetLastCalledPolyclinic";
            var pars = new esParameters();
            var pDate = new esParameter("p_Date", ApptDate, esParameterDirection.Input, DbType.DateTime, 0);
            pars.Add(pDate);
            return FillDataTable(esQueryType.StoredProcedure, cmd, pars);
        }

        public DataTable GetLastCalledBeeingServed(DateTime Date, string Prefix)
        {
            string cmd = "sp_ApptQueueingGetLastCalledBeeingServed";
            var pars = new esParameters();
            pars.Add("p_Date", Date, esParameterDirection.Input, DbType.DateTime, 0);
            pars.Add("p_Prefix", Prefix, esParameterDirection.Input, DbType.String, 6);
            return FillDataTable(esQueryType.StoredProcedure, cmd, pars);
        }

        public DataTable GetListQueuePrefix(DateTime Date, string Prefix)
        {
            string cmd = "sp_ApptQueueingGetListQueuePrefix";
            var pars = new esParameters();
            pars.Add("p_Date", Date, esParameterDirection.Input, DbType.DateTime, 0);
            pars.Add("p_Prefix", Prefix, esParameterDirection.Input, DbType.String, 6);
            return FillDataTable(esQueryType.StoredProcedure, cmd, pars);
        }
    }
}
