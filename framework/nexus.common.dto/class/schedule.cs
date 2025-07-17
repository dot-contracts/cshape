using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nexus.common.dal;
using nexus.common.core;
using nexus.common.cache;
//using nexus.common.control;

namespace nexus.common.dto
{
    public class Schedule
    {
        public enum ScheduleTypes  { System, Media, Advert, Special, Announce, Music, Daily, Gaming, EGM, Cashier, Member, Holiday, Hitch, User }
       // public enum ScheduleStates { disabled, neverrun, waiting, playing, finished, deleted, error }
        public enum IntervalTypes  { Allways, Basic, Hour, Day, Week, Month, Year }
        public enum DayTypes       { Mon, Tue, Wed, Thr, Fri, Sat, Sun }

        private string mScheduleSource;
        private string mScheduleFilter;

        private DataTable mSchedules;


        private string mType;
        private int mStateNeverRun;
        private int mStateWaiting;
        private int mStatePlaying;
        private int mStateFinished;
        private int mStateDeleted;
        private int mState;
        private IntervalTypes  mIntType;

        private bool      mIsBusy = false;
        private string    mTypeId     = "";
        private string    mEntityId   = "";
        private string    mScheduleId = "";

        private bool      mShowDeleted;
        private bool      mShowDisabled;
        private string    mScheduleStart = "";
        private DateTime  mLastRefresh = DateTime.Now.AddDays(-1);
        private DateTime  mLastStateRefresh = DateTime.Now.AddDays(-1);
        private bool      mFirstRun = true;

        public event OnNewDataEventHandler        OnNewData;         public delegate void OnNewDataEventHandler();
        public event OnPreviewStartEventHandler   OnPreviewStart;    public delegate void OnPreviewStartEventHandler  (string ScheduleID, string EntityId);
        public event OnScheduleStartEventHandler  OnScheduleStart;   public delegate void OnScheduleStartEventHandler (string ScheduleID, string EntityId);
        public event OnScheduleFinishEventHandler OnScheduleFinish;  public delegate void OnScheduleFinishEventHandler(string ScheduleID, string EntityId);

        public Schedule()
        {
            mSchedules = new DataTable();

            mStateNeverRun = EnumCache.Instance.intState("Schedule", "NeverRun");
            mStateWaiting  = EnumCache.Instance.intState("Schedule", "Waiting");
            mStatePlaying  = EnumCache.Instance.intState("Schedule", "Playing");
            mStateFinished = EnumCache.Instance.intState("Schedule", "Finished");
            mStateDeleted  = EnumCache.Instance.intState("Schedule", "Deleted");
        }

        public bool Create(ScheduleTypes ScheduleType, string EntityId, bool ShowDeleted = false, bool ShowDisabled = false)
        {
            return Create(ScheduleType.ToString(), EntityId, ShowDeleted, ShowDisabled);
        }

        public bool Create(string ScheduleType, string EntityId, bool ShowDeleted, bool ShowDisabled)
        {
            mType          = ScheduleType;
            mEntityId      = EntityId;
            mShowDeleted   = ShowDeleted;
            mShowDisabled  = ShowDisabled;
            mScheduleStart = DateTime.Now.ToString("HH:mm tt");

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                mTypeId = DB.ExecLookup("select valuepk from cmn.vue_EnumValue where enumcode='schedule' and valuetype=1 and valuecode='" + mType + "'");

            return true;
        }

        public bool Create(string FilterPair, bool ShowDeleted = false, bool ShowDisabled = false)
        {

            Array Arr = FilterPair.Split('=');

            mScheduleSource = Arr.GetValue(0).ToString();
            mScheduleFilter = Arr.GetValue(1).ToString();

            return ReFresh();

        }
        public bool ReFresh() 
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                switch (mScheduleSource)
                {
                    case "PromotionType":
                        mSchedules = DB.GetDataTable("Select PromotionPk as EntityId, ScheduleId, ScheduleState, Active, InType, [Starts], StartsOn, StartsAt, Finishes, FinishesOn, FinishesAt, Days, Hours, Priority, ScheduleDur, Repeats, NextRun, LastRun, Runs, MaxRuns from pro.vue_promotion where isnull(Scheduleid,0)>0 and PromotionStateId<>cmn.getStatePK('Promotion','Deleted') and Active=1 and PromotionType='" + mScheduleFilter + "' order by Starts");
                        AddColumns();
                        return true;

                    default:
                        break;
                }
            }

