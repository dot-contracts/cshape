using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.cache; 

namespace nexus.common.dal
{
    public class dlEntity
    {

        private string     mEntityGK = "";    public string   EntityGK      { get { return mEntityGK; }  }
        private string     mEntityPK   = "";  public string   EntityPK      { get { return mEntityPK; }          set { mEntityPK = value; mNewEntry = false; } }
        private string     mSystemID   = "";  public string   SystemID      { get { return mSystemID; }          set { mSystemID = value; } }

        private string     mParentID   = "";  public string   ParentID      { get { return mParentID; }          set { mParentID = value; } }
        private string     mOwnerID    = "";  public string   OwnerID       { get { return mOwnerID; }           set { mOwnerID  = value; } }
        private string     mMediaID    = "";  public string   MediaID       { get { return mMediaID; }           set { mMediaID = value; } }
        private string     mScheduleID = "";  public string   ScheduleID    { get { return mScheduleID; }        set { mScheduleID = value; } }

        private nxProperty mEntityType;       public string   EntityType    { get { return mEntityType.Value; }  set {                                                     mEntityType.Value = value;  mEntityTypeID      = EnumCache.Instance.getTypeFromDesc  ("Entity",   mEntityType.Value); } }
        private string     mEntityTypeID;     public string   EntityTypeID  { get { return mEntityTypeID; }      set { if (!value.Equals(mEntityTypeID))  mChanged = true; mEntityTypeID = value;      mEntityType.Value  = EnumCache.Instance.getDescFromId    (            mEntityTypeID); } }

        private nxProperty mEntityState;      public string   EntityState   { get { return mEntityState.Value; } set {                                                     mEntityState.Value = value; mEntityStateID     = EnumCache.Instance.getStateFromDesc ("Entity",   mEntityState.Value); } }
        private string     mEntityStateID;    public string   EntityStateID { get { return mEntityStateID; }     set { if (!value.Equals(mEntityStateID)) mChanged = true; mEntityStateID = value;     mEntityState.Value = EnumCache.Instance.getDescFromId    (            mEntityStateID); } }

        private nxProperty mSharing;          public string   Sharing       { get { return mSharing.Value; }     set {                                                     mSharing.Value = value;     mSharingID         = EnumCache.Instance.getTypeFromDesc  ("Sharing",  mSharing.Value); } }
        private string     mSharingID;        public string   SharingID     { get { return mSharingID; }         set { if (!value.Equals(mSharingID)) mChanged = true;     mSharingID = value;         mSharing.Value     = EnumCache.Instance.getDescFromId    (            mSharingID); } }
         
        private nxProperty mShareState;       public string   ShareState    { get { return mShareState.Value; }  set {                                                     mShareState.Value = value;  mShareStateID      = EnumCache.Instance.getStateFromDesc ("Sharing",  mShareState.Value); } }
        private string     mShareStateID;     public string   ShareStateID  { get { return mShareStateID; }      set { if (!value.Equals(mShareStateID)) mChanged = true;  mShareStateID = value;      mShareState.Value  = EnumCache.Instance.getDescFromId    (            mShareStateID); } }

        private nxProperty mOperability;      public string   Operability   { get { return mOperability.Value; } set {                                                     mOperability.Value = value; mOperabilityID     = EnumCache.Instance.getTypeFromDesc  ("Operability",   mOperability.Value); } }
        private string     mOperabilityID;    public string   OperabilityID { get { return mOperabilityID; }     set { if (!value.Equals(mOperabilityID)) mChanged = true; mOperabilityID = value;     mOperability.Value = EnumCache.Instance.getDescFromId    (            mOperabilityID); } }

        private nxProperty mVenue;            public string   Venue         { get { return mVenue.Value; }       set {                                                     mVenue.Value = value;       mVenueID           = VenueCache.Instance.Value           (            mVenue.Value); } }
        private string     mVenueID;          public string   VenueID       { get { return mVenueID; }           set { if (!value.Equals(mVenueID)) mChanged = true;       mVenueID = value;           mVenue.Value       = VenueCache.Instance.Display         (            mVenueID); } }
        private nxProperty mDescription;      public string   Description   { get { return mDescription.Value; } set { if (!value.Equals(mDescription.Value)) mChanged = true;  mDescription.Value = value;   } }

        private nxProperty mLogging;          public string   Logging       { get { return mLogging.Value; }     set {                                                     mLogging.Value = value;     mLoggingID         = EnumCache.Instance.getTypeFromDesc  ("Logging",  mLogging.Value); } }
        private string     mLoggingID;        public string   LoggingID     { get { return mLoggingID; }         set { if (!value.Equals(mLoggingID)) mChanged = true;     mLoggingID = value;         mLogging.Value     = EnumCache.Instance.getDescFromId    (            mLoggingID); } }

