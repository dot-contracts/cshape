using System.Security.Principal;

namespace nexus.shared.gaming
{
    public class PlayerRequest
    {
        public int    ActionType { get; set; } = 0;
        public string Vendor     { get; set; } = string.Empty;
        public string Location   { get; set; } = string.Empty;
        public string Search     { get; set; } = string.Empty;
        public string Value      { get; set; } = string.Empty;
        public int    LimitRows  { get; set; } = 0;
    }

    public class PlayerResponse
    {
        public string   Status    { get; set; } = string.Empty;
        public string   Response  { get; set; } = string.Empty;
        public Player[] Results   { get; set; } = Array.Empty<Player>();
    }

    public class Player
    {
        public string?   Name         { get; set; } = String.Empty;
        public string?   Tier         { get; set; } = String.Empty;
        public double?   Turnover     { get; set; } = 0; 
        public double?   AvgBet       { get; set; } = 0; 
        public string?   Duration     { get; set; } = String.Empty;
        public double?   Spent        { get; set; } = 0; 
        public double?   Earned       { get; set; } = 0; 
        public string?   LastPlayedAt { get; set; } = String.Empty;
        public int?      HumanId      { get; set; } = 0; 
        public int?      PlayTime     { get; set; } = 0; 

        public bool HasData()
        {
            return (true);
        }
    }
}

