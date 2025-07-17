

using System.Data;
using System.Security.Cryptography;

using nexus.common;

namespace nexus.shared.local
{

    static public class HumanHelpers
    {

        public static string GetParams(Human human)
        {
            string Params = string.Empty;
            if (human.HasData())
            {
                if (!string.IsNullOrEmpty(human.PatronType))    Params += ";I~S~PatronType~" +   human.PatronType;
                if (!string.IsNullOrEmpty(human.PatronState))   Params += ";I~S~PatronState~" +  human.PatronState;
                if (!string.IsNullOrEmpty(human.WorkerType))    Params += ";I~S~WorkerType~" +   human.WorkerType;
                if (!string.IsNullOrEmpty(human.WorkerState))   Params += ";I~S~WorkerState~" +  human.WorkerState;
                if (!string.IsNullOrEmpty(human.Gender))        Params += ";I~S~Gender~" +       human.Gender;
                if (!string.IsNullOrEmpty(human.Title))         Params += ";I~S~Title~" +        human.Title;
                if (!string.IsNullOrEmpty(human.FirstName))     Params += ";I~S~FirstName~" +    human.FirstName;
                if (!string.IsNullOrEmpty(human.OtherName))     Params += ";I~S~OtherName~" +    human.OtherName;
                if (!string.IsNullOrEmpty(human.LastName))      Params += ";I~S~LastName~" +     human.LastName;
                if (!string.IsNullOrEmpty(human.NickName))      Params += ";I~S~NickName~" +     human.NickName;
                if (!string.IsNullOrEmpty(human.MaritalState))  Params += ";I~S~MaritalState~" + human.MaritalState;
                if (helpers.IsDate(human.BirthDate))            Params += ";I~S~BirthDate~" +    human.BirthDate.ToString();
                if (human.NextOfKin > 0)                        Params += ";I~S~NextOfKin~" +    human.NextOfKin.ToString();
            }

            return Params;
        }

        public static Human LoadFromRow(DataRow row)
        {

            Human human = new Human()
            {
                PatronType    =                 row["PatronType"].ToString(),
                PatronState   =                 row["PatronState"].ToString(),
                WorkerType    =                 row["WorkerType"].ToString(),
                WorkerState   =                 row["WorkerState"].ToString(),
                Gender        =                 row["Gender"].ToString(),
                Title         =                 row["Title"].ToString(),
                FirstName     =                 row["FirstName"].ToString(),
                OtherName     =                 row["OtherName"].ToString(),
                LastName      =                 row["LastName"].ToString(),
                NickName      =                 row["NickName"].ToString(),
                MaritalState  =                 row["MaritalState"].ToString(),
                BirthDate     = helpers.ToDate (row["BirthDate"].ToString()).ToString("dd MMM, yyyy"),
                NextOfKin     = helpers.ToInt  (row["NextOfKinId"].ToString())
            };
            return human;
        }
    }

    public class HumanRequest
    {
        public int     ActionType { get; set; } = 0;
        public string  Vendor     { get; set; } = string.Empty;
        public string  Location   { get; set; } = string.Empty;
        public string? BadgeNo    { get; set; }
        public string? CardNo     { get; set; }
        public Human?  Human      { get; set; } = new Human();
    }

    public class HumanResponse
    {
        public string  Status   { get; set; } = string.Empty;
        public string  Response { get; set; } = string.Empty;
        public Human[] Humans   { get; set; } = Array.Empty<Human>();
    }

    public class Human
    {
        public int?      HumanId        { get; set; } = -1;
        public string?   PatronType     { get; set; } = string.Empty;
        public string?   PatronState    { get; set; } = string.Empty;
        public string?   WorkerType     { get; set; } = string.Empty;
        public string?   WorkerState    { get; set; } = string.Empty;
        public string?   Gender         { get; set; } = string.Empty;
        public string?   Title          { get; set; } = string.Empty;
        public string?   FirstName      { get; set; } = string.Empty;
        public string?   OtherName      { get; set; } = string.Empty;
        public string?   LastName       { get; set; } = string.Empty;
        public string?   NickName       { get; set; } = string.Empty;
        public int?      UseNickName    { get; set; } = -1;
        public string?   BirthDate      { get; set; } = string.Empty;
        public int?      NextOfKin      { get; set; } = -1;
        public string?   MaritalState   { get; set; } = string.Empty;
        public int?      PatronTypeId   { get; set; } = -1;
        public int?      PatronStateId  { get; set; } = -1;
        public int?      WorkerTypeId   { get; set; } = -1;
        public int?      WorkerStateId  { get; set; } = -1;
        public int?      GenderId       { get; set; } = -1;
        public int?      TitleId        { get; set; } = -1;
        public int?      MaritalStateId { get; set; } = -1;

        public bool HasData()
        {
            return (HumanId > 0
            || !string.IsNullOrEmpty(PatronType)
            || !string.IsNullOrEmpty(PatronState)
            || !string.IsNullOrEmpty(WorkerType)
            || !string.IsNullOrEmpty(WorkerState)
            || !string.IsNullOrEmpty(Gender)
            || !string.IsNullOrEmpty(Title)
            || !string.IsNullOrEmpty(FirstName)
            || !string.IsNullOrEmpty(OtherName)
            || !string.IsNullOrEmpty(LastName)
            || !string.IsNullOrEmpty(NickName)
            || !string.IsNullOrEmpty(MaritalState)
            || helpers.IsDate(BirthDate)
            || UseNickName >= 0
            || NextOfKin >= 0
            || PatronTypeId >= 0
            || PatronStateId >= 0
            || WorkerTypeId >= 0
            || WorkerStateId >= 0
            || GenderId >= 0
            || TitleId >= 0
            || MaritalStateId >= 0);
        }
    }
}

