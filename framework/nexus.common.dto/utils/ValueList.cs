using System;
using System.Data;

using nexus.common.dal;
using nexus.common.core;
using System.Collections.Generic;

namespace nexus.common.cache
{

    public class ValueList
    {
        private DataTable     mValueList   = new DataTable();
        private string        mValuePath   = "Id";               public string     ValuePath    { get { return mValuePath; }      set { mValuePath   = value;  } }
        private string        mDisplayPath = "Description";      public string     DisplayPath  { get { return mDisplayPath; }    set { mDisplayPath = value; } }

        private string        mValue   = "";                     public string     Value        { get { return mValue; }          set { mValue   = value; getDisplay(mValue  ); } }
        private string        mDisplay = "";                     public string     Display      { get { return mDisplay; }        set { mDisplay = value; getId     (mDisplay); } }
        
        public ValueList()  { }
        public ValueList(string ProcName, string Parameters, string DisplayPath, string ValuePath) { Create(ProcName, Parameters, DisplayPath, ValuePath); }

        public bool Create(string ProcName, string Parameters, string DisplayPath, string ValuePath)
        {

            mDisplayPath = DisplayPath;
            mValuePath   = ValuePath;

            SQLServer DB = new SQLServer(setting.ConnectionString);
            mValueList = DB.GetDataTable(ProcName, Parameters);
            DB.Dispose();
            DB = null;

            return (mValueList.Rows.Count > 0);
        }

        public string getId(string Display)
        {
            string ValueUI = "";

            try
            {
                for (int i = 0; i <= mValueList.Rows.Count - 1; i++)
                {
                    if (mValueList.Rows[i][mDisplayPath].ToString().ToUpper().Equals(Display.ToUpper()))
                    {
                        ValueUI = mValueList.Rows[i][mValuePath].ToString();
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                string ex = x.Message;
            }

            return ValueUI;
        }

        public string getDisplay(string ValueUI)
        {
            string Display = "";

            try
            {
                for (int i = 0; i <= mValueList.Rows.Count - 1; i++)
                {
                    if (mValueList.Rows[i][mValuePath].ToString().ToUpper().Equals(ValueUI))
                    {
                        Display = mValueList.Rows[i][mDisplayPath].ToString();
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                string ex = x.Message;
            }

            return Display;
        }

    }
}
