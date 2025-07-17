using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{
    public class dlDenote
    {

        private string     mDenotePK = "";    public string DenotePK      { get { return mDenotePK; }          set { mDenotePK = value; } }

        private nxProperty mDenoteType;       public string DenoteType    { get { return mDenoteType.Value; }  set { mDenoteType.Value = value;  mDenoteTypeId     = EnumCache.Instance.getTypeFromDesc ("Denote", mDenoteType.Value); } }
        private string     mDenoteTypeId;     public string DenoteTypeId  { get { return mDenoteTypeId; }      set { mDenoteTypeId = value;      mDenoteType.Value = EnumCache.Instance.getDescFromId   (          mDenoteTypeId); } }

        private nxProperty mDescription;          public nxProperty Description   { get { return mDescription; }           set { mDescription = value; } }
        private nxProperty mParentID;             public nxProperty ParentID      { get { return mParentID; }              set { mParentID = value; } }
        private nxProperty mHeader;               public nxProperty Header        { get { return mHeader; }                set { mHeader = value; } }
        private nxProperty mBody;                 public nxProperty Body          { get { return mBody; }                  set { mBody = value; } }
        private nxProperty mFooter;               public nxProperty Footer        { get { return mFooter; }                set { mFooter = value; } }
        private nxProperty mShape;                public nxProperty Shape         { get { return mShape; }                 set { mShape = value; } }

        private string     mDataError;            public string     DataError     { get { return mDataError; }             set { mDataError = value; } }

        private bool mNewEntry; public bool NewEntry { get { return mNewEntry; } set { mNewEntry = value; } }

        public dlDenote()
        {

            mDenoteType      = new nxProperty("Denote", "DenoteType");
            mDescription     = new nxProperty("Denote", "Description");
            mHeader          = new nxProperty("Denote", "Header");
            mBody            = new nxProperty("Denote", "Body");
            mFooter          = new nxProperty("Denote", "Footer");
            mShape           = new nxProperty("Denote", "Shape");

            Reset(DenoteType);
        }

        public bool Open(string DenotePK)
        {
            mNewEntry = true;
            mDenotePK = DenotePK;

            DataRow row = DenoteCache.Instance.LookUpRow("DenotePK",mDenotePK);
            if (row != null)
            {
                mNewEntry = false;
                LoadFromRow(row);
            }
            return true;
        }

        private void LoadFromRow(DataRow Row)
        {
            DenoteTypeId       = Row["DenoteType"].ToString();

            mDescription.Value = Row["Description"].ToString();
            mHeader.Value      = Row["Header"].ToString();
            mBody.Value        = Row["Body"].ToString();
            mFooter.Value      = Row["Footer"].ToString();
            mShape.Value       = Row["Shape"].ToString();

        }

        public bool Update()
        {

            SQLServer DB = new SQLServer(setting.ConnectionString);

            string Params = "";

            if (!mNewEntry)
                Params = "I~S~DenotePK~" + mDenotePK + ";";

            Params += "I~S~DenoteType~"     + mDenoteTypeId  + ";";
            Params += "I~S~Description~"    + mDescription.Value + ";";
            Params += "I~S~ParentID~"       + mParentID          + ";";
            Params += "I~S~Header~"         + mHeader.Value      + ";";
            Params += "I~S~Body~"           + mBody.Value        + ";";
            Params += "I~S~Footer~"         + mFooter.Value      + ";";
            Params += "I~S~Shape~"          + mShape.Value       + ";";

            mDenotePK  = DB.ExecLookup("cmn.mod_Denote", Params.ToString());

            mDataError = DB.DataError;
            if (String.IsNullOrEmpty(mDataError))
            {
                mDenoteType.Update();
                mDescription.Update();
                mHeader.Update();
                mBody.Update();
                mFooter.Update();
                mShape.Update();
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Denote", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }

        //public string getInsertSQL()
        //{

        //    string SQL = "declare @DenotePK int; " +  getColumns() + " ";
        //    SQL += "insert into enums.[Denote] ([DenoteType],[DenoteState],[MacAddr],[IPaddr],[Domain],[Inserted],[Modified]) ";
        //    SQL += "values (@DenoteType,@DenoteState,@MacAddr,@IPAddr,@Domain,getdate(),getdate()) ";
        //    SQL += "select @DenotePK = scope_identity() ";

        //    //SQL += "insert into cmn.[Event] ([ParentID],[HistoryType],[HistoryDate]) ";//,[Operator],[Location]) ";
        //    //SQL += "values (@DenotePK," + EnumCache.Instance.getTypeId("HistoryType", "Insert") + ",getdate()) ";

        //    return SQL;
        //}

        //public string getUpdateSQL()
        //{

        //    string SQL = getColumns();
        //    SQL += "update cmn.[Denote] set DenoteType=@DenoteType,DenoteState=@DenoteState,Venue=@Venue,Sharing=@Sharing,ShareState=@ShareState,Serial=@Serial,Fingerprint=@Fingerprint,MAC=@MAC,Modified=getdate() ";
        //    SQL += "where DenotePK=@DenotePK ";
        //    //SQL += "insert into cmn.[DenoteHistory] ([Denote],[HistoryType],[HistoryDate]) ";//,[Operator],[Location]) ";
        //    //SQL += "values (@DenotePK," + EnumCache.Instance.getTypeId("HistoryType", "Update") + ",getdate()) ";
        //    return SQL;
        //}

        //private string getColumns()
        //{
        //    string SQL = "";
        //    SQL += "declare @DenotePK int; Set @DenotePK=" + mDenotePK.ToString() + " ";
        //    SQL += "declare @DenoteType int; Set @DenoteType=" + mDenoteTypeId.ToString() + " ";
        //    SQL += "declare @MacAddr varchar(64); Set @MacAddr=" + mMACAddr + " ";
        //    SQL += "declare @IPAddr varchar(64); Set @IPAddr=" + mIPAddr + " ";
        //    SQL += "declare @Domain varchar(64); Set @Domain=" + mDomain + " ";
        //    return SQL;
        //}


        public void Find(string Column, string Value)
        {
        }

        public void Reset(string DenoteType)
        {
            mNewEntry = true;

            DenoteTypeId = EnumCache.Instance.getTypeId("Denote", DenoteType);

            mDenoteType.Reset();
            mDescription.Reset();
            mHeader.Reset();
            mBody.Reset();
            mFooter.Reset();
            mShape.Reset();

        }
    }
}
