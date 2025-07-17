using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlCard
    {
        private bool       mNewEntry = true;   public bool     NewEntry        { get { return mNewEntry; } }

        private string     mConnectionString = "";
        private string     mDataError = "";    public string   DataError       { get { return mDataError; }   set { mDataError = value; } }

        private string     mCardPK = "0";      public string   CardPK          { get { return mCardPK; }      set { mCardPK = value; } }

        private string     mSystemID = "0";    public string   SystemID        { get { return mSystemID; } }
        private string     mVenueID = "0";     public string   VenueID         { get { return mVenueID; }     set { mVenueID = value; } }

        private nxProperty mCardType;          public string   CardType        { get { return mCardType.Value; }        set { mCardType.Value = value;  mCardTypeID      = EnumCache.Instance.getTypeFromDesc  ("Card", mCardType.Value); } }
        private string     mCardTypeID;        public string   CardTypeID      { get { return mCardTypeID; }            set { mCardTypeID = value;      mCardType.Value  = EnumCache.Instance.getDescFromId    (        mCardTypeID); } }

        private nxProperty mCardState;         public string   CardState       { get { return mCardState.Value; }       set { mCardState.Value = value; mCardStateID     = EnumCache.Instance.getStateFromDesc ("Card", mCardState.Value); } }
        private string     mCardStateID;       public string   CardStateID     { get { return mCardStateID; }           set { mCardStateID = value;     mCardState.Value = EnumCache.Instance.getDescFromId    (         mCardStateID); } }

        private nxProperty mCardNo;            public string   CardNo          { get { return mCardNo.Value; }          set { mCardNo.Value = value; } }
        private nxProperty mPinNo;             public string   PinNo           { get { return mPinNo.Value; }           set { mPinNo.Value = value; } }
        private nxProperty mPrintDate;         public string   PrintDate       { get { return mPrintDate.Value; }       set { mPrintDate.Value = value; } }
        private nxProperty mPrints;            public string   Prints          { get { return mPrints.Value; }          set { mPrints.Value = value; } }

        private nxProperty mLastUsed;          public string   LastUsed        { get { return mLastUsed.Value; }        set { mLastUsed.Value = value; } }
        private nxProperty mLastUsedAt;        public string   LastUsedAt      { get { return mLastUsedAt.Value; }      set { mLastUsedAt.Value = value; } }
        private string     mLastUsedAtID;      public string   LastUsedAtID    { get { return mLastUsedAtID; }          set { mLastUsedAtID = value; } }

        private nxProperty mHuman;             public string   Human           { get { return mHuman.Value; }           set { mHuman.Value = value;  } }
        private string     mHumanID;           public string   HumanID         { get { return mHumanID; }               set { mHumanID = value; } }

        private nxProperty mCohort;            public string   Cohort          { get { return mCohort.Value; }          set { mCohort.Value = value; } }
        private string     mCohortID;          public string   CohortID        { get { return mCohortID; }              set { mCohortID = value; } }

        private string     mAccountID;         public string   AccountID       { get { return mAccountID; }             set { mAccountID = value; } }

        private nxProperty mAccountType;       public string   AccountType     { get { return mAccountType.Value; }     set { mAccountType.Value = value;  mAccountTypeID      = EnumCache.Instance.getTypeFromDesc  ("Account", mAccountType.Value); } }
        private string     mAccountTypeID;     public string   AccountTypeID   { get { return mAccountTypeID; }         set { mAccountTypeID = value;      mAccountType.Value  = EnumCache.Instance.getDescFromId    (           mAccountTypeID); } }

        private nxProperty mAccountState;      public string   AccountState    { get { return mAccountState.Value; }    set { mAccountState.Value = value; mAccountStateID     = EnumCache.Instance.getStateFromDesc ("Account", mAccountState.Value); } }
        private string     mAccountStateID;    public string   AccountStateID  { get { return mAccountStateID; }        set { mAccountStateID = value;     mAccountState.Value = EnumCache.Instance.getDescFromId    (           mAccountStateID); } }

        
        public event         OnEntryStatusChangeEventHandler OnEntryStatusChange;    public delegate void OnEntryStatusChangeEventHandler(bool Status);

        public dlCard() 
        {
            mCardType     = new nxProperty("Card", "CardType");
            mCardState    = new nxProperty("Card", "CardState");
            
            mCardNo       = new nxProperty("Card", "CardNo");
            mPinNo        = new nxProperty("Card", "PinNo");
            mPrintDate    = new nxProperty("Card", "PrintDate");
            mPrints       = new nxProperty("Card", "Prints");

            mLastUsed     = new nxProperty("Card", "LastUsed");
            mLastUsedAt   = new nxProperty("Card", "LastUsedAt");

            mHuman        = new nxProperty("Card", "Human");
            mCohort       = new nxProperty("Card", "Cohort");

            mAccountType  = new nxProperty("Card", "AccountType");
            mAccountState = new nxProperty("Card", "AccountState");

            Reset(); 
        }

        public bool Open(int    CardPK) { return Open(CardPK.ToString()); }
        public bool Open(string CardPK)
        {
            mDataError = "";

            mCardPK = CardPK.ToString() ;

            mAccountID = "";
            mCardNo.Value = "";

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.lst_Card", "I~S~CardPK~" + mCardPK);
                if (DT.Rows.Count > 0) LoadFromRow(DT.Rows[0]); else mDataError = "No Card Found";
            }

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {

            mCardTypeID        = Row["CardTypeID"].ToString();
            mCardStateID       = Row["CardStateID"].ToString();

            mAccountID         = Row["AccountID"].ToString();
            mAccountTypeID     = Row["AccountTypeID"].ToString();
            mAccountStateID    = Row["AccountStateID"].ToString();

            mCardNo.Value      = Row["CardNo"].ToString();
            mPinNo.Value       = Row["PinNo"].ToString();
            mPrintDate.Value   = Row["PrintDate"].ToString();
            mPrints.Value      = Row["Prints"].ToString();

            mLastUsed.Value    = Row["LastUsed"].ToString();
            mLastUsedAt.Value  = Row["LastUsedAt"].ToString();

            mHuman.Value       = Row["Human"].ToString();
            mHumanID           = Row["HumanId"].ToString();

            mCohort.Value      = Row["Tier"].ToString();
            mCohortID          = Row["CohortId"].ToString();

        }

        public bool Search(string SearchType, string SerachValue)
        {
            bool ret = false;
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params = "";
                switch (SearchType)
                {
                    case "HumanId": Params = "I~S~HumanId~"  + SerachValue; break;
                    case "Name":    Params = "I~S~Fullname~" + SerachValue; break;

                    default:
                        break;
                }

                using (DataTable DT = DB.GetDataTable("loc.lst_Card", Params))
                {
                    if (DT.Rows.Count > 0)
                    {
                        ret = true;
                        mCardPK = DT.Rows[0]["CardPk"].ToString();
                        LoadFromRow(DT.Rows[0]);
                    }
                }
            }
            return ret;
        }

        public bool Update()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);

            string Params = "";
            if (mCardPK == "0") mCardPK = String.Empty;
            if (!String.IsNullOrEmpty(mCardPK)) Params = "O~I~CardPK~"   + mCardPK + ";";

            Params += "I~S~CardNo~"   + mCardNo.Value  + ";";
            Params += "I~S~PinNo~"    + (String.IsNullOrEmpty(mPinNo.Value) ? "0000" : mPinNo.Value) + ";";

            if (!String.IsNullOrEmpty(mCardTypeID))      Params += "I~S~CardType~"     + mCardTypeID + ";";
            if (!String.IsNullOrEmpty(mCardStateID))     Params += "I~S~CardState~"    + mCardStateID + ";";

            if (!String.IsNullOrEmpty(mAccountID))       Params += "I~S~AccountId~"    + mAccountID + ";";
            //if (!String.IsNullOrEmpty(mAccountTypeID))   Params += "I~S~AccountType~"  + mAccountTypeID + ";";
            //if (!String.IsNullOrEmpty(mAccountStateID))  Params += "I~S~AccountState~" + mAccountStateID + ";";

            //if (!String.IsNullOrEmpty(mPrintDate.Value)) Params += "I~S~PrintDate~"    + mPrintDate.Value + ";";
            //if (!String.IsNullOrEmpty(mPrints.Value))    Params += "I~S~Prints~"       + mPrints.Value  + ";";
            //if (!String.IsNullOrEmpty(mLastUsed.Value))  Params += "I~S~LastUsed~"     + mLastUsed.Value + ";";
            //if (!String.IsNullOrEmpty(mLastUsedAtID))    Params += "I~S~LastUsedAt~"   + mLastUsedAtID + ";";
            if (!String.IsNullOrEmpty(mHumanID))         Params += "I~S~HumanId~"      + mHumanID + ";";
            if (!String.IsNullOrEmpty(mCohortID))        Params += "I~S~CohortId~"     + mCohortID + ";";

            mCardPK = DB.ExecLookup("loc.mod_Card", Params).ToString();

            mDataError = DB.DataError;

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mNewEntry)
                {
                    mCardType.Update();
                    mCardState.Update();

                    mHuman.Update();
                    mCohort.Update();

                    mAccountType.Update();
                    mAccountState.Update();

                    mCardNo.Update();
                    mPinNo.Update();
                    mPrints.Update();
                    mPrintDate.Update();
                    mLastUsed.Update();
                    mLastUsedAt.Update();
                }
                Open(mCardPK);
                mNewEntry = false;
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Card", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }

        public void Dispose() { }

        public void Reset()
        {
            mNewEntry     = true;

            mCardPK = "";
            mCardNo.Value = "";

            CardTypeID    = EnumCache.Instance.getTypeId    ("Card", "Member");
            CardStateID   = EnumCache.Instance.getStateId   ("Card", "Active");

            mPinNo.Reset();
            mPrintDate.Reset();
            mPrints.Reset();
            mLastUsed.Reset();

        }

        public override string ToString()
        {
            return "";
        }

        public bool Changed
        {
            get { return mCardType.Changed() || mCardState.Changed() || mAccountType.Changed() || mAccountState.Changed() || mPinNo.Changed() || mPrints.Changed() || mPrintDate.Changed() || mLastUsed.Changed() || mLastUsedAt.Changed() || mHuman.Changed() || mCohort.Changed();  }
        }
        public bool Delete()
        {
            //string SQL = "DECLARE @CardPK int; SET @CardPK=" + CardPK + " ";
            //SQL += "DECLARE @CardState int; SET @CardState=" + EnumCache.Instance.getTypeId("Huamn", "Deleted") + " ";
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
