//using nexus.api.Model.Local;

namespace nexus.shared.gaming
{
    public class GamingRequest
    {
        public int      HumanId      { get; set; } = 0;
    }

    public class GamingResponse
    {
        public string   Status       { get; set; } = String.Empty;
        public string   Response     { get; set; } = String.Empty;
        public Gamings  Gaming       { get; set; } = new Gamings();

        public GamingResponse()
        {
            Gaming = new Gamings();
        }

    }

    public class Gamings
    {
        public decimal?  HotPlay      { get; set; } = -1;
        public decimal?  Turnover     { get; set; } = -1;
        public decimal?  TotalWin     { get; set; } = -1;
        public decimal?  TransferOn   { get; set; } = -1;
        public decimal?  Stroke       { get; set; } = -1;
        public string?   LastPlay     { get; set; } = String.Empty;
        public string?   LastPrized   { get; set; } = String.Empty;
        public int?      LastPrize    { get; set; } = -1;

        public bool HasData()
        {
            return (!this.Equals(new Gamings()));
        }

    }
}

