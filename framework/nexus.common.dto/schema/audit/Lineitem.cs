using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlLineitem : IDisposable
    {

        public enum LineitemTypes { Cancel, Cashless, Cheque, Docket, EgmMeter, EgmNote }
        
        private string        mLineitemGroup = "Lineitem";

        private bool          mNewEntry;          public bool   NewEntry         { get { return mNewEntry; }             set { mNewEntry = value; } }
        private string        mDataError;         public string DataError        { get { return mDataError; }            set { mDataError = value; } }

        private nxProperty    mLineitemType;     
        public string LineitemType
        {
            get { return mLineitemType.Value; }
            set 
            { 
                mLineitemType.Value   = value;  
                mLineitemTypeID      = EnumCache.Instance.getTypeFromDesc  (mLineitemGroup, mLineitemType.Value);
            }
        }
        private string        mLineitemTypeID;    public string LineitemTypeID   { get { return mLineitemTypeID; }       set { mLineitemTypeID       = value;  mLineitemType.Value  = EnumCache.Instance.getDescFromId    (                mLineitemTypeID); } }

        private nxProperty    mLineitemState;     public string LineitemState    { get { return mLineitemState.Value; }  set { mLineitemState.Value  = value;  mLineitemStateID     = EnumCache.Instance.getStateFromDesc (mLineitemGroup, mLineitemState.Value ); } }
        private string        mLineitemStateID;   public string LineitemStateID  { get { return mLineitemStateID; }      set { mLineitemStateID      = value;  mLineitemState.Value = EnumCache.Instance.getDescFromId    (                mLineitemStateID); } }

        private nxProperty    mLineitemDate;      public string LineitemDate     { get { return mLineitemDate.Value; }   set { mLineitemDate.Value = value; } }
        private nxProperty    mLineitemNo;        public string LineitemNo       { get { return mLineitemNo.Value; }     set { mLineitemNo.Value = value; } }
        private nxProperty    mReference;         public string Reference        { get { return mReference.Value; }      set { mReference.Value = value; } }
        private nxProperty    mAmount;            public string Amount           { get { return mAmount.Value; }         set { mAmount.Value = value; } }
        private nxProperty    mDetail;            public string Detail           { get { return mDetail.Value; }         set { mDetail.Value = value; } }


        private nxProperty    mHuman;             public string Human            { get { return mHuman.Value; }          set { mHuman.Value = value; } }
        private string        mHumanID;           public string HumanID          { get { return mHumanID; }              set { mHumanID      = value; } } //mHuman     =  shell.Instance..Description(mHumanID.ToString()); } }

        private nxProperty    mDevice;            public string Device           { get { return mDevice.Value; }         set { mDevice.Value   = value; } }
        private string        mDeviceID;          public string DeviceID         { get { return mDeviceID; }             set { mDeviceID       = value; } }

        private nxProperty    mMedia;             public string Media            { get { return mMedia.Value; }          set { mMedia.Value   = value; } }
        private string        mMediaID;           public string MediaID          { get { return mMediaID; }              set { mMediaID       = value; } }

        private string        mLineitemPK;        public string LineitemPK       { get { return mLineitemPK; }           set { mLineitemPK    = value; } }
        private string        mBatchID;           public string BatchID          { get { return mBatchID; }              set { mBatchID       = value; } }
        private string        mEntryID;           public string EntryID          { get { return mEntryID; }              set { mEntryID       = value; } }

        private dlBatch       mBatch;             public dlBatch Batch           { get { return mBatch; }                set { mBatch = value; } }

                                                  public string Computer         { get { return mBatch.Computer; }       set { mBatch.Computer   = value; } }
                                                  public string ComputerID       { get { return mBatch.ComputerID; }     set { mBatch.ComputerID = value; } }

                                                  public string Worker           { get { return mBatch.Worker; }         set { mBatch.Worker = value; } }
                                                  public string WorkerID         { get { return mBatch.WorkerID; }       set { mBatch.WorkerID = value; } }



        public event         OnNewDataEventHandler OnNewData;   public delegate void OnNewDataEventHandler();

        public dlLineitem()
        {
            mBatch = new dlBatch();
            Create();
        }
        public dlLineitem(dlBatch batch, string LineitemGroup)
        {
            mBatch = batch;
            mLineitemGroup = LineitemGroup;

            Create();
        }
        private void Create()
        {
            mBatchID       = mBatch.BatchPK.ToString();

            mLineitemType = new nxProperty("Lineitem", "LineitemType");
            mLineitemState  = new nxProperty("Lineitem", "LineitemState");
            mLineitemDate   = new nxProperty("Lineitem", "LineitemDate");
            mLineitemNo     = new nxProperty("Lineitem", "LineitemNo");
            mReference      = new nxProperty("Lineitem", "Reference");
            mAmount         = new nxProperty("Lineitem", "Amount");
            mDetail         = new nxProperty("Lineitem", "Detail");
            mHuman          = new nxProperty("Lineitem", "Human");
            mDevice         = new nxProperty("Lineitem", "Device");
            mMedia          = new nxProperty("Lineitem", "Media");

            Reset();
        }

        public bool Open(int LineitemPK) { return Open(LineitemPK.ToString()); }
        /// <summary>
        /// Load values from database.
        /// </summary>
        /// <param name="targetId">The Primary Key of the row to load values from.</param>
        /// <returns>True if successful, else false.</returns>
        public bool Open(string LineitemPK)
        {
            bool success = false;

            mLineitemPK = LineitemPK;

            SQLServer DB = new SQLServer(setting.ConnectionString);

            DataTable DT = DB.GetDataTable("acur.lst_Lineitem", "I~S~LineitemPK~" + mLineitemPK);
            if (DT.Rows.Count > 0)
                loadFromRow(DT.Rows[0]);

            success = true;
  
            DB.Dispose();
            DB = null;

            return success;
        }

        public bool OpenCurrent(string DeviceID, string ItemType)
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("[acur].[lst_Lineitem_EgmMeter]", "I~S~LimitRows~1;I~S~DeviceID~" + DeviceID + ";I~S~ItemType~" + EnumCache.Instance.getTypeId("Lineitem", ItemType));
            if (DT.Rows.Count > 0)
                loadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }

        public bool OpenItemDate(string DeviceID, string ItemDate, string ItemType)
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("[acur].[lst_Lineitem_EgmMeter]", "I~S~LimitRows~1;I~S~DeviceID~" + DeviceID + ";I~S~MeterDate~" + ItemDate + ";I~S~ItemType~" + EnumCache.Instance.getTypeId("Lineitem", ItemType));
            if (DT.Rows.Count > 0)
                loadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }

        public bool OpenBatch(string DeviceID, string BatchID, string ItemType)
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("[acur].[lst_Lineitem_EgmMeter]", "I~S~LimitRows~1;I~S~DeviceID~" + DeviceID + ";I~S~BatchID~" + BatchID + ";I~S~ItemType~" + EnumCache.Instance.getTypeId("Lineitem", ItemType));
            if (DT.Rows.Count > 0)
                loadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }

        public bool OpenPrevious(string DeviceID, string ItemType, string ItemDate)
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);

            string PrevUK = DB.ExecLookup("Select top 1 MeterUK from acur.Lineitem_EgmMeter as m left join acur.Lineitem as l where l.DeviceID=" + DeviceID + " and m.ItemType=" + EnumCache.Instance.getTypeId("Lineitem", ItemType) + " and m.meterdate<" + ItemDate);

            DB.Dispose();
            DB = null;

            Open(PrevUK);

            return String.IsNullOrEmpty(mDataError);
        }


        /// <summary>
        /// Populates the instance of this class with data from a DataRow.
        /// </summary>
        /// <param name="Row">A DataRow from the database table.</param>
        public void loadFromRow(DataRow Row)
        {
            if (Row != null)
            {
                if (Row.Table.Columns.Contains("LineitemPK")) 
                    mLineitemPK = Row["LineitemPK"].ToString();

                mBatchID              = Row["BatchID"].ToString();
                mEntryID              = Row["EntryID"].ToString();

                mLineitemType.Value   = Row["LineitemType"].ToString();
                mLineitemTypeID       = Row["LineitemTypeID"].ToString();
                mLineitemState.Value  = Row["LineitemState"].ToString();
                mLineitemStateID      = Row["LineitemStateID"].ToString();

                mLineitemDate.Value   = Row["LineitemDate"].ToString();
                mLineitemNo.Value     = Row["LineitemNo"].ToString();
                mReference.Value      = Row["Reference"].ToString();
                mAmount.Value         = Row["Amount"].ToString();
                mDetail.Value         = Row["Detail"].ToString();

                mHuman.Value          = Row["Human"].ToString();
                mHumanID              = Row["HumanID"].ToString();
                mDevice.Value         = Row["Device"].ToString();
                mDeviceID             = Row["DeviceID"].ToString();
                mMedia.Value          = Row["Media"].ToString();
                mMediaID              = Row["MediaID"].ToString();

                mNewEntry = false;
            }
        }


        /// <summary>
        /// If mNewEntry is true, will attempt to insert a new row into the database, otherwise will attempt to update an existing row using the mLineitemPK.
        /// </summary>
        /// <returns>Will return true if successful, else false.</returns>
        public bool Update() { return Update(""); }
        public bool Update(string Externals)
        {
            string Params = "";
            if (!mNewEntry) Params = "I~S~LineitemPK~" + mLineitemPK + ";";
            return Update("acur.mod_Lineitem", Params + Externals + getParams()   );
        }

        public bool Update(string ProcName, string Params)
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);

            mLineitemPK = DB.ExecLookup(ProcName, Params ).ToString();

            mDataError = DB.DataError;
            if (String.IsNullOrEmpty(mDataError))
                 { UpdateProperty(); }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Change", "Event", "UpdateError"), mDataError); }

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);

        }

        public string getParams()
        {
            string Params = string.Empty;            

            if (!string.IsNullOrEmpty(mBatchID))            Params += "I~I~BatchID~"        + BatchID + ";";
            if (!string.IsNullOrEmpty(mLineitemTypeID))     Params += "I~I~LineitemType~"   + mLineitemTypeID       + ";";
            if (!string.IsNullOrEmpty(mLineitemStateID))    Params += "I~I~LineitemState~"  + mLineitemStateID      + ";";
            //if (string.IsNullOrEmpty(mLineitemDate.Value))
            {
                mLineitemDate.Value = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff");
            }
            Params += "I~D~LineitemDate~" + mLineitemDate.Value + ";";

            if (!string.IsNullOrEmpty(mAmount.Value))       Params += "I~F~Amount~"         + mAmount.Value         + ";";

            if (!string.IsNullOrEmpty(mReference.Value))    Params += "I~S~Reference~"      + mReference.Value      + ";";
            if (!string.IsNullOrEmpty(mDetail.Value))       Params += "I~S~Detail~"         + mDetail.Value         + ";";
            if (!string.IsNullOrEmpty(mLineitemNo.Value))   Params += "I~S~LineitemNo~"     + mLineitemNo.Value     + ";";

            if (!string.IsNullOrEmpty(mHumanID))            Params += "I~I~HumanID~"        + mHumanID  + ";";
            if (!string.IsNullOrEmpty(mDeviceID))           Params += "I~I~DeviceID~"       + mDeviceID + ";";
            if (!string.IsNullOrEmpty(mMediaID))            Params += "I~I~MediaID~"        + mMediaID  + ";";
            if (!string.IsNullOrEmpty(mEntryID))            Params += "I~I~EntryID~"        + mEntryID  + ";";
            if (!string.IsNullOrEmpty(mEntryID))            Params += "I~I~ComputerID~"     + shell.Instance.ComputerID;

            return Params;
        }

        public void UpdateProperty()
        {
            if (!mNewEntry)
            {
                mBatch.UpdateProperty();

                mLineitemType.Update();
                mLineitemState.Update();
                mLineitemDate.Update();
                mLineitemNo.Update();
                mReference.Update();
                mAmount.Update();
                mDetail.Update();
                mHuman.Update();
                mDevice.Update();
                mMedia.Update();
            }
        }

        public bool SetState(string LineitemID, string State, string DocketNo)
        {
            bool ret = false;

            dlLineitem lineitem = new dlLineitem(mBatch, "Lineitem");
            if (lineitem.Open(LineitemID))
            {
                lineitem.LineitemState = State;
                ret = lineitem.Update();
            }

            return ret;

        }

        /// <summary>
        /// Sets the EventAction state to 'Deleted'. EventAction will still remain in the database.
        /// </summary>
        /// <returns>True if the state change was successful, else false.</returns>
        public bool Delete()
        {
            string SQL = "declare @LineitemPK int; Set @LineitemPK=" + mLineitemPK + " ";
            SQL += "declare @LineitemState int; Set @LineitemState=" + EnumCache.Instance.getTypeId("Entity", "Deleted") + " ";
            SQL += "update [acur].[Lineitem] set LineitemState=@LineitemState where LineitemPK=@LineitemPK";

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DB.ExecNonQuery(SQL);
                mDataError = DB.DataError;
            }

            return String.IsNullOrEmpty(mDataError);
        }


        public void Reset() { Reset(""); }
        public void Reset(string LineType)
        {
            LineitemType          = LineitemType;
            LineitemState         = "Pending";

            mLineitemDate.Value   = DateTime.Now.ToString("MMM dd, yyyy HH:mm");
            mLineitemNo.Value     = "";
            mReference.Value      = "";
            mAmount.Value         = "";
            mDetail.Value         = "";
            HumanID               = "0";
            DeviceID              = "0";
            MediaID               = "0";
            EntryID               = "0";

            mNewEntry = true;
        }

        #region IDisposable Implementation
        // To detect redundant calls
        private bool disposedValue = false;
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing) { }
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
