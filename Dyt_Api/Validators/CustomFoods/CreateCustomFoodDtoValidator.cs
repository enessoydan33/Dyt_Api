using Dyt_Api.DTOs.CustomFoods;
using FluentValidation;

namespace Dyt_Api.Validators.CustomFoods
{
    public class CreateCustomFoodDtoValidator : AbstractValidator<CreateCustomFoodDto>
    {
        public CreateCustomFoodDtoValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty()
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .MaximumLength(200);

            RuleFor(request => request.Calories)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100000);

            RuleFor(request => request.Protein)
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.Carbohydrate)
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.Fat)
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.Fiber)
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.ServingSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100000);

            RuleFor(request => request.ServingUnit)
                .NotEmpty()
                .Must(unit => !string.IsNullOrWhiteSpace(unit))
                .MaximumLength(50);

            RuleFor(request => request.Description)
                .MaximumLength(1000)
                .When(request => request.Description is not null);
        }
    }
}
