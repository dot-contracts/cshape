using System.Data;
using System.Reflection;
using System.Security.Principal;

using nexus.common;
using nexus.common.dal;
using nexus.shared.common;

namespace nexus.shared.promo
{
    public class PromotionRequest
    {
        public int        ActionType { get; set; } = 0;
        public string     Vendor     { get; set; } = string.Empty;
        public string     Location   { get; set; } = string.Empty;
        public Promotion? Promotion { get; set; } = new Promotion();

        public PromotionRequest()
        {
            this.Vendor     = setting.Vendor;
            this.Location   = setting.Location;
            this.Promotion  = (Promotion == null ? new Promotion() : Promotion);
        }

        public PromotionRequest(int ActionType, string Vendor, string Location, Promotion Promotion = null)
        {
            this.ActionType = ActionType;
            this.Vendor     = Vendor;
            this.Location   = Location;
            this.Promotion  = (Promotion == null ? new Promotion() : Promotion);
        }
    }

    public class PromotionResponse
    {
        public string      Status     { get; set; } = string.Empty;
        public string      Response   { get; set; } = string.Empty;
        public Promotion[] Promotions { get; set; } = Array.Empty<Promotion>();
    }

    public class Promotion
    {
        public int Counter
        {
            get
            {
                if (DateTime.TryParse(Schedule?.StartsOn, out var start)) 
                    return (int)(DateTime.Now - start).TotalMinutes;
                return 0;
            }
        }

        public int?      PromotionId      { get; set; } = -1;
        public string?   PromotionType    { get; set; } = string.Empty;
        public string?   PromotionState   { get; set; } = string.Empty;
        public string?   Description      { get; set; } = string.Empty;
        public string?   Budget           { get; set; } = string.Empty;
        public int?      BudgetId         { get; set; } = -1;
        public string?   Advert           { get; set; } = string.Empty;
        public int?      AdvertId         { get; set; } = -1;
        public string?   StartPage        { get; set; } = string.Empty;
        public int?      StartPageId      { get; set; } = -1;
        public string?   Presenter        { get; set; } = string.Empty;
        public int?      PresenterId      { get; set; } = -1;
        public string?   Award            { get; set; } = string.Empty;
        public int?      AwardId          { get; set; } = -1;
        public string?   Sponsor          { get; set; } = string.Empty;
        public int?      SponsorId        { get; set; } = -1;
        public Schedule? Schedule         { get; set; }
        public string?   Trigger          { get; set; } = string.Empty;
        public string?   TriggerType      { get; set; } = string.Empty;
        public decimal?  TriggerValue     { get; set; } = -1;
        public int?      TriggerId        { get; set; } = -1;
        public string?   Action           { get; set; } = string.Empty;
        public string?   ActionType       { get; set; } = string.Empty;
        public decimal?  ActionAmount     { get; set; } = -1;
        public int?      ActionId         { get; set; } = -1;
        public int?      GameTimeout      { get; set; } = -1;
        public int?      MaxDraws         { get; set; } = -1;
        public bool?     AutoDraw         { get; set; } = false;
        public int?      DrawInterval     { get; set; } = -1;
        public int?      MaxZones         { get; set; } = -1;
        public int?      PoolId           { get; set; } = -1;
        public int?      TargetId         { get; set; } = -1;
        public int?      PromotionTypeId  { get; set; } = -1;
        public int?      PromotionStateId { get; set; } = -1;

        public bool HasData()
        {
            return (PromotionId >= 0 || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(PromotionType) || !string.IsNullOrEmpty(PromotionState) || !string.IsNullOrEmpty(PromotionType) 
            || PromotionTypeId >= 0 || PromotionStateId >= 0 );
        }

        public Promotion()
        {
            this.Schedule = (Schedule == null ? new Schedule() : Schedule);
        }
    }
}

