
// C# libraries.
using System;
using System.Data;
using System.Linq;
using System.Text;

// Nexus libraries.
using nexus.common.dal;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{
    public class dlEnumType
    {
        // Private members                // Public mutators and Operabilityors.
        private string  mEnumPK = "0";    public string   EnumPK       { get { return mEnumPK; } }

        private string  mEnumType;        public string   Type         { get { return mEnumType; }     set { mEnumType = value;   mEnumTypeUI    = EnumCache.Instance.getTypeFromDesc  ("Enum",   mEnumType); } }
        private string  mEnumTypeUI;      public string   TypeId       { get { return mEnumTypeUI; }   set { mEnumTypeUI = value; mEnumType      = EnumCache.Instance.getDescFromId    (          mEnumTypeUI); } }

        private string  mEnumState;       public string   State        { get { return mEnumState; }    set { mEnumState = value;   mEnumStateUI  = EnumCache.Instance.getStateFromDesc ("Enum",   mEnumState); } }
        private string  mEnumStateUI;     public string   StateId      { get { return mEnumStateUI; }  set { mEnumStateUI = value; mEnumState    = EnumCache.Instance.getDescFromId    (          mEnumStateUI); } }

        private string  mEnumCode;        public string   Defn         { get { return mEnumCode; }     set { mEnumCode = value; } }
        private string  mDescription;     public string   Description  { get { return mDescription; }  set { mDescription = value; } }
        private string  mOperability;     public string   Operability  { get { return mOperability; }  set { mOperability = value;  } }
        private string  mVisibility;      public string   Visibility   { get { return mVisibility; }   set { mVisibility = value;  } }
        private string  mParentID;        public string   Parent       { get { return mParentID; }     set { mParentID = value;  } }

        private string  mValueType;       public string   ValueType    { get { return mValueType; }    set { mValueType = value;   mValueTypeUI  = EnumCache.Instance.getTypeFromDesc ("ValueType", mValueType); } }
        private string  mValueTypeUI;     public string   ValueTypeUI  { get { return mValueTypeUI; }  set { mValueTypeUI = value; mValueType    = EnumCache.Instance.getDescFromId   (mValueTypeUI); } }

        private string  mDataError;       public string   DataError    { get { return mDataError; }    set { mDataError = value; } }

        private bool    mNewEntry = true; // Flag to determine if an Update() method call requires an SQL insert.

        /// <summary>
        /// Constructor.
        /// </summary>
        public dlEnumType() { }


        /// <summary>
        /// Create an SQL query for a row of the enum table and populate a DataTable if found.
        /// </summary>
        /// <param name="EnumPK">Primary Key (Unique) for SQL query.</param>
        /// <returns>True if the class has been populated from an SQL query. False if no row with matching key found.</returns>
        ///         
        public bool Open(int EnumPK)    { return Open(EnumPK.ToString()); }
        public bool Open(string EnumPK)
        {
            mNewEntry = true;

            string SQL = "select EnumPK, Enum1.Description AS EnumType, Enum2.Description AS EnumState,[EnumCode],a.Description,Operability, Visibility,a.[ParentID],a.[ValueType],EnumType as EnumTypeUI, EnumState as EnumStateUI from cmn.EnumType as a left join cmn.EnumValue as Enum1 ON a.EnumType = Enum1.ValuePK left join cmn.EnumValue AS Enum2 ON a.Enumstate = Enum2.ValuePK WHERE EnumPK=" + EnumPK + " order by EnumType, EnumCode";
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);

            if (DT.Rows.Count > 0)
            {
                mNewEntry = false;        // flag to determine whether this instance is a new entry or not.
                loadFromRow(DT.Rows[0]);
            }

            DB.Dispose();
            DB = null;

            return !mNewEntry;

        }

        /// <summary>
        /// Populates the instance of this class with data from the SQL query performed by Open(string) method.
        /// </summary>
        /// <param name="Row">The row returned from the SQL query as above.</param>
        // Tested - OK.
        private void loadFromRow(DataRow Row)
        {
            if (Row != null) { 
                mEnumPK        = Row["EnumPK"].ToString();
                mEnumType      = Row["EnumType"].ToString();
                mEnumState     = Row["EnumState"].ToString();
                mEnumCode      = Row["EnumCode"].ToString();
                mDescription   = Row["Description"].ToString();
                mEnumTypeUI    = Row["EnumTypeId"].ToString();
                mEnumStateUI   = Row["EnumStateId"].ToString();
                mValueTypeUI   = Row["ValueType"].ToString();
                mParentID      = Row["ParentID"].ToString();

                Operability    = Row["Operability"].ToString();
                Visibility     = Row["Visibility"].ToString();

            }

        }

        /// <summary>
        /// If the EnumType exists, the method will update the existing type. 
        /// Else will create a new type.
        /// </summary>
        /// <returns>1 if update was a success. Else 0. Currently only works for updates.</returns>
        public bool Update()
        {
            bool ret = false;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                if (mNewEntry)
                {
                    mEnumPK = DB.ExecLookup(getInsertSQL());
                    ret = !String.IsNullOrEmpty(mEnumPK);
                }
                else
                {
                    ret = DB.ExecNonQuery(getUpdateSQL()).Equals(1);
                }

                mDataError = DB.DataError;
            }

            mNewEntry = false;

            return ret;
        }


        /// <summary>
        /// SQL insert string to insert a new EnumType entry into the database.
        /// </summary>
        /// <returns>An SQL insert string.</returns>
        public string getInsertSQL()
        {
            string SQL = "declare @EnumPK int ";
            SQL += getColumns(); 
            SQL += "insert into [cmn].[EnumType] ([EnumGK],[EnumType],[EnumState],[EnumCode],[Description],[Operability],[Visibility],[ParentID],[ValueType],[Updated],[Created]) ";
            SQL += "values (newId(),@EnumType,@EnumState,@EnumCode,@Description,@Operability,@Visibility,@ParentID,@ValueType,getdate(),getdate()) ";
            SQL += "select @EnumPK = Scope_identity() ";
            SQL += "select @EnumPK";
            return SQL;
        }

        /// <summary>
        /// Called from Update()
        /// Will return an SQL string update the values of a current EnumType.
        /// </summary>
        /// <returns>An SQL string which updates the values of a current EnumType.</returns>
        public string getUpdateSQL()
        {
            string SQL = "declare @EnumPK int; Set @EnumPK=" + mEnumPK + " " + getColumns();
            SQL += "update [cmn].[EnumType] set EnumType=@EnumType,EnumState=@EnumState,[EnumCode]=@EnumCode,[Description]=@Description,[Operability]=@Operability,[Visibility]=@Visibility,[ParentID]=@ParentID,[ValueType]=@ValueType,[Updated]=getdate() ";
            SQL += "where EnumPK=@EnumPK";
            return SQL;
        }

        /// <summary>
        /// Will return an SQL string with variables which match class members to corrsponding database fields.
        /// </summary>
        /// <returns>An SQL string with the variables corrosponding to the database fields.</returns>
        private string getColumns()
        {
            string SQL = "declare @EnumCode varchar(32); Set @EnumCode='" + mEnumCode + "' ";
            SQL += "declare @EnumType int; Set @EnumType=" + mEnumTypeUI + " ";
            SQL += "declare @EnumState int; Set @EnumState=" + mEnumStateUI + " ";
            SQL += "declare @Description varchar(256); Set @Description='" + mDescription + "' ";
            SQL += "declare @Operability int; Set @Operability=" + mOperability + " ";
            SQL += "declare @Visibility int; Set @Visibility=" + mVisibility + " ";
            SQL += "declare @ParentID int; Set @ParentID=" + (String.IsNullOrEmpty(mParentID) ? "NULL" : mParentID) + " ";
            SQL += "declare @ValueType int; Set @ValueType=" +  mValueTypeUI + " ";
            return SQL;
        }


        public bool Delete() 
        {
            string SQL = "declare @EnumPK int; Set @EnumPK=" + mEnumPK + " ";
            SQL += "declare @EnumState int; Set @EnumState=" + EnumCache.Instance.getTypeId("Entity", "Deleted") + " ";
            SQL += "update [cmn].[EnumType] set EnumState=@EnumState,[Updated]=getdate() where EnumPK=@EnumPK";

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
        public void Reset()
        {
            StateId         = EnumCache.Instance.getStateId("Enum", "Active");

            mEnumPK         = "";
            mEnumCode       = "";
            mDescription    = "";

            mOperability    = "";
            mVisibility     = "";
            mParentID       = "";
            mValueType      = "";

            mNewEntry       = true;
        }

        public override string ToString() { return mDescription; }

    }
}