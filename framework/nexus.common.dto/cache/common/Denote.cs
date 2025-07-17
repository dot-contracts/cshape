using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using nexus.common.dal;
using nexus.common.core;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{
    public sealed class DenoteCache : NxCacheBase 
    {
        private dlDenote mDenote; 

        private static DenoteCache mInstance = new DenoteCache();
        private DenoteCache() { Create(); }
        public static DenoteCache Instance { get { return mInstance; } }

        /// <summary>
        /// Populates the mDenoteTable with a list of Denotes from the Denote table in the database.
        /// </summary>
        public bool Create()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            CreateCache(DB.GetDataTable("cmn.lst_Denote", ""));
            DB.Dispose();
            DB = null;
            return (CacheLoaded);
        }

        public bool Open(string targetId)  { return mDenote.Open(targetId); }

        public string   Display   (string DenotePk)                                       { return                LookUpValue("DenotePk",    DenotePk, "Description"); }
        public string   Value     (string Display)                                        { return                LookUpValue("Description", Display,  "DenotePk"); }


    }
}
