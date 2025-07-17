using System.Drawing;

namespace nexus.api.local.Model
{
    public class CohortRequest
    {
        public int     ActionType { get; set; } = 0;
        public string  Vendor     { get; set; } = string.Empty;
        public string  Location   { get; set; } = string.Empty;
        public Cohort? Cohort     { get; set; } = new Cohort();
    }

    public class CohortResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public Cohort[] Cohorts  { get; set; } = Array.Empty<Cohort>();
    }

    public class Cohort
    {
        public int?     CohortId      { get; set; } = -1;
        public string?  UseageType    { get; set; } = string.Empty;
        public string?  CohortType    { get; set; } = string.Empty;
        public string?  CohortState   { get; set; } = string.Empty;
        public int?     Start         { get; set; } = -1;
        public int?     Finish        { get; set; } = -1;
        public int?     Level         { get; set; } = -1;
        public decimal? Value1        { get; set; } = -1;
        public decimal? Value2        { get; set; } = -1;
        public decimal? Value3        { get; set; } = -1;
        public decimal ?Value4        { get; set; } = -1;
        public int?     Refresh       { get; set; } = -1;
        public int?     PromotionId   { get; set; } = -1;
        public int?     UseageTypeId  { get; set; } = -1;
        public int?     CohortTypeId  { get; set; } = -1;
        public int?     CohortStateId { get; set; } = -1;
        public int?     InterfaceType { get; set; } = -1;

        public bool HasData()
        {
            return (CohortId > 0 || !string.IsNullOrEmpty(UseageType) || !string.IsNullOrEmpty(CohortType) || !string.IsNullOrEmpty(CohortState) || Start >= 0 || Finish >= 0 || Level >= 0 || Value1 >= 0 || Value2 >= 0 || Value3 >= 0 || Value4 >= 0 ||
            PromotionId >= 0 || UseageTypeId >= 0 || CohortTypeId >= 0 || CohortStateId >=0 || InterfaceType >= 0);
        }
    }
}

