using Dyt_Api.Enums;

namespace Dyt_Api.Services
{
    public class MeasurementCalculationService : IMeasurementCalculationService
    {
        private const int AdultAge = 18;

        public MeasurementCalculationResult Calculate(
            decimal weightKg,
            decimal? measurementHeightCm,
            decimal clientHeightCm,
            DateOnly birthDate,
            DateOnly measurementDate,
            Gender gender,
            ActivityLevel activityLevel)
        {
            if (weightKg <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(weightKg));
            }

            var age = CalculateAge(birthDate, measurementDate);
            var height = ResolveHeight(measurementHeightCm, clientHeightCm);

            if (!height.HasValue)
            {
                return new MeasurementCalculationResult(null, null, null, null);
            }

            var heightMeters = height.Value / 100m;
            var bodyMassIndex = Round(weightKg / (heightMeters * heightMeters));
            var energyStatus = GetEnergyCalculationStatus(birthDate, measurementDate, gender);
            var basalMetabolicRate = energyStatus == EnergyCalculationStatus.Calculated
                ? CalculateBasalMetabolicRate(weightKg, height.Value, age, gender)
                : null;

            var activityFactor = GetActivityFactor(activityLevel);
            decimal? dailyCalorieNeed =
                basalMetabolicRate.HasValue && activityFactor.HasValue
                    ? Round(basalMetabolicRate.Value * activityFactor.Value)
                    : null;

            return new MeasurementCalculationResult(
                height,
                bodyMassIndex,
                basalMetabolicRate,
                dailyCalorieNeed);
        }

        public EnergyCalculationStatus GetEnergyCalculationStatus(
            DateOnly birthDate,
            DateOnly measurementDate,
            Gender gender)
        {
            if (measurementDate < birthDate.AddYears(AdultAge))
            {
                return EnergyCalculationStatus.UnsupportedForMinor;
            }

            return gender is Gender.Male or Gender.Female
                ? EnergyCalculationStatus.Calculated
                : EnergyCalculationStatus.UnsupportedForGender;
        }

        private static int CalculateAge(DateOnly birthDate, DateOnly measurementDate)
        {
            if (measurementDate < birthDate)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(measurementDate),
                    "MeasurementDate cannot be before BirthDate.");
            }

            var age = measurementDate.Year - birthDate.Year;

            if (measurementDate < birthDate.AddYears(age))
            {
                age--;
            }

            return age;
        }

        private static decimal? ResolveHeight(
            decimal? measurementHeightCm,
            decimal clientHeightCm)
        {
            if (measurementHeightCm is > 0)
            {
                return measurementHeightCm;
            }

            return clientHeightCm > 0 ? clientHeightCm : null;
        }

        private static decimal? CalculateBasalMetabolicRate(
            decimal weightKg,
            decimal heightCm,
            int age,
            Gender gender)
        {
            var commonValue =
                10m * weightKg +
                6.25m * heightCm -
                5m * age;

            var result = gender switch
            {
                Gender.Male => commonValue + 5m,
                Gender.Female => commonValue - 161m,
                _ => (decimal?)null
            };

            return result.HasValue ? Round(result.Value) : null;
        }

        private static decimal? GetActivityFactor(ActivityLevel activityLevel)
        {
            return activityLevel switch
            {
                ActivityLevel.Sedentary => 1.2m,
                ActivityLevel.LightlyActive => 1.375m,
                ActivityLevel.ModeratelyActive => 1.55m,
                ActivityLevel.VeryActive => 1.725m,
                ActivityLevel.ExtraActive => 1.9m,
                _ => null
            };
        }

        private static decimal Round(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
