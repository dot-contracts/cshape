using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlEventAction
    {

        private DataTable     mActionTable = new DataTable();


        private bool          mNewEntry;
        private string        mDataError;        public string   DataError     { get { return mDataError; }          set { mDataError = value; } }
        private string        mActionPK;         public string   ActionPK      { get { return mActionPK; } }

        private nxProperty    mActionType;       public string ActionType      { get { return mActionType.Value; }   set { mActionType.Value = value;   mActionTypeID     = ActionCache.Instance.Value  (mActionType.Value); } }
        private string        mActionTypeID;     public string ActionTypeID    { get { return mActionTypeID; }       set { mActionTypeID = value;       mActionType.Value = ActionCache.Instance.Display(mActionTypeID); } }


        private nxProperty    mActionState;      public string ActionState     { get { return mActionState.Value; }  set { mActionState.Value = value;  mActionStateID     = EnumCache.Instance.getStateFromDesc("Entity", mActionState.Value); } }
        private string        mActionStateID;    public string ActionStateID   { get { return mActionStateID; }      set { mActionStateID = value;      mActionState.Value = EnumCache.Instance.Display(mActionStateID); } }

        private nxProperty    mDescription;      public nxProperty Description { get { return mDescription; }        set { mDescription = value; } }
        private nxProperty    mActionCode;       public nxProperty ActionCode  { get { return mActionCode; }         set { mActionCode = value; } }

        private string        mParentID;         public string   Parent        { get { return mParentID; }           set { mParentID = value;  } }



        public dlEventAction()
        {
            mActionType  = new nxProperty("Action", "ActionType");
            mActionState = new nxProperty("Action", "ActionState");
            mDescription = new nxProperty("Action", "Description");
            mActionCode  = new nxProperty("Action", "ActionCode");
            Reset();
        }
        public void Dispose() { }

     
        /// <summary>
        /// Load values from database.
        /// </summary>
        /// <param name="targetId">The Primary Key of the row to load values from.</param>
        /// <returns>True if successful, else false.</returns>
        public bool Open(string ActionPK)
        {
            mActionPK = ActionPK;
            DataRow dr=nexus.common.cache.ActionCache.Instance.LookUpRow("ActionPk",mActionPK);
            if (dr != null)
            {
                loadFromRow(dr);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Populates the member variables with values from a database row from a given DataRow.
        /// </summary>
        /// <param name="Row">A row from the database.</param>
        private void loadFromRow(DataRow Row)
        {
            mNewEntry = true;

            if (Row != null) 
            { 
                mActionTypeID      = Row["ActionTypeID"].ToString();
                mActionStateID     = Row["ActionStateID"].ToString();
                ActionType         = Row["ActionType"].ToString();
                ActionState        = Row["ActionState"].ToString();
                mActionCode.Value  = Row["ActionCode"].ToString();
                mDescription.Value = Row["Description"].ToString();
                mParentID          = Row["ParentID"].ToString();
                mNewEntry          = false;
            }
        }

        /// <summary>
        /// If mNewEntry is true, will attempt to insert a new row into the database, otherwise will attempt to update an existing row using the mActionPK.
        /// </summary>
        /// <returns>Will return true if successful, else false.</returns>
        public bool Update()
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);

            string Params = "";

            if (!mNewEntry)
                Params = "I~S~ActionPK~"  + mActionPK + ";";

            Params += "I~S~ActionType~"  + mActionTypeID + ";";
            Params += "I~S~ActionState~" + mActionStateID + ";";
            Params += "I~S~ActionCode~"  + mActionCode.Value + ";";
            Params += "I~S~Description~" + mDescription.Value + ";";
            Params += "I~S~ParentID~"    + mParentID;

            mActionPK = DB.ExecLookup("cmn.mod_Occurrence_Action", Params).ToString();

            mDataError = DB.DataError;
            if (String.IsNullOrEmpty(mDataError))
            {
                mActionType.Update();
                mActionState.Update();
                mActionCode.Update();
                mDescription.Update();
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "EventAction", "UpdateError"), mDataError); }

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }


        /// <summary>
        /// Set all member variables to default values.
        /// </summary>
        public void Reset() { Reset(null); }
        public void Reset(string ActionType)
        {
            if (String.IsNullOrEmpty(ActionType)) { ActionTypeID = "0"; ActionStateID = "0"; }
            else
            {
                ActionTypeID = EnumCache.Instance.getTypeId("Action", ActionType);
                ActionStateID = EnumCache.Instance.getStateId("Action", "Active");
            }

            mActionPK          = "0";
            mActionCode.Value  = "";
            mDescription.Value = "";
            mParentID          = "NULL";
            mDataError         = "";

            mNewEntry = true;
        }

        /// <summary>
        /// Sets the EventAction state to 'Deleted'. EventAction will still remain in the database.
        /// </summary>
        /// <returns>True if the state change was successful, else false.</returns>
        public bool Delete()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);

            string Params = "I~S~ActionPK~" + mActionPK + ";";
            Params += "I~S~ActionState~" + EnumCache.Instance.getTypeId("Entity", "Deleted");

            mActionPK = DB.ExecLookup("cmn.mod_Occurrence_Action", Params).ToString();

            mDataError = DB.DataError;
            if (String.IsNullOrEmpty(mDataError)) { mActionState.Update(); }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "EventAction", "UpdateError"), mDataError); }

            DB.Dispose();
            DB = null;

            nexus.common.cache.ActionCache.Instance.Create(); // reload the cache

            return String.IsNullOrEmpty(mDataError);
        }

        public override string ToString()
        {
            return mDescription.Value ;
        }

        public string strId(string ActionCode) {return getId(ActionCode).ToString();}
        public int    getId(string ActionCode)
        {
            int ValueId = 0;


            return ValueId;
        }

        public string strIdFromDesc(string ActionDescription) {return getIdFromDesc(ActionDescription).ToString();}
        public int    getIdFromDesc(string ActionDescription)
        {

            int ValueId = 0;


            return ValueId;
        }

        public string getDescFromId(string ActionPK) 
        {
            string Desc = "";


            return Desc;

        }

        public string getList(string ActionType)
        {

            string List = "";


            return List;
        }

        public DataTable getTable(string ActionType)
        {
            DataTable Table = new DataTable();

            return Table;
        }

        public bool SyncWithWAN()
        {
            bool ret=true;



            return ret;
        }


        ///// <summary>
        ///// Inserted the required SQL string to create a new row in the database.
        ///// </summary>
        ///// <returns>A valid insert SQL string.</returns>
        //public string getInsertSQL()
        //{
        //    string SQL = getColumns();

        //    SQL += "declare @EventActionId int; ";
        //    SQL += "INSERT INTO [cmn].[Event_Action] ([ActionGK],[ActionType],[ActionState],[ActionCode],[Description],[ParentID],[ExtnType],[Code],[Severity],[Pager],[Route],[Logging],[Modified],[Inserted]) ";
        //    SQL += "VALUES (newId(),@ActionType,@ActionState,@ActionCode,@Description,@ParentID,@ExtnType,@Code,@Severity,@Pager,@Route,@Logging,getdate(),getdate()) ";
        //    SQL += "select @EventActionId = scope_identity() select @EventActionId ";
        //    return SQL;
        //}

        ///// <summary>
        ///// Creates the required SQL string to update a row in the database.
        ///// </summary>
        ///// <returns>A valid update SQL string.</returns>
        //public string getUpdateSQL()
        //{
        //    string SQL = getColumns();
        //    SQL += "UPDATE [cmn].[Event_Action] set ActionType=@ActionType,ActionState=@ActionState,[ActionCode]=@ActionCode,[Description]=@Description,[ParentID]=@ParentID,[ExtnType]=@ExtnType,[Code]=@Code,[Severity]=@Severity,[Pager]=@Pager,[Route]=@Route,[Logging]=@Logging,[Modified]=getdate() ";
        //    SQL += "WHERE ActionPK=" + mActionPK;
        //    return SQL;
        //}

        //// Modified 20.2.2015
        ///// <summary>
        ///// Define the member variables as database variables and assign values from members.
        ///// </summary>
        ///// <returns>The SQL string as defined above.</returns>
        //private string getColumns()
        //{
        //    string SQL = "DECLARE @ActionType int; SET @ActionType=" + mActionTypeID + " ";
        //    SQL += "DECLARE @ActionState int; SET @ActionState=" + mActionStateID + " ";
        //    SQL += "DECLARE @ActionCode varchar(32); SET @ActionCode='" + mActionCode + "' ";
        //    SQL += "DECLARE @Description varchar(32); SET @Description='" + mDescription + "' ";
        //    // Check for top teir EventActions which will have empty mParent variable.
        //    SQL += "DECLARE @ParentID int; SET @ParentID=" + (String.IsNullOrEmpty(mParentID) ? " NULL " : mParentID) + " ";
        //    SQL += "DECLARE @ExtnType int; SET @ExtnType=" + (String.IsNullOrEmpty(mExtnType) ? " NULL " : mExtnType) + " ";
        //    SQL += "DECLARE @Code smallint; SET @Code=" + mCode + " ";
        //    SQL += "DECLARE @Severity smallint; SET @Severity=" + mSeverity + " ";
        //    SQL += "DECLARE @Pager varchar(2); SET @Pager='" + mPager + "' ";
        //    SQL += "DECLARE @Route bit; SET @Route=" + mRoute + " ";
        //    SQL += "DECLARE @Logging bit; SET @Logging=" + mLogging + " ";
        //    return SQL;
        //}


    }
}
