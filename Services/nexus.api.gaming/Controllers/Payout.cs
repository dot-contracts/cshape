
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
    public class apiPayout : ControllerBase
    {
        [HttpPost("Payout")]
        [Authorize]
        public IActionResult Post([FromBody] PayoutRequest request)
        {
            PayoutResponse response = new PayoutResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request!=null)
            {
                if (helpers.IsDate(request.OpenDate) && helpers.IsDate(request.CloseDate))
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {
                        DataTable Cash = db.GetDataTable("Select Device As Type, sum(Amount)*-1 as Amount from ahis.vue_Lineitem_Cashless where device like '%crt%' and CloseDate>='" + request.OpenDate + "' and CloseDate<='" +  request.CloseDate + "' Group By Device");
                        if (Cash.Columns.Count.Equals(0))  // if no data found, add the columns and get it r5eady for the other payout types
                        {
                            Cash.Columns.Add(new DataColumn("Type",   Type.GetType("System.String")));
                            Cash.Columns.Add(new DataColumn("Amount", Type.GetType("System.Decimal")));
                            Cash.AcceptChanges();
                        }

                        string SQL = "SELECT l.LineitemType, SUM(l.Amount) AS Amount FROM ahis.vue_Batch AS b LEFT OUTER JOIN ahis.vue_LineItem AS l ON b.BatchPK = l.FloatId WHERE(b.CloseDate >= '" + request.OpenDate + "') AND(b.CloseDate <= '" + request.CloseDate + "') AND(ISNULL(l.LineitemType, '') <> '') AND(ISNULL(l.LineitemType, '') NOT IN('Float Close', 'Float In')) GROUP BY l.LineitemType";
                        DataTable DT = db.GetDataTable(SQL);
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            DataRow newRow = Cash.NewRow();
                            newRow[0] = DT.Rows[i][0];
                            newRow[1] = DT.Rows[i][1];
                            Cash.Rows.Add(newRow);
                        }

                        if (Cash.Rows.Count > 0)
                        {
                            response.Status   = "Valid";
                            response.Response = "";

                            Payout[] Results = new Payout[Cash.Rows.Count];
                            for (int i = 0; i < Cash.Rows.Count; i++)
                            {
                                Payout Result = new Payout()
                                {
                                    Description =                   Cash.Rows[i]["Type"].ToString(),
                                    Value       = helpers.ToDecimal(Cash.Rows[i]["Amount"].ToString()),
                                };
                                Results[i] = Result;
                            }
                            response.Results = Results;

                            if (response.Status.Equals("Valid")) return Ok(response);
                        }
                        else    response.Response = "No Data Found, " + db.DataError;
                    }
                }
            }

            return BadRequest(response);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public PayoutResponse getPayout(int ActionType, string Vendor, string Location, string Search = "", string Value = "")
        {
            PayoutRequest request = new PayoutRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Search     = Search,
                Value      = Value
            };

            PayoutResponse response = new PayoutResponse();

            apiPayout api = new apiPayout();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (PayoutResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (PayoutResponse)bad.Value;
            return response;
        }

    }
}
