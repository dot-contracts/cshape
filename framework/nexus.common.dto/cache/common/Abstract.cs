using System;
using System.Data;

using nexus.common.dal;
using nexus.common.core;
using System.Collections.Generic;

namespace nexus.common.cache
{

    public sealed class AbstractCache
    {
        private DataTable     mAbstractTable = new DataTable();

        private string        mAbstractUI = "0";    public string   AbstractUI       { get { return mAbstractUI; }        }
        private string        mAbstractType;        public string   AbstractType     { get { return mAbstractType; }      }
        private string        mAbstractState;       public string   AbstractState    { get { return mAbstractState; }     }
        private string        mAbstractCode;        public string   AbstractCode     { get { return mAbstractCode; }      }
        private string        mAbstractDesc;        public string   AbstractDesc     { get { return mAbstractDesc; }      }
        private string        mAbstractPath;        public string   AbstractPath     { get { return mAbstractPath; }      }

        private string        mValueUI = "0";   public string   ValueUI      { get { return mValueUI; }       }
        private string        mValueState;      public string   ValueState   { get { return mValueState; }    }
        private string        mValueCode;       public string   ValueCode    { get { return mValueCode; }     }
        private string        mValueDesc;       public string   ValueDesc    { get { return mValueDesc; }     }
        private int           mVarType;         public int      VarType      { get { return mVarType; }     }

        private int           mOperability;     public int      Operability  { get { return mOperability; }   }
        private int           mVisibility;      public int      Visibility   { get { return mVisibility; }    }
        private int           mParentUI;        public int      ParentUI     { get { return mParentUI; }      }
        private int           mDenoteId;        public int      DenoteId     { get { return mDenoteId; }    }

        private bool          mNewEntry = false;

        private static AbstractCache mInstance = new AbstractCache();
        private AbstractCache() { mAbstractTable   = new DataTable(); }
        public static AbstractCache Instance { get { return mInstance; } }


        public bool Create()
        {
            SQLServer DB = new SQLServer(setting.ConnectionString);
            mAbstractTable = DB.GetDataTable("cmn.lst_AbstractValue", "");
            DB.Dispose();
            DB = null;

            return (mAbstractTable.Rows.Count > 0);
        }

        public int intId(string AbstractCode, string ValueCode)
        {
            int ret = 0;
            int.TryParse(getTypeId(AbstractCode, ValueCode), out ret);
            return ret;
        }

        public  string getTypeId(string AbstractCode, string ValueCode)
        {
            return getId(EnumCache.ValueTypes.Type, AbstractCode, ValueCode);
        }
        public  string getStateId(string AbstractCode, string ValueCode)
        {
            return getId(EnumCache.ValueTypes.State, AbstractCode, ValueCode);
        }
        public  string getFormatId(string AbstractCode, string ValueCode)
        {
            return getId(EnumCache.ValueTypes.Format, AbstractCode, ValueCode);
        }
        private string getId(EnumCache.ValueTypes ValueType, string AbstractCode, string ValueCode)
        {
            string ValueUI = "0";

            try
            {
                DataTable dt = FilterList("AbstractPath", AbstractCode, valueType(ValueType));
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataTable vt = dt.Select("[ValueCode]='" + ValueCode + "'").CopyToDataTable();
                        if (vt != null)
                        {
                            if (vt.Rows.Count > 0)
                                ValueUI = vt.Rows[0]["ValueUK"].ToString();
                        }
                    }
                }
            }
            catch (Exception x)
            {
                string ex = x.Message;
            }

