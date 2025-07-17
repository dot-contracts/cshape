using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlDocket : IDisposable
    {

        private string        mDataError;         public string DataError        { get { return mDataError; }                 set { mDataError = value; } }

        private string        mSnapshotID;        public string SnapshotID       { get { return mSnapshotID; }           set { mSnapshotID     = value;  } }
        private nxProperty    mLink;              public string Link             { get { return mLink.Value; }           set { mLink.Value   = value; } }
        private string        mLinkID;            public string LinkID           { get { return mLinkID; }               set { mLinkID       = value;  } }
        private nxProperty    mLinkLevel;         public string LinkLevel        { get { return mLinkLevel.Value; }      set { mLinkLevel.Value = value; } }

        private nxProperty    mValue1;            public string Value1           { get { return mValue1.Value; }         set { mValue1.Value = value; } }
        private nxProperty    mValue2;            public string Value2           { get { return mValue2.Value; }         set { mValue2.Value = value; } }
        private nxProperty    mValue3;            public string Value3           { get { return mValue3.Value; }         set { mValue3.Value = value; } }
        private nxProperty    mValue4;            public string Value4           { get { return mValue4.Value; }         set { mValue4.Value = value; } }
        private nxProperty    mValue5;            public string Value5           { get { return mValue5.Value; }         set { mValue5.Value = value; } }
        private nxProperty    mValue6;            public string Value6           { get { return mValue6.Value; }         set { mValue6.Value = value; } }
        private nxProperty    mValue7;            public string Value7           { get { return mValue7.Value; }         set { mValue7.Value = value; } }
        private nxProperty    mValue8;            public string Value8           { get { return mValue8.Value; }         set { mValue8.Value = value; } }
        private nxProperty    mValue9;            public string Value9           { get { return mValue9.Value; }         set { mValue9.Value = value; } }
        private nxProperty    mValue10;           public string Value10          { get { return mValue10.Value; }        set { mValue10.Value = value; } }
        private nxProperty    mValue11;           public string Value11          { get { return mValue11.Value; }        set { mValue11.Value = value; } }
        private nxProperty    mValue12;           public string Value12          { get { return mValue12.Value; }        set { mValue12.Value = value; } }
        private nxProperty    mValue13;           public string Value13          { get { return mValue13.Value; }        set { mValue13.Value = value; } }
        private nxProperty    mValue14;           public string Value14          { get { return mValue14.Value; }        set { mValue14.Value = value; } }
        private nxProperty    mValue15;           public string Value15          { get { return mValue15.Value; }        set { mValue15.Value = value; } }
        private nxProperty    mValue16;           public string Value16          { get { return mValue16.Value; }        set { mValue16.Value = value; } }


        private dlLineitem    mLineitem;          public dlLineitem Lineitem     { get { return mLineitem; }                  set { mLineitem = value; } }

                                                  public string DocketPK         { get { return mLineitem.LineitemPK; }       set { mLineitem.LineitemPK = value; } }
                                                  public string BatchID          { get { return mLineitem.BatchID; }          set { mLineitem.BatchID = value; } }

                                                  public string DocketType       { get { return mLineitem.LineitemType; }     set { mLineitem.LineitemType   = value;  } }
                                                  public string DocketTypeID     { get { return mLineitem.LineitemTypeID; }   set { mLineitem.LineitemTypeID = value;  } }

                                                  public string DocketState      { get { return mLineitem.LineitemState; }    set { mLineitem.LineitemState   = value; } }
                                                  public string DocketStateID    { get { return mLineitem.LineitemStateID; }  set { mLineitem.LineitemStateID = value; } }


                                                  public string Computer         { get { return mLineitem.Computer; }         set { mLineitem.Computer = value; } }
                                                  public string ComputerID       { get { return mLineitem.ComputerID; }       set { mLineitem.ComputerID     = value;  } }

                                                  public string Worker           { get { return mLineitem.Worker; }      set { mLineitem.Worker   = value; } }
                                                  public string WorkerID         { get { return mLineitem.WorkerID; }    set { mLineitem.WorkerID = value;  } }

                                                  public string Human            { get { return mLineitem.Human; }       set { mLineitem.Human    = value; } }
                                                  public string HumanID          { get { return mLineitem.HumanID; }     set { mLineitem.HumanID  = value;  } }

                                                  public string Device           { get { return mLineitem.Device; }      set { mLineitem.Device   = value; } }
                                                  public string DeviceID         { get { return mLineitem.DeviceID; }    set { mLineitem.DeviceID = value;  } }

                                                  public string DocketNo         { get { return mLineitem.LineitemNo; }  set { mLineitem.LineitemNo = value; } }
                                                  public string Reference        { get { return mLineitem.Reference; }   set { mLineitem.Reference = value; } }
                                                  public string Amount           { get { return mLineitem.Amount; }      set { mLineitem.Amount = value; } }
                                                  public string Detail           { get { return mLineitem.Detail; }      set { mLineitem.Detail = value; } }


        public event         OnNewDataEventHandler OnNewData;  public delegate void OnNewDataEventHandler();

        public const string ITEM_STATE_PENDING   = "Pending";
        public const string ITEM_STATE_PROCESSED = "Processed";

        public dlDocket(dlBatch batch, string dtype = null)
        {
            mLineitem       = new dlLineitem(batch, "Docket");
            if (!string.IsNullOrEmpty(dtype))
            {
                DocketType = dtype;
            }
            
            mLinkLevel      = new nxProperty("Docket", "LinkLevel");

            mValue1         = new nxProperty("Docket", "Value1");
            mValue2         = new nxProperty("Docket", "Value2");
            mValue3         = new nxProperty("Docket", "Value3");
            mValue4         = new nxProperty("Docket", "Value4");
            mValue5         = new nxProperty("Docket", "Value5");
            mValue6         = new nxProperty("Docket", "Value6");
            mValue7         = new nxProperty("Docket", "Value7");
            mValue8         = new nxProperty("Docket", "Value8");
            mValue9         = new nxProperty("Docket", "Value9");
            mValue10        = new nxProperty("Docket", "Value10");
            mValue11        = new nxProperty("Docket", "Value11");
            mValue12        = new nxProperty("Docket", "Value12");
            mValue13        = new nxProperty("Docket", "Value13");
            mValue14        = new nxProperty("Docket", "Value14");
            mValue15        = new nxProperty("Docket", "Value15");
            mValue16        = new nxProperty("Docket", "Value16");
            
            Reset();

        }

        public bool Open(int targetId)
        {
            bool success = false;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("acur.lst_Lineitem_Docket", "I~S~DocketPK~" + targetId);
                if (DT.Rows.Count > 0)
                    loadFromRow(DT.Rows[0]);
                success = true;
            }

            return success;
        }

        public void loadFromRow(DataRow Row)
        {
            if (Row != null)
            {
                mLineitem.loadFromRow(Row);

                mLinkLevel.Value     = Row["LinkLevel"].ToString();

                mValue1.Value        = Row["Value1"].ToString();
                mValue2.Value        = Row["Value2"].ToString();
                mValue3.Value        = Row["Value3"].ToString();
                mValue4.Value        = Row["Value4"].ToString();
                mValue5.Value        = Row["Value5"].ToString();
                mValue6.Value        = Row["Value6"].ToString();
                mValue7.Value        = Row["Value7"].ToString();
                mValue8.Value        = Row["Value8"].ToString();
                mValue9.Value        = Row["Value9"].ToString();
                mValue10.Value       = Row["Value10"].ToString();
                mValue11.Value       = Row["Value11"].ToString();
                mValue12.Value       = Row["Value12"].ToString();
                mValue13.Value       = Row["Value13"].ToString();
                mValue14.Value       = Row["Value14"].ToString();
                mValue15.Value       = Row["Value15"].ToString();
                mValue16.Value       = Row["Value16"].ToString();                
            }
        }

        public bool Close(string dtype, int shiftid)
        {
            Update(dtype, shiftid);
            return true;
        }

        public bool Update(string dtype = null, int shiftid = 0, string state = null)
        {
            if (shiftid != 0)
            {
                mLineitem.BatchID = shiftid.ToString();
            }

            if (!string.IsNullOrEmpty(dtype))
            {
                DocketType = dtype;
            }

            mLineitem.LineitemState = !string.IsNullOrEmpty(state) ? state : ITEM_STATE_PENDING;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params = string.Empty;
                string ret;

                if (!mLineitem.NewEntry)
                {
                    Params = "I~I~DocketPK~" + mLineitem.LineitemPK + ";";
                }

                Params += getParams();
                ret = DB.ExecLookup("acur.mod_Lineitem_Docket", Params);

                mDataError = DB.DataError;

                if (string.IsNullOrEmpty(mDataError))
                {
                    if (!string.IsNullOrEmpty(ret))
                    {                                                
                        string[] dpk = ret.Split(',');

                        DocketPK = dpk[1];
                    }

                    mLinkLevel.Update();

                    mValue1.Update();
                    mValue2.Update();
                    mValue3.Update();
                    mValue4.Update();
                    mValue5.Update();
                    mValue6.Update();
                    mValue7.Update();
                    mValue8.Update();
                    mValue9.Update();
                    mValue10.Update();
                    mValue11.Update();
                    mValue12.Update();
                    mValue13.Update();
                    mValue14.Update();
                    mValue15.Update();
                    mValue16.Update();
                }
                else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Change", "Event", "UpdateError"), mDataError); }
            }

            return String.IsNullOrEmpty(mDataError);

        }

        public string getParams()
        {
            string Params = string.Empty;

            if (!string.IsNullOrEmpty(mLinkID))          Params += "I~N~LinkID~"    + mLinkID + ";";
            if (!string.IsNullOrEmpty(mLinkLevel.Value)) Params += "I~N~LinkLevel~" + mLinkLevel.Value + ";";
            if (!string.IsNullOrEmpty(mValue1.Value))    Params += "I~F~Value1~"    + mValue1.Value  + ";";
            if (!string.IsNullOrEmpty(mValue2.Value))    Params += "I~F~Value2~"    + mValue2.Value  + ";";
            if (!string.IsNullOrEmpty(mValue3.Value))    Params += "I~F~Value3~"    + mValue3.Value  + ";";
            if (!string.IsNullOrEmpty(mValue4.Value))    Params += "I~F~Value4~"    + mValue4.Value  + ";";
            if (!string.IsNullOrEmpty(mValue5.Value))    Params += "I~F~Value5~"    + mValue5.Value  + ";";
            if (!string.IsNullOrEmpty(mValue6.Value))    Params += "I~F~Value6~"    + mValue6.Value  + ";";
            if (!string.IsNullOrEmpty(mValue7.Value))    Params += "I~F~Value7~"    + mValue7.Value  + ";";
            if (!string.IsNullOrEmpty(mValue8.Value))    Params += "I~F~Value8~"    + mValue8.Value  + ";";
            if (!string.IsNullOrEmpty(mValue9.Value))    Params += "I~F~Value9~"    + mValue9.Value  + ";";
            if (!string.IsNullOrEmpty(mValue10.Value))   Params += "I~F~Value10~"   + mValue10.Value + ";";
            if (!string.IsNullOrEmpty(mValue11.Value))   Params += "I~F~Value11~"   + mValue11.Value + ";";
            if (!string.IsNullOrEmpty(mValue12.Value))   Params += "I~F~Value12~"   + mValue12.Value + ";";
            if (!string.IsNullOrEmpty(mValue13.Value))   Params += "I~F~Value13~"   + mValue13.Value + ";";
            if (!string.IsNullOrEmpty(mValue14.Value))   Params += "I~F~Value14~"   + mValue14.Value + ";";
            if (!string.IsNullOrEmpty(mValue15.Value))   Params += "I~F~Value15~"   + mValue15.Value + ";";
            if (!string.IsNullOrEmpty(mValue16.Value))   Params += "I~F~Value16~"   + mValue16.Value + ";";
            
            Params += mLineitem.getParams();
            
            return Params;
        }

        public bool SetState(string DocketUI, string State, string DocketNo)
        {
            bool ret = false;

            //dlDocket docket = new dlDocket();
            //if (docket.Open(DocketUI)) 
            //{
            //    docket.DocketState = State;

            //    switch (State.ToUpper())
            //    {
            //        case "VALIDATED": //
            //            docket.DocketNo  = DocketNo;
            //            docket.CloseDate = DateTime.Now.ToString("dd MMM, yyyy HH:mm");
            //            break;
            //        case "PENDING": //
            //            docket.DocketNo = "";
            //            docket.CloseDate = DateTime.MinValue.ToString("dd MMM, yyyy HH:mm");
            //            break;
            //        case "VOIDED": // Leave as is
            //            break;
            //    }
            //    ret = docket.Update();
            //}

            return ret;

        }


        public bool Insert(string Type, double Amount, string Detail, int EgmId, int LinkId, int PlayerId, int MediaId, int EntryId)
        {

            mLineitem.NewEntry   = true;
            mLineitem.LineitemPK = "";

            mLineitem.EntryID       = EntryId.ToString();
            mLineitem.LineitemType  = Type;
            mLineitem.LineitemState = "Pending";
            mLineitem.LineitemDate  = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff");
            mLineitem.Amount        = Amount.ToString("0.00");
            mLineitem.Detail        = Detail;
            mLineitem.HumanID       = PlayerId.ToString();
            mLineitem.DeviceID      = EgmId.ToString();
            mLineitem.MediaID       = MediaId.ToString();

            mLinkID                 = LinkId.ToString();
            mValue1.Value           = Amount.ToString("0.00");

            return Update();
        }



        /// <summary>
        /// Set all Docket variables to default values.
        /// </summary>
        public void Reset()
        {
            DocketTypeID          = "";
            DocketStateID         = "";
            HumanID               = "";
            DeviceID              = "";

            mValue1.Value         = "";
            mValue3.Value         = "";
            mValue4.Value         = "";
            mValue5.Value         = "";
            mValue6.Value         = "";
            mValue7.Value         = "";
            mValue8.Value         = "";
            mValue9.Value         = "";
            mValue10.Value        = "";
            mValue11.Value        = "";
            mValue12.Value        = "";
            mValue13.Value        = "";
            mValue14.Value        = "";
            mValue15.Value        = "";
            mValue16.Value        = "";

            mLineitem.Reset();
        }

        #region IDisposable Implementation
        // To detect redundant calls
        private bool disposedValue = false;
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: free managed resources when explicitly called
                }

                // TODO: free shared unmanaged resources
            }
            this.disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
