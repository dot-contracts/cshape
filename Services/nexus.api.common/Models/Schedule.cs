

using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.Reflection.Metadata.Ecma335;

using nexus.common;
using nexus.common.dal;
using nexus.common.cache;
using nexus.api.common;

namespace nexus.api.common
{

    static public class ScheduleHelpers
    {

        public static string GetParams( Schedule schedule) 
        {
            string Params = string.Empty;
            if (schedule.HasData())
            {
                if (schedule.ScheduleId > 0) Params += ";I~S~ScheduleId~" + schedule.ScheduleId;

                if (!string.IsNullOrEmpty(schedule.ScheduleCode)) Params += ";I~S~ScheduleCode~"  + schedule.ScheduleCode;
                if (!string.IsNullOrEmpty(schedule.StartsOn))     Params += ";I~S~StartsOn~"      + schedule.StartsOn;
                if (!string.IsNullOrEmpty(schedule.StartsAt))     Params += ";I~S~StartsAt~"      + schedule.StartsAt;
                if (!string.IsNullOrEmpty(schedule.FinishesOn))   Params += ";I~S~FinishesOn~"    + schedule.FinishesOn;
                if (!string.IsNullOrEmpty(schedule.FinishesAt))   Params += ";I~S~FinishesAt~"    + schedule.FinishesAt;
                if (!string.IsNullOrEmpty(schedule.Days))         Params += ";I~S~Days~"          + schedule.Days;
                if (!string.IsNullOrEmpty(schedule.Hours))        Params += ";I~S~Hours~"         + schedule.Hours;

                if (schedule.ScheduleTypeId > 0)                  Params += ";I~S~ScheduleType~"  + schedule.ScheduleTypeId;
                if (schedule.ScheduleStateId > 0)                 Params += ";I~S~ScheduleState~" + schedule.ScheduleStateId;
                if (schedule.IntType > 0)                         Params += ";I~S~IntType~"       + schedule.IntType;
                if (schedule.Priority > 0)                        Params += ";I~S~Priority~"      + schedule.Priority;
                if (schedule.Duration > 0)                        Params += ";I~S~Duration~"      + schedule.Duration;
                if (schedule.Repeats > 0)                         Params += ";I~S~Repeats~"       + schedule.Repeats;
                if (schedule.Runs > 0)                            Params += ";I~S~Runs~"          + schedule.Runs;
                if (schedule.MaxRuns > 0)                         Params += ";I~S~MaxRuns~"       + schedule.MaxRuns;
            }

            return Params;
        }

        public static Schedule LoadFromRow(DataRow row)
        {

            Schedule schedule = new Schedule()
            {
                Description   = row["Description"].ToString(),
                ScheduleType  = row["ScheduleType"].ToString(),
                ScheduleState = row["ScheduleState"].ToString(),
                ScheduleCode  = row["ScheduleCode"].ToString(),
                Starts        = row["Starts"].ToString(),
                StartsOn      = row["StartsOn"].ToString(),
                StartsAt      = row["StartsAt"].ToString(),
                Finishes      = row["Finishes"].ToString(),
                FinishesOn    = row["FinishesOn"].ToString(),
                FinishesAt    = row["FinishesAt"].ToString(),
                Days          = row["Days"].ToString(),
                Hours         = row["Hours"].ToString(),
                EntityType    = row["EntityType"].ToString(),
                TypeValue     = row["TypeValue"].ToString(),
                StateValue    = row["StateValue"].ToString(),
                IntValueType  = row["IntValueType"].ToString(),
                NextRun       = row["NextRun"].ToString(),
                LastRun       = row["LastRun"].ToString(),

                ScheduleId      = helpers.ToInt(row["SchedulePk"].ToString()),
                Increment       = helpers.ToInt(row["Increment"].ToString()),
                IntType         = helpers.ToInt(row["IntType"].ToString()),
                Priority        = helpers.ToInt(row["Priority"].ToString()),
                Duration        = helpers.ToInt(row["Duration"].ToString()),
                Repeats         = helpers.ToInt(row["Repeats"].ToString()),
                Runs            = helpers.ToInt(row["Runs"].ToString()),
                MaxRuns         = helpers.ToInt(row["MaxRuns"].ToString()),
                ScheduleTypeId  = helpers.ToInt(row["ScheduleTypeId"].ToString()),
                ScheduleStateId = helpers.ToInt(row["ScheduleStateId"].ToString()),
                EntityTypeId    = helpers.ToInt(row["EntityTypeId"].ToString())
            };
            return schedule;
        }

