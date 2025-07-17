using System;
using System.Data;

using nexus.common.dal;
using nexus.common.core;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{

    public sealed class ClassCache : NxCacheBase 
    {
        private static ClassCache mInstance = new ClassCache();
        private ClassCache() { }
        public static ClassCache Instance { get { return mInstance; } }

        public bool Create()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            CreateCache(DB.GetDataTable("loc.lst_Class", ""));
            DB.Dispose();
            DB = null;
            return (CacheLoaded);
        }

        public string   Display   (string ClassPk)     { return                LookUpValue("ClassPk",     ClassPk,  "Description"); }
        public string   Value     (string Display)     { return                LookUpValue("Description", Display,  "ClassPk"); }

    }
}
