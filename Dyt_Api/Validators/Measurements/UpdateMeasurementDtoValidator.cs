using Dyt_Api.DTOs.Measurements;
using FluentValidation;

namespace Dyt_Api.Validators.Measurements
{
    public class UpdateMeasurementDtoValidator : AbstractValidator<UpdateMeasurementDto>
    {
        public UpdateMeasurementDtoValidator()
        {
            RuleFor(request => request.MeasurementDate)
                .NotEmpty()
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("MeasurementDate cannot be in the future.");

            RuleFor(request => request.Weight)
                .GreaterThan(0)
                .LessThanOrEqualTo(500);

            RuleFor(request => request.Height)
                .GreaterThan(0)
                .LessThanOrEqualTo(300)
                .When(request => request.Height.HasValue);

            RuleFor(request => request.BodyFatPercentage)
                .InclusiveBetween(0, 100)
                .When(request => request.BodyFatPercentage.HasValue);

            RuleFor(request => request.MusclePercentage)
                .InclusiveBetween(0, 100)
                .When(request => request.MusclePercentage.HasValue);

            RuleFor(request => request.WaterPercentage)
                .InclusiveBetween(0, 100)
                .When(request => request.WaterPercentage.HasValue);

            RuleFor(request => request.WaistCircumference)
                .GreaterThan(0)
                .LessThanOrEqualTo(500)
                .When(request => request.WaistCircumference.HasValue);

            RuleFor(request => request.HipCircumference)
                .GreaterThan(0)
                .LessThanOrEqualTo(500)
                .When(request => request.HipCircumference.HasValue);

            RuleFor(request => request.NeckCircumference)
                .GreaterThan(0)
                .LessThanOrEqualTo(500)
                .When(request => request.NeckCircumference.HasValue);
        }
    }
}
