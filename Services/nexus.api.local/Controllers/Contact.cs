
using System.Data;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using nexus.common;
using nexus.common.dal;
using nexus.shared.local;

namespace nexus.api.local
{
    [Route("loc")]
    public class apiContact : ControllerBase
    {
        [HttpPost("Contact")]
        [Authorize]
        public IActionResult Post([FromBody] ContactRequest request)
        {
            ContactResponse conRes = new ContactResponse()
            {
                Status   = "Bad Request",
                Response = "Poorly formatted"
            };

            if (request != null)
            {
                string diag = "";
                try
                {
                    using (SQLServer db = new SQLServer(setting.ConnectionString))
                    {

                        string Params = "I~S~ActionType~" + request.ActionType;
                        Params       += ";I~S~Vendor~" +    request.Vendor;
                        Params       += ";I~S~Location~" +  request.Location;

                        if (request.Contact != null)
                        {
                            Contact contact = request.Contact;

                            if (contact.HasData())
                            {
                                diag = "1";
                                switch (contact.ContactType) 
                                {
                                    case "Street Address":   contact.ContactType = "Street";   break;
                                    case "Postal Address":   contact.ContactType = "Postal";   break;
                                    case "Work Address":     contact.ContactType = "Work";     break;
                                    case "Retreat Address":  contact.ContactType = "Retreat";  break;
                                    case "Other Address":    contact.ContactType = "Other";    break;
                                    case "Home Phone":       contact.ContactType = "Home";     break;
                                    case "Mobile Phone":     contact.ContactType = "Mobile";   break;
                                    case "Business Phone":   contact.ContactType = "Business"; break;
                                    case "Facsimile Phone":  contact.ContactType = "FAX";      break;
                                }

                                diag = "2";
                                if (string.IsNullOrEmpty(contact.Detail))
                                {
                                    switch (contact.ContactType) 
                                    {
                                        case "Street": case "Postal": case "Work":  case "Retreat": case "Other":
                                            if (!string.IsNullOrEmpty(contact.StreetName + contact.SuburbName + contact.PostCode + contact.ProvinceName + contact.PostCode))
                                                contact.Detail = contact.StreetName + "_" + contact.SuburbName + "_" + contact.PostCode + "_" + contact.ProvinceName + "_" + contact.PostCode;
                                            break;

                                        case "Home": case "Mobile": case "Business": case "FAX":
                                            if (!string.IsNullOrEmpty(contact.Contact1))
                                                contact.Detail = contact.Contact1;
                                            break;
                                    }
                                }

                                diag = "3";
                                if (contact.ContactId > 0)                       Params += ";I~S~ContactId~" +    contact.ContactId.ToString();
                                if (contact.EntityId > 0)                        Params += ";I~S~EntityId~" +     contact.EntityId.ToString();
                                if (!string.IsNullOrEmpty(contact.ContactType))  Params += ";I~S~ContactType~" +  contact.ContactType;
                                if (!string.IsNullOrEmpty(contact.ContactState)) Params += ";I~S~ContactState~" + contact.ContactState;
                                if (!string.IsNullOrEmpty(contact.ContactLocation)) Params += ";I~S~ContactLocation~" +  contact.ContactLocation;
                                if (!string.IsNullOrEmpty(contact.Detail))       Params += ";I~S~Detail~" +       contact.Detail;
                                if (!string.IsNullOrEmpty(contact.Contact1))     Params += ";I~S~Contact1~" +     contact.Contact1;
                                if (!string.IsNullOrEmpty(contact.Contact2))     Params += ";I~S~Contact2~" +     contact.Contact2;
                                if (!string.IsNullOrEmpty(contact.Contact3))     Params += ";I~S~Contact3~" +     contact.Contact3;
                                if (contact.StreetId > 0)                        Params += ";I~S~StreetId~" +     contact.StreetId.ToString();
                                if (!string.IsNullOrEmpty(contact.StreetName))   Params += ";I~S~StreetName~" +   contact.StreetName;
                                if (contact.SuburbId > 0)                        Params += ";I~S~SuburbId~" +     contact.SuburbId.ToString();
                                if (!string.IsNullOrEmpty(contact.SuburbName))   Params += ";I~S~SuburbName~" +   contact.SuburbName;
                                if (!string.IsNullOrEmpty(contact.PostCode))     Params += ";I~S~PostCode~" +     contact.PostCode;
                                if (!string.IsNullOrEmpty(contact.ProvinceName)) Params += ";I~S~ProvinceName~" + contact.ProvinceName;
                                if (!string.IsNullOrEmpty(contact.CountryName))  Params += ";I~S~CountryName~" +  contact.CountryName;
                                if (contact.Default >= 0)                        Params += ";I~S~Default~" +      contact.Default.ToString();
                                diag = "4";
                            }
                        }

                        diag = "5";
                        helpers.WriteToLog("apiContact:" + Params);
                        DataTable dt = db.GetDataTable("loc.api_Contact", Params);
                        if (dt.Rows.Count > 0)
                        {
                            conRes.Status   = dt.Rows[0]["Status"].ToString();
                            conRes.Response = dt.Rows[0]["Response"].ToString();

                            diag = "6";
                            Contact[] Contacts = new Contact[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                diag = "7" + i.ToString();
                                Contact contact = new Contact()
                                {
                                    ContactId    = helpers.ToInt (dt.Rows[i]["ContactId"].ToString()),
                                    EntityId     = helpers.ToInt (dt.Rows[i]["EntityId"].ToString()),
                                    ContactType  =                dt.Rows[i]["ContactType"].ToString(),
                                    ContactState =                dt.Rows[i]["ContactState"].ToString(),
                                    ContactLocation =             dt.Rows[i]["ContactLocation"].ToString(),
                                    Detail       =                dt.Rows[i]["Detail"].ToString(),
                                    Contact1     =                dt.Rows[i]["Contact1"].ToString(),
                                    Contact2     =                dt.Rows[i]["Contact2"].ToString(),
                                    Contact3     =                dt.Rows[i]["Contact3"].ToString(),
                                    StreetId     = helpers.ToInt (dt.Rows[i]["StreetId"].ToString()),
                                    StreetName   =                dt.Rows[i]["StreetName"].ToString(),
                                    SuburbId     = helpers.ToInt (dt.Rows[i]["SuburbId"].ToString()),
                                    SuburbName   =                dt.Rows[i]["SuburbName"].ToString(),
                                    PostCode     =                dt.Rows[i]["PostCode"].ToString(),
                                    ProvinceName =                dt.Rows[i]["ProvinceName"].ToString(),
                                    CountryName  =                dt.Rows[i]["CountryName"].ToString(),
                                    Default      = helpers.ToInt (dt.Rows[i]["Default"].ToString())
                                };
                                Contacts[i] = contact;
                            }
                            conRes.Contacts = Contacts;

                            if (conRes.Status.Equals("Valid")) return Ok(conRes);
                        }
                        else
                        {
                            conRes.Status   = "Error";
                            conRes.Response = "No Data Found, " + db.DataError;
                        }
                    }

                }
                catch (Exception ex)
                {
                    helpers.WriteToLog("Contact Err:" + diag + ";" + ex.Message);
                }
            }

            return BadRequest(conRes);

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public ContactResponse getContacts(int ActionType, string Vendor, string Location, Contact[] Contacts)
        {
            return getContact(ActionType, Vendor, Location, Contacts[0]);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public ContactResponse getContact(int ActionType, string Vendor, string Location, Contact Contact)
        {
            ContactRequest  request  = new ContactRequest(ActionType, Vendor, Location, Contact);
            ContactResponse response = new ContactResponse();

            apiContact api = new apiContact();
            IActionResult action = api.Post(request);
            if (action is OkObjectResult ok) response = (ContactResponse)ok.Value;
            else if (action is BadRequestObjectResult bad) response = (ContactResponse)bad.Value;
            return response;
        }

    }
}
