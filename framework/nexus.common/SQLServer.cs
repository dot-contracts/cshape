using System;
using System.Data;
using System.Drawing;
using System.Data.Common;
using System.Data.SqlClient;

namespace nexus.common.dal
{
    public partial class SQLServer : IDisposable
    {
        private SqlConnection       mCon;
        private SqlTransaction      mTransaction;
        private DataTable           mDataTable;
        private SqlDataAdapter      mDataAdap;
        private SqlCommandBuilder   mCommBuild;

        //private string  mPort     = "";
        private string  mServer   = "";
        private string  mCatalog  = "";
        private string  mUserName = "";
        private string  mPassword = "";

        private string  mConnectionStringTest     = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Trusted_Connection=False;Connection Timeout=5";
        private string  mConnectionStringUsername = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Trusted_Connection=False;Connection Timeout=600;Encrypt=True;TrustServerCertificate=True";
        private string  mConnectionStringNoUser   = "Server={0};Database={1};Trusted_Connection=True";
        private string  mConnectionString         = "";
        private string  mConnectionStringTemplate = "";
        private string  mDataError;

        private int     mCommandTimeOut = 120;
        private bool    mConnOpened = false;

        #region Property
        public  string  ConnectionString    { get { return mConnectionString; }     set { mConnectionString = value; } }
        public  string  DataError           { get { return mDataError; } }
        public  int     CommandTimeOut      { get { return mCommandTimeOut; }       set { mCommandTimeOut = value; } }
        public  SqlConnection getConnection { get { return ( getConnect() ); } }
        public  SqlConnection Connection    { get { return mCon; } }
        public  SqlConnection Con           { get { return mCon; }                  set { mCon = value; } }

        public  string  Server      { get { return mServer; }       set { mServer   = Server;  mConnectionString = _ConnectionString(); } }  //?? Should this not be <value>
        public  string  Catalog     { get { return mCatalog; }      set { mCatalog  = value;   mConnectionString = _ConnectionString(); } }
        public  string  UserName    { get { return mUserName; }     set { mUserName = value;   mConnectionString = _ConnectionString(); } }
        public  string  Password    { get { return mPassword; }     set { mPassword = value;   mConnectionString = _ConnectionString(); } }
        #endregion

        #region Constructors
        public SQLServer()
        {
            //mConnectionString = (String.IsNullOrEmpty(shell.Instance.LANConnStr.ToString()) ? "" : shell.Instance.LANConnStr);
        }

        public SQLServer(string Server, string Catalog, string UserName, string Password)
        {
            mServer   = Server;
            mCatalog  = Catalog;
            mUserName = UserName;
            mPassword = Password;
            mConnectionStringTemplate = mConnectionStringUsername;
            mConnectionString         = _ConnectionString();
            Close();
        }

        public SQLServer(string Server, string Catalog)
        {
            mServer  = Server;
            mCatalog = Catalog;
            mConnectionStringTemplate = mConnectionStringNoUser;
            mConnectionString         = _ConnectionString();
            Close();
        }

        public SQLServer(string ConnectionString)
        {
            mConnectionString = ConnectionString;
            Close();
        }

        public SQLServer(string ConnectionString, int TimeOut)
        {
            mCommandTimeOut   = TimeOut;
            mConnectionString = ConnectionString;
            Close();
        }
        #endregion

        #region Public Methods
        private string _ConnectionString()
        {
            if (!string.IsNullOrEmpty(mUserName))
            {
                return string.Format(mConnectionStringUsername, mServer, mCatalog, mUserName, mPassword);
            }
            else
            {
                return string.Format(mConnectionStringNoUser, mServer, mCatalog);
            }
        }

