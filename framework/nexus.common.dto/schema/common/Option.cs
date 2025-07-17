using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dal;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlOption
    {
        // Private members.                     // Public accessors and mutators.
        private string      mOptionPK;          public string OptionPK          { get { return mOptionPK; }         set { mOptionPK = value;} }

        private string      mOptionType;        public string OptionType        { get { return mOptionType; }       set { mOptionType = value;   mOptionTypeID    = EnumCache.Instance.getTypeFromDesc  ("Option",   mOptionType); } }
        private string      mOptionTypeID;      public string OptionTypeID      { get { return mOptionTypeID; }     set { mOptionTypeID = value; mOptionType      = EnumCache.Instance.getDescFromId    (            mOptionTypeID); } }

        private string      mOptionState;       public string OptionState       { get { return mOptionState; }      set { mOptionState = value;   mOptionStateID  = EnumCache.Instance.getStateFromDesc ("Entity",   mOptionState); } }
        private string      mOptionStateID;     public string OptionStateID     { get { return mOptionStateID; }    set { mOptionStateID = value; mOptionState    = EnumCache.Instance.getDescFromId    (            mOptionStateID); } }

        private string      mOptionCode;        public string OptionCode        { get { return mOptionCode; }       set { mOptionCode = value;} }
        private string      mDescription;       public string Description       { get { return mDescription; }      set { mDescription= value;} }
        private string      mParentID;          public string ParentID          { get { return mParentID; }         set { mParentID = value;} }
        private string      mSequence;          public string Sequence          { get { return mSequence; }         set { mSequence = value;} }

        private string      mValueType;         public string ValueType         { get { return mValueType; }        set { mValueType = value;} }
        private string      mVarDefault;        public string VarDefault        { get { return mVarDefault; }       set { mVarDefault = value;} }

        private string      mComputer;          public string Computer          { get { return mComputer; }         set { mComputer = value;} }
        private string      mComputerRole;      public string ComputerRole      { get { return mComputerRole; }     set { mComputerRole = value;} }
        private string      mWorker;            public string Worker            { get { return mWorker; }           set { mWorker = value;} }
        private string      mWorkerRole;        public string WorkerRole        { get { return mWorkerRole; }       set { mWorkerRole = value;} }
        private string      mServerID;          public string ServerID          { get { return mServerID; }         set { mServerID = value;} }

        private string      mDataError;         public string   DataError       { get { return mDataError; }        set { mDataError = value; } }

        private DateTime    mModified;          public DateTime Modified        { get { return mModified; }         set { mModified = value;} }
        private DateTime    mInserted;          public DateTime Inserted        { get { return mInserted; }         set { mInserted = value;} }

        public bool NewEntry;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public dlOption() { Reset(); }
        public void Dispose() { }


        public bool Open(string OptionId)
        {
            bool ret = false;
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("cmn.lst_Option", "I~S~OptionPK~" + OptionId);
                if (DT.Rows.Count > 0) ret = loadFromRow(DT.Rows[0]);
            }
            return ret;
        }

        public bool Update()
        {
            string Params = "";
            if (!NewEntry) Params = "I~S~OptionPK~" + mOptionPK + ";";

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                mOptionPK = DB.ExecLookup("cmn.mod_Option", Params + getParams()).ToString();
                mDataError = DB.DataError;
            }

            if (String.IsNullOrEmpty(mDataError))
                Open(mOptionPK);
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Option", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);
        }


        public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mOptionType))     Params += "I~S~OptionType~"     + mOptionType    + ";";
            if (!String.IsNullOrEmpty(mOptionState))    Params += "I~S~OptionState~"    + mOptionState   + ";";
            if (!String.IsNullOrEmpty(mOptionCode))     Params += "I~S~OptionCode~"     + mOptionCode    + ";";
            if (!String.IsNullOrEmpty(mDescription))    Params += "I~S~Description~"    + mDescription   + ";";
            if (!String.IsNullOrEmpty(mParentID))       Params += "I~S~ParentID~"       + mParentID      + ";";
            if (!String.IsNullOrEmpty(mValueType))      Params += "I~S~ValueType~"      + mValueType     + ";";
            if (!String.IsNullOrEmpty(mVarDefault))     Params += "I~S~VarDefault~"     + mVarDefault    + ";";
            if (!String.IsNullOrEmpty(mSequence))       Params += "I~S~Sequence~"       + mSequence      + ";";
            if (!String.IsNullOrEmpty(mComputer))       Params += "I~S~Computer~"       + mComputer      + ";";
            if (!String.IsNullOrEmpty(mComputerRole))   Params += "I~S~@ComputerRole~"  + mComputerRole  + ";";
            if (!String.IsNullOrEmpty(mWorker))         Params += "I~S~Worker~"         + mWorker        + ";";
            if (!String.IsNullOrEmpty(mWorkerRole))     Params += "I~S~@WorkerRole~"    + mWorkerRole;
            return Params;
        }


        private bool loadFromRow(DataRow Row)
        {
            NewEntry = false;
            if (Row != null) 
            { 
                mOptionPK     = Row["OptionPK"].ToString();
                mOptionType   = Row["OptionType"].ToString();
                mOptionState  = Row["OptionState"].ToString();
                mOptionCode   = Row["OptionCode"].ToString();
                mDescription  = Row["Description"].ToString();
                mParentID     = Row["ParentID"].ToString();
                mSequence     = Row["Sequence"].ToString();
                mValueType    = Row["ValueType"].ToString();
                mVarDefault   = Row["VarDefault"].ToString();
                mComputer     = Row["Computer"].ToString();
                mComputerRole = Row["ComputerRole"].ToString();
                mWorker       = Row["Worker"].ToString();
                mWorkerRole   = Row["WorkerRole"].ToString();
                mServerID     = Row["ServerID"].ToString();
                mModified     = (DateTime)Row["Modified"];
                mInserted     = (DateTime)Row["Inserted"];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the state of the current row as defined by OptionPK to the EnumCache EntityState "Deleted".
        /// </summary>
        /// <returns>True if successful, else false.</returns>
        public bool Delete() 
        {

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params = "";
                if (!NewEntry) Params = "I~S~OptionPK~" + mOptionPK + ";I~S~OptionState~" + EnumCache.Instance.getTypeId("Entity", "Deleted");
                DB.ExecLookup("cmn.mod_Option", Params).ToString();
            }

            nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            return String.IsNullOrEmpty(mDataError);
        }

        public void Reset()
        {
            mOptionPK         = "0";
            mOptionType       = "0";
            mOptionState      = "0";
            mOptionCode       = "";
            mDescription      = "";
            mParentID         = "NULL";
            mSequence         = "0";

            mValueType        = "0";
            mVarDefault       = "";
            mComputer         = "0";      // Bit-type; 1 = true, 0 = false
            mComputerRole     = "0";      // Bit-type; 1 = true, 0 = false
            mWorker           = "0";      // Bit-type; 1 = true, 0 = false
            mWorkerRole       = "0";      // Bit-type; 1 = true, 0 = false
            mServerID         = "0";
            mModified          = new DateTime();
            mInserted          = new DateTime();

            NewEntry = true;
        }


    }
}
