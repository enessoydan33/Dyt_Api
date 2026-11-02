using Dyt_Api.DTOs.CustomFoods;
using FluentValidation;

namespace Dyt_Api.Validators.CustomFoods
{
    public class UpdateCustomFoodDtoValidator : AbstractValidator<UpdateCustomFoodDto>
    {
        public UpdateCustomFoodDtoValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty()
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .MaximumLength(200);

            RuleFor(request => request.Calories)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100000);

            RuleFor(request => request.Protein)
                .NotNull()
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.Carbohydrate)
                .NotNull()
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.Fat)
                .NotNull()
                .GreaterThanOrEqualTo(0);

            RuleFor(request => request.Fiber)
                .NotNull()
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

            RuleFor(request => request.IsActive)
                .NotNull();
        }
    }
}
