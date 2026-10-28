using Dyt_Api.Enums;

namespace Dyt_Api.Services
{
    public interface IMeasurementCalculationService
    {
        MeasurementCalculationResult Calculate(
            decimal weightKg,
            decimal? measurementHeightCm,
            decimal clientHeightCm,
            DateOnly birthDate,
            DateOnly measurementDate,
            Gender gender,
            ActivityLevel activityLevel);
    }

    public sealed record MeasurementCalculationResult(
        decimal? Height,
        decimal? BodyMassIndex,
        decimal? BasalMetabolicRate,
        decimal? DailyCalorieNeed);
}
