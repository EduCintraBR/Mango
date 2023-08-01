using AutoMapper;
using Mango.Services.OrderAPI.Dto;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderDetailsDto, OrderDetails>()
                .ReverseMap();
                config.CreateMap<OrderHeaderDto, OrderHeader>()
                .ReverseMap();

                config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal))
                .ReverseMap();
            });

            return mappingConfig;
        }
    }
}
