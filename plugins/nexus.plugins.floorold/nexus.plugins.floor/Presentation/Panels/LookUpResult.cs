using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nexus.plugins.floor
{
    public class LookupResult
    {
        public string BadgeNo    { get; set; }
        public string Player     { get; set; }
        public string GameName   { get; set; }
        public string Tier       { get; set; }
        public string PlayTime   { get; set; }
        public string AverageBet { get; set; }
    }
}
