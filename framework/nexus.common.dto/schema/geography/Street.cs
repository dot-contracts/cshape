using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlStreet
    {
        private string     mStreetPK = "0";   public string   StreetPK          { get { return mStreetPK; } }
        private string     mDescription = "";  

        private nxProperty mStreetState;      public string   StreetState       { get { return mStreetState.Value; }     set { mStreetState.Value = value;  mStreetStateID     = EnumCache.Instance.getStateFromDesc ("Street", mStreetState.Value); } }
        private string     mStreetStateID;    public string   StreetStateID     { get { return mStreetStateID; }         set { mStreetStateID = value;      mStreetState.Value = EnumCache.Instance.getDescFromId    (          mStreetStateID); } }
        private nxProperty mStreetName;       public string   StreetName        { get { return (mStreetName.Value); }    set { mStreetName.Value = value; } }
        private nxProperty mAbbreviation;     public string   Abbreviation      { get { return mAbbreviation.Value; }    set { mAbbreviation.Value = value; } }

        private bool       mNewEntry;

        public dlStreet() 
        {
            mStreetState  = new nxProperty("Address", "StreetState");
            mStreetName   = new nxProperty("Address", "StreetName");
            mAbbreviation = new nxProperty("Address", "Abbreviation");
            Reset(); 
        }

        public bool Open(string StreetPK)
        {

            mStreetPK       = StreetPK;

            string SQL = "select (ProvinceName,Enum1.Description AS StreetState,[Abbreviation],[AreaCode],[Path],[StreetName],[Location], StreetState as StreetStateID, Province as ProvinceId ) ";
                   SQL += "from loc.Street as s left join cmn.EnumValue as Enum1 ON s.Streetstate = Enum1.ValuePK left join geo.Province as p on s.Province =p.Provinceid where StreetPK=" + mStreetPK.ToString();
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable(SQL);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return true;
        }

        public string Description(string StreetPK)
        {
            Open(StreetPK);
            return mDescription;
        }


        private void LoadFromRow(DataRow Row)
        {
            mDescription = Row["StreetName"].ToString();

            mStreetStateID      = Row["StreetStateID"].ToString();
            mStreetName.Value   = Row["StreetName"].ToString();
            mStreetState.Value  = Row["StreetState"].ToString();
            mAbbreviation.Value = Row["Abbreviation"].ToString();
        }

        public void Update()
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);

            if (mNewEntry)
                mStreetPK = DB.ExecLookup(getInsertSQL());
            else
                DB.ExecNonQuery(getUpdateSQL());

            if (String.IsNullOrEmpty(DB.DataError))
            {
                mStreetState.Update();
                mStreetName.Update();
                mAbbreviation.Update();
            }

            DB.Dispose();
            DB = null;
        }

        public string getInsertSQL()
        {
            string SQL = "declare @StreetPK int; ";
            SQL += getColumns();
            SQL += "insert into geo.[Street] ([StreetGK],[StreetState],[Province],[StreetName],[Abbreviation],[AreaCode],[Path],[Modified],[Inserted]) ";
            SQL += "values (newId(),@StreetState,@Province,@StreetName,@Abbreviation,@AreaCode,@Path,getdate(),getdate()) ";
            SQL += "select @StreetPK = scope_identity() ";
            return SQL;
        }

        public string getUpdateSQL()
        {
            string SQL = "declare @StreetPK int; Set @StreetPK=" + mStreetPK.ToString() + " " + getColumns();
            SQL += "update [geo].[Street] set Province=@Province,StreetState=@StreetState,StreetName=@StreetName,Abbreviation=@Abbreviation,AreaCode=@AreaCode,Path=@Path,Location=@Location,Modified=getdate() ";
            SQL += "where StreetPK=@StreetPK";
            return SQL;
        }

        private string getColumns()
        {
            string SQL = "declare @StreetState int; Set @StreetState=" + mStreetStateID.ToString() + " ";
            SQL += "declare @StreetName varchar(32); Set @StreetName=" + mStreetName.Value + " ";
            SQL += "declare @Abbreviation varchar(256); Set @Abbreviation=" + mAbbreviation + " ";
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
            StreetStateID = EnumCache.Instance.getStateId("Entity", "Active");
            mStreetState.Reset();
            mStreetName.Reset();
            mStreetName.Reset();
            mAbbreviation.Reset();
        }

        public override string ToString()
        {
            return mDescription;
        }


    }
}
