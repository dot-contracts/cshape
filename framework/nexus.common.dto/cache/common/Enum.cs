using System;
using System.Data;

using nexus.common.dal;
//using nexus.common.control.foundation;
using nexus.common.core;

namespace nexus.common.cache
{

    public sealed class EnumCache : NxCacheBase
    {
        public enum ValueTypes { Type, State, Format }

        private static EnumCache mInstance = new EnumCache();
        private EnumCache() { }
        public static EnumCache Instance { get { return mInstance; } }

        public bool Create()
        {
            if (!CacheLoaded)
            {
                using (SQLServer DB = new SQLServer(setting.ConnectionString))
                    CreateCache(DB.GetDataTable("cmn.lst_EnumValue", ""));

            }
            return (CacheLoaded);
        }

        public string   Display   (string ValuePk)                                        { return                LookUpValue("ValuePk", ValuePk, "ValueDesc"); }
        public string   ValuePk   (string ValueDesc)                                      { return                LookUpValue("ValueType", "1", "ValueDesc", ValueDesc, "ValuePk"); }
        public string   StatePk   (string ValueDesc)                                      { return                LookUpValue("ValueType", "2", "ValueDesc", ValueDesc, "ValuePk"); }

        public int intId(string EnumCode, string ValueCode)
        {
            int ret = 0;
            int.TryParse(getTypeId(EnumCode, ValueCode), out ret);
            return ret;
        }
        public int intState(string EnumCode, string ValueCode)
        {
            int ret = 0;
            int.TryParse(getStateId(EnumCode, ValueCode), out ret);
            return ret;
        }

        public string getType           (string EnumCode, string ValueCode)        { return getDesc(ValueTypes.Type,   EnumCode, ValueCode);        }
        public string getState          (string EnumCode, string ValueCode)        { return getDesc(ValueTypes.State,  EnumCode, ValueCode);        }
        public string getFormat         (string EnumCode, string ValueCode)        { return getDesc(ValueTypes.Format, EnumCode, ValueCode);        }

        public string getTypeId         (string EnumCode, string ValueCode)        { return getId(ValueTypes.Type,   EnumCode, ValueCode);        }
        public string getStateId        (string EnumCode, string ValueCode)        { return getId(ValueTypes.State,  EnumCode, ValueCode);        }
        public string getFormatId       (string EnumCode, string ValueCode)        { return getId(ValueTypes.Format, EnumCode, ValueCode);        }
        public string getTypeFromDesc   (string EnumCode, string ValueDescription) { return getId(ValueTypes.Type,   EnumCode, ValueDescription); }
        public string getStateFromDesc  (string EnumCode, string ValueDescription) { return getId(ValueTypes.State,  EnumCode, ValueDescription); }
        public string getFormatFromDesc (string EnumCode, string ValueDescription) { return getId(ValueTypes.Format, EnumCode, ValueDescription); }

        public int    intTypeId         (string EnumCode, string ValueCode)        { return getInt(ValueTypes.Type,   EnumCode, ValueCode);        }
        public int    intStateId        (string EnumCode, string ValueCode)        { return getInt(ValueTypes.State,  EnumCode, ValueCode);        }
        public int    intFormatId       (string EnumCode, string ValueCode)        { return getInt(ValueTypes.Format, EnumCode, ValueCode);        }
        public int    intTypeFromDesc   (string EnumCode, string ValueDescription) { return getInt(ValueTypes.Type,   EnumCode, ValueDescription); }
        public int    intStateFromDesc  (string EnumCode, string ValueDescription) { return getInt(ValueTypes.State,  EnumCode, ValueDescription); }
        public int    intFormatFromDesc (string EnumCode, string ValueDescription) { return getInt(ValueTypes.Format, EnumCode, ValueDescription); }


        public int getInt(ValueTypes ValueType, string EnumCode, string ValueCode)
        {
            return LookUpInt("ValueType", valueType(ValueType), "EnumCode", EnumCode, "ValueCode", ValueCode, "ValuePk");
        }

        public string getId (ValueTypes ValueType, string EnumCode, string ValueCode)  
        {
            return LookUpValue("ValueType", valueType(ValueType), "EnumCode", EnumCode, "ValueCode", ValueCode, "ValuePk");
        }

        public string getDesc(ValueTypes ValueType, string EnumCode, string ValueCode)
        {
            return LookUpValue("ValueType", valueType(ValueType), "EnumCode", EnumCode, "ValueCode", ValueCode, "ValueDesc");
        }

