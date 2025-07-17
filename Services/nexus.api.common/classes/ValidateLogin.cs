using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

using nexus.common;
using nexus.common.dal;
using nexus.common.cache;
using nexus.shared.common;

namespace nexus.api.common
{
    public class ValidateLogin : IDisposable
    {

        public bool Validate(string username, string password)
        {
            string savepass = string.Empty;

            if (string.IsNullOrEmpty(password)) return false;

            using (SQLServer db = new SQLServer(settings.ConnectionString))
            {
                using (DataTable dt = db.GetDataTable("loc.lst_credential", "I~S~Username~" + username))
                {
                    if (dt.Rows.Count > 0) savepass = dt.Rows[0]["password"].ToString();
                }
            }

            return (password.Equals(savepass));
        }

        public LoginResponse Login(string username, string password)
        {
            string savepass = string.Empty;

            LoginResponse login = new LoginResponse()
            {
                Status   = "Bad",
                Response = "No Password"
            };

            if (string.IsNullOrEmpty(password)) return login;

            using (SQLServer db = new SQLServer(settings.ConnectionString))
            {
                using (DataTable dt = db.GetDataTable("loc.lst_Human_Worker", "I~S~Username~" + username))
                {
                    if (dt.Rows.Count.Equals(0))                                  login.Response = "Username not found";
                    else if (!password.Equals(dt.Rows[0]["password"].ToString())) login.Response = "Wrong password";
                    else
                    {
                        login.WorkerType  = dt.Rows[0]["WorkerType"].ToString();
                        login.WorkerState = dt.Rows[0]["WorkerState"].ToString();
                        login.Department  = dt.Rows[0]["Department"].ToString();
                        login.Gender      = dt.Rows[0]["Gender"].ToString();
                        login.Title       = dt.Rows[0]["Title"].ToString();
                        login.FirstName   = dt.Rows[0]["FirstName"].ToString();
                        login.LastName    = dt.Rows[0]["LastName"].ToString();
                        login.BirthDate   = dt.Rows[0]["BirthDate"].ToString();
                        login.LastUse     = dt.Rows[0]["LastIdentified"].ToString();

                        login.WorkerId      = helpers.ToInt(dt.Rows[0]["WorkerPk"].ToString());
                        login.WorkerTypeId  = helpers.ToInt(dt.Rows[0]["WorkerTypeId"].ToString());
                        login.WorkerStateId = helpers.ToInt(dt.Rows[0]["WorkerStateId"].ToString());
                        login.GenderId      = helpers.ToInt(dt.Rows[0]["GenderId"].ToString());
                        login.TitleId       = helpers.ToInt(dt.Rows[0]["TitleId"].ToString());

                        DataTable MT = db.GetDataTable("loc.ValidateUserMenu", "I~I~WorkerId~" + login.WorkerId);
                        if (MT.Rows.Count>0)
                        {
                            login.Status   = "OK";
                            login.Response = "";

                            Menu[] Menus = new Menu[MT.Rows.Count];
                            for (int i = 0; i < MT.Rows.Count; i++)
                            {
                                Menu mn = new Menu()
                                {
                                    MenuId   = helpers.ToInt(MT.Rows[i]["MenuId"].ToString()),
                                    ItemId   = helpers.ToInt(MT.Rows[i]["ItemId"].ToString()),
                                    ModuleId = helpers.ToInt(MT.Rows[i]["ModuleId"].ToString()),

                                    MenuDescription = MT.Rows[i]["MenuDescription"].ToString(),
                                    ItemDescription = MT.Rows[i]["ItemDescription"].ToString(),
                                    EndPoint        = MT.Rows[i]["Assembly"].ToString()
                                };
                                Menus[i] = mn;
                            }
                            login.Menus = Menus;
                        }
                        else
                        {
                            login.Response = "No Menu Found";
                        }
                    }
                }
                helpers.WriteToLog("Validate: " + username + ":" + password + " " + ((login.Status == "OK") ? "Validated OK" : "Validate Failed") + "  " + login.Response)   ;
            }

            return login;
        }


        #region IDisposable Implementation
        // To detect redundant calls
        private bool disposedValue = false;
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: free managed resources when explicitly called
                }

                // TODO: free shared unmanaged resources
            }
            this.disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }



}
