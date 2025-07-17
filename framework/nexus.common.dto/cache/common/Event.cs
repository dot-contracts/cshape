using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dal;

namespace nexus.common.cache
{
    public sealed class EventCache
    {
        private object    syncLock     = new object();
        private DataTable mEventData;

        public event         NewDataEventHandler NewData;
        public delegate void NewDataEventHandler();

        public DataTable LogData  { get { return mEventData; }  set { mEventData = value; } }

        private static EventCache mInstance = new EventCache();
        private EventCache() { }
        public static EventCache Instance { get { return mInstance; } }
        public bool Create()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                mEventData = DB.GetDataTable("cmn.lst_Occurrence_Event", "I~S~LimitRows~500");
            return true;
        }

        public bool InsertEvent(string EventType, string EventAction, string Property, string Description)
        {
            return InsertEvent(EnumCache.Instance.getTypeId("Event", EventType),  EventAction, "", setting.ComputerId, "", setting.WorkerId, "", Property, "", Description);
        }

        public bool InsertEvent(string EventType, string EventAction, string Property)
        {
            return InsertEvent(EnumCache.Instance.getTypeId("Event", EventType), EventAction, "", setting.ComputerId, "", setting.WorkerId, "", Property, "");
        }

        public bool InsertEvent(string EventType, string TypeCode, string ActionType, string ActionCode, string Property, string ExtensionId)
        {
            return InsertEvent(EnumCache.Instance.getTypeId(EventType, TypeCode), ActionCache.Instance.strId(TypeCode, ActionType, ActionCode), "", setting.ComputerId, "", setting.WorkerId, "0", Property, ExtensionId);
        }
        public bool InsertEvent(string EventType, string ActionType, string ParentID, string EntityID, string HumanID, string WorkerID, string FunctionID, string Property, string ExtensionId, string Description="")
        {

            lock (this.syncLock)
            {
                bool ret = false;

                try
                {
                    System.DateTime Start = DateTime.Now;

                    Property = Property.Replace("\n\r", " | ");

                    string Params = "I~N~EventStateID~" + nexus.common.cache.EnumCache.Instance.getStateId("Event", "Active") + ";";

                           if (!String.IsNullOrEmpty(ActionType))  Params += "I~N~EventActionID~" + ActionType + ";";
                           if (!String.IsNullOrEmpty(EventType))   Params += "I~N~EventTypeID~"   + EventType + ";";
                           if (!String.IsNullOrEmpty(ParentID))    Params += "I~N~ParentID~"      + ParentID + ";";
                           if (!String.IsNullOrEmpty(EntityID))    Params += "I~N~EntityID~"      + EntityID + ";";
                           if (!String.IsNullOrEmpty(HumanID))     Params += "I~N~HumanID~"       + HumanID + ";";
                           if (!String.IsNullOrEmpty(WorkerID))    Params += "I~N~WorkerID~"      + WorkerID + ";";
                           if (!String.IsNullOrEmpty(FunctionID))  Params += "I~N~FunctionID~"    + FunctionID + ";";
                           if (!String.IsNullOrEmpty(Property))    Params += "I~S~Property~"      + Property + ";";
                           if (!String.IsNullOrEmpty(Description)) Params += "I~S~Description~"   + Description;

                    string EventPK = "";
                    using (SQLServer DB = new SQLServer(setting.ConnectionString))
                    {
                        EventPK = DB.ExecLookup("cmn.mod_Occurrence_Event", Params).ToString();
                        using (DataTable DT = DB.GetDataTable("cmn.lst_Occurrence_Event", "I~S~EventPK~" + EventPK))
                        {
                            if (DT.Rows.Count > 0)
                            {
                                if (mEventData == null) Create();

                                while (mEventData.Rows.Count - 1 > 512)
                                {
                                    while (mEventData.Rows.Count - 1 > 512)
                                    {
                                        mEventData.Rows.RemoveAt(mEventData.Rows.Count - 1);
                                        if (DateTime.Now.Subtract(Start).TotalSeconds > 1) break;
                                    }

                                    DataRow newRow = mEventData.NewRow();
                                    for (int i = 0; i <= DT.Columns.Count - 1; i++)
                                    {
                                        newRow[i] = DT.Rows[0][i];
                                    }
                                    mEventData.Rows.InsertAt(newRow, 0);
                                }


                                ret = true;
                                NewData?.Invoke();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                }

                return ret;

            }
        }

        public string InsertEvent_Direct(string EventType, string ActionType, string Property)
        {
            return InsertEvent_Direct(EnumCache.Instance.getTypeId("Event", EventType), EnumCache.Instance.getTypeId("Action", ActionType), "", "", "", "", Property, "", "");
        }

        public string InsertEvent_Direct(string EventTypeId, string ActionTypeID, string ParentID, string EntityID, string WorkerID, string FunctionID, string Property, string PreSQL, string PostSQL)
        {

            lock (this.syncLock)
            {
                string EventPK = "";

                //EntityID   = (String.IsNullOrEmpty(EntityID))   ? " NULL " : EntityID;
                //FunctionID = (String.IsNullOrEmpty(FunctionID)) ? " NULL " : FunctionID;
                //ParentID   = (String.IsNullOrEmpty(ParentID))   ? " NULL " : ParentID;
                //WorkerID   = (String.IsNullOrEmpty(WorkerID))   ? " NULL " : WorkerID;

                //try
                //{
                //    System.DateTime Start = DateTime.Now;

                //    string SQL = PreSQL + " ";

                //    string Params = "";
                //    Params += "I~S~EventStateID~" + EnumCache.Instance.getTypeId("Event", "Active") + ";";
                //    Params += "I~S~EventTypeID~"   + EventTypeId + ";";
                //    Params += "I~S~EventActionID~" + ActionTypeID + ";";
                //    Params += "I~S~Property~"      + Property + ";";
                //    Params += "I~S~EntityID~"      + EntityID + ";";
                //    Params += "I~S~FunctionID~"    + FunctionID + ";";
                //    Params += "I~S~ParentID~"      + ParentID + ";";
                //    Params += "I~S~WorkerID~"      + WorkerID + ";";


                //    SQLServer DB = new SQLServer(setting.ConnectionString);
                //    EventPK = DB.ExecLookup("cmn.mod_Occurrence_Event", Params);
                //    DB.Dispose();
                //    DB = null;

                //    SQL += "declare @EventPK int execute @EventPK = cmn.mod_Occurrence ";
                //    SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event', 'Active') ";

                //    SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, ParentID, FunctionID, EntityID, WorkerID, Property) ";
                //    SQL += "values (@EventPK," + ActionTypeID + "," + EventTypeId + ",@EventState, getdate()," + ParentID + "," + FunctionID + "," + EntityID + "," + WorkerID + ",'" + Property + "') ";
                //    SQL += " " + PostSQL+ " "; 
                //    SQL += "Select @EventPK ";

                //    SQLServer DB = new SQLServer(setting.ConnectionString);
                //    EventPK = DB.ExecLookup(SQL);
                //    DB.Dispose();
                //    DB = null;

                //}
                //catch (Exception ex)
                //{
                //}

                return EventPK;

            }
        }

        public string InsertEGMFault(string EventPK, bool Active, string EgmId, string EventTypeId, string ActionTypeID, string EntityID, string WorkerID, string Property, string PreSQL, string PostSQL)
        {

            lock (this.syncLock)
            {
                string SQL = PreSQL + " ";

                WorkerID   = (String.IsNullOrEmpty(WorkerID))   ? " NULL " : WorkerID;


                if (!String.IsNullOrEmpty(EventPK))
                {
                    SQL += "update [cmn].[Event] set EventState=cmn.getStatePK('Event', 'InActive') where EventPK=" + EventPK; 
                }

                if (Active)
                {
                    try
                    {
                        SQL += "declare @EventPK int ";
                        SQL += "execute @EventPK = cmn.mod_Occurrence ";
                        SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event', 'Active') ";
                        SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityID, WorkerID, Property) ";
                        SQL += "values (@EventPK," + ActionTypeID + "," + EventTypeId + ",@EventState, getdate()," + EntityID + "," + WorkerID + ",'" + Property + "') ";
                        SQL += "update [gam].[current] set Fault=1, FaultCount = FaultCount + 1, FaultId=@EventPK where egmpk=" + EgmId;
                        SQL += " " + PostSQL + " ";
                        SQL += "Select @EventPK ";

                        SQLServer DB = new SQLServer(setting.ConnectionString);
                        EventPK = DB.ExecLookup(SQL);
                        DB.Dispose();
                        DB = null;
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    try
                    {
                        SQL += "IF not EXISTS(SELECT * FROM  [cmn].[Event] where EntityID=" + EntityID + " and EventAction = " + ActionTypeID + " and EventType=" + EventTypeId + ") ";
                        SQL += "update [gam].[current] set Fault=0, FaultId=null where egmpk=" + EgmId;
                        SQLServer DB = new SQLServer(setting.ConnectionString);
                        DB.ExecNonQuery(SQL);
                        DB.Dispose();
                        DB = null;
                    }
                    catch (Exception ex)
                    {
                    }
                }

                return EventPK;
            }
        }


        public bool AddEvent(string Worker, string Computer, string Event)
        {

            lock (this.syncLock)
            {
                bool ret = false;

                try
                {
                    System.DateTime Start = DateTime.Now;

                    while (mEventData.Rows.Count - 1 > 512)
                    {
                        mEventData.Rows.RemoveAt(mEventData.Rows.Count - 1);
                        if (DateTime.Now.Subtract(Start).TotalSeconds > 1) break;
                    }

                    DataRow newRow = mEventData.NewRow();
                    mEventData.Rows.InsertAt(newRow, 0);


                    ret = true;
                    if (NewData != null) NewData();

                }
                catch (Exception ex)
                {
                }

                return ret;

            }
        }



        public void Clear()
        {
            mEventData.Rows.Clear();
        }

        public string Description(string targetId)
        {
            //return mEvent.Description(targetId);
            return "";
        }


        public void Find(string Column, string Value)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }

        public void Reset(string EventType)
        {
            throw new NotImplementedException();
        }


        public string strId(string EventDefn) { return getId(EventDefn).ToString(); }
        public int getId(string EventDefn)
        {
            int ValueId = 0;


            return ValueId;
        }

        public string strIdFromDesc(string EventDesc) { return getIdFromDesc(EventDesc).ToString(); }

        public int getIdFromDesc(string EventDesc)
        {

            int ValueId = 0;


            return ValueId;
        }

        public string getDescFromId(string targetId)
        {
            string Desc = "";


            return Desc;

        }

        public string getList(string EventType)
        {

            string List = "";


            return List;
        }

        public DataTable getTable(string EventType)
        {
            Create();
            return mEventData;
        }

        public bool SyncWithWAN()
        {
            bool ret = true;


            return ret;
        }
    }
}
