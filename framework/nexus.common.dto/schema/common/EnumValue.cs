using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dal;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlEnumValue
    {

        // Private members.               // Public mutators and accessors.
        private string  mEnumID = "0";    public string   EnumID       { get { return mEnumID; }       set { mEnumID = value; } }
        private string  mValuePK;         public string   ValuePK      { get { return mValuePK; }      set { mValuePK = value; } }

        private string  mValueState;      public string   ValueState   { get { return mValueState; }   set { mValueState = value;     mValueStateId  = EnumCache.Instance.getTypeFromDesc ("Entity", mValueState); } }
        private string  mValueStateId;    public string   ValueStateId { get { return mValueStateId; } set { mValueStateId = value;   mValueState    = EnumCache.Instance.getDescFromId   (          mValueStateId); } }
        private string  mValueCode;       public string   ValueCode    { get { return mValueCode; }    set { mValueCode = value; } }

        private string  mDescription;     public string   Description  { get { return mDescription; }  set { mDescription = value; } }
        private string  mSequence;        public string   Sequence     { get { return mSequence; }     set { mSequence = value; } }
        private string  mDenoteId;        public string   DenoteId     { get { return mDenoteId; }     set { mDenoteId = value; } }
        private string  mParentID;        public string   Parent       { get { return mParentID; }     set { mParentID = value;  } }
        private string  mValue;           public string   Value        { get { return mValue; }        set { mValue = value;  } }

        private string  mDataError;       public string   DataError    { get { return mDataError; }    set { mDataError = value; } }

        private bool    mNewEntry = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        public dlEnumValue()    
        {
            mDenoteId = "0";
        }

        /// <summary>
        /// Loads property from database by ValuePK into the current instance.
        /// </summary>
        /// <param name="ValuePK">The primary key for a row in the EnumValue table.</param>
        /// <returns>true if successful load. false otherwise.</returns>
        public bool Open(string Operability, int ValuePK) { return Open(Operability, ValuePK.ToString()); }
        public bool Open(string Operability, string ValuePK)
        {
            mNewEntry = true;

            mValuePK = ValuePK;

            string SQL = "select a.[EnumID], e.Description AS ValueState,a.[ValueCode],a.[Description],a.[Sequence],a.[DenoteID],a.[ParentID],a.[Value],a.ValueState as ValueStateId ";
            if (Operability.Equals(EnumCache.Instance.getTypeId("Operability", "System")))
                SQL += "from cmn.EnumValue as a left join enums.EnumValue as e ON a.ValueState = e.ValuePK where a.ValuePK=" + mValuePK + " order by a.sequence";
            else
                SQL += "from cmn.EnumUser as a left join enums.EnumValue as e ON a.ValueState = e.ValuePK where a.ValuePK=" + mValuePK + " order by a.sequence";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);
            if (DT.Rows.Count > 0)
            {
                mNewEntry = false;
                LoadFromRow(DT.Rows[0]);
            }

            DB.Dispose();
            DB = null;

            return !mNewEntry;
        }

        /// <summary>
        /// Populates members with data from a DataRow.
        /// </summary>
        /// <param name="Row">A DataRow from EnumValue table.</param>
        private void LoadFromRow(DataRow Row)
        {
            if (Row != null) { 
                mEnumID       = Row["EnumID"].ToString();
                mValueStateId = Row["ValueStateId"].ToString();

                mValue        = Row["Value"].ToString();
                mValueState   = Row["ValueState"].ToString();
                mValueCode    = Row["ValueCode"].ToString();
                mDescription  = Row["Description"].ToString();

                mSequence     = Row["Sequence"].ToString();
                mDenoteId     = Row["DenoteID"].ToString();
                mParentID     = Row["ParentID"].ToString();
            }
        }

        /// <summary>
        /// Will attempt to update row by value of mEnumID, else will attempt to create new row.
        /// </summary>
        /// <returns>1 if successful, else 0. Note: Only updates will return a correct return code.</returns>
        public bool Update(string Operability)
        {
            bool flag = false;

            SQLServer DB = new SQLServer(setting.ConnectionString);

            if (mNewEntry)
            {
                mValuePK = DB.ExecLookup(getInsertSQL(Operability));
                flag = !String.IsNullOrEmpty(mEnumID);
            }
            else
            {
                // Returns 1 if successful, else 0.
                flag = DB.ExecNonQuery(getUpdateSQL(Operability)).Equals(1);
            }

            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            if (flag) nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            mNewEntry = false;

            return flag;
        }

        /// <summary>
        /// SQL insert string to insert a new EnumValue entry into the database.
        /// </summary>
        /// <returns>An SQL insert string.</returns>
        public string getInsertSQL(string Operability)
        {
            string SQL = "declare @ValuePK int ";
            SQL += "declare @EnumID int; Set @EnumID=" + mEnumID + " " + getColumns();
            if (Operability.Equals(nexus.common.cache.EnumCache.Instance.getTypeId("Operability", "System")))
                SQL += "insert into [cmn].[EnumValue] ([ValueGK],[EnumID],[ValueState],[ValueCode],[Description],[Sequence],[DenoteID],[ParentID],[Value],[Updated],[Created]) ";
            else
                SQL += "insert into [cmn].[EnumUser] ([ValueGK],[EnumID],[ValueState],[ValueCode],[Description],[Sequence],[DenoteID],[ParentID],[Value],[Updated],[Created]) ";
            SQL += "values (newId(),@EnumID,@ValueState,@ValueCode,@Description,@Sequence,@DenoteID,@ParentID,@Value,getdate(),getdate()) ";
            SQL += "select @ValuePK = scope_identity() ";
            SQL += "select @ValuePK";
            return SQL;
        }

        /// <summary>
        /// Called from Update()
        /// Will return an SQL string update the values of a current EnumValue.
        /// </summary>
        /// <returns>An SQL string which updates the values of a current EnumValue.</returns>
        public string getUpdateSQL(string Operability)
        {
            string SQL = "declare @ValuePK int; Set @ValuePK=" + mValuePK + " " + getColumns();
            if (Operability.Equals(nexus.common.cache.EnumCache.Instance.getTypeId("Operability", "System")))
                SQL += "update [cmn].[EnumValue] set [ValueState]=@ValueState,[ValueCode]=@ValueCode,[Description]=@Description,[Sequence]=@Sequence,[DenoteID]=@DenoteID,[Parent]=@ParentID,[Value]=@Value,[Updated]=getdate() ";
            else
                SQL += "update [cmn].[EnumUser] set [ValueState]=@ValueState,[ValueCode]=@ValueCode,[Description]=@Description,[Sequence]=@Sequence,[DenoteID]=@DenoteID,[ParentID]=@ParentID,[Value]=@Value,[Updated]=getdate() ";
            SQL += "where ValuePK=@ValuePK";
            return SQL;
        }

        
        /// <summary>
        /// Will return an SQL string with variables which match class members to corrsponding database fields.
        /// </summary>
        /// <returns>An SQL string with the variables corrosponding to the database fields.</returns>
        private string getColumns()
        {
            string SQL = "declare @ValueCode varchar(16); Set @ValueCode='" + mValueCode + "' ";
            SQL += "declare @ValueState tinyint; Set @ValueState=" + mValueStateId + " ";
            SQL += "declare @Description varchar(32); Set @Description='" + mDescription + "' ";
            SQL += "declare @Sequence smallint; Set @Sequence=" + mSequence + " ";
            SQL += "declare @DenoteID int; Set @DenoteID=" + mDenoteId + " ";
            SQL += "declare @ParentID int; Set @ParentID=" + mParentID + " ";
            SQL += "declare @Value varchar(32) Set @Value='" + mValue + "' ";
            return SQL;
        }


        /// <summary>
        /// Sets the current record to a state of deleted
        /// </summary>
        public bool Delete(string Operability)
        {
            string SQL = "declare @ValuePK int; Set @ValuePK=" + mValuePK + " ";
            SQL += "declare @ValueState int; Set @ValueState=" + EnumCache.Instance.getTypeId("Entity", "Deleted") + " ";
            SQL += "update [cmn].[EnumValue] set ValueState=@ValueState,[Updated]=getdate() where ValuePK=@ValuePK";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DB.ExecNonQuery(SQL);
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            return String.IsNullOrEmpty(mDataError);

        }

        /// <summary>
        /// Sets all private members to default values.
        /// </summary>
        /// <param name="EnumType"></param>        
        public void Reset(string EnumValue)
        {
            ValueStateId = EnumCache.Instance.getStateId("Entity", "Active");

            mValueCode   = "";
            mDescription = "";
            mSequence    = "";
            mDenoteId    = "";
            mParentID    = "";
            mValue       = "";
        }

        public override string ToString()   { return mDescription; }

    }
}
