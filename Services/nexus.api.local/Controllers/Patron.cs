
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{

    [Route("loc")]
    public class apiPatron : ControllerBase
    {
        [HttpPost("Patron")]
        [Authorize]
        public IActionResult Post([FromBody] PatronRequest request)
        {
            PatronResponse response = new PatronResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request != null)
            {
                bool process = true;

                string diag = "";
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {
                        DataTable dt = new DataTable();
                    
                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;
                               Params += ";I~S~PatronType~" + request.PatronType;

                        if (request.Patron != null)
                        {
                            Patron patron = request.Patron;

                            bool HasData = false;
                        
                            if (request.ActionType>0 || (request.ActionType == 0 && patron.ValidSearch()))
                            {
                                diag = "1";
                                if (patron.HasData())
                                { 
                                    if (patron.MemberId  > 0)                           Params += ";I~S~PatronPk~" +        patron.MemberId.ToString();
                                    if (!string.IsNullOrEmpty(patron.MemberType))       Params += ";I~S~MemberType~" +      patron.MemberType;
                                    if (!string.IsNullOrEmpty(patron.MemberState))      Params += ";I~S~PatronState~" +     patron.MemberState;
                                    if (!string.IsNullOrEmpty(patron.Gender))           Params += ";I~S~Gender~" +          patron.Gender;
                                    if (!string.IsNullOrEmpty(patron.Title))            Params += ";I~S~Title~" +           patron.Title;
                                    if (!string.IsNullOrEmpty(patron.FirstName))        Params += ";I~S~FirstName~" +       patron.FirstName;
                                    if (!string.IsNullOrEmpty(patron.OtherName))        Params += ";I~S~OtherName~" +       patron.OtherName;
                                    if (!string.IsNullOrEmpty(patron.LastName))         Params += ";I~S~LastName~" +        patron.LastName;
                                    if (!string.IsNullOrEmpty(patron.NickName))         Params += ";I~S~NickName~" +        patron.NickName;
                                    if (!string.IsNullOrEmpty(patron.MaritalState))     Params += ";I~S~MaritalState~" +    patron.MaritalState;

                                    diag = "2";
                                    if (helpers.IsDate(patron.BirthDate))               Params += ";I~D~BirthDate~" +       patron.BirthDate.ToString();
                                    if (patron.NextOfKin > 0)                           Params += ";I~S~NextOfKin~" +       patron.NextOfKin.ToString();
                                    if (!string.IsNullOrEmpty(patron.Occupation))       Params += ";I~S~Occupation~" +      patron.Occupation;
                                    if (!string.IsNullOrEmpty(patron.OccupationProp))   Params += ";I~S~OccupationProp~" +  patron.OccupationProp;
                                    if (!string.IsNullOrEmpty(patron.BadgeNo))          Params += ";I~S~BadgeNo~" +         patron.BadgeNo;
                                    if (!string.IsNullOrEmpty(patron.CardNo))           Params += ";I~S~CardNo~" +          patron.CardNo;

                                    diag = "3";
                                    if (patron.PatronStateId > 0)                       Params += ";I~S~PatronStateId~" +   patron.PatronStateId.ToString();
                                    if (patron.MemberTypeId > 0)                        Params += ";I~S~MemberTypeId~" +    patron.MemberTypeId.ToString();
                                    if (patron.GenderId > 0)                            Params += ";I~S~GenderId~" +        patron.GenderId.ToString();
                                    if (patron.TitleId > 0)                             Params += ";I~S~TitleId~" +         patron.TitleId.ToString();
                                    if (patron.MaritalStateId > 0)                      Params += ";I~S~MaritalStateId~" +  patron.MaritalStateId.ToString();
                                    if (patron.PhoneId > 0)                             Params += ";I~S~PhoneId~" +         patron.PhoneId.ToString();
                                    if (patron.AddressId > 0)                           Params += ";I~S~AddressId~" +       patron.AddressId.ToString();
                                    if (patron.EMailId  > 0)                            Params += ";I~S~EMailId~" +         patron.EMailId.ToString();
                                    if (patron.NoticeId > 0)                            Params += ";I~S~NoticeId~" +        patron.NoticeId.ToString();
                                    if (patron.MessageId > 0)                           Params += ";I~S~MessageId~" +       patron.MessageId.ToString();
                                    if (patron.CredentialId > 0)                        Params += ";I~S~CredentialId~" +    patron.CredentialId.ToString();
                                    if (patron.PromotionId > 0)                         Params += ";I~S~PromotionId~" +     patron.PromotionId.ToString();
                                    if (patron.PointsRate > 0)                          Params += ";I~S~PointsRate~" +      patron.PointsRate.ToString();
                                    if (helpers.IsDate(patron.PromoExpiry))             Params += ";I~D~PromoExpiry~" +     patron.PromoExpiry.ToString();
                                    if (patron.ImportId > 0)                            Params += ";I~S~ImportId~" +        patron.ImportId.ToString();
                                }

                                diag = "4";
                                helpers.WriteToLog("apiPatron:" + Params);
                                dt = db.GetDataTable("loc.api_Human_Patron", Params);
                                HasData = (dt.Rows.Count > 0);
                            }
                            if (HasData)
                            {
                                response.Status   = dt.Rows[0]["Status"].ToString();
                                response.Response = dt.Rows[0]["Response"].ToString();

                                diag = "5";
                                Patron[] Patrons = new Patron[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "6" + i.ToString();
                                    patron = new Patron()
                                    {
                                        MemberId        = helpers.ToInt(  dt.Rows[i]["PatronPk"].ToString()),
                                        MemberType      =                 dt.Rows[i]["MemberType"].ToString(),
                                        MemberState     =                 dt.Rows[i]["PatronState"].ToString(),
                                        BadgeNo         =                 dt.Rows[i]["BadgeNo"].ToString(),
                                        CardNo          =                 dt.Rows[i]["CardNo"].ToString(),
                                        Gender          =                 dt.Rows[i]["Gender"].ToString(),
                                        Title           =                 dt.Rows[i]["Title"].ToString(),
                                        FirstName       =                 dt.Rows[i]["FirstName"].ToString(),
                                        OtherName       =                 dt.Rows[i]["OtherName"].ToString(),
                                        LastName        =                 dt.Rows[i]["LastName"].ToString(),
                                        NickName        =                 dt.Rows[i]["NickName"].ToString(),
                                        UseNickName     = helpers.ToInt  (dt.Rows[i]["UseNickName"].ToString()),
                                        BirthDate       = helpers.ToDate (dt.Rows[i]["BirthDate"].ToString()).ToString("dd MMM, yyyy"),
                                        NextOfKin       = helpers.ToInt  (dt.Rows[i]["NextOfKinId"].ToString()),
                                        MaritalState    =                 dt.Rows[i]["MaritalState"].ToString(),

                                        PatronTypeId    = helpers.ToInt  (dt.Rows[i]["PatronTypeId"].ToString()),
                                        PatronStateId   = helpers.ToInt  (dt.Rows[i]["PatronStateId"].ToString()),
                                        MemberTypeId    = helpers.ToInt  (dt.Rows[i]["MemberTypeId"].ToString()),
                                        GenderId        = helpers.ToInt  (dt.Rows[i]["GenderId"].ToString()),
                                        TitleId         = helpers.ToInt  (dt.Rows[i]["TitleId"].ToString()),
                                        MaritalStateId  = helpers.ToInt  (dt.Rows[i]["MaritalStateId"].ToString()),
                                        Occupation      =                 dt.Rows[i]["Occupation"].ToString(),
                                        OccupationProp  =                 dt.Rows[i]["OccupationProp"].ToString(),
                                        NoticeId        = helpers.ToInt  (dt.Rows[i]["NoticeId"].ToString()),
                                        MessageId       = helpers.ToInt  (dt.Rows[i]["MessageId"].ToString()),
                                        CredentialId    = helpers.ToInt  (dt.Rows[i]["CredentialId"].ToString()),
                                        PromotionId     = helpers.ToInt  (dt.Rows[i]["PromotionId"].ToString()),
                                        PointsRate      = helpers.ToDbl  (dt.Rows[i]["PointsRate"].ToString()),
                                        PromoExpiry     = helpers.ToDate (dt.Rows[i]["PromoExpiry"].ToString()).ToString("dd MMM, yyyy HH:mm:ss"),
                                        ImportId        = helpers.ToInt  (dt.Rows[i]["ImportId"].ToString()),
                                    };
                                    Patrons[i] = patron;
                                }
                                response.Patrons = Patrons;

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
                    helpers.WriteToLog("Patron Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(response);



            ////var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            ////IConfiguration Configuration = builder.Build();
            ////using (SQLServer db = new SQLServer(Configuration.GetConnectionString("DefaultConnection").ToString()))

            //if (request != null)
            //{
            //    apiPatron     apiPatron     = new apiPatron();
            //    apiMemberClub apiMemberClub = new apiMemberClub();
            //    apiClub       apiClub       = new apiClub();
            //    apiCard       apiCard       = new apiCard();
            //    apiContact    apiContact    = new apiContact();
            //    apiIdentity   apiIdentity   = new apiIdentity();
            //    apiBadge      apiBadge      = new apiBadge();

            //    // first define the Human Request
            //    PatronRequest humReq = new PatronRequest()
            //    {
            //        ActionType = request.ActionType,
            //        Vendor     = request.Vendor,
            //        Location   = request.Location,
            //        Patron     = request.Patron,
            //        CardNo     = request.CardNo,
            //        BadgeNo    = request.BadgeNo,
            //    };

            //    IActionResult humRes = apiPatron.Post(humReq);  // Post to Human 

            //    if (humRes is OkObjectResult okHuman)
            //    {
            //        var humData      = okHuman.Value as HumanResponse;
            //        request.Status   = humData.Status;
            //        request.Response = humData.Response;

            //        Human human = humData.Humans[0];

                   

            //        for (int i = 0; i < memReq.MemberClubs.Length; i++)  // do the clubs first
            //        {
            //            memReq.MemberClubs[i].MemberId = human.HumanId;
            //            MemberClubResponse mcRes = apiMemberClub.getMemberClub(humReq.ActionType, humReq.Vendor, humReq.Location, memReq.MemberClubs[i]);
            //            process = mcRes.Status.Equals("Valid");
            //            memRes.Status = mcRes.Status;
            //            memRes.Response = mcRes.Response;
            //            if (!memRes.Status.Equals("Valid")) break;
            //        }



            //        if (humReq.ActionType == 1)
            //        {
            //            string BadgeNo = "";
            //            for (int i = 0; i < memReq.MemberClubs.Length; i++)  // do the clubs first
            //            {
            //                if (!string.IsNullOrEmpty(memReq.MemberClubs[i].BadgeNo))
            //                {
            //                    BadgeNo = memReq.MemberClubs[i].BadgeNo;
            //                    break;
            //                }

            //            }
            //            if (string.IsNullOrEmpty(BadgeNo))
            //            {
            //                Badge badge = new Badge();
            //                BadgeResponse badRes = apiBadge.getBadge(humReq.ActionType, humReq.Vendor, humReq.Location, badge);
            //                if (badRes.Status.Equals("Valid"))
            //                {
            //                    BadgeNo = badRes.Badges[0].BadgeNo;
            //                    for (int i = 0; i < memReq.MemberClubs.Length; i++)  // do the clubs first
            //                        memReq.MemberClubs[i].BadgeNo = BadgeNo;
            //                }
            //            }

            //        }



            //        if (process)
            //        {
            //            Human[] Humans = humData.Humans;
            //            Member[] Members = new Member[Humans.Length];
            //            for (int i = 0; i < Humans.Length; i++)
            //            {
            //                Member member = new Member() { Human = Humans[i] };

            //                MemberClub memClub = new MemberClub() { MemberId = member.Human.HumanId };
            //                MemberClubResponse mcRes = apiMemberClub.getMemberClub(0, humReq.Vendor, humReq.Location, memClub);
            //                member.MemberClubs = mcRes.MemberClubs;

            //                Card card = memReq.Card; card.HumanId = member.Human.HumanId;
            //                CardResponse cRes = apiCard.getCard(0, humReq.Vendor, humReq.Location, card);
            //                if (cRes.Cards.Count() > 0) member.Card = cRes.Cards[0];

            //                Contact contact = new Contact(); contact.EntityId = member.Human.HumanId;
            //                ContactResponse ctRes = apiContact.getContact(0, humReq.Vendor, humReq.Location, contact);
            //                member.Contacts = ctRes.Contacts;

            //                Identity identity = new Identity(); identity.EntityId = member.Human.HumanId;
            //                IdentityResponse idRes = apiIdentity.getIdentity(0, humReq.Vendor, humReq.Location, identity);
            //                member.Identities = idRes.Identities;

            //                Members[i] = member;
            //            }
            //            memRes.Members = Members;
            //        }

            //        if (memRes.Status.Equals("Valid")) return Ok(memRes);
            //    }
            //    else if (humRes is BadRequestObjectResult badHuman)
            //    {
            //        var humData = badHuman.Value as HumanResponse;
            //        memRes.Status = humData.Status;
            //        memRes.Response = humData.Response;
            //    }
            //}
            //return BadRequest(memRes);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public PatronResponse getPatrons(int ActionType, string Vendor, string Location, Patron[] Patrons)
        {
            return getPatron(ActionType, Vendor, Location, Patrons[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public PatronResponse getPatron(int ActionType, string Vendor, string Location, Patron Patron)
        {
            PatronRequest  request  = new PatronRequest(ActionType, Vendor, Location, Patron);
            PatronResponse response = new PatronResponse();

            apiPatron apiPatron = new apiPatron();
            IActionResult action = apiPatron.Post(request);
            if (action is OkObjectResult ok) response = (PatronResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (PatronResponse)bad.Value;
            return response;
        }

    }
}
