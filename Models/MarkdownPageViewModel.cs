namespace Headstrong.Models
{
    public class MarkdownPageViewModel
    {
        public string Name { get; set; }
        public bool Directory { get; set; }
        public int Id { get; set; }
        public int? ParentId { get; set; }
    }
}
