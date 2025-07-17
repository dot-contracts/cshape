using System;
using System.Globalization;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace nexus.shared.common
{
    static public class nextjson
    {
        public static DataTable ArrayToDataTable(Array data)
        {
            DataTable table = new DataTable();

            if (data != null && data.Length > 0)
            {
                // Get properties from the first role
                var properties = ((JObject)data.GetValue(0)).Properties();

                // Create columns based on JSON object keys
                foreach (var prop in properties)
                {
                    table.Columns.Add(prop.Name);
                }

                // Add rows
                foreach (var role in data)
                {
                    var values = ((JObject)role).Properties().Select<JProperty, object>(p => (p.Value.Type == JTokenType.Null) ? DBNull.Value : (object)p.Value).ToArray();
                    table.Rows.Add(values);
                }
            }

            return table;
        }
    }

}
