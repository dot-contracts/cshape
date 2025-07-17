using System;
using System.Runtime.InteropServices;
using System.Data;
using System.Timers;
using System.Text;


namespace nexus.common
{
	public static class enums
	{
		
        public enum posn
        {                                                   //these are field positions in data packet. 
            CMDTYPE = 0,                                    //byte zero which holds command in interface
            TYPE    = 0,                                    //command byte posn in host
            DATA    = 1,                                    //which is added by TCP Send()
            TOVER   = 2,
            CMND    = 8,
            CARD    = 10,
            MDBETER = 46,
            VAR     = 19,
            NAME    = CARD + 10,                            //where the name will be when returned by host
            MACPOS  = 6,
            CMDPOS  = 23,                                   //for the host packet
            CMD     = 0,                                    //change of nomenclature
        };

        public enum command                                   //packet commands
        {
            NONE = 0x30,
            LOGIN = 0x31,
            METER = 0x32,
            TOVER = 0x33,
            CARDIN = 0x34,
            CARDOT = 0x35,
            CREDIT = 0x36,
            CCRCREDIT = 0x37,
            CCU = 0x38,
            STATUS = 0x39,
            EGMERR = 0x3D,
            TRANSFER = 0x3E,
            INITIAL = 0x3F,
            REQUEST = 0x3A,
            COMMAND = 0x3B,
            FRTASK = 0x41,
            MESSAGE = 0x45,
            CCCE = 0x46,
            EGMSEND = 0x47,
            TITO = 0x48,
            MACSEND  = 0x49,
            MACRESULT = 0x50,
            GMIDSEND = 0x51,
            GMIDRESULT = 0x52,
            EGMCHANGES = 0x53,
            GOODTOGO = 0x54,
            NOTGOOD = 0x55,
			PROMO = 0x56,
        };

        public enum EgmStatus
        {
            GoodToGo,
            NewPAM,
            NewEGM,
            NotFound,
			Error,
        };

        public enum TCPStates
		{
			CLOSED,
			RUNNING,
			CLOSING,
			STARTING,
            RECIEVING,
            WAITING,
			DOLOGIN,
			WAITLOGIN,
            OKTOGO,
            SENDEGM,
            WAITEGM,
            WAITHOSTRX,
			CHECKHOUSE,
			WAITHOUSE,
			WAITHOSTERR,
			INITIALDELAY,
            MACSEND,
            MACRESULT,
            GMIDSEND,
            GMIDRESULT,
            EGMCHANGES,
            GOODTOGO,
            NOTGOOD,
            CHANGERESULT,
			WAITGOOD,
        };

		public enum ReadStates
		{
			WAITSTX,
			RXPKT,
			WAITCR,
			DIRECTION,
			WAITETX,
		};
		public enum MeterStates
		{
			LOGIN,
			FIRST,
			WAIT_ONE,
			WAIT_START,
			NORMAL,
			UNKNOWN,
			WAITNEWLOGIN,
			ALTERED,
			RSTDONE,
			RSTSENT,
			METERERR,
			NEWGMIDSTART,
			NEWGMIDEND,
			MEMCLEAR,
			WAITREPLY,
			METERFAULT,
			METERECOVER,
			INITIAL,
            WAITLOGIN,
			CHECKHOUSE,
			GOTGMID,
		};

        public enum EGMmeters { INITIAL, PERIOD, INTERVAL, NEWGMID, OPENING, CLEAR, INVALID, RECOVER, TRANSIN, TRANSOUT, CREDITIN, ERROR, REQUESTED, GAMECHG, NOCREDIT, OVERLIMIT };

        public enum EGMerrors
		{
			NEWGMIDSTART,
			NEWGMIDEND,
			MEMCLEAR,
			METERFAULT,
			METERECOVER,
			NORMAL,
		}
		public enum CardStates
		{
			IDLE,
			ERROR,
			TRANSFERIN,
			TRANSFEROUT,
			UNABLE,
			WAITINGCCR,
			WAITINGHOSTIN,
			WAITINGREMOVE,
			WAITINGXFER,
			CCUOUT,
			CCUIN,
			WAITINGCCUIN,
			WAITINGCCUOUT,
			CCUXFERIN,
			DELAY,
			WAITAMOUNT,
            FRACTIVE,
            ATTENDANT,
            WAITINGPIN,
            WAITAKEWIN,
            MEMBERPLAYING,
            XFERERROR,
            FROMEGMDONE,
            WAITINGIMAGE,
			TRYCOLLECT,
			VISITORCARDOUT,
			MEMBERCARDOUT,
		};

        public enum CCRStates
		{
			CCRIDLE,
			CCRDELAY,
			CCRSENT,
			CCRSTART,
			CCRAUTOSTRT,
			CCRBACKOUT,
			CCRNEXTPKT,
			CCREND,
			CCRWAIT
		};

