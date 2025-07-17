
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
using Microsoft.OpenApi.Any;

namespace nexus.api.gaming
{
    [Route("cmn")]
    public class apiEvent : ControllerBase
    {
        [HttpPost("Event")]
        [Authorize]
        public IActionResult Post([FromBody] EventRequest request)
        {
            EventResponse response = new EventResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                using (SQLServer db = new SQLServer(setting.ConnectionString))
                {
                    string SQL = "SELECT " + (string.IsNullOrEmpty(request.MaxRows) ? "" : "top " + request.MaxRows) + " EventPK, EventDate, Property, Description, Entity, Human, EntityID, HumanID ";
                    if (helpers.IsDate(request.OpenDate) || helpers.IsDate(request.CloseDate))
                        SQL += "FROM ahis.vue_Occurrence_Event ";
                    else
                        SQL += "FROM  cmn.vue_Occurrence_Event ";

                    string Where = "";
                    if (helpers.IsDate(request.OpenDate)) Where = "EventDate>='" + request.OpenDate + "'";
                    if (helpers.IsDate(request.CloseDate))
                    {
                        if (!string.IsNullOrEmpty(Where)) Where += " and ";
                        Where += "EventDate<='" + request.CloseDate + "'";
                    }
                    if (!string.IsNullOrEmpty(Where)) Where = " Where " + Where;


                    helpers.WriteToLog("Event:" + SQL + " " + Where);

                    DataTable DT = db.GetDataTable(SQL + " " + Where);

                    if (DT.Rows.Count > 0)
                    {
                        response.Status   = "Valid";
                        response.Response = "";

                        Event[] Results = new Event[DT.Rows.Count];
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            Event Result = new Event()
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
        public EventResponse getEvent(int ActionType, string Vendor, string Location, string Search = "", string Value = "")
        {
            EventRequest request = new EventRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Search     = Search,
                Value      = Value
            };

            EventResponse response = new EventResponse();

            apiEvent api = new apiEvent();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (EventResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (EventResponse)bad.Value;
            return response;
        }

    }
}
