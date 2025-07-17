using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache;

namespace nexus.common.dal
{
    public enum DateInterval { Day, DayOfYear, Hour, Minute, Month, Quarter, Second, Weekday, WeekOfYear, Year }

    public class dlBatch : IDisposable
    {
        private bool          mNewEntry;          public bool   NewEntry      { get { return mNewEntry; }           set { mNewEntry      = value; } }
        private string        mDataError;         public string DataError     { get { return mDataError; }          set { mDataError     = value; } }

        private int           mBatchPK;           public int    BatchPK       { get { return mBatchPK; }            set { mBatchPK       = value; } }
        private string        mParentID = "";     public string ParentID      { get { return mParentID; }           set { mParentID      = value; } }
        private string        mVenueID = "";      public string VenueID       { get { return mVenueID; }            set { mVenueID       = value; } }

        private nxProperty    mBatchType;         public string BatchType     { get { return mBatchType.Value; }    set { mBatchType.Value = value;   mBatchTypeID       = EnumCache.Instance.getTypeFromDesc  ("Batch",  mBatchType.Value); } }
        private string        mBatchTypeID;       public string BatchTypeID   { get { return mBatchTypeID; }        set { mBatchTypeID  = value;      mBatchType.Value   = EnumCache.Instance.getDescFromId    (           mBatchTypeID); } }

        private nxProperty    mBatchState;        public string BatchState    { get { return mBatchState.Value; }   set { mBatchState.Value = value;  mBatchStateID      = EnumCache.Instance.getStateFromDesc ("Batch",  mBatchState.Value); } }
        private string        mBatchStateID;      public string BatchStateID  { get { return mBatchStateID; }       set { mBatchStateID = value;      mBatchState.Value  = EnumCache.Instance.getDescFromId    (           mBatchStateID); } }
         
        private nxProperty    mComputer;          public string Computer      { get { return mComputer.Value; }     set { mComputer.Value = value; } }
        private string        mComputerID;        public string ComputerID    { get { return mComputerID; }         set { mComputerID    = value;  } }

        private nxProperty    mWorker;            public string Worker        { get { return mWorker.Value; }       set { mWorker.Value = value; } }
        private string        mWorkerID;          public string WorkerID      { get { return mWorkerID; }           set { mWorkerID      = value;      mWorker.Value       = shell.Instance.Worker.Description; } }

        private nxProperty    mDebit;             public string Debit         { get { return mDebit.Value; }        set { mDebit.Value = value;  } }
        private nxProperty    mCredit;            public string Credit        { get { return mCredit.Value; }       set { mCredit.Value = value;  } }
        private nxProperty    mOpening;           public string Opening       { get { return mOpening.Value; }      set { mOpening.Value = value;  } }
        private nxProperty    mExpected;          public string Expected      { get { return mExpected.Value; }     set { mExpected.Value = value; } }
        private nxProperty    mActual;            public string Actual        { get { return mActual.Value; }       set { mActual.Value = value; } }
        private nxProperty    mAdjustment;        public string Adjustment    { get { return mAdjustment.Value; }   set { mAdjustment.Value = value; } }

        private nxProperty    mReference;         public string Reference     { get { return mReference.Value; }    set { mReference.Value = value; } }
        private string        mMemo   = "";       public string Memo          { get { return mMemo; }               set { mMemo          = value; } }
        private string        mMemoID = "";       public string MemoID        { get { return mMemoID; }             set { mMemoID        = value; } }

        private nxProperty    mOpenTime;          public string StartTime     { get { return mOpenTime.Value; }     set { mOpenTime.Value = value; } }
        private nxProperty    mCloseTime;         public string FinishTime    { get { return mCloseTime.Value; }    set { mCloseTime.Value = value; } }

        public const string STATE_OPEN      = "Open";
        public const string STATE_CLOSED    = "Closed";
        public const string STATE_SUSPENDED = "Suspended";
        public const string STATE_WAITING   = "Waiting";