            return ValueUI;
        }

        public  string getTypeFromDesc(string AbstractCode, string ValueDescription)
        {
            return getId(EnumCache.ValueTypes.Type, AbstractCode, ValueDescription);
        }
        public  string getStateFromDesc(string AbstractCode, string ValueDescription)
        {
            return getId(EnumCache.ValueTypes.State, AbstractCode, ValueDescription);
        }
        public  string getFormatFromDesc(string AbstractCode, string ValueDescription)
        {
            return getId(EnumCache.ValueTypes.Format, AbstractCode, ValueDescription);
        }
        private string getIdFromDesc(EnumCache.ValueTypes ValueType, string AbstractCode, string ValueDescription)
        {

            string ValueUI = "0";

            try
            {
                DataTable dt = FilterList("AbstractPath", AbstractCode, valueType(ValueType));
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataTable vt = dt.Select("[ValueDesc]='" + ValueDescription + "'").CopyToDataTable();
                        if (vt != null)
                        {
                            if (vt.Rows.Count > 0)
                                ValueUI = vt.Rows[0]["ValueUK"].ToString();
                        }
                    }
                }
            }
            catch (Exception x)
            {
                string ex = x.Message;
            }

            return ValueUI;

        }

        public string getDescFromId(string ValueUI)
        {
            string Desc    = "";
            string ErrMess = "";

            if (mAbstractTable != null)
            {
                if (mAbstractTable.Rows.Count > 0)
                {
                    try
                    {
                        DataRow[] rows = mAbstractTable.Select("[ValueUK]=" + ValueUI);
                        if (rows.GetLength(0) > 0) Desc = rows[0]["ValueDesc"].ToString();
                    }
                    catch (Exception x) { ErrMess = x.Message; }

                    if (!String.IsNullOrEmpty(ErrMess))
                    {
                        try
                        {
                            for (int i = 0; i <= mAbstractTable.Rows.Count - 1; i++)
                            {
                                if (mAbstractTable.Rows[i]["ValueUK"].ToString().Equals(ValueUI))
                                {
                                    Desc = mAbstractTable.Rows[0]["ValueDesc"].ToString();
                                    break;
                                }
                            }
                        }
                        catch (Exception x) { ErrMess = x.Message; }
                    }
                }
            }

            return Desc;

        }


        public string getList(string AbstractType)
        {

            string List = "";


            return List;
        }


        public DataTable getAbstractType(string AbstractCode)
        {
            if (mAbstractTable != null)
            {
                if (String.IsNullOrEmpty(AbstractCode))
                    return mAbstractTable;
                else
                    return FilterList("AbstractCode", AbstractCode, "1");
            }

            return new DataTable();
        }
        public DataTable getAbstractState(string AbstractCode)
        {
            if (mAbstractTable != null)
            {
                if (String.IsNullOrEmpty(AbstractCode))
                    return mAbstractTable;
                else
                    return FilterList("AbstractCode", AbstractCode, "2");
            }

            return new DataTable();
        }
        public DataTable getAbstract(string AbstractTypeUI)
        {
            if (mAbstractTable != null)
            {
                if (String.IsNullOrEmpty(AbstractTypeUI))
                    return mAbstractTable;
                else
                    return FilterList("AbstractUK", AbstractTypeUI, "1");
            }
            return new DataTable();
        }
        public DataTable getValue(string ValueUI)
        {
            if (mAbstractTable != null)
            {
                if (String.IsNullOrEmpty(ValueUI))
                    return mAbstractTable;
                else
                    return FilterList("ValueUK", ValueUI, "1");
            }
            return new DataTable();
        }



        public DataTable FilterList()
        {
            return FilterList("", "","1");
        }
        public DataTable FilterList(string Key, string Value, string Type)
        {
            try
            {
                if (mAbstractTable != null)
                {
                    if (String.IsNullOrEmpty(Value))
                    {
                        return mAbstractTable;
                    }
                    else
                    {
                        if (mAbstractTable.Rows.Count > 0)
                        {
                            DataTable dt = mAbstractTable.Select("[" + Key + "]='" + Value + "'").CopyToDataTable();
                            if (dt != null)
                            {
                                DataTable vt = dt.Select("[ValueType]=" + Type).CopyToDataTable();
                                if (vt != null) return vt;
                            }
                        }
                     }
                }
            }
            catch (Exception x)
            { }
            return null;
        }

        public DataTable getTable()
        {
            return mAbstractTable;
        }
        public DataRow getDataRow(string DenoteId)
        {
            try
            {
                if (mAbstractTable != null)
                {
                    DataRow[] rows = mAbstractTable.Select("DenoteUK=" + DenoteId);
                    if (rows.GetLength(0) > 0)
                        return rows[0];

                }
            }
            catch (Exception x) { string ex = x.Message; }

            return null;

        }

        public int RowCount()
        {
            int ret = 0;
            if (mAbstractTable != null)
                ret = mAbstractTable.Rows.Count;
            return ret;
        }

        public void Dispose() { }

        public void Reset(string AbstractType)
        {
            mAbstractCode    = "";
            mAbstractDesc    = "";
            mValueDesc   = "";
            mOperability = 0;
            mVisibility  = 0;
            mParentUI    = 0;
            mVarType   = 0;
        }

        private string valueType(EnumCache.ValueTypes ValueType)
        {

            string ret = "";
            switch (ValueType)
            {
                case EnumCache.ValueTypes.Type:
                    ret = "1";
                    break;
                case EnumCache.ValueTypes.State:
                    ret = "2";
                    break;
                case EnumCache.ValueTypes.Format:
                    ret = "3";
                    break;
                default:
                    break;
            }
            return ret;
        }


    }
}
