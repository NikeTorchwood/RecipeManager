using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Ingredients;

public class IngredientDto : IFullDto<Ingredient>, IShortDto<Ingredient>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}