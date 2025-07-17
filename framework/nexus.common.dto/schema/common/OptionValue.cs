using nexus.common.cache;
using nexus.common.core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace nexus.common.dal
{
    public class dlOptionValue
    {
        // Private members.             // Public accessors and mutators.
        private string mOptionID;       public string OptionID      { get { return mOptionID; }     set { mOptionID = value; } }
        private string mValuePK;        public string ValuePK       { get { return mValuePK; }      set { mValuePK = value; } }

        private string mComputerID;       public string ComputerID      { get { return mComputerID; }     set { mComputerID = value; } }
        private string mComputerRoleID;   public string ComputerRoleID  { get { return mComputerRoleID; } set { mComputerRoleID = value; } }
        private string mWorkerID;       public string WorkerID      { get { return mWorkerID; }     set { mOptionID = value; } }
        private string mWorkerRollID;   public string WorkerRollID  { get { return mWorkerRollID; } set { mWorkerRollID = value; } }

        private string mValueText;      public string ValueText     { get { return mValueText; }    set { mValueText = value; } }
        private DateTime mModified;     public DateTime Modified    { get { return mModified; }     set { mModified = value; } }
        private DateTime mInserted;     public DateTime Inserted    { get { return mInserted; }     set { mInserted = value; } }

        private string mDataError;      public string   DataError   { get { return mDataError; }    set { mDataError = value; } }

        private bool mNewEntry;

        /// <summary>
        /// Default constructor - sets all member variables to default values.
        /// </summary>
        public dlOptionValue() {
            Reset();
        }

        /// <summary>
        /// Sets all member variables to default values.
        /// </summary>
        public void Reset()
        {
            mOptionID       = "0";
            mValuePK        = "0";
            mComputerID       = "0";
            mComputerRoleID   = "0";
            mWorkerID       = "0";
            mWorkerRollID   = "0";
            mValueText      = "";
            mModified        = new DateTime();
            mInserted        = new DateTime();
            mNewEntry       = true;
        }

        public bool Open(int targetId) {  return Open(targetId.ToString()); }
        public bool Open(string targetId)
        {
            string SQL = "SELECT [OptionID], [ValuePK], [ComputerID], [ComputerRoleID], [WorkerID], [WorkerRollID], [ValueText], [Modified], [Inserted] FROM [cmn].[OptionValue] WHERE ValuePK=" + targetId;
            SQLServer DB = new SQLServer (setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);

            // Check if datatable has been Inserted.
            if (DT.Rows.Count > 0)
            {
                this.mNewEntry = false;
                LoadFromRow(DT.Rows[0]);
                DB.Dispose();
                DB = null;
            }
            else
            {
                DB.Dispose();
                DB = null;
                return false; // Unsuccessful.
            }

            return true; // Successful.
        }

        /// <summary>
        /// Populates the instance of this class with data from the SQL query performed by Open(string) method.
        /// </summary>
        /// <param name="Row">The row returned from the SQL query as above.</param>
        private void LoadFromRow(DataRow Row)
        {
            if (Row != null) { 
                mOptionID       = Row["OptionID"].ToString();
                mValuePK        = Row["ValuePK"].ToString();
                mComputerID       = Row["ComputerID"].ToString();
                mComputerRoleID   = Row["ComputerRoleID"].ToString();
                mWorkerID       = Row["WorkerID"].ToString();
                mWorkerRollID   = Row["WorkerRollID"].ToString();
                mValueText      = Row["ValueText"].ToString();
                mModified        = (DateTime)Row["Modified"];
                mInserted        = (DateTime)Row["Inserted"];
            }
        }

        /// <summary>
        /// If the OptionValue already exists, the method will Modified the existing row.
        /// Else will create a new row in the database.
        /// </summary>
        /// <returns>1 if update was a success. Else 0. Currently only works for updates.</returns>
        public bool Update()
        {
            bool flag = false;

            SQLServer DB = new SQLServer(setting.ConnectionString);

            if (this.mNewEntry)
            {
                DataError = DB.ExecLookup(getInsertSQL());
                if(DataError.Equals(""))
                    flag = true;
                else
                    flag = false;
            }
            else
            {
                if(DB.ExecNonQuery(getModifiedSQL()) == 1)
                    flag = true;
                else
                    flag = false;
            }

            DB.Dispose();
            DB = null;

            return flag;
        }


        // There is currently no state table in the OptionValue table therefore cannot set state to "Deleted".
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            //string SQL = "declare @OptionID int; Set @OptionID=" + mOptionID + " ";
            //SQL += "declare @OptionState int; Set @OptionState=" + EnumCache.Instance.getTypeId("Entity", "Deleted") + " ";
            //SQL += "update [cmn].[Option] set OptionState=@OptionState,[Modified]=getdate() where OptionPK=@OptionID";

            //SQLServer DB = new SQLServer(setting.ConnectionString);
            //DB.ExecNonQuery(SQL);
            //mDataError = DB.DataError;
            //DB.Dispose();
            //DB = null;

            //nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            //return String.IsNullOrEmpty(mDataError);

            return false;
        }

        /// <summary>
        /// Gets and defines the SQL for variables to be entered into the database fields matching class members.
        /// </summary>
        /// <returns>SQL String with declarations matching class members.</returns>
        public string getColumns()
        {
            string SQL = "DECLARE @OptionID int; SET @OptionID=" + mOptionID + " ";
            SQL += "DECLARE @ComputerID int; SET @ComputerID=" + mComputerID + " ";
            SQL += "DECLARE @ComputerRoleID int; SET @ComputerRoleID=" + mComputerRoleID + " ";
            SQL += "DECLARE @WorkerID int; SET @WorkerID=" + mWorkerID + " ";
            SQL += "DECLARE @WorkerRollID int; SET @WorkerRollID=" + mWorkerRollID + " ";
            SQL += "DECLARE @valueText varchar(256); SET @valueText='" + mValueText + "' ";
            SQL += "DECLARE @Modified datetime; SET @OptionID=" + mOptionID + " ";

            return SQL;
        }

        /// <summary>
        /// Called from Update()
        /// Will return an SQL string to update the values of a current OptionValue.
        /// </summary>
        /// <returns>An SQL string which updates the values of a current OptionValue.</returns>
        public string getModifiedSQL()
        {
            string SQL = "DECLARE @ValuePK int; SET @ValuePK=" + mValuePK + " " + getColumns();
            SQL += "UPDATE [cmn].[OptionValue] SET [OptionID]=@OptionID,[ComputerID]=@ComputerID,[ComputerRoleID]=@ComputerRoleID,[WorkerID]=@WorkerID,[WorkerRollID]=@WorkerRollID,[ValueText]=@valueText,[Modified]=getDate() ";
            SQL += "WHERE ValuePK=@ValuePK";
            return SQL;
        }

        /// <summary>
        /// Get an SQL insert string to insert a new OptionValue entry into the database.
        /// </summary>
        /// <returns>An SQL insert string which inserts a new OptionValue into the database.</returns>
        public string getInsertSQL()
        {
            string SQL = getColumns();
            SQL += "INSERT INTO [cmn].[OptionValue] () VALUES (OptionID, ComputerID, ComputerRoleID, WorkerID, WorkerRollID, ValueText, Modified, Inserted";
            SQL += "@OptionID, @ComputerID, @ComputerRoleID, @WorkerID, @WorkerRollID, @valueText, getDate(), getDate())";
            return SQL;
        }

        /// <summary>
        /// Creates a Datatable of all Options available.
        /// </summary>
        /// <returns>A Datatable with the fields [OptionPK] and [Defn]. Null if none exists.</returns>
        public static DataTable loadOptionList() {
            //string SQL = "SELECT OptionParent.OptionPK, OptionParent.OptionCode, OptionParent.Description as OptionDescription,  OptionParent.Parent, OptionChild.OptionPK as ChildPk, OptionChild.OptionCode as ChildDefn, OptionChild.Description as ChildDescription, OptionChild.Parent as ChildParent FROM [cmn].[Option] AS OptionParent LEFT JOIN [cmn].[Option] AS OptionChild ON OptionParent.OptionPK=OptionChild.Parent";
            //string SQL = "SELECT CONVERT(VARCHAR(16),OptionPK) as OptionPK, OptionCode, [Description], CONVERT(VARCHAR(16),Parent) as ParentID FROM cmn.[Option]";
            string SQL = "SELECT ValuePK, ValueText FROM cmn.[OptionValue]";
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);
            DB.Dispose();
            DB = null;

            return DT;
        }

    }
}
