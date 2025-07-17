using System;
using System.Linq;

using nexus.common.dal;
using nexus.common.cache;
using System.Runtime;

namespace nexus.common.core
{
    public class Venue : nexus.common.dal.dlVenue 
    {

        private int       mLocalUI  = 0;
        private int       mSystemID = 0;
       
        private string    mConnStr = "";

        private string    mWANIp  = "";   public string WanIP    { get { return getWanIP();} }

        private dlHuman    mContact;
        private dlContact  mAddress;
        private dlIdentity mPhone;
        private dlIdentity mContactPhone;
        private dlIdentity mContactEMail;

        public string StreetNo     { get { return mAddress.Contact1; }      set { mAddress.Contact1 = value; } }
        public string StreetName   { get { return mAddress.Contact2; }      set { mAddress.Contact2 = value; } }
        public string StreetType   { get { return mAddress.Street; }        set { mAddress.Street   = value; } }

        public string Phone        { get { return mPhone.Contact1; }        set { mPhone.Contact1 = value; } }
        public string Contact      { get { return mContact.FullName; }      set { mContact.FullName = value; } }
        public string ContactPhone { get { return mContactPhone.Contact1; } set { mContactPhone.Contact1 = value; } }
        public string ContactEMail { get { return mContactEMail.Contact1; } set { mContactEMail.Contact1 = value; } }

        public string Address      { get { return (mAddress.Contact1 + " " + mAddress.Contact2 + " " + mAddress.Street).ToString().TrimStart().TrimEnd(); ; } }

        public enum VenueChangeType { offline, invalid, valid, active, error };

        public event OnVenueStatusChangeHandler OnVenueStatusChange;
        public delegate void OnVenueStatusChangeHandler(VenueChangeType reason, string Venue, int VenueID);

        public Venue()
        {
           mContact      = new dlHuman();
           mAddress      = new dlContact();
           mPhone        = new dlIdentity();
           mContactPhone = new dlIdentity();
           mContactEMail = new dlIdentity();
        }

        public Venue(string ConnectionString, string Settings)
        {
            mContact      = new dlHuman();
            mAddress      = new dlContact();
            mPhone        = new dlIdentity();
            mContactPhone = new dlIdentity();
            mContactEMail = new dlIdentity();

            base.Create(ConnectionString);

            Array arr = Settings.Split(';');
            for (int i = 0; i <= arr.GetLength(0) - 1; i++)
            {
                if (arr.GetValue(i).ToString().Contains('='))
                {
                    Array art = arr.GetValue(i).ToString().Split('=');
                    switch (art.GetValue(0).ToString().ToUpper())
                    {
                        case "LocalId":
                            Int32.TryParse(art.GetValue(1).ToString(), out mLocalUI);
                            break;
                        case "SystemID":
                            Int32.TryParse(art.GetValue(1).ToString(), out mSystemID);
                            break;
                    }
                }
            }
        }

