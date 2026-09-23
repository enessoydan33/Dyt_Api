using Dyt_Api.Enums;

namespace Dyt_Api.Entities
{
    public class NutritionPlan
    {
        public int Id { get; set; }
        public int CreatedByUserId { get; set; }
        public int? ClientId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public NutritionPlanStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser CreatedByUser { get; set; } = null!;
        public Client? Client { get; set; }
        public ICollection<Meal> Meals { get; set; } = [];
    }
}
