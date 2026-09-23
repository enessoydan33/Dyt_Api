using Microsoft.AspNetCore.Identity;

namespace Dyt_Api.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Client> AssignedClients { get; set; } = [];
        public ICollection<CustomFood> CustomFoods { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];
        public ICollection<NutritionPlan> NutritionPlans { get; set; } = [];
    }
}
