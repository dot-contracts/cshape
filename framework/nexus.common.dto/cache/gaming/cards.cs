using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.core;
using nexus.common.dal;

namespace nexus.common.cache
{
    public sealed class CardCache
    {
        private object    syncLock     = new object();

        SQLServer mDB;

        private static CardCache mInstance = new CardCache();
        private CardCache() { }
        public static CardCache Instance { get { return mInstance; } }
        public bool Create()
        {
            mDB = new SQLServer(setting.ConnectionString);
            return true;
        }

        public string InsertCardTransfer(int EgmID, int CardState, int CardError, int CardCredit, int CurrCredit, int DeltCredit, int DeltaMoney, int TransAmount, int MemberId, string Badge, string Property)
        {

            string EventPK = "";

            lock (this.syncLock)
            {
                try
                {
                    string SQL = "declare @EventPK int ";
                    SQL += "declare @EgmID      int set @EgmID = " + EgmID + " ";
                    //SQL += "declare @MeterType  int set @MeterType  = cmn.getValuePK('Meter', 'Soft') ";
                    //SQL += "declare @MeterState int set @MeterState = " + MeterStateId + " ";
                    //SQL += "declare @OccrType   int set @OccrType   = cmn.getValuePK('Occurrence', 'SnapShot') ";
                    //SQL += "declare @SnapType   int set @SnapType   = " + SnapShotTypeId + " ";
                    //SQL += "declare @EventState int set @EventState = cmn.getStatePK('Event',     'Active') ";
                    //SQL += "declare @EventType  int set @EventType  = " + EventTypeId + " ";
                    //SQL += "declare @ActionType int set @ActionType  = " + ActionTypeID + " ";

                    SQL += "IF not EXISTS(SELECT * FROM  [gam].[CurrentSnap] where EgmID=@EgmID) ";
                    SQL += " Begin ";
                    SQL += "     set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "     insert into [cmn].[Occurrence] (OccurrencePk,OccurrenceType, OccurrenceState, Inserted, Modified) values (@EventPK,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate() ) ";
                    SQL += "     insert into [gam].[CurrentSnap] (CurrentUK,EgmID,StateType) values(@EventPK,@EgmID,@SnapType) ";
                    SQL += " End ";

                    SQL += "set @EventPK = NEXT VALUE FOR cmn.Occurrence_seq ";
                    SQL += "insert into [cmn].[Occurrence] (OccurrencePk,OccurrenceType, OccurrenceState, Inserted, Modified) values (@EventPK,cmn.getValuePK('Occurrence','Event'), cmn.getStatePK('Occurrence','Active'), getdate(), getdate() ) ";
                    SQL += "insert into [cmn].[Event] (EventPK, EventAction, EventType, EventState, EventDate, EntityPK, Property) ";
                    //SQL += "values (@EventPK,@ActionType,@EventType,@EventState, getdate(),@EgmID,'" + Property + "') ";
                    //SQL += MeterSQL;

                    SQL += "Select @EventPK ";

                    EventPK = mDB.ExecLookup(SQL);

                    mDB.ExecLookup(SQL);


                }
                catch (Exception ex) { }
            }
            return EventPK;
        }

    }
}
