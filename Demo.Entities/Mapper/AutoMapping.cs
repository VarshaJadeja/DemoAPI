using AutoMapper;
using Demo.Entities.Entities;
using Demo.Entities.ViewModels;

namespace Demo.Entities.Mapper;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<ProductDetailsViewModel, ProductDetails>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName)).ReverseMap()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName));
    }
}
