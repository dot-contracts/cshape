using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlCashless : IDisposable
    {

        private bool          mNewEntry;
        private string        mDataError;         public string DataError        { get { return mDataError; }            set { mDataError = value; } }

        private string        mMediaUI;           public string MediaUI          { get { return mMediaUI; }              set { mMediaUI          = value; } }


        private nxProperty    mPlayer;            public string Player           { get { return mPlayer.Value; }         set { mPlayer.Value      = value; } }
        private string        mPlayerID;          public string PlayerID         { get { return mPlayerID; }             set { mPlayerID          = value;  } }

        //private nxProperty    mAmount;            public string Amount           { get { return mAmount.Value; }     set { mAmount.Value = value; } }
        private nxProperty    mCashDate;          public string CashDate         { get { return mCashDate.Value; }       set { mCashDate.Value = value; } }


        private dlLineitem    mLineitem;          public dlLineitem Lineitem     { get { return mLineitem; }                  set { mLineitem = value; } }

                                                  public string CashlessPK       { get { return mLineitem.LineitemPK; }       set { mLineitem.LineitemPK = value; } }
                                                  public string BatchID          { get { return mLineitem.BatchID; }          set { mLineitem.BatchID = value; } }

                                                  public string CashlessType     { get { return mLineitem.LineitemType; }     set { mLineitem.LineitemType   = value;  } }
                                                  public string CashlessTypeID   { get { return mLineitem.LineitemTypeID; }   set { mLineitem.LineitemTypeID = value;  } }

                                                  public string CashlessState    { get { return mLineitem.LineitemState; }    set { mLineitem.LineitemState   = value; } }
                                                  public string CashlessStateID  { get { return mLineitem.LineitemStateID; }  set { mLineitem.LineitemStateID = value; } }

                                                  public string Computer         { get { return mLineitem.Computer; }         set { mLineitem.Computer = value; } }
                                                  public string ComputerID       { get { return mLineitem.ComputerID; }       set { mLineitem.ComputerID     = value;  } }

                                                  public string Worker           { get { return mLineitem.Worker; }      set { mLineitem.Worker   = value; } }
                                                  public string WorkerID         { get { return mLineitem.WorkerID; }    set { mLineitem.WorkerID = value;  } }

                                                  public string Human            { get { return mLineitem.Human; }       set { mLineitem.Human    = value; } }
                                                  public string HumanID          { get { return mLineitem.HumanID; }     set { mLineitem.HumanID  = value;  } }

                                                  public string Device           { get { return mLineitem.Device; }      set { mLineitem.Device   = value; } }
                                                  public string DeviceID         { get { return mLineitem.DeviceID; }    set { mLineitem.DeviceID = value;  } }

                                                  public string DocketNo         { get { return mLineitem.LineitemNo; }  set { mLineitem.LineitemNo = value; } }
                                                  public string Reference        { get { return mLineitem.Reference; }   set { mLineitem.Reference = value; } }
                                                  public string Amount           { get { return mLineitem.Amount; }      set { mLineitem.Amount = value; } }
                                                  public string Detail           { get { return mLineitem.Detail; }      set { mLineitem.Detail = value; } }

        public event         OnNewDataEventHandler OnNewData;  public delegate void OnNewDataEventHandler();

        public dlCashless()
        {
            //mCashlessNo     = new nxProperty("Cashless", "CashlessNo");
            //mDevice     = new nxProperty("Cashless", "Device");
            //mAmount     = new nxProperty("Cashless", "Amount");
            //mCashDate       = new nxProperty("Cashless", "CashDate");
            //mCloseDevice    = new nxProperty("Cashless", "CloseDevice");
            //mCloseAmount    = new nxProperty("Cashless", "CloseAmount");
            //mCloseDate      = new nxProperty("Cashless", "CloseDate");
            mPlayer         = new nxProperty("Cashless", "Player");

            mLineitem = new dlLineitem();

            Reset();

        }


        public bool Open(int targetId) { return Open(targetId.ToString()); }
        /// <summary>
        /// Load values from database.
        /// </summary>
        /// <param name="targetId">The Primary Key of the row to load values from.</param>
        /// <returns>True if successful, else false.</returns>
        public bool Open(string targetId)
        {
            bool success = false;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("acur.lst_Lineitem_Cashless", "I~S~CashlessUK~" + targetId);
                if (DT.Rows.Count > 0)
                    loadFromRow(DT.Rows[0]);
            }

            success = true;

            return success;
        }
        /// <summary>
        /// Populates the instance of this class with data from a DataRow.
        /// </summary>
        /// <param name="Row">A DataRow from the database table.</param>
        private void loadFromRow(DataRow Row)
        {
            if (Row != null)
            {
                mLineitem.loadFromRow(Row);

                //mCashlessNo.Value     = Row["CashlessNo"].ToString();

                //mDevice.Value     = Row["Device"].ToString();
                //mAmount.Value     = Row["Amount"].ToString();
                //mCashDate.Value       = Row["CashDate"].ToString();

                //mCloseDevice.Value    = Row["CloseDevice"].ToString();
                //mCloseAmount.Value    = Row["CloseAmount"].ToString();
                //mCloseDate.Value      = Row["CloseDate"].ToString();

                mPlayer.Value         = Row["Player"].ToString();
                mPlayerID             = Row["PlayerID"].ToString();

                mNewEntry = false;
            }
        }


        /// <summary>
        /// If mNewEntry is true, will attempt to insert a new row into the database, otherwise will attempt to update an existing row using the mCashlessPK.
        /// </summary>
        /// <returns>Will return true if successful, else false.</returns>
        public bool Update()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params = "";

                if (!mNewEntry)
                    Params = "I~S~CashlessPK~" + mLineitem.LineitemPK + ";";

                DB.ExecNonQuery("acur.mod_Lineitem_Cashless", Params + getParams(), false).ToString();

                mDataError = DB.DataError;
                if (String.IsNullOrEmpty(mDataError))
                {
                    //mCashlessNo.Update();
                    //mDevice.Update();
                    //mAmount.Update();
                    //mCashDate.Update();
                    //mCloseDevice.Update();
                    //mCloseAmount.Update();
                    //mCloseDate.Update();
                    mPlayer.Update();
                }
                else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Change", "Event", "UpdateError"), mDataError); }

            }

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        {
            string Params = "I~S~LineItemNo~";// + mCashlessNo.Value + ";";
            //Params += "I~S~Device~"       + mDevice.Value;
            //Params += "I~S~Amount~"       + mAmount.Value;
            //Params += "I~D~CashDate~"         + mCashDate.Value;
            //Params += "I~S~CloseDevice~"      + mCloseDevice.Value;
            //Params += "I~S~CloseAmount~"      + mCloseAmount.Value;
            //Params += "I~D~CloseDate~"        + mCloseDate.Value;
            return Params + mLineitem.getParams();
        }


        //public bool Create(string BatchID, string CashlessType, string MediaUI, string DeviceID, string HumanID, double Amount, string CreditDate)
        //{
        //    dlCashless cashless = new dlCashless();

        //    cashless.CashlessType  = CashlessType;
        //    cashless.BatchID       = BatchID;
        //    cashless.MediaUI       = MediaUI;
        //    cashless.DeviceID  = DeviceID;
        //    cashless.Amount    = Amount.ToString("0.00");
        //    cashless.CashDate      = CreditDate;

        //    bool ret = cashless.Update();
        //    mCashlessPK = cashless.CashlessUK;
        //    return ret;
        //}

        public bool SetState(string DocketUI, string State, string DocketNo)
        {
            bool ret = false;

            //dlDocket docket = new dlDocket();
            //if (docket.Open(DocketUI))
            //{
            //    docket.DocketState = State;

            //    switch (State.ToUpper())
            //    {
            //        case "VALIDATED": //
            //            docket.DocketNo = DocketNo;
            //            docket.CloseDate = DateTime.Now.ToString("dd MMM, yyyy HH:mm");
            //            break;
            //        case "PENDING": //
            //            docket.DocketNo = "";
            //            docket.CloseDate = DateTime.MinValue.ToString("dd MMM, yyyy HH:mm");
            //            break;
            //        case "VOIDED": // Leave as is
            //            break;
            //    }
            //    ret = docket.Update();
            //}

            return ret;

        }
        /// <summary>
        /// Sets the EventAction state to 'Deleted'. EventAction will still remain in the database.
        /// </summary>
        /// <returns>True if the state change was successful, else false.</returns>
        public bool Delete()
        {
            string SQL = "declare @CashlessUK int; Set @CashlessUK=" + mLineitem.LineitemPK + " ";
            SQL += "declare @CashlessState int; Set @CashlessState=" + EnumCache.Instance.getTypeId("Entity", "Deleted") + " ";
            SQL += "update [acur].[Lineitem_Cashless] set CashlessState=@CashlessState where CashlessUK=@CashlessUK";

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DB.ExecNonQuery(SQL);
                mDataError = DB.DataError;
            }

            return String.IsNullOrEmpty(mDataError);
        }




        /// <summary>
        /// Set all Player variables to default values.
        /// </summary>
        public void Reset()
        {
            CashlessTypeID        = "";
            CashlessStateID       = "";
            //mCashlessNo.Value     = "";
            //mDevice.Value     = "";
            //mAmount.Value     = "";
            //mCashDate.Value       = "";
            //mCloseDevice.Value    = "";
            //mCloseAmount.Value    = "";
            //mCloseDate.Value      = "";
            ComputerID            = "";
            WorkerID              = "";
            PlayerID              = "";

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
