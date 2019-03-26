using System.Collections.Generic;

namespace AdventureService.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Adventure> Adventures { get; set; }
    }
}