using System;
using System.Data;
using System.Linq;
using System.Text;

using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlPayment
    {        
        private string        mDataError;         public string DataError        { get { return mDataError; }            set { mDataError = value; } }


        private dlLineitem    mLineitem;          public dlLineitem Lineitem     { get { return mLineitem; }                  set { mLineitem = value; } }

                                                  public string PaymentPK        { get { return mLineitem.LineitemPK; }       set { mLineitem.LineitemPK = value; } }
                                                  public string BatchID          { get { return mLineitem.BatchID; }          set { mLineitem.BatchID = value; } }

                                                  public string PaymentType      { get { return mLineitem.LineitemType; }     set { mLineitem.LineitemType   = value;  } }
                                                  public string PaymentTypeID    { get { return mLineitem.LineitemTypeID; }   set { mLineitem.LineitemTypeID = value;  } }

                                                  public string PaymentState     { get { return mLineitem.LineitemState; }    set { mLineitem.LineitemState   = value; } }
                                                  public string PaymentStateID   { get { return mLineitem.LineitemStateID; }  set { mLineitem.LineitemStateID = value; } }


                                                  public string Computer         { get { return mLineitem.Computer; }         set { mLineitem.Computer = value; } }
                                                  public string ComputerID       { get { return mLineitem.ComputerID; }       set { mLineitem.ComputerID     = value;  } }

                                                  public string Worker           { get { return mLineitem.Worker; }      set { mLineitem.Worker   = value; } }
                                                  public string WorkerID         { get { return mLineitem.WorkerID; }    set { mLineitem.WorkerID = value;  } }

                                                  public string Human            { get { return mLineitem.Human; }       set { mLineitem.Human    = value; } }
                                                  public string HumanID          { get { return mLineitem.HumanID; }     set { mLineitem.HumanID  = value;  } }

                                                  public string Device           { get { return mLineitem.Device; }      set { mLineitem.Device   = value; } }
                                                  public string DeviceID         { get { return mLineitem.DeviceID; }    set { mLineitem.DeviceID = value;  } }

                                                  public string PaymentNo        { get { return mLineitem.LineitemNo; }  set { mLineitem.LineitemNo = value; } }
                                                  public string Reference        { get { return mLineitem.Reference; }   set { mLineitem.Reference = value; } }
                                                  public string Amount           { get { return mLineitem.Amount; }      set { mLineitem.Amount = value; } }
                                                  public string Detail           { get { return mLineitem.Detail; }      set { mLineitem.Detail = value; } }


       

        private nxProperty    mCloseDate;         public string CloseDate        { get { return mCloseDate.Value; }      set { mCloseDate.Value = value; } }
        private string        mAccountID;         public string AccountID        { get { return mAccountID; }            set { mAccountID = value;  } }

        private nxProperty    mBank;              public string Bank             { get { return mBank.Value; }           set { mBank.Value = value; } }
        private string        mBankID;            public string BankID           { get { return mBankID; }               set { mBankID     = value;  } }

        private nxProperty    mPlayer;            public string Player           { get { return mPlayer.Value; }         set { mPlayer.Value         = value;  } }
        private nxProperty    mAddress;           public string Address          { get { return mAddress.Value; }        set { mAddress.Value        = value;  } }
        private nxProperty    mReferenceProp;     public string ReferenceProp    { get { return mReferenceProp.Value; }  set { mReferenceProp.Value = value; } }
        private nxProperty    mAccountDetail;     public string AccountDetail    { get { return mAccountDetail.Value; }  set { mAccountDetail.Value  = value; } }


        public event         OnNewDataEventHandler OnNewData;  public delegate void OnNewDataEventHandler();

        public const string ITEM_STATE_PENDING   = "Pending";
        public const string ITEM_STATE_PROCESSED = "Processed";

        public dlPayment(dlBatch batch, string ptype = null)
        {
            mLineitem = new dlLineitem(batch, "Payment");

            if (!string.IsNullOrEmpty(ptype))
            {
                PaymentType = ptype;
            }

            mCloseDate      = new nxProperty("Payment", "CloseDate");
            mBank           = new nxProperty("Payment", "Bank");
            mReferenceProp  = new nxProperty("Payment", "ReferenceProp");
            mAccountDetail  = new nxProperty("Payment", "AccountDetail");
            mPlayer         = new nxProperty("Payment", "Player");
            mAddress        = new nxProperty("Payment", "Address");
        }

        /// <summary>
        /// Load values from database.
        /// </summary>
        /// <param name="targetId">The Primary Key of the row to load values from.</param>
        /// <returns>True if successful, else false.</returns>
        public bool Open(string targetId)
        {
            bool success = false;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                DataTable DT = DB.GetDataTable("acur.lst_Lineitem_Payment", "I~I~PaymentPK~" + targetId);
                if (DT.Rows.Count > 0)
                    loadFromRow(DT.Rows[0]);
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Populates the instance of this class with data from a DataRow.
        /// </summary>
        /// <param name="Row">A DataRow from the database table.</param>
        public void loadFromRow(DataRow Row)
        {
            if (Row != null)
            {
                mLineitem.loadFromRow(Row);

                mCloseDate.Value      = Row["CloseDate"].ToString();
                mReferenceProp.Value  = Row["ReferenceProp"].ToString();
                mAccountDetail.Value  = Row["AccountDetail"].ToString();
                mBank.Value           = Row["Bank"].ToString();
                mPlayer.Value         = Row["Player"].ToString();
                mAddress.Value        = Row["Address"].ToString();
            }
        }

        public bool Update(string ptype = null, int shiftid = 0, string state = null)
        {
            if (shiftid != 0)
            {
                mLineitem.BatchID = shiftid.ToString();
            }

            if (!string.IsNullOrEmpty(ptype))
            {
                PaymentType = ptype;
            }

            mLineitem.LineitemState = !string.IsNullOrEmpty(state) ? state : ITEM_STATE_PENDING;

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                string Params = string.Empty;

                if (!mLineitem.NewEntry)
                {
                    Params = "I~I~PaymentPK~" + mLineitem.LineitemPK + ";";
                }

                Params += GetParams();

                mLineitem.LineitemPK = DB.ExecLookup("acur.mod_Lineitem_Payment", Params);

                mDataError = DB.DataError;

                if (string.IsNullOrEmpty(mDataError))
                {
                    mCloseDate.Update();
                    mBank.Update();
                    mReferenceProp.Update();
                    mAccountDetail.Update();
                    mPlayer.Update();
                    mAddress.Update();
                }
                else
                {
                    EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Change", "Event", "UpdateError"), mDataError);
                }
            }

            return string.IsNullOrEmpty(mDataError);
        }

        public string GetParams()
        {
            string Params = string.Empty;

            mLineitem.LineitemDate = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff");

            Params += "I~D~CloseDate~" + (string.IsNullOrEmpty(mCloseDate.Value) ? DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff") : mCloseDate.Value) + ";";

            if (!string.IsNullOrEmpty(mReferenceProp.Value)) Params += "I~S~ReferenceProp~" + mReferenceProp.Value  + ";";
            if (!string.IsNullOrEmpty(mAccountID))           Params += "I~I~AccountID~"     + mAccountID            + ";";
            if (!string.IsNullOrEmpty(mAccountDetail.Value)) Params += "I~S~AccountDetail~" + mAccountDetail.Value  + ";";
            if (!string.IsNullOrEmpty(mBankID))              Params += "I~I~BankID~"        + mBankID               + ";";
            if (!string.IsNullOrEmpty(mPlayer.Value))        Params += "I~S~Player~"        + mPlayer.Value         + ";";
            if (!string.IsNullOrEmpty(mAddress.Value))       Params += "I~S~Address~"       + mAddress.Value        + ";";
            if (!string.IsNullOrEmpty(mLineitem.WorkerID))   Params += "I~I~WorkerID~"      + mLineitem.WorkerID    + ";";

            Params += mLineitem.getParams();

            return  Params;
        }

        public bool Insert(string Type, double Amount, string ID, string PlayerId, string Player, string Address, string Reference)
        {
            return Insert("", Type, Amount, "", "", "", "", PlayerId, Player, "", Address, "", "", Reference);
        }

        public bool Insert(string Type, double Amount, string PlayerId, string Player, string AddressId, string Address1, string Address2, string Address3, string PayeeRef)
        {
            return Insert("", Type, Amount, "", "", "", "", PlayerId, Player, AddressId, Address1, Address2, Address3, PayeeRef);
        }


        public bool Insert(string BatchId, string Type, double Amount, string BankId, string Bank, string ReferenceNo, string ReferenceProp, string PlayerId, string Player, string AddressId,string Address1, string Address2, string Address3, string PayeeRef)
        {
            bool ret = false;

            //string BatchID, string Amount, string InstType, string InstState, string ReferenceNo, string ReferenceProp, 
            //    string PlayerId, string PayeeName, string PayeeAddr, string Addr2, string Addr3, string PayeeRef

            mLineitem.NewEntry      = true;

            mLineitem.BatchID       = BatchId.ToString();
            mLineitem.LineitemType  = Type;
            mLineitem.LineitemState = "Pending";
            mLineitem.LineitemDate  = DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss.fff");
            mLineitem.Amount        = Amount.ToString("0.00");
            mLineitem.Detail        = Detail;
            mLineitem.HumanID       = PlayerId;
            mLineitem.Reference     = Reference;

            mPlayer.Value           = Player;
            mAddress.Value          = Address1 + ',' + Address2 + ',' + Address3;

            if (string.IsNullOrEmpty(BankId))
            {
                dlBank bank = new dlBank();
                if (bank.Find(Bank))
                    mBankID = bank.BankPK;
                else if (!string.IsNullOrEmpty(Bank))
                {
                    bank.Description = Bank;
                    if (bank.Update())
                        mBankID = bank.BankPK;
                }
            }

            if (string.IsNullOrEmpty(mLineitem.HumanID))
            {
                dlHuman human = new dlHuman();
                if (human.Find(Player))
                    mLineitem.HumanID = human.HumanPK;
                else if (!string.IsNullOrEmpty(Player))
                {
                    human.FullName = Player;
                    if (human.Update())
                        mLineitem.HumanID = human.HumanPK;
                }
            }

            if (string.IsNullOrEmpty(AddressId))
            {
                dlContact address = new dlContact();
                if (!address.Find("Street", mLineitem.HumanID))
                {
                    address.Create("Address");
                    address.EntityID = mLineitem.HumanID;
                    address.Contact1 = Address1;
                    address.Contact2 = Address2;
                    address.Contact3 = Address3;
                    address.Update();
                }
            }

            return ret;
        }
    }
}
