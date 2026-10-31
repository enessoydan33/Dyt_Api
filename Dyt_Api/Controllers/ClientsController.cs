using System.Linq.Expressions;
using System.Security.Claims;
using Dyt_Api.Data;
using Dyt_Api.DTOs.Clients;
using Dyt_Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dyt_Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private static readonly Expression<Func<Client, ClientResponseDto>> ClientProjection =
            client => new ClientResponseDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                BirthDate = client.BirthDate,
                Gender = client.Gender,
                Height = client.Height,
                ActivityLevel = client.ActivityLevel,
                Goal = client.Goal,
                Notes = client.Notes,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt
            };

        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ClientResponseDto>> Create(
            CreateClientDto request,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var now = DateTime.UtcNow;
            var client = new Client
            {
                AssignedDietitianId = userId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = NormalizeOptional(request.PhoneNumber),
                Email = NormalizeOptional(request.Email),
                BirthDate = request.BirthDate!.Value,
                Gender = request.Gender!.Value,
                Height = request.Height,
                ActivityLevel = request.ActivityLevel!.Value,
                Goal = request.Goal!.Value,
                Notes = NormalizeOptional(request.Notes),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = client.Id },
                ToResponseDto(client));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<ClientResponseDto>>> GetAll(
            [FromQuery] bool? isActive,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var query = _context.Clients
                .AsNoTracking()
                .Where(client => client.AssignedDietitianId == userId);

            if (isActive.HasValue)
            {
                query = query.Where(client => client.IsActive == isActive.Value);
            }

            var clients = await query
                .OrderByDescending(client => client.CreatedAt)
                .Select(ClientProjection)
                .ToListAsync(cancellationToken);

            return Ok(clients);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClientResponseDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var client = await _context.Clients
                .AsNoTracking()
                .Where(client =>
                    client.Id == id &&
                    client.AssignedDietitianId == userId)
                .Select(ClientProjection)
                .SingleOrDefaultAsync(cancellationToken);

            return client is null ? NotFound() : Ok(client);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ClientResponseDto>> Update(
            int id,
            UpdateClientDto request,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var client = await _context.Clients
                .SingleOrDefaultAsync(
                    client =>
                        client.Id == id &&
                        client.AssignedDietitianId == userId,
                    cancellationToken);

            if (client is null)
            {
                return NotFound();
            }

            client.FirstName = request.FirstName.Trim();
            client.LastName = request.LastName.Trim();
            client.PhoneNumber = NormalizeOptional(request.PhoneNumber);
            client.Email = NormalizeOptional(request.Email);
            client.BirthDate = request.BirthDate;
            client.Gender = request.Gender;
            client.Height = request.Height;
            client.ActivityLevel = request.ActivityLevel;
            client.Goal = request.Goal;
            client.Notes = NormalizeOptional(request.Notes);
            client.IsActive = request.IsActive;
            client.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(ToResponseDto(client));
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

            var client = await _context.Clients
                .SingleOrDefaultAsync(
                    client =>
                        client.Id == id &&
                        client.AssignedDietitianId == userId,
                    cancellationToken);

            if (client is null)
            {
                return NotFound();
            }

            client.IsActive = false;
            client.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
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

        private static ClientResponseDto ToResponseDto(Client client)
        {
            return new ClientResponseDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                BirthDate = client.BirthDate,
                Gender = client.Gender,
                Height = client.Height,
                ActivityLevel = client.ActivityLevel,
                Goal = client.Goal,
                Notes = client.Notes,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt
            };
        }
    }
}
