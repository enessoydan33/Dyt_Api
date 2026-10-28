namespace Dyt_Api.DTOs.Measurements
{
    public class CreateMeasurementDto
    {
        public DateOnly MeasurementDate { get; set; }
        public decimal Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public decimal? MusclePercentage { get; set; }
        public decimal? WaterPercentage { get; set; }
        public decimal? WaistCircumference { get; set; }
        public decimal? HipCircumference { get; set; }
        public decimal? NeckCircumference { get; set; }
        public string? Notes { get; set; }
    }
}
