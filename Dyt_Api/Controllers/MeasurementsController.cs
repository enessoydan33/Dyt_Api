using System.Linq.Expressions;
using System.Security.Claims;
using Dyt_Api.Data;
using Dyt_Api.DTOs.Measurements;
using Dyt_Api.Entities;
using Dyt_Api.Enums;
using Dyt_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dyt_Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/clients/{clientId:int}/measurements")]
    public class MeasurementsController : ControllerBase
    {
        private static readonly Expression<Func<Measurement, MeasurementResponseDto>>
            MeasurementProjection =
                measurement => new MeasurementResponseDto
                {
                    Id = measurement.Id,
                    ClientId = measurement.ClientId,
                    MeasurementDate = measurement.MeasurementDate,
                    Weight = measurement.Weight,
                    Height = measurement.Height,
                    BodyMassIndex = measurement.BodyMassIndex,
                    BodyFatPercentage = measurement.BodyFatPercentage,
                    MusclePercentage = measurement.MusclePercentage,
                    WaterPercentage = measurement.WaterPercentage,
                    WaistCircumference = measurement.WaistCircumference,
                    HipCircumference = measurement.HipCircumference,
                    NeckCircumference = measurement.NeckCircumference,
                    BasalMetabolicRate = measurement.BasalMetabolicRate,
                    DailyCalorieNeed = measurement.DailyCalorieNeed,
                    Notes = measurement.Notes,
                    CreatedAt = measurement.CreatedAt
                };

        private readonly ApplicationDbContext _context;
        private readonly IMeasurementCalculationService _calculationService;

        public MeasurementsController(
            ApplicationDbContext context,
            IMeasurementCalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }

        [HttpPost]
        public async Task<ActionResult<MeasurementResponseDto>> Create(
            int clientId,
            CreateMeasurementDto request,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var client = await GetOwnedClientAsync(clientId, userId, cancellationToken);

            if (client is null)
            {
                return NotFound();
            }

            if (!ValidateMeasurementDate(request.MeasurementDate, client.BirthDate))
            {
                return ValidationProblem(ModelState);
            }

            var calculation = _calculationService.Calculate(
                request.Weight,
                request.Height,
                client.Height,
                client.BirthDate,
                request.MeasurementDate,
                client.Gender,
                client.ActivityLevel);

            var measurement = new Measurement
            {
                ClientId = clientId,
                MeasurementDate = request.MeasurementDate,
                Weight = request.Weight,
                Height = calculation.Height,
                BodyMassIndex = calculation.BodyMassIndex,
                BodyFatPercentage = request.BodyFatPercentage,
                MusclePercentage = request.MusclePercentage,
                WaterPercentage = request.WaterPercentage,
                WaistCircumference = request.WaistCircumference,
                HipCircumference = request.HipCircumference,
                NeckCircumference = request.NeckCircumference,
                BasalMetabolicRate = calculation.BasalMetabolicRate,
                DailyCalorieNeed = calculation.DailyCalorieNeed,
                Notes = NormalizeOptional(request.Notes),
                CreatedAt = DateTime.UtcNow
            };

            _context.Measurements.Add(measurement);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { clientId, measurementId = measurement.Id },
                ApplyEnergyState(ToResponseDto(measurement), client));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<MeasurementResponseDto>>> GetAll(
            int clientId,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var client = await GetOwnedClientAsync(clientId, userId, cancellationToken);

            if (client is null)
            {
                return NotFound();
            }

            var measurements = await _context.Measurements
                .AsNoTracking()
                .Where(measurement => measurement.ClientId == clientId)
                .OrderByDescending(measurement => measurement.MeasurementDate)
                .ThenByDescending(measurement => measurement.Id)
                .Select(MeasurementProjection)
                .ToListAsync(cancellationToken);

            return Ok(measurements.ConvertAll(measurement => ApplyEnergyState(measurement, client)));
        }

        [HttpGet("{measurementId:int}")]
        public async Task<ActionResult<MeasurementResponseDto>> GetById(
            int clientId,
            int measurementId,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var client = await GetOwnedClientAsync(clientId, userId, cancellationToken);

            if (client is null)
            {
                return NotFound();
            }

            var measurement = await _context.Measurements
                .AsNoTracking()
                .Where(measurement =>
                    measurement.Id == measurementId &&
                    measurement.ClientId == clientId)
                .Select(MeasurementProjection)
                .SingleOrDefaultAsync(cancellationToken);

            return measurement is null ? NotFound() : Ok(ApplyEnergyState(measurement, client));
        }

        [HttpPut("{measurementId:int}")]
        public async Task<ActionResult<MeasurementResponseDto>> Update(
            int clientId,
            int measurementId,
            UpdateMeasurementDto request,
            CancellationToken cancellationToken)
        {
            if (!TryGetAuthenticatedUserId(out var userId))
            {
                return Unauthorized();
            }

            var client = await GetOwnedClientAsync(clientId, userId, cancellationToken);

            if (client is null)
            {
                return NotFound();
            }

            if (!ValidateMeasurementDate(request.MeasurementDate, client.BirthDate))
            {
                return ValidationProblem(ModelState);
            }

            var measurement = await _context.Measurements
                .SingleOrDefaultAsync(
                    measurement =>
                        measurement.Id == measurementId &&
                        measurement.ClientId == clientId,
                    cancellationToken);

            if (measurement is null)
            {
                return NotFound();
            }

            var calculation = _calculationService.Calculate(
                request.Weight,
                request.Height,
                client.Height,
                client.BirthDate,
                request.MeasurementDate,
                client.Gender,
                client.ActivityLevel);

            measurement.MeasurementDate = request.MeasurementDate;
            measurement.Weight = request.Weight;
            measurement.Height = calculation.Height;
            measurement.BodyMassIndex = calculation.BodyMassIndex;
            measurement.BodyFatPercentage = request.BodyFatPercentage;
            measurement.MusclePercentage = request.MusclePercentage;
            measurement.WaterPercentage = request.WaterPercentage;
            measurement.WaistCircumference = request.WaistCircumference;
            measurement.HipCircumference = request.HipCircumference;
            measurement.NeckCircumference = request.NeckCircumference;
            measurement.BasalMetabolicRate = calculation.BasalMetabolicRate;
            measurement.DailyCalorieNeed = calculation.DailyCalorieNeed;
            measurement.Notes = NormalizeOptional(request.Notes);

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(ApplyEnergyState(ToResponseDto(measurement), client));
        }

        // Stored energy values can predate a change to the client's birth date or gender.
        // The response always follows the current client, without rewriting history.
        private MeasurementResponseDto ApplyEnergyState(
            MeasurementResponseDto measurement,
            Client client)
        {
            measurement.EnergyCalculationStatus = _calculationService.GetEnergyCalculationStatus(
                client.BirthDate,
                measurement.MeasurementDate,
                client.Gender);

            if (measurement.EnergyCalculationStatus != EnergyCalculationStatus.Calculated)
            {
                measurement.BasalMetabolicRate = null;
                measurement.DailyCalorieNeed = null;
            }
            else if (measurement.BasalMetabolicRate is null)
            {
                var calculation = _calculationService.Calculate(
                    measurement.Weight,
                    measurement.Height,
                    client.Height,
                    client.BirthDate,
                    measurement.MeasurementDate,
                    client.Gender,
                    client.ActivityLevel);

                measurement.BasalMetabolicRate = calculation.BasalMetabolicRate;
                measurement.DailyCalorieNeed = calculation.DailyCalorieNeed;
            }

            return measurement;
        }

        private Task<Client?> GetOwnedClientAsync(
            int clientId,
            int userId,
            CancellationToken cancellationToken)
        {
            return _context.Clients
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    client =>
                        client.Id == clientId &&
                        client.AssignedDietitianId == userId,
                    cancellationToken);
        }

        private bool TryGetAuthenticatedUserId(out int userId)
        {
            return int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out userId);
        }

        private bool ValidateMeasurementDate(
            DateOnly measurementDate,
            DateOnly birthDate)
        {
            if (measurementDate >= birthDate)
            {
                return true;
            }

            ModelState.AddModelError(
                nameof(measurementDate),
                "MeasurementDate cannot be before the client's BirthDate.");

            return false;
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static MeasurementResponseDto ToResponseDto(Measurement measurement)
        {
            return new MeasurementResponseDto
            {
                Id = measurement.Id,
                ClientId = measurement.ClientId,
                MeasurementDate = measurement.MeasurementDate,
                Weight = measurement.Weight,
                Height = measurement.Height,
                BodyMassIndex = measurement.BodyMassIndex,
                BodyFatPercentage = measurement.BodyFatPercentage,
                MusclePercentage = measurement.MusclePercentage,
                WaterPercentage = measurement.WaterPercentage,
                WaistCircumference = measurement.WaistCircumference,
                HipCircumference = measurement.HipCircumference,
                NeckCircumference = measurement.NeckCircumference,
                BasalMetabolicRate = measurement.BasalMetabolicRate,
                DailyCalorieNeed = measurement.DailyCalorieNeed,
                Notes = measurement.Notes,
                CreatedAt = measurement.CreatedAt
            };
        }
    }
}
