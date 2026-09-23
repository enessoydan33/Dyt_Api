using Dyt_Api.Enums;

namespace Dyt_Api.Entities
{
    public class MealFood
    {
        public int Id { get; set; }
        public int MealId { get; set; }

        public FoodSource FoodSource { get; set; }

        // USDA food: FoodSource = USDA, FoodId set, CustomFoodId null.
        // Custom food: FoodSource = Custom, FoodId null, CustomFoodId set.
        // FoodId is a USDA table identifier only; no USDA navigation is mapped here.
        public long? FoodId { get; set; }
        public int? CustomFoodId { get; set; }

        public decimal Amount { get; set; }
        public string Unit { get; set; } = string.Empty;

        public long? PortionId { get; set; }

        public string? CookingMethod { get; set; }
        public int DisplayOrder { get; set; }

        public string? Notes { get; set; }

        public Meal Meal { get; set; } = null!;
        public CustomFood? CustomFood { get; set; }
    }
}
