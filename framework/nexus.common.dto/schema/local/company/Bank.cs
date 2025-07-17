using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlBank
    {

        private string     mConnectionString = "";
        private string     mDataError        = "";

        private dlCompany  mCompany;           public dlCompany Company       { get { return mCompany; }                       set { mCompany = value; } }
                                               public string BankPK           { get { return mCompany.CompanyPK; }             set { mCompany.CompanyPK = value; } }
                                               public string CompanyNo        { get { return mCompany.CompanyNo; }             set { mCompany.CompanyNo = value; } }
                                               public string Description      { get { return mCompany.Description; }           set { mCompany.Description = value; } }
                                               public string Contact          { get { return mCompany.Contact; }               set { mCompany.Contact = value;   } }
                                               public string ContactID        { get { return mCompany.ContactID; }             set { mCompany.ContactID = value;   } }
                                               public string Address          { get { return mCompany.Address; }               set { mCompany.Address = value; } }
                                               public string AddressID        { get { return mCompany.AddressID; }             set { mCompany.AddressID = value; } }
                                               public string Phone            { get { return mCompany.Phone; }                 set { mCompany.Phone = value; } }
                                               public string PhoneID          { get { return mCompany.PhoneID; }               set { mCompany.PhoneID = value; } }
                                               public string Property         { get { return mCompany.Property; }              set { mCompany.Property = value; } }
        
        private nxProperty mBankType;          public string  BankType       { get { return mBankType.Value; }                 set { mBankType.Value = value;    mBankTypeID       = EnumCache.Instance.getTypeFromDesc  ("Entity.Company.Bank",  mBankType.Value); } }
        private string     mBankTypeID;        public string  BankTypeID     { get { return mBankTypeID; }                     set { mBankTypeID = value;        mBankType.Value   = EnumCache.Instance.getDescFromId    (                        mBankTypeID); } }
        private nxProperty mBankState;         public string  BankState      { get { return mBankState.Value; }                set { mBankState.Value = value;   mBankStateID      = EnumCache.Instance.getStateFromDesc ("Entity.Company",       mBankState.Value); } }
        private string     mBankStateID;       public string  BankStateID    { get { return mBankStateID; }                    set { mBankStateID = value;       mBankState.Value  = EnumCache.Instance.getDescFromId    (                        mBankStateID); } }

        private nxProperty mManager;           public string  Manager        { get { return mManager.Value; }                  set { mManager.Value = value; } }
        private string     mManagerID;         public string  ManagerID      { get { return mManagerID; }                      set { mManagerID = value; } }
        private nxProperty mBranchNumber;      public string  BranchNumber   { get { return mBranchNumber.Value; }             set { mBranchNumber.Value = value; } }

        //private Windows.UI.Xaml.Media.Imaging.BitmapImage mBankLogo;

        private bool mHasLogo = false;

        public dlBank() 
        {
            if (nexus.common.core.shell.Instance != null)
            {
                mCompany = new dlCompany(setting.ConnectionString);
                Create(setting.ConnectionString);
            }
        }
        public dlBank(string ConnectionString)
        {
            mCompany = new dlCompany(ConnectionString);
            Create(ConnectionString);
        }
        public void Dispose() { }

        public void Create(string ConnectionString)
        {
            mConnectionString = ConnectionString;

            mBankType     = new nxProperty("Bank",    "BankType");
            mBankState    = new nxProperty("Bank",    "BankState");
            mBranchNumber = new nxProperty("Bank",    "BranchNumber");
            mManager      = new nxProperty("Bank",    "Manager");

            Reset("Branch");
        }
        public bool Open(string LocalUI = null, string SystemID = null)
        {
            SQLServer DB = new SQLServer(mConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Company_Bank",  ((LocalUI == null) ? "I~S~SystemID~" + SystemID :  "I~S~BankPK~" + LocalUI));
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);

        }

        public bool Find(string Description)
        {
            bool ret = false;

            SQLServer DB = new SQLServer(mConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Company_Bank", "I~S~Description~" + Description);
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

        private void LoadFromRow(DataRow Row)
        {
            mCompany.Open(Row["BankPK"].ToString());

            mBankType.Value      = Row["BankType"].ToString();
            mBankTypeID          = Row["BankTypeID"].ToString();
            mBankState.Value     = Row["BankState"].ToString();
            mBankStateID         = Row["BankStateID"].ToString();
            mManager.Value       = Row["Manager"].ToString();
            mManagerID           = Row["ManagerID"].ToString();
        }

        public bool Update()
        {

            string Params = "";
            if (!mCompany.NewEntry) Params = "I~S~BankPK~" + mCompany.CompanyPK + ";";
            Params += "I~S~BankType~"    + mBankTypeID  + ";";
            Params += "I~S~BankState~"   + mBankStateID + ";";

            SQLServer DB = new SQLServer(mConnectionString);
            mCompany.CompanyPK = DB.ExecLookup("loc.mod_Company_Bank", Params + getParams()).ToString();
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mCompany.NewEntry)
                {
                    mBankType.Update();
                    mBankState.Update();
                    mBranchNumber.Update();
                    mManager.Update();
                }
                Open(LocalUI: mCompany.CompanyPK);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Bank", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mManagerID)) Params += "I~S~ManagerID~"   + mManagerID   + ";";
            Params += mCompany.getParams();
            return Params;
        }

        //public System.Drawing.Image BankLogo
        //{
        //    get { return mBankLogo; }
        //    set
        //    {
        //        mBankLogo = value;
        //        mHasLogo = true;
        //    }
        //}
        //public bool HasLogo
        //{
        //    get { return mHasLogo; }
        //    set { mHasLogo = value; }
        //}

        public bool Changed
        {
            get { return mCompany.Changed || mBankType.Changed() || mBankState.Changed() || mBranchNumber.Changed() || mManager.Changed() || mManager.Changed() ; }
        }
        
        public void Reset(string BankType)
        {
            mCompany.Reset();
             
            mBankType.Reset();
            mBankState.Reset();
            mBranchNumber.Reset();
            mManager.Reset();

            mCompany.Description = "The Bank";
            BankTypeID        = EnumCache.Instance.getTypeId  ("Entity.Company.Bank", BankType);
            BankStateID       = EnumCache.Instance.getStateId ("Entity.Company",      "Active");
        }

        public override string ToString()
        {
            return mCompany.Description;
        }
    }
}
