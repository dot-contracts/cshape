
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//using nexus.web.auth.Cache;
using nexus.web.auth.Data;
using nexus.common;
using nexus.common.dal;
using nexus.api.Model.Local;
using nexus.api.Controller.Membership;
using nexus.api.Model.Membership;
using Microsoft.IdentityModel.Tokens;
using nexus.api.Controller.External;
using Microsoft.Extensions.Hosting;
using nexus.api.Controllers.Local;
using System.Diagnostics;
using System.Collections.Generic;
//using StackExchange.Redis;

namespace nexus.api.Controller.Local
{

    [Route("mem")]
    public class apiVisitor : ControllerBase
    {
        [HttpPost("Visitor")]
        [Authorize]
        public IActionResult Post([FromBody] VisitorRequest request)
        {

            VisitorResponse visRes = new VisitorResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            //IConfiguration Configuration = builder.Build();
            //using (SQLServer db = new SQLServer(Configuration.GetConnectionString("DefaultConnection").ToString()))

            if (request != null)
            {
                string diag = "";
                try
                {
                    setting.GetSettings();
                    string conStr = setting.ConnectionString;//   Configuration.GetConnectionString("DefaultConnection").ToString();
                    using (SQLServer db = new SQLServer(conStr))
                    {

                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;

                        diag = "1";

                        helpers.WriteToLog("apiVisitor:" + Params);

                        if (request.Visitor != null)
                        {
                            PatronRequest patReq = new PatronRequest(request.ActionType, request.Vendor, request.Location);
                            apiPatron     patapi     = new apiPatron();
                            apiCard       cardApi    = new apiCard();

                            diag = "2";

                            Patron patron = new Patron()
                            {
                                FirstName = "Visitor",
                                LastName  = request.Visitor.VisitorNo,
                                Gender    = "Unknown",
                                BirthDate = "Jan 1, 2000"
                            };

                            diag = "3";

                            if (string.IsNullOrEmpty(patron.LastName))
                            {
                                string SQL = "declare @VisNo int declare @Visitors table(VisNo int) ";
                                       SQL += "insert into @Visitors(VisNo) select LastName from loc.human where patrontype = cmn.getvaluepk('Human', 'Visitor') ";
                                       SQL += "select top 1 @VisNo = VisNo from @Visitors order by VisNo desc ";
                                       SQL += "select @VisNo + 1 ";
                                patron.LastName = db.ExecLookup(SQL);
                            }

                            diag = "4";
                            patReq.Patron      = patron;
                            patReq.PatronType  = "Visitor";
                            IActionResult post = patapi.Post(patReq);

                            if (post is OkObjectResult patok)
                            {
                                var data = patok.Value as PatronResponse;
                                visRes.Visitor.VisitorId      = helpers.ToInt( data.Patrons[0].MemberId.ToString());
                                visRes.Visitor.VisitorNo      =                data.Patrons[0].LastName;
                                visRes.Visitor.VisitorType    =                data.Patrons[0].MemberType;
                                visRes.Visitor.VisitorTypeId  = helpers.ToInt( data.Patrons[0].MemberTypeId.ToString());
                                visRes.Visitor.VisitorState   =                data.Patrons[0].MemberState ;
                                visRes.Visitor.VisitorStateId = helpers.ToInt( data.Patrons[0].MemberStateId.ToString());
                                visRes.Status                 = data.Status;
                                visRes.Response               = data.Response;
                                diag = "5";

                            }
                            else if (post is BadRequestObjectResult patbad)
                            {
                                var data = patbad.Value as PatronResponse;
                                visRes.Status   = data.Status;
                                visRes.Response = data.Response;
                            }

                            if (visRes.Status.Equals("Valid"))
                            {
                                diag = "6";
                                int VisitorId = helpers.ToInt(visRes.Visitor.VisitorId.ToString());
                                if (VisitorId > 0)
                                {
                                    Card card = new Card();
                                    card.HumanId     = VisitorId;
                                    card.CardNo      = request.Visitor.CardNo;
                                    card.CardTypeId  = helpers.ToInt(db.ExecLookup("select cmn.getvaluepk('Card', 'Visitor')"));
                                    card.CardStateId = helpers.ToInt(db.ExecLookup("select cmn.getstatePK('Card', 'Available')"));

                                    diag = "7";
                                    card.Account     = new Account[1];
                                    card.Account[0]  = new Account()
                                    {
                                        HumanId        = VisitorId, 
                                        AccountTypeId  = helpers.ToInt(db.ExecLookup("select cmn.getvaluepk('Account', 'Points')")),
                                        AccountStateId = helpers.ToInt(db.ExecLookup("select cmn.getstatePK('Account', 'Available')"))
                                    };

                                    diag = "8";
                                    if (request.ActionType>0)  //if creating or editing, update extra data
                                    {
                                        CardRequest cardReq = new CardRequest(request.ActionType, request.Vendor, request.Location, card, VisitorId );
                                        post = cardApi.Post(cardReq);
                                    }

                                    // go ahead and retreive the Visitors details
                                    if (visRes.Status.Equals("Valid"))
                                    {
                                        visRes.Visitor.VisitorId = VisitorId;

                                        diag = "9";
                                        CardRequest cardReq = new CardRequest(0, request.Vendor, request.Location, card, VisitorId);
                                        post = cardApi.Post(cardReq);

                                        if (post is OkObjectResult cardok)
                                        {
                                            var data = cardok.Value as CardResponse;
                                            visRes.Visitor.CardNo = data.Cards[0].CardNo;
                                            visRes.Status         = data.Status;
                                            visRes.Response       = data.Response;
                                            diag = "10";
                                        }
                                        else if (post is BadRequestObjectResult cardBad)
                                        {
                                            var data = cardBad.Value as CardResponse;
                                            visRes.Status = data.Status;
                                            visRes.Response = data.Response;
                                        }

                                        if (post is BadRequestObjectResult bad)
                                        {
                                            var data = bad.Value as CardResponse;
                                            visRes.Status   = data.Status;
                                            visRes.Response = data.Response;
                                        }
                                    }
                                }

                                if (visRes.Status.Equals("Valid")) return Ok(visRes);
                            }
                            else
                            {
                                visRes.Status   = "Error";
                                visRes.Response = "No Data Found, " + db.DataError;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Visitor Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(visRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public VisitorResponse getVisitors(int ActionType, string Vendor, string Location, Visitor[] Visitors)
        {
            return getVisitor(ActionType, Vendor, Location, Visitors[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public VisitorResponse getVisitor(int ActionType, string Vendor, string Location, Visitor Visitor)
        {
            VisitorRequest request = new VisitorRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Visitor      = Visitor
            };

            VisitorResponse response = new VisitorResponse();

            apiVisitor api = new apiVisitor();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (VisitorResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (VisitorResponse)bad.Value;
            return response;
        }

    }
}
