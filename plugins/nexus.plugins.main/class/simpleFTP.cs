using System;
using System.IO;
using System.Net;
using System.Data;
using System.Xml.Schema;
using System.IO.Pipes;

using nexus.common.dal;

namespace nexus.common.dto
{
    public class FTP : IDisposable
    {
        private bool   mProcess    = true;
        private string mServer     = @"ftp:\\server.nextnetgaming.com.au";
        private string mPort       = "21";
        private string mUsername   = "content";
        private string mPassword   = "content";

        private string mFileId     = "";
        private string mFileName   = "";
        private string mLocalPath  = "";
        private string mRemotePath = "C:\\NextNet\\ftp\\NextNet\\Updates";
        private int    mRemoteSize = 0;

        public bool   Process  { get { return mProcess; } set { mProcess = value; }}
        public string FTPError { get { return mError;   } }


        // Private WithEvents mWorker As BackgroundWorker
        private FtpWebResponse mResponse = null;
        private FileStream     mOutputStream = null;
        private Stream         mFTPStream = null;
        private int            mBytesRead = 0;
        private string         mError = "";
        private bool mLastDownloadTest = false;
        private DateTime mLastDownloadTesttime = DateTime.Now.AddDays(-1);

        public event OnStartEventHandler    OnStart;    public delegate void OnStartEventHandler    (string FileId, int FileSize);
        public event OnProgressEventHandler OnProgress; public delegate void OnProgressEventHandler (string FileId, int ProgressPercentage);
        public event OnFinishedEventHandler OnFinished; public delegate void OnFinishedEventHandler (string FileId);
        public event OnErrorEventHandler    OnError;    public delegate void OnErrorEventHandler    (string FileId, string FtpError);

        public FTP(string server, string username = "content" , string password = "content") 
        {
            mServer   = server;
            mUsername = username;
            mPassword = password;
        }

        public bool CanDownload
        {
            get
            {
                if (DateTime.Now.Subtract(mLastDownloadTesttime).TotalMinutes > 15 | !mLastDownloadTest)
                {
                    mLastDownloadTesttime = DateTime.Now;

                    string localPath = @"C:\nexus\downloads\test";
                    if (!Directory.Exists(localPath))
                        Directory.CreateDirectory(localPath);
                    string localFile = Path.Combine(localPath, "testdl.txt");
                    try
                    {
                        if (File.Exists(localFile)) File.Delete(localFile);
                    }
                    catch (Exception ex) { }

                    mLastDownloadTest = getFile("testDL.txt", "", localPath);

                    try { if (File.Exists(localFile)) File.Delete(localFile); }
                    catch (Exception ex) {}
                    return mLastDownloadTest;
                }
                return mLastDownloadTest;
            }
        }

        private FtpWebRequest CreateFtpWebRequest(string ftpServer, string userName, string password, bool keepAlive = false)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServer);//    new Uri(ftpServer));

            //Set proxy to null. Under current configuration if this option is not set then the proxy that is used will get an html response from the web content gateway (firewall monitoring system)
            request.Proxy = null;

            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = keepAlive;

            request.Credentials = new NetworkCredential(userName, password);

            return request;
        }

        public bool getFile(string FileName, string RemotePath, string LocalPath)
        {

            bool ret = false;


            int bytesRead = 0;
            byte[] buffer = new byte[2048];

            try
            {
                string webReq = mServer + ((String.IsNullOrEmpty(RemotePath)) ? "" : "/") + RemotePath + "/" + FileName;
                FtpWebRequest request = CreateFtpWebRequest(webReq, mUsername, mPassword, true);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                Stream reader = request.GetResponse().GetResponseStream();

                string LocalFile = Path.Combine(LocalPath, FileName);
                FileStream fileStream = new FileStream(LocalFile, FileMode.Create);

                while (true)
                {
                    ret = true;
                    bytesRead = reader.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    fileStream.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
            }
            catch (Exception ex)
            {
                mError = ex.Message; 
            }

            return ret;
        }

        public DataTable getDirTable(string directory)
        {
            DataTable dirList = CreateDirTable();

            WebRequest request = WebRequest.Create(directory);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(mUsername, mPassword);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();

                    string[] lines = html.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        // Parse each line to get file details
                        string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        long fileSize = long.Parse(parts[4]);
                        if (fileSize>0)
                        {
                            string fileName = parts[parts.Length - 1];
                            DateTime fileDate = DateTime.Now;
                            try { fileDate = DateTime.ParseExact($"{parts[5]} {parts[6]}", "MM-dd-yy hh:mmtt", null); }
                            catch (Exception) { }

                            DataRow dr = dirList.NewRow();
                            dr["FileName"] = fileName;
                            dr["FileSize"] = fileSize;
                            dr["FileDate"] = fileDate;
                            dirList.Rows.Add(dr);
                        }
                    }
                }
            }

            return dirList;
        }

        public DataTable CreateDirTable()
        {
            DataTable DirList = new DataTable();
            DirList.Columns.Add("Selected",    System.Type.GetType("System.Boolean"));
            DirList.Columns.Add("Package",     System.Type.GetType("System.String"));
            DirList.Columns.Add("FileName",    System.Type.GetType("System.String"));
            DirList.Columns.Add("LocalPath",   System.Type.GetType("System.String"));
            DirList.Columns.Add("RemotePath",  System.Type.GetType("System.String"));
            DirList.Columns.Add("ExistLocal",  System.Type.GetType("System.Boolean"));
            DirList.Columns.Add("ExistRemote", System.Type.GetType("System.Boolean"));
            DirList.Columns.Add("FileSize",    System.Type.GetType("System.String"));
            DirList.Columns.Add("FileDate",    System.Type.GetType("System.DateTime"));
            DirList.Columns.Add("Changed",     System.Type.GetType("System.Boolean"));
            DirList.AcceptChanges();
            return DirList;
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

