using System;
using System.Data;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;

using nexus.common;
using nexus.common.dal;
using nexus.common.core;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace nexus.common
{
    public class ZIP : IDisposable
    {


        public event OnStartEventHandler    OnStart;    public delegate void OnStartEventHandler    (string FileName);
        public event OnFinishedEventHandler OnFinished; public delegate void OnFinishedEventHandler ();
        public event OnErrorEventHandler    OnError;    public delegate void OnErrorEventHandler    (string FileId, string FtpError);

        public ZIP()
        {
        }


        public bool ProcessZip(string ZipFile, string LocalPath)
        {
            string FullFileName = "";
            bool Processed = true;
            if (string.IsNullOrEmpty(LocalPath))
            {
                LocalPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ZipFile), System.IO.Path.GetFileNameWithoutExtension(ZipFile));
            }

            var MyFileStream = default(FileStream);
            ICSharpCode.SharpZipLib.Zip.ZipInputStream MyZipInputStream;
            try
            {
                MyZipInputStream = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(new FileStream(ZipFile, FileMode.Open, FileAccess.Read));
                ICSharpCode.SharpZipLib.Zip.ZipEntry MyZipEntry = MyZipInputStream.GetNextEntry();
                while (MyZipEntry is object)
                {
                    string FileName = MyZipEntry.Name;
                    FullFileName = System.IO.Path.Combine(LocalPath, FileName);

                    OnStart?.Invoke(FullFileName);
                    try
                    {
                        if (!Directory.Exists(System.IO.Path.GetDirectoryName(FullFileName)))
                        {
                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FullFileName));
                        }

                        if (System.IO.File.Exists(FullFileName))
                            System.IO.File.Delete(FullFileName);
                        try
                        {
                            MyFileStream = new FileStream(FullFileName, FileMode.OpenOrCreate, FileAccess.Write);
                            int count;
                            var buffer = new byte[4097];
                            count = MyZipInputStream.Read(buffer, 0, 4096);
                            while (count > 0)
                            {
                                MyFileStream.Write(buffer, 0, count);
                                count = MyZipInputStream.Read(buffer, 0, 4096);
                            }

                            MyFileStream.Close();
                            
                            System.IO.File.SetCreationTime(FullFileName, MyZipEntry.DateTime);
                            System.IO.File.SetLastAccessTime(FullFileName, MyZipEntry.DateTime);
                            System.IO.File.SetLastWriteTime(FullFileName, MyZipEntry.DateTime);

                        }
                        catch (Exception ex)
                        {
                            // Props.Instance.InsertLogEntry(True, "Updater", "ProcZip", "Download", "ex1", ex.Message)
                        }

                        try
                        {
                            MyZipEntry = MyZipInputStream.GetNextEntry();
                        }
                        catch (Exception ex)
                        {
                            // Props.Instance.InsertLogEntry(True, "Updater", "ProcZip", "Download", "ex2", ex.Message)
                            MyZipEntry = default;
                            Processed = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Props.Instance.InsertLogEntry(True, "Updater", "ProcZip", "Download", "ex3", ex.Message)
                        Processed = false;
                        break;
                    }
                }

                if (MyZipInputStream is object)
                    MyZipInputStream.Close();
                if (MyFileStream is object)
                    MyFileStream.Close();
            }
            catch (Exception ex)
            {
                // Props.Instance.InsertLogEntry(True, "SockHub", "ProcZip", "Download", "ex4", ex.Message)
                Processed = false;
            }

            return Processed;
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