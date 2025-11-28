using Application.Contracts.Abstractions;
using Core;

namespace Application.Contracts.Ingredients;

public class IngredientCreateDto : ICreateDto<Ingredient>
{
    public Guid IngredientId { get; set; }
    public string Quantity { get; set; }
}