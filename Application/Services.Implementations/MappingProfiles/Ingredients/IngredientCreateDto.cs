using Application.Contracts.Abstractions;
using Core;

namespace Application.Services.Implementations.MappingProfiles.Ingredients;

public class IngredientCreateDto : ICreateDto<Ingredient>
{
    public Guid IngredientId { get; set; }
    public string Quantity { get; set; }
}