using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

using nexus.common.dal;

namespace nexus.web.auth.Data
{
    public class ValidateLogin : IDisposable
    {

        public bool Validate(string username, string password)
        {
            string savepass = string.Empty;

            if (string.IsNullOrEmpty(password)) return false;

            //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            //IConfiguration Configuration = builder.Build();
            //using (SQLServer db = new SQLServer(Configuration.GetConnectionString("DefaultConnection").ToString()))

            setting.GetSettings();
            string conStr = setting.ConnectionString;//   Configuration.GetConnectionString("DefaultConnection").ToString();
            using (SQLServer db = new SQLServer(conStr))
            {
                //using (StreamWriter w = File.AppendText("C:\\Nexus\\logging\\nexus_api.txt")) w.WriteLine(DateTime.Now.ToLongTimeString() + "|| Validate:" + username + ":" + password);

                using (DataTable dt = db.GetDataTable("loc.lst_credential", "I~S~Username~" + username))
                {
                    if (dt.Rows.Count > 0) savepass = dt.Rows[0]["password"].ToString();

                    //using (StreamWriter w = File.AppendText("C:\\Nexus\\logging\\nexus_api.txt")) w.WriteLine(DateTime.Now.ToLongTimeString() + "|| Rows:" + (dt.Rows.Count > 0).ToString() + ":" + (password.Equals(savepass)));
                }
            }

            return (password.Equals(savepass));
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
