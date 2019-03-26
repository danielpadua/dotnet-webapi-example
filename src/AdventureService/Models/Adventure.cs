using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace AdventureService.Models
{
    public class Adventure
    {
        public string Id { get; set; }
        public string MainPhotoUrl { get; set; }
        public string[] PhotosUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Point Location { get; set; }
        public Level Level { get; set; }
        public Category Category { get; set; }
        public ICollection<AdventureTag> AdventureTags { get; set; }
        public int Rating { get; set; }
    }
}