            for (int i = 0; i < mSchedules.Rows.Count; i++)  UpdateStarts(mSchedules.Rows[i]);
            
            return false;
        }

        private void AddColumns()
        {
            mSchedules.Columns.Add(new DataColumn("Repeat",     typeof(Int32)));
            //mSchedules.Columns.Add(new DataColumn("StateValue", typeof(Int32)));
            mSchedules.AcceptChanges();
            for (int i = 0; i < mSchedules.Rows.Count; i++)
                mSchedules.Rows[i]["ScheduleState"] = mStateNeverRun;
        }

        public bool Process()
        {
            bool ReQueue  = false;
            var LastState = mState;
            var ThisState = mState;
            var Starts    = DateTime.Now;
            var Finishes  = DateTime.Now;

            if (!mIsBusy)
            {
                mIsBusy = true;
                mState = mStateWaiting;
                if (mSchedules.Rows.Count > 0)
                {
                    try
                    {
                        DataRow Row = mSchedules.Rows[0];

                        Starts      = helpers.ToDate(  Row["Starts"].ToString());
                        Finishes    = helpers.ToDate(  Row["Finishes"].ToString());
                        LastState   = helpers.ToInt (  Row["ScheduleState"].ToString());
                        ThisState   = LastState;

                        mScheduleId = Row["ScheduleId"].ToString();
                        mEntityId   = Row["EntityId"].ToString();

                        if (ThisState.Equals(mStateNeverRun))
                        {
                           ThisState = mStateWaiting;
                           if (DateTime.Now.Subtract(Finishes).TotalSeconds < 5d) OnPreviewStart?.Invoke(mScheduleId, mEntityId);
                           else                                                   ReQueue = true;
                        }
                        else if (ThisState.Equals(mStateWaiting))
                        {
                            if (DateTime.Now.Subtract(Starts).TotalSeconds > 0d) ThisState = RunCurrent();
                        }
                        else if (ThisState.Equals(mStatePlaying))
                        {
                            if (DateTime.Now.Subtract(Finishes).TotalSeconds > 0d)
                            {
                                ThisState = mStateFinished;
                                ReQueue   = true;
                            }
                        }
                        else if (ThisState.Equals(mStateFinished) || ThisState.Equals(mStateDeleted))
                        {
                            ReQueue = true;
                        }


                        if (ReQueue)
                        {
                            if (DateTime.Now.Subtract(Finishes).TotalSeconds > 0d)
                            {

                                bool Remove = true;
                                if (!mIntType.Equals(IntervalTypes.Basic)) // if its a basic schedule remove it .. its done.
                                    Remove = true;//  !IsActive(ref Row, false, true);

                                int Repeat = helpers.ToInt(Row["Repeat"].ToString());  Repeat += 1;  mSchedules.Rows[0]["Repeat"] = Repeat;
                                int Plays  = helpers.ToInt(Row["Runs"].ToString());    Plays  += 1;  mSchedules.Rows[0]["Runs"]   = Plays;

                                Row["NextRun"] = Row["Starts"];

                                ThisState = mStateNeverRun;
                                mSchedules.Rows[0]["ScheduleState"] = ThisState;
                                bool Scheduled = (ThisState == mStateWaiting | ThisState == mStatePlaying);

                                UpdateStarts(Row);

                                OnScheduleFinish?.Invoke(mScheduleId, mEntityId);
                                EventCache.Instance.InsertEvent("Information", "Schedule", "Finish " + mScheduleId + "," + mEntityId);

                                if (Remove | mIntType.Equals(IntervalTypes.Basic))  mSchedules.Rows.RemoveAt(0);
                                else                                                ReOrder();

                                if (mSchedules.Rows.Count.Equals(0)) ReFresh();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    if (mSchedules.Rows.Count>0)
                    {
                        mSchedules.Rows[0]["ScheduleState"] = ThisState;
                        //if  (ThisState != LastState) mSchedules.Rows[0]["ScheduleState"] = ThisState;
                        //else ReFresh();
                    }
                }
                else if (DateTime.Now.Subtract(mLastRefresh).TotalMinutes > 1d)  ReFresh();
                mIsBusy = false;
            }

            mIsBusy = false;
            return mSchedules.Rows.Count > 0;
        }

        public void SetState(int State)
        {
            mSchedules.Rows[0]["ScheduleState"] = State;
        }

        private void ReOrder()
        {
            mSchedules.Rows[0]["ScheduleState"] = mStateNeverRun;
            DataView DataView = new DataView(mSchedules);
            DataView.Sort = "Starts";
            mSchedules = DataView.ToTable();
            DataView.Dispose();
            DataView = null;
        }

        public int RunCurrent()
        {
            int ThisState = mStateFinished;
            {   // run current schedule
                OnScheduleStart?.Invoke(mScheduleId, mEntityId);
                EventCache.Instance.InsertEvent("Information", "Schedule", "Start " + mScheduleId + "," + mEntityId);
                ThisState = mStatePlaying;

                mSchedules.Rows[0]["LastRun"] = DateTime.Now;//.ToString("MMM dd, yyyy HH:mm");
                mSchedules.Rows[0]["ScheduleState"] = ThisState;
                UpdateStarts(mSchedules.Rows[0]);
            }
            return ThisState;
        }

        public void Finished()
        {
            if (mSchedules.Rows.Count > 0)
            {
                if (mType.Equals(ScheduleTypes.Music))      mSchedules.Rows[0]["Finishes"] = DateTime.Now;

                mSchedules.Rows[0]["ScheduleState"] = mStateFinished;
                EventCache.Instance.InsertEvent("Information", "Schedule", "Finished " + mScheduleId + "," + mEntityId);
            }
        }

        public string InsertSchedule(ScheduleTypes ScheduleType)
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                mScheduleId = DB.ExecLookup("loc.mod_Schedule", "I~S~ScheduleType~" + ScheduleType).ToString();

            return mScheduleId;
        }

        public void DeleteSchedule(string ScheduleId)
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                DB.ExecLookup("loc.mod_Schedule", "I~S~SchedulePk~" + ScheduleId  + ";I~S~ScheduleState~" + EnumCache.Instance.getState("Shedule","Deleted") ).ToString();

            ReFresh();
        }

        public void UpdateStarts(DataRow Row)
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params  =  "I~S~SchedulePk~"    + Row["ScheduleId"].ToString();
                       Params += ";I~S~ScheduleState~" + helpers.ToInt (Row["ScheduleState"].ToString());
                       if (helpers.ToDate(Row["Starts"].ToString()).Year > 2022)
                          Params += ";I~D~starts~"        + helpers.ToDate(Row["Starts"]  .ToString()).ToString("MMM dd, yyyy HH:mm");
                       if (helpers.ToDate(Row["finishes"].ToString()).Year >2022)
                          Params += ";I~D~finishes~"      + helpers.ToDate(Row["finishes"].ToString()).ToString("MMM dd, yyyy HH:mm");
                       if (helpers.ToDate(Row["NextRun"].ToString()).Year > 2022)
                          Params += ";I~D~NextRun~"       + helpers.ToInt (Row["NextRun"].ToString()).ToString("MMM dd, yyyy HH:mm");
                       if (helpers.ToDate(Row["LastRun"].ToString()).Year > 2022)
                          Params += ";I~D~LastRun~" + helpers.ToDate(Row["LastRun"].ToString()).ToString("MMM dd, yyyy HH:mm");
                Params += ";I~S~Runs~"          + helpers.ToInt (Row["Runs"].ToString());

                DB.ExecLookup("loc.mod_Schedule", Params);
            }
        }


        public int TimeToSerial(string txTime)
        {
            return helpers.ToDate(txTime).Hour + Convert.ToDateTime(txTime).Minute * 60;
        }

        public string SerialToTime(string intSerial)
        {
            var Hr  = default(int);
            var Min = default(int);
            if (Convert.ToInt32(intSerial) > 0d)
            {
                Hr  = (int)Math.Round (helpers.ToDbl(intSerial) % 60d);
                Min = (int)Math.Round((helpers.ToDbl(intSerial) - Hr) / 60d);
            }

            return Hr.ToString("00") + ":" + Min.ToString("00");
        }



        //public bool ReFresh()                                                       { return ReFresh(false, false, true);                }
        //public bool ReFresh(bool ShowDeleted, bool ShowDisabled)                    { return ReFresh(ShowDeleted, ShowDisabled, true);   }
        //public bool ReFresh(bool ShowDeleted, bool ShowDisabled, bool CalcStarts)
        //{
        //    //mShowDeleted  = ShowDeleted;
        //    //mShowDisabled = ShowDisabled;
        //    //mIsBusy       = true;
        //    //mLastRefresh  = DateTime.Now;

        //    //mSchedules.Rows.Clear();

        //    //using (SQLServer DB = new SQLServer(setting.ConnectionString))
        //    //{
        //    //    if (helpers.ToInt (mTypeId)>0)
        //    //        mSchedules = DB.GetDataTable("loc.lst_Schedule", "I~S~ScheduleTypeID~" + mTypeId);

        //    //    mSchedules =
        //    //    string Filter   = GetTableFilter(mShowDeleted);
        //    //    bool ResetState = DateTime.Now.Subtract(mLastStateRefresh).TotalDays > 1d;
        //    //    //DB.ExecNonQuery("Update media.dbo.Schedule set scheduled=0", ResetState ? ",state=" + ScheduleStates.neverrun : " ");


        //    //    //               SchedulePK,Entity,Description,ScheduleType,ScheduleState,ScheduleCode,IntType,Starts,StartsOn,StartsAt,Finishes,FinishesOn,FinishesAt,Days,Hours,Priority,Duration,Repeats,
        //    //    // EntityType, EntityTypeId, TypeValue, StateValue,IntTypeValue,EntityId,NextRun,LastRun,Runs,MaxRuns,ScheduleTypeId,ScheduleStateId

        //    //    DT = DB.GetDataTable("loc.lst_Schedule", "I~S~ScheduleTypeID~" + mTypeId);

        //    //    for (int i = 0; i < DT.Rows.Count; i++)
        //    //    {
        //    //        try
        //    //        {
        //    //            DataRow  Row = DT.Rows[i];
        //    //            DateTime StartDate  = DateTime.Now;
        //    //            bool     Process    = false;
        //    //            double   Increment  = 0d;
        //    //            string   FinishTime = "";
        //    //            bool     NextPlay   = false;

        //    //            DateTime LastPlay;
        //    //            if (!DateTime.TryParse(Row["LastRun"].ToString(), out LastPlay))
        //    //                LastPlay = DateTime.Now.AddDays(-8);

        //    //            mState = (ScheduleStates)Row["StateValue"];

        //    //            if ( !mState.Equals(ScheduleStates.disabled) | mShowDisabled)
        //    //            {
        //    //                Row["StartsOn"]   = string.IsNullOrEmpty(Row["StartsOn"].ToString())   ? "AllDay" : Row["StartsOn"].ToString();
        //    //                Row["StartsAty"]  = string.IsNullOrEmpty(Row["StartsAt"].ToString())   ? "AllDay" : Row["StartsAt"].ToString();
        //    //                Row["FinishesOn"] = string.IsNullOrEmpty(Row["FinishesOn"].ToString()) ? "Never"  : Row["FinishesOn"].ToString();
        //    //                Row["FinishesAt"] = string.IsNullOrEmpty(Row["FinishesAt"].ToString()) ? "Never"  : Row["FinishesAt"].ToString();

        //    //                if (IsActive(ref Row, false, CalcStarts) | mShowDisabled)
        //    //                {
        //    //                    Process = true;


        //    //                    //string Code = Row["Code"].ToString();

        //    //                    if (!DateTime.TryParse(Row["Starts"].ToString(), out StartDate)) StartDate = DateTime.Now;

        //    //                    //mScheduleID = Conversions.ToString(Row["Id"]);

        //    //                    mType    = (ScheduleTypes)Row["TypeValue"];
        //    //                    mIntType = (IntervalTypes)Row["IntTypeValue"];

        //    //                    switch (mType)
        //    //                    {
        //    //                        case ScheduleTypes.Announce:
        //    //                            if (mIntType.Equals((IntervalTypes.Basic)))
        //    //                                Process = true; 
        //    //                            else
        //    //                                Process = mShowDisabled | DateTime.Now.Subtract(StartDate).TotalMinutes < 1d;
        //    //                            break;

        //    //                        case ScheduleTypes.Music:
        //    //                            Process = mFirstRun | mState.Equals(ScheduleStates.neverrun);
        //    //                            break;
        //    //                    }

        //    //                    if (Process)
        //    //                    {
        //    //                        DataRow newSched = mSchedules.NewRow();

        //    //                        newSched["Id"]              = Row["SchedulePk"].ToString();
        //    //                        newSched["DisplayTime"]     = StartDate.ToString("dd HH:mm");

        //    //                        newSched["Entity"]          = Row["Entity"].ToString();
        //    //                        newSched["Description"]     = Row["Description"].ToString();
        //    //                        newSched["ScheduleType"]    = Row["ScheduleType"].ToString();
        //    //                        newSched["ScheduleState"]   = Row["ScheduleState"].ToString();
        //    //                        newSched["ScheduleCode"]    = Row["ScheduleCode"].ToString();
        //    //                        newSched["IntType"]         = Row["IntType"].ToString();

        //    //                        newSched["Starts"]          = Row["Starts"];
        //    //                        newSched["StartsAt"]        = Row["StartsAt"].ToString();
        //    //                        newSched["StartsOn"]        = Row["StartsOn"].ToString();
        //    //                        newSched["Finishes"]        = Row["Finishes"];
        //    //                        newSched["FinishesAt"]      = Row["FinishesAt"].ToString();
        //    //                        newSched["FinishesOn"]      = Row["FinishesOn"].ToString();

        //    //                        newSched["Days"]            = Row["Days"].ToString();
        //    //                        newSched["Hours"]           = Row["Hours"].ToString();

        //    //                        newSched["Priority"]        = Convert.ToInt32(Row["Priority"].ToString());
        //    //                        newSched["Duration"]        = Convert.ToInt32(Row["Duration"].ToString());
        //    //                        newSched["Repeats"]         = Row["Repeats"].ToString();
        //    //                        newSched["Increment"]       = Row["Increment"].ToString();

        //    //                        newSched["EntityType"]      = Row["EntityType"].ToString();
        //    //                        newSched["EntityTypeId"]    = Row["EntityTypeId"].ToString();
        //    //                        newSched["TypeValue"]       = Row["TypeValue"].ToString();
        //    //                        newSched["StateValue"]      = Row["StateValue"].ToString();
        //    //                        newSched["IntTypeValue"]    = Row["IntTypeValue"].ToString();
        //    //                        newSched["EntityId"]        = Row["EntityId"].ToString();
        //    //                        newSched["ScheduleTypeId"]  = Row["ScheduleTypeId"].ToString();
        //    //                        newSched["ScheduleStateId"] = Row["ScheduleStateId"].ToString();

        //    //                        newSched["NextRun"]         = Row["NextRun"].ToString();
        //    //                        newSched["LastRun"]         = Row["LastRun"].ToString();
        //    //                        newSched["Runs"]            = Row["Runs"];
        //    //                        newSched["MaxRuns"]         = Convert.ToInt32(Row["MaxRuns"].ToString());

        //    //                        mSchedules.Rows.Add(newSched);
        //    //                    }
        //    //                }
        //    //            }

        //    //            if (CalcStarts) UpdateStarts(Row);
        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //            //common.Props.Instance.InsertLogEntry(true, mZoneId, "Common", "Schedules", "LoadRaw", "Exception", mMediaType + " " + ex.Message);
        //    //        }
        //    //    }
        //    //}

        //    //if (mSchedules.Rows.Count > 0)
        //    //{
        //    //    var DataView = new DataView(mSchedules);
        //    //    DataView.Sort = "Code,Active desc,Starts,Priority";
        //    //    mSchedules = DataView.ToTable();
        //    //    DataView.Dispose();
        //    //    DataView = null;

        //    //    if (mType.Equals(ScheduleTypes.Music))
        //    //    {
        //    //        if (mSchedules.Rows.Count > 1)
        //    //        {
        //    //            for (int i = 0; i <= mSchedules.Rows.Count - 2; i++)
        //    //                mSchedules.Rows[i]["Finishes"] = mSchedules.Rows[i + 1]["Starts"];
        //    //        }
        //    //    }
        //    //}

        //    //mIsBusy      = false;
        //    //mLastRefresh = DateTime.Now;
        //    //mFirstRun    = false;

        //    return mSchedules.Rows.Count > 0;
        //}

        private string GetTableFilter(bool ShowDeleted)
        {
            string Filter = "";
            if (!ShowDeleted) Filter = "deleted=0 ";

            // If Filter <> "" Then Filter &= " and "
            //switch (mMediaType.ToUpper() ?? "")
            //{
            //    case "HD":     case "FULL": case "SIDE": 
            //    case "BANNER": case "TOP":  case "BOTTOM":   Filter += (!string.IsNullOrEmpty(Filter)? " and ": "") +  " mediatype like '%" + mMediaType +  "%'";                        break;
            //    case "SCROLL": case "SMARTART":              Filter += (!string.IsNullOrEmpty(Filter)? " and ": "") +  " mediatype like '%" + mMediaType +  "%'";                        break;
            //    case "ANNOUNCE":
            //            if (mArea.ToUpper().Equals("ADHOC")) Filter += (!string.IsNullOrEmpty(Filter) ? " and " : "") + " Code = '" + mMediaType + "'"  + (!string.IsNullOrEmpty(mArea), " and mediatype = '" + mArea + "'", ""); break;
            //        Filter = Conversions.ToString(Filter + Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Interaction.IIf(!string.IsNullOrEmpty(Filter), " and ", ""), " (Code='"), mMediaType), "') "), Interaction.IIf(!string.IsNullOrEmpty(mArea), " and mediatype = '" + mArea + "'", "")));
            //            else
            //                Filter = Conversions.ToString(Filter + Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Interaction.IIf(!string.IsNullOrEmpty(Filter), " and ", ""), " mediatype like '%"), mMediaType), "%'"));

            //            break;

            //    case "ADVERTS":
            //        {
            //            Filter = Conversions.ToString(Filter + Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Interaction.IIf(!string.IsNullOrEmpty(Filter), " and ", ""), " (Code='"), mMediaType), "' or Code='SyncDir') "), Interaction.IIf(!string.IsNullOrEmpty(mArea), " and mediatype = '" + mArea + "'", "")));
            //            break;
            //        }

            //    case "BGM":
            //        {
            //            Filter = Conversions.ToString(Filter + Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Interaction.IIf(!string.IsNullOrEmpty(Filter), " and ", ""), " Code like '%"), mMediaType), "%'"));
            //            break;
            //        }

            //    case "ANNOUNCE;BGM":
            //        {
            //            Filter = Conversions.ToString(Filter + Operators.ConcatenateObject(Interaction.IIf(!string.IsNullOrEmpty(Filter), " and ", ""), " (Code IN ('Announce', 'bgm'))"));
            //            break;
            //        }

            //    default:
            //        {
            //            Filter = Conversions.ToString(Filter + Operators.ConcatenateObject(Interaction.IIf(!string.IsNullOrEmpty(Filter), " and ", ""), " issystem = 0"));
            //            break;
            //        }
            //}

            return " Where " + Filter;
        }


    }
}