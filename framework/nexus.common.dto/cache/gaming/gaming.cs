using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dal;

namespace nexus.common.cache
{
    public sealed class Gaming
    {
        private object    syncLock     = new object();

        SQLServer mDB;

        private static Gaming mInstance = new Gaming();
        private Gaming() { }
        public static Gaming Instance { get { return mInstance; } }
        public bool Create()
        {
            mDB = new SQLServer(setting.ConnectionString) ;
            return true;
        }


        public int InsertEGMFault(int EventPK, bool Active, int EgmID, int GamingTypeUI, int ActionTypeID, string Fault)
        {

            lock (this.syncLock)
            {
                string SQL = "";

                if (EventPK==0)
                    SQL += "update [cmn].[Event] set EventState=cmn.getStatePK('Event', 'InActive'),EventDate=getdate() where EventPK=" + EventPK.ToString()  + " ";

                try
                {
                    SQL += "declare @EventPK int ";
                    SQL += "set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "insert into [cmn].[Occurrence] (OccurrencePk, OccurrenceType, OccurrenceState, OccurrenceDate, Inserted, Modified) values (@EventPK, cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(),getdate(), getdate() ) ";
                    SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event', '" + (Active ? "Active" : "InActive") + "' ) ";
                    SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityID, Property) ";
                    SQL += "values (@EventPK," + ActionTypeID + "," + GamingTypeUI + ",@EventState, getdate()," + EgmID + ",'" + Fault + "') ";

                    SQL += "update [gam].[CurrentSnap] set Fault=1, FaultCount = isnull(FaultCount,0) + 1, FaultId=@EventPK where EgmID=" + EgmID + " ";
                    SQL += "Select @EventPK ";

                    int.TryParse(mDB.ExecLookup(SQL), out EventPK);
                }
                catch (Exception ex) { }

                if (!Active)
                {
                    try
                    {   // check to see if any other faults ... if none reset faults condition
                        SQL += "IF not EXISTS(SELECT * FROM  [cmn].[Event] where EntityID=" + EgmID + " and EventAction = " + ActionTypeID + " and EventType=" + GamingTypeUI + " and EventState=cmn.getStatePK('Event', 'Active') ) ";
                        SQL += "update [gam].[Current] set Fault=0, FaultId=null where EgmID=" + EgmID;
                        mDB.ExecNonQuery(SQL);
                        EventPK = 0;
                    }
                    catch (Exception ex) { }
                }

                return EventPK;
            }
        }

