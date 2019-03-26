using System.Drawing;
using AdventureService.Models;

namespace AdventureService.ViewModels
{
    public class AdventureCreateRequest
    {
        public string MainPhotoUrl { get; set; }
        public string[] PhotosUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PointViewModel Location { get; set; }
        public Level Level { get; set; }
        public CategoryReference Category { get; set; }
        public string[] Tags { get; set; }
        public int Rating { get; set; }
    }

    public class AdventureUpdateRequest
    {
        public string Id { get; set; }
        public string MainPhotoUrl { get; set; }
        public string[] PhotosUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PointViewModel Location { get; set; }
        public Level Level { get; set; }
        public CategoryReference Category { get; set; }
        public string[] Tags { get; set; }
        public int Rating { get; set; }
    }

    public class AdventureResponse
    {
        public string Id { get; set; }
        public string MainPhotoUrl { get; set; }
        public string[] PhotosUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public PointViewModel Location { get; set; }
        public Level Level { get; set; }
        public CategoryResponse Category { get; set; }
        public string[] Tags { get; set; }
        public int Rating { get; set; }
    }

    public class CategoryReference
    {
        public string Id { get; set; }
    }
}