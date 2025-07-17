using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using nexus.common.dal;
using nexus.common.dto;
using nexus.common.core;
using nexus.shared.common;


namespace nexus.common.core
{
    public class Option : IDisposable
    {

        string ParentCode = string.Empty;
        string ParentId   = string.Empty;

        bool Computer;
        bool ComputerRole;
        bool Worker;
        bool WorkerRole;

        DataTable mOptions;

        public Option(string OptionPath)              { Create(OptionPath, "0000"); }
        public Option(string OptionPath, string Mask) { Create(OptionPath, Mask  ); }

        private void Create(string OptionPath, string Mask) 
        {

            string OptionCode = string.Empty;

            if (OptionPath.Contains("."))
            {
                Array ParArr = OptionPath.Split('.');
                ParentCode = ParArr.GetValue(0).ToString();
                OptionCode = ParArr.GetValue(1).ToString();
            }
            else
            {
                ParentCode = OptionPath;
            }

            Computer     = Mask.Substring(0, 1).Equals("1");
            ComputerRole = Mask.Substring(1, 1).Equals("1");
            Worker       = Mask.Substring(2, 1).Equals("1");
            WorkerRole   = Mask.Substring(3, 1).Equals("1");

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
               string Params = "";
               Params += "I~S~ParentCode~" + ParentCode + ";";
               Params += "I~S~OptionCode~" + OptionCode + ";";
               Params += "I~S~ComputerId~" + Session.ComputerId + ";";
               Params += "I~S~WorkerId~"   + Session.CurrentUser.WorkerId;
                //Params += "I~S~ComputerRoleId~"  + mContactStateID    + ";";
                //Params += "I~S~WorkerRoleId~"    + mStreetID          + ";";

                mOptions = DB.GetDataTable("[cmn].[OptionGet]", Params);
                if (mOptions.Rows.Count == 0) CreateTable();

                mOptions.Columns.Add(new DataColumn("Dirty", Type.GetType("System.Boolean")));
                mOptions.AcceptChanges();

                if (mOptions.Rows.Count>0)
                   ParentId = mOptions.Rows[0]["ParentId"].ToString();

            }
        }