        public event         OnNewDataEventHandler OnNewData;
        public delegate void OnNewDataEventHandler();

        public dlBatch(string BatchType= "Batch")
        {
            mBatchType      = new nxProperty("Batch", "BatchType");
            mBatchState     = new nxProperty("Batch", "BatchState");
            mWorker         = new nxProperty("Batch", "Worker");
            mComputer       = new nxProperty("Batch", "Computer");
            mDebit          = new nxProperty("Batch", "Debit");
            mCredit         = new nxProperty("Batch", "Credit");
            mOpening        = new nxProperty("Batch", "Opening");
            mExpected       = new nxProperty("Batch", "Expected");
            mActual         = new nxProperty("Batch", "Actual");
            mReference      = new nxProperty("Batch", "Reference");
            mOpenTime       = new nxProperty("Batch", "StartTime");
            mCloseTime      = new nxProperty("Batch", "FinishTime");
            mAdjustment     = new nxProperty("Batch", "Adjustment");

            Reset(BatchType);
        }

 
        public void Dispose() { }

        public bool Open(int targetId)
        {
            string shiftid = (targetId > 0) ? targetId.ToString() : string.Empty;

            return Open(shiftid, null, STATE_OPEN);
        }

        /// <summary>
        /// Load values from database.
        /// </summary>
        /// <param name="targetId">The Primary Key of the row to load values from.</param>
        /// <returns>True if successful, else false.</returns>
        public bool Open(string bpk = null, string pid = null, string state = STATE_OPEN)
        {
            bool success = false;
            string param = string.Empty;
            DataTable DT = new DataTable();

            BatchStateID = EnumCache.Instance.getStateId("Batch", state);
            mBatchPK = 0;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                if (!string.IsNullOrEmpty(bpk))           param += "I~I~BatchPK~"     + bpk + ";";                
                if (!string.IsNullOrEmpty(mBatchTypeID))  param += "I~S~BatchType~"   + mBatchTypeID + ";";
                if (!string.IsNullOrEmpty(mBatchStateID)) param += "I~S~BatchState~"  + mBatchStateID + ";";
                if (!string.IsNullOrEmpty(pid))           param += "I~S~ParentID~"    + pid + ";";
                //if (!string.IsNullOrEmpty(mComputerID))   param += "I~I~ComputerID~"  + mComputerID + ";";
                if (!string.IsNullOrEmpty(mWorkerID))     param += "I~I~WorkerID~"    + mWorkerID + ";";


                DT = DB.GetDataTable("acur.lst_Batch", param);

                if (DT.Rows.Count > 0)
                {
                    loadFromRow(DT.Rows[0]);
                    success = true;
                }
            }

            DT.Dispose();
  
            return success;
        }        

        /// <summary>
        /// Populates the instance of this class with data from a DataRow.
        /// </summary>
        /// <param name="Row">A DataRow from the database table.</param>
        public bool loadFromRow(DataRow Row)
        {
            bool ret = false;
            if (Row != null)
            {
                int.TryParse (Row["BatchPK"].ToString(), out mBatchPK);  

                mBatchType.Value   = Row["BatchType"].ToString();
                mBatchTypeID       = Row["BatchTypeID"].ToString();
                mBatchState.Value  = Row["BatchState"].ToString();
                mBatchStateID      = Row["BatchStateID"].ToString();

                mParentID          = Row["ParentID"].ToString();
                mWorker.Value      = Row["Worker"].ToString();
                mWorkerID          = Row["WorkerID"].ToString();
                mComputer.Value    = Row["Computer"].ToString();
                mComputerID        = Row["ComputerID"].ToString();
                mMemo              = Row["Memo"].ToString();
                mMemoID            = Row["MemoID"].ToString();

                mDebit.Value       = Row["Value1"].ToString();
                mCredit.Value      = Row["Value2"].ToString();
                mOpening.Value     = Row["Value3"].ToString();
                mExpected.Value    = Row["Value4"].ToString();
                mActual.Value      = Row["Value5"].ToString();
                //mReference.Value   = Row["Reference"].ToString();

                if (mBatchType.Equals("Period"))
                {
                    DateTime dt;
                    if (DateTime.TryParse(Row["OpenTime"].ToString(), out dt))
                        mOpenTime.Value = dt.ToString("MMM dd, yyyy") + " " + shell.Instance.Venue.DayStart;
                    else
                        mOpenTime.Value= DateTime.Now.ToString("MMM dd, yyyy") +  " " + shell.Instance.Venue.DayStart;

                    if (DateTime.TryParse(Row["CloseTime"].ToString(), out dt))
                        mCloseTime.Value = dt.ToString("MMM dd, yyyy") + " " + shell.Instance.Venue.DayStart;
                    else
                        mCloseTime.Value = DateTime.Now.ToString("MMM dd, yyyy") + " " + shell.Instance.Venue.DayStart;
                }
                else
                {
                    mOpenTime.Value  = Row["OpenTime"].ToString();
                    mCloseTime.Value = Row["CloseTime"].ToString();
                }

                ret       = true;
                mNewEntry = false;
            }
            return ret;
        }

