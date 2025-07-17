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
using nexus.common.cache; 

namespace nexus.common.dal
{
    public class dlEndpoint
    {

        private string     mConnectionString = "";
        private string     mDataError        = "";

        private string     mEndpointUK = "";  public string EndpointUK      { get { return mEndpointUK; }          set { mEndpointUK = value; } }
        private string     mEndpointTypeId;   public string EndpointTypeId  { get { return mEndpointTypeId; }      set { mEndpointTypeId = value;  } }
        private string     mEndpointStateId;  public string EndpointStateId { get { return mEndpointStateId; }     set { mEndpointStateId = value; } }

        private string     mEntityID = "";    public string EntityID        { get { return mEntityID; }            set { mEntityID = value; } }
        private string     mMACAddr = "";     public string MACAddr         { get { return mMACAddr; }             set { mMACAddr = value; } }
        private string     mIPAddr ="";       public string IPAddr          { get { return mIPAddr; }              set { mIPAddr = value; } }
        private string     mDomain = "";      public string Domain          { get { return mDomain; }              set { mDomain = value; } }

        private bool mNewEntry; public bool NewEntry { get { return mNewEntry; } set { mNewEntry = value; } }

        public dlEndpoint(string ConnectionString)
        {
            mConnectionString = ConnectionString;
            Reset("Local");
        }

        public bool Open(string EndpointUK)
        {

            mNewEntry = true;
            mEndpointUK = EndpointUK;

            SQLServer DB = new SQLServer(mConnectionString);

            DataTable DT = DB.GetDataTable("loc.lst_EndpointUK", "I~S~EndpointUK~" + mEndpointUK);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {
            if (Row != null)
            {
                mNewEntry = false;
                EndpointTypeId   = Row["EndpointType"].ToString();
                mEndpointStateId = Row["EndpointState"].ToString();
                EntityID         = Row["EntityID"].ToString();
                MACAddr          = Row["MACAddr"].ToString();
                IPAddr           = Row["IPAddr"].ToString();
                Domain           = Row["Domain"].ToString();
            }
        }

        public bool Update()
        {

            int entId;
            int.TryParse(mEntityID, out entId);
            if (!String.IsNullOrEmpty(mMACAddr) & entId>0)
            {
                SQLServer DB = new SQLServer(mConnectionString);

                string Params = "";

                if (!mNewEntry)
                    Params = "I~S~EndpointUK~" + mEndpointUK + ";";

                Params += "I~S~EndPointType~"  + mEndpointTypeId + ";";
                Params += "I~S~EndPointState~" + mEndpointStateId + ";";
                Params += "I~S~EntityID~"      + mEntityID + ";";
                Params += "I~S~MACAddr~"       + mMACAddr + ";";
                Params += "I~S~IPAddr~"        + mIPAddr + ";";
                Params += "I~S~Domain~"        + mDomain;


                mEndpointUK = DB.ExecLookup("loc.mod_EndPoint", Params).ToString();

                mDataError = DB.DataError;
                if (!String.IsNullOrEmpty(mDataError)) EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "EndPoint", "UpdateError"), mDataError);
                DB.Dispose();
                DB = null;

            }
            mNewEntry = false;

            return String.IsNullOrEmpty(mDataError);

        }

        public string LookUp(string MAC)
        {
            SQLServer DB = new SQLServer(mConnectionString);
            mEndpointUK = DB.ExecLookup("select EntityID from loc.endpoint where macaddr='" + MAC + "' and endpointstate=cmn.getStatePK('Entity','Active'))");
            DB.Dispose();
            DB = null;
            return mEndpointUK; 
        }

        public void Reset(string EndpointType)
        {
            mNewEntry = true;

            EndpointTypeId  = EnumCache.Instance.getTypeId  ("Endpoint", EndpointType);
            EndpointStateId = EnumCache.Instance.getStateId ("Entity",   "Active");

        }

    }
}
