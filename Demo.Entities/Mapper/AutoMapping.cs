using AutoMapper;
using Demo.Entities.Entities;
using Demo.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.Mapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<ProductDetailsViewModel, ProductDetails>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName)).ReverseMap()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName));
        }
    }
}
