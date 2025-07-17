using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlContact 
    {

        private string     mDefaultType = "";
        private string     mEntityID;           public string   EntityID         { get { return mEntityID; }               set { mEntityID  = value; } }
        private string     mContactPK = "0";    public string   ContactPK        { get { return mContactPK; }              set { mContactPK = value; } }
        private string     mDescription = "";  
        private string     mDataError;          public string   DataError        { get { return mDataError; }              set { mDataError = value; } }



        private nxProperty mContactType;        public string   ContactType      { get { return mContactType.Value; }      set { mContactType.Value = value;  mContactTypeID      = EnumCache.Instance.getTypeFromDesc  ("Contact", mContactType.Value); } }
        private string     mContactTypeID="";   public string   ContactTypeID    { get { return mContactTypeID; }          set { mContactTypeID = value;      mContactType.Value  = EnumCache.Instance.getDescFromId    (           mContactTypeID); } }

        private nxProperty mContactState;       public string   ContactState     { get { return mContactState.Value; }     set { mContactState.Value = value; mContactStateID     = EnumCache.Instance.getStateFromDesc ("Contact", mContactState.Value); } }
        private string     mContactStateID="";  public string   ContactStateID   { get { return mContactStateID; }         set { mContactStateID = value;     mContactState.Value = EnumCache.Instance.getDescFromId    (           mContactStateID); } }

        private nxProperty mContactLocation;    public string   ContactLocation  { get { return mContactLocation.Value; }  set { mContactLocation.Value = value; mContactLocationID     = EnumCache.Instance.getTypeFromDesc ("ContactLocation", mContactLocation.Value); } }
        private string mContactLocationID = ""; public string   ContactLocationID{ get { return mContactLocationID; }      set { mContactLocationID = value;     mContactLocation.Value = EnumCache.Instance.getDescFromId   (                   mContactLocationID); } }


        private nxProperty mContact1;           public string   Contact1         { get { return mContact1.Value; }         set { mContact1.Value = value; } }
        private nxProperty mContact2;           public string   Contact2         { get { return mContact2.Value; }         set { mContact2.Value = value; } }
        private nxProperty mContact3;           public string   Contact3         { get { return mContact3.Value; }         set { mContact3.Value = value; } }
        private nxProperty mDetail;             public string   Detail           { get { return mDetail.Value; }           set { mDetail.Value = value; } }

        private nxProperty mStreet;             public string   Street           { get { return mStreet.Value; }           set { mStreet.Value = value;       mStreetID           = GeoCache.Instance.getStreetIdFromDescription(mStreet.Value); } }
        private int        mStreetID;           public int      StreetID         { get { return mStreetID; }               set { mStreetID = value;           mStreet.Value       = GeoCache.Instance.getStreetFromId(mStreetID.ToString()); } }

        private nxProperty mSuburb;             public string   Suburb           { get { return mSuburb.Value; }           set { mSuburb.Value = value;       mSuburbID           = GeoCache.Instance.getSuburbIdFromDescription(mSuburb.Value); } }
        private int        mSuburbID;           public int      SuburbID         { get { return mSuburbID; }               set { if (!mSuburbID.Equals(value)) { mSuburbChanged = true; mSuburbID = value; mSuburb.Value = GeoCache.Instance.getSuburbFromId(mSuburbID.ToString()); } } }

        private nxProperty mDefault;            public bool     Default          { get { return mDefault.ValueBool; }      set { mDefault.Value = value.ToString(); } }

        private string     mPostalCode = "";    public string   PostalCode       { get { return mPostalCode; }             set { mPostalCode = value; } }

        private string     mProvince   = "";    public string   Province         { get { return mProvince; } }
        private int        mProvinceID;         public int      ProvinceId       { get { return mProvinceID; }             set { mProvinceID = value;  } }
        private string     mCountry = "";       public string   Country          { get { return mCountry; } }
        private int        mCountryID;          public int      CountryId        { get { return mCountryID; }              set { mCountryID = value;  } }

        private bool       mForceUpdate     = false;
        private bool       mSuburbChanged   = false;
        private bool       mNewEntry = true;    public bool     NewEntry         { get { return mNewEntry; }               set { mNewEntry = value; } }
        private bool       mStatus = false;     public bool     Status           { get { return mStatus; }                 set { mStatus = value; } }

        public event         OnEntryStatusChangeEventHandler OnEntryStatusChange;
        public delegate void OnEntryStatusChangeEventHandler(bool Status);

        public dlContact() 
        {
            mContactType     = new nxProperty("Contact", "ContactType");
            mContactState    = new nxProperty("Contact", "ContactState");
            mContactLocation = new nxProperty("Contact", "ContactLocation");
            mContact1        = new nxProperty("Contact", "Contact1");
            mContact2        = new nxProperty("Contact", "Contact2");
            mContact3        = new nxProperty("Contact", "Contact3");
            mDetail          = new nxProperty("Contact", "Detail");
            mStreet          = new nxProperty("Contact", "Street");
            mSuburb          = new nxProperty("Contact", "Suburb");
            mDefault         = new nxProperty("Contact", "Default");

            
        }
        public void Create(string ContactType)
        {
            mContactType     = new nxProperty("Contact", "ContactType");
            mContactState    = new nxProperty("Contact", "ContactState");
            mContactLocation = new nxProperty("Contact", "ContactLocation");
            mContact3        = new nxProperty("Contact", "Contact3");
            mDetail          = new nxProperty("Contact", "Detail");

            switch (ContactType.ToUpper())
            {
                case "ADDRESS":
                    mContact1            = new nxProperty("Contact", "StreetNo");
                    mContact1.Property   = mContactType + " [StreetNo]";
                    mContact1.PropertyId = mContactTypeID.ToString();

                    mContact2            = new nxProperty("Contact", "StreetName");
                    mContact2.Property   = mContactType + " [StreetName]";
                    mContact2.PropertyId = mContactTypeID.ToString();

                    mStreet              = new nxProperty("Contact", "StreetType");
                    mStreet.Property     = mContactType + " [StreetType]";
                    mStreet.PropertyId   = mContactTypeID.ToString();

                    mSuburb              = new nxProperty("Contact", "Suburb");
                    mSuburb.Property     = mContactType + " [Suburb]";
                    mSuburb.PropertyId   = mSuburbID.ToString();

                    mDefault             = new nxProperty("Contact", "Default");
                    mDefault.Property    = mContactType + " [Default]";

                    Reset(ContactType,"Street");
                    break;

                case "CHANNEL":
                    mContact1            = new nxProperty("Contact", "Service");
                    mContact1.Property   = mContactType + " [Service]";
                    mContact1.PropertyId = mContactTypeID.ToString();

                    mContact2            = new nxProperty("Contact", "Other");
                    mContact2.Property   = mContactType + " [Other]";
                    mContact2.PropertyId = mContactTypeID.ToString();

                    Reset(ContactType,"EMail");
                    break;

                default:
                    break;
            }
            
        }


        public bool Open(string ContactPK) { return Open(mEntityID, ContactPK); }


        public bool Open(string EntityId, string ContactPK)
        {

            mEntityID  = EntityId;
            mContactPK = ContactPK;

            int cnPk = 0;
            int.TryParse(mContactPK, out cnPk);
            mNewEntry  = (cnPk==0);

            if (mNewEntry)
                Reset();
            else
            {
                SQLServer DB = new SQLServer(setting.ConnectionString);
                DataTable DT = DB.GetDataTable("loc.lst_Contact", "I~S~ContactPK~" + mContactPK);
                if (DT.Rows.Count > 0)
                    LoadFromRow(DT.Rows[0]);

                DB.Dispose();
                DB = null;
            }
            return true;
        }
        public string Description(string ContactPK)
        {
            Open(mEntityID,ContactPK);
            return mDescription;
        }


        private void LoadFromRow(DataRow Row)
        {

            int.TryParse(Row["StreetID"].ToString(), out mStreetID);
            int.TryParse(Row["SuburbID"].ToString(), out mSuburbID);

            mContactTypeID      = Row["ContactTypeID"].ToString();
            mContactStateID     = Row["ContactStateID"].ToString();

            mContactType.Create  (Row["ContactType"].ToString());
            mContactState.Create (Row["ContactState"].ToString());
            mContact1.Create     (Row["Contact1"].ToString());
            mContact2.Create     (Row["Contact2"].ToString());
            mContact3.Create     (Row["Contact3"].ToString());
            mSuburb.Create       (Row["SuburbName"].ToString());
            mDetail.Create       (Row["Detail"].ToString());
            mDefault.Create      (Row["Default"].ToString());
            mStreet.Create       (Row["StreetName"].ToString());
            mPostalCode         = Row["PostCode"].ToString();
            mProvince           = Row["ProvinceName"].ToString();
            mCountry            = Row["CountryName"].ToString();
               
            int.TryParse(Row["SuburbId"].ToString(),   out mSuburbID);
            int.TryParse(Row["ProvinceId"].ToString(), out mProvinceID);
            int.TryParse(Row["CountryId"].ToString(),  out mCountryID);

            mNewEntry = false;

        }

        public bool Update()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);

            string Params = "";

            if (!mNewEntry)
                Params = "I~S~ContactPK~" + mContactPK + ";";

            Params += "I~S~EntityID~"        + mEntityID          + ";";
            Params += "I~S~ContactType~"     + mContactTypeID     + ";";
            Params += "I~S~ContactState~"    + mContactStateID    + ";";
            Params += "I~S~ContactLocation~" + mContactLocationID + ";";
            Params += "I~S~StreetID~"        + mStreetID          + ";";
            Params += "I~S~SuburbID~"        + mSuburbID          + ";";
            Params += "I~S~Contact1~"        + mContact1.Value    + ";";
            Params += "I~S~Contact2~"        + mContact2.Value    + ";";
            Params += "I~S~Contact3~"        + mContact3.Value    + ";";
            Params += "I~S~Detail~"          + mDetail.Value      + ";";

            mContactPK = DB.ExecLookup ("loc.mod_Contact", Params).ToString();

            mDataError = DB.DataError;
            if (String.IsNullOrEmpty(mDataError))
            {
                mContactState.Update();
                mContactLocation.Update();
                mStreet.Update();
                mSuburb.Update();
                mContact1.Update();
                mContact2.Update();
                mContact3.Update();
                mDetail.Update();

                mSuburbChanged = false;
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Change", "Contact", "UpdateError"), mDataError); }

            DB.Dispose();
            DB = null;

            mNewEntry = false;

            return String.IsNullOrEmpty(mDataError);

        }

        public bool Find(string ContactType, string EntityID)
        {
            bool ret = false;

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Contact", "I~S~ContactType~" + ContactType  + ";I~S~EntityID~" + EntityID);
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


        public string Address
        {
            get { return (mContact1.Value + " " + mContact2.Value).Trim() + " " + mStreet.Value; }
            set
            {
                mStatus = false;

                string StreetNo = "";
                string StreetType = "";
                string StreetName = "";
                int StreetTypeID = 0;
                string testvalue = (value == null) ? "" : value.ToUpper();

                if (testvalue.Contains("PO BOX") | testvalue.Contains("P.O BOX") | testvalue.Contains("PO. BOX") | testvalue.Contains("P.O. BOX") | testvalue.Contains("P O BOX"))
                {
                    Array Arr = "PO BOX;P.O BOX;PO. BOX;P.O. BOX;P O BOX".Trim().Split(new char[] { ';' });
                    for (int p = 0; p <= Arr.GetLength(0) - 1; p++)
                    {
                        string test = Arr.GetValue(p).ToString();

                        if (testvalue.Contains(test))
                        {
                            int indx = testvalue.IndexOf(test);
                            string l = ""; if (indx > 0) l = value.Substring(0, indx).TrimEnd();
                            string r = ""; r = value.Substring(indx + test.Length, value.Length - (indx + test.Length)).TrimStart();

                            value = l + " PO Box " + r;
                            break;
                        }
                    }

                    StreetNo = "";
                    StreetType = "";
                    StreetTypeID = 0;
                    StreetName = value;
                }
                else
                {
                    Array Arr = ((value == null) ? "" : value).Trim().Split(new char[] { ' ' });
                    switch (Arr.GetLength(0))
                    {
                        case 0:
                            break;
                        case 1:
                            StreetName = Arr.GetValue(0).ToString();
                            break;
                        case 2:
                            int streetNum = 0;
                            int.TryParse(Arr.GetValue(0).ToString(), out streetNum);
                            if (streetNum > 0)
                                StreetName = Arr.GetValue(0).ToString() + " " + Arr.GetValue(1).ToString();
                            else
                            {
                                StreetNo   = "";
                                StreetName = Arr.GetValue(0).ToString();
                                StreetType = Arr.GetValue(1).ToString();
                            }
                            break;
                        default:
                            StreetNo = Arr.GetValue(0).ToString();
                            StreetType = Arr.GetValue(Arr.GetLength(0) - 1).ToString();
                            StreetName = "";
                            for (int i = 1; i <= Arr.GetLength(0) - 2; i++)
                            {
                                StreetName = StreetName + " " + Arr.GetValue(i).ToString();
                            }
                            break;
                    }
                    StreetName = StreetName.Trim();

                    if (StreetType != "")
                    {
                        SQLServer DB = new SQLServer(setting.ConnectionString);
                        int.TryParse(DB.ExecLookup("Select StreetPK from geo.street where streetname='" + StreetType + "'"), out StreetTypeID);

                        if (StreetTypeID.Equals(0))
                        {
                            int.TryParse(DB.ExecLookup("Select StreetPK from geo.street where abbreviation='" + StreetType + "'"), out StreetTypeID);

                            if (StreetTypeID.Equals(0))
                            {
                                switch (StreetType.ToUpper())
                                {
                                    case "RAD":
                                    case "ROD":
                                        StreetType = "Road";
                                        break;
                                    case "STRET":
                                    case "SREET":
                                        StreetType = "Street";
                                        break;
                                    case "AV":
                                    case "AVN":
                                    case "AVENU":
                                    case "AVANUE":
                                    case "AVNUE":
                                        StreetType = "Avenue";
                                        break;
                                    case "HW":
                                    case "H/WAY":
                                        StreetType = "Highway";
                                        break;
                                    case "DVE":
                                        StreetType = "Drive";
                                        break;
                                    case "UK":
                                        StreetType = "Park";
                                        break;
                                    case "CT":
                                    case "CRT":
                                        StreetType = "Court";
                                        break;
                                    case "BLD":
                                        StreetType = "Boulevard";
                                        break;
                                }
                                int.TryParse(DB.ExecLookup("Select StreetPK from geo.street where streetname='" + StreetType + "'"), out StreetTypeID);
                            }

                            if (!(StreetTypeID.Equals(0)))
                                StreetType = DB.ExecLookup("Select streetName from geo.street where StreetPk=" + StreetTypeID);
                        }

                        DB.Dispose();
                        DB = null;
                    }

                    if (StreetTypeID.Equals(0))
                    {
                        StreetName = StreetName + " " + StreetType;
                        StreetType = "";
                    }
                }

                mContact1.Value = StreetNo;
                mContact2.Value = StreetName;
                mStreet.Value   = StreetType;
                mStreetID       = StreetTypeID;

                mStatus = (!String.IsNullOrEmpty(StreetName)) && (!StreetTypeID.Equals(0));

                if (OnEntryStatusChange != null) OnEntryStatusChange(mStatus);
            }
        }

        public bool SetNewDistrictId()
        {
            bool result = false; 

            //ServiceResponse sr;

            //try
            //{
            //    if (!string.IsNullOrEmpty(mNewDistrict) || !string.IsNullOrEmpty(mNewProvince) || !string.IsNullOrEmpty(mNewPostCode) || string.IsNullOrEmpty(mCountry))
            //    {
            //        DboHandler handler = new DboHandler();

            //        if (string.IsNullOrEmpty(mStateID.ToString()) || mStateID.ToString().Equals("0"))
            //        {
            //            mPostalProvince = new DtoPostalProvince();

            //            mPostalProvince.ProvinceGK = UdtGK.Null;
            //            mPostalProvince.ProvincePk = string.Null;
            //            mPostalProvince.CountryId = (string)mCountryID;
            //            mPostalProvince.Status = (string)Convert.ToInt32(EnumCache.Instance.getTypeId("POSTALSTATUS", "PENDING"));  // user defined postal code

            //            mPostalProvince.Province = mNewProvince;
            //            mPostalProvince.ProvinceCode = mNewProvince;

            //            mPostalProvince.PhonePrefix = string.Empty;
            //            mPostalProvince.MobilePrefix = string.Empty;


            //            sr = handler.AddBase(mPostalProvince);


            //            if (sr.retc == ServiceReturnCode.Succeeded)
            //            {

            //                string SQL = "select ProvincePk from css.PostalProvince where Province = '" + mNewProvince + "'";
            //                string mProvicePk = shell.Instance.ExecuteLookUp("", SQL);

            //                if (!string.IsNullOrEmpty(mProvicePk))
            //                {
            //                    int mProPk = Convert.ToInt32(mProvicePk);
            //                    mPostalDistrict = new DtoPostalDistrict();

            //                    mPostalDistrict.DistrictGK = UdtGK.Null;
            //                    mPostalDistrict.DistrictPk = string.Null;
            //                    mPostalDistrict.Status = (string)Convert.ToInt32(EnumCache.Instance.getTypeId("POSTALSTATUS", "PENDING"));  // user defined postal code
            //                    mPostalDistrict.ProvinceId = (string)mProPk;

            //                    mPostalDistrict.Suburb = mNewDistrict;
            //                    mPostalDistrict.Postcode = mNewPostCode;

            //                    mPostalDistrict.Longitude = string.Empty;
            //                    mPostalDistrict.Latitude = string.Empty;

            //                    sr = handler.AddBase(mPostalDistrict);

            //                    if (sr.retc == ServiceReturnCode.Succeeded)
            //                    {
            //                        string mDistId = GetDistrictId(mProPk, mCountryID, mNewDistrict, mNewPostCode, mNewProvince, mCountry);
            //                        mDistrictId = (string)Convert.ToInt32(mDistId);

            //                        result = true;
            //                    }
            //                }

            //            }
            //        }
            //        else
            //        {
            //            mPostalDistrict = new DtoPostalDistrict();

            //            mPostalDistrict.DistrictGK = UdtGK.Null;
            //            mPostalDistrict.DistrictPk = string.Null;
            //            mPostalDistrict.Status = (string)Convert.ToInt32(EnumCache.Instance.getTypeId("POSTALSTATUS", "PENDING"));  // user defined postal code
            //            mPostalDistrict.ProvinceId = (string)mStateID;

            //            mPostalDistrict.Suburb = mNewDistrict;
            //            mPostalDistrict.Postcode = mNewPostCode;

            //            mPostalDistrict.Longitude = string.Empty;
            //            mPostalDistrict.Latitude = string.Empty;

            //            sr = handler.AddBase(mPostalDistrict);

            //            if (sr.retc == ServiceReturnCode.Succeeded)
            //            {
            //                string mDistId = GetDistrictId(mStateID, mCountryID, mNewDistrict, mNewPostCode, mNewProvince, mCountry);
            //                mDistrictId = (string)Convert.ToInt32(mDistId);

            //                result = true;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //}

            return result;
        }

        public bool Changed
        {
            get { return mContact1.Changed() || mContact2.Changed() || mContact3.Changed() || mStreet.Changed() || mSuburb.Changed() || mContactState.Changed() || mContactLocation.Changed() || mDetail.Changed() || mSuburbChanged;  }
        }


        public void Dispose() { }

        public void Reset() { Reset(mContactType.Value, mDefaultType); }
        public void Reset(string ContactType, string DefaultType)
        {
            mNewEntry = true;

            ContactTypeID     = EnumCache.Instance.getTypeId  (ContactType,       DefaultType);
            ContactStateID    = EnumCache.Instance.getStateId ("Enum",            "Active");
            ContactLocationID = EnumCache.Instance.getTypeId  ("ContactLocation", "General");
            StreetID          = GeoCache.Instance.getStreetIdFromDescription("Street");

            mContactType.OriginalValue     = mContactType.Value;
            mContactLocation.OriginalValue = mContactLocation.Value;
            mContactState.OriginalValue    = mContactState.Value;
            mStreet.OriginalValue          = mStreet.Value;


            mContact1.Reset();
            mContact2.Reset();
            mContact3.Reset();
            mDetail.Reset();
            mStreet.Reset();
            mSuburb.Reset();

            mStreetID   = 0;
            mSuburbID   = 0;
            mPostalCode = "";
        }

        public override string ToString()
        {
            return "";
        }
    }
}
