using System;
using System.Data;

using nexus.common.dal;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{

    public sealed class VenueCache : NxCacheBase 
    {
        private static VenueCache mInstance = new VenueCache();
        private VenueCache() { }
        public static VenueCache Instance { get { return mInstance; } }

        public bool Create()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            CreateCache(DB.GetDataTable("loc.lst_Company_Venue", ""));
            DB.Dispose();
            DB = null;

            return (CacheLoaded);
        }

        public string  Display       (string VenuePk)   { return  LookUpValue("VenuePk",     VenuePk,  "Description"); }
        public string  Value         (string Display)   { return  LookUpValue("Description", Display,  "VenuePk"); }

    }
}
