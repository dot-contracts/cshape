
using nexus.shared.common;

namespace nexus.shared.media
{
    public class FeedItemRequest
    {
        public int       ActionType { get; set; } = 0;
        public string    Vendor     { get; set; } = string.Empty;
        public string    Location   { get; set; } = string.Empty;
        public FeedItem? FeedItem   { get; set; } = new FeedItem();
    }

    public class FeedItemResponse
    {
        public string     Status    { get; set; } = string.Empty;
        public string     Response  { get; set; } = string.Empty;
        public FeedItem[] FeedItems { get; set; } = Array.Empty<FeedItem>();
    }

    public class FeedItem
    {

        public string?  MediaType       { get; set; } = string.Empty;
        public string?  Description     { get; set; } = string.Empty;
        public string?  FeedType        { get; set; } = string.Empty;
        public string?  FeedState       { get; set; } = string.Empty;
        public string?  FeedItemType    { get; set; } = string.Empty;
        public string?  FeedItemState   { get; set; } = string.Empty;
        public int?     Duration        { get; set; }  = -1;
        public string?  FileName        { get; set; } = string.Empty;
        public string?  FileDate        { get; set; } = string.Empty;
        public int?     FileSize        { get; set; } = -1;

        public string?  ImageData       { get; set; } = string.Empty;
        public string?  ThumbData       { get; set; } = string.Empty;
        public string?  Scroller        { get; set; } = string.Empty;
        public int?     ImageWidth      { get; set; } = -1;
        public int?     ImageHeight     { get; set; } = -1;

        public Schedule Schedule        { get; set; } = new Schedule();

        public int?     FeedId          { get; set; } = -1;
        public int?     FeedItemId      { get; set; } = -1;
        public int?     ParentId        { get; set; } = -1;

        public int?     MediaTypeId     { get; set; } = -1;
        public int?     FeedTypeId      { get; set; } = -1;
        public int?     FeedStateId     { get; set; } = -1;
        public int?     FeedItemTypeId  { get; set; } = -1;
        public int?     FeedItemStateId { get; set; } = -1;



        public bool HasData()
        {
            return (FeedId > 0 || FeedItemId > 0 || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(FeedItemType) || !string.IsNullOrEmpty(FeedItemState) || FeedItemTypeId >= 0 || FeedItemStateId >=0 );
        }
    }
}

