using Dyt_Api.Enums;

namespace Dyt_Api.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        public int DietitianId { get; set; }
        public int ClientId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public AppointmentType AppointmentType { get; set; }
        public AppointmentStatus Status { get; set; }

        public string Subject { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser Dietitian { get; set; } = null!;
        public Client Client { get; set; } = null!;
    }
}
