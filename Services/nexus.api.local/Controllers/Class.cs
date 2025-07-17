
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiClass : ControllerBase
    {
        [HttpPost("Class")]
        [Authorize]
        public IActionResult Post([FromBody] ClassRequest request)
        {
            ClassResponse response = new ClassResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                string diag = "";
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {

                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;

                        diag = "1";

                        if (request.Class != null)
                        {
                            Class clas =  request.Class;
                            if (clas.HasData())
                            {
                                diag = "2";
                                if (!string.IsNullOrEmpty(clas.Parent))        Params += ";I~S~Parent~" +       clas.Parent;
                                if (!string.IsNullOrEmpty(clas.Description))   Params += ";I~S~Description~" +  clas.Description;
                                if (!string.IsNullOrEmpty(clas.ClassType))     Params += ";I~S~ClassType~" +    clas.ClassType;
                                if (!string.IsNullOrEmpty(clas.ClassState))    Params += ";I~S~ClassState~" +   clas.ClassState;

                                diag = "3";
                                if (clas.ClassId>0)                            Params += ";I~S~ClassId~" +      clas.ClassId.ToString();
                                if (clas.ClassTypeId > 0)                      Params += ";I~S~ClassTypeId~" +  clas.ClassTypeId.ToString();
                                if (clas.ClassStateId > 0)                     Params += ";I~S~ClassStateId~" + clas.ClassStateId.ToString();
                                if (clas.ParentId>0)                           Params += ";I~S~ParentId~" +     clas.ParentId.ToString();
                            }

                            diag = "4";
                            helpers.WriteToLog("apiClass:" + Params);
                            DataTable dt = db.GetDataTable("loc.api_Class", Params);
                            if (dt.Rows.Count > 0)
                            {
                                response.Status   = dt.Rows[0]["Status"].ToString();
                                response.Response = dt.Rows[0]["Response"].ToString();

                                Class[] Classs = new Class[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "5" + i.ToString();
                                    clas = new Class()
                                    {
                                        Parent         =                 dt.Rows[i]["Parent"].ToString(),
                                        Description    =                 dt.Rows[i]["Description"].ToString(),
                                        ClassType      =                 dt.Rows[i]["ClassType"].ToString(),
                                        ClassState     =                 dt.Rows[i]["ClassState"].ToString(),
                                        ClassId        = helpers.ToInt ( dt.Rows[i]["ClassId"].ToString()),
                                        ClassTypeId    = helpers.ToInt ( dt.Rows[i]["ClassTypeId"].ToString()),
                                        ClassStateId   = helpers.ToInt ( dt.Rows[i]["ClassStateId"].ToString()),
                                        ParentId       = helpers.ToInt ( dt.Rows[i]["ParentId"].ToString())
                                    };
                                    Classs[i] = clas;

                                }
                                response.Classes = Classs;

                                if (response.Status.Equals("Valid")) return Ok(response);
                            }
                            else
                            {
                                response.Status   = "Error";
                                response.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Class Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(response);
        }

    }
}
