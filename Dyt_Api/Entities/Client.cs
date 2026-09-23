using Dyt_Api.Enums;

namespace Dyt_Api.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public int AssignedDietitianId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public DateOnly BirthDate { get; set; }
        public Gender Gender { get; set; }
        public decimal Height { get; set; }
        public ActivityLevel ActivityLevel { get; set; }
        public ClientGoal Goal { get; set; }

        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser AssignedDietitian { get; set; } = null!;
        public ICollection<Measurement> Measurements { get; set; } = [];
        public ICollection<NutritionPlan> NutritionPlans { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
