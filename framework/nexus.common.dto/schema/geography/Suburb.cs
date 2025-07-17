using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlSuburb
    {
        private string     mSuburbPK = "0";   public string   SuburbPK          { get { return mSuburbPK; } }
        private string     mDescription = "";  

        private nxProperty mProvince;         public string   Province          { get { return mProvince.Value; }        set { mProvince.Value = value;     mProvinceID     = GeoCache.Instance.getSuburbIdFromDescription(mProvince.Value) ; } }
        private int        mProvinceID;       public int      ProvinceID        { get { return mProvinceID; }            set { mProvinceID = value;         mProvince.Value = GeoCache.Instance.getSuburbFromId(mProvinceID.ToString()); } }

        private nxProperty mSuburbState;      public string   SuburbState       { get { return mSuburbState.Value; }     set { mSuburbState.Value = value;  mSuburbStateID     = EnumCache.Instance.getStateFromDesc ("Suburb",    mSuburbState.Value); } }
        private string     mSuburbStateID;    public string   SuburbStateID     { get { return mSuburbStateID; }         set { mSuburbStateID = value;      mSuburbState.Value = EnumCache.Instance.getDescFromId    (             mSuburbStateID.ToString()); } }

        private nxProperty mSuburbName;       public string   SuburbName        { get { return (mSuburbName.Value); }    set { mSuburbName.Value = value; } }
        private nxProperty mPostCode;         public string   PostCode          { get { return mPostCode.Value; }        set { mPostCode.Value = value; } }
        private nxProperty mAreaCode;         public string   AreaCode          { get { return mAreaCode.Value; }        set { mAreaCode.Value = value; } }

        private nxProperty mDeliveryOffice;   public string   DeliveryOffice    { get { return mDeliveryOffice.Value; }  set { mDeliveryOffice.Value = value; } }
        private string     mDeliveryOfficeID; public string   DeliveryOfficeID  { get { return mDeliveryOfficeID; }      set { mDeliveryOfficeID = value; } }

        //'private nxProperty mPath;             public string   Path              { get { return mPath.Value; }            set { mPath.Value = value; } }
        //'private nxProperty mLocation;         public string   Location          { get { return mLocation.Value; }        set { mLocation.Value = value; } }

        private bool       mNewEntry;

        public dlSuburb() 
        {
            mProvince        = new nxProperty("Suburb", "Province");
            mSuburbState     = new nxProperty("Suburb", "SuburbState");
            mSuburbName      = new nxProperty("Suburb", "SuburbName");
            mPostCode        = new nxProperty("Suburb", "PostCode");
            mAreaCode        = new nxProperty("Suburb", "AreaCode");
            //'mPath            = new nxProperty("Suburb", "Path");
            //'mLocation        = new nxProperty("Suburb", "Location");
            mDeliveryOffice  = new nxProperty("Suburb", "DeliveryOffice");

            Reset(); 
        }

        public bool Open(string SuburbPK)
        {

            mSuburbPK       = SuburbPK;

            string SQL = "select ProvinceName,Enum1.Description AS SuburbState,[PostCode],[AreaCode],s.[Path],[SuburbName], SuburbState as SuburbStateID, s.ProvinceId as ProvinceID ";
                   SQL += "from geo.Suburb as s left join cmn.EnumValue as Enum1 ON s.Suburbstate = Enum1.ValuePK left join geo.Province as p on s.Provinceid =p.Provincepk where SuburbPK=" + mSuburbPK.ToString();
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return true;
        }

        public string Description(string SuburbPK)
        {
            Open(SuburbPK);
            return mDescription;
        }


        private void LoadFromRow(DataRow Row)
        {
            mDescription       = Row["SuburbName"].ToString();

            mSuburbStateID     = Row["SuburbStateID"].ToString();

            int.TryParse(Row["ProvinceID"].ToString(), out mProvinceID);

            mSuburbState.Value = Row["SuburbState"].ToString();
            mProvince.Value    = Row["ProvinceName"].ToString();
            mPostCode.Value    = Row["PostCode"].ToString();
            mAreaCode.Value    = Row["AreaCode"].ToString();
            //'mPath.Value        = Row["Path"].ToString();
            mSuburbName.Value  = Row["SuburbName"].ToString();
            //'mLocation.Value    = Row["Location"].ToString();
        }

        public string Update()
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);

            if (mNewEntry)
                mSuburbPK = DB.ExecLookup(getInsertSQL());
            else
                DB.ExecNonQuery(getUpdateSQL());

            if (String.IsNullOrEmpty(DB.DataError))
            {
                mProvince.Update();
                mSuburbState.Update();
                mSuburbName.Update();
                mPostCode.Update();
                mAreaCode.Update();
                //'mPath.Update();
                //'mLocation.Update();
            }

            DB.Dispose();
            DB = null;

            return mSuburbPK;
        }

        public string getInsertSQL()
        {
            string SQL = "declare @SuburbPK int; ";
            SQL += getColumns();
            SQL += "insert into geo.[Suburb] ([SuburbGK],[SuburbState],[Province],[SuburbName],[PostCode],[AreaCode],[Path],[Modified],[Inserted]) ";
            SQL += "values (newId(),@SuburbState,@Province,@SuburbName,@PostCode,@AreaCode,@Path,getdate(),getdate()) ";
            SQL += "select @SuburbPK = scope_identity() ";
            return SQL;
        }

        public string getUpdateSQL()
        {
            string SQL = "declare @SuburbPK int; Set @SuburbPK=" + mSuburbPK.ToString() + " " + getColumns();
            SQL += "update [geo].[Suburb] set Province=@Province,SuburbState=@SuburbState,SuburbName=@SuburbName,PostCode=@PostCode,AreaCode=@AreaCode,Modified=getdate() ";
            SQL += "where SuburbPK=@SuburbPK";
            return SQL;
        }

        private string getColumns()
        {
            string SQL = "declare @Province int; Set @Province="       + mProvinceID.ToString() + " ";
            SQL += "declare @SuburbState int; Set @SuburbState="       + mSuburbStateID.ToString() + " ";
            SQL += "declare @SuburbName varchar(32); Set @SuburbName=" + mSuburbName.Value + " ";
            SQL += "declare @PostCode varchar(256); Set @PostCode="    + mPostCode + " ";
            SQL += "declare @AreaCode varchar(256); Set @AreaCode="    + mAreaCode + " ";
            //'SQL += "declare @Path varchar(Max); Set @Path="            + mPath + " ";
           //' SQL += "declare @Location varchar(16); Set @Location="     + mLocation + " ";
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
            SuburbStateID = EnumCache.Instance.getStateId("Entity", "Active");
            mSuburbState.Reset();
            mSuburbName.Reset();
            //'mLocation.Reset();
            mSuburbName.Reset();
            mPostCode.Reset();
            mAreaCode.Reset();
           //' mPath.Reset();
        }

        public override string ToString()
        {
            return mDescription;
        }

        public string strId(string SuburbDefn) {return getId(SuburbDefn).ToString();}
        public int    getId(string SuburbDefn)
        {
            int ValueId = 0;


            return ValueId;
        }

        public string strIdFromDesc(string SuburbDescription) {return getIdFromDesc(SuburbDescription).ToString();}
        public int    getIdFromDesc(string SuburbDescription)
        {

            int ValueId = 0;


            return ValueId;
        }

        public string getDescFromId(string SuburbPK) 
        {
            string Desc = "";


            return Desc;

        }

        public string getList(string SuburbType)
        {

            string List = "";


            return List;
        }

        public DataTable getTable(string SuburbType)
        {
            DataTable Table = new DataTable();

            return Table;
        }
    }
}
