using System;
using System.Data;
using System.Globalization;
using System.IO;


namespace nexus.common

{
    static public partial class helpers
    {
        public static DataTable CreateDataTable(string source)
        {
            DataTable Data = new DataTable();
            Data.Columns.Add("Set", System.Type.GetType("System.Boolean"));
            Data.Columns.Add("Description", System.Type.GetType("System.String"));
            Data.Columns.Add("Id", System.Type.GetType("System.String"));
            Data.AcceptChanges();

            Array Arr = source.Split(new char[] { ';' });
            for (int i = 0; i <= Arr.GetLength(0) - 1; i++)
            {
                try
                {
                    string Name = Arr.GetValue(i).ToString();
                    Name = Name + "," + Name;
                    Array Art = Name.Split(new char[] { ',' });

                    DataRow newRow = Data.NewRow();
                    newRow["Set"] = false;
                    newRow["Description"] = Art.GetValue(0).ToString();
                    newRow["Id"] = Art.GetValue(1).ToString();
                    Data.Rows.Add(newRow);
                }
                catch (Exception) { }
            }

            return Data;
        }

    }

}
