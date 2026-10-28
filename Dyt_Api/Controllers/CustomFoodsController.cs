using System.Linq.Expressions;
using System.Security.Claims;
using Dyt_Api.Data;
using Dyt_Api.DTOs.CustomFoods;
using Dyt_Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dyt_Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/custom-foods")]
    public class CustomFoodsController : ControllerBase
    {
        private static readonly Expression<Func<CustomFood, CustomFoodResponseDto>>
            CustomFoodProjection =
                customFood => new CustomFoodResponseDto
                {
                    Id = customFood.Id,
                    Name = customFood.Name,
                    Calories = customFood.Calories,
                    Protein = customFood.Protein,
                    Carbohydrate = customFood.Carbohydrate,
                    Fat = customFood.Fat,
                    Fiber = customFood.Fiber,
                    ServingSize = customFood.ServingSize,
                    ServingUnit = customFood.ServingUnit,
                    Description = customFood.Description,
                    IsActive = customFood.IsActive,
                    CreatedAt = customFood.CreatedAt,
                    UpdatedAt = customFood.UpdatedAt
                };

        private readonly ApplicationDbContext _context;

        public CustomFoodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<CustomFoodResponseDto>> Create(
            CreateCustomFoodDto request,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var name = request.Name.Trim();

            if (await ActiveDuplicateExistsAsync(
                    userId,
                    name,
                    excludedId: null,
                    cancellationToken))
            {
                return Conflict(new
                {
                    message = "An active custom food with this name already exists."
                });
            }

            var now = DateTime.UtcNow;
            var customFood = new CustomFood
            {
                CreatedByUserId = userId,
                Name = name,
                Calories = request.Calories,
                Protein = request.Protein,
                Carbohydrate = request.Carbohydrate,
                Fat = request.Fat,
                Fiber = request.Fiber,
                ServingSize = request.ServingSize,
                ServingUnit = request.ServingUnit.Trim(),
                Description = NormalizeOptional(request.Description),
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true
            };

            _context.CustomFoods.Add(customFood);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customFood.Id },
                ToResponseDto(customFood));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<CustomFoodResponseDto>>> GetAll(
            [FromQuery] bool? isActive,
            [FromQuery] string? search,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var query = _context.CustomFoods
                .AsNoTracking()
                .Where(customFood => customFood.CreatedByUserId == userId);

            if (isActive.HasValue)
            {
                query = query.Where(customFood => customFood.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";

                query = query.Where(customFood =>
                    EF.Functions.ILike(customFood.Name, pattern));
            }

            var customFoods = await query
                .OrderBy(customFood => customFood.Name)
                .ThenBy(customFood => customFood.Id)
                .Select(CustomFoodProjection)
                .ToListAsync(cancellationToken);

            return Ok(customFoods);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomFoodResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var customFood = await _context.CustomFoods
                .AsNoTracking()
                .Where(customFood =>
                    customFood.Id == id &&
                    customFood.CreatedByUserId == userId)
                .Select(CustomFoodProjection)
                .SingleOrDefaultAsync(cancellationToken);

            return customFood is null ? NotFound() : Ok(customFood);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomFoodResponseDto>> Update(
            int id,
            UpdateCustomFoodDto request,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var customFood = await _context.CustomFoods
                .SingleOrDefaultAsync(
                    customFood =>
                        customFood.Id == id &&
                        customFood.CreatedByUserId == userId,
                    cancellationToken);

            if (customFood is null)
            {
                return NotFound();
            }

            var name = request.Name.Trim();

            if (request.IsActive &&
                await ActiveDuplicateExistsAsync(
                    userId,
                    name,
                    customFood.Id,
                    cancellationToken))
            {
                return Conflict(new
                {
                    message = "An active custom food with this name already exists."
                });
            }

            customFood.Name = name;
            customFood.Calories = request.Calories;
            customFood.Protein = request.Protein;
            customFood.Carbohydrate = request.Carbohydrate;
            customFood.Fat = request.Fat;
            customFood.Fiber = request.Fiber;
            customFood.ServingSize = request.ServingSize;
            customFood.ServingUnit = request.ServingUnit.Trim();
            customFood.Description = NormalizeOptional(request.Description);
            customFood.IsActive = request.IsActive;
            customFood.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(ToResponseDto(customFood));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Deactivate(
            int id,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var customFood = await _context.CustomFoods
                .SingleOrDefaultAsync(
                    customFood =>
                        customFood.Id == id &&
                        customFood.CreatedByUserId == userId,
                    cancellationToken);

            if (customFood is null)
            {
                return NotFound();
            }

            customFood.IsActive = false;
            customFood.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        private Task<bool> ActiveDuplicateExistsAsync(
            int userId,
            string name,
            int? excludedId,
            CancellationToken cancellationToken)
        {
            var normalizedName = name.ToLower();

            return _context.CustomFoods
                .AsNoTracking()
                .AnyAsync(
                    customFood =>
                        customFood.CreatedByUserId == userId &&
                        customFood.IsActive &&
                        (!excludedId.HasValue || customFood.Id != excludedId.Value) &&
                        customFood.Name.ToLower() == normalizedName,
                    cancellationToken);
        }

        private bool TryGetAuthenticatedUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static CustomFoodResponseDto ToResponseDto(CustomFood customFood)
        {
            return new CustomFoodResponseDto
            {
                Id = customFood.Id,
                Name = customFood.Name,
                Calories = customFood.Calories,
                Protein = customFood.Protein,
                Carbohydrate = customFood.Carbohydrate,
                Fat = customFood.Fat,
                Fiber = customFood.Fiber,
                ServingSize = customFood.ServingSize,
                ServingUnit = customFood.ServingUnit,
                Description = customFood.Description,
                IsActive = customFood.IsActive,
                CreatedAt = customFood.CreatedAt,
                UpdatedAt = customFood.UpdatedAt
            };
        }
    }
}
