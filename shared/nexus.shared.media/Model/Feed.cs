
using nexus.shared.common;

namespace nexus.shared.media
{
    public class FeedRequest
    {
        public int     ActionType { get; set; } = 0;
        public string  Vendor     { get; set; } = string.Empty;
        public string  Location   { get; set; } = string.Empty;
        public Feed?   Feed       { get; set; } = new Feed();
    }

    public class FeedResponse
    {
        public string   Status   { get; set; } = string.Empty;
        public string   Response { get; set; } = string.Empty;
        public Feed[]   Feeds    { get; set; } = Array.Empty<Feed>();
    }

    public class Feed
    {
        public string?  Description   { get; set; } = string.Empty;
        public int?     FeedId        { get; set; } = -1;
        public string?  FeedType      { get; set; } = string.Empty;
        public string?  FeedState     { get; set; } = string.Empty;
        public Schedule Schedule      { get; set; } = new Schedule();    

        public int?     FeedTypeId    { get; set; } = -1;
        public int?     FeedStateId   { get; set; } = -1;

        public bool HasData()
        {
            return (FeedId > 0 || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(FeedType) || !string.IsNullOrEmpty(FeedState) || FeedTypeId >= 0 || FeedStateId >=0 );
        }
    }
}

