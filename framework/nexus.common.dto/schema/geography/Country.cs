using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlCountry
    {
        private string      mCountryPK = "0";   public string   CountryPK      { get { return mCountryPK; } }
        private string      mDescription = "";  

        private nxProperty  mCountryState;      public string   CountryState   { get { return mCountryState.Value; }   set { mCountryState.Value = value; mCountryStateId     = EnumCache.Instance.getTypeFromDesc ("Country", mCountryState.Value); } }
        private string      mCountryStateId;    public string   CountryStateId { get { return mCountryStateId; }       set { mCountryStateId = value;     mCountryState.Value = EnumCache.Instance.getDescFromId   (           mCountryStateId); } }

        private nxProperty  mCountryName;       public string   CountryName    { get { return (mCountryName.Value); }  set { mCountryName.Value = value; } }
        private nxProperty  mCode2;             public string   Code2          { get { return mCode2.Value; }          set { mCode2.Value = value; } }
        private nxProperty  mCode3;             public string   Code3          { get { return mCode3.Value; }          set { mCode3.Value = value; } }
        private nxProperty  mPath;              public string   Path           { get { return mPath.Value; }           set { mPath.Value = value; } }
        private nxProperty  mPhonePrefix;       public string   PhonePrefix    { get { return mPhonePrefix.Value; }    set { mPhonePrefix.Value = value; } }

        private bool        mNewEntry;

        public dlCountry() 
        {
            mCountryState   = new nxProperty("Country", "CountryState");

            mCountryName    = new nxProperty("Country", "CountryName");
            mCode2          = new nxProperty("Country", "Code2");
            mCode3          = new nxProperty("Country", "Code3");
            mPath           = new nxProperty("Country", "Path");
            mPhonePrefix    = new nxProperty("Country", "PhonePrefix");

            Reset(); 
        }

        public bool Open(string CountryPK)
        {

            mCountryPK       = CountryPK;

            string SQL  = "select ([Description],[CountryPK],Enum1.Description AS CountryState,[Code2],[Code3],[Path],[CountryName],[PhonePrefix], CountryState as CountryStateId ) ";
                   SQL += "from geo.Country as c left join cmn.EnumValue AS Enum1 ON c.Countrystate = Enum1.ValuePK where CountryPK=" + mCountryPK.ToString();
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return true;
        }

        public string Description(string CountryPK)
        {
            Open(CountryPK);
            return mDescription;
        }


        private void LoadFromRow(DataRow Row)
        {
            mDescription         = Row["Description"].ToString();
            mCountryStateId      = Row["CountryStateId"].ToString();
            mCountryState.Value  = Row["CountryState"].ToString();
            mCode2.Value         = Row["Code2"].ToString();
            mCode3.Value         = Row["Code3"].ToString();
            mPath.Value          = Row["Path"].ToString();
            mCountryName.Value   = Row["CountryName"].ToString();
            mPhonePrefix.Value   = Row["PhonePrefix"].ToString();
        }

        public void Update()
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);

            if (mNewEntry)
                mCountryPK = DB.ExecLookup(getInsertSQL());
            else
                DB.ExecNonQuery(getUpdateSQL());

            if (String.IsNullOrEmpty(DB.DataError))
            {
                mCountryState.Update();
                mCountryName.Update();
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
            string SQL = "declare @CountryPK int; ";
            SQL += getColumns();
            SQL += "insert into geo.[Country] ([CountryGK],[CountryState],[CountryName],[Code2],[Code3],[Path],[PhonePrefix],[Modified],[Inserted]) ";
            SQL += "values (newId(),@CountryState,@CountryName,@Code2,@Code3,@Path,@PhonePrefix,getdate(),getdate()) ";
            SQL += "select @CountryPK = scope_identity() ";
            return SQL;
        }

        public string getUpdateSQL()
        {
            string SQL = "declare @CountryPK int; Set @CountryPK=" + mCountryPK.ToString() + " " + getColumns();
            SQL += "update [geo].[Country] set CountryState=@CountryState,CountryName=@CountryName,Code2=@Code2,Code3=@Code3,Path=@Path,PhonePrefix=@PhonePrefix,Modified=getdate() ";
            SQL += "where CountryPK=@CountryPK";
            return SQL;
        }

        private string getColumns()
        {
            string SQL = "declare @CountryState int; Set @CountryState=" + mCountryStateId.ToString() + " ";
            SQL += "declare @CountryName varchar(32); Set @CountryName=" + mCountryName.Value + " ";
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
            CountryStateId = EnumCache.Instance.getStateId("Entity", "Active");
            mCountryState.Reset();
            mCountryName.Reset();
            mPhonePrefix.Reset();
            mCountryName.Reset();
            mCode2.Reset();
            mCode3.Reset();
            mPath.Reset();
            mPhonePrefix.Reset();
        }

        public override string ToString()
        {
            return mDescription;
        }

        public string strId(string CountryDefn) {return getId(CountryDefn).ToString();}
        public int    getId(string CountryDefn)
        {
            int ValuePK = 0;


            return ValuePK;
        }

        public string strIdFromDesc(string CountryDescription) {return getIdFromDesc(CountryDescription).ToString();}
        public int    getIdFromDesc(string CountryDescription)
        {

            int ValuePK = 0;


            return ValuePK;
        }

        public string getDescFromId(string CountryPK) 
        {
            string Desc = "";


            return Desc;

        }

        public string getList(string CountryType)
        {

            string List = "";


            return List;
        }

        public DataTable getTable(string CountryType)
        {
            DataTable Table = new DataTable();

            return Table;
        }



    }
}
