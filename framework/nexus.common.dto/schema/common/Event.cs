using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dal;
using nexus.common.core;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlEvent
    {

        //private DataTable     mEvents;

        private string        mEventPK;           public string EventPK      { get { return mEventPK; }       set { mEventPK = value; } }
        private string        mParentID = "";     public string Parent       { get { return mParentID; }      set { mParentID = value; } }

        private string        mEventType;         public string EventType    { get { return mEventType; }     set { mEventType= value;     mEventTypeID  = EnumCache.Instance.getTypeFromDesc  ("Event",  mEventType); } }
        private string        mEventTypeID;       public string EventTypeID  { get { return mEventTypeID; }   set { mEventTypeID = value;  mEventType    = EnumCache.Instance.getDescFromId    (          mEventTypeID); } }

        private string        mEventState;        public string EventState   { get { return mEventState; }    set { mEventState = value;   mEventStateID = EnumCache.Instance.getStateFromDesc ("Event",  mEventState); } }
        private string        mEventStateID;      public string EventStateID { get { return mEventStateID; }  set { mEventStateID = value; mEventState   = EnumCache.Instance.getDescFromId    (          mEventStateID); } }
         
        private string        mFunction;          public string Function     { get { return mFunction; }      set { mFunction = value; } }
        private string        mFunctionID;        public string FunctionID   { get { return mFunctionID; }    set { mFunctionID = value; } } //mFunction     =  shell.Instance..Description(mFunctionID.ToString()); } }

        private string        mEntity;            public string Entity       { get { return mEntity; }        set { mEntity = value; } }
        private string        mEntityID;          public string EntityID     { get { return mEntityID; }      set { mEntityID = value;  } }

        private string        mEntityType;        public string EntityType   { get { return mEntityType; }    set { mEntityType = value; } }
        private string        mEntityTypeID;      public string EntityTypeID { get { return mEntityTypeID; }  set { mEntityTypeID = value;  } }

        private string        mWorker;            public string Worker       { get { return mWorker; }        set { mWorker = value; } }
        private string        mWorkerID;          public string WorkerID     { get { return mWorkerID; }      set { mWorkerID = value;     mWorker       = shell.Instance.Worker.Description; } }

        private string        mTypeCode;          public string TypeCode     { get { return mTypeCode; }      set { mTypeCode = value;     mActionUI     = ActionCache.Instance.strId(mTypeCode, mActionType, mActionCode); } }
        private string        mActionType;        public string ActionType   { get { return mActionType; }    set { mActionType = value;   mActionUI     = ActionCache.Instance.strId(mTypeCode, mActionType, mActionCode); } }
        private string        mActionCode;        public string ActionCode   { get { return mActionCode; }    set { mActionCode = value;   mActionUI     = ActionCache.Instance.strId(mTypeCode, mActionType, mActionCode); } }
        private string        mActionUI;          public string ActionTypeID { get { return mActionUI; }      set { mActionUI = value; } }

        private string        mProperty = "";     public string Property     { get { return mProperty; }      set { mProperty = value; } }

        private string        mEventDate;         public string EventDate    { get { return mEventDate; }     set {mEventDate = value; } }

        private bool          mNewEntry;
        private string        mDataError;         public string   DataError  { get { return mDataError; }     set { mDataError = value; } }

        public event         OnNewDataEventHandler OnNewData;
        public delegate void OnNewDataEventHandler();

        public dlEvent() { Reset(); }

        //public bool Create()
        //{
        //    bool Valid = false;

        //    SQLServer DB = new SQLServer(setting.ConnectionString);
        //    string SQL = "SELECT top 100 e.EventPK, e.EventDate, Enum1.Description AS EventType, Enum2.Description AS ActionType, ent1.Description as Entity, ent2.Description as Worker, e.Property, e.eventtype as EventTypeID, a.actiontype as ActionTypeID, e.Entity as EntityID, e.worker as WorkerID, e.action as ActionUI ";
        //    SQL += "FROM cmn.Event as e INNER JOIN enums.Event_Action as a ON e.Action = a.ActionUI INNER JOIN cmn.EnumValue as Enum1 ON EventType = Enum1.ValuePK INNER JOIN cmn.EnumValue AS Enum2 ON e.ActionType = Enum2.ValuePK INNER JOIN cmn.Entity as ent1 ON e.EntityID = ent1.EntityID INNER JOIN cmn.Entity as ent2 ON e.WorkerID = ent2.EntityID   ";
        //    SQL += "order by EventDate Desc";
        //    mEvents = DB.GetDataTable(SQL);
        //    if (mEvents.Rows.Count > 0)
        //        LoadFromRow(mEvents.Rows[0]);
        //    else
        //        Reset();

        //    DB.Dispose();
        //    DB = null;

        //    return Valid;
        //}


        public void Dispose() { }

        public bool Open(int targetId) { return Open(targetId.ToString()); }
        /// <summary>
        /// Load values from database.
        /// </summary>
        /// <param name="targetId">The Primary Key of the row to load values from.</param>
        /// <returns>True if successful, else false.</returns>
        public bool Open(string targetId)
        {
            bool success = false;

            mEventPK = targetId;

            SQLServer DB = new SQLServer(setting.ConnectionString);

            DataTable DT = DB.GetDataTable("cmn.lst_Occurrence_Event", "I~S~EventPK~" + mEventPK);
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
                mEventPK      = Row["EventPK"].ToString();      // EventPK

                mEventTypeID  = Row["EventTypeID"].ToString();
                mEventStateID = Row["EventStateID"].ToString();
                mEntityID     = Row["EntityID"].ToString();
                mEntityTypeID = Row["EntityTypeID"].ToString();
                mWorkerID     = Row["WorkerID"].ToString();

                mTypeCode     = Row["TypeCode"].ToString();
                mActionType   = Row["ActionType"].ToString();
                mActionCode   = Row["ActionCode"].ToString();
                mActionUI     = ActionCache.Instance.strId(mTypeCode, mActionType, mActionCode);

                mEventType    = Row["EventType"].ToString();
                mEventState   = Row["EventState"].ToString();
                mEntity       = Row["Entity"].ToString();
                mWorker       = Row["Worker"].ToString();
                mWorker       = Row["Worker"].ToString();
                mProperty     = Row["Property"].ToString();

                mNewEntry = false;
            }
        }


        /// <summary>
        /// If mNewEntry is true, will attempt to insert a new row into the database, otherwise will attempt to update an existing row using the mEventPK.
        /// </summary>
        /// <returns>Will return true if successful, else false.</returns>
        public bool Update()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);

            string Params = "";

            if (!mNewEntry)
                Params = "I~S~EventPK~" + mEventPK + ";";

            Params += "I~S~EventTypeID~"   + mEventTypeID + ";";
            Params += "I~S~EventStateID~"  + mEventStateID + ";";
            Params += "I~S~EventActionID~" + mActionUI + ";";

            if (!mParentID.Equals("")) Params += "I~S~ParentID~" + mParentID + ";";
            if (!mEntityID.Equals("")) Params += "I~S~EntityID~" + mEntityID + ";";
            if (!mWorkerID.Equals("")) Params += "I~S~WorkerID~" + mWorkerID + ";";

            Params += "I~S~Property~" + mProperty;

            mEventPK = DB.ExecLookup("cmn.mod_Occurrence_Event", Params).ToString();

            mDataError = DB.DataError;
            if (String.IsNullOrEmpty(mDataError))
            {
                //mCompanyType.Update();
                //mCompanyState.Update();
                //mDescription.Update();
                //mCompanyNo.Update();
                //mContact.Update();
                //mProperty.Update();
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Change", "Event", "UpdateError"), mDataError); }

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);

        }
        
  
        /// <summary>
        /// Sets the EventAction state to 'Deleted'. EventAction will still remain in the database.
        /// </summary>
        /// <returns>True if the state change was successful, else false.</returns>
        public bool Delete()
        {
            string SQL = "declare @EventPK int; Set @EventPK=" + mEventPK + " ";
            SQL += "declare @EventState int; Set @EventState=" + EnumCache.Instance.getTypeId("Entity", "Deleted") + " ";
            SQL += "update [cmn].[Event] set EventState=@EventState where EventPK=@EventPK";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DB.ExecNonQuery(SQL);
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            return String.IsNullOrEmpty(mDataError);
        }




        /// <summary>
        /// Set all member variables to default values.
        /// </summary>
        public void Reset()
        {
            mEventType = "";
            mEventTypeID = "";
            mEventState = "";
            mEventStateID = EnumCache.Instance.getStateId("Event", "Active");
            mEventPK = "";
            mParentID = "";
            mEntity = "";
            mWorker = "";
            mActionCode = "";
            mProperty = "";
            mEventDate = DateTime.Now.ToString("dd MMM, yyyy HH:mm");

            mNewEntry = true;
        }
    }
}
