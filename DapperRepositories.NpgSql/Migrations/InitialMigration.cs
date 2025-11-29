using System.Data;
using Core;
using FluentMigrator;

namespace DapperRepositories.NpgSql.Migrations;

[Migration(202511291409)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");

        Create.Table(Constats.Categories)
            .WithColumn(nameof(Category.Id)).AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn(nameof(Category.Name)).AsString(255).NotNullable();

        Create.Table(Constats.Ingredients)
            .WithColumn(nameof(Ingredient.Id)).AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn(nameof(Ingredient.Name)).AsString(255).NotNullable();

        Create.Table(Constats.Recipes)
            .WithColumn(nameof(Recipe.Id)).AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn(nameof(Recipe.Name)).AsString(255).NotNullable()
            .WithColumn(nameof(Recipe.Description)).AsString(4000).Nullable()
            .WithColumn(nameof(Recipe.CreatedAt)).AsDateTimeOffset().NotNullable()
            .WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn(nameof(Recipe.CategoryId)).AsGuid().NotNullable()
            .ForeignKey("FK_Recipes_Categories", Constats.Categories, nameof(Category.Id))
            .OnDelete(Rule.Cascade);

        Create.Table(Constats.RecipeIngredients)
            .WithColumn(nameof(RecipeIngredient.Id)).AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn(nameof(RecipeIngredient.Quantity)).AsString(100).NotNullable()
            .WithColumn(nameof(RecipeIngredient.RecipeId)).AsGuid().NotNullable()
            .ForeignKey("FK_RecipeIngredients_Recipes", Constats.Recipes, nameof(Recipe.Id))
            .OnDelete(Rule.Cascade)
            .WithColumn(nameof(RecipeIngredient.IngredientId)).AsGuid().NotNullable()
            .ForeignKey("FK_RecipeIngredients_Ingredients", Constats.Ingredients, nameof(Ingredient.Id))
            .OnDelete(Rule.Cascade);

        Create.Index("IX_Recipes_CategoryId")
            .OnTable(Constats.Recipes).OnColumn("CategoryId");

        Create.Index("IX_RecipeIngredients_RecipeId")
            .OnTable(Constats.RecipeIngredients).OnColumn("RecipeId");
    }

    public override void Down()
    {
        Delete.Table(Constats.RecipeIngredients);
        Delete.Table(Constats.Recipes);
        Delete.Table(Constats.Ingredients);
        Delete.Table(Constats.Categories);

        Execute.Sql("DROP EXTENSION IF EXISTS \"uuid-ossp\";");
    }
}