        public bool TestConnect(string Server, string Catalog, string Username, string Password)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(string.Format(mConnectionStringTest, Server, Catalog, Username, Password)))
                {
                    con.Open();
                    con.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool Connect(string Catalog)
        {
            if (mCatalog != Catalog)  Close();

            mCatalog = Catalog;
            return Connect();
        }

        public SqlConnection getConnect()
        {
            if ( Connect() ) return ( mCon );

            return ( null );
        }

        public bool Connect()
        {
            mDataError = "";
            if (!mConnOpened)
                Close();
            if (mConnOpened)
            {
                if (!(mCon.State == ConnectionState.Open))
                    Close();
            }
            if (!mConnOpened & !string.IsNullOrEmpty(mConnectionString))
            {
                try
                {
                    mCon = new SqlConnection(mConnectionString);
                    mCon.Open();
                    mConnOpened = true;
                }
                catch (SqlException ex)
                {
                    mDataError = ex.Message;
                    mConnOpened = false;
                }
            }
            return mConnOpened;
        }

        public void BeginTransaction()
        {
            mTransaction = mCon.BeginTransaction();
        }

        public bool CommitTransaction()
        {
            try
            {
                if (mConnOpened)
                    mTransaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mDataError = ex.Message;
                //Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public void Close()
        {
            try
            {
                if (mCon != null)
                {
                    if (mCon != null) mCon.Close();
                    //if (mCon != null) mCon.Dispose();
                    //if (mCon != null) mCon = null;
                }
                mConnOpened = false;
            }
            catch (Exception ex)
            {
                mDataError = ex.Message;
            }
        }

        public DataTable SetDataTable(string SQL)
        {
            mDataTable = new DataTable();
            mDataAdap  = new SqlDataAdapter(SQL, mCon);
            mCommBuild = new SqlCommandBuilder(mDataAdap);
            mDataAdap.Fill(mDataTable);
            return mDataTable;
        }

        public bool ModifiedataTable()
        {
            DataTable changes = mDataTable.GetChanges();
            mDataAdap.Update(changes);
            mDataTable.AcceptChanges();
            return true;
        }

        #endregion

        #region Without Params

        public int ExecNonQuery(string sql)
        {
            int retcode = 0;
            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand("", sql))
                {
                    try                  { cmd.ExecuteNonQuery(); retcode = 1; }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }
            return retcode;
        }

        public DataSet GetDataSet(string SQL)
        {
            DataSet ds = new DataSet();
            if (Connect())
            {

                using (SqlCommand cmd = CreateCommand("", SQL))
                {
                    try 
                    {
                        SqlDataAdapter DA = new SqlDataAdapter(cmd);
                        SqlCommandBuilder CB = new SqlCommandBuilder(DA);
                        DA.Fill(ds);
                    }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }
            return ds;
        }

        public string ExecLookup(string sql)
        {
            string Value = "";
            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand("", sql))
                {
                    try                  { Value = GetValueFromObj(cmd, cmd.ExecuteScalar()); }
                    catch (Exception ex) 
                    { 
                        mDataError = ex.Message;
                       // InsertEvent(EventType: "ExecLookup", Description: "Error::" + mDataError , Property: "SQL::" + sql);
                    }
                }
            }
            Close();

            return Value;
        }

        public DataTable GetDataTable(string SQL)
        {
            DataTable dt = new DataTable();

            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand("", SQL))
                {
                    try
                    {
                        dt.TableName = "MSSQL";
                        using (var reader = cmd.ExecuteReader())
                            dt.Load(reader);  // Efficiently fills the DataTable
                    }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }

            return dt;

        }

        public SqlDataReader GetDataReader(string SQL)
        {
            SqlCommand CMD = new SqlCommand();
            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand("", SQL))
                {
                    try { return CMD.ExecuteReader(); }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }

