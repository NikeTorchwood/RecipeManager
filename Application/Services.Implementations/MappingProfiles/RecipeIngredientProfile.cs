namespace Application.Services.Implementations.MappingProfiles;
//
// public class RecipeIngredientProfile : Profile
// {
//     public RecipeIngredientProfile()
//     {
//         CreateMap<RecipeIngredientCreateDto, RecipeIngredient>()
//             .ForMember(dest=>dest.IngredientId, opt=>opt.MapFrom(src=>src.IngredientId))
//             .ForMember(dest=>dest.RecipeId, opt=>opt.MapFrom(src=>src.))
//             .ForMember(dest=>dest.Quantity, opt=>opt.MapFrom(src=>src.Quantity));
//         
//         CreateMap<RecipeIngredient, RecipeIngredientDto>()
//             .ForMember(dest=>dest.Id, opt=>opt.MapFrom(src=>src.Id))
//             .ForMember(dest=>dest.IngredientId, opt=>opt.MapFrom(src=>src.IngredientId))
//             .ForMember(dest=>dest.RecipeId, opt=>opt.MapFrom(src=>src.RecipeId))
//             .ForMember(dest=>dest.Quantity, opt=>opt.MapFrom(src=>src.Quantity));
//     }
// }