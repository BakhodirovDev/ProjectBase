using AutoMapper;

namespace Application.Mappers;

public abstract class BaseMappingProfile : Profile
{
    protected void CreateBidirectionalMap<TSource, TDestination>()
    {
        CreateMap<TSource, TDestination>();
        CreateMap<TDestination, TSource>();
    }

    protected void CreateOneWayMap<TSource, TDestination>()
    {
        CreateMap<TSource, TDestination>();
    }
}