		public enum TicketStates
		{
			IDLE,
			SENDSTARTIN,
			WAITSDB,
			WAITMDB,
			INVALID,
			SENTPHASEA,
			WAITSTACK,
			STACKSTART,
			STACKDONE,
		}
		public enum TransferStates
		{
			IDLE,
			WAITMETERSIN,
			//WAITSIM,
			TRANSFERIN,
			TRANSFERABORT,
			MTIMEOUT,
			PHASEAIN,
			PHASEBIN,
			AINTIMEOUT,
			BINTIMEOUT,
			ERRORCHECK,
			CHECKXFERIN,
			METERXCESS,
			NORESULT,
			NOTENUF,
			NEGATIVE,
			FAILGET,
			TRANSFEROUT,
			PHASEAOUT,
			PHASEBOUT,
			BASECREDIT,
			AOUTIMEOUT,
			BOUTIMEOUT,
			CHECKTRANSFEROT,
			GETIMEOUT,
			CARDREMOVED,
			TOOLARGE,
			CCUXFER,
            SETTIME,
            BUSY,
            FAULT,
            CANCELED,
			WAITSIM,
			LOCALRETURN,
			COMPAREINWAIT,
			COMPAREOTWAIT,
		};
		public enum PageStates
		{																																			// Buttons
			Loading,              // Loading screen, Lives in PI shows status and list box messages													   No Buttons?
			Technicians,          // Technicians Screen			                                                                                       Fix Credit etc
			Error,                // Error Screen     			                                                                                       Show error
			NoCreditNoMember,     // Basically Idle machine sitting there, may as well show advertising etc.                                           Security, Service
			CreditNoMember,       // non member promotions could be run in this state                                                                  Security, Service
			MemberCardIn,         // when card goes in, show Credit Transfer Screen straight up. Hide button takes them to NoCreditMember state        Security, Service, Points, Credit
			NoCreditMember,       // Member card in, Can check poiints, update member details, transfer creditpay for raffles etc                      Security, Service, Points, Credit, Games, Member
			CreditMember,         // Basically same as NoCreditMember, but promotions could be run in this state                                       Security, Service, Points, Credit
			Security,             // Show security screen                                                                                              Back, Emergence,Social Distance, Mobility
			Service,              // Show service screen                                                                                               Back, Food, Cold Drinks, Hot Drinks
			Game,                 // Show Game screen                                                                                                  Back, Raffle, Members Draw
			Members,              // Show Members screen                                                                                               Back, Membership, Details, Points
			Payment,              // Show Payment screen                                                                                               Back, Points, Cash, Credit Card
			Promotions,           // Show Promotions screen                                                                                            System initiated
		};

       // public enum MessageTypes { Clear, NoPage, Card, Face, GenDisp, ToHost, Image }
        public enum MessageTypes     // Include Error in Enum to get message to hold
		{
            Clear,
			NoPage,
            NetworkFault,
            MachineFault,
            CollectIn,
            CollectOut,
            CollectFault,
            TransferIn,
            TransferInError,
            TransferOut,
            TransferOutError,
            TransferError,
            AmountInError,
			AmountOutError,
			CreditInError,
			CreditOutError,
            Card,
            CardFault,
            CardError,
            Face,
            FaceFault,
            PinError,
            PlayerLimits,
            PlayerExclusion,
            RegisterCard,
            RegisterFace,
            MemberFault,
            AccountFault,
            GenDisp,
            SysLog, 
            HostError,
            ToHost,
			Image,
            Promotion,
			TransferEnd,
        };

        public enum CommandTypes  // normally sent from host to egm
        {
            PlaceHolder, // don't make first command a zero
            SyncClock,   // syncronize the PC clock 
            PostMeters,  // post meters
            Reboot,      // reboot the PC
            RestartApp,  // Restart the App
            SendVersion, // send software version
            UpdateSW,    // update the software
            FlashScreen, // cause the GUI to flash
            LaunchVNC,   // launch VNC 
            OpenCashbox, // cause the cashbox door to open
            Collect,     // collect credit from machine and put back into account
            Shutdown,    // to sleep, perchance to dream
            ClearFlash,  // unlock screen
            Ping,        // moved from UDP commands
            ClearEGM,    // fix P1 error
			ShowLoader,  // Show Loader Screen
            UpdateEGMData,
            UDPMeters,   // UDP EGM simulator
            UDPxfer,     // UDP transfer
            TITO,        // ticket ops
            PromoStart,
			PromoEnd
        };


        public enum LoggingTypes  { none, simple, more, heaps, verbose };
        public enum EventTypes    { status, progress, context, content, filter, messageq, command, snapin, topmenu };

        public enum EgmRequests   { Service, Staff, Cancel, Reserve }
        public enum EgmStates     { Login, Fault,BillMeter,CommsFail, MoneyFault, MeterFault, XFerAbort, BACalcErr, EgmWin, EgmWin1, EgmWin2, EgmWin3, EgmWin4, MainDoor, LogicDoor, ADTError,
                                  MemError, CashError, AuditMode, TestMode, PowerSave, SEFError, MeterDisc, EgmErr1, EgmErr2, TamperDoor, CashBoxDoor, TamperSwitch, BAFailure,
                                  BADoorOpen, BACommsError, BillAccepFull, BillStacker, BAService, BAZero, CashBoxOpen, Meters, NewEGM }

        public enum CashlessTypes { CCU, CARD, TICKET, FACE, ERROR };	//ensure this matches Human.MediaTypes!

        public enum CCStates { Delay, Sent, Start, AutoStart, BackOut, NextPkt, End, Wait }

		public enum MediaTypes   { CCU, Card, TICKET, Face, ERROR, Unknown } //ensure this matches enums.CashlessTypes !
		public enum ParentTypes  { Patron, Worker }
		public enum HumanTypes   { NoData, Visitor, Member, Floor, Supervisor, Contractor }
		public enum HumanStates  { Valid, BadMember, NotFinancial, BadCard, BadAccount, CardIn, CardInTrans, OverLimit, InValid }


    }

}


