using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dal;
using nexus.common.core;

namespace nexus.common.cache
{

    public sealed class GeoCache
    {

        private DataTable     mStreets = new DataTable();
        private DataTable     mSuburbs = new DataTable();
        private DataTable     mStates  = new DataTable();
        private DataTable     mCountry = new DataTable();


        private static GeoCache mInstance = new GeoCache();
        private GeoCache() { mStreets = new DataTable(); }
        public static GeoCache Instance { get { return mInstance; } }

        public bool Create()
        {
            bool Valid = false;

            SQLServer DB = new SQLServer(setting.ConnectionString);

            string SQL = "select StreetName,Enum1.Description AS StreetState,[Abbreviation],StreetState as StreetStateId, StreetPk  from geo.Street as s left join cmn.EnumValue as Enum1 ON s.Streetstate = Enum1.ValuePK order by StreetName";
            mStreets = DB.GetDataTable(SQL);

            SQL = "select ProvinceName,Enum1.Description AS SuburbState,[PostCode],[AreaCode],p.[Path],[SuburbName],[Location], SuburbState as SuburbStateId, ProvincePk as ProvinceId, s.SuburbPk from geo.Suburb as s left join cmn.EnumValue as Enum1 ON s.Suburbstate = Enum1.ValuePK left join geo.Province as p on s.ProvinceId =p.Provincepk";
            mSuburbs = DB.GetDataTable(SQL);

            SQL = "select CountryName,Enum1.Description AS ProvinceState,c.[Path],[ProvinceName],c.[PhonePrefix], ProvinceState as ProvinceStateId, CountryPk as CountryId from geo.Province as c left join cmn.EnumValue as Enum1 ON c.Provincestate = Enum1.ValuePK left join geo.Country as n on c.countryid=countrypk";
            mStates = DB.GetDataTable(SQL);

            SQL = "select [Description],[Countrypk],Enum1.Description AS CountryState,[Path],[CountryName],[PhonePrefix], CountryState as CountryStateId from geo.Country as c left join cmn.EnumValue AS Enum1 ON c.Countrystate = Enum1.ValuePK";
            mCountry = DB.GetDataTable(SQL);

            DB.Dispose();
            DB = null;

            return Valid;
        }


        public string getStreetFromId(string StreetId)
        {
            string ret = "";

            try
            {
                if (mStreets != null)
                {
                    DataTable dt = mStreets.Select("StreetPk=" + StreetId).CopyToDataTable();
                    if (dt.Rows.Count > 0)
                        ret = dt.Rows[0]["StreetName"].ToString();
                }
            }
            catch (Exception x)
            { }

            return ret;
        }

        public int getStreetIdFromDescription(string Description)
        {
            int ret = 0;

            try
            {
                if (mStreets != null)
                {
                    DataTable dt = mStreets.Select("StreetName='" + Description + "'").CopyToDataTable();
                    if (dt.Rows.Count > 0)
                        int.TryParse(dt.Rows[0]["StreetPk"].ToString(), out ret);
                }
            }
            catch (Exception x)
            { }

            return ret;
        }

        public string getSuburbFromId(string SuburbId)
        {
            string ret = "";

            try
            {
                if (mSuburbs != null)
                {
                    DataTable dt = mSuburbs.Select("SuburbPk=" + SuburbId).CopyToDataTable();
                    if (dt.Rows.Count > 0)
                        ret = dt.Rows[0]["SuburbName"].ToString();
                }
            }
            catch (Exception x)
            { }

            return ret;
        }

        public int getSuburbIdFromDescription(string Description)
        {
            int ret = 0;

            try
            {
                if (mSuburbs != null)
                {
                    DataTable dt = mSuburbs.Select("StreetName='" + Description + "'").CopyToDataTable();
                    if (dt.Rows.Count > 0)
                        int.TryParse(dt.Rows[0]["SuburbPk"].ToString(), out ret);
                }
            }
            catch (Exception x)
            { }

            return ret;
        }

        public string getProvinceFromId(string StreetId)
        {
            string ret = "";

            try
            {
                if (mStates != null)
                {
                    DataRow[] dr = mStates.Select("StreetPk=" + StreetId);
                    if (dr.GetLength(0) > 0)
                        ret = dr.GetValue(5).ToString();
                }
            }
            catch (Exception x)
            { }

            return ret;
        }

        public int getProvinceIdFromDescription(string Description)
        {
            int ret = 0;

            try
            {
                if (mStreets != null)
                {
                    DataRow[] dr = mStates.Select("StreetName=" + Description);
                    if (dr.GetLength(0) > 0)
                        int.TryParse(dr.GetValue(5).ToString(), out ret);
                }
            }
            catch (Exception x)
            { }

            return ret;
        }

        public string getCountryFromId(string StreetId)
        {
            string ret = "";

            try
            {
                if (mCountry != null)
                {
                    DataRow[] dr = mCountry.Select("StreetPk=" + StreetId);
                    if (dr.GetLength(0) > 0)
                        ret = dr.GetValue(5).ToString();
                }
            }
            catch (Exception x)
            { }

            return ret;
        }

        public int getCountryIdFromDescription(string Description)
        {
            int ret = 0;

            try
            {
                if (mCountry != null)
                {
                    DataRow[] dr = mCountry.Select("StreetName=" + Description);
                    if (dr.GetLength(0) > 0)
                        int.TryParse(dr.GetValue(5).ToString(), out ret);
                }
            }
            catch (Exception x)
            { }

            return ret;
        }

        public string getList(string EnumType)
        {

            string List = "";


            return List;
        }

        public DataTable getStreets()
        {
            return getStreets("","");
        }

        public DataTable getStreets(string Value, string Key)
        {
            try
            {
                if (mStreets != null)
                {
                    DataTable newTable = mStreets.AsEnumerable()
                      .Where(i => i.Field<string>(Key).ToLower().Contains(Value.ToLower())).CopyToDataTable();

                    if (newTable != null)
                        return newTable;
                }
            }
            catch (Exception x)
            { }
            return null;
        }

        public void Dispose() { }

        public void Reset(string EnumType)
        {
        }
    }
}
