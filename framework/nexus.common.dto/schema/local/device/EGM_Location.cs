using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlEgm_Game_Location
    {

        private string     mConnectionString = "";

        private bool       mNewEntry = true;   public bool     NewEntry        { get { return mNewEntry; } }
        private string     mDataError;         public string   DataError       { get { return mDataError; }           set { mDataError = value; } }

        private dlEgm      mEgm;               public dlEgm    Egm             { get { return mEgm; }                 set { mEgm  = value; } }

        private string     mVarnoPk;           public string   VarnoPk         { get { return mVarnoPk; }             set { mVarnoPk = value;         } }


        private nxProperty mVarnoState;        public string   VarnoState      { get { return mVarnoState.Value; }    set { mVarnoState.Value = value;     } }
        private string     mVarnoStateId;      public string   VarnoStateId    { get { return mVarnoStateId; }        set { mVarnoStateId = value;         } }

        private nxProperty mVarno;             public string   Varno           { get { return mVarno.Value; }         set { mVarno.Value = value;  } }
        private nxProperty mDenom;             public string   Denom           { get { return mDenom.Value; }         set { mDenom.Value = value;  } }
        private nxProperty mRTP;               public string   RTP             { get { return mRTP.Value; }           set { mRTP.Value = value;  } }

                                               public string   SystemId        { get; set; }
                                               public string   ShareState      { get; set; }
                                               public DateTime Inserted        { get; set; }
                                               public DateTime Modified        { get; set; }
                                               public string   VarnoGK         { get; set; }

        public dlEgm_Game_Location() {}
        public void Create()
        {
            mVarnoState = new nxProperty("Egm", "VarnoState");
            mVarno      = new nxProperty("Egm", "Varno");
            mDenom      = new nxProperty("Egm", "Denom");
            mRTP        = new nxProperty("Egm", "RTP");

            Reset(); 
        }

        public bool Open(int VarnoPK)    { return Open(VarnoPK.ToString()); }
        public bool Open(string VarnoPK)
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.lst_Device_Egm_Varno", "I~S~VarnoPK~" + VarnoPK);
                if (DT.Rows.Count > 0) LoadFromRow(DT.Rows[0]);
            }

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {


            mVarnoPk               = Row["VarnoPk"].ToString();

            mVarnoState.Create     (  Row["VarnoState"].ToString() ) ;
            mVarnoStateId           = Row["VarnoStateId"].ToString();

            mVarno.Create          (  Row["Varno"].ToString() ) ;
            mDenom.Create          (  Row["Denom"].ToString() ) ;
            mRTP  .Create          (  Row["RTP"].ToString() );

            SystemId                =                Row["SystemId"    ].ToString();
            ShareState              =                Row["ShareStateId"].ToString();
            Inserted                = helpers.ToDate(Row["Inserted"    ].ToString());
            Modified                = helpers.ToDate(Row["Modified"    ].ToString());
            SystemId                =                Row["SystemId"    ].ToString();

        }

        /// <summary>
        /// 
        /// </summary>
        public bool Update()
        {
            string Params = "";
            if (!string.IsNullOrEmpty(mVarnoPk)) Params = "I~S~VarnoPK~" + mVarnoPk + ";";
            mNewEntry = string.IsNullOrEmpty(Params);

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                mVarnoPk = DB.ExecLookup("loc.mod_Device_Egm_Varno", Params + getParams()).ToString();
                mDataError = DB.DataError;
            }

            if (String.IsNullOrEmpty(mDataError))
            {
                UpdateProperty();
                Open(mVarnoPk);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Varno", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        {
            string Params = " ";
            Params += "I~S~VarnoState~" + mVarnoStateId  + ";";
            Params += "I~S~Varno~"  + mVarno  + ";";
            Params += "I~S~Denom~"  + mDenom  + ";";
            Params += "I~S~RTP~"    + mRTP    + ";";

            if (!String.IsNullOrEmpty(SystemId))   Params += "I~N~SystemID~"   + SystemId + ";";
            if (!String.IsNullOrEmpty(ShareState)) Params += "I~N~ShareState~" + ShareState + ";";
            if (!String.IsNullOrEmpty(VarnoGK))    Params += "I~S~VarnoGK~"    + VarnoGK + ";";

            return Params;
        }
        public void UpdateProperty()
        {
            if (mNewEntry)
            {
                mVarno.Update();
                mDenom.Update();
                mRTP.Update();
                mVarnoState.Update();
            }
        }


        public void Find(string Column, string Value)
        {

        }

        public void Dispose() { }

        public void Reset()
        {
            VarnoStateId = EnumCache.Instance.getStateId ("Entity", "Available");
            mVarnoState.Value = VarnoState; 

            mNewEntry = true;
        }

        public string GetSettings()
        {
            string SaveStr = "Id=";// +mVenueID;
            //SaveStr += ";No=" + mVenueNo;
            //SaveStr += ";Type=" + mVenueType;
            //SaveStr += ";Name=" + mVenueName;
            return SaveStr;
        }

        public int intVarnoPk()
        {
            int VarnoPk;
            int.TryParse(mVarnoPk, out VarnoPk);
            return VarnoPk; 
        }
        public override string ToString()
        {
            return mVarno.Value + ", " + mRTP.Value;
        }

        public bool Delete()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params  = "I~S~VarnoPK~"     + mVarnoPk + ";";
                       Params += "I~S~VarnoState~"  + EnumCache.Instance.getStateId("Entity", "Deleted");
                DB.ExecNonQuery ("loc.mod_Device_Egm_Varno", Params);
                mDataError = DB.DataError;
            }

            return String.IsNullOrEmpty(mDataError);
        }


    }
}