        private string getIdFromDesc(ValueTypes ValueType, string EnumCode, string ValueDescription)
        {
            return LookUpValue("ValueType", valueType(ValueType), "EnumCode", EnumCode, "ValueDesc", ValueDescription, "ValuePk");
        }

        public string getDescFromId(string ValuePk)
        {
            if (!string.IsNullOrEmpty(ValuePk))
                return LookUpValue("ValuePk", ValuePk, "ValueDesc");
            else
                return "";
        }


        public DataTable getEnumType(string EnumCode)
        {

            if (CacheTable != null)
            {
                if (String.IsNullOrEmpty(EnumCode))
                    return CacheTable;
                else
                    return LookUpTable("ValueType", "1", "EnumCode", EnumCode);
            }
            return new DataTable();
        }

        public DataTable getEnumType(string EnumCode, string Group)
        {

            if (CacheTable != null)
            {
                if (String.IsNullOrEmpty(EnumCode))
                    return CacheTable;
                else
                    return LookUpTable("ValueType", "1", "EnumCode", EnumCode);
            }
            return new DataTable();
        }

        public DataTable getEnumState(string EnumCode)
        {
            if (CacheTable != null)
            {
                if (String.IsNullOrEmpty(EnumCode))
                    return CacheTable;
                else
                    return LookUpTable("ValueType", "2", "EnumCode", EnumCode);
            }

            return new DataTable();
        }

        public DataTable getEnumState(string EnumCode, string Group)
        {
            if (CacheTable != null)
            {
                if (String.IsNullOrEmpty(EnumCode))
                    return CacheTable;
                else
                    return LookUpTable("Group", Group, "ValueType", "2", "EnumCode", EnumCode);
            }

            return new DataTable();
        }

        public DataTable getEnum(string EnumTypeUI)
        {
            if (CacheTable != null)
            {
                if (String.IsNullOrEmpty(EnumTypeUI))
                    return CacheTable;
                else
                    return LookUpTable("ValueType", "1", "EnumPk", EnumTypeUI);
            }
            return new DataTable();
        }
        public DataTable getValue(string ValueUI)
        {
            if (CacheTable != null)
            {
                if (String.IsNullOrEmpty(ValueUI))
                    return CacheTable;
                else
                    return LookUpTable("ValueType", "1", "ValuePK", ValueUI);
            }
            return new DataTable();
        }


        //public DataTable FilterList()
        //{
        //    return FilterList("", "","1");
        //}
        //public DataTable FilterList(string Key, string Value, string Type)
        //{
        //    try
        //    {
        //        if (mEnumTable != null)
        //        {
        //            if (String.IsNullOrEmpty(Value))
        //            {
        //                return mEnumTable;
        //            }
        //            else
        //            {
        //                if (mEnumTable.Rows.Count > 0)
        //                {
        //                    DataTable dt = mEnumTable.Select("[" + Key + "]='" + Value + "'").CopyToDataTable();
        //                    if (dt != null)
        //                    {
        //                        DataTable vt = dt.Select("[ValueType]=" + Type).CopyToDataTable();
        //                        if (vt != null) return vt;
        //                    }
        //                }
        //             }
        //        }
        //    }
        //    catch (Exception x)
        //    { }
        //    return null;
        //}

        //public DataTable getTable()
        //{
        //    return mEnumTable;
        //}
        //public DataRow getDataRow(string DenoteId)
        //{
        //    try
        //    {
        //        if (mEnumTable != null)
        //        {
        //            DataRow[] rows = mEnumTable.Select("DenotePK=" + DenoteId);
        //            if (rows.GetLength(0) > 0)
        //                return rows[0];

        //        }
        //    }
        //    catch (Exception x) { string ex = x.Message; }

        //    return null;

        //}

        //public int RowCount()
        //{
        //    int ret = 0;
        //    if (mEnumTable != null)
        //        ret = mEnumTable.Rows.Count;
        //    return ret;
        //}

        //public void Dispose() { }

        //public void Reset(string EnumType)
        //{
        //    mEnumCode    = "";
        //    mEnumDesc    = "";
        //    mValueDesc   = "";
        //    mOperability = 0;
        //    mVisibility  = 0;
        //    mParentID    = 0;
        //    mValueType   = 0;
        //}

        private string valueType (ValueTypes ValueType)
        {

            string ret = "";
            switch (ValueType)
            {
                case ValueTypes.Type:
                    ret = "1";
                    break;
                case ValueTypes.State:
                    ret = "2";
                    break;
                case ValueTypes.Format:
                    ret = "3";
                    break;
                default:
                    break;
            }
            return ret;
        }


    }
}
