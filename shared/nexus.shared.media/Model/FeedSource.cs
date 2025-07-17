
using nexus.shared.common;

namespace nexus.shared.media
{
    public class FeedSourceRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public FeedSource? FeedSource   { get; set; } = new FeedSource();
    }

    public class FeedSourceResponse
    {
        public string     Status    { get; set; } = string.Empty;
        public string     Response  { get; set; } = string.Empty;
        public FeedSource[] FeedSources { get; set; } = Array.Empty<FeedSource>();
    }

    public class FeedSource
    {

        public string?  FeedSourceType    { get; set; } = string.Empty;
        public string?  FeedSourceState   { get; set; } = string.Empty;
        public string?  LocalSource       { get; set; } = string.Empty;
        public string?  RemoteSource      { get; set; } = string.Empty;
        public bool?    CopyLocal         { get; set; } = true;
        public Schedule Schedule          { get; set; } = new Schedule();
        public int?     FeedSourceId      { get; set; } = -1;
        public int?     FeedSourceTypeId  { get; set; } = -1;
        public int?     FeedSourceStateId { get; set; } = -1;


        public bool HasData()
        {
            return (FeedSourceId > 0 || !string.IsNullOrEmpty(FeedSourceType) || !string.IsNullOrEmpty(FeedSourceState) || FeedSourceTypeId >= 0 || FeedSourceStateId >=0 );
        }
    }
}

