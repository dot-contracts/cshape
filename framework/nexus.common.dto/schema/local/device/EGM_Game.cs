using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache; 

namespace nexus.common.dal
{

    public class dlEgm_Game
    {

        private string     mConnectionString = "";

        private bool       mNewEntry = true;   public bool     NewEntry        { get { return mNewEntry; } }
        private string     mDataError;         public string   DataError       { get { return mDataError; }           set { mDataError = value; } }

        private string     mGamePk;            public string   GamePk          { get { return mGamePk; }              set { mGamePk = value;         } }

        private nxProperty mManufacture;      public string   Manufacture    { get { return mManufacture.Value; }  set { mManufacture.Value = value;     } }
        private string     mManufactureId;    public string   ManufactureId  { get { return mManufactureId; }      set { mManufactureId = value;         } }

        private nxProperty mGameType;          public string   GameType        { get { return mGameType.Value; }      set { mGameType.Value = value;     } }
        private string     mGameTypeId;        public string   GameTypeId      { get { return mGameTypeId; }          set { mGameTypeId = value;         } }

        private nxProperty mGameState;         public string   GameState       { get { return mGameState.Value; }     set { mGameState.Value = value;     } }
        private string     mGameStateId;       public string   GameStateId     { get { return mGameStateId; }         set { mGameStateId = value;         } }

        private nxProperty mGameName;          public string   GameName        { get { return mGameName.Value; }      set { mGameName.Value = value;  } }
        private nxProperty mSpecNo;            public string   SpecNo          { get { return mSpecNo.Value; }        set { mSpecNo.Value = value;  } }

                                               public string   SystemId        { get; set; }
                                               public string   ShareState      { get; set; }
                                               public DateTime Inserted        { get; set; }
                                               public DateTime Modified        { get; set; }
                                               public string   GameGK          { get; set; }


        public dlEgm_Game() {}
        public void Create()
        {
            mManufacture = new nxProperty("Egm", "Manufacture");
            mGameType     = new nxProperty("Egm", "GameType");
            mGameName     = new nxProperty("Egm", "GameName");
            mSpecNo       = new nxProperty("Egm", "SpecNo");

            Reset(); 
        }

        public bool Open(int GamePK)    { return Open(GamePK.ToString()); }
        public bool Open(string GamePK)
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("loc.lst_Device_Egm_Game", "I~S~GamePK~" + GamePK);
                if (DT.Rows.Count > 0) LoadFromRow(DT.Rows[0]);
            }

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {


            mGamePk               = Row["GamePk"].ToString();

            mGameType.Create     (  Row["GameType"].ToString() ) ;
            mGameTypeId           = Row["GameTypeId"].ToString();

            mManufacture.Create (  Row["Manufacture"].ToString() ) ;
            mManufactureId       = Row["ManufactureId"].ToString();

            mGameName.Create     (  Row["GameType"].ToString() ) ;
            mSpecNo  .Create     (  Row["GameState"].ToString() );

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
            if (!string.IsNullOrEmpty(mGamePk)) Params = "I~S~GamePK~" + mGamePk + ";";
            mNewEntry = string.IsNullOrEmpty(Params);

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                mGamePk = DB.ExecLookup("loc.mod_Device_Egm_Game", Params + getParams()).ToString();
                mDataError = DB.DataError;
            }

            if (String.IsNullOrEmpty(mDataError))
            {
                UpdateProperty();
                Open(mGamePk);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Game", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);

        }
        public string getParams()
        {
            string Params = " ";
            Params += "I~S~GameType~"  + mGameTypeId   + ";";
            Params += "I~S~GameState~" + mGameStateId  + ";";
            Params += "I~S~GameName~"  + mGameName     + ";";
            Params += "I~S~SpecNo~"    + mSpecNo       + ";";

            if (!String.IsNullOrEmpty(SystemId))   Params += "I~N~SystemID~"   + SystemId + ";";
            if (!String.IsNullOrEmpty(ShareState)) Params += "I~N~ShareState~" + ShareState + ";";
            if (!String.IsNullOrEmpty(GameGK))     Params += "I~S~GameGK~"     + GameGK + ";";

            return Params;
        }
        public void UpdateProperty()
        {
            if (mNewEntry)
            {
                mManufacture.Update();
                mGameName.Update();
                mSpecNo.Update();
                mGameType.Update();
                mGameState.Update();
            }
        }


        public void Find(string Column, string Value)
        {

        }

        public void Dispose() { }

        public void Reset()
        {
            GameTypeId  = EnumCache.Instance.getTypeId  ("Game",  mGameType.Value);
            GameStateId = EnumCache.Instance.getStateId ("Entity", "Available");

            mGameType.Value  = GameType;
            mGameState.Value = GameState; 

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

        public int intGamePk()
        {
            int gamePk;
            int.TryParse(mGamePk, out gamePk);
            return gamePk; 
        }
        public override string ToString()
        {
            return mGameName.Value + ", " + mSpecNo.Value;
        }

        public bool Delete()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params  = "I~S~GamePK~"     + mGamePk + ";";
                       Params += "I~S~GameState~"  + EnumCache.Instance.getStateId("Entity", "Deleted");
                DB.ExecNonQuery ("loc.mod_Device_Egm_Game", Params);
                mDataError = DB.DataError;
            }

            return String.IsNullOrEmpty(mDataError);
        }


    }
}