            return null;
        }

        public DataTable GetDataReaderTable(string sql)
        {
            DataTable dt = new DataTable();

            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand("", sql))
                {
                    try
                    {
                        dt.TableName = "MSSQL";
                        using (var reader = cmd.ExecuteReader())
                            dt.Load(reader);  // Efficiently fills the DataTable
                    }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }
            Close();
            return dt;
        }

        #endregion

        #region With Params
       
        public int ExecNonQuery(string ProcName, string Parameters)   { return ExecNonQuery(ProcName, Parameters, false); }

        public int ExecNonQuery(string ProcName, string Parameters, bool Async)
        {

            int retcode = 0;

            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand(ProcName, Parameters))
                {
                    try
                    {
                        if (Async)            cmd.BeginExecuteNonQuery();
                        else        retcode = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)  { mDataError = ex.Message; }
                }
            }

            return retcode;
        }


        public string ExecLookup(string ProcName, string Parameters)
        {
            string Value = "";

            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand(ProcName, Parameters))
                {
                    try                  { Value = GetValueFromObj(cmd, cmd.ExecuteScalar()); }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }
            Close();

            return Value;
        }

        public DataTable GetDataTable(string ConnectionString, string ProcName, string Parameters)
        {
            string SaveConectionString = mConnectionString;
            mConnectionString = ConnectionString;

            DataTable DT = GetDataTable(ProcName, Parameters);

            mConnectionString = SaveConectionString;

            return DT;
        }

        public DataTable GetDataTable(string ProcName, string Parameters)
        {
            DataTable dt = new DataTable();

            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand(ProcName, Parameters))
                {
                    try
                    {
                        dt.TableName = "MSSQL";
                        using (var reader = cmd.ExecuteReader())
                            dt.Load(reader);  // Efficiently fills the DataTable
                    }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }

            return dt;
        }

        public SqlDataReader GetDataReader(string ProcName, string Parameters)
        {
            SqlCommand CMD = new SqlCommand();
            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand(ProcName, Parameters))
                {
                    try                  { return CMD.ExecuteReader();  }
                    catch (Exception ex) { mDataError = ex.Message;     }
                }
            }

            return null;
        }


        public DataTable GetDataReaderTable(string ProcName, string Parameters)
        {
            DataTable dt = new DataTable();

            if (Connect())
            {
                using (SqlCommand cmd = CreateCommand(ProcName, Parameters))
                {
                    try
                    {
                        dt.TableName = "MSSQL";
                        using (var reader = cmd.ExecuteReader())
                            dt.Load(reader);  // Efficiently fills the DataTable
                    }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }
            Close();

            return dt;
        }

        public DataSet GetDataSet(string ProcName, string Parameters)
        {
            DataSet ds = new DataSet();
            if (Connect())
            {

                using (SqlCommand cmd = CreateCommand(ProcName, Parameters))
                {
                    try
                    {
                        SqlDataAdapter    DA = new SqlDataAdapter(cmd);
                        SqlCommandBuilder CB = new SqlCommandBuilder(DA);
                        DA.Fill(ds);
                    }
                    catch (Exception ex) { mDataError = ex.Message; }
                }
            }
            return ds;
        }

        #endregion

        public SqlCommand CreateCommand(string ProcName, string Parameters)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection     = mCon;
            cmd.CommandTimeout = mCommandTimeOut;

            if (!string.IsNullOrEmpty(ProcName))
            {
                if (ProcName.Contains("."))
                {
                    Array arr = ProcName.Split('.');
                    string Arr0 = arr.GetValue(0).ToString();
                    if (Arr0.StartsWith("[")) Arr0 = Arr0.Remove(0, 1);
                    if (Arr0.Substring(Arr0.Length - 1, 1).Equals("]")) Arr0 = Arr0.Remove(Arr0.Length - 1, 1);

                    string Arr1 = arr.GetValue(1).ToString();
                    if (Arr1.StartsWith("[")) Arr1 = Arr1.Remove(0, 1);
                    if (Arr1.Substring(Arr1.Length - 1, 1).Equals("]")) Arr1 = Arr1.Remove(Arr1.Length - 1, 1);

                    ProcName = "[" + Arr0 + "].[" + Arr1 + "]";
                }
                cmd.CommandText = ProcName;
                cmd.CommandType = CommandType.StoredProcedure;

                if (!string.IsNullOrEmpty(Parameters)) BuildParams(ref cmd, Parameters);
            }
            else
            {
                cmd.CommandText = Parameters;
                cmd.CommandType = CommandType.Text;
            }

            return cmd;
        }

        private void  BuildParams(ref SqlCommand cmd, string Parameters)
        {
            int OutPrm = -1;

            Array ParArr = Parameters.Split(new char[] { ';' });
            for (int i = 0; i <= ParArr.GetLength(0) - 1; i++)
            {
                SqlParameter Parameter = new SqlParameter();

                string Param = ParArr.GetValue(i).ToString().Trim();
                if (!String.IsNullOrEmpty(Param))
                {
                    Array VarArr = (Param+"~~~~").Split(new char[] { '~' });

                    if (!String.IsNullOrEmpty(VarArr.GetValue(2).ToString()))
                    {
                        try
                        {
                            string ParamDir   = VarArr.GetValue(0).ToString().ToUpper().Trim();
                            string ParamName  = "@" + VarArr.GetValue(2).ToString().Trim();
                            string ParamValue = VarArr.GetValue(3).ToString().Trim();
                            string ParamType  = VarArr.GetValue(1).ToString().Trim();

                            SqlParameter prm  = new SqlParameter();
                            prm.ParameterName = ParamName;
                            prm.Direction = (ParamDir.Equals("I") ? ParameterDirection.Input : ParameterDirection.InputOutput);

                            if (!ParamDir.Equals("I")) { String z = ""; }

                            if (prm.Direction.Equals(ParameterDirection.Output)) { String z = ""; }


                            if (ParamDir.Equals("O")) OutPrm = cmd.Parameters.Count;

                            switch (ParamType)
                            {
                                case "I":
                                case "N": if (ParamValue.Contains(".")) ParamValue = helpers.ToDbl(ParamValue).ToString("0");
                                          prm.DbType = DbType.Int64;    prm.Value = Convert.ToInt64  ((String.IsNullOrEmpty(ParamValue)) ? "0" : ParamValue); break;
                                case "F":
                                case "R": prm.DbType = DbType.Decimal;  prm.Value = Convert.ToDecimal((String.IsNullOrEmpty(ParamValue)) ? "0" : ParamValue); break;
                                case "D": prm.DbType = DbType.DateTime; prm.Value = helpers.ToDate (ParamValue);                                              break;
                                default:  prm.DbType = DbType.String;   prm.Value = ParamValue; prm.Size = 256;                                               break;
                            }
                            cmd.Parameters.Add(prm);

                        }
                        catch (Exception ex) { string m = ex.Message; }
                    }
                }
            }

        }

        public string GetValueFromObj(SqlCommand cmd, Object obj)
        {
            int    OutPrm = -1;
            string Value  = string.Empty;

            if      (obj is Guid)  { Guid  GK   = new System.Guid(); GK = (System.Guid)obj;  Value = GK.ToString(); }
            else if (obj is Byte)  { Int32 data = Convert.ToInt32(obj);                      Value = data.ToString(); }
            else if (obj is Array) { Int32 data = Convert.ToInt32(obj);                      Value = data.ToString(); }
            else if (obj == null && OutPrm >= 0) {                                           Value = cmd.Parameters[OutPrm].Value.ToString(); }

            else if (obj != null)
            {
                try
                {
                    Value = obj.ToString();
                    if (Value.ToUpper() == "SYSTEM.OBJECT") Value = "";
                }
                catch (Exception ex)
                {
                    mDataError = ex.Message;
                }
            }

            return Value;

        }

        public SqlDataAdapter InsertedataAdapter(string sql)
        {
            if (Connect())
            {
                SqlDataAdapter da = new SqlDataAdapter();
                try
                {
                    da = new SqlDataAdapter(sql, mCon);
                    //CloseDB()
                    return (da);
                }
                catch (SqlException ex)
                {
                    mDataError = ex.Message;
                    //Debug.WriteLine(ex.Message);
                }
                finally
                {
                    da = null;
                }
                da.Dispose();
                da = null;
            }
            return null;
        }

        public bool IsValidGK(string ID)
        {
            return (Convert.ToDecimal(ID) > 0);
        }

        public string NewGuid()
        {
            return System.Guid.NewGuid().ToString();
        }

        public string getUpdateSQL(DataTable Data, int Row, string OmitFields)
        {

            //string SQL = "";
            string Values = "";

            OmitFields = (!string.IsNullOrEmpty(OmitFields) ? ";" + OmitFields.ToUpper() + ";" : "");

            for (int i = 0; i <= Data.Columns.Count - 1; i++)
            {
                bool Process = true;
                string ColName = Data.Columns[i].ColumnName.ToUpper();
                if (!string.IsNullOrEmpty(OmitFields))
                {
                    Process = !OmitFields.Contains(";" + ColName + ";");
                }
                if (Process)
                {
                    string Value = Data.Rows[Row][i].ToString();
                    switch (Data.Columns[i].DataType.ToString().ToUpper())
                    {
                        case "SYSTEM.STRING":
                            Value = "'" + Value + "'";
                            break;
                        case "SYSTEM.INT32":
                        case "SYSTEM.INT16":
                        case "SYSTEM.INT64":
                        case "SYSTEM.DECIMAL":
                        case "SYSTEM.DOUBLE":
                        case "SYSTEM.SINGLE":
                        case "SYSTEM.REAL":
                        case "SYSTEM.FLOAT":
                        case "SYSTEM.BYTE":
                        case "SYSTEM.SBYTE":
                            if (!string.IsNullOrEmpty(Value))
                                Value = Convert.ToInt32(Value).ToString();
                            break;
                        case "SYSTEM.BOOLEAN":
                            Value = (Value == "True" ? "1" : Value);// Convert.ToInt32(Value));
                            break;
                        case "SYSTEM.DATETIME":
                        case "SYSTEM.DATE":
                        case "SYSTEM.SMALLDATETIME":
                            if (helpers.IsDate(Value))
                            {
                                Value = "'" + Convert.ToDateTime(Value).ToString("MMM dd, yyyy HH:mm") + "'";
                            }
                            else
                            {
                                Value = "";
                            }
                            break;
                        default:
                            Value = "'" + Value + "'";
                            break;
                    }
                    if (!string.IsNullOrEmpty(Value))
                    {
                        if (!string.IsNullOrEmpty(Values))
                            Values += ",";
                        Values += Data.Columns[i].ColumnName + "=" + Value;
                    }
                }
            }
            return Values;
        }

        public string getInsertSQL(DataTable Data, int Row, string OmitFields)
        {
            OmitFields = (!string.IsNullOrEmpty(OmitFields) ? ";" + OmitFields.ToUpper() + ";" : "");

            string Fields = "";
            string Values = "";

            for (int i = 0; i <= Data.Columns.Count - 1; i++)
            {
                bool Process = true;
                string ColName = Data.Columns[i].ColumnName.ToUpper();
                if (!string.IsNullOrEmpty(OmitFields))
                {
                    Process = !OmitFields.Contains(";" + ColName + ";");
                }
                if (Process)
                {
                    if (Data.Columns[i].DataType.ToString().ToUpper() != "SYSTEM.BYTE[]")
                    {
                        string Value = Data.Rows[Row][i].ToString();
                        switch (Data.Columns[i].DataType.ToString().ToUpper())
                        {
                            case "SYSTEM.STRING":
                                Value = "'" + Value + "'";
                                break;
                            case "SYSTEM.INT32":
                            case "SYSTEM.INT16":
                            case "SYSTEM.DECIMAL":
                            case "SYSTEM.INT64":
                            case "SYSTEM.DOUBLE":
                            case "SYSTEM.SINGLE":
                            case "SYSTEM.REAL":
                            case "SYSTEM.FLOAT":
                            case "SYSTEM.BYTE":
                            case "SYSTEM.SBYTE":
                            case "SYSTEM.BYTE[]":
                                if (!string.IsNullOrEmpty(Value))
                                    Value = Convert.ToInt32(Value).ToString();
                                break;
                            case "SYSTEM.BOOLEAN":
                                Value = (Value == "True" ? "1" : Value);// Convert.ToInt32(Value));
                                break;
                            case "SYSTEM.DATETIME":
                            case "SYSTEM.DATE":
                            case "SYSTEM.SMALLDATETIME":
                                if (helpers.IsDate(Value))
                                {
                                    Value = "'" + Convert.ToDateTime(Value).ToString("MMM dd, yyyy HH:mm") + "'";
                                }
                                else
                                {
                                    Value = "";
                                }
                                break;
                            default:
                                Value = "'" + Value + "'";
                                break;
                        }
                        if (!string.IsNullOrEmpty(Value))
                        {
                            if (!string.IsNullOrEmpty(Fields))
                                Fields += ",";
                            Fields += Data.Columns[i].ColumnName;
                            if (!string.IsNullOrEmpty(Values))
                                Values += ",";
                            Values += Value;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(Fields))
            {
                return "(" + Fields + ") Values (" + Values + ")";
            }

            return "";
        }

        public bool UpdateImage(byte[] FaceImage, string ImageId)
        {
            bool ret = true;

            using (SqlConnection CN = new SqlConnection("")) //dwr setting.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        CN.Open();
                        cmd.Connection  = CN;
                        cmd.CommandText = "Update [loc].[Image] set ImageData = isnull(@ImageData, ImageData) where ImagePK = @ImagePK";
                        cmd.Parameters.AddWithValue("@ImagePK", ImageId);

                        IDataParameter par = cmd.CreateParameter();
                        par.ParameterName  = "ImageData";
                        par.DbType         = DbType.Binary;
                        par.Value          = FaceImage;

                        cmd.Parameters.Add(par);
                        cmd.ExecuteNonQuery();

                    }
                    catch (Exception ex) { ret = false; }
                }
            }


            return ret;

        }
    
        public DataTable GetSchema(string Database, string Owner, string TableName, string TableType)
        {
            string[] Restrictions = new string[4];
            // 1=owner,2=tablename
            Restrictions[0] = Database;
            // database/catalog name
            Restrictions[3] = TableType;
            //"BASE TABLE" ' get user Inserted tables only
            if (mCon.State != ConnectionState.Open)
                mCon.Open();
            return mCon.GetSchema(SqlClientMetaDataCollectionNames.Tables, Restrictions);
        }

        public int LookUpValue(string TypeCode, string ValueCode)
        {
            int ret = 0;
            if (Connect())
            {
                int.TryParse(ExecLookup("select ValuePk from cmn.EnumValue where ValueType = 1 and ValueCode = '" + ValueCode + "' and EnumId = (select EnumPk from cmn.EnumType where EnumCode = '" + TypeCode + "')"), out ret);
            }
            return ret;
        }

        public int LookUpState(string TypeCode, string ValueCode)
        {
            int ret = 0;
            if (Connect())
            {
                int.TryParse(ExecLookup("select ValuePk from cmn.EnumValue where ValueType = 2 and ValueCode = '" + ValueCode + "' and EnumId = (select EnumPk from cmn.EnumType where EnumCode = '" + TypeCode + "')"), out ret);
            }
            return ret;
        }
        public int LookUpAction(string TypeCode, string ActionType, string ActionCode)
        {
            int ret = 0;
            if (Connect())
            {
                int.TryParse(ExecLookup("select ActionPk from [cmn].[vue_Occurrence_Action] where TypeCode='" + TypeCode + "' and ActionType = '" + ActionType + "' and ActionCode = '" + ActionCode + "'"), out ret);
            }
            return ret;
        }

        public string LookUpActionFormat(string Format, string TypeCode, string ActionType, string ActionCode)
        {
            string ret = "";
            if (Connect())
            {
                ret = ExecLookup("select " + Format + " from [cmn].[vue_Occurrence_Action] where TypeCode='" + TypeCode + "' and ActionType = '" + ActionType + "' and ActionCode = '" + ActionCode + "'");
            }
            return ret;
        }


        public bool InsertEvent(string EventType = null, string ActionTypeId = null, string ParentID = null, string EntityID = null, string HumanID = null, string WorkerID = null, string FunctionID = null, string Property = null, string Description = null, string ExtensionId = null)
        {
            bool ret = false;

            try
            {
                string Params = "";

                if (!String.IsNullOrEmpty(ActionTypeId)) Params += "I~I~EventActionID~" + ActionTypeId + ";";
                if (!String.IsNullOrEmpty(EventType))    Params += "I~I~EventTypeID~"   + LookUpValue("Error", EventType) + ";";
                if (!String.IsNullOrEmpty(ParentID))     Params += "I~I~ParentID~"      + ParentID + ";";
                if (!String.IsNullOrEmpty(EntityID))     Params += "I~I~EntityID~"      + EntityID + ";";
                if (!String.IsNullOrEmpty(HumanID))      Params += "I~I~HumanID~"       + HumanID + ";";
                if (!String.IsNullOrEmpty(WorkerID))     Params += "I~I~WorkerID~"      + WorkerID + ";";
                if (!String.IsNullOrEmpty(FunctionID))   Params += "I~I~FunctionID~"    + FunctionID + ";";
                if (!String.IsNullOrEmpty(Description))  Params += "I~S~Description~"   + Description + ";";
                if (!String.IsNullOrEmpty(Property))     Params += "I~S~Property~"      + Property;

                string EventPK = ExecLookup("cmn.mod_Occurrence_Event", Params).ToString();
            }
            catch (Exception ex)
            {

            }

            return ret;

        }

        public int InsertEGMRequest(string Request, int EventPK, bool Active, int EgmID, int MemberID, int GamingTypeUI, int ActionTypeID, string Property, string Description)
        {
            string SQL = "";

            if (EventPK > 0)
                SQL += "update [cmn].[Event] set EventState=cmn.getStatePK('Event', 'InActive'),EventDate='" + DateTime.Now.ToString("MMM dd, yyyy HH:mm") + "', Description='" + Description + "' where EventPK=" + EventPK.ToString() + " ";

            if (Active)
            {
                try
                {
                    SQL += "declare @EventPK int ";
                    SQL += "set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "insert into [cmn].[Occurrence] (OccurrencePk, OccurrenceType, OccurrenceState, OccurrenceDate, Inserted, Modified) values (@EventPK,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate(), getdate() ) ";
                    SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event', 'Active') ";
                    SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityID, HumanID, Property, Description) ";
                    SQL += "values (@EventPK," + ActionTypeID + "," + GamingTypeUI + ",@EventState, '" + DateTime.Now.ToString("MMM dd, yyyy HH:mm") + "'," + EgmID + "," + (MemberID > 0 ? MemberID.ToString() : "NULL") + ",'" + Property + "','" + Description + "') ";
                    SQL += "Select @EventPK ";

                    int.TryParse(ExecLookup(SQL), out EventPK);

                    ExecLookup("update [gam].[Current] set " + Request + "=" +  EventPK.ToString() + ", " + Request + "Start='" + DateTime.Now.ToString("MMM dd, yyyy HH:mm") + "' where EgmID=" + EgmID);

                }
                catch (Exception ex) { }
            }
            else
            {
                try
                {   // reset request condition
                    SQL += "update [gam].[Current] set " + Request + "=NULL where EgmID=" + EgmID;
                    ExecNonQuery(SQL);
                    EventPK = 0;
                }
                catch (Exception ex) { }
            }

            return EventPK;
        }

        public int InsertEGMFault(int EventPK, bool Active, int EgmID, int GamingTypeUI, int ActionTypeID, string Property, string Description)
        {

            string SQL = "";

            if (EventPK > 0)
                SQL += "update [cmn].[Event] set EventState=cmn.getStatePK('Event', 'InActive'),EventDate='" + DateTime.Now.ToString("MMM dd, yyyy HH:mm") + "',Description='" + Description + "' where EventPK=" + EventPK.ToString() + " ";

            if (Active)
            {
                try
                {
                    SQL += "declare @EventPK int ";
                    SQL += "set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "insert into [cmn].[Occurrence] (OccurrencePk, OccurrenceType, OccurrenceState, OccurrenceDate, Inserted, Modified) values (@EventPK,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate(), getdate() ) ";
                    SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event', 'Active') ";
                    SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityID, Property, Description) ";
                    SQL += "values (@EventPK," + ActionTypeID + "," + GamingTypeUI + ",@EventState, '" + DateTime.Now.ToString("MMM dd, yyyy HH:mm") + "'," + EgmID + ",'" + Property + "','" + Description + "') ";

                    SQL += "update [gam].[Current] set Fault=" + (Active ? "1" : "0") + ", FaultCount = isnull(FaultCount,0) + 1, FaultId=@EventPK where EgmID=" + EgmID;
                    SQL += "Select @EventPK ";

                    int.TryParse(ExecLookup(SQL), out EventPK);
                }
                catch (Exception ex) { }
            }
            else
            {
                try
                {   // check to see if any other faults ... if none reset faults condition
                    SQL += "IF not EXISTS(SELECT * FROM  [cmn].[Event] where EntityID=" + EgmID + " and EventAction = " + ActionTypeID + " and EventType=" + GamingTypeUI + " and EventState=cmn.getStatePK('Event', 'Active') ) ";
                    SQL += "update [gam].[Current] set Fault=0, FaultId=null where EgmID=" + EgmID;
                    ExecNonQuery(SQL);
                    EventPK = 0;
                }
                catch (Exception ex) { }
            }

            return EventPK;
        }

        public int LookUpEgm(int GMID)
        {
            int EgmId = 0;
            int.TryParse(ExecLookup("Select DevicePk from loc.device where devicestate=cmn.getStatePK('Device','Available') and Referenceno='" + GMID.ToString() + "'"), out EgmId);
            return EgmId;
        }

        public string LookUpEnumDesc(string ValueId)
        {
            return ExecLookup("Select Description from cmn.EnumValue where valuepk=" + ValueId);
        }



        #region IDisposable Implementation
        // To detect redundant calls
        private bool disposedValue = false;
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    Close();
                    // TODO: free managed resources when explicitly called
                }

                // TODO: free shared unmanaged resources
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

    //public class DataReaderAdapter : DbDataAdapter
    //{
    //    public int FillFromReader(ref DataTable dt, IDataReader dr)
    //    {
    //        return this.Fill(dt, dr);
    //    }
    //}
}
