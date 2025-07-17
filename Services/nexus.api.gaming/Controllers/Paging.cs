
using System.Text;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using nexus.common;
using nexus.common.cache;
using nexus.common.dal;
using nexus.shared.common;
using nexus.shared.gaming;
using Newtonsoft.Json.Linq;

namespace nexus.api.gaming
{
    [Route("gam")]
    public class apiPaging : ControllerBase
    {
        [HttpPost("Paging")]
        [Authorize]
        public IActionResult Post([FromBody] PagingRequest request)
        {
            PagingResponse response = new PagingResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                using (SQLServer db = new SQLServer(setting.ConnectionString))
                {
                    DataTable DT = db.GetDataTable("SELECT EventPK, EventDate, Property, Description, Entity, Human, EntityID, HumanID FROM cmn.vue_Occurrence_Event WHERE EventStateID = cmn.getStatePK('event', 'active') AND  (Description ='Pager' or ((Property LIKE '%request drinks%' OR Property LIKE '%request service%')))  order by eventdate");

                    if (DT.Rows.Count > 0)
                    {
                        response.Status   = "Valid";
                        response.Response = "";

                        Paging[] Results = new Paging[DT.Rows.Count];
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            Paging Result = new Paging()
                            {
                                EventDate   =                DT.Rows[i]["EventDate"].ToString(),
                                Property    =                DT.Rows[i]["Property"].ToString(),
                                Description =                DT.Rows[i]["Description"].ToString(),
                                Entity      =                DT.Rows[i]["Entity"].ToString(),
                                Human       =                DT.Rows[i]["Human"].ToString(),
                                EntityId    = helpers.ToInt (DT.Rows[i]["EntityId"].ToString()),
                                HumanId     = helpers.ToInt (DT.Rows[i]["HumanID"].ToString()),
                                EventId     = helpers.ToInt (DT.Rows[i]["EventPK"].ToString()),
                            };
                            Results[i] = Result;
                        }
                        response.Results = Results;

                        if (response.Status.Equals("Valid")) return Ok(response);
                    }
                    else    response.Response = "No Data Found, " + db.DataError;
                }
            }

            return BadRequest(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public PagingResponse getPaging(int ActionType, string Vendor, string Location, string Search = "", string Value = "")
        {
            PagingRequest request = new PagingRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Search     = Search,
                Value      = Value
            };

            PagingResponse response = new PagingResponse();

            apiPaging api = new apiPaging();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (PagingResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (PagingResponse)bad.Value;
            return response;
        }

    }
}
