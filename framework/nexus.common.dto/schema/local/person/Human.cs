using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlHuman 
    {
        private string     mConnectionString = "";
        private string     mDataError = "";    public string   DataError       { get { return mDataError; }             set { mDataError = value; } }

        private dlEntity   mEntity;            public dlEntity Entity          { get { return mEntity; }                set { mEntity = value; } }
                                               public string   HumanPK         { get { return mEntity.EntityPK; }       set { mEntity.EntityPK = value; } }
                                               public bool     NewEntry        { get { return mEntity.NewEntry; }       set { mEntity.NewEntry = value; } }

        private nxProperty mHumanType;         public string   HumanType       { get { return mHumanType.Value; }       set { mHumanType.Value = value;  mHumanTypeID      = EnumCache.Instance.getTypeFromDesc  ("Entity.Human", mHumanType.Value); } }
        private string     mHumanTypeID;       public string   HumanTypeID     { get { return mHumanTypeID; }           set { mHumanTypeID = value;      mHumanType.Value  = EnumCache.Instance.getDescFromId    (                mHumanTypeID); } }

        private nxProperty mHumanState;        public string   HumanState      { get { return mHumanState.Value; }      set { mHumanState.Value = value; mHumanStateID     = EnumCache.Instance.getStateFromDesc ("Entity.Human", mHumanState.Value); } }
        private string     mHumanStateID;      public string   HumanStateID    { get { return mHumanStateID; }          set { mHumanStateID = value;     mHumanState.Value = EnumCache.Instance.getDescFromId    (                mHumanStateID); } }

        private nxProperty mFirstName;         public string   FirstName       { get { return mFirstName.Value; }       set { mFirstName.Value = value; } }
        private nxProperty mOtherName;         public string   OtherName       { get { return mOtherName.Value; }       set { mOtherName.Value = value; } }
        private nxProperty mLastName;          public string   LastName        { get { return mLastName.Value; }        set { mLastName.Value = value; } }
        private nxProperty mNickName;          public string   NickName        { get { return mNickName.Value; }        set { mNickName.Value = value; } }
        private nxProperty mUseNickName;       public bool     UseNickName     { get { return mUseNickName.ValueBool; } set { mUseNickName.Value = (value ? 1 : 0 ).ToString() ; } }
        private nxProperty mBirthdate;         public string   Birthdate       { get { return mBirthdate.Value; }       set { mBirthdate.Value = value; } }

        private nxProperty mGender;            
        private string     mGenderID;          public string   GenderID        { get { return mGenderID; }              set { mGenderID = value;          mGender.Value      = EnumCache.Instance.getDescFromId   (                           mGenderID); } }

        private nxProperty mTitle;             public string   Title           { get { return mTitle.Value; }           set { mTitle.Value = value;       mTitleID           = EnumCache.Instance.getTypeFromDesc ("Entity.Human.Title",      mTitle.Value); } }
        private string     mTitleID;           public string   TitleID         { get { return mTitleID; }               set { mTitleID = value;           mTitle.Value       = EnumCache.Instance.getDescFromId   (                           mTitleID); } }

        private nxProperty mNextOfKin;         public string   NextOfKin       { get { return mNextOfKin.Value; }       set { mNextOfKin.Value = value;   mNextOfKinID       = EnumCache.Instance.getTypeFromDesc ("Entity.Human.NextOfKin",  mNextOfKin.Value); } }
        private string     mNextOfKinID;       public string   NextOfKinID     { get { return mNextOfKinID; }           set { mNextOfKinID = value;       mNextOfKin.Value   = EnumCache.Instance.getDescFromId   (                           mNextOfKinID); } }

        private nxProperty mAddress;           public string   Address         { get { return mAddress.Value; }         set { mAddress.Value = value;   } }
        private string     mAddressID;         public string   AddressID       { get { return mAddressID; }             set { mAddressID = value;       } }

        private nxProperty mPhone;             public string   Phone           { get { return mPhone.Value; }           set { mPhone.Value = value;   } }
        private string     mPhoneID;           public string   PhoneID         { get { return mPhoneID; }               set { mPhoneID = value;       } }

        private nxProperty mMobile;            public string   Mobile          { get { return mMobile.Value; }          set { mMobile.Value = value;   } }
        private string     mMobileID;          public string   MobileID        { get { return mMobileID; }              set { mMobileID = value;       } }

        private nxProperty mMaritalState;      public string   MaritalState    { get { return mMaritalState.Value; }    set { mMaritalState.Value = value; mMaritalStateID     = EnumCache.Instance.getStateFromDesc ("Entity.Humann.Marital", mMaritalState.Value); } }
        private string     mMaritalStateID;    public string   MaritalStateID  { get { return mMaritalStateID; }        set { mMaritalStateID = value;     mMaritalState.Value = EnumCache.Instance.getDescFromId    (                         mMaritalStateID); } }

        private string     mFaceID;            public string   FaceID          { get { return mFaceID; }                set { mFaceID = value;       } }


        private bool   _Changed = false;
        public  string ParentType   = "";
        public  string ParentTypeId = "";

        public event         OnEntryStatusChangeEventHandler OnEntryStatusChange;
        public delegate void OnEntryStatusChangeEventHandler(bool Status);

        public dlHuman() 
        {
            mEntity = new dlEntity("Human");
            if (shell.Instance != null)
                Create(setting.ConnectionString);
        }
        public dlHuman(string ConnectionString)
        {
            mEntity  = new dlEntity("Human");
            Create(ConnectionString);
        }

        public void Create(string ConnectionString)
        {
            mConnectionString = ConnectionString;
            mEntity.Create(mConnectionString);

            mHumanType    = new nxProperty("Human", "HumanType");
            mHumanState   = new nxProperty("Human", "HumanState");
            
            mFirstName    = new nxProperty("Human", "FirstName");
            mOtherName    = new nxProperty("Human", "OtherName");
            mLastName     = new nxProperty("Human", "LastName");
            mNickName     = new nxProperty("Human", "NickName");
            mUseNickName  = new nxProperty("Human", "UseNickName");
            mBirthdate    = new nxProperty("Human", "Birthdate");
            mGender       = new nxProperty("Human", "Gender");
            mTitle        = new nxProperty("Human", "Title");
            mNextOfKin    = new nxProperty("Human", "NextOfKin");
            mAddress      = new nxProperty("Human", "Address");
            mPhone        = new nxProperty("Human", "Phone");
            mMobile       = new nxProperty("Human", "Mobile");
            mMaritalState = new nxProperty("Human", "MaritalState");

            Reset(); 
        }


        public bool Open(int HumanPK, int HumanType)    { return Open( HumanPK.ToString(), HumanType.ToString() ); }
        public bool Open(int    HumanPK) { return Open(HumanPK.ToString()); }
        public bool Open(string HumanPK) { return Open(HumanPK, HumanType); }
        public bool Open(string HumanPK, string HumanType)
        {

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.lst_Human", "I~S~HumanPK~" + HumanPK);
                if (DT.Rows.Count > 0)
                    LoadFromRow(DT.Rows[0]);
            }

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {

            mEntity.Open(Row["HumanPk"].ToString());

            mHumanTypeID       = Row["HumanTypeID"].ToString();
            mHumanStateID      = Row["HumanStateID"].ToString();
            mGenderID          = Row["GenderID"].ToString();
            mTitleID           = Row["TitleID"].ToString();

            mHumanType.Create   ( Row["HumanType"].ToString());
            mHumanState.Create  ( Row["HumanState"].ToString());
            mGender.Create      ( Row["Gender"].ToString());
            mTitle.Create       ( Row["Title"].ToString());
            mFirstName.Create   ( Row["FirstName"].ToString());
            mOtherName.Create   ( Row["OtherName"].ToString());
            mLastName.Create    ( Row["LastName"].ToString());
            mNickName.Create    ( Row["NickName"].ToString());
            mUseNickName.Create ( Row["UseNickName"].ToString());

            //mFaceID    =        ( Row["FaceId"].ToString());
            ParentType =        ( Row["ParentType"].ToString());


            DateTime dob;
            string DOB = Row["Birthdate"].ToString();
            if (string.IsNullOrEmpty(DOB))
                mBirthdate.Create( DateTime.Now.AddYears(-16).ToString("dd MMM, yyyy"));
            else
            {
                if (DateTime.TryParse(Row["Birthdate"].ToString(), out dob))
                {
                    if (DateTime.Now.Subtract(dob).TotalDays > (365 * 150)) dob = DateTime.Now.AddYears(-150);
                    mBirthdate.Create( dob.ToString("dd MMM, yyyy"));
                }
                else
                    mBirthdate.Create( DateTime.Now.AddYears(-16).ToString("dd MMM, yyyy"));
            }
            _Changed = false;
        }

        public bool Find(string Description)
        {
            bool ret = false;

            SQLServer DB = new SQLServer(mConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Human", "I~S~Fullname~" + Description);
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
            if (Changed)
            {
                string EntityPK = "";
                string Params   = "";
                if (ParentType.Equals("Patron"))
                {
                    if (!mEntity.NewEntry) Params = "I~S~PatronPK~" + mEntity.EntityPK + ";";
                    if (!String.IsNullOrEmpty(mHumanTypeID))  Params += "I~I~MemberType~"  + mHumanTypeID + ";";
                    if (!String.IsNullOrEmpty(mHumanStateID)) Params += "I~I~MemberState~" + mHumanStateID + ";";
                }
                else
                {
                    if (!mEntity.NewEntry) Params = "I~S~WorkerPK~" + mEntity.EntityPK + ";";
                    if (!String.IsNullOrEmpty(mHumanTypeID))  Params += "I~I~WorkerType~"   + mHumanTypeID + ";";
                    if (!String.IsNullOrEmpty(mHumanStateID)) Params += "I~I~WorkerState~" + mHumanStateID + ";";
                }


                mFirstName.Value = helpers.TitleCase( mFirstName.Value );
                mOtherName.Value = helpers.TitleCase( mOtherName.Value );
                mLastName.Value  = helpers.TitleCase( mLastName .Value );
                mNickName.Value  = helpers.TitleCase( mNickName .Value );

                mEntity.Description = FullName;

                using (SQLServer DB = new SQLServer(setting.ConnectionString))
                {
                    EntityPK = DB.ExecLookup("loc.mod_Human_" + ParentType, Params + getParams()).ToString();
                    mDataError = DB.DataError;
                }

                if (String.IsNullOrEmpty(mDataError))
                {
                    if (!mEntity.NewEntry)
                    {
                        mFirstName.Update();
                        mLastName.Update();
                        mOtherName.Update();
                        mNickName.Update();
                        mUseNickName.Update();
                        mBirthdate.Update();
                        mTitle.Update();
                        mGender.Update();
                    }
                    Open(EntityPK);
                }
                else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Human", "UpdateError"), mDataError); }
            }
            return String.IsNullOrEmpty(mDataError);
        }

        public string getParams()
        {
            string Params = " ";

            if (!String.IsNullOrEmpty(mGenderID))          Params += "I~I~Gender~"      + mGenderID + ";";
            if (!String.IsNullOrEmpty(mTitleID))           Params += "I~I~Title~"       + mTitleID + ";";
            if (!String.IsNullOrEmpty(mFirstName.Value))   Params += "I~S~FirstName~"   + mFirstName.Value  + ";";
            if (!String.IsNullOrEmpty(mOtherName.Value))   Params += "I~S~OtherName~"   + mOtherName.Value + ";";
            if (!String.IsNullOrEmpty(mLastName.Value))    Params += "I~S~LastName~"    + mLastName.Value  + ";";
            if (!String.IsNullOrEmpty(mNickName.Value))    Params += "I~S~NickName~"    + mNickName.Value  + ";";
            if (!String.IsNullOrEmpty(mUseNickName.Value)) Params += "I~S~UseNickName~" + mUseNickName.Value + ";";
            if (!String.IsNullOrEmpty(mFaceID))            Params += "I~S~FaceId~"      + mFaceID + ";";

            DateTime dob = mBirthdate.ValueDate;
            if (DateTime.Now.Subtract(dob).TotalDays > (365 * 150)) dob = DateTime.Now.AddYears(-150);
            Params += "I~D~Birthdate~" + DOB + ";";
            Params += mEntity.getParams();
            return Params;
        }


        public void Dispose() { }

        public void Reset()
        {
            ParentType = "Patron";

            if (mEntity == null)
                mEntity = new dlEntity("Human");
            else
                mEntity.Reset("Human");

            HumanTypeID   = EnumCache.Instance.getTypeId    ("Entity.Human",        "Member");
            HumanStateID  = EnumCache.Instance.getStateId   ("Entity.Human",        "Active");
            GenderID      = EnumCache.Instance.getTypeId    ("Entity.Human.Gender", "Male");
            TitleID       = EnumCache.Instance.getTypeId    ("Entity.Human.Title",  "Mr");

            mFirstName.Reset();
            mOtherName.Reset();
            mLastName.Reset();
            mNickName.Reset();
            mUseNickName.Reset();
            mBirthdate.Reset();
            mBirthdate.Value = DateTime.Now.AddYears(-16).ToString("dd MMM, yyyy");

        }

        public override string ToString()
        {
            return FullName;
        }
        public bool IsValid()
        {
            return mEntity.IsValid();
        }


        public bool ValidName
        {
            get
            {
                DateTime dout;
                return (!mFirstName.Value.Equals("")) && (!mLastName.Value.Equals("")) && (DateTime.TryParse(mBirthdate.Value, out dout));
            }
        }
        public bool Changed
        {
            get { return _Changed || mFirstName.Changed() || mLastName.Changed() || mOtherName.Changed() || mNickName.Changed() || mBirthdate.Changed() || mUseNickName.Changed() || mGender.Changed() || mTitle.Changed(); }
            set { _Changed = value; }
        }
        private void TestName()
        {
            //if (mUsePrefName.Value.Equals("True") & !String.IsNullOrEmpty(mUsePrefName.Value))
            //    FullName = mPreferred.Value + " " + mMiddleName.Value + " " +  mLastName.Value;
            //else
            FullName = mFirstName.Value + " " + mOtherName.Value + " " + mLastName.Value;
        }

        public string Gender
        {
            get { return mGender.Value; }
            set
            {
                switch (value.ToUpper())
                {
                    case "M": case "MALE":   mGender.Value = "Male";   break;
                    case "F": case "FEMALE": mGender.Value = "FeMale"; break;
                    default:                 mGender.Value = value;    break;
                }
                mGenderID = EnumCache.Instance.getTypeFromDesc("Gender", mGender.Value);
            }
        }

        public string FullName
        {
            get { return mFirstName.Value + " " + mOtherName.Value + " " + mLastName.Value; }
            set
            {
                string FirstName = "";
                string OtherName = "";
                string LastName = "";

                Array Arr = value.Trim().Split(new char[] { ' ' });
                switch (Arr.GetLength(0))
                {
                    case 0:
                        FirstName = "";
                        OtherName = "";
                        LastName = "";
                        break;
                    case 1:
                        FirstName = Arr.GetValue(0).ToString();
                        OtherName = "";
                        LastName = "";
                        break;
                    case 2:
                        FirstName = Arr.GetValue(0).ToString();
                        OtherName = "";
                        LastName = Arr.GetValue(1).ToString();
                        break;
                    case 3:
                        FirstName = Arr.GetValue(0).ToString();
                        OtherName = Arr.GetValue(1).ToString();
                        LastName = Arr.GetValue(2).ToString();
                        break;

                    default:
                        FirstName = Arr.GetValue(0).ToString();
                        LastName = Arr.GetValue(Arr.GetLength(0) - 1).ToString();

                        for (int i = 1; i <= Arr.GetLength(0) - 2; i++)
                            OtherName = OtherName + " " + Arr.GetValue(i).ToString();
                        break;
                }
                mFirstName.Value = helpers.TitleCase(FirstName);
                mOtherName.Value = helpers.TitleCase(OtherName);
                mLastName.Value = helpers.TitleCase(LastName);

                if (OnEntryStatusChange != null) OnEntryStatusChange(ValidName);
            }
        }
        public string Age
        {
            get
            {
                string age = "";
                DateTime dout;
                if (DateTime.TryParse(mBirthdate.Value, out dout))
                    age = ((DateTime.Now.Subtract(dout).TotalDays) / 365).ToString("0.0");
                return age;
            }
        }

        public string DOB
        {
            get
            {
                DateTime dout;
                DateTime.TryParse(mBirthdate.Value, out dout); 

                if (DateTime.Now.Subtract(dout).TotalDays > (365 * 150))
                    dout = DateTime.Now.AddYears(-150);

                return dout.ToString("dd MMM, yyyy"); 
            }
        }

        public bool Delete()
        {
            //string SQL = "DECLARE @HumanPK int; SET @HumanPK=" + HumanPK + " ";
            //SQL += "DECLARE @HumanState int; SET @HumanState=" + EnumCache.Instance.getTypeId("Huamn", "Deleted") + " ";
            //SQL += "update [loc].[Computer] set ComputerState=@ComputerState WHERE ComputerPK=@ComputerPK";

            //SQLServer DB = new SQLServer(setting.ConnectionString);
            //DB.ExecNonQuery(SQL);
            //mDataError = DB.DataError;
            //DB.Dispose();
            //DB = null;

            //nexus.common.cache.EnumCache.Instance.Create(); // reload the cache

            return String.IsNullOrEmpty(mDataError);
        }

        public DataTable getTable(string HumanPK, string SystemId, string FullName, int LimitRows)
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable ret = DB.GetDataTable("loc.lst_Human", "");
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            return ret;
        }

    }
}
