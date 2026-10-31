using Dyt_Api.Enums;

namespace Dyt_Api.DTOs.Measurements
{
    public class MeasurementResponseDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateOnly MeasurementDate { get; set; }
        public decimal Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? BodyMassIndex { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public decimal? MusclePercentage { get; set; }
        public decimal? WaterPercentage { get; set; }
        public decimal? WaistCircumference { get; set; }
        public decimal? HipCircumference { get; set; }
        public decimal? NeckCircumference { get; set; }
        public decimal? BasalMetabolicRate { get; set; }
        public decimal? DailyCalorieNeed { get; set; }
        public EnergyCalculationStatus EnergyCalculationStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
