using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Ingredients;

public class IngredientCreateDto : ICreateDto<Ingredient>
{
    public string Name { get; set; }
}