        public void Update()
        {
            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                for (int i = 0; i <= mOptions.Rows.Count - 1; i++)
                {
                    if (mOptions.Rows[i]["Dirty"].Equals(true))
                    {
                        string Params = "";
                        if (!string.IsNullOrEmpty(ParentId))
                            Params += "I~S~ParentId~" + ParentId + ";";

                        Params += "I~S~ParentCode~" + mOptions.Rows[i]["ParentCode"].ToString() + ";";
                        Params += "I~S~OptionCode~" + mOptions.Rows[i]["OptionCode"].ToString() + ";";
                        Params += "I~S~VarDefault~" + mOptions.Rows[i]["VarDefault"].ToString() + ";";
                        Params += "I~S~ValueText~"  + mOptions.Rows[i]["ValueText"].ToString()  + ";";

                        Params += "I~S~ComputerId~" + setting.ComputerId        + ";";
                        Params += "I~S~WorkerId~"   + setting.WorkerId          + ";";
                        //Params += "I~S~ComputerRoleId~"  + mContactStateID    + ";";
                        //Params += "I~S~WorkerRoleId~"    + mStreetID          + ";";

                        Params += "I~S~Computer~"     + (Computer     ? "1" : "0") + ";";
                        Params += "I~S~ComputerRole~" + (ComputerRole ? "1" : "0") + ";";
                        Params += "I~S~Worker~"       + (Worker       ? "1" : "0") + ";";
                        Params += "I~S~WorkerRole~"   + (WorkerRole   ? "1" : "0") + ";";

                        using (DataTable DT = DB.GetDataTable("[cmn].[OptionPut]", Params))
                            if (DT.Rows.Count>0 && String.IsNullOrEmpty(ParentId))
                               ParentId = DT.Rows[0]["ParentId"].ToString();
                    }
                }
            }

        }

        public double GetDoubleValue(string Name, string Default)
        {
            double rval = 0;
            string ret = GetValue(Name, Default);
            Double.TryParse(ret, out rval);
            return rval;
        }

        public string GetValue(string OptionCode)                    { return GetValue(OptionCode, ""); }
        public string GetValue(string OptionCode, string VarDefault)
        {
            for (int i = 0; i <= mOptions.Rows.Count - 1; i++)
            {
                if (mOptions.Rows[i]["OptionCode"].ToString().ToUpper().Equals(OptionCode.ToUpper()))
                {
                    string vText = mOptions.Rows[i]["ValueText"].ToString();
                    string dText = mOptions.Rows[i]["VarDefault"].ToString();

                    if (string.IsNullOrEmpty(vText) && !string.IsNullOrEmpty(dText)) return dText;
                    else                                                             return vText;

                }
            }

            DataRow newRow = mOptions.NewRow();
            newRow["ParentCode"] = ParentCode;
            newRow["OptionCode"] = OptionCode;
            newRow["VarDefault"] = VarDefault;
            newRow["Dirty"]      = true;
            if (!string.IsNullOrEmpty(ParentId))
               newRow["ParentID"]   = ParentId;
            mOptions.Rows.Add(newRow);

            return VarDefault;
        }

        public void SetValue(string OptionCode, string ValueText) 
        {
            bool Found = false;

            for (int i = 0; i <= mOptions.Rows.Count - 1; i++)
            {
                if (mOptions.Rows[i]["OptionCode"].ToString().ToUpper().Equals(OptionCode))
                {
                    if (!mOptions.Rows[i]["ValueText"].ToString().Equals(ValueText))
                    {
                        mOptions.Rows[i]["ValueText"] = ValueText;
                        mOptions.Rows[i]["Dirty"]     = true;
                    }
                    Found = true;
                    break;
                }
            }

            if (!Found)
            {
               DataRow newRow = mOptions.NewRow();
               newRow["ParentCode"] = ParentCode;
               newRow["OptionCode"] = OptionCode;
               newRow["VarDefault"] = ValueText;
               newRow["Dirty"]      = true;
                if (!string.IsNullOrEmpty(ParentId))
                    newRow["ParentID"]   = ParentId;
               mOptions.Rows.Add(newRow);
            }
        }


        private void CreateTable()
        {
            mOptions = new DataTable();
            mOptions.Columns.Add("Value", System.Type.GetType("System.String"));
            mOptions.Columns.Add("ValueId", System.Type.GetType("System.Int32"));
            mOptions.Columns.Add("ParentId", System.Type.GetType("System.Int32"));
            mOptions.Columns.Add("OptionId", System.Type.GetType("System.Int32"));
            mOptions.Columns.Add("ParentCode", System.Type.GetType("System.String"));
            mOptions.Columns.Add("OptionCode", System.Type.GetType("System.String"));
            mOptions.Columns.Add("VarDefault", System.Type.GetType("System.String"));
            mOptions.Columns.Add("ValueText", System.Type.GetType("System.String"));
            mOptions.Columns.Add("ValueType", System.Type.GetType("System.String"));
            mOptions.Columns.Add("Computer", System.Type.GetType("System.Boolean"));
            mOptions.Columns.Add("ComputerId", System.Type.GetType("System.Int32"));
            mOptions.Columns.Add("ComputerRole", System.Type.GetType("System.Boolean"));
            mOptions.Columns.Add("ComputerRoleId", System.Type.GetType("System.Int32"));
            mOptions.Columns.Add("Worker", System.Type.GetType("System.Boolean"));
            mOptions.Columns.Add("WorkerId", System.Type.GetType("System.Int32"));
            mOptions.Columns.Add("WorkerRole", System.Type.GetType("System.Boolean"));
            mOptions.Columns.Add("WorkerRoleId", System.Type.GetType("System.Int32"));
            mOptions.Columns.Add("ValueTypeId", System.Type.GetType("System.Int32"));
            mOptions.AcceptChanges();
        }

        #region IDisposable Implementation
        // To detect redundant calls
        private bool disposedValue = false;
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (mOptions != null )
                    {
                        mOptions.Dispose();
                        mOptions = null;
                    }
                    // TODO: free managed resources when explicitly called
                }

                // TODO: free shared unmanaged resources
            }
            this.disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
