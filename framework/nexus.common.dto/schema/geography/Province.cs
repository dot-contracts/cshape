using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlProvince
    {
        private string     mProvincePK = "0";  public string   ProvincePK      { get { return mProvincePK; } }
        private string     mDescription = "";  

        private nxProperty mCountry;           public string   Country         { get { return mCountry.Value; }        set { mCountry.Value = value;       mCountryID     = GeoCache.Instance.getCountryIdFromDescription(mCountry.Value).ToString(); } }
        private string     mCountryID;         public string   CountryID       { get { return mCountryID; }            set { mCountryID = value;           mCountry.Value = GeoCache.Instance.getCountryFromId (mCountryID); } }

        private nxProperty mProvinceState;     public string   ProvinceState   { get { return mProvinceState.Value; }  set { mProvinceState.Value = value; mProvinceStateID     = EnumCache.Instance.getStateFromDesc ("Province", mProvinceState.Value); } }
        private string     mProvinceStateID;   public string   ProvinceStateID { get { return mProvinceStateID; }      set { mProvinceStateID = value;     mProvinceState.Value = EnumCache.Instance.getDescFromId    (            mProvinceStateID); } }

        private nxProperty mProvinceName;      public string   ProvinceName    { get { return (mProvinceName.Value); } set { mProvinceName.Value = value; } }
        private nxProperty mCode2;             public string   Code2           { get { return mCode2.Value; }          set { mCode2.Value = value; } }
        private nxProperty mCode3;             public string   Code3           { get { return mCode3.Value; }          set { mCode3.Value = value; } }
        private nxProperty mPath;              public string   Path            { get { return mPath.Value; }           set { mPath.Value = value; } }
        private nxProperty mPhonePrefix;       public string   PhonePrefix     { get { return mPhonePrefix.Value; }    set { mPhonePrefix.Value = value; } }

        private bool       mNewEntry;

        public dlProvince() 
        {
            mCountry        = new nxProperty("Province", "Country");
            mProvinceState  = new nxProperty("Province", "ProvinceState");
            mProvinceName   = new nxProperty("Province", "ProvinceName");
            mCode2          = new nxProperty("Province", "Code2");
            mCode3          = new nxProperty("Province", "Code3");
            mPath           = new nxProperty("Province", "Path");
            mPhonePrefix    = new nxProperty("Province", "PhonePrefix");

            Reset(); 
        }

        public bool Open(string ProvincePK)
        {

            mProvincePK       = ProvincePK;

            string SQL = "select (CountryName,Enum1.Description AS ProvinceState,[Code2],[Code3],[Path],[ProvinceName],[PhonePrefix], ProvinceState as ProvinceStateID, Country as CountryID ) ";
                   SQL += "from geo.Province as c left join cmn.EnumValue as Enum1 ON c.Provincestate = Enum1.ValuePK left join geo.Country as n on c.country.n.CountryID where ProvincePK=" + mProvincePK.ToString();
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return true;
        }

        public string Description(string ProvincePK)
        {
            Open(ProvincePK);
            return mDescription;
        }


        private void LoadFromRow(DataRow Row)
        {
            mDescription = Row["ProvinceName"].ToString();

            mProvinceStateID     = Row["ProvinceStateID"].ToString();
            mCountryID           = Row["CountryID"].ToString();

            mProvinceState.Value = Row["ProvinceState"].ToString();
            mCountry.Value       = Row["Country"].ToString();
            mCode2.Value         = Row["Code2"].ToString();
            mCode3.Value         = Row["Code3"].ToString();
            mPath.Value          = Row["Path"].ToString();
            mProvinceName.Value  = Row["ProvinceName"].ToString();
            mPhonePrefix.Value   = Row["PhonePrefix"].ToString();
        }

        public void Update()
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);

            if (mNewEntry)
                mProvincePK = DB.ExecLookup(getInsertSQL());
            else
                DB.ExecNonQuery(getUpdateSQL());

            if (String.IsNullOrEmpty(DB.DataError))
            {
                mCountry.Update();
                mProvinceState.Update();
                mProvinceName.Update();
                mCode2.Update();
                mCode3.Update();
                mPath.Update();
                mPhonePrefix.Update();
            }

            DB.Dispose();
            DB = null;
        }

        public string getInsertSQL()
        {
            string SQL = "declare @ProvincePK int; ";
            SQL += getColumns();
            SQL += "insert into geo.[Province] ([ProvinceGK],[ProvinceState],[Country],[ProvinceName],[ProvinceCode],[Path],[Modified],[Inserted]) ";
            SQL += "values (newId(),@ProvinceState,@Country,@ProvinceName,@ProvinceCode,@Path,getdate(),getdate()) ";
            SQL += "select @ProvincePK = scope_identity() ";
            return SQL;
        }

        public string getUpdateSQL()
        {
            string SQL = "declare @ProvincePK int; Set @ProvincePK=" + mProvincePK.ToString() + " " + getColumns();
            SQL += "update [geo].[Province] set Country=@Country,ProvinceState=@ProvinceState,ProvinceName=@ProvinceName,Code2=@Code2,Code3=@Code3,Path=@Path,PhonePrefix=@PhonePrefix,Modified=getdate() ";
            SQL += "where ProvincePK=@ProvincePK";
            return SQL;
        }

        private string getColumns()
        {
            string SQL = "declare @Country int; Set @Country=" + mCountryID.ToString() + " ";
            SQL += "declare @ProvinceState int; Set @ProvinceState=" + mProvinceStateID.ToString() + " ";
            SQL += "declare @ProvinceName varchar(32); Set @ProvinceName=" + mProvinceName.Value + " ";
            SQL += "declare @Code2 varchar(256); Set @Code2=" + mCode2 + " ";
            SQL += "declare @Code3 varchar(256); Set @Code3=" + mCode3 + " ";
            SQL += "declare @Path varchar(Max); Set @Path=" + mPath + " ";
            SQL += "declare @PhonePrefix varchar(16); Set @PhonePrefix=" + mPhonePrefix + " ";
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

        public void Reset()
        {
            ProvinceStateID = EnumCache.Instance.getStateId("Entity", "Active");
            mProvinceState.Reset();
            mProvinceName.Reset();
            mPhonePrefix.Reset();
            mProvinceName.Reset();
            mCode2.Reset();
            mCode3.Reset();
            mPath.Reset();
            mPhonePrefix.Reset();
        }

        public override string ToString()
        {
            return mDescription;
        }

        public string strId(string ProvinceDefn) {return getId(ProvinceDefn).ToString();}
        public int    getId(string ProvinceDefn)
        {
            int ValuePK = 0;


            return ValuePK;
        }

        public string strIdFromDesc(string ProvinceDescription) {return getIdFromDesc(ProvinceDescription).ToString();}
        public int    getIdFromDesc(string ProvinceDescription)
        {

            int ValuePK = 0;


            return ValuePK;
        }

        public string getDescFromId(string ProvincePK) 
        {
            string Desc = "";


            return Desc;

        }

        public string getList(string ProvinceType)
        {

            string List = "";


            return List;
        }

        public DataTable getTable(string ProvinceType)
        {
            DataTable Table = new DataTable();

            return Table;
        }

    }
}
