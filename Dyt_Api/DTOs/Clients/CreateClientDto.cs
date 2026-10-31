using Dyt_Api.Enums;

namespace Dyt_Api.DTOs.Clients
{
    public class CreateClientDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateOnly? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public decimal Height { get; set; }
        public ActivityLevel? ActivityLevel { get; set; }
        public ClientGoal? Goal { get; set; }
        public string? Notes { get; set; }
    }
}
