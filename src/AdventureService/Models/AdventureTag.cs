namespace AdventureService.Models
{
    public class AdventureTag
    {
        public string AdventureId { get; set; }
        public Adventure Adventure { get; set; }
        public string TagName { get; set; }
        public Tag Tag { get; set; }
    }
}