using System;
using System.Data;

using nexus.common.dal;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{

    public sealed class ComputerCache : NxCacheBase 
    {

        private static ComputerCache mInstance = new ComputerCache();
        private ComputerCache() { }
        public static ComputerCache Instance { get { return mInstance; } }


        public bool Create()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                CreateCache(DB.GetDataTable("loc.lst_Device_Computer", ""));
            return (CacheLoaded);
        }


        public string Search(string MAC)
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.lst_Device_Computer", "I~S~MacAddr~" + MAC);
                if (DT.Rows.Count > 0) return (DT.Rows[0]["ComputerPk"].ToString());
            }
            return string.Empty;
        }
        public int CreateComputer(string mac)
        {
            int DevId = 0;
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                dlComputer   dev = new dlComputer();

                string ComputerPk = Search(mac);
               
                if (!dev.Open(ComputerPk))
                {
                    dev.MACAddr = mac;
                    dev.Update();
                }

                return dev.intComputerPk();
            }

            return DevId;
        }

        public bool ValidateComputer(string ComputerId, string Username, string Password)
        {
            bool Validated = true;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                dlComputer dev = new dlComputer();


                //return dev.intComputerPk();
            }

            return Validated;
        }


        public string   Display   (string ComputerPk)                                         { return                LookUpValue("ComputerPk",  ComputerPk, "Description"); }
        public string   Value     (string Display)                                            { return                LookUpValue("Description", Display,    "ComputerPk"); }

    }
}
