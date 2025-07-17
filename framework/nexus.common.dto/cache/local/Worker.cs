using System;
using System.Data;

using nexus.common.dal;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{

    public sealed class WorkerCache : NxCacheBase 
    {
        private string mWorkerId;

        public bool HasLoggedIn { get; set; } = false;

        private static WorkerCache mInstance = new WorkerCache();

        public event OnChangedLoginEventHandler  OnChangedLogin;  public delegate void OnChangedLoginEventHandler();
        public event OnChangedLogoutEventHandler OnChangedLogout; public delegate void OnChangedLogoutEventHandler();

        private WorkerCache() { HasLoggedIn = false; }
        public static WorkerCache Instance { get { return mInstance; } }

        public bool Create()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                CreateCache(DB.GetDataTable("loc.lst_Person_Worker", ""));

            return (CacheLoaded);
        }

        public string  Display       (string WorkerPk)   { return  LookUpValue("WorkerPk",    WorkerPk,  "Description"); }
        public string  Value         (string Display)    { return  LookUpValue("Description", Display,   "WorkerPk"); }
        public string  WorkerId      ()                  { return  mWorkerId; }

        public bool Validate(string username, string password)
        {
            HasLoggedIn = false;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                using (DataTable DT = DB.GetDataTable("loc.ValidateUser", "I~S~username~" + username + ";I~S~password~" + password))
                {
                    if (DT.Rows.Count > 0)
                    {
                        if (DT.Rows[0]["RoleType"].ToString().Equals("Allowed"))
                        {
                            HasLoggedIn = true;
                            mWorkerId = DT.Rows[0]["WorkerId"].ToString();
                            LoggerCache.Instance.InsertLogEntry(true, "piLogin", "", "Message", "1 Login");
                            WorkerCache.Instance.Login(mWorkerId);
                            OnChangedLogin?.Invoke();

                            return true;
                        }
                    }
                }
            }

            return false;
        }


        public bool Login(string userId)
        {
            mWorkerId = userId;

            HasLoggedIn = true;

            LoggerCache.Instance.InsertLogEntry(true, "piLogin", "", "Message", userId +  " Login");

            OnChangedLogin?.Invoke();

            return true;
        }

        public bool Logout()
        {
            HasLoggedIn = false;

            OnChangedLogout?.Invoke();

            return true;
        }
    }
}
