using System;
using System.Data;

using nexus.common.dal;
using nexus.common.core;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{

    public sealed class EgmGameCache : NxCacheBase 
    {

        private static EgmGameCache mInstance = new EgmGameCache();
        private EgmGameCache() { }
        public static EgmGameCache Instance { get { return mInstance; } }


        public bool Create()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            CreateCache(DB.GetDataTable("loc.lst_Device_Egm_Game", ""));
            DB.Dispose();
            DB = null;
            return (CacheLoaded);
        }

        public string CheckName(string ManufactureId, string GameName, string SpecNo)
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            string GameId = DB.ExecLookup("Select GamePk from loc.Device_Egm_Game where ManufactureId=" + ManufactureId + " and Gamename='" + GameName + "' and SpecNo='" + SpecNo + "'");
            if (String.IsNullOrEmpty(GameId))
            {
                GameId = DB.ExecLookup("Insert Into loc.Device_Egm_Game ( ManufactureId,Gamename,SpecNo,GameGK) values(" + ManufactureId + ",'" + GameName + "','" + SpecNo + "',newid()) select top 1 GamePk from loc.Device_Egm_Game order by GamePk desc");
                Create(); 
            }
            DB.Dispose();
            DB = null;
            return GameId;
        }

        public string   Display   (string GamePk)   { return LookUpValue("GamePk",   GamePk,  "GameName"); }
        public string   Value     (string Display)  { return LookUpValue("GameName", Display, "GamePk"); }

        public DataTable getManList(string ManufactureId) { return LookUpTable("ManufactureId", ManufactureId); }

        public DataTable Denoms()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("Select Display, DenomPK from [cmn].[DenomList]()");
            DB.Dispose();
            DB = null;
            return (DT);
        }

    }

}