        private nxProperty mLogType;          public string   LogType       { get { return mLogType.Value; }     set { mLogType.Value = value;     mLogTypeID         = EnumCache.Instance.getTypeFromDesc     ("LogType",    mLogType.Value); } }
        private string     mLogTypeID;        public string   LogTypeID     { get { return mLogTypeID; }         set { mLogTypeID = value;         mLogType.Value     = EnumCache.Instance.getDescFromId       (              mLogTypeID); } }
        private nxProperty mLoggedAt;         public string   LoggedAt      { get { return mLoggedAt.Value; }    set { mLoggedAt.Value = value;    mLoggedAtID        = ComputerCache.Instance.Value           (              mLoggedAt.Value); } }
        private string     mLoggedAtID;       public string   LoggedAtID    { get { return mLoggedAtID; }        set { mLoggedAtID = value;        mLoggedAt.Value    = ComputerCache.Instance.Display         (              mLoggedAtID); } }
        private nxProperty mLoggedBy;         public string   LoggedBy      { get { return mLoggedBy.Value; }    set { mLoggedBy.Value = value;    mLoggedByID        = WorkerCache.Instance.Value             (              mLoggedBy.Value); } }
        private string     mLoggedByID;       public string   LoggedByID    { get { return mLoggedByID; }        set { mLoggedByID = value;        mLoggedBy.Value    = WorkerCache.Instance.Display           (              mLoggedByID); } }
        private nxProperty mLogModule;        public string   LogModule     { get { return mLogModule.Value; }   set { mLogModule.Value = value;   mLogModuleID       = EnumCache.Instance.getTypeFromDesc     ("LogModule",  mLogModule.Value); } }
        private string     mLogModuleID;      public string   LogModuleID   { get { return mLogModuleID; }       set { mLogModuleID = value;       mLogModule.Value   = EnumCache.Instance.getDescFromId       (              mLogModuleID); } }

        private bool       mNewEntry = true;  public bool     NewEntry      { get { return mNewEntry; }          set { mNewEntry = value; } }
        private string     mSelected;         public string   Selected      { get { return mSelected; }          set { mSelected = value; } }
        private string     mSequence;         public string   Sequence      { get { return mSequence; }          set { mSequence = value; } }

        private string     mDataError;        public string   DataError     { get { return mDataError; }         set { mDataError = value; } }
        private bool       mChanged = false;

        public dlEntity(string EntityType) 
        {

            mVenue        = new nxProperty("Entity", "Venue");
            mDescription  = new nxProperty("Entity", "Description");

            mEntityType   = new nxProperty("Entity", "EntityType");
            mEntityState  = new nxProperty("Entity", "EntityState");
            mSharing      = new nxProperty("Entity", "Sharing");
            mShareState   = new nxProperty("Entity", "ShareState");
            mOperability  = new nxProperty("Entity", "Operability");
            mLogging      = new nxProperty("Entity", "Logging");
            mLogType      = new nxProperty("Entity", "LogType");
            mLoggedAt     = new nxProperty("Entity", "LoggedAt");
            mLoggedBy     = new nxProperty("Entity", "LoggedBy");
            mLogModule    = new nxProperty("Entity", "LogModule");

            Reset(EntityType); 
        }
        public void Create(string ConnectionString)
        {
           // mConnectionString = ConnectionString;

            Reset(EntityType);
        }

        public bool Open(int EntityPK) { return Open(EntityPK.ToString()); }
        public bool Open(string EntityPK)
        {
            Reset();

            EntityPK = EntityPK.Equals("0") ? string.Empty : EntityPK;
            if (!string.IsNullOrEmpty(EntityPK))
            {
                SQLServer DB = new SQLServer(setting.ConnectionString);
                DataTable DT = DB.GetDataTable("loc.lst_Entity", "I~S~EntityPK~" + EntityPK);
                if (DT.Rows.Count > 0)
                    LoadFromRow(DT.Rows[0]);

                DB.Dispose();
                DB = null;
            }

            return String.IsNullOrEmpty(mDataError);

        }


        private void LoadFromRow(DataRow Row)
        {
            mEntityGK          = Row["EntityGK"].ToString(); 
            mEntityPK          = Row["EntityPK"].ToString(); 
            mSystemID          = Row["SystemID"].ToString();

            EntityTypeID       = Row["EntityType"].ToString();
            mEntityStateID     = Row["EntityState"].ToString();

            mSharing.Create    ( Row["Sharing"].ToString());
            mShareState.Create ( Row["ShareState"].ToString());
            mOperability.Create( Row["Operability"].ToString());

            mLogging.Create    ( Row["Logging"].ToString());
            mLogType.Create    ( Row["LogType"].ToString());
            mLoggedAt.Create   ( Row["LoggedAt"].ToString());
            mLoggedBy.Create   ( Row["LoggedBy"].ToString());
            mLogModule.Create  ( Row["LogModule"].ToString());

            mVenue.Create      ( Row["VenueID"].ToString());
            mDescription.Create( Row["Description"].ToString());

            mSequence          = Row["Sequence"].ToString();
            mSelected          = Row["Selected"].ToString();

            mNewEntry = false;
            mChanged  = false;

        }


