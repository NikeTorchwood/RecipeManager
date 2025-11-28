namespace Core;

public static class Constats
{
    public static Dictionary<string, string> TableNames => new()
    {
        { nameof(Category), Categories },
        { nameof(Ingredient), Ingredients },
        { nameof(Recipe), Recipes },
        { nameof(RecipeIngredient), RecipeIngredients },
    };
    public const string Categories = "Categories";
    public const string Ingredients = "Ingredients";
    public const string Recipes = "Recipes";
    public const string RecipeIngredients = "RecipeIngredients";
}