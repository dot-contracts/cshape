
using System.CodeDom;
using System.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.cache;
using nexus.common.dal;
using nexus.shared.common;
using nexus.shared.application;

namespace nexus.api.application
{

    [Route("app")]
    public class apiWorker : ControllerBase
    {
        [HttpPost("Worker")]
        [Authorize]
        public IActionResult Post([FromBody] WorkerRequest request)
        {
            WorkerResponse response = new WorkerResponse()
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
                    //setting.GetSettings();
                    string conStr = setting.ConnectionString;//   Configuration.GetConnectionString("DefaultConnection").ToString();
                    using (SQLServer db = new SQLServer(conStr))
                    {
                        DataTable dt = new DataTable();
                    
                        string Params  =  "I~S~ActionType~" + request.ActionType;
                               Params += ";I~S~Vendor~"     + request.Vendor;
                               Params += ";I~S~Location~"   + request.Location;
                               Params += ";I~S~WorkerType~" + request.WorkerType;

                        if (request.Worker != null)
                        {
                            Worker Worker = request.Worker;

                            bool HasData = false;
                        
                            if (request.ActionType>0 || (request.ActionType == 0 && Worker.ValidSearch()))
                            {
                                diag = "1";
                                if (Worker.HasData())
                                { 
                                    if (Worker.WorkerId  > 0)                           Params += ";I~S~WorkerPk~" +        Worker.WorkerId.ToString();
                                    if (!string.IsNullOrEmpty(Worker.WorkerType))       Params += ";I~S~WorkerType~" +      Worker.WorkerType;
                                    if (!string.IsNullOrEmpty(Worker.WorkerState))      Params += ";I~S~WorkerState~" +     Worker.WorkerState;
                                    if (!string.IsNullOrEmpty(Worker.Gender))           Params += ";I~S~Gender~" +          Worker.Gender;
                                    if (!string.IsNullOrEmpty(Worker.Title))            Params += ";I~S~Title~" +           Worker.Title;
                                    if (!string.IsNullOrEmpty(Worker.FirstName))        Params += ";I~S~FirstName~" +       Worker.FirstName;
                                    if (!string.IsNullOrEmpty(Worker.OtherName))        Params += ";I~S~OtherName~" +       Worker.OtherName;
                                    if (!string.IsNullOrEmpty(Worker.LastName))         Params += ";I~S~LastName~" +        Worker.LastName;
                                    if (!string.IsNullOrEmpty(Worker.NickName))         Params += ";I~S~NickName~" +        Worker.NickName;
                                    if (!string.IsNullOrEmpty(Worker.MaritalState))     Params += ";I~S~MaritalState~" +    Worker.MaritalState;

                                    diag = "2";
                                    if (helpers.IsDate(Worker.BirthDate))               Params += ";I~D~BirthDate~" +       Worker.BirthDate.ToString();
                                    if (Worker.NextOfKin > 0)                           Params += ";I~S~NextOfKin~" +       Worker.NextOfKin.ToString();
                                    if (!string.IsNullOrEmpty(Worker.Occupation))       Params += ";I~S~Occupation~" +      Worker.Occupation;
                                    if (!string.IsNullOrEmpty(Worker.OccupationProp))   Params += ";I~S~OccupationProp~" +  Worker.OccupationProp;

                                    if (!string.IsNullOrEmpty(Worker.UserName))         Params += ";I~S~UserName~" +        Worker.UserName;
                                    if (!string.IsNullOrEmpty(Worker.Password))         Params += ";I~S~Password~" +        Worker.Password;
                                    if (!string.IsNullOrEmpty(Worker.CardNo))           Params += ";I~S~CardNo~" +          Worker.CardNo;
                                    if (!string.IsNullOrEmpty(Worker.PinNo))            Params += ";I~S~PinNo~" +           Worker.PinNo;

                                    diag = "3";
                                    if (Worker.WorkerStateId > 0)                       Params += ";I~S~WorkerStateId~" +   Worker.WorkerStateId.ToString();
                                    if (Worker.WorkerTypeId > 0)                        Params += ";I~S~WorkerTypeId~" +    Worker.WorkerTypeId.ToString();
                                    if (Worker.GenderId > 0)                            Params += ";I~S~GenderId~" +        Worker.GenderId.ToString();
                                    if (Worker.TitleId > 0)                             Params += ";I~S~TitleId~" +         Worker.TitleId.ToString();
                                    if (Worker.MaritalStateId > 0)                      Params += ";I~S~MaritalStateId~" +  Worker.MaritalStateId.ToString();
                                    if (Worker.PhoneId > 0)                             Params += ";I~S~PhoneId~" +         Worker.PhoneId.ToString();
                                    if (Worker.AddressId > 0)                           Params += ";I~S~AddressId~" +       Worker.AddressId.ToString();
                                    if (Worker.EMailId  > 0)                            Params += ";I~S~EMailId~" +         Worker.EMailId.ToString();
                                    if (Worker.NoticeId > 0)                            Params += ";I~S~NoticeId~" +        Worker.NoticeId.ToString();
                                    if (Worker.MessageId > 0)                           Params += ";I~S~MessageId~" +       Worker.MessageId.ToString();
                                    if (Worker.CredentialId > 0)                        Params += ";I~S~CredentialId~" +    Worker.CredentialId.ToString();
                                    if (Worker.PromotionId > 0)                         Params += ";I~S~PromotionId~" +     Worker.PromotionId.ToString();
                                    if (Worker.PointsRate > 0)                          Params += ";I~S~PointsRate~" +      Worker.PointsRate.ToString();
                                    if (helpers.IsDate(Worker.PromoExpiry))             Params += ";I~D~PromoExpiry~" +     Worker.PromoExpiry.ToString();
                                    if (Worker.ImportId > 0)                            Params += ";I~S~ImportId~" +        Worker.ImportId.ToString();
                                }

                                diag = "4";
                                helpers.WriteToLog("apiWorker:" + Params);
                                dt = db.GetDataTable("loc.api_Human_Worker", Params);
                                HasData = (dt.Rows.Count > 0);
                            }
                            if (HasData)
                            {
                                response.Status   = dt.Rows[0]["Status"].ToString();
                                response.Response = dt.Rows[0]["Response"].ToString();

                                diag = "5";
                                Worker[] Workers = new Worker[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    diag = "6" + i.ToString();
                                    Worker = new Worker()
                                    {
                                        WorkerId        = helpers.ToInt(  dt.Rows[i]["WorkerPk"].ToString()),
                                        WorkerType      =                 dt.Rows[i]["WorkerType"].ToString(),
                                        WorkerState     =                 dt.Rows[i]["WorkerState"].ToString(),
                                        CardNo          =                 dt.Rows[i]["CardNo"].ToString(),
                                        PinNo           =                 dt.Rows[i]["PinNo"].ToString(),
                                        UserName        =                 dt.Rows[i]["UserName"].ToString(),
                                        Password        =                 dt.Rows[i]["Password"].ToString(),
                                        Title           =                 dt.Rows[i]["Title"].ToString(),
                                        FirstName       =                 dt.Rows[i]["FirstName"].ToString(),
                                        OtherName       =                 dt.Rows[i]["OtherName"].ToString(),
                                        LastName        =                 dt.Rows[i]["LastName"].ToString(),
                                        NickName        =                 dt.Rows[i]["NickName"].ToString(),

                                        Gender          =                 dt.Rows[i]["Gender"].ToString(),
                                        UseNickName     = helpers.ToInt  (dt.Rows[i]["UseNickName"].ToString()),
                                        BirthDate       = helpers.ToDate (dt.Rows[i]["BirthDate"].ToString()).ToString("dd MMM, yyyy"),
                                        NextOfKin       = helpers.ToInt  (dt.Rows[i]["NextOfKinId"].ToString()),

                                        WorkerTypeId    = helpers.ToInt  (dt.Rows[i]["WorkerTypeId"].ToString()),
                                        WorkerStateId   = helpers.ToInt  (dt.Rows[i]["WorkerStateId"].ToString()),
                                        GenderId        = helpers.ToInt  (dt.Rows[i]["GenderId"].ToString()),
                                        TitleId         = helpers.ToInt  (dt.Rows[i]["TitleId"].ToString()),
                                        CredentialId    = helpers.ToInt  (dt.Rows[i]["CredentialId"].ToString())
                                    };
                                    Workers[i] = Worker;
                                }
                                response.Workers = Workers;

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
                    helpers.WriteToLog("Worker Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(response);



            ////var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            ////IConfiguration Configuration = builder.Build();
            ////using (SQLServer db = new SQLServer(Configuration.GetConnectionString("DefaultConnection").ToString()))

            //if (request != null)
            //{
            //    apiWorker     apiWorker     = new apiWorker();
            //    apiWorkerClub apiWorkerClub = new apiWorkerClub();
            //    apiClub       apiClub       = new apiClub();
            //    apiCard       apiCard       = new apiCard();
            //    apiContact    apiContact    = new apiContact();
            //    apiIdentity   apiIdentity   = new apiIdentity();
            //    apiBadge      apiBadge      = new apiBadge();

            //    // first define the Human Request
            //    WorkerRequest humReq = new WorkerRequest()
            //    {
            //        ActionType = request.ActionType,
            //        Vendor     = request.Vendor,
            //        Location   = request.Location,
            //        Worker     = request.Worker,
            //        CardNo     = request.CardNo,
            //        BadgeNo    = request.BadgeNo,
            //    };

            //    IActionResult humRes = apiWorker.Post(humReq);  // Post to Human 

            //    if (humRes is OkObjectResult okHuman)
            //    {
            //        var humData      = okHuman.Value as HumanResponse;
            //        request.Status   = humData.Status;
            //        request.Response = humData.Response;

            //        Human human = humData.Humans[0];

                   

            //        for (int i = 0; i < memReq.WorkerClubs.Length; i++)  // do the clubs first
            //        {
            //            memReq.WorkerClubs[i].WorkerId = human.HumanId;
            //            WorkerClubResponse mcRes = apiWorkerClub.getWorkerClub(humReq.ActionType, humReq.Vendor, humReq.Location, memReq.WorkerClubs[i]);
            //            process = mcRes.Status.Equals("Valid");
            //            memRes.Status = mcRes.Status;
            //            memRes.Response = mcRes.Response;
            //            if (!memRes.Status.Equals("Valid")) break;
            //        }



            //        if (humReq.ActionType == 1)
            //        {
            //            string BadgeNo = "";
            //            for (int i = 0; i < memReq.WorkerClubs.Length; i++)  // do the clubs first
            //            {
            //                if (!string.IsNullOrEmpty(memReq.WorkerClubs[i].BadgeNo))
            //                {
            //                    BadgeNo = memReq.WorkerClubs[i].BadgeNo;
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
            //                    for (int i = 0; i < memReq.WorkerClubs.Length; i++)  // do the clubs first
            //                        memReq.WorkerClubs[i].BadgeNo = BadgeNo;
            //                }
            //            }

            //        }



            //        if (process)
            //        {
            //            Human[] Humans = humData.Humans;
            //            Worker[] Workers = new Worker[Humans.Length];
            //            for (int i = 0; i < Humans.Length; i++)
            //            {
            //                Worker Worker = new Worker() { Human = Humans[i] };

            //                WorkerClub memClub = new WorkerClub() { WorkerId = Worker.Human.HumanId };
            //                WorkerClubResponse mcRes = apiWorkerClub.getWorkerClub(0, humReq.Vendor, humReq.Location, memClub);
            //                Worker.WorkerClubs = mcRes.WorkerClubs;

            //                Card card = memReq.Card; card.HumanId = Worker.Human.HumanId;
            //                CardResponse cRes = apiCard.getCard(0, humReq.Vendor, humReq.Location, card);
            //                if (cRes.Cards.Count() > 0) Worker.Card = cRes.Cards[0];

            //                Contact contact = new Contact(); contact.EntityId = Worker.Human.HumanId;
            //                ContactResponse ctRes = apiContact.getContact(0, humReq.Vendor, humReq.Location, contact);
            //                Worker.Contacts = ctRes.Contacts;

            //                Identity identity = new Identity(); identity.EntityId = Worker.Human.HumanId;
            //                IdentityResponse idRes = apiIdentity.getIdentity(0, humReq.Vendor, humReq.Location, identity);
            //                Worker.Identities = idRes.Identities;

            //                Workers[i] = Worker;
            //            }
            //            memRes.Workers = Workers;
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
        public WorkerResponse getWorkers(int ActionType, string Vendor, string Location, Worker[] Workers)
        {
            return getWorker(ActionType, Vendor, Location, Workers[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public WorkerResponse getWorker(int ActionType, string Vendor, string Location, Worker Worker)
        {
            WorkerRequest  request  = new WorkerRequest(ActionType, Vendor, Location, Worker);
            WorkerResponse response = new WorkerResponse();

            apiWorker apiWorker = new apiWorker();
            IActionResult action = apiWorker.Post(request);
            if (action is OkObjectResult ok) response = (WorkerResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (WorkerResponse)bad.Value;
            return response;
        }

    }
}
