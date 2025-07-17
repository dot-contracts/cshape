using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlServer
    {
        private dlEntity   mEntity;            public dlEntity Entity        { get { return mEntity; } set { mEntity = value; } }
        private string     mServerPK = "0";    public string   ServerPK      { get { return mEntity.EntityPK; } }
        private string     mDescription = "";

        private nxProperty mServerType;        public string   ServerType    { get { return mServerType.Value; }      set { mServerType.Value = value;  mServerTypeID      = EnumCache.Instance.getTypeFromDesc  ("Server",  mServerType.Value); } }
        private string     mServerTypeID;      public string   ServerTypeID  { get { return mServerTypeID; }          set { mServerTypeID = value;      mServerType.Value  = EnumCache.Instance.getDescFromId    (           mServerTypeID); } }
        private nxProperty mServerState;       public string   ServerState   { get { return mServerState.Value; }     set { mServerState.Value = value; mServerStateID     = EnumCache.Instance.getStateFromDesc ("Server",  mServerState.Value); } }
        private string     mServerStateID;     public string   ServerStateID { get { return mServerStateID; }         set { mServerStateID = value;     mServerState.Value = EnumCache.Instance.getDescFromId    (           mServerStateID); } }

        private nxProperty mName;              public string   Name          { get { return mName.Value; }            set { mName.Value = value; } }

        private nxProperty mIPAddr;            public string   IPAddr        { get { return mIPAddr.Value; }          set { mIPAddr.Value = value; } }
        private nxProperty mCatalog;           public string   Catalog       { get { return mCatalog.Value; }         set { mCatalog.Value = value; } }
        private nxProperty mPort;              public string   Port          { get { return mPort.Value; }            set { mPort.Value = value; } }
        private nxProperty mUsername;          public string   Username      { get { return mUsername.Value; }        set { mUsername.Value = value; } }
        private nxProperty mPassword;          public string   Password      { get { return mPassword.Value; }        set { mPassword.Value = value; } }
        private nxProperty mWindowsAuth;       public string   WindowsAuth   { get { return mWindowsAuth.Value; }     set { mWindowsAuth.Value = value; } }


        public dlServer()  
        {
            mServerType     = new nxProperty("Server", "ServerType");
            mServerState    = new nxProperty("Server", "ServerState");

            mName           = new nxProperty("Server", "Name");
            mIPAddr         = new nxProperty("Server", "IPAddr");
            mCatalog        = new nxProperty("Server", "Catalog");
            mPort           = new nxProperty("Server", "Port");
            mUsername       = new nxProperty("Server", "Username");
            mPassword       = new nxProperty("Server", "Password");
            mWindowsAuth    = new nxProperty("Server", "WindowsAuth");

            Reset("Server"); 
        }

        public bool Open(string ServerPK)
        {

            mServerPK = ServerPK;

            string SQL = "select ([Description],[ServerPK],Enum1.Description AS ServerType, Enum2.Description AS ServerState,[Name],[IPAddr],[Catalog],[Port],[Username],[Password],[WindowsAuth], ServerType as ServerTypeID, ServerState as ServerStateID ) ";
            SQL += "from loc.Server as s left join cmn.Entity as e on s.ServerPK=e.EntityID left join cmn.EnumValue as Enum1 ON s.ServerType = Enum1.ValuePK left join cmn.EnumValue AS Enum2 ON s.Serverstate = Enum2.ValuePK where ServerPK=" + mServerPK.ToString();
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return true;
        }

        public string Description(string ServerPK)
        {
            Open(ServerPK);
            return mDescription;
        }


        private void LoadFromRow(DataRow Row)
        {
            mDescription = Row["Description"].ToString();

            mServerTypeID      = Row["ServerTypeID"].ToString();
            mServerStateID     = Row["ServerStateID"].ToString();

            mServerType.Value  = Row["ServerType"].ToString();
            mServerState.Value = Row["ServerState"].ToString();

            mName.Value        = Row["Name"].ToString();
            mIPAddr.Value      = Row["IPAddr"].ToString();
            mCatalog.Value     = Row["Catalog"].ToString();
            mPort.Value        = Row["Port"].ToString();
            mUsername.Value    = Row["Username"].ToString();
            mPassword.Value    = Row["Password"].ToString();
            mWindowsAuth.Value = Row["WindowsAuth"].ToString();

        }

        public void Update()
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);

            //if (mEntity.NewEntry)
            //    mServerPK = DB.ExecLookup(getInsertSQL());
            //else
            //    DB.ExecNonQuery(getUpdateSQL());

            if (String.IsNullOrEmpty(DB.DataError))
            {
                mServerType.Update();
                mServerState.Update();
                mName.Update();
                mIPAddr.Update();
                mCatalog.Update();
                mPort.Update();
                mUsername.Update();
                mPassword.Update();
            }

            DB.Dispose();
            DB = null;
        }

        //public string getInsertSQL()
        //{
        //    string SQL = mEntity.getInsertSQL() + " ";
        //    SQL += "declare @ServerType int; Set @ServerType=" + mServerTypeID.ToString() + " ";
        //    SQL += "declare @ServerState int; Set @ServerState=" + mServerStateID.ToString() + " ";
        //    SQL += getColumns();
        //    SQL += "insert into [loc].[Server] ([ServerPK],[ServerType],[ServerState],[Name],[IPAddr],[Catalog],[Port],[Username],[Password],[WindowsAuth]) ";
        //    SQL += "values (@EntityID,@ServerTypeID,@ServerStateID,@Name,@IPAddr,@Catalog,@Port,@Username,@Password,@WindowsAuth) ";
        //    return SQL;
        //}

        //public string getUpdateSQL()
        //{
        //    string SQL = mEntity.getUpdateSQL() + " " + getColumns();
        //    SQL += "update [loc].[Server] set ServerType=@ServerType,ServerState=@ServerState,[Name]=@Name,[IPAddr]=@IPAddr,[Catalog]=@Catalog,[Port]=@Port,[Username]=@Username,[Password]=@Password,[WindowsAuth]=@WindowsAuth ";
        //    SQL += "where ServerPK=@EntityID";
        //    return SQL;
        //}

        private string getColumns()
        {
            string SQL = "declare @Name varchar(32); Set @Name=" + mName.Value + " ";
            SQL += "declare @IPAddr varchar(256); Set @IPAddr=" + mIPAddr + " ";
            SQL += "declare @Catalog varchar(256); Set @Catalog=" + mCatalog + " ";
            SQL += "declare @Port varchar(256); Set @Port=" + mPort + " ";
            SQL += "declare @Username varchar(256); Set @Username=" + mUsername + " ";
            SQL += "declare @Password varchar(256); Set @Password=" + mPassword + " ";
            SQL += "declare @WindowsAuth varchar(256); Set @WindowsAuth=" + mWindowsAuth + " ";
            return SQL;
        }

        public string GetSettings()
        {
            string SaveStr = "Id=";// +mVenueID;
            //SaveStr += ";No=" + mVenueNo;
            //SaveStr += ";Type=" + mVenueType;
            //SaveStr += ";Name=" + mVenueName;
            return SaveStr;
        }

        public void Find(string Column, string Value)
        {

        }

        public void Dispose() { }

        public void Reset(string ServerType)
        {
            ServerTypeID  = EnumCache.Instance.getTypeId  ("Server", ServerType);
            ServerStateID = EnumCache.Instance.getStateId ("Entity",   "Active");

            if (mEntity == null) 
                mEntity = new dlEntity("Server");
            else
                mEntity.Reset("Server");

            mName.Reset();
            mIPAddr.Reset();
            mCatalog.Reset();
            mPort.Reset();
            mUsername.Reset();
            mPassword.Reset();
            mWindowsAuth.Reset();


        }

        public override string ToString()
        {
            return mDescription;
        }

    }
}
