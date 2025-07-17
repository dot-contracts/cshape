using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace nexus.common.cache
{
    public sealed class LoggerCache
    {
        private object    syncLock     = new object();

        private string    mLogFile     = "c:\\Nexus\\programs\\log.txt";
        private int       mLogging    = 0;
        private string    mLastMessage = "";
        private DataTable mLogData;


        public event         NewDataEventHandler NewData;
        public delegate void NewDataEventHandler();

        public int       Logging { get { return mLogging; } set { mLogging = value; } }
        public DataTable LogData  { get { return mLogData; }  set { mLogData = value; } }


        private static LoggerCache mInstance = new LoggerCache();
        private LoggerCache() { }
        public static LoggerCache Instance { get { return mInstance; } }
        public bool Create()
        {
            //mLogFile = LogFile;

            mLogData = new DataTable();
            mLogData.Columns.Add("Id",       System.Type.GetType("System.String"));
            mLogData.Columns.Add("LogDate",  System.Type.GetType("System.String"));
            mLogData.Columns.Add("Terminal", System.Type.GetType("System.String"));
            mLogData.Columns.Add("Operator", System.Type.GetType("System.String"));
            mLogData.Columns.Add("Module",   System.Type.GetType("System.String"));
            mLogData.Columns.Add("Command",  System.Type.GetType("System.String"));
            mLogData.Columns.Add("MsgType",  System.Type.GetType("System.String"));
            mLogData.Columns.Add("Message",  System.Type.GetType("System.String"));
            mLogData.AcceptChanges();

            return true;
        }

        public string InsertLogEntryNoRepeats(bool StoreIt, string ModuleName, string Command, string MessageType, string Message)
        {
            string RetMess = "";
            if (mLastMessage != Message)
            {
                mLastMessage = Message;
                RetMess = InsertLogEntry(StoreIt, ModuleName, Command, MessageType, Message);
            }
            return RetMess;
        }

        public string InsertLogEntry(bool StoreIt, string ModuleName, string Command, string MessageType, string Message)
        {
            lock (this.syncLock)
            {
                string Ret = "";

                try
                {
                    System.DateTime Start = DateTime.Now;

                    if (mLogData == null) Create();
                    while (mLogData.Rows.Count - 1 > 512)
                    {
                        mLogData.Rows.RemoveAt(mLogData.Rows.Count - 1);
                        if (DateTime.Now.Subtract(Start).TotalSeconds > 1) break;
                    }

                    DataRow newRow = mLogData.NewRow();
                    newRow["LogDate"]   = DateTime.Now;
                    newRow["Terminal"]  = "";
                    newRow["Operator"]  = "";
                    newRow["Module"]    = ModuleName;
                    newRow["Command"]   = Command;
                    newRow["MsgType"]   = MessageType;
                    newRow["Message"]   = Message;
                    mLogData.Rows.InsertAt(newRow, 0);

                    if (MessageType.Length > 8) MessageType = MessageType.Substring(0, 8);

                    NewData?.Invoke();
                }
                catch (Exception ex)
                {
                }

                return Ret;

            }
        }

        public void LogEntry(string Entry)
        {
            try
            {
                StreamWriter FI = default(StreamWriter);
                bool Opened = false;
                try
                {
                    if (File.Exists(mLogFile))
                    {
                        FileInfo FP = new FileInfo(mLogFile);
                        if (FP.Length > 32000) File.Delete(mLogFile);
                    }
                    FI = new StreamWriter(mLogFile, true);
                    Opened = true;
                    FI.WriteLine(DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff"));
                    FI.WriteLine(Entry);
                }
                catch (Exception ex)
                {
                }
                if (Opened) FI.Close();
            }
            catch (Exception ex)
            {
            }
        }

        public void Clear()
        {
            mLogData.Rows.Clear();
        }

    }
}
