
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
//using StackExchange.Redis;

namespace nexus.api.Controller.Local
{

    [Route("mem")]
    public class apiMember : ControllerBase
    {
        [HttpPost("Member")]
        [Authorize]
        public IActionResult Post([FromBody] MemberRequest request)
        {

            MemberResponse memRes = new MemberResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            //IConfiguration Configuration = builder.Build();
            //using (SQLServer db = new SQLServer(Configuration.GetConnectionString("DefaultConnection").ToString()))

            if (request != null)
            {
                setting.GetSettings();
                string conStr = setting.ConnectionString;//   Configuration.GetConnectionString("DefaultConnection").ToString();
                using (SQLServer db = new SQLServer(conStr))
                {
                    int MainClub = helpers.ToInt(db.ExecLookup("Select top 1 clubpk from mem.club where clubtype= cmn.getValuePK('Club','Main') order by clubpk desc"));

                    string Params  =  "I~S~ActionType~" + request.ActionType;
                           Params += ";I~S~Vendor~"     + request.Vendor;
                           Params += ";I~S~Location~"   + request.Location;

                    helpers.WriteToLog("apiMember:" + Params);

                    if (request.Member != null)
                    {

                        MemberClub         memClub = new MemberClub();
                        MemberClubRequest  memReq  = new MemberClubRequest();
                        apiMemberClub      mcapi   = new apiMemberClub();

                        Card               card     = new Card();
                        CardRequest        cardReq  = new CardRequest(request.ActionType, request.Vendor, request.Location, card);
                        apiCard            cardApi  = new apiCard();
 
                        Contact            contact  = new Contact();
                        ContactRequest     conReq   = new ContactRequest(request.ActionType, request.Vendor, request.Location, contact);
                        apiContact         conApi   = new apiContact();

                        ClassEntity        interest = new ClassEntity();
                        ClassEntityRequest idReq    = new ClassEntityRequest(request.ActionType, request.Vendor, request.Location, interest);
                        apiClassEntity     idApi    = new apiClassEntity();

                        MemberClubDelivery        memClubD = new MemberClubDelivery();
                        MemberClubDeliveryRequest mcdReq   = new MemberClubDeliveryRequest();
                        apiMemberClubDelivery     mcdapi   = new apiMemberClubDelivery();

                        PatronRequest patReq = new PatronRequest(request.ActionType, request.Vendor, request.Location);
                        apiPatron patapi     = new apiPatron();
                        patReq.Patron        = request.Member.Patron;
                        patReq.PatronType    = "Member";
                        IActionResult post   = patapi.Post(patReq);

                        if (post is OkObjectResult patok)
                        {
                            var data = patok.Value as PatronResponse;
                            memRes.Member.Patron = data.Patrons[0];
                            memRes.Status        = data.Status;
                            memRes.Response      = data.Response;
                        }
                        else if (post is BadRequestObjectResult patbad)
                        {
                            var data = patbad.Value as PatronResponse;
                            memRes.Status        = data.Status;
                            memRes.Response      = data.Response;
                            memRes.Member.Patron = data.Patrons[0];
                        }

                        
                        int MemberId = helpers.ToInt(memRes.Member.Patron.MemberId.ToString());

                        if (memRes.Status.Equals("Valid"))
                        {
                            if (MemberId > 0)
                            {
                                if (request.ActionType > 0)  //if creating or editing, update extra data
                                {
                                    bool Found = false;

                                    for (int i = 0; i < request.Member.MemberClub.MemberClubs.Length; i++)
                                    {
                                        memClub = request.Member.MemberClub.MemberClubs[i];    // Process the Member Clubs
                                        if (memClub.HasData())
                                        {
                                            memReq = new MemberClubRequest(request.ActionType, request.Vendor, request.Location, memClub, MemberId);

                                            if (memReq.MemberClub.ClubId <= 0) memReq.MemberClub.ClubId = helpers.ToInt(db.ExecLookup("Select ClubId from mem.vue_club where Description='" + memClub.Club + "'"));
                                            if (memReq.MemberClub.ClubId <= 0) memReq.MemberClub.ClubId = MainClub;

                                            post = mcapi.Post(memReq);
                                            Found = true;

                                            if (post is BadRequestObjectResult bad)
                                            {
                                                var data = bad.Value as MemberClubResponse;
                                                memRes.Member.MemberClub.Status = data.Status;
                                                memRes.Member.MemberClub.Response = data.Response;
                                            }
                                        }
                                    }

                                    if (!Found)                         // no member club in request default it to use the main club
                                    {
                                        memClub = new MemberClub();
                                        memClub.MemberId = MemberId;
                                        memClub.ClubId = MainClub;
                                        memClub.BadgeNo = memRes.Member.Patron.BadgeNo;
                                        memReq = new MemberClubRequest(request.ActionType, request.Vendor, request.Location, memClub, MemberId);
                                        post = mcapi.Post(memReq);

                                        if (post is BadRequestObjectResult bad)
                                        {
                                            var data = bad.Value as MemberClubResponse;
                                            memRes.Member.MemberClub.Status = data.Status;
                                            memRes.Member.MemberClub.Response = data.Response;
                                        }
                                    }

                                    Found = false;
                                    for (int i = 0; i < request.Member.Card.Cards.Length; i++)
                                    {
                                        card = request.Member.Card.Cards[i];    // Process the Card
                                        if (card.HasData())
                                        {
                                            cardReq = new CardRequest(request.ActionType, request.Vendor, request.Location, card, MemberId);
                                            post = cardApi.Post(cardReq);
                                            Found = true;

                                            if (post is BadRequestObjectResult bad)
                                            {
                                                Found = false;
                                                var data = bad.Value as CardResponse;
                                                memRes.Member.Card.Status = data.Status;
                                                memRes.Member.Card.Response = data.Response;
                                                break;
                                            }
                                        }
                                    }

                                    if (!Found && !string.IsNullOrEmpty(patReq.Patron.CardNo))   // no card in request but there is a card number in header so use it.
                                    {
                                        card = new Card();
                                        card.CardNo = patReq.Patron.CardNo;
                                        card.PinNo = "0000";
                                        cardReq = new CardRequest(request.ActionType, request.Vendor, request.Location, card, MemberId);
                                        post = cardApi.Post(cardReq);
                                        if (post is BadRequestObjectResult bad)
                                        {
                                            var data = bad.Value as CardResponse;
                                            memRes.Member.Card.Status = data.Status;
                                            memRes.Member.Card.Response = data.Response;
                                        }
                                    }

                                    for (int i = 0; i < request.Member.Contact.Contacts.Length; i++)
                                    {
                                        contact = request.Member.Contact.Contacts[i];    // Process the Member Clubs
                                        if (contact.HasData())
                                        {
                                            conReq = new ContactRequest(request.ActionType, request.Vendor, request.Location, contact, MemberId);
                                            post = conApi.Post(conReq);

                                            if (post is BadRequestObjectResult conBad)
                                            {
                                                var data = conBad.Value as ContactResponse;
                                                memRes.Member.Contact.Status = data.Status;
                                                memRes.Member.Contact.Response = data.Response;
                                            }
                                        }
                                    }

                                    // get defaults and update patron record
                                    int SMSId = helpers.ToInt(db.ExecLookup("Select top 1 ContactPk from loc.vue_contact where contactcode='phone'   and entityid=" + MemberId + " order by cast(isnull([default],0) as int)"));
                                    int EDMId = helpers.ToInt(db.ExecLookup("Select top 1 ContactPk from loc.vue_contact where contactcode='digital' and entityid=" + MemberId + " order by cast(isnull([default],0) as int)"));
                                    int PSTId = helpers.ToInt(db.ExecLookup("Select top 1 ContactPk from loc.vue_contact where contactcode='address' and entityid=" + MemberId + " order by cast(isnull([default],0) as int)"));

                                    if (PSTId > 0 || EDMId > 0 || SMSId > 0)
                                    {
                                        patReq.ActionType = 2;
                                        patReq.Patron.MemberId = MemberId;
                                        patReq.Patron.PhoneId = SMSId;
                                        patReq.Patron.AddressId = PSTId;
                                        patReq.Patron.EMailId = EDMId;
                                        post = patapi.Post(patReq);

                                        for (int i = 0; i < request.Member.MemberClubDelivery.MemberClubDeliveries.Length; i++)
                                        {
                                            memClubD = request.Member.MemberClubDelivery.MemberClubDeliveries[i];
                                            if (memClubD.HasData())
                                            {
                                                mcdReq = new MemberClubDeliveryRequest(request.ActionType, request.Vendor, request.Location, memClubD, MemberId);
                                                mcdReq.MemberClubDelivery.ClubId = MainClub;
                                                mcdReq.MemberClubDelivery.POST = PSTId;
                                                mcdReq.MemberClubDelivery.SMS = SMSId;
                                                mcdReq.MemberClubDelivery.EDM = EDMId;

                                                post = mcdapi.Post(mcdReq);

                                                if (post is BadRequestObjectResult conBad)
                                                {
                                                    var data = conBad.Value as MemberClubDeliveryResponse;
                                                    memRes.Member.MemberClubDelivery.Status = data.Status;
                                                    memRes.Member.MemberClubDelivery.Response = data.Response;
                                                }
                                            }
                                        }
                                    }

                                    for (int i = 0; i < request.Member.Interest.Classes.Length; i++)
                                    {
                                        interest = request.Member.Interest.Classes[i];    // Process the Member Clubs
                                        if (interest.HasData())
                                        {
                                            idReq = new ClassEntityRequest(request.ActionType, request.Vendor, request.Location, interest, MemberId);
                                            post = idApi.Post(idReq);

                                            if (post is OkObjectResult idOk)
                                            {
                                                var data = idOk.Value as ClassEntityResponse;
                                            }
                                            else if (post is BadRequestObjectResult bad)
                                            {
                                                var data = bad.Value as ClassEntityResponse;
                                                memRes.Member.Interest.Status = data.Status;
                                                memRes.Member.Interest.Response = data.Response;
                                            }

                                        }
                                    }

                                }
                            }
                            else
                            {
                                memRes.Status = "Error";
                                memRes.Response = "No Data Found, " + db.DataError;
                            }
                        }

                        if (MemberId>0)
                        {

                            // go ahead and retreive the members details
                            memReq   = new MemberClubRequest(ActionType: 0, Vendor: request.Vendor, Location: request.Location, MemberId: MemberId);
                            post          = mcapi.Post(memReq);

                            if (post is OkObjectResult ok)
                            {
                                var data = ok.Value as MemberClubResponse;
                                memRes.Member.MemberClub.MemberClubs = data.MemberClubs;
                                memRes.Member.MemberClub.Status      = data.Status;
                                memRes.Member.MemberClub.Response    = data.Response;
                            }
                            else if (post is BadRequestObjectResult bad)
                            {
                                var data = bad.Value as MemberClubResponse;
                                memRes.Member.MemberClub.Status   = data.Status;
                                memRes.Member.MemberClub.Response = data.Response;
                            }

                            cardReq = new CardRequest(ActionType:0, Vendor: request.Vendor,Location: request.Location, HumanId: MemberId);
                            post     = cardApi.Post(cardReq);

                            if (post is OkObjectResult cardok)
                            {
                                var data = cardok.Value as CardResponse;
                                memRes.Member.Card.Cards    = data.Cards;
                                memRes.Member.Card.Status   = data.Status;
                                memRes.Member.Card.Response = data.Response;
                            }
                            else if (post is BadRequestObjectResult bad)
                            {
                                var data = bad.Value as CardResponse;
                                memRes.Member.Card.Status   = data.Status;
                                memRes.Member.Card.Response = data.Response;
                            }

                            conReq = new ContactRequest(ActionType: 0, Vendor: request.Vendor, Location: request.Location,EntityId: MemberId);
                            post = conApi.Post(conReq);

                            if (post is OkObjectResult conok)
                            {
                                var data = conok.Value as ContactResponse;
                                memRes.Member.Contact.Contacts = data.Contacts;
                                memRes.Member.Contact.Status   = data.Status;
                                memRes.Member.Contact.Response = data.Response;
                            }
                            else if (post is BadRequestObjectResult bad)
                            {
                                var data = bad.Value as ContactResponse;
                                memRes.Member.Contact.Status   = data.Status;
                                memRes.Member.Contact.Response = data.Response;
                            }

                            idReq = new ClassEntityRequest(ActionType: 0, Vendor: request.Vendor, Location: request.Location, EntityId: MemberId);
                            post = idApi.Post(idReq);

                            if (post is OkObjectResult idok)
                            {
                                var data = idok.Value as ClassEntityResponse;
                                memRes.Member.Interest.Classes  = data.Classes;
                                memRes.Member.Interest.Status   = data.Status;
                                memRes.Member.Interest.Response = data.Response;
                            }
                            else if (post is BadRequestObjectResult bad)
                            {
                                var data = bad.Value as ClassEntityResponse;
                                memRes.Member.Interest.Status   = data.Status;
                                memRes.Member.Interest.Response = data.Response;
                            }

                            mcdReq = new MemberClubDeliveryRequest(ActionType: 0, request.Vendor, request.Location, memClubD, MemberId);
                            //mcdReq.MemberClubDelivery.ClubId = MainClub;
                            post = mcdapi.Post(mcdReq);

                            if (post is OkObjectResult mcdok)
                            {
                                var data = mcdok.Value as MemberClubDeliveryResponse;
                                memRes.Member.MemberClubDelivery.MemberClubDeliveries = data.MemberClubDeliveries;
                                memRes.Member.Interest.Status   = data.Status;
                                memRes.Member.Interest.Response = data.Response;
                            }
                            else if (post is BadRequestObjectResult bad)
                            {
                                var data = bad.Value as MemberClubDeliveryResponse;
                                memRes.Member.MemberClubDelivery.Status   = data.Status;
                                memRes.Member.MemberClubDelivery.Response = data.Response;
                            }

                        }


                        if (memRes.Status.Equals("Valid")) return Ok(memRes);
                    }
                }
            }

            return BadRequest(memRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MemberResponse getMembers(int ActionType, string Vendor, string Location, Member[] Members)
        {
            return getMember(ActionType, Vendor, Location, Members[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public MemberResponse getMember(int ActionType, string Vendor, string Location, Member Member)
        {
            MemberRequest request = new MemberRequest()
            {
                ActionType = ActionType,
                Vendor     = Vendor,
                Location   = Location,
                Member      = Member
            };

            MemberResponse response = new MemberResponse();

            apiMember api = new apiMember();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (MemberResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (MemberResponse)bad.Value;
            return response;
        }

    }
}
