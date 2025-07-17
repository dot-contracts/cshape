using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace nexus.common.cache
{ 
    public class NxCacheBase
    {
        public  bool      CacheLoaded = false;
        public  DataTable CacheTable  = new DataTable();

        public void CreateCache(DataTable cacheTable)
        {
            CacheTable = cacheTable;
            CacheLoaded = (CacheTable.Rows.Count > 0);
        }

        public void Dispose() { }

        public string    LookUpValue(string KeyColumn, string KeyValue, string ValueColumn)
        {
            try
            {
                DataTable dt = LookUpTable(KeyColumn, KeyValue);
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][ValueColumn].ToString();
                else
                {
                    //if (!String.IsNullOrEmpty(ErrMess) || string.IsNullOrEmpty(Desc))
                    //{
                    //    try
                    //    {
                    //        for (int i = 0; i <= mEnumTable.Rows.Count - 1; i++)
                    //        {
                    //            if (mEnumTable.Rows[i]["ValuePK"].ToString().Equals(ValueUI))
                    //            {
                    //                Desc = mEnumTable.Rows[i]["ValueDesc"].ToString();
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    catch (Exception x) { ErrMess = x.Message; }
                    //}

                }
            }
            catch (Exception x) { string ex = x.Message; }
            return "";
        }
        public string    LookUpValue(string GroupColumn, string GroupValue, string KeyColumn, string KeyValue, string ValueColumn)
        {
            try
            {
                DataTable dt = LookUpTable(GroupColumn, GroupValue, KeyColumn, KeyValue);
                if (dt.Rows.Count > 0) return dt.Rows[0][ValueColumn].ToString();
            }
            catch (Exception x) { string ex = x.Message; }
            return "";
        }
        public string    LookUpValue(string SectionColumn, string SectionValue, string GroupColumn, string GroupValue, string KeyColumn, string KeyValue, string ValueColumn)
        {
            try
            {
                DataTable dt = LookUpTable(SectionColumn, SectionValue,GroupColumn, GroupValue, KeyColumn, KeyValue);
                if (dt.Rows.Count > 0) return dt.Rows[0][ValueColumn].ToString();
            }
            catch (Exception x) { string ex = x.Message; }
            return "";
        }
        public int       LookUpInt(string SectionColumn, string SectionValue, string GroupColumn, string GroupValue, string KeyColumn, string KeyValue, string ValueColumn)
        {
            try
            {
                int ret = 0;
                DataTable dt = LookUpTable(SectionColumn, SectionValue, GroupColumn, GroupValue, KeyColumn, KeyValue);
                if (dt.Rows.Count > 0)
                    int.TryParse(dt.Rows[0][ValueColumn].ToString(), out ret);
                dt.Dispose();
                return ret;


            }
            catch (Exception x) { string ex = x.Message; }
            return 0;
        }
        public DataRow   LookUpRow(string KeyColumn, string KeyValue)
        {
            try
            {
                DataTable dt = LookUpTable(KeyColumn, KeyValue);
                if (dt.Rows.Count > 0) return dt.Rows[0];
            }
            catch (Exception x) { string ex = x.Message; }
            return null;
        }
        public DataRow   LookUpRow(string GroupColumn, string GroupValue, string KeyColumn, string KeyValue)
        {
            try
            {
                DataTable dt = LookUpTable(GroupColumn, GroupValue, KeyColumn, KeyValue);
                if (dt.Rows.Count > 0) return dt.Rows[0];
            }
            catch (Exception x) { string ex = x.Message; }
            return null;
        }
        public DataRow   LookUpRow(string SectionColumn, string SectionValue, string GroupColumn, string GroupValue, string KeyColumn, string KeyValue)
        {
            try
            {
                DataTable dt = LookUpTable(SectionColumn, SectionValue, GroupColumn, GroupValue, KeyColumn, KeyValue);
                if (dt.Rows.Count > 0) return dt.Rows[0];
            }
            catch (Exception x) { string ex = x.Message; }
            return null;
        }

        public DataTable LookUpTable()
        {
            return CacheTable;
        }
        public DataTable LookUpTable(string KeyColumn, string KeyValue)
        {
            // Lookup     = column in action table which hold the value we are looking for
            // Typecode   = ie EgmMeter
            // ActionType = ie 
            // ActionCode =

            try
            {
                if (CacheTable != null)
                {
                    if (CacheTable.Rows.Count > 0)
                    {
                        DataTable keyDT = CacheTable.Select("[" + KeyColumn + "]='" + KeyValue + "'").CopyToDataTable();
                        if (keyDT != null)
                        { if (keyDT.Rows.Count > 0) return keyDT; }
                    }
                }
            }
            catch (Exception x) { string ex = x.Message; }
            return new DataTable();
        }
        public DataTable LookUpTable(string TypeColumn, string TypeValue, string KeyColumn, string KeyValue)
        {
            try
            {
                if (CacheTable != null)
                {
                    if (CacheTable.Rows.Count > 0)
                    {
                        DataTable groupTB = CacheTable.Select("[" + TypeColumn + "]='" + TypeValue + "'").CopyToDataTable();
                        if (groupTB != null)
                        {
                            DataTable keyTB = groupTB.Select("[" + KeyColumn + "]='" + KeyValue + "'").CopyToDataTable();
                            if (keyTB != null) { if (keyTB.Rows.Count > 0) return keyTB; }
                        }
                    }
                }
            }
            catch (Exception x) { string ex = x.Message; }
            return new DataTable();
        }
        public DataTable LookUpTable(string SectionColumn, string SectionValue, string TypeColumn, string TypeValue, string KeyColumn, string KeyValue)
        {
            // Lookup     = column in action table which hold the value we are looking for
            // Typecode   = ie EgmMeter
            // ActionType = ie 
            // ActionCode =

            try
            {
                if (CacheTable != null)
                {
                    if (CacheTable.Rows.Count > 0)
                    {
                        DataTable sectionTB = CacheTable.Select("[" + SectionColumn + "] in (" + SectionValue + ")").CopyToDataTable();
                        if (sectionTB != null)
                        {
                            DataTable groupTB = sectionTB.Select("[" + TypeColumn + "]='" + TypeValue + "'").CopyToDataTable();
                            if (groupTB != null)
                            {
                                DataTable keyTB = groupTB.Select("[" + KeyColumn + "]='" + KeyValue + "'").CopyToDataTable();
                                if (keyTB != null) { if (keyTB.Rows.Count > 0) return keyTB; }
                            }
                        }
                    }
                }
            }
            catch (Exception x) { string ex = x.Message; }
            return new DataTable();
        }
    
        public string getList(string EnumType)
        {

            string List = "";


            return List;
        }

        public int RowCount()
        {
            int ret = 0;
            if (CacheTable != null) ret = CacheTable.Rows.Count;
            return ret;
        }

        public bool SyncWithWAN()
        {
            bool ret = true;

            return ret;
        }

    }
}
