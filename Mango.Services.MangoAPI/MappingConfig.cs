using AutoMapper;
using Mango.Services.MangoAPI.Models;
using Mango.Services.MangoAPI.Models.Dto;

namespace Mango.Services.MangoAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();
            });

            return mappingConfig;
        }
    }
}