        public static DataTable AddScheduleColumns(DataTable dt)
        {
            DataTable dtNew = dt.Clone();
            dtNew.Columns.Add("ScheduleId", typeof(int));
            dtNew.Columns.Add("Description", typeof(string));
            dtNew.Columns.Add("ScheduleType", typeof(string));
            dtNew.Columns.Add("ScheduleState", typeof(string));
            dtNew.Columns.Add("ScheduleCode", typeof(string));
            dtNew.Columns.Add("Starts", typeof(string));
            dtNew.Columns.Add("StartsOn", typeof(string));
            dtNew.Columns.Add("StartsAt", typeof(string));
            dtNew.Columns.Add("Finishes", typeof(string));
            dtNew.Columns.Add("FinishesOn", typeof(string));
            dtNew.Columns.Add("FinishesAt", typeof(string));
            dtNew.Columns.Add("Days", typeof(string));
            dtNew.Columns.Add("Hours", typeof(string));
            dtNew.Columns.Add("Increment", typeof(int));
            dtNew.Columns.Add("IntType", typeof(int));
            dtNew.Columns.Add("Priority", typeof(int));
            dtNew.Columns.Add("Duration", typeof(int));
            dtNew.Columns.Add("Repeats", typeof(int));
            dtNew.Columns.Add("EntityType", typeof(string));
            dtNew.Columns.Add("TypeValue", typeof(string));
            dtNew.Columns.Add("StateValue", typeof(string));
            dtNew.Columns.Add("IntValueType", typeof(string));
            dtNew.Columns.Add("NextRun", typeof(string));
            dtNew.Columns.Add("LastRun", typeof(string));
            dtNew.Columns.Add("Runs", typeof(int));
            dtNew.Columns.Add("MaxRuns", typeof(int));
            dtNew.Columns.Add("ScheduleTypeId", typeof(int));
            dtNew.Columns.Add("ScheduleStateId", typeof(int));
            dtNew.Columns.Add("EntityTypeId", typeof(int));
            dtNew.Columns.Add("Active", typeof(int));
            return dtNew;
        }
    }

    public class ScheduleRequest
    {
        public int           ActionType    { get; set; } = 0;
        public string        Vendor        { get; set; } = string.Empty;
        public string        Location      { get; set; } = string.Empty;
        public string?       ScheduleType  { get; set; } = string.Empty;
        public Schedule?     Schedule      { get; set; } = new Schedule();

        public ScheduleRequest(int ActionType, string Vendor, string Location, Schedule Schedule = null)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;
            this.Schedule   = (Schedule  == null ? new Schedule() : Schedule);
        }
    }

    public class ScheduleResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public Schedule[] Schedules { get; set; } = Array.Empty<Schedule>();
    }

    public class Schedule
    {
        public int?      ScheduleId      { get; set; } = -1;
        public string?   Entity          { get; set; } = string.Empty;
        public int?      Active          { get; set; } = -1;
        public string?   Description     { get; set; } = string.Empty;
        public string?   ScheduleType    { get; set; } = string.Empty;
        public string?   ScheduleState   { get; set; } = string.Empty;
        public string?   ScheduleCode    { get; set; } = string.Empty;
        public string?   Starts          { get; set; } = string.Empty;
        public string?   StartsOn        { get; set; } = string.Empty;
        public string?   StartsAt        { get; set; } = string.Empty;
        public string?   Finishes        { get; set; } = string.Empty;
        public string?   FinishesOn      { get; set; } = string.Empty;
        public string?   FinishesAt      { get; set; } = string.Empty;
        public string?   Days            { get; set; } = string.Empty;
        public string?   Hours           { get; set; } = string.Empty;
        public int?      Increment       { get; set; } = -1;
        public int?      IntType         { get; set; } = -1;
        public int?      Priority        { get; set; } = -1;
        public int?      Duration        { get; set; } = -1;
        public int?      Repeats         { get; set; } = -1;
        public string?   EntityType      { get; set; } = string.Empty;
        public string?   TypeValue       { get; set; } = string.Empty;
        public string?   StateValue      { get; set; } = string.Empty;
        public string?   IntValueType    { get; set; } = string.Empty;
        public string?   NextRun         { get; set; } = string.Empty;
        public string?   LastRun         { get; set; } = string.Empty;
        public int?      Runs            { get; set; } = -1;
        public int?      MaxRuns         { get; set; } = -1;
        public int?      ScheduleTypeId  { get; set; } = -1;
        public int?      ScheduleStateId { get; set; } = -1;
        public int?      EntityTypeId    { get; set; } = -1;

        public bool HasData()
        {
            return (ScheduleId > 0
            || !string.IsNullOrEmpty(Entity)
            || !string.IsNullOrEmpty(Description)
            || !string.IsNullOrEmpty(ScheduleType)
            || !string.IsNullOrEmpty(ScheduleState)
            || !string.IsNullOrEmpty(ScheduleCode)
            || !string.IsNullOrEmpty(Starts)
            || !string.IsNullOrEmpty(StartsOn)
            || !string.IsNullOrEmpty(StartsAt)
            || !string.IsNullOrEmpty(Finishes)
            || !string.IsNullOrEmpty(FinishesOn)
            || !string.IsNullOrEmpty(FinishesAt)
            || !string.IsNullOrEmpty(Days)
            || !string.IsNullOrEmpty(Hours)
            || !string.IsNullOrEmpty(EntityType)
            || !string.IsNullOrEmpty(TypeValue)
            || !string.IsNullOrEmpty(StateValue)
            || !string.IsNullOrEmpty(IntValueType)
            || !string.IsNullOrEmpty(NextRun)
            || !string.IsNullOrEmpty(LastRun)
            || Active >= 0
            || Increment >= 0
            || IntType >= 0
            || ScheduleStateId >= 0
            || Priority >= 0
            || Duration >= 0
            || Repeats >= 0
            || Runs >= 0
            || MaxRuns >= 0
            || ScheduleTypeId >= 0
            || ScheduleStateId >= 0
            || EntityTypeId >= 0);
        }

        public bool ValidSearch()
        {
            return true;
        }
    }
}

