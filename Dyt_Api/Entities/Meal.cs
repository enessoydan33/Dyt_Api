using Dyt_Api.Enums;

namespace Dyt_Api.Entities
{
    public class Meal
    {
        public int Id { get; set; }
        public int NutritionPlanId { get; set; }

        public DateOnly? PlanDate { get; set; }

        public MealType MealType { get; set; }
        public TimeOnly MealTime { get; set; }

        public int DisplayOrder { get; set; }
        public string? Notes { get; set; }

        public NutritionPlan NutritionPlan { get; set; } = null!;
        public ICollection<MealFood> MealFoods { get; set; } = [];
    }
}
