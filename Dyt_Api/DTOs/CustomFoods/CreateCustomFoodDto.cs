namespace Dyt_Api.DTOs.CustomFoods
{
    public class CreateCustomFoodDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbohydrate { get; set; }
        public decimal Fat { get; set; }
        public decimal Fiber { get; set; }
        public decimal ServingSize { get; set; }
        public string ServingUnit { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