        public int InsertEGMRequest(string Request, int EventPK, bool Active, int EgmID, int MemberID, int GamingTypeUI, int ActionTypeID, string Property)
        {
            lock (this.syncLock)
            {
                string SQL = "";

                if (EventPK==0)
                    SQL += "update [cmn].[Event] set EventState=cmn.getStatePK('Event', 'InActive'),EventDate=getdate() where EventPK=" + EventPK.ToString() + " ";

                if (Active)
                {
                    try
                    {
                        SQL += "declare @EventPK int ";
                        SQL += "set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                        SQL += "insert into [cmn].[Occurrence] (OccurrencePk, OccurrenceType, OccurrenceState, OccurrenceDate, Inserted, Modified) values (@EventPK,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate(), getdate() ) ";
                        SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event', 'Active') ";
                        SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityID, HumanID, Property) ";
                        SQL += "values (@EventPK," + ActionTypeID + "," + GamingTypeUI + ",@EventState, getdate()," + EgmID + "," + (MemberID >0 ? MemberID.ToString() : "NULL" ) + ",'" + Property + "') ";

                        SQL += "update [gam].[Current] set " + Request + "=@EventPK, " + Request + "Start=getdate() where EgmID=" + EgmID;
                        SQL += "Select @EventPK ";

                        int.TryParse(mDB.ExecLookup(SQL), out EventPK);
                    }
                    catch (Exception ex) {}
                }
                else
                {
                    try
                    {   // reset request condition
                        SQL += "update [gam].[Current] set " + Request + "=NULL where EgmID=" + EgmID;
                        mDB.ExecNonQuery(SQL);
                        EventPK = 0;
                    }
                    catch (Exception ex) {}
                }
            }
            return EventPK;
        }

        public int InsertEGMCancel( double CancelAmount, int EventPK, bool Active, int EgmID, int MemberID, int GamingTypeUI, int ActionTypeID, string Property)
        {
            lock (this.syncLock)
            {
                string SQL = "";

                if (EventPK == 0)
                    SQL += "update [cmn].[Event] set EventState=cmn.getStatePK('Event', 'InActive'),EventDate=getdate() where EventPK=" + EventPK.ToString() + " ";

                if (Active)
                {
                    try
                    {
                        SQL += "declare @EventPK int ";
                        SQL += "set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                        SQL += "insert into [cmn].[Occurrence] (OccurrencePk, OccurrenceType, OccurrenceState, OccurrenceDate, Inserted, Modified) values (@EventPK,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate(), getdate() ) ";
                        SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event', 'Active') ";
                        SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityID, HumanID, Property) ";
                        SQL += "values (@EventPK," + ActionTypeID + "," + GamingTypeUI + ",@EventState, getdate()," + EgmID + "," + (MemberID > 0 ? MemberID.ToString() : "NULL") + ",'" + Property + "') ";

                        SQL += "update [gam].[Current] set Cancel=@EventPK, CancelStart=getdate(), CancelAmount=" + CancelAmount.ToString("0.00") + " where EgmID=" + EgmID;
                        SQL += "Select @EventPK ";

                        int.TryParse(mDB.ExecLookup(SQL), out EventPK);
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    try
                    {   // reset request condition
                        SQL += "update [gam].[Current] set Cancel=NULL where EgmID=" + EgmID;
                        mDB.ExecNonQuery(SQL);
                        EventPK = 0;
                    }
                    catch (Exception ex) { }
                }
            }
            return EventPK;
        }

        public int UpdateCurrent(int EventPK, int EgmID, int EventTypeId, int ActionTypeID, int SnapShotTypeId, int MeterStateId, string Property, string MeterSQL)
        {

            lock (this.syncLock)
            {
                try
                {
                    string SQL = "declare @EventPK int ";
                    SQL += "declare @EgmID      int set @EgmID      = " + EgmID + " ";
                    SQL += "declare @MeterType  int set @MeterType  = cmn.getValuePK('EgmMeter', 'SoftMeter') ";
                    SQL += "declare @MeterState int set @MeterState = " + MeterStateId + " ";
                    SQL += "declare @OccrType   int set @OccrType   = cmn.getValuePK('Occurrence', 'SnapShot') ";
                    SQL += "declare @SnapType   int set @SnapType   = " + SnapShotTypeId + " ";
                    SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event',     'Active') ";
                    SQL += "declare @EventType  int set @EventType  = " + EventTypeId + " ";
                    SQL += "declare @ActionType int set @ActionType = " + ActionTypeID + " ";
                    SQL += "declare @MeterType  int set @MeterType  = cmn.getValuePK('CurrentGame', 'Meters') ";
                    SQL += "declare @MovemType  int set @MovemType  = cmn.getValuePK('CurrentGame', 'Movement') ";


                    SQL += "IF not EXISTS(SELECT * FROM  [gam].[Current] where EgmID=@EgmID) ";
                    SQL += " Begin ";
                    SQL += "     set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "     insert into [cmn].[Occurrence] (OccurrencePk,OccurrenceType, OccurrenceState, OccurrenceDate, Inserted, Modified) values (@EventPk,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate(), getdate() ) ";
                    SQL += "     insert into [gam].[Current] (CurrentPK,EgmID,StateType) values(@EventPK,@EgmID,@SnapType) ";
                    SQL += " End ";

                    SQL += "set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "insert into [cmn].[Occurrence] (OccurrencePk,OccurrenceType, OccurrenceState, Inserted, Modified) values (@EventPK,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate() ) ";
                    SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityId, Property) ";
                    SQL += "values (@EventPK,@ActionType,@EventType,@EventState, getdate(),@EgmID,'" + Property + "') ";
                    SQL += MeterSQL;

                    SQL += "Select @EventPK ";

                    int.TryParse(mDB.ExecLookup(SQL), out EventPK);

                    //mDB.ExecLookup(SQL);


                }
                catch (Exception ex) { }
            }
            return EventPK;
        }

        public int UpdateCancel(int CancelPK, int CancelType, int CancelState, double Amount, double Current, double Previous, string CancelDate, string Detail, string Reference, int LineitemNo, int ComputerId, int WorkerId, int HumanId, int DeviceId, int BatchId)
        {

            lock (this.syncLock)
            {
                try
                {

                    string SQL = "";
                    if (CancelPK > 0)
                        SQL += "declare @CancelPK  int set @CancelPK    = " + CancelPK + " ";

                    SQL += "declare @CancelType   int set @CancelType  = " + CancelType + " ";
                    SQL += "declare @CancelState  int set @CancelState = " + CancelState + " ";
                    SQL += "declare @LineitemNo   int set @LineitemNo  = " + LineitemNo + " ";
                    SQL += "declare @Reference    int set @Reference   = " + Reference + " ";
                    SQL += "declare @Detail       int set @Detail      = " + Detail + " ";
                    SQL += "declare @CancelDate   int set @CancelDate  = " + CancelDate + " ";
                    SQL += "declare @Amount       int set @Amount      = " + Amount + " ";
                    SQL += "declare @Previous     int set @Previous    = " + Previous + " ";
                    SQL += "declare @DeviceId     int set @DeviceID    = " + DeviceId + " ";
                    SQL += "declare @ComputerId   int set @ComputerID  = " + ComputerId + " ";
                    SQL += "declare @WorkerId     int set @WorkerId    = " + WorkerId + " ";
                    SQL += "declare @HumanId      int set @HumanId     = " + HumanId + " ";
                    SQL += "declare @BatchId      int set @BatchId     = " + BatchId + " ";


                    SQL += "IF not EXISTS(SELECT * FROM  [gam].[Current] where EgmID=@EgmID) ";
                    SQL += " Begin ";
                    SQL += "     set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "     insert into [cmn].[Occurrence] (OccurrencePk,OccurrenceType, OccurrenceState, OccurrenceDate, Inserted, Modified) values (@EventPk,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate(), getdate() ) ";
                    SQL += "     insert into [gam].[Current] (CurrentPK,EgmID,StateType) values(@EventPK,@EgmID,@SnapType) ";
                    SQL += " End ";

                    SQL += "set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "insert into [cmn].[Occurrence] (OccurrencePk,OccurrenceType, OccurrenceState, Inserted, Modified) values (@EventPK,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate() ) ";
                    SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityId, Property) ";
                    //SQL += "values (@EventPK,@ActionType,@EventType,@EventState, getdate(),@EgmID,'" + Property + "') ";
                    //SQL += MeterSQL;

                    SQL += "Select @EventPK ";

                    int.TryParse(mDB.ExecLookup(SQL), out CancelPK);

                    //mDB.ExecLookup(SQL);


                }
                catch (Exception ex) { }
            }
            return CancelPK;
        }

    }

}
