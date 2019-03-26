namespace AdventureService.ViewModels
{
    public class CategoryCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class CategoryUpdateRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class CategoryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}