using Application.Contracts.Recipes;
using Application.Exceptions;
using Application.Repositories.Abstractions;
using Application.Services.Implementations;
using AutoMapper;
using Core;
using FluentAssertions;
using Moq;

namespace RecipeManager.Tests;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _recipeRepoMock;
    private readonly Mock<IIngredientRepository> _ingredientRepoMock;
    private readonly Mock<IRecipeIngredientRepository> _recipeIngredientRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    
    private readonly RecipeService _service;

    public RecipeServiceTests()
    {
        _recipeRepoMock = new Mock<IRecipeRepository>();
        _ingredientRepoMock = new Mock<IIngredientRepository>();
        _recipeIngredientRepoMock = new Mock<IRecipeIngredientRepository>();
        _mapperMock = new Mock<IMapper>();
        
        _service = new RecipeService(
            _recipeRepoMock.Object,
            _ingredientRepoMock.Object,
            _recipeIngredientRepoMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_Should_CreateRecipe_And_AddIngredients_When_Valid()
    {
        // Arrange (Подготовка)
        var recipeId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        
        var dto = new RecipeCreateDto
        {
            Name = "Borsch",
            CategoryId = Guid.NewGuid(),
            Ingredients =
            [
                new RecipeIngredientCreateDto { IngredientId = ingredientId, Quantity = "500g" }
            ]
        };
        
        _mapperMock.Setup(m => m.Map<Recipe>(dto)).Returns(new Recipe { Name = "Borsch" });

        _ingredientRepoMock.Setup(r => r.ExistingIdsAsync(It.IsAny<Guid[]>(), CancellationToken.None))
            .ReturnsAsync([ingredientId]);

        _recipeRepoMock.Setup(r => r.AddAsync(It.IsAny<Recipe>(), CancellationToken.None))
            .ReturnsAsync(recipeId);

        var result = await _service.CreateAsync(dto);

        result.Should().Be(recipeId);
        
        _recipeRepoMock.Verify(r => r.AddAsync(It.IsAny<Recipe>(), CancellationToken.None), Times.Once);
        
        _recipeIngredientRepoMock.Verify(r => r.AddRangeAsync(
            It.Is<IEnumerable<RecipeIngredient>>(list => 
                list.Count() == 1 && 
                list.First().IngredientId == ingredientId &&
                list.First().RecipeId == recipeId
            ), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_ThrowBadRequest_When_Ingredient_NotFound()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        var dto = new RecipeCreateDto
        {
            Name = "Bad Soup",
            Ingredients = [new RecipeIngredientCreateDto { IngredientId = missingId }]
        };

        _ingredientRepoMock.Setup(r => r.ExistingIdsAsync(It.IsAny<Guid[]>(), CancellationToken.None))
            .ReturnsAsync([]);

        // Act
        Func<Task> act = async () => await _service.CreateAsync(dto);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage($"*ids not found: {missingId}*");
        
        _recipeRepoMock.Verify(r => r.AddAsync(It.IsAny<Recipe>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Should_UpdateMeta_And_SyncIngredients()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var oldIngId = Guid.NewGuid();
        var newIngId = Guid.NewGuid();

        var dto = new RecipeCreateDto
        {
            Name = "Updated Borsch",
            Ingredients =
            [
                new RecipeIngredientCreateDto { IngredientId = newIngId, Quantity = "1kg" }
            ]
        };
        
        var existingLinks = new List<RecipeIngredient>
        {
            new() { Id = Guid.NewGuid(), RecipeId = recipeId, IngredientId = oldIngId, Quantity = "10g" }
        };
        
        _ingredientRepoMock.Setup(r => r.ExistingIdsAsync(It.IsAny<Guid[]>(), CancellationToken.None))
            .ReturnsAsync([newIngId]);
        
        _recipeIngredientRepoMock.Setup(r => r.GetByRecipeIdAsync(recipeId, CancellationToken.None))
            .ReturnsAsync(existingLinks);
        
        _recipeRepoMock.Setup(r => r.GetByIdAsync(recipeId, CancellationToken.None))
            .ReturnsAsync(new Recipe());

        // Act
        await _service.UpdateAsync(recipeId, dto);

        // Assert
        
        _recipeRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Recipe>(), CancellationToken.None), Times.Once);

        _recipeIngredientRepoMock.Verify(r => r.DeleteManyAsync(
            It.Is<Guid[]>(ids => ids.Contains(existingLinks[0].Id)), CancellationToken.None), Times.Once);

        
        _recipeIngredientRepoMock.Verify(r => r.AddRangeAsync(
            It.Is<IEnumerable<RecipeIngredient>>(list => list.Any(x => x.IngredientId == newIngId)), CancellationToken.None), Times.Once);
    }
}