using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlCompany
    {

        private string     mConnectionString = "";
        private string     mDataError        = "";
        private bool       mChanged;             public bool     Changed        { get { return mChanged; }                          set { mChanged = value; } }


        private dlEntity   mEntity;              public dlEntity Entity         { get { return mEntity; }                           set { mEntity = value; } }
                                                 public string   CompanyPK      { get { return mEntity.EntityPK; }                  set { mEntity.EntityPK = value; } }
                                                 public string   Description    { get { return mEntity.Description; }               set { mEntity.Description = value; } }   
                                                 public bool     NewEntry       { get { return mEntity.NewEntry; }                  set { mEntity.NewEntry = value; } }

        private nxProperty mCompanyType;         public string CompanyType      { get { return mCompanyType.Value; }                set { mCompanyType.Value = value;                                                       mCompanyTypeID      = EnumCache.Instance.getTypeFromDesc  ("Entity.Company",  mCompanyType.Value); } }
        private string     mCompanyTypeID = "";  public string CompanyTypeID    { get { return mCompanyTypeID; }                    set { if (!value.Equals(mCompanyTypeID)) mChanged = true; mCompanyTypeID = value;       mCompanyType.Value  = EnumCache.Instance.getDescFromId    (                   mCompanyTypeID); } }
        private nxProperty mCompanyState;        public string CompanyState     { get { return mCompanyState.Value; }               set { mCompanyState.Value = value;                                                      mCompanyStateID     = EnumCache.Instance.getStateFromDesc ("Entity.Company",  mCompanyState.Value); } }
        private string     mCompanyStateID = ""; public string CompanyStateID   { get { return mCompanyStateID; }                   set { if (!value.Equals(mCompanyStateID)) mChanged = true; mCompanyStateID = value;     mCompanyState.Value = EnumCache.Instance.getDescFromId    (                   mCompanyStateID); } }

        private nxProperty mCompanyNo;           public string CompanyNo        { get { return (mCompanyNo.ValueInt.ToString()); }  set { if (!value.Equals(mCompanyNo.ValueInt.ToString())) mChanged = true;               mCompanyNo.Value = value; } }

        private nxProperty mContact;             public string Contact          { get { return mContact.Value; }                    set { mContact.Value = value;                                                           mContactID      = EnumCache.Instance.getTypeFromDesc("Contact",  mContact.Value); } }
        private string     mContactID = "";      public string ContactID        { get { return mContactID; }                        set { if (!value.Equals(mContactID)) mChanged = true;   mContactID = value;             mContact.Value  = EnumCache.Instance.getDescFromId(              mContactID); } }

        private nxProperty mAddress;             public string Address          { get { return mAddress.Value; }                    set { mAddress.Value = value; } }
        private string     mAddressID = "";      public string AddressID        { get { return mAddressID; }                        set { mAddressID = value; } }

        private nxProperty mPhone;               public string Phone            { get { return mPhone.Value; }                      set { mPhone.Value = value; } }
        private string     mPhoneID = "";        public string PhoneID          { get { return mPhoneID; }                          set { mPhoneID = value; } }

        private nxProperty mProperty;            public string Property         { get { return mProperty.Value; }                   set { if (!value.Equals(mProperty.Value)) mChanged = true; mProperty.Value = value; } }


        public dlCompany() 
        {
            mEntity = new dlEntity("Company");
            if (nexus.common.core.shell.Instance != null) Create(setting.ConnectionString);
        }
        public dlCompany(string ConnectionString)
        {
            mEntity = new dlEntity("Company");
            Create(ConnectionString);
        }
        public void Dispose() { }

        public void Create(string ConnectionString)
        {
            mConnectionString = ConnectionString;
            mEntity.Create(mConnectionString);

            mCompanyType    = new nxProperty("Company", "CompanyType");
            mCompanyState   = new nxProperty("Company", "CompanyState");
            mCompanyNo      = new nxProperty("Company", "CompanyNo");
            mContact        = new nxProperty("Company", "Contact");
            mAddress        = new nxProperty("Company", "Address");
            mPhone          = new nxProperty("Company", "Phone");
            mProperty       = new nxProperty("Company", "Property");

            Reset("Venue");
        }
        public bool Open(string CompanyPK)
        {
            using (SQLServer DB = new SQLServer(mConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.lst_Company", "I~S~CompanyPK~" + CompanyPK);
                if (DT.Rows.Count > 0) LoadFromRow(DT.Rows[0]);
                mDataError = DB.DataError;
            }

            mChanged = false;

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {
            if (Row != null)
            {
                mEntity.Open(Row["CompanyPK"].ToString());

                CompanyTypeID      = Row["CompanyTypeID"].ToString();
                CompanyStateID     = Row["CompanyStateID"].ToString();
                mContactID         = Row["ContactID"].ToString();

                mCompanyNo.Value   = Row["CompanyNo"].ToString();
                mProperty.Value    = Row["Property"].ToString();

                mAddress.Value     = Row["Address"].ToString();
                mAddressID         = Row["AddressID"].ToString();
                mPhone.Value       = Row["Phone"].ToString();
                mPhoneID           = Row["PhoneID"].ToString();
            }
        }

        public bool Update()
        {

            string Params = "";
            if (!mEntity.NewEntry) Params = "I~S~CompanyPK~" + mEntity.EntityPK + ";";
            Params += "I~S~CompanyType~"  + mCompanyTypeID + ";";
            Params += "I~S~CompanyState~" + mCompanyStateID + ";";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            mEntity.EntityPK = DB.ExecLookup("loc.mod_Company", Params + getParams()).ToString();
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mEntity.NewEntry)
                {
                    mCompanyType.Update();
                    mCompanyState.Update();
                    mCompanyNo.Update();
                    mContact.Update();
                    mProperty.Update();
                    mAddress.Update();
                    mPhone.Update();
                }
                Open(mEntity.EntityPK);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Company", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mCompanyNo.Value))  Params += "I~S~CompanyNo~"    + mCompanyNo.Value  + ";";
            if (!String.IsNullOrEmpty(mContactID))        Params += "I~S~Contact~"      + mContactID        + ";";
            if (!String.IsNullOrEmpty(mAddressID))        Params += "I~S~AddressID~"    + mAddressID        + ";";
            if (!String.IsNullOrEmpty(mPhoneID))          Params += "I~S~PhoneID~"      + mPhoneID          + ";";
            if (!String.IsNullOrEmpty(mProperty.Value))   Params += "I~S~Property~"     + mProperty.Value   + ";";
            Params += mEntity.getParams();
            return Params;
        }


        public void Create(string ConnectionString, string MAC) {  }


        public void Reset() { Reset(CompanyType); }

        public void Reset(string CompanyType)
        {
            if (mEntity == null)
                mEntity = new dlEntity("Company");
            else
                mEntity.Reset("Company");

            mCompanyType.Reset();
            mCompanyState.Reset();
            mCompanyNo.Reset();
            mContact.Reset();
            mProperty.Reset();

            CompanyTypeID  = EnumCache.Instance.getTypeId  ("Entity.Company", CompanyType);
            CompanyStateID = EnumCache.Instance.getStateId ("Entity.Company", "Active");

            if (mEntity == null) 
                mEntity = new dlEntity("Company");

            mEntity.Reset("Company");
        }

        public override string ToString()
        {
            return mEntity.Description;
        }

    }
}
