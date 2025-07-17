using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlWorker
    {

        private string     mConnectionString = "";
        private string     mDataError        = "";       public string DataError    { get { return mDataError; } set { mDataError = value; }}
                                                         public string Description  { get { return mHuman.FullName; } }

        private dlHuman    mHuman;             public dlHuman   Human           { get { return mHuman; }                 set { mHuman = value; } }
                                               public string    WorkerPK        { get { return mHuman.HumanPK; }         set { mHuman.HumanPK = value; } }
                                               public bool      NewEntry        { get { return mHuman.NewEntry; }        set { mHuman.NewEntry = value; } }


                                               public string    HumanType       { get { return mHuman.HumanType; }       set { mHuman.HumanType = value;   } }
                                               public string    HumanTypeID     { get { return mHuman.HumanTypeID; }     set { mHuman.HumanTypeID = value;       } }
                                               public string    HumanState      { get { return mHuman.HumanState; }      set { mHuman.HumanState= value;  } }
                                               public string    HumanStateID    { get { return mHuman.HumanStateID; }    set { mHuman.HumanStateID = value;      } }
                                               public string    FirstName       { get { return mHuman.FirstName; }       set { mHuman.FirstName = value; } }
                                               public string    OtherName       { get { return mHuman.OtherName; }       set { mHuman.OtherName = value; } }
                                               public string    LastName        { get { return mHuman.LastName; }        set { mHuman.LastName = value; } }
                                               public string    NickName        { get { return mHuman.NickName; }        set { mHuman.NickName = value; } }
                                               public string    Birthdate       { get { return mHuman.Birthdate; }       set { mHuman.Birthdate = value; } }
                                               public bool      UseNickName     { get { return mHuman.UseNickName; }     set { mHuman.UseNickName = value; } }
                                               public string    Gender          { get { return mHuman.Gender; }          set { mHuman.Gender = value;     } }
                                               public string    GenderID        { get { return mHuman.GenderID; }        set { mHuman.GenderID = value;         } }
                                               public string    Title           { get { return mHuman.Title; }           set { mHuman.Title = value; } }
                                               public string    TitleID         { get { return mHuman.TitleID; }         set { mHuman.TitleID = value;     } }

        private nxProperty mDepartment;        public string    Department      { get { return mDepartment.Value; }      set { mDepartment.Value = value;  mDepartmentID      = EnumCache.Instance.getTypeFromDesc  ("Department", mDepartment.Value); } }
        private string     mDepartmentID="";   public string    DepartmentID    { get { return mDepartmentID; }          set { mDepartmentID = value;      mDepartment.Value  = EnumCache.Instance.getDescFromId    (              mDepartmentID.ToString()); } }

        private nxProperty mWorkerType;        public string    WorkerType      { get { return mWorkerType.Value; }      set { mWorkerType.Value = value;  mWorkerTypeID      = EnumCache.Instance.getTypeFromDesc  ("Worker",     mWorkerType.Value); } }
        private string     mWorkerTypeID="";   public string    WorkerTypeID    { get { return mWorkerTypeID; }          set { mWorkerTypeID = value;      mWorkerType.Value  = EnumCache.Instance.getDescFromId    (              mWorkerTypeID.ToString()); } }

        private nxProperty mWorkerState;       public string    WorkerState     { get { return mWorkerState.Value; }     set { mWorkerState.Value = value; mWorkerStateID     = EnumCache.Instance.getStateFromDesc ("Worker",     mWorkerState.Value); } }
        private string     mWorkerStateID="";  public string    WorkerStateID   { get { return mWorkerStateID; }         set { mWorkerStateID = value;     mWorkerState.Value = EnumCache.Instance.getDescFromId    (              mWorkerStateID.ToString()); } }

        private nxProperty mRole;              public string    Role            { get { return mRole.Value; }            set { mRole.Value = value;        mRoleID            = EnumCache.Instance.getTypeFromDesc  ("Role",        mRole.Value); } }
        private string     mRoleID="";         public string    RoleID          { get { return mRoleID; }                set { mRoleID = value;            mRole.Value        = EnumCache.Instance.getDescFromId    (               mRoleID.ToString()); } }

        private nxProperty mTeamLeader;        public string    TeamLeader      { get { return mTeamLeader.Value; }      set { mTeamLeader.Value = value;  mTeamLeaderID      = WorkerCache.Instance.Value          (               mTeamLeader.Value); } }
        private string     mTeamLeaderID="";   public string    TeamLeaderID    { get { return mTeamLeaderID; }          set { mTeamLeaderID = value;      mTeamLeader.Value  = WorkerCache.Instance.Display        (               mTeamLeaderID.ToString()); } }

        private nxProperty mNextOfKin;         public string    NextOfKin       { get { return mNextOfKin.Value; }       set { mNextOfKin.Value = value;   mNextOfKinID       = WorkerCache.Instance.Value          (               mNextOfKin.Value); } }
        private string     mNextOfKinID="";    public string    NextOfKinID     { get { return mNextOfKinID; }           set { mNextOfKinID = value;       mNextOfKin.Value   = WorkerCache.Instance.Display        (               mNextOfKinID.ToString()); } }

        private nxProperty mStartDate;         public string    StartDate       { get { return mStartDate.Value; }       set { mStartDate.Value = value; } }

        private nxProperty mUserName;          public string    UserName        { get { return mUserName.Value; }        set { mUserName.Value = value; } }
        private nxProperty mPassword;          public string    Password        { get { return mPassword.Value; }        set { mPassword.Value = value; } }

        private string     mFaceID;            public string    FaceID          { get { return mFaceID; }                set { mFaceID = value; } }

        public dlWorker()
        {
            if (nexus.common.core.shell.Instance != null)
            {
                mHuman = new dlHuman(setting.ConnectionString);
                Create(setting.ConnectionString);
            }
            else
            {
                mHuman = new dlHuman();
            }
        }
        public dlWorker(string ConnectionString)
        {
            mHuman = new dlHuman(ConnectionString);
            Create(ConnectionString);
        }

        public void Create(string ConnectionString)
        {
            mConnectionString = ConnectionString;

            mWorkerType   = new nxProperty("Worker", "WorkerType");
            mWorkerState  = new nxProperty("Worker", "WorkerState");
            mRole         = new nxProperty("Worker", "Role");
            mDepartment   = new nxProperty("Worker", "Department");
            mTeamLeader   = new nxProperty("Worker", "TeamLeader");
            mNextOfKin    = new nxProperty("Worker", "NextOfKin");
            mStartDate    = new nxProperty("Member", "StartDate");
            mUserName     = new nxProperty("Member", "UserName");
            mPassword     = new nxProperty("Member", "Password");

            Reset("Worker");
        }


        public bool Open(int WorkerPK)    { return Open(WorkerPK.ToString()); }
        public bool Open(string WorkerPK)
        {
            WorkerPK = WorkerPK.Equals("0") ? string.Empty: WorkerPK;
            if (!string.IsNullOrEmpty(WorkerPK))
            {
                SQLServer DB = new SQLServer(setting.ConnectionString);
                DataTable DT = DB.GetDataTable("loc.lst_Human_Worker", "I~S~WorkerPK~" + WorkerPK);
                if (DT.Rows.Count > 0)
                    LoadFromRow(DT.Rows[0]);

                DB.Dispose();
                DB = null;
            }

            setting.WorkerId = WorkerPK;

            return String.IsNullOrEmpty(mDataError);


        }
        public void LoadRootWorker()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                Open(DB.ExecLookup("select top 1 e.EntityPK from[loc].[Entity] as e left join loc.human_worker as h on e.EntityPK= h.workerPK where h.humantype = cmn.getValuePK('Entity.Human.Worker', 'System') order by e.EntityPK"));
        }

        private void LoadFromRow(DataRow Row)
        {
            mHuman.Open(Row["WorkerPK"].ToString());

            mWorkerTypeID         = Row["WorkerTypeID"].ToString();
            mWorkerStateID        = Row["WorkerStateID"].ToString();
            mWorkerType.Value     = Row["WorkerType"].ToString();
            mWorkerState.Value    = Row["WorkerState"].ToString();
            mRole.Value           = Row["RoleID"].ToString();
            mDepartment.Value     = Row["DepartmentID"].ToString();
            mTeamLeader.Value     = Row["TeamLeaderID"].ToString();
            mNextOfKin.Value      = Row["NextOfKin"].ToString();
            mStartDate.Value      = Row["StartDate"].ToString();
            mUserName.Value       = Row["UserName"].ToString();
            mPassword.Value       = Row["Password"].ToString();

            mFaceID = (Row["FaceId"].ToString());
        }

        public bool Update()
        {

            string Params = "";
            if (!mHuman.NewEntry) Params = "I~S~WorkerPK~" + mHuman.HumanPK + ";";
            Params += "I~S~WorkerType~"  + mWorkerTypeID      + ";";
            Params += "I~S~WorkerState~" + mWorkerStateID     + ";";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            mHuman.HumanPK = DB.ExecLookup("loc.mod_Human_Worker", Params).ToString();
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mHuman.NewEntry)
                {
                    mWorkerType.Update();
                    mWorkerState.Update();
                }
                Open(mHuman.HumanPK);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Human", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mRole.Value))       Params += "I~S~RoleID~"       + mRole.Value  + ";";
            if (!String.IsNullOrEmpty(mDepartment.Value)) Params += "I~S~DepartmentID~" + mDepartment.Value + ";";
            if (!String.IsNullOrEmpty(mTeamLeader.Value)) Params += "I~S~TeamLeaderID~" + mTeamLeader.Value + ";";
            if (!String.IsNullOrEmpty(mNextOfKin.Value))  Params += "I~S~NextOfKin~"    + mNextOfKin.Value + ";";
            if (!String.IsNullOrEmpty(mStartDate.Value))  Params  += "I~S~StartDate~"   + mStartDate.Value + ";";
            if (!String.IsNullOrEmpty(mUserName.Value))   Params += "I~S~UserName~"     + mUserName.Value + ";";
            if (!String.IsNullOrEmpty(mPassword.Value))   Params += "I~S~Password~"     + mPassword.Value + ";";
            if (!String.IsNullOrEmpty(mFaceID))           Params += "I~S~FaceId~"       + mFaceID + ";";
            Params += mHuman.getParams();
            return Params;
        }


        public void Dispose() { }

        public void Reset(string WorkerType)
        {

            mHuman.Reset();

            WorkerTypeID  = EnumCache.Instance.getTypeId  ("Entity.Human.Worker", WorkerType);
            WorkerStateID = EnumCache.Instance.getStateId ("Entity.Human", "Available");
            mUserName.Reset();
            mPassword.Reset();
        }

        public override string ToString() { return mHuman.HumanPK + "," + mHuman.FullName; }


    }
}
