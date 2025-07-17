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
    public class dlAbstract
    {

        private dlEntity   mEntity;           public dlEntity   Entity          { get { return mEntity; }              set { mEntity = value; } }
                                              public string     Description     { get { return mEntity.Description; }  set { mEntity.Description = value; } }
                                              public bool       NewEntry        { get { return mEntity.NewEntry; }     set { mEntity.NewEntry = value; } }

        private string     mAbstractPK = "";  public string     AbstractPK      { get { return mAbstractPK; }          set { mAbstractPK = value; } }

        private nxProperty mAbstractType;     public string     AbstractType    { get { return mAbstractType.Value; }  set { mAbstractType.Value = value;   mAbstractTypeID       = EnumCache.Instance.getTypeFromDesc ("Abstract", mAbstractType.Value); } }
        private string     mAbstractTypeID;   public string     AbstractTypeID  { get { return mAbstractTypeID; }      set { mAbstractTypeID = value;       mAbstractType.Value   = EnumCache.Instance.getDescFromId   (            mAbstractTypeID); } }

        private nxProperty mAbstractState;    public string     AbstractState   { get { return mAbstractState.Value; } set { mAbstractState.Value = value;  mAbstractStateID      = EnumCache.Instance.getStateFromDesc ("Abstract", mAbstractState.Value); } }
        private string     mAbstractStateID;  public string     AbstractStateID { get { return mAbstractStateID; }     set { mAbstractStateID = value;      mAbstractState.Value  = EnumCache.Instance.getDescFromId   (             mAbstractStateID); } }

        private nxProperty mValue;            public nxProperty Value           { get { return mValue; }               set { mValue = value; } }
        private nxProperty mModified;         public nxProperty Modified        { get { return mModified; }            set { mModified = value; } }

        private string     mDataError;        public string     DataError       { get { return mDataError; }           set { mDataError = value; } }

        public dlAbstract()
        {

            mAbstractType  = new nxProperty("Abstract", "AbstractType");
            mAbstractState = new nxProperty("Abstract", "AbstractState");
            mValue         = new nxProperty("Abstract", "Value");
            mModified      = new nxProperty("Abstract", "Modified");

            Reset(AbstractType);
        }

        public bool Open(string AbstractPK)
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Abstract", "I~S~AbstractPK~" + AbstractPK);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {
            mEntity.Open(Row["AbstractPK"].ToString());

            AbstractTypeID   = Row["AbstractType"].ToString();
            mValue.Value     = Row["Value"].ToString();
            mModified.Value  = Row["Modified"].ToString();

        }

        public bool Update()
        {

            string Params = "";
            if (!mEntity.NewEntry) Params = "I~S~AbstractPK~" + mEntity.EntityPK + ";";
            Params += "I~S~AbstractType~"  + mAbstractTypeID + ";";
            Params += "I~S~AbstractState~" + mAbstractStateID + ";";

            SQLServer DB = new SQLServer(setting.ConnectionString);
            mEntity.EntityPK = DB.ExecLookup("loc.mod_Abstract", Params + getParams()).ToString();
            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mEntity.NewEntry)
                {
                    mAbstractType.Update();
                    mAbstractState.Update();
                    mValue.Update();
                    mModified.Update();
                }
                Open(mEntity.EntityPK);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Abstract", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);


        }
       public string getParams()
        {
            string Params = " ";
            if (!String.IsNullOrEmpty(mValue.Value))    Params += "I~S~Value~" + mValue.Value  + ";";
            if (!String.IsNullOrEmpty(mModified.Value)) Params += "I~S~Modified~" + mModified.Value  + ";";
            Params += mEntity.getParams();
            return Params;
        }



        public void Find(string Column, string Value)
        {
        }

        public void Reset(string AbstractType)
        {
            mEntity.Reset(); 

            AbstractTypeID = EnumCache.Instance.getTypeId("Abstract", AbstractType);

            mAbstractType.Reset();
            mValue.Reset();
            mModified.Reset();

        }
    }
}
