using System;

namespace nexus.common.helper
{
    public enum ValueItemStatus  { ReadOnly, ReadWrite, ReadWriteDelete }
    public enum SetStatus        { ReadOnly, ReadWrite, ReadWriteDelete, ReadWriteInsert }

    public class NameValueItem
    {
        private string mName;
        public string name { get { return mName; } }
        
        private Object mValue;
        public Object value { get { return mValue; } set { mValue = value; } }
        
        private ValueItemStatus mState;
        public ValueItemStatus state { get { return mState; } set { mState = value; } }


        public NameValueItem()
        {
            mName  = null;
            mValue = null;
            mState = ValueItemStatus.ReadOnly;
        }

        public NameValueItem( String name, Object value, ValueItemStatus state )
        {
            mName  = name;
            mValue = value;
            mState = state;
        }
    }
}
