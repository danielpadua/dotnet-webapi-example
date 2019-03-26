using System.Collections.Generic;

namespace AdventureService.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public ICollection<AdventureTag> AdventureTags { get; set; }
    }
}