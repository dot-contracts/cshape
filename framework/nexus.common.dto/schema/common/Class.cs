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
    public class dlClass
    {

        private dlEntity   mEntity;          public dlEntity Entity       { get { return mEntity; }                set { mEntity = value; } }
                                             public string   ClassPK      { get { return mEntity.EntityPK; }       set { mEntity.EntityPK = value; } }
                                             public string   Description  { get { return mEntity.Description; }    set { mEntity.Description = value; } }
                                             public bool     NewEntry     { get { return mEntity.NewEntry; }       set { mEntity.NewEntry = value; } }

        private string     mEntityID;        public string   EntityID     { get { return mEntityID; }              set { mEntityID = value; } }

        private nxProperty mClassType;       public string   ClassType    { get { return mClassType.Value; }  set { mClassType.Value = value;  mClassTypeID       = EnumCache.Instance.getTypeFromDesc ("Class", mClassType.Value); } }
        private string     mClassTypeID;     public string   ClassTypeID  { get { return mClassTypeID; }      set { mClassTypeID = value;      mClassType.Value   = EnumCache.Instance.getDescFromId   (          mClassTypeID); } }

        private nxProperty mClassState;      public string   ClassState   { get { return mClassState.Value; } set { mClassState.Value = value;  mClassStateID     = EnumCache.Instance.getStateFromDesc ("Class", mClassState.Value); } }
        private string     mClassStateID;    public string   ClassStateID { get { return mClassStateID; }     set { mClassStateID = value;      mClassState.Value = EnumCache.Instance.getDescFromId   (          mClassStateID); } }

        private nxProperty mGSpotX;          public nxProperty GSpotX     { get { return mGSpotX; }           set { mGSpotX = value; } }
        private nxProperty mGSpotY;          public nxProperty GSpotY     { get { return mGSpotY; }           set { mGSpotY = value; } }

        private string     mDataError;       public string     DataError  { get { return mDataError; }        set { mDataError = value; } }

        public dlClass()
        {

            mClassType  = new nxProperty("Class", "ClassType");
            mClassState = new nxProperty("Class", "ClassState");
            mGSpotX     = new nxProperty("Class", "GSpotX");
            mGSpotY     = new nxProperty("Class", "GSpotY");

            Reset(ClassType);
        }

        public bool Open(string ClassPK, string EntityID)
        {

            mEntityID = EntityID;

            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Class", "I~S~ClassPK~" + ClassPK  + ";I~S~EntityID~" + mEntityID);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {
            mEntity.Open(Row["ClassPK"].ToString());

            ClassTypeID   = Row["ClassType"].ToString();
            mGSpotX.Value = Row["GSpotX"].ToString();
            mGSpotY.Value = Row["GSpotY"].ToString();

        }

        public bool Update()
        {

            string Params = "";
            if (!mEntity.NewEntry) Params = "I~S~ClassPK~" + mEntity.EntityPK + ";";
            Params += "I~S~ClassType~"  + mClassTypeID + ";";
            Params += "I~S~ClassState~" + mClassStateID + ";";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            mEntity.EntityPK = DB.ExecLookup("loc.mod_Class", Params + getParams()).ToString();
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mEntity.NewEntry)
                {
                    mClassType.Update();
                    mClassState.Update();
                    mGSpotX.Update();
                    mGSpotY.Update();
                }
                Open(mEntity.EntityPK, mEntityID);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Class", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);


        }
       public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mGSpotX.Value)) Params += "I~S~GSpotX~" + mGSpotX.Value  + ";";
            if (!String.IsNullOrEmpty(mGSpotY.Value)) Params += "I~S~GSpotY~" + mGSpotY.Value  + ";";
            Params += mEntity.getParams();
            return Params;
        }



        public void Find(string Column, string Value)
        {
        }

        public void Reset(string ClassType)
        {
            mEntity.Reset(); 

            ClassTypeID = EnumCache.Instance.getTypeId("Class", ClassType);

            mClassType.Reset();
            mGSpotX.Reset();
            mGSpotY.Reset();

        }
    }
}
