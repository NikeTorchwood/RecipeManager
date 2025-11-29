using Application.Contracts.Ingredients;
using Application.Repositories.Abstractions;
using Application.Services.Abstractions;
using Application.Services.Implementations.Base;
using AutoMapper;
using Core;

namespace Application.Services.Implementations;

public class IngredientService(
    IIngredientRepository repository,
    IMapper mapper)
    : ServiceBase<
            Ingredient,
            IngredientDto,
            IngredientDto,
            IngredientFilter,
            IngredientCreateDto>(repository, mapper),
        IIngredientService;