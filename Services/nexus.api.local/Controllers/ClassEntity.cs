
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiClassEntity : ControllerBase
    {
        [HttpPost("ClassEntity")]
        [Authorize]
        public IActionResult Post([FromBody] ClassEntityRequest request)
        {
            ClassEntityResponse response = new ClassEntityResponse()
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
                        ClassEntity clas = request.ClassEntity;
                        if (clas.HasData())
                        {
                            if (clas.ClassId > 0)                       Params += ";I~S~ClassId~" +     clas.ClassId.ToString();
                            if (clas.EntityId > 0)                      Params += ";I~S~EntityId~" +    clas.EntityId.ToString();
                            if (!string.IsNullOrEmpty(clas.Class))      Params += ";I~S~Class~" +       clas.Class.ToString();
                            if (!string.IsNullOrEmpty(clas.ClassType))  Params += ";I~S~ClassType~" +   clas.ClassType.ToString();
                            if (!string.IsNullOrEmpty(clas.ClassState)) Params += ";I~S~ClassState~" +  clas.ClassState.ToString();
                        }

                        diag = "2";
                        helpers.WriteToLog("apiClassEntity:" + Params);
                        DataTable dt = db.GetDataTable("loc.api_Class_Entity", Params);
                        if (dt.Rows.Count > 0)
                        {
                            response.Status   = dt.Rows[0]["Status"].ToString();
                            response.Response = dt.Rows[0]["Response"].ToString();

                            ClassEntity[] ClassEntitys = new ClassEntity[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                diag = "3" + i.ToString();
                                clas = new ClassEntity()
                                {
                                    Parent     =                 dt.Rows[i]["Parent"].ToString(),
                                    Class      =                 dt.Rows[i]["Class"].ToString(),
                                    Entity     =                 dt.Rows[i]["Entity"].ToString(),
                                    ClassType  =                 dt.Rows[i]["ClassType"].ToString(),
                                    ClassState =                 dt.Rows[i]["ClassState"].ToString(),
                                    ClassId    = helpers.ToInt ( dt.Rows[i]["ClassId"].ToString()),
                                    EntityId   = helpers.ToInt ( dt.Rows[i]["EntityId"].ToString()),
                                    ParentId   = helpers.ToInt ( dt.Rows[i]["ParentId"].ToString())
                                };
                                ClassEntitys[i] = clas;

                            }
                            response.Classes = ClassEntitys;

                            if (response.Status.Equals("Valid")) return Ok(response);
                        }
                        else
                        {
                            response.Status   = "Error";
                            response.Response = "No Data Found, " + db.DataError;
                        }
                    }
                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("ClassEnt Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(response);
        }

    }
}
