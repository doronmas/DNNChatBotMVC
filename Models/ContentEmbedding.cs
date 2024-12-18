namespace DNNChatBotMVC.Models
{
    public class ContentEmbedding
    {
        public int Id { get; set; }
        public required string SourceType { get; set; }
        public int? SourceId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public required byte[] Embedding { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Metadata { get; set; }
        public string? TabPath { get; set; }
        public string? Url { get; set; }
    }
}