        /// <summary>
        /// If mNewEntry is true, will attempt to insert a new row into the database, otherwise will attempt to update an existing row using the mBatchPK.
        /// </summary>
        /// <returns>Will return true if successful, else false.</returns>
        public bool Update(string state = null)
        {
            if (!string.IsNullOrEmpty(state))
            {
                BatchStateID = EnumCache.Instance.getStateId("Batch", state);
            }

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params = "";

                if (mBatchPK > 0)
                {
                    Params = "I~I~BatchPK~" + mBatchPK + ";";
                }

                if (!string.IsNullOrEmpty(mBatchTypeID))      Params +=  "I~I~BatchType~"   + mBatchTypeID      + ";";
                if (!string.IsNullOrEmpty(mBatchStateID))     Params +=  "I~I~BatchState~"  + mBatchStateID     + ";";

                if (!string.IsNullOrEmpty(mVenueID))          Params += "I~I~VenueID~"      + mVenueID          + ";";
                if (!string.IsNullOrEmpty(mParentID))         Params += "I~I~ParentID~"     + mParentID         + ";";
                if (!string.IsNullOrEmpty(mComputerID))       Params += "I~I~ComputerID~"   + mComputerID       + ";";
                if (!string.IsNullOrEmpty(mWorkerID))         Params += "I~I~WorkerID~"     + mWorkerID         + ";";

                if (!string.IsNullOrEmpty(mCloseTime.Value))  Params += "I~D~CloseTime~" + mCloseTime.Value + ";";
                if (!string.IsNullOrEmpty(mCloseTime.Value))  Params += "I~D~CloseDate~" + mCloseTime.Value + ";";

                if (!string.IsNullOrEmpty(mOpenTime.Value))   Params += "I~D~OpenTime~" + mOpenTime.Value + ";";
                if (!string.IsNullOrEmpty(mOpenTime.Value))   Params += "I~D~OpenDate~" + mOpenTime.Value + ";";

                if (!string.IsNullOrEmpty(mDebit.Value))      Params += "I~F~Value1~"       + mDebit.Value      + ";";
                if (!string.IsNullOrEmpty(mCredit.Value))     Params += "I~F~Value2~"       + mCredit.Value     + ";";
                if (!string.IsNullOrEmpty(mOpening.Value))    Params += "I~F~Value3~"       + mOpening.Value    + ";";
                if (!string.IsNullOrEmpty(mExpected.Value))   Params += "I~F~Value4~"       + mExpected.Value   + ";";
                if (!string.IsNullOrEmpty(mActual.Value))     Params += "I~F~Value5~"       + mActual.Value     + ";";                
                //Params += "I~S~Reference~" + mReference.Value + ";";

                int.TryParse(DB.ExecLookup("acur.mod_Batch", Params).ToString(), out mBatchPK);
                mDataError = DB.DataError;
                if (String.IsNullOrEmpty(mDataError))
                {
                    UpdateProperty();
                }
                else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Change", "Batch", "UpdateError"), mDataError); }
            }