        public bool LoadVenue(string ConnectionString)
        {

            bool ret = false;

            mConnStr = ConnectionString;

            using (SQLServer DB = new SQLServer(mConnStr))
            {
                base.Reset("Club");

                if (mSystemID > 0)
                    base.Open(SystemID: mSystemID.ToString());

                if (base.NewEntry)
                    base.Open(LocalUI: mLocalUI.ToString());

                if (base.NewEntry)
                {
                    //uno Option opt = new Option("Venue.Settings");

                    string VenueID = "";  //uno  opt.GetValue("DefaultId", "");
                    if (string.IsNullOrEmpty(VenueID))
                    {
                        VenueID = DB.ExecLookup("select VenueID from loc.entity where EntityPK=(select EntityID from loc.endpoint where macaddr='" + shell.Instance.MAC + "' and endpointstate=cmn.getStatePK('Entity','Active'))");
                        if (string.IsNullOrEmpty(VenueID))
                        {
                            VenueID = DB.ExecLookup("select top 1 VenuePK from loc.company_venue order by VenuePK");
                            if (string.IsNullOrEmpty(VenueID))
                            {
                                int CompanyNo = 0;

                                int.TryParse(DB.ExecLookup("Select top 1 CompanyNo from loc.company order by companyno desc"), out CompanyNo);
                                CompanyNo += 1;

                                dlVenue Venue = new dlVenue(mConnStr);
                                Venue.Description = "The Company " + CompanyNo.ToString();
                                Venue.CompanyNo = CompanyNo.ToString();
                                Venue.Update();

                                EventCache.Instance.InsertEvent("Information", ActionCache.Instance.strId("Change", "Company", "Company"), "Base Venue Inserted");

                                base.Open(Venue.VenuePK);

                                //uno opt.SetValue("DefaultId", Venue.VenuePK);
                                //uno opt.Update();

                                ret = true;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(VenueID))
                    {
                        ret = base.Open(VenueID);
                        if (ret)
                        {
                            mContact.Open(base.ContactID);
                            mAddress.Open(base.AddressID);
                            mPhone  .Open(base.PhoneID);
                        }
                    }
                }
            }

            return ret;
        }

        public string FindVenue(string LanConnectionString, string WanConnectionString, string MAC)
        {
            int VenueID = 0;

            SQLServer DB = new SQLServer(LanConnectionString);

            string SQL = "select VenueID from loc.entity where EntityPK=(select EntityID from loc.endpoint where macaddr='" + shell.Instance.MAC + "' and endpointstate=cmn.getStatePK('Entity','Active'))";

            if (!int.TryParse(DB.ExecLookup(SQL), out VenueID))
            {                                                                    // couldnt find it locally
                DB = new SQLServer(WanConnectionString);                         // so now look globally
                if (!int.TryParse(DB.ExecLookup(SQL), out VenueID))
                {

                    SQL = "select VenueID from loc.entity where EntityPK=(select EntityID from loc.endpoint where ipaddr='" + WanIP + "' and endpointstate=cmn.getStatePK('Entity','Active'))";
                    int.TryParse(DB.ExecLookup(SQL), out VenueID);
                }
            }

            DB.Dispose();
            DB = null;

            return VenueID.ToString();

        }

        private string getWanIP()
        {
            if (!String.IsNullOrEmpty(mWANIp)) return mWANIp;

            //  determine the internet IP of the venue .. get this from whatismyip.com
            System.Net.WebClient WC = new System.Net.WebClient();
            mWANIp = System.Text.Encoding.ASCII.GetString((WC.DownloadData("http://automation.whatismyip.com/n09230945.asp")));
            WC.Dispose();

            return mWANIp;
        }

        public string GetSettings()
        {
            string SaveStr = "LocalUI="   + base.VenuePK;
            SaveStr       += ";SystemID=" + base.Company.Entity.SystemID;
            SaveStr       += ";Type="     + base.VenueType;
            SaveStr       += ";Name="     + base.Description;
            return SaveStr;
        }


        //private string CalcQtr(string CloseDate)
        //{
        //    //System.DateTime QtrStart = Convert.ToDateTime((modSetup.oTermSet.VenueType == "CL" ? "Dec 1, 1996" : "Jan 1, 1997"));
        //    //string WeekID = (DateTime.DateDiff(DateInterval.WeekOfYear, QtrStart, Convert.ToDateTime(CloseDate))).ToString("0");
        //    //string MonthID = DateTime.DateDiff(DateInterval.Month, QtrStart, Convert.ToDateTime(CloseDate)).ToString("0.0");
        //    //string QtrID = (DateTime.DateDiff(DateInterval.Month, QtrStart, Convert.ToDateTime(CloseDate)) / 3).ToString("0.0");
        //    //QtrID = QtrID.Substring(0, QtrID.IndexOf("."));
        //    //string YearID = (DateTime.DateDiff(DateInterval.Month, QtrStart, Convert.ToDateTime(CloseDate)) / 12).ToString("0.0");
        //    //YearID = YearID.Substring(0, YearID.IndexOf("."));
        //    return "";  // QtrID + "-" + YearID;
        //}

    }
}
