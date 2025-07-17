using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlBiometric
    {
        private bool       mNewEntry = true;   public bool     NewEntry         { get { return mNewEntry; } }

        private string     mConnectionString = "";
        private string     mDataError = "";    public string   DataError        { get { return mDataError; }   set { mDataError = value; } }

        private string     mBiometricPK = "0"; public string   BiometricPK      { get { return mBiometricPK; } }
        private string     mSystemID = "0";    public string   SystemID         { get { return mSystemID; } }
        private string     mVenueID = "0";     public string   VenueID          { get { return mVenueID; }               set { mVenueID = value; } }

        private nxProperty mBiometricType;     public string   BiometricType    { get { return mBiometricType.Value; }   set { mBiometricType.Value = value;  mBiometricTypeID      = EnumCache.Instance.getTypeFromDesc  ("Biometric", mBiometricType.Value); } }
        private string     mBiometricTypeID;   public string   BiometricTypeID  { get { return mBiometricTypeID; }       set { mBiometricTypeID = value;      mBiometricType.Value  = EnumCache.Instance.getDescFromId    (        mBiometricTypeID); } }

        private nxProperty mBiometricState;    public string   BiometricState   { get { return mBiometricState.Value; }  set { mBiometricState.Value = value; mBiometricStateID     = EnumCache.Instance.getStateFromDesc ("Biometric", mBiometricState.Value); } }
        private string     mBiometricStateID;  public string   BiometricStateID { get { return mBiometricStateID; }      set { mBiometricStateID = value;     mBiometricState.Value = EnumCache.Instance.getDescFromId    (         mBiometricStateID); } }

        private nxProperty mSubjectID;         public string   SubjectID        { get { return mSubjectID.Value; }       set { mSubjectID.Value = value; } }
        private nxProperty mPinNo;             public string   PinNo            { get { return mPinNo.Value; }           set { mPinNo.Value = value; } }

        private nxProperty mLastUsed;          public string   LastUsed         { get { return mLastUsed.Value; }        set { mLastUsed.Value = value; } }
        private nxProperty mLastUsedAt;        public string   LastUsedAt       { get { return mLastUsedAt.Value; }      set { mLastUsedAt.Value = value; } }
        private string     mLastUsedAtID;      public string   LastUsedAtID     { get { return mLastUsedAtID; }          set { mLastUsedAtID = value; } }

        private nxProperty mHuman;             public string   Human            { get { return mHuman.Value; }           set { mHuman.Value = value;  } }
        private string     mHumanID;           public string   HumanID          { get { return mHumanID; }               set { mHumanID = value; } }

        private nxProperty mCard;              public string   Card             { get { return mCard.Value; }            set { mCard.Value = value;  } }
        private string     mCardID;            public string   CardID           { get { return mCardID; }                set { mCardID = value; } }
         
        private nxProperty mPromotion;         public string   Promotion        { get { return mPromotion.Value; }       set { mPromotion.Value = value; } }
        private string     mPromotionID;       public string   PromotionID      { get { return mPromotionID; }           set { mPromotionID = value; } }

        private string     mAccountID;         public string   AccountID        { get { return mAccountID; }             set { mAccountID = value; } }

        private nxProperty mAccountType;       public string   AccountType      { get { return mAccountType.Value; }     set { mAccountType.Value = value;  mAccountTypeID      = EnumCache.Instance.getTypeFromDesc  ("Account", mAccountType.Value); } }
        private string     mAccountTypeID;     public string   AccountTypeID    { get { return mAccountTypeID; }         set { mAccountTypeID = value;      mAccountType.Value  = EnumCache.Instance.getDescFromId    (           mAccountTypeID); } }

        private nxProperty mAccountState;      public string   AccountState     { get { return mAccountState.Value; }    set { mAccountState.Value = value; mAccountStateID     = EnumCache.Instance.getStateFromDesc ("Account", mAccountState.Value); } }
        private string     mAccountStateID;    public string   AccountStateID   { get { return mAccountStateID; }        set { mAccountStateID = value;     mAccountState.Value = EnumCache.Instance.getDescFromId    (           mAccountStateID); } }

        
        public event         OnEntryStatusChangeEventHandler OnEntryStatusChange;    public delegate void OnEntryStatusChangeEventHandler(bool Status);

        public dlBiometric() 
        {
            mBiometricType  = new nxProperty("Biometric", "BiometricType");
            mBiometricState = new nxProperty("Biometric", "BiometricState");
            
            mPinNo          = new nxProperty("Biometric", "PinNo");
            mSubjectID      = new nxProperty("Biometric", "SubjectId");

            mLastUsed       = new nxProperty("Biometric", "LastUsed");
            mLastUsedAt     = new nxProperty("Biometric", "LastUsedAt");

            mCard           = new nxProperty("Biometric", "Card");
            mHuman          = new nxProperty("Biometric", "Human");
            mPromotion      = new nxProperty("Biometric", "Promotion");

            mAccountType    = new nxProperty("Biometric", "AccountType");
            mAccountState   = new nxProperty("Biometric", "AccountState");

            Reset(); 
        }

        public bool Open(int    BiometricPK) { return Open(BiometricPK.ToString()); }
        public bool Open(string BiometricPK)
        {
            mDataError = "";
            mBiometricPK = BiometricPK.ToString() ;

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Biometric", "I~S~BiometricPK~" + mBiometricPK);
            if (DT.Rows.Count > 0) 
            {
                LoadFromRow(DT.Rows[0]);
                mNewEntry = false;
            }

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {

            mBiometricTypeID   = Row["BioTypeID"].ToString();
            mBiometricStateID  = Row["BioStateID"].ToString();

            mAccountID         = Row["AccountID"].ToString();
            mAccountTypeID     = Row["AccountTypeId"].ToString();
            mAccountStateID    = Row["AccountStateID"].ToString();

            mSubjectID.Value   = Row["SubjectId"].ToString();
            mPinNo.Value       = Row["PinNo"].ToString();

            mLastUsed.Value    = Row["LastUsed"].ToString();
            mLastUsedAt.Value  = Row["LastUsedAt"].ToString();

            mHuman.Value       = Row["Human"].ToString();
            mHumanID           = Row["HumanId"].ToString();

            mCard.Value        = Row["CardNo"].ToString();
            mCardID            = Row["CardId"].ToString();

            mPromotion.Value   = Row["Promotion"].ToString();
            mPromotionID       = Row["PromotionId"].ToString();

        }

        public bool Find(string Description)
        {
            bool ret = false;

            SQLServer DB = new SQLServer(mConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Biometric", "I~S~Fullname~" + Description);
            if (DT.Rows.Count > 0)
            {
                ret = true;
                LoadFromRow(DT.Rows[0]);
            }

            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            return ret;
        }

        public bool Update()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);

            string Params = "";

            if (!mNewEntry)
                Params = "I~I~BiometricPK~"  + mBiometricPK + ";";

            if (!String.IsNullOrEmpty(mBiometricTypeID))   Params += "I~I~BioType~"         + mBiometricTypeID + ";";
            if (!String.IsNullOrEmpty(mBiometricStateID))  Params += "I~I~BioState~"        + mBiometricStateID + ";";

            if (!String.IsNullOrEmpty(mAccountID))         Params += "I~I~AccountId~"       + mAccountID + ";";
            //if (!String.IsNullOrEmpty(mAccountTypeID))     Params += "I~I~AccountType~"     + mAccountTypeID + ";";
            //if (!String.IsNullOrEmpty(mAccountStateID))    Params += "I~I~AccountState~"    + mAccountStateID + ";";

            if (!String.IsNullOrEmpty(mSubjectID.Value))   Params += "I~S~SubjectID~"       + mSubjectID.Value  + ";";
            if (!String.IsNullOrEmpty(mPinNo.Value))       Params += "I~S~PinNo~"           + mPinNo.Value  + ";";
            if (!String.IsNullOrEmpty(mLastUsed.Value))    Params += "I~D~LastUsed~"        + mLastUsed.Value  + ";";
            if (!String.IsNullOrEmpty(mLastUsedAtID))      Params += "I~I~LastUsedAt~"      + mLastUsedAtID + ";";

            if (!String.IsNullOrEmpty(mHumanID))           Params += "I~I~HumanId~"         + mHumanID + ";";
            if (!String.IsNullOrEmpty(mCardID))            Params += "I~I~CardId~"          + mCardID + ";";
            if (!String.IsNullOrEmpty(mPromotionID))       Params += "I~I~PromotionId~"     + mPromotionID;


            mBiometricPK = DB.ExecLookup("loc.mod_Biometric", Params).ToString();

            mDataError = DB.DataError;

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mNewEntry)
                {
                    mBiometricType.Update();
                    mBiometricState.Update();

                    mHuman.Update();
                    mPromotion.Update();

                    mAccountType.Update();
                    mAccountState.Update();

                    mSubjectID.Update();
                    mPinNo.Update();
                    mLastUsed.Update();
                    mLastUsedAt.Update();
                }
                Open(mBiometricPK);
                mNewEntry = false;
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Biometric", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }

        public void Dispose() { }

        public void Reset()
        {
            BiometricTypeID    = EnumCache.Instance.getTypeId    ("Biometric", "Member");
            BiometricStateID   = EnumCache.Instance.getStateId   ("Biometric", "Active");

            mPinNo.Reset();
            mLastUsed.Reset();

        }

        public override string ToString()
        {
            return "";
        }

        public bool Changed
        {
            get { return mBiometricType.Changed() || mBiometricState.Changed() || mAccountType.Changed() || mAccountState.Changed() || mPinNo.Changed() || mLastUsed.Changed() || mLastUsedAt.Changed() || mHuman.Changed() || mPromotion.Changed();  }
        }
        public bool Delete()
        {
            //string SQL = "DECLARE @BiometricPK int; SET @BiometricPK=" + BiometricPK + " ";
            //SQL += "DECLARE @BiometricState int; SET @BiometricState=" + EnumCache.Instance.getTypeId("Huamn", "Deleted") + " ";
            //SQL += "update [loc].[Computer] set ComputerState=@ComputerState WHERE ComputerPK=@ComputerPK";

            //SQLServer DB = new SQLServer(setting.ConnectionString);
            //DB.ExecNonQuery(SQL);
            //mDataError = DB.DataError;
            //DB.Dispose();
            //DB = null;

            //nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            return String.IsNullOrEmpty(mDataError);
        }

    }
}
