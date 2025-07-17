using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using nexus.common.dal;
//using nexus.common.control.foundation;

namespace nexus.common.cache
{

    public sealed class ActionCache : NxCacheBase
    {
        private dlEventAction mAction; 

        private static ActionCache mInstance = new ActionCache();
        private ActionCache() { Create(); }
        public static ActionCache Instance { get { return mInstance; } }

        public bool Create()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))   CreateCache(DB.GetDataTable("cmn.lst_Occurrence_Action", ""));
            return (CacheLoaded);
        }

        public bool Open(string ActionPK) { return mAction.Open(ActionPK); }

        public string   Display   (string ActionPk)                                       { return                LookUpValue("ActionPk",    ActionPk, "Description"); }
        public string   Value     (string Display)                                        { return                LookUpValue("Description", Display,  "ActionPk"); }
        public string   strId     (string TypeCode, string ActionType, string ActionCode) { return                LookUpValue("TypeCode",    TypeCode, "ActionType", ActionType, "ActionCode", ActionCode, "ActionPk"); }
        public int      intId     (string TypeCode, string ActionType, string ActionCode) { int ret; int.TryParse(LookUpValue("TypeCode",    TypeCode, "ActionType", ActionType, "ActionCode", ActionCode, "ActionPk"), out ret); return ret; }
        public string   StringOn  (string TypeCode, string ActionType, string ActionCode) { return                LookUpValue("TypeCode",    TypeCode, "ActionType", ActionType, "ActionCode", ActionCode, "StringOn"); }
        public string   StringOff (string TypeCode, string ActionType, string ActionCode) { return                LookUpValue("TypeCode",    TypeCode, "ActionType", ActionType, "ActionCode", ActionCode, "StringOff"); }


    }
}
