using Dyt_Api.DTOs.Clients;
using FluentValidation;

namespace Dyt_Api.Validators.Clients
{
    public class CreateClientDtoValidator : AbstractValidator<CreateClientDto>
    {
        private const string PhoneNumberPattern = @"^\+?[0-9\s().-]{7,20}$";

        public CreateClientDtoValidator()
        {
            RuleFor(request => request.FirstName)
                .NotEmpty();

            RuleFor(request => request.LastName)
                .NotEmpty();

            RuleFor(request => request.BirthDate)
                .NotNull()
                .Must(date => !date.HasValue || ClientBirthDateRules.IsNotInFuture(date.Value))
                .WithMessage(ClientBirthDateRules.FutureDateMessage)
                .Must(date => !date.HasValue || ClientBirthDateRules.IsWithinMaximumAge(date.Value))
                .WithMessage(ClientBirthDateRules.MaximumAgeMessage);

            RuleFor(request => request.Height)
                .GreaterThan(0)
                .LessThanOrEqualTo(300);

            RuleFor(request => request.Email)
                .EmailAddress()
                .When(request => !string.IsNullOrWhiteSpace(request.Email));

            RuleFor(request => request.PhoneNumber)
                .Matches(PhoneNumberPattern)
                .When(request => !string.IsNullOrWhiteSpace(request.PhoneNumber));

            RuleFor(request => request.Gender)
                .NotNull()
                .IsInEnum();

            RuleFor(request => request.ActivityLevel)
                .NotNull()
                .IsInEnum();

            RuleFor(request => request.Goal)
                .NotNull()
                .IsInEnum();
        }
    }
}
