
using nexus.common.dal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace nexus.common.cache
{
    public sealed class OptionCache
    {
        private DataTable mOptionTable = new DataTable();
        private dlOption mOption; 

        private static OptionCache mInstance = new OptionCache();
        private OptionCache() { Create(); }

        public static OptionCache Instance { get { return mInstance; } }


        private int mParentID = 1;

        /// <summary>
        /// Populates the mOptionTable with a list of Options from the Option table in the database.
        /// </summary>
        public void Create()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
                mOptionTable = DB.GetDataTable("cmn.lst_Option", "");
        }

        public void Update()
        {
            //SQLServer DB = new SQLServer();

            //for (int i = 0; i <= mOptionTable.Rows.Count - 1; i++)
            //{

            //    if (mOptions.Rows[i]["Dirty"].ToString().Equals("True"))
            //    {
            //        string SQL = "";
            //        string Defn = mOptions.Rows[i]["Defn"].ToString();
            //        string Name = mOptions.Rows[i]["Name"].ToString();
            //        string Value = mOptions.Rows[i]["Value"].ToString();
            //        string Default = mOptions.Rows[i]["Default"].ToString();
            //        mOptions.Rows[i]["Dirty"] = false;

            //        //string Mask =  mOptions.Rows[i]["Mask"].ToString();
            //        //SetMask += (Mask.Substring(0, 1).Equals("1")) ? "1" : "0";
            //        //SetMask += "," + ((Mask.Substring(1, 1).Equals("1")) ? "1" : "0");
            //        //SetMask += ",0,0";

            //        SetMask = "0,0,0,0";

            //        int OptionPK = 0;
            //        Int32.TryParse(mOptions.Rows[i]["Id"].ToString(), out OptionPK);
            //        if (OptionPK == 0)
            //            Int32.TryParse(DB.ExecLookup("Select OptionPk from cmn.Option where ParentID=" + mParentID + " and OptnCode='" + Defn + "'"), out OptionPK);

            //        if (OptionPK == 0)
            //        {

            //            SQL = "set nocount on ";

            //            SQL += "declare @ObjID       int; ";
            //            SQL += "declare @UserID      int;        Set @UserID     = (select min(PersonPk) from [mem].[Person]) ";
            //            SQL += "declare @ModlID      int;        Set @ModlID     = (select min(ModulePk) from [gen].[Module]) ";
            //            SQL += "declare @Authority   varchar(32);Set @Authority  = 'Nexus' ";
            //            SQL += "declare @HomeVenue   int;        Set @HomeVenue  = obj.GetLocalVenueSuid() ";

            //            SQL += "declare @OptnType int;           Set @OptnType   = obj.GetCodeSuid('ParameterNode','Leaf') ";
            //            SQL += "declare @OptnStatus int;         Set @OptnStatus = obj.GetCodeSuid('Entity','Active') ";
            //            SQL += "declare @OptnDefn varchar(32);   Set @OptnDefn   = '" + Defn.ToUpper() + "' ";
            //            SQL += "declare @OptnName varchar(32);   Set @OptnName   = '" + Name + "' ";
            //            SQL += "declare @ParentID int;           Set @ParentID   = " + mParentID + " ";

            //            SQL += "declare @ValueType int;          Set @ValueType  = 10; ";

            //            SQL += "insert into cmn.[Option] (OptnType, OptnState, OptnDefn, OptnName, ParentID,ValueType,VarDefault,Sequence,IsClientable,IsClientRoleable,IsWorkerable,IsWorkerRoleable, Userid, ModlId,  Authority, Modified, Inserted) ";
            //            SQL += "values ( @OptnType, @OptnStatus, @OptnDefn, @OptnName, @ParentID, @ValueType,'',0,0,0,0,0, @UserID, @ModlID, @Authority, getdate(), getdate()) ";

            //            SQL += "select @ObjID = scope_identity() ";

            //            SQL += "select @ObjID ";

            //            Int32.TryParse(DB.ExecLookup(SQL), out OptionPK);

            //            mOptions.Rows[i]["Id"] = OptionPK;
            //        }

            //        SetMask = shell.Instance.ComputerID + ",NULL,NULL,NULL";

            //        //SetMask = (Mask.Substring(0, 1).Equals("1")) ? shell.Instance.ComputerId : "NULL";
            //        //SetMask += "," + ((Mask.Substring(1, 1).Equals("1")) ? shell.Instance.Worker.DescriptionId : "NULL");
            //        //SetMask += ",NULL,NULL";

            //        //string Filter = GetFilter(Mask);
            //        //if (String.IsNullOrEmpty(Filter))
            //        //    DB.ExecNonQuery("Update css.[option] Set VarDefault='" + Value + "' where OptionPK=" + OptionPK);
            //        //else
            //        {
            //            int SettingPk = 0;
            //            Int32.TryParse(mOptions.Rows[i]["SettingId"].ToString(), out SettingPk);
            //            if (SettingPk == 0)
            //                Int32.TryParse(DB.ExecLookup("Select OptionSettingPk from css.OptionSetting where OptionID=" + OptionPK + " and ClientId=" + shell.Instance.ComputerID), out SettingPk);

            //            if (Value.Equals(Default))
            //            {
            //                if (!SettingPk.Equals(0))
            //                {
            //                    DB.ExecNonQuery("Delete from css.OptionSetting where OptionSettingPk=" + SettingPk);
            //                    SettingPk = 0;
            //                    mOptions.Rows[i]["SettingId"] = SettingPk;
            //                }
            //            }
            //            else
            //            {
            //                if (SettingPk.Equals(0))
            //                {
            //                    SQL = "set nocount on ";

            //                    SQL += "declare @ObjID       int; ";
            //                    SQL += "declare @UserID      int;        Set @UserID     = (select min(PersonPk) from [mem].[Person]) ";
            //                    SQL += "declare @ModlID      int;        Set @ModlID     = (select min(ModulePk) from [gen].[Module]) ";
            //                    SQL += "declare @Authority   varchar(32);Set @Authority  = 'Nexus' ";

            //                    SQL += "declare @OptionID int;   Set @OptionID=" + OptionPK + " ";

            //                    SQL += "insert into css.[optionsetting] (OptionID, ClientId, ClientRoleId, WorkerId, WorkerRoleId, ValueText, UserId, ModlId, Authority, Modified, Inserted) ";
            //                    SQL += "values ( @OptionID, " + SetMask + ",'" + Value + "', @UserID, @ModlID, @Authority, getdate(), getdate()) ";

            //                    SQL += "select @ObjID = scope_identity() ";

            //                    SQL += "select @ObjID ";

            //                    Int32.TryParse(DB.ExecLookup(SQL), out SettingPk);

            //                    mOptions.Rows[i]["SettingId"] = SettingPk;
            //                }
            //                else
            //                    DB.ExecNonQuery("Update css.[optionsetting] Set ValueText='" + Value + "' where OptionSettingPk=" + SettingPk);
            //            }
            //        }
            //    }
            //}

        }

        public bool Open(string targetId)
        {
            return mOption.Open(targetId);
        }

        public string Description(string targetId)
        {
            //return mOption.Description(targetId);
            return "";
        }


        public void Find(string Column, string Value)
        {
        }

        public void Dispose() { }

        public void Reset(string OptionType)
        {
        }


        public string strId(string OptionCode) { return getId(OptionCode).ToString(); }
        public int getId(string OptionCode)
        {
            int ValueId = 0;


            return ValueId;
        }

        public string strIdFromDesc(string OptionDesc) { return getIdFromDesc(OptionDesc).ToString(); }

        public int getIdFromDesc(string OptionDesc)
        {

            int ValueId = 0;


            return ValueId;
        }

        public string getDescFromId(string targetId)
        {
            string Desc = "";


            return Desc;

        }

        public string getList(string OptionType)
        {

            string List = "";


            return List;
        }

        public DataTable getTable(string OptionType)
        {
            return mOptionTable;
        }

        public bool SyncWithWAN()
        {
            bool ret = true;


            return ret;
        }

        public string GetValue(string Name) { return GetValue("1000", Name, ""); }
        public string GetValue(string Name, string Default) { return GetValue("1000", Name, Default); }

        public string GetValue(string Mask, string Name, string Default)
        {
            string Value = Default;
            string Defn = Name.ToUpper();

            for (int i = 0; i <= mOptionTable.Rows.Count - 1; i++)
            {
                if (mOptionTable.Rows[i]["Defn"].ToString().ToUpper().Equals(Defn))
                    return mOptionTable.Rows[i]["Value"].ToString();
            }

            DataRow newRow = mOptionTable.NewRow();
            newRow["Defn"] = Defn;
            newRow["Name"] = Name;
            newRow["Value"] = Value;
            newRow["Default"] = Value;
            newRow["Type"] = "";
            newRow["Dirty"] = true;
            newRow["Id"] = 0;
            newRow["Mask"] = Mask;
            newRow["ParentID"] = mParentID;
            mOptionTable.Rows.Add(newRow);

            return Value;
        }

        public void SetValue(string Name, string Value) { SetValue("1000", Name, Value); }

        public void SetValue(string Mask, string Name, string Value)
        {
            bool Found = false;
            string Defn = Name.ToUpper();

            for (int i = 0; i <= mOptionTable.Rows.Count - 1; i++)
            {
                if (mOptionTable.Rows[i]["Defn"].ToString().ToUpper().Equals(Defn))
                {
                    if (!mOptionTable.Rows[i]["Value"].ToString().Equals(Value))
                    {
                        mOptionTable.Rows[i]["Value"] = Value;
                        mOptionTable.Rows[i]["Dirty"] = true;
                    }
                    Found = true;
                    break;
                }
            }

            if (!Found)
            {
                DataRow newRow = mOptionTable.NewRow();
                newRow["Defn"] = Defn;
                newRow["Name"] = Name;
                newRow["Value"] = Value;
                newRow["Type"] = "";
                newRow["Dirty"] = true;
                newRow["Id"] = 0;
                newRow["Mask"] = Mask;
                newRow["ParentID"] = mParentID;
                mOptionTable.Rows.Add(newRow);
            }
        }


        private string GetFilter(string Mask)
        {
            string Filter = "";
            if (Mask.Substring(0, 1).Equals("1"))
                Filter = "ClientId=" + setting.ComputerId;

            if (Mask.Substring(1, 1).Equals("1"))
            {
                if (!String.IsNullOrEmpty(Filter)) Filter += " and ";
                Filter = "UserId=" + setting.WorkerId;
            }

            if (Mask.Substring(2, 1).Equals("1"))
            {
                if (!String.IsNullOrEmpty(Filter)) Filter += " and ";
                Filter = "ClientRoleId=" + setting.WorkerId;  // wrong
            }

            if (Mask.Substring(3, 1).Equals("1"))
            {
                if (!String.IsNullOrEmpty(Filter)) Filter += " and ";
                Filter = "UserRoleId=" + setting.WorkerId;  //wrong
            }

            return Filter;
        }

    }
}
