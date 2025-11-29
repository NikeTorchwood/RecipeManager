using Application.Contracts.Ingredients;
using AutoMapper;
using Core;

namespace Application.Services.Implementations.MappingProfiles;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<IngredientCreateDto, Ingredient>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src=>src.Name));

        CreateMap<Ingredient, IngredientDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}
