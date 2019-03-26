using System.Collections.Generic;
using System.Linq;
using AdventureService.Models;
using AdventureService.ViewModels;
using AutoMapper;
using NetTopologySuite.Geometries;

namespace AdventureService.Automapper
{
    public class AdventureMapProfile : Profile
    {
        public AdventureMapProfile()
        {
            CreateMap<Category, CategoryCreateRequest>().ReverseMap();
            CreateMap<Category, CategoryUpdateRequest>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();

            CreateMap<Point, PointViewModel>().ReverseMap();

            CreateMap<string[], ICollection<AdventureTag>>().ConvertUsing<AdventureTagConverter>();
            CreateMap<ICollection<AdventureTag>, string[]>().ConvertUsing<AdventureTagConverterReverse>();

            CreateMap<Adventure, AdventureCreateRequest>()
                .ReverseMap()
                    .ForMember(x => x.Category, opt => opt.Ignore())
                    .ForMember(x => x.AdventureTags, opt => opt.MapFrom(y => y.Tags));

            CreateMap<Adventure, AdventureUpdateRequest>()
                .ReverseMap()
                    .ForMember(x => x.Category, opt => opt.Ignore())
                    .ForMember(x => x.AdventureTags, opt => opt.MapFrom(y => y.Tags));

            CreateMap<Adventure, AdventureResponse>()
                .ForMember(x => x.Tags, opt => opt.MapFrom(y => y.AdventureTags)).ReverseMap();
        }
    }
}