        public bool Update()
        {
            string Params = "";
            if (!mNewEntry) Params = "I~S~EntityPK~" + mEntityPK + ";";
            Params += "I~S~EntityType~"   + mEntityTypeID + ";";
            Params += "I~S~EntityState~"  + mEntityStateID + ";";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            string EntityPK = DB.ExecLookup("loc.mod_Entity", Params + getParams()).ToString();
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            if (String.IsNullOrEmpty(mDataError))
            {
                UpdateProperty();
                Open(EntityPK);
                mNewEntry = false;
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Human", "UpdateError"), mDataError); }

            mChanged = false;

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        { 
            string Params = " ";
            //if (!String.IsNullOrEmpty(mDescription.Value)) Params += "I~S~Description~" + mDescription.Value + ";";
            if (!String.IsNullOrEmpty(mSharingID))         Params += "I~N~Sharing~"     + mSharingID + ";";
            if (!String.IsNullOrEmpty(mShareStateID))      Params += "I~N~ShareState~"  + mShareStateID + ";";
            if (!String.IsNullOrEmpty(mOperabilityID))     Params += "I~N~Operability~" + mOperabilityID + ";";
            if (!String.IsNullOrEmpty(mLoggingID))         Params += "I~N~Logging~"     + mLoggingID + ";";

            //if (!String.IsNullOrEmpty(mLogType.Value))     Params += "I~S~LogType~"     + mLogType.Value + ";";
            //if (!String.IsNullOrEmpty(mLoggedAt.Value))    Params += "I~S~LoggedAt~"    + mLoggedAt.Value + ";";
            //if (!String.IsNullOrEmpty(mLoggedBy.Value))    Params += "I~S~LoggedBy~"    + mLoggedBy.Value + ";";
            //if (!String.IsNullOrEmpty(mLogModule.Value))   Params += "I~S~LogModule~"   + mLogModule.Value + ";";

            if (!String.IsNullOrEmpty(mSystemID))          Params += "I~N~SystemID~"    + mSystemID + ";";
            if (!String.IsNullOrEmpty(mParentID))          Params += "I~N~ParentID~"    + mParentID + ";";
            if (!String.IsNullOrEmpty(mSystemID))          Params += "I~N~OwnerID~"     + mOwnerID + ";";
            if (!String.IsNullOrEmpty(mOwnerID))           Params += "I~N~VenueID~"     + mVenueID + ";";
            if (!String.IsNullOrEmpty(mMediaID))           Params += "I~N~MediaID~"     + mMediaID + ";";
            if (!String.IsNullOrEmpty(mScheduleID))        Params += "I~N~ScheduleID~"  + mScheduleID + ";";
            if (!String.IsNullOrEmpty(mSelected))          Params += "I~N~Selected~"    + mSelected + ";";
            if (!String.IsNullOrEmpty(mSequence))          Params += "I~N~Sequence~"    + mSequence + ";";
            return Params;
        }
        public void UpdateProperty()
        {
            if (!mNewEntry)
            {
                mDescription.Update();
                mEntityType.Update();
                mEntityState.Update();
                mSharing.Update();
                mShareState.Update();
                mOperability.Update();
                mLogging.Update();
                mLogType.Update();
                mLoggedAt.Update();
                mLoggedBy.Update();
                mLogModule.Update();
                mVenue.Update();
                mDescription.Update();
            }
        }


        public void Find(string Column, string Value)
        {
        }

        public void Reset() { Reset(EntityType); }
        public void Reset(string EntityType)
        {
            EntityPK  = "";
            mNewEntry = true;
            mChanged  = false;

            EntityTypeID       = EnumCache.Instance.getTypeId   ("Entity", EntityType);
            EntityStateID      = EnumCache.Instance.getStateId  ("Entity", "Active");
            SharingID          = EnumCache.Instance.getTypeId   ("Sharing", "Never");
            ShareStateID       = EnumCache.Instance.getStateId  ("Sharing", "Complete");

            //if (shell.Instance != null)
            //{
            //    if (shell.Instance.Valid)
            //    {
            //        if (shell.Instance.ComputerID != null) LoggedAt  = shell.Instance.ComputerID.ToString();
            //        if (shell.Instance.WorkerID != null)   LoggedBy  = shell.Instance.WorkerID.ToString();
            //        if (shell.Instance.ModuleID != null)   LogModule = shell.Instance.ModuleID.ToString();
            //        if (shell.Instance != null)            VenueID   = shell.Instance.VenueID;
            //    }
            //}

            mEntityType.Reset();
            mEntityState.Reset();
            mDescription.Reset();

        }

        public bool Changed
        {
            get { return mChanged ||  mVenue.Changed() || mDescription.Changed() || mEntityType.Changed() || mEntityState.Changed() || mSharing.Changed() || mShareState.Changed() || mOperability.Changed() || mLogging.Changed() || mLogType.Changed() || mLoggedAt.Changed() || mLoggedBy.Changed() || mLogModule.Changed(); }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(mEntityPK);
        }

        public int intEntityPk()
        {
            return IsValid() ? Convert.ToInt32(mEntityPK) : 0;
        }

    }

}
