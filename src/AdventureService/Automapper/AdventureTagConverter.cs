using System.Collections.Generic;
using AdventureService.Models;
using AutoMapper;

namespace AdventureService.Automapper
{
    public class AdventureTagConverter : ITypeConverter<string[], ICollection<AdventureTag>>
    {
        public ICollection<AdventureTag> Convert(string[] source, ICollection<AdventureTag> destination, ResolutionContext context)
        {
            var adventureTags = new List<AdventureTag>();
            foreach (var tagName in source)
            {
                var adventureTag = new AdventureTag()
                {
                    Tag = new Tag()
                    {
                        Name = tagName
                    },
                    TagName = tagName
                };
                adventureTags.Add(adventureTag);
            }
            return adventureTags;
        }
    }

    public class AdventureTagConverterReverse : ITypeConverter<ICollection<AdventureTag>, string[]>
    {
        public string[] Convert(ICollection<AdventureTag> source, string[] destination, ResolutionContext context)
        {
            var tags = new List<string>();
            foreach (var tag in source)
            {
                tags.Add(tag.Tag.Name);
            }
            return tags.ToArray();
        }
    }
}