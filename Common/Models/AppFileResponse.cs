namespace Common.Models
{
    public class AppFileResponse
    {
        public required string Id { get; set; }
        public required string OwnerId { get; set; }
        public required string Category { get; set; }
        public required string Type { get; set; }
        public required string Url { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
