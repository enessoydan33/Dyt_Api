namespace Dyt_Api.Entities
{
    public class CustomFood
    {
        public int Id { get; set; }
        public int CreatedByUserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbohydrate { get; set; }
        public decimal Fat { get; set; }
        public decimal Fiber { get; set; }

        public decimal ServingSize { get; set; }
        public string ServingUnit { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public ApplicationUser CreatedByUser { get; set; } = null!;
        public ICollection<MealFood> MealFoods { get; set; } = [];
    }
}
