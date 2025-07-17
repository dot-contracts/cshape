using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nexus.common.cache;

namespace nexus.common.dto
{
    public class nxProperty
    {
        int    mEntityID = 0;

        string mActionType = String.Empty;
        string mActionCode = String.Empty;

        string mPropertyId = String.Empty;    public string Property    { set { mProperty   = value; } }
        string mProperty   = String.Empty;    public string PropertyId  { set { mPropertyId = value; } }
        string mOriginal   = String.Empty;
        string mNewValue   = String.Empty;


        public nxProperty(string Property)
        {
            mActionType = Property;
            mActionCode = Property;
            mProperty = Property;
        }
        public nxProperty(string ActionType, string Property)
        {
            mActionType = ActionType;
            mActionCode = Property;
            mProperty   = Property;
        }
        public nxProperty(string ActionType, string ActionCode, string Property)
        {
            mActionType = ActionType;
            mActionCode = ActionCode;
            mProperty   = Property;
        }
        public nxProperty(string ActionType, string ActionCode, string Property, string PropertyId)
        {
            mActionType = ActionType;
            mActionCode = ActionCode;
            mProperty   = Property;
            mPropertyId = PropertyId;
        }

        public void Create(string Value)
        {
            mNewValue = Value.Trim();
            mOriginal = Value.Trim();
        }
        public void Create(int EntityID, string Value)
        {
            mEntityID = EntityID;
            mNewValue = Value.Trim();
            mOriginal = Value.Trim();
        }
        public void Create(int EntityID, string Value, string Property, string PropertyId)
        {
            mEntityID   = EntityID;
            mNewValue   = Value.Trim();
            mOriginal   = Value.Trim();
            mPropertyId = PropertyId;
            mProperty   = Property;
        }

        public string Value
        {
            get { return mNewValue; }
            set { mNewValue = ""; if (value != null) mNewValue = value.Trim(); }
        }
        public string OriginalValue
        {
            get { return mOriginal; }
            set { mOriginal = ""; if (value != null) mOriginal = value.Trim(); }
        }

        public int ValueInt
        {
            get
            {
                int val = 0;
                int.TryParse(mNewValue, out val);
                return val;
            }
        }
        public bool ValueBool
        {
            get
            {
                return (mNewValue.Equals("True") || mNewValue.Equals("1"));
            }
        }
        public DateTime ValueDate
        {
            get
            {
                DateTime val = DateTime.Now;
                DateTime.TryParse(mNewValue, out val);
                return val;
            }
        }
        public string ValueTitleCase()
        {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo ti = ci.TextInfo;
            return ti.ToTitleCase(mNewValue.ToLower());
        }

        public bool Changed()
        {
            return !mOriginal.ToUpper().Equals(mNewValue.ToUpper());
        }

        public string Confirm(bool AddDeliminator)
        {
            string ret = String.Empty;
            if (AddDeliminator) ret += ";";
            if (!mOriginal.ToUpper().Equals(mNewValue.ToUpper()))
            {
                ret = " changed from [" + mOriginal + "] to [" + Value + "]  \n";
            }
            return ret;
        }

        public void Update() { Update(false); }
        public void Update(bool ForceInsert)
        {
            if (!mOriginal.ToUpper().Equals(mNewValue.ToUpper()) || ForceInsert)
            {
            try
            {
               //  InsertEvent(EventType,   TypeCode,  ActionType, ActionCode, Property, ExtensionId)
               EventCache.Instance.InsertEvent("Modify", "Change", mActionType, mActionCode, mProperty + " changed from [" + mOriginal + "] to [" + mNewValue + "]", mPropertyId);
               mOriginal = mNewValue;

            }
            catch (Exception)
            {
// dwr
               //throw;
            }
            }
        }



        public void Reset()
        {
            mNewValue = String.Empty;
            mOriginal = String.Empty;
        }
    }

}
