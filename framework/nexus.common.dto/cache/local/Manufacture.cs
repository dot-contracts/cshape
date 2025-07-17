using System;
using System.Data;

using nexus.common.dal;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{

    public sealed class ManufactureCache : NxCacheBase
    {

        private static ManufactureCache mInstance = new ManufactureCache();
        private ManufactureCache() { }
        public static ManufactureCache Instance { get { return mInstance; } }


        public bool Create()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                CreateCache(DB.GetDataTable("loc.lst_Company", "I~S~CompanyType~" + EnumCache.Instance.getTypeId("Company", "Manufacture")));
            return (CacheLoaded);
        }

        public string Display(string ManufacturePk) { return LookUpValue("CompanyPk", ManufacturePk, "Description"); }
        public string Value(string Display) { return LookUpValue("Description", Display, "CompanyPk"); }

    }

}