            return String.IsNullOrEmpty(mDataError);
        }

        public void UpdateProperty()
        {
            if (!mNewEntry)
            {
                mBatchType.Update();
                mBatchState.Update();
                mOpenTime.Update();
                mCloseTime.Update();

                mWorker.Update();
                mComputer.Update();
                mDebit.Update();
                mCredit.Update();
                mOpening.Update();
                mExpected.Update();
                mActual.Update();
                mReference.Update();
                mAdjustment.Update();
            }
        }

        public void Close()
        {
            Delete(); // need to close this and all ist siblings down properly
        }

        public bool Close(double expected, double actual, string notes)
        {
            bool ret = false;

            if (mBatchPK > 0)
            {
                mCloseTime.Value = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff");
                mExpected.Value  = expected.ToString("0.00");
                mActual.Value    = actual.ToString("0.00");                
                BatchStateID     = EnumCache.Instance.getStateId("Batch", STATE_CLOSED);
                ret = Update();
            }
            return ret;
        }

        public bool ChangeState(string state)
        {
            bool ret = false;

            if (mBatchPK > 0)
            {
                if (state == STATE_CLOSED)
                {
                    mCloseTime.Value = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff");
                }                
                BatchStateID = EnumCache.Instance.getStateId("Batch", state);
                ret = Update();
            }
            return ret;
        }

        public bool IsSuspended()
        {
            return (BatchStateID == EnumCache.Instance.getStateId("Batch", STATE_SUSPENDED));
        }
    
        /// <summary>
        /// Sets the BatchAction state to 'Deleted'. BatchAction will still remain in the database.
        /// </summary>
        /// <returns>True if the state change was successful, else false.</returns>
        public bool Delete()
        {
            string SQL = "declare @BatchPK int; Set @BatchPK=" + mBatchPK + " ";
            SQL += "declare @BatchState int; Set @BatchState=" + EnumCache.Instance.getStateId("Batch", "Deleted") + " ";
            SQL += "update acur.Batch set BatchState=@BatchState where BatchPK=@BatchPK";

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DB.ExecNonQuery(SQL);
                mDataError = DB.DataError;
            }

            return String.IsNullOrEmpty(mDataError);
        }

        /// <summary>
        /// Set all member variables to default values.
        /// </summary>
        public void Reset() { Reset(mBatchType.Value); }

        public void Reset(string btype)
        {
            BatchType = EnumCache.Instance.getTypeFromDesc("Batch", btype);

            mBatchPK         = 0;
            BatchTypeID      = EnumCache.Instance.getTypeId("Batch", btype); ;
            BatchStateID     = EnumCache.Instance.getStateId("Batch", "Open");

            mParentID         = "";
            mVenueID          = shell.Instance.Venue.VenuePK; 
            mComputerID       = shell.Instance.ComputerID;
            mWorkerID         = shell.Instance.WorkerID;

            if (mBatchType.Equals("Period"))
            {
                DateTime dout;
                using (SQLServer DB = new SQLServer(setting.ConnectionString))
                {
                    DateTime.TryParse(DB.ExecLookup("select top 1 FinishTime from acur.Batch where BatchType=cmn.gettypeUK('Batch','Period') order by FinishTime desc"), out dout);
                }

                mOpenTime.Value = dout.ToString("MMM dd, yyyy") + " " + shell.Instance.Venue.DayStart;
                if (mOpenTime.Value.Equals("")) mOpenTime.Value = DateTime.Now.ToString("MMM dd, yyyy")   + " " + shell.Instance.Venue.DayStart;
                                                mOpenTime.Value = Convert.ToDateTime(mOpenTime.Value).ToString("MMM dd, yyyy HH:mm:ss.fff");

                if (DateTime.Now.Hour < 8) mCloseTime.Value = DateTime.Now.ToString("MMM dd, yyyy")            + " " + shell.Instance.Venue.DayStart;
                else                       mCloseTime.Value = DateTime.Now.AddDays(1).ToString("MMM dd, yyyy") + " " + shell.Instance.Venue.DayStart;
                                           mCloseTime.Value = Convert.ToDateTime(mCloseTime.Value).ToString("MMM dd, yyyy HH:mm:ss.fff");
            }
            else
            {
                mOpenTime.Value = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff");
                if (DateTime.Now.Hour < 8)
                    mCloseTime.Value = DateTime.Now.ToString("MMM dd, yyyy") + " " + shell.Instance.Venue.DayStart;
                else
                    mCloseTime.Value = DateTime.Now.AddDays(1).ToString("MMM dd, yyyy") + " " + shell.Instance.Venue.DayStart;
            }


            mOpening.Value    = "0";
            mDebit.Value      = string.Empty;
            mCredit.Value     = string.Empty;
            mOpening.Value    = string.Empty;
            mExpected.Value   = string.Empty;
            mActual.Value     = string.Empty;
            mReference.Value  = string.Empty;
            mAdjustment.Value = string.Empty;
            mNewEntry = true;
        }

    }

    public class DateAndTimeDiff
    {
        private static int GetQuarter(int nMonth)
        {
            if (nMonth <= 3)
                return 1;
            if (nMonth <= 6)
                return 2;
            if (nMonth <= 9)
                return 3;
            return 4;
        }

        public static DateTime DateAdd(DateInterval interval, DateTime dt, Int32 val)
        {
            switch (interval)
            {
                case DateInterval.Year:     return (dt.AddYears(val));
                case DateInterval.Quarter:  return (dt.AddMonths(val * 3));
                case DateInterval.Month:    return (dt.AddMonths(val));
                case DateInterval.Day:      return (dt.AddDays(val));
                case DateInterval.Hour:     return (dt.AddHours(val));
                case DateInterval.Minute:   return (dt.AddMinutes(val));
                case DateInterval.Second:   return (dt.AddSeconds(val));
            }

            return (dt);
        }

        public static double DateDiff(DateInterval interval, DateTime dt1, DateTime dt2, DayOfWeek eFirstDayOfWeek)
        {
            TimeSpan ts = dt2.Subtract(dt1);

            switch (interval)
            {
                case DateInterval.Year: //
                    return (dt2.Subtract(dt1).TotalDays / 365); //  '      .Year – dt1.Year);
                case DateInterval.Quarter: //
                    int Q1 = GetQuarter(dt1.Month);
                    int Q2 = GetQuarter(dt2.Month);
                    int d1 = Q2 - Q1;
                    int d2 = (4 * (dt2.Year - dt1.Year));
                    return (d1 + d2);
                case DateInterval.Month: //
                    int Month1 = dt1.Month;
                    int Month2 = dt2.Month;
                    int Months = Month2 - Month1;
                    int Year1 = dt1.Year;
                    int Year2 = dt2.Year;
                    int Years = Year2 - Year1;
                    return (Months + (12 * Years));
                case DateInterval.Day:
                case DateInterval.DayOfYear: //
                    return (Round(ts.TotalDays));
                case DateInterval.Hour: //
                    return (Round(ts.TotalHours));
                case DateInterval.Minute: //
                    return (Round(ts.TotalMinutes));
                case DateInterval.Second: //
                    return (Round(ts.TotalSeconds));

                case DateInterval.Weekday: //
                    return (Round(ts.TotalDays / 7.0));
                case DateInterval.WeekOfYear: //
                    while (dt2.DayOfWeek != eFirstDayOfWeek) dt2 = dt2.AddDays(-1);
                    while (dt1.DayOfWeek != eFirstDayOfWeek) dt1 = dt1.AddDays(-1);
                    return (Round(dt2.Subtract(dt1).TotalDays / 7.0));
            }

            return (0);
        }

        private static long Round(double dVal)
        {
            if (dVal >= 0) return ((long)System.Math.Floor(dVal));

            return ((long)System.Math.Ceiling(dVal));
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
