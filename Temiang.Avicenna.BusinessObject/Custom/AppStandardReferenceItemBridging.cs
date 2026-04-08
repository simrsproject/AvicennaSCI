/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 29/01/2024 11:57:05
===============================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using Temiang.Dal.Core;
using Temiang.Dal.Interfaces;
using Temiang.Dal.DynamicQuery;

namespace Temiang.Avicenna.BusinessObject
{
    public partial class AppStandardReferenceItemBridging
    {
        public enum SatusehatRef
        {
            CvxName,
            DosageUnit,
            Route,
            ImmReason,
            ImmTiming
        }
        public static AppStandardReferenceItemBridging Load(SatusehatRef stusehatRef, string itemID, string bridgingType)
        {
            var br = new AppStandardReferenceItemBridging();
            if (br.LoadByPrimaryKey(stusehatRef.ToString(), itemID, bridgingType)) return br;
            return null;
        }

        public static string GetBridgingID(SatusehatRef stusehatRef, string itemID, string bridgingType)
        {
            return GetBridgingID(stusehatRef.ToString(), itemID, bridgingType);
        }
        public static string GetBridgingID(string standardReferenceID, string itemID, string bridgingType)
        {
            var br = new AppStandardReferenceItemBridging();
            if (br.LoadByPrimaryKey(standardReferenceID, itemID, bridgingType)) return br.BridgingID;
            return string.Empty;
        }
    }


}
