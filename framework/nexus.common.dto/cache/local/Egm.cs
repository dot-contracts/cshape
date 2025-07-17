using System;
using System.Data;

using nexus.common.dal;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{

    public sealed class EgmCache : NxCacheBase 
    {
        private string        mAvailableState = "";

        private static EgmCache mInstance = new EgmCache();  
        private EgmCache()    { }
        public static EgmCache Instance { get { return mInstance; } }


        public bool Create()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                CreateCache(DB.GetDataTable("loc.lst_Device_Egm", ""));

            mAvailableState = cache.EnumCache.Instance.getStateId("Device", "Available");

            return (CacheLoaded);
        }

        public string  Display       (string EgmPk)        { return  LookUpValue("EgmPk",       EgmPk,    "Description"); }
        public string  Value         (string Display)      { return  LookUpValue("Description", Display,  "EgmPk"); }
        public string  getIdFromGMID (string GMID)         { return  LookUpValue("GMID",        GMID,     "EgmPk"); }
        public string  getIdFromMAC  (string MAC)          { return  LookUpValue("MACAddr",     MAC,      "EgmPk"); }

        public int CreateEgm(int GMID, string MAC)
        {
            // should be done in the dto not here

            dlEgm Egm = new dlEgm(setting.ConnectionString);

            Egm.Device.Entity.VenueID = setting.VenueId;
            Egm.GMID                  = GMID;
            Egm.MACAddr               = MAC;
            Egm.EgmStateId            = mAvailableState;
            Egm.Description           = "New " + GMID.ToString();
            Egm.Update();

            dlEndpoint EP = new dlEndpoint(setting.ConnectionString);
            EP.EntityID = Egm.EgmPK;
            EP.MACAddr  = MAC;
            EP.Update();

            Create();

            int newPk = 0;
            int.TryParse(Egm.EgmPK, out newPk);
            return newPk;
        }

        public void UpdateEgm(int EgmId, int GMID)
        {
            dlEgm Egm = new dlEgm(setting.ConnectionString);
            Egm.Open(EgmId);
            Egm.GMID = GMID;
            Egm.Update();
        }

        public void UpdateEGMRow(int EgmPk)
        {

        }

        public int FindEGM(string MacAddr)
        {

            int egmId = 0;
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                using (DataTable DT = DB.GetDataTable("loc.lst_Device_Egm", "I~S~MacAddr~" + MacAddr))
                    if (DT.Rows.Count > 0)
                    {
                        int.TryParse(DT.Rows[0]["EgmPk"].ToString(), out egmId);

                    }
            }
            return egmId;
        }

        public int FindGMID(string MacAddr)
        {

            int GMID = 0;
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                using (DataTable DT = DB.GetDataTable("loc.lst_Device_Egm", "I~S~MacAddr~" + MacAddr))
                    if (DT.Rows.Count > 0) int.TryParse(DT.Rows[0]["GMID"].ToString(), out GMID);
            }
            return GMID;
        }

        public DataTable LookupMAC(string MacAddr)
        {
            string Dates = "'" + DateTime.Now.ToString("dd MMM, yyyy") + " 4:00 AM','" + DateTime.Now.AddDays(1).ToString("dd MMM, yyyy") + " 4:00 AM'";
            DataTable DT = new DataTable();
            using (SQLServer DB = new SQLServer(setting.ConnectionString)) DT = DB.GetDataTable("select EgmId, Description, HouseNo, MACAddr, GMID, LocationState as LocationStateId from gam.getEgmList (" + Dates + ") where MacAddr='" + MacAddr + "'");
            return DT;
        }


    }
}
