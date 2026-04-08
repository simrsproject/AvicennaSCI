using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temiang.Avicenna.BusinessObject
{
    public partial class ServiceRoomBridging
    {
        public string BridgingTypeName
        {
            get { return GetColumn("refToAppStandardReferenceItem_ItemName").ToString(); }
            set { SetColumn("refToAppStandardReferenceItem_ItemName", value); }
        }
    }

    public partial class ServiceRoomBridgingCollection {
        public bool LoadByBridgingID(string BridgingID) {
            //var subColl = new ServiceRoomBridgingCollection();
            this.QueryReset();
            this.Query.Where(this.Query.SRBridgingType == "BridgingType-001", this.Query.BridgingID == BridgingID);
            return this.LoadAll();
        }
        public bool LoadByRoomID(string RoomID)
        {
            //var subColl = new ServiceRoomBridgingCollection();
            this.QueryReset();
            this.Query.Where(this.Query.SRBridgingType == "BridgingType-001", this.Query.RoomID == RoomID);
            return this.LoadAll();
        }
    }
}
