using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlCredential 
    {

        private dlEntity   mEntity;             public dlEntity BaseEntity       { get { return mEntity; }                 set { mEntity = value; } }
        private string     mCredentialPK = "0"; public string   CredentialPK     { get { return mEntity.EntityPK; } }
        private string     mDescription = "";  

        private nxProperty mCredentialType;     public string   CredentialType   { get { return mCredentialType.Value; }   set { mCredentialType.Value = value;  mCredentialTypeID      = EnumCache.Instance.getTypeFromDesc  ("Credential", mCredentialType.Value); } }
        private string     mCredentialTypeID;   public string   CredentialTypeID { get { return mCredentialTypeID; }       set { mCredentialTypeID = value;      mCredentialType.Value  = EnumCache.Instance.getDescFromId    (            mCredentialTypeID); } }

        private nxProperty mCredentialState;    public string   CredentialState  { get { return mCredentialState.Value; }  set { mCredentialState.Value = value; mCredentialStateID     = EnumCache.Instance.getStateFromDesc ("Credential", mCredentialState.Value); } }
        private string     mCredentialStateID;  public string   CredentialStateID{ get { return mCredentialStateID; }      set { mCredentialStateID = value;     mCredentialState.Value = EnumCache.Instance.getDescFromId    (            mCredentialStateID); } }

        private nxProperty mContact1;           public string   Contact1         { get { return mContact1.Value; }         set { mContact1.Value = value; } }
        private nxProperty mContact2;           public string   Contact2         { get { return mContact2.Value; }         set { mContact2.Value = value; } }
        private nxProperty mContact3;           public string   Contact3         { get { return mContact3.Value; }         set { mContact3.Value = value; } }

        private nxProperty mStreet;             public string   Street           { get { return mStreet.Value; }           set { mStreet.Value = value;       mStreetID           = GeoCache.Instance.getStreetIdFromDescription(mStreet.Value); } }
        private int        mStreetID;           public int      StreetID         { get { return mStreetID; }               set { mStreetID = value;           mStreet.Value       = GeoCache.Instance.getStreetFromId(mStreetID.ToString()); } }

        private nxProperty mSuburb;             public string   Suburb           { get { return mSuburb.Value; }           set { mSuburb.Value = value;       mSuburbID           = GeoCache.Instance.getSuburbIdFromDescription(mSuburb.Value); } }
        private int        mSuburbID;           public int      SuburbID         { get { return mSuburbID; }               set { mSuburbID = value; if (!mSuburbID.Equals(value)) { mDistrictChanged = true; mSuburbID = value; mSuburb.Value = GeoCache.Instance.getSuburbFromId(mSuburbID.ToString()); } } }

        private bool mForceUpdate     = false;
        private bool mDistrictChanged = false;
        private bool mNewEntry = true;             public bool     NewEntry         { get { return mNewEntry; }               set { mNewEntry = value; } }
        private bool mStatus = false;              public bool     Status           { get { return mStatus; }                 set { mStatus = value; } }


        public event         OnEntryStatusChangeEventHandler OnEntryStatusChange;
        public delegate void OnEntryStatusChangeEventHandler(bool Status);


        public dlCredential() 
        {
            mCredentialType  = new nxProperty("Credential", "CredentialType");
            mCredentialState = new nxProperty("Credential", "CredentialState");
            mContact1        = new nxProperty("Credential", "Contact1");
            mContact2        = new nxProperty("Credential", "Contact2");
            mContact3        = new nxProperty("Credential", "Contact3");
            mStreet          = new nxProperty("Credential", "Street");
            mSuburb          = new nxProperty("Credential", "Suburb");
            Reset("Contact"); 
        }
        public void Create(string CredentialType)
        {
            mCredentialType  = new nxProperty("Credential", "CredentialType");
            mCredentialState = new nxProperty("Credential", "CredentialState");
            mContact3    = new nxProperty("Credential", "Other");

            switch (CredentialType.ToUpper())
            {
                case "ADDRESS":
                    mContact1            = new nxProperty("Credential", "StreetNo");
                    mContact1.Property   = mCredentialType + " [StreetNo]";
                    mContact1.PropertyId = mCredentialTypeID.ToString();

                    mContact2            = new nxProperty("Credential", "StreetName");
                    mContact2.Property   = mCredentialType + " [StreetName]";
                    mContact2.PropertyId = mCredentialTypeID.ToString();

                    mStreet              = new nxProperty("Credential", "StreetType");
                    mStreet.Property     = mCredentialType + " [StreetType]";
                    mStreet.PropertyId   = mCredentialTypeID.ToString();

                    mSuburb              = new nxProperty("Credential", "Suburb");
                    mSuburb.Property     = mCredentialType + " [Suburb]";
                    mSuburb.PropertyId   = mSuburbID.ToString();

                    break;
                case "CHANNEL":
                    mContact1            = new nxProperty("Credential", "Service");
                    mContact1.Property   = mCredentialType + " [Service]";
                    mContact1.PropertyId = mCredentialTypeID.ToString();

                    mContact2            = new nxProperty("Credential", "Other");
                    mContact2.Property   = mCredentialType + " [Other]";
                    mContact2.PropertyId = mCredentialTypeID.ToString();
                    break;
                default:
                    break;
            }
            Reset("Contact"); 
        }


        public bool Open(string CredentialPK)
        {

            mCredentialPK = CredentialPK;

            string SQL = "select ([CredentialPK],Enum1.Description AS CredentialType, Enum2.Description AS CredentialState,Enum3.Description AS Street, Enum4.Description AS Suburb,[Contact1],[Contact2],[Contact3],[CredentialType] as CredentialTypeID,[CredentialState] As CredentialStateID,[Street] as StreetID,[Suburb] as SuburbID) ";
                   SQL += "from loc.Contact as p left Join cmn.EnumValue as Enum1 ON p.CredentialType = Enum1.ValuePK left Join cmn.EnumValue AS Enum2 ON p.CredentialState = Enum2.ValuePK left Join cmn.EnumValue as Enum3 ON p.Street = Enum3.ValueId left Join enums.EnumValue AS Enum4 ON p.Suburb = Enum4.ValueId where CredentialPK=" + mCredentialPK.ToString();
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return true;
        }
        public string Description(string CredentialPK)
        {
            Open(CredentialPK);
            return mDescription;
        }


        private void LoadFromRow(DataRow Row)
        {

            int.TryParse(Row["StreetID"].ToString(), out mStreetID);
            int.TryParse(Row["SuburbID"].ToString(), out mSuburbID);

            mCredentialTypeID      = Row["CredentialTypeID"].ToString();
            mCredentialStateID     = Row["CredentialStateID"].ToString();
            mCredentialType.Value  = Row["CredentialType"].ToString();
            mCredentialState.Value = Row["CredentialState"].ToString();
            mStreet.Value        = Row["Street"].ToString();
            mSuburb.Value        = Row["Suburb"].ToString();
            mContact1.Value      = Row["Contact1"].ToString();
            mContact2.Value      = Row["Contact2"].ToString();
            mContact3.Value      = Row["Contact3"].ToString();
        }

        public bool Update()
        {
            bool ret=false;

            SQLServer DB = new SQLServer(setting.ConnectionString);

            //if (string.IsNullOrEmpty(mCredentialPK))
            //    mCredentialPK = DB.ExecLookup(getInsertSQL());
            //else
            //    DB.ExecNonQuery(getUpdateSQL());

            if (String.IsNullOrEmpty(DB.DataError))
            {
                ret = true;
                mCredentialState.Update();
                mStreet.Update();
                mSuburb.Update();
                mContact1.Update();
                mContact2.Update();
                mContact3.Update();
            }

            DB.Dispose();
            DB = null;

            return ret;
        }

        //public string getInsertSQL()
        //{

        //    //string SQL = mEntity.getInsertSQL() + " " + getColumns();
        //    //SQL += "insert into [loc].[Contact] ([CredentialPK],[CredentialType],[CredentialState],[Contact1],[Contact2],[Contact3],[Street],[Suburb]) ";
        //    //SQL += "values (@EntityID,@CredentialTypeID,@CredentialStateID,@Contact1,@Contact2,@Contact3,@Street,@Suburb) ";
        //    //return SQL;

        //}

        //public string getUpdateSQL()
        //{
        //    //string SQL = mEntity.getUpdateSQL() + " " + getColumns();
        //    //SQL += "update loc.[Contact] set [CredentialState] = @CredentialState,[Street] = @Street,[Suburb] = @Suburb,[Contact1]=@Contact1,[Contact2]=@Contact2,[Contact3]=@Contact3 ";
        //    //SQL += "where CredentialPK=@EntityID";
        //    //return SQL;
       // }

        private string getColumns()
        {
            string SQL = "declare @CredentialType int; Set @CredentialType=" + mCredentialTypeID.ToString() + " ";
            SQL += "declare @CredentialState int; Set @CredentialState=" + mCredentialStateID.ToString() + " ";
            SQL += "declare @Contact1 varchar(256); Set @Contact1=" + mContact1.Value + " ";
            SQL += "declare @Contact2 varchar(256); Set @Contact2=" + mContact2.Value + " ";
            SQL += "declare @Contact3 varchar(256); Set @Contact3=" + mContact3.Value + " ";
            SQL += "declare @Street varchar(256); Set @Street=" + mStreetID.ToString() + " ";
            SQL += "declare @Suburb varchar(256); Set @Suburb=" + mSuburbID.ToString() + " ";
            return SQL;
        }

        public void Find(string Column, string Value)
        {

        }



        public string Address1
        {
            get { return (mContact1.Value + " " + mContact2.Value + " " + mStreet.Value).Trim(); }
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
                            StreetName = Arr.GetValue(0).ToString() + " " + Arr.GetValue(1).ToString();
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
                        int.TryParse(DB.ExecLookup("Select StreetPK from geo.street where name='" + StreetType + "'"), out StreetTypeID);

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
                                    case "PK":
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
                                int.TryParse(DB.ExecLookup("Select StreetPK from geo.street where name='" + StreetType + "'"), out StreetTypeID);
                            }

                            if (!(StreetTypeID.Equals(0)))
                                StreetType = DB.ExecLookup("Select Name from geo.street where StreetPk=" + StreetTypeID);
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
                mStreet.Value = StreetType;
                mStreetID = StreetTypeID;

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
            get { return mContact1.Changed() || mContact2.Changed() || mContact3.Changed() || mStreet.Changed() || mSuburb.Changed() || mCredentialState.Changed(); }
        }


        public void Dispose() { }

        public void Reset(string CredentialType)
        {
            CredentialTypeID  = EnumCache.Instance.getTypeId  ("Credential",  CredentialType);
            CredentialStateID = EnumCache.Instance.getStateId ("Credential", "Active");
            StreetID        = EnumCache.Instance.intId      ("Street",   "Male");
            SuburbID        = EnumCache.Instance.intId      ("Suburb",   "Mr");

            if (mEntity == null)
                mEntity = new dlEntity("Contact");
            else
                mEntity.Reset("Contact");

            mContact1.Reset();
            mContact2.Reset();
            mContact3.Reset();
            mStreet.Reset();
            mSuburb.Reset();
        }

        public override string ToString()
        {
            return "";
        }
    }
}
