using nexus.common.core;
using nexus.common.dal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

using nexus.common.dto;

namespace nexus.common.dto
{
    public class Updater : IDisposable
    {
        public bool IsAppRunning;

        private string mLogFile = @"c:\Nexus\Programs\Updater.log";

        public event OnMessageEventHandler          OnMessage;          public delegate void OnMessageEventHandler          (string Message);

        public event OnStartUpdateEventHandler      OnStartUpdate;      public delegate void OnStartUpdateEventHandler      (int TotalFiles);
        public event OnProgressUpdateEventHandler   OnProgressUpdate;   public delegate void OnProgressUpdateEventHandler   (int file);

        public event OnStartDownloadEventHandler    OnStartDownload;    public delegate void OnStartDownloadEventHandler    ();
        public event OnProgressDownloadEventHandler OnProgressDownload; public delegate void OnProgressDownloadEventHandler (int Percent);

        public Updater() { }    

        public bool Process(string Package, bool FullInstall = false, bool Replace = true)
        {
            bool Proc = false;

            if (!Directory.Exists(System.IO.Path.GetDirectoryName(mLogFile))) Directory.CreateDirectory(System.IO.Path.GetDirectoryName(mLogFile));
            LogEntry("Log File:" + mLogFile);

            //LogEntry("Update Server:" + setting.ftpServer);

            //FTP sFTP = new FTP(setting.ftpServer, setting.ftpUser,  setting.ftpPW);
            //if (sFTP.CanDownload)
            //{
            //    string localDL = "c:\\Nexus\\downloads\\" + Package; if (!System.IO.Directory.Exists(localDL)) System.IO.Directory.CreateDirectory(localDL);
            //    string localXL = "c:\\Nexus\\Programs\\"  + Package; if (!System.IO.Directory.Exists(localXL)) System.IO.Directory.CreateDirectory(localXL);

            //    string ExecFile = "c:\\Nexus\\Programs\\" + Package + "\\" + Package + ".exe";
            //    string srcDir =  (FullInstall)? "Full" : (System.IO.File.Exists(ExecFile) ) ? "Updates" : "Full";

            //    using (Processes proc = new Processes())
            //    {
            //        IsAppRunning = proc.AppState(ExecFile).Equals(Processes.ProcStates.Running);
            //        proc.KillApp(System.IO.Path.Combine(localXL, Package + ".exe"), true);
            //    }
            //    LogEntry("Package:" + Package + " " + IsAppRunning.ToString());

            //    sFTP.OnProgress += SFTP_OnProgress;
            //    sFTP.OnStart    += SFTP_OnStart;

            //    ZIP sZIP = new ZIP();
            //    sZIP.OnStart += SZIP_OnStart;


            //    DataTable xDir = sFTP.getDirTable(setting.ftpServer + "/NextNet/" + srcDir + "/" + Package);
            //    LogEntry("NextNet/" + srcDir + "/" + Package + " Files:" + xDir.Rows.Count.ToString());

            //    OnStartUpdate?.Invoke(xDir.Rows.Count);

            //    for (int i = 0; i <= xDir.Rows.Count - 1; i++)
            //    {
            //        OnProgressUpdate?.Invoke(i);

            //        string fileName = xDir.Rows[i]["FileName"].ToString();
            //        string dlFile = System.IO.Path.Combine("c:\\Nexus\\downloads\\" + Package, fileName);
            //        DateTime fileDate = Convert.ToDateTime(xDir.Rows[i]["FileDate"]);

            //        LogEntry(fileName + "  " + fileDate.ToString());
            //        if (Replace && System.IO.File.Exists(System.IO.Path.Combine(localDL, fileName))) System.IO.File.Delete(System.IO.Path.Combine(localDL,fileName));

            //        sFTP.getFile ( fileName, "NextNet/" + srcDir + "/" + Package, localDL);

            //        if (System.IO.File.Exists(dlFile))
            //        {
            //            Proc = true;

            //            string localFile = System.IO.Path.Combine(localXL, fileName);

            //            if (localFile.ToUpper().Contains(".ZIP"))
            //            {
            //                sZIP.ProcessZip(dlFile, localXL);
            //            }
            //            else
            //            {
            //                if (System.IO.File.Exists(localFile)) System.IO.File.Delete(localFile);
            //                System.IO.File.Copy(dlFile, localFile);

            //                System.IO.File.SetCreationTime   (localFile, fileDate);
            //                System.IO.File.SetLastAccessTime (localFile, fileDate);
            //                System.IO.File.SetLastWriteTime  (localFile, fileDate);
            //            }
            //        }
            //    }

            //    if (IsAppRunning)
            //        using (Processes proc = new Processes()) proc.StartApp(ExecFile);
            //}
            //else
            //    LogEntry("Can't download " + sFTP.FTPError);

            return Proc;

        }

        private void SZIP_OnStart(string FileName)
        {
            LogEntry("UnZipping " + FileName);
        }

        private void SFTP_OnStart(string FileId, int FileSize)                  { OnStartDownload?.Invoke(); }
        private void SFTP_OnProgress(string FileId, int ProgressPercentage)     { OnProgressDownload?.Invoke(ProgressPercentage); }

        public void LogEntry(string Entry)
        {
            OnMessage?.Invoke(Entry);

            try
            {
                var FI = default(StreamWriter);
                bool Opened = false;
                try
                {
                    if (System.IO.File.Exists(mLogFile))
                    {
                        var FP = new FileInfo(mLogFile);
                        if (FP.Length > 32000L)
                            System.IO.File.Delete(mLogFile);
                    }

                    FI = new StreamWriter(mLogFile, true);
                    Opened = true;
                    FI.WriteLine(DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss") + "  " + Entry);
                }
                catch (Exception ex)
                {
                }

                if (Opened)
                    FI.Close();
            }
            catch (Exception ex)
            {
            }
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
