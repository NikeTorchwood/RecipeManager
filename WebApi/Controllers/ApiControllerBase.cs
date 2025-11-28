using System.Net.Mime;
using Application.Contracts.Abstractions;
using Application.Contracts.Base;
using Application.Services.Abstractions.Base;
using Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Abstract base controller providing standard CRUD operations.
/// </summary>
/// <typeparam name="T">The domain entity type.</typeparam>
/// <typeparam name="TShortDto">The DTO type for list/paged views.</typeparam>
/// <typeparam name="TFullDto">The DTO type for detailed views.</typeparam>
/// <typeparam name="TFilter">The filter type for queries.</typeparam>
/// <typeparam name="TCreateDto">The DTO type used for creation and updates.</typeparam>
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public abstract class ApiControllerBase<T, TShortDto, TFullDto, TFilter, TCreateDto>(
    IServiceBase<T, TShortDto, TFullDto, TFilter, TCreateDto> service)
    : ControllerBase
    where T : class, IEntity
    where TShortDto : class, IShortDto<T>
    where TCreateDto : class, ICreateDto<T>
    where TFullDto : class, IFullDto<T>
    where TFilter : class, IFilter<T>
{
    /// <summary>
    /// Retrieves a paged list of entities based on the provided filter.
    /// </summary>
    /// <param name="filter">Filter parameters (pagination, search terms).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A paged result of entities.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PageResult<TShortDto>>> GetAllAsync(
        [FromQuery] TFilter filter,
        CancellationToken token = default)
    {
        var results = await service.GetAllAsync(filter, token);
        return Ok(results);
    }

    /// <summary>
    /// Retrieves a specific entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The detailed entity DTO.</returns>
    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TFullDto>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var result = await service.GetByIdAsync(id, token);

        if (result is null)
            return Problem(detail: $"Entity with id {id} not found", statusCode: 404);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="request">The creation data transfer object.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The ID of the newly created entity.</returns>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> PostAsync(
        [FromBody] TCreateDto request, CancellationToken token = default)
    {
        var createdId = await service.CreateAsync(request, token);

        return CreatedAtAction(
            actionName: nameof(GetByIdAsync),
            routeValues: new { id = createdId },
            value: createdId);
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update.</param>
    /// <param name="request">The update data.</param>
    /// <param name="token">Cancellation token.</param>
    [HttpPut("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] TCreateDto request, CancellationToken token = default)
    {
        await service.UpdateAsync(id, request, token);

        return NoContent();
    }

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="token">Cancellation token.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken token = default)
    {
        await service.DeleteAsync(id, token);

        return NoContent();
    }
}