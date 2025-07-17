using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlCancel
    {

        private string        mDataError;         public string DataError        { get { return mDataError; }            set { mDataError = value; } }

        private nxProperty    mCurrent;           public string Current          { get { return mCurrent.Value; }        set { mCurrent.Value = value;  } }
        private nxProperty    mPrevious;          public string Previous         { get { return mPrevious.Value; }       set { mPrevious.Value = value; } }

        private dlLineitem    mLineItem;          public dlLineitem LineItem     { get { return mLineItem; }                 set { mLineItem = value; } }

                                                  public string CancelPK         { get { return mLineItem.LineitemPK; }      set { mLineItem.LineitemPK = value; } }
                                                  public bool   NewEntry         { get { return mLineItem.NewEntry; }        set { mLineItem.NewEntry = value; } }

                                                  public string CancelType       { get { return mLineItem.LineitemType; }    set { mLineItem.LineitemType = value; } }
                                                  public string CancelTypeID     { get { return mLineItem.LineitemTypeID; }  set { mLineItem.LineitemTypeID = value; } }
                                                  public string CancelState      { get { return mLineItem.LineitemState; }   set { mLineItem.LineitemState = value; } }
                                                  public string CancelStateID    { get { return mLineItem.LineitemStateID; } set { mLineItem.LineitemStateID = value; } }
                                                  public string CancelDate       { get { return mLineItem.LineitemDate; }    set { mLineItem.LineitemDate = value; ; } }
                                                  public string CancelNo         { get { return mLineItem.LineitemNo; }      set { mLineItem.LineitemNo = value; ; } }
                                                  public string Reference        { get { return mLineItem.Reference; }       set { mLineItem.Reference = value; } }
                                                  public string Detail           { get { return mLineItem.Detail; }          set { mLineItem.Detail = value; } }
                                                  public string Amount           { get { return mLineItem.Amount; }          set { mLineItem.Amount = value; } }
        
                                                  public string Computer         { get { return mLineItem.Computer; }        set { mLineItem.Computer = value; } }
                                                  public string ComputerID       { get { return mLineItem.ComputerID ; }     set { mLineItem.ComputerID = value; } }
                                                  public string Worker           { get { return mLineItem.Worker; }          set { mLineItem.Worker = value; } }
                                                  public string WorkerID         { get { return mLineItem.WorkerID ; }       set { mLineItem.WorkerID = value; } }
                                                  public string Human            { get { return mLineItem.Human; }           set { mLineItem.Human = value; } }
                                                  public string HumanID          { get { return mLineItem.HumanID ; }        set { mLineItem.HumanID = value; } }
                                                  public string Device           { get { return mLineItem.Device; }          set { mLineItem.Device = value; } }
                                                  public string DeviceID         { get { return mLineItem.DeviceID ; }       set { mLineItem.DeviceID = value; } }
        

        public event         OnNewDataEventHandler OnNewData; public delegate void OnNewDataEventHandler();

        public dlCancel(dlBatch batch)
        {
            mLineItem       = new dlLineitem(batch, "Cancel");
            mCurrent        = new nxProperty("Cancel", "Current");
            mPrevious       = new nxProperty("Cancel", "Previous");

            Reset();

        }


        public void Dispose() { }

        public bool Open(int targetId) { return Open(targetId.ToString()); }
        /// <summary>
        /// Load values from database.
        /// </summary>
        /// <param name="targetId">The Primary Key of the row to load values from.</param>
        /// <returns>True if successful, else false.</returns>
        public bool Open(string CancelPK)
        {
            bool success = false;

            SQLServer DB = new SQLServer(setting.ConnectionString);

            DataTable DT = DB.GetDataTable("acur.lst_Lineitem_Cancel", "I~S~CancelPK~" + CancelPK);
            if (DT.Rows.Count > 0)
                loadFromRow(DT.Rows[0]);

            success = true;
  
            DB.Dispose();
            DB = null;

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
                mLineItem.loadFromRow(Row);

                mPrevious.Value       = Row["Previous"].ToString();
                mCurrent.Value        = Row["Current"].ToString();

            }
        }


        /// <summary>
        /// If mNewEntry is true, will attempt to insert a new row into the database, otherwise will attempt to update an existing row using the mCancelPK.
        /// </summary>
        /// <returns>Will return true if successful, else false.</returns>
        public bool Update()
        {

            string Params = "";
            if (!mLineItem.NewEntry)
                Params = "I~S~CancelPK~" + mLineItem.LineitemPK + ";";
            Params += "I~S~Current~"     + mCurrent.Value       + ";";
            Params += "I~S~Previous~"    + mPrevious.Value      + ";";

            if (mLineItem.Update("acur.mod_Lineitem_Cancel", Params + mLineItem.getParams() ))
            { 
                mPrevious.Update();
                mCurrent.Update();
                return true;
            }
            return false;
        }

        public bool Create(int BatchId, int EgmPk, double Amount, double Current, double Previous, string LineItemNo, string Reference, int ComputerId, int WorkerId, int HumanId, int DeviceId)
        {
            Reset();

            mPrevious     = new nxProperty("Cancel", "Previous");
            mCurrent      = new nxProperty("Cancel", "Current");

            return true;
        }

        public bool Insert(int BatchId, int EgmPk, double Amount, double Current, double Previous, string LineItemNo, string Reference, int ComputerId, int WorkerId, int HumanId, int DeviceId)
        {
            Reset();

            mPrevious = new nxProperty("Cancel", "Previous");
            mCurrent = new nxProperty("Cancel", "Current");

            return true;
        }




        /// <summary>
        /// Set all Human variables to default values.
        /// </summary>
        public void Reset()
        {
            mLineItem.Reset("Cancel");
            mPrevious.Value = "";
            mCurrent.Value = "";
        }

    }
}
