using Dyt_Api.DTOs.Clients;
using FluentValidation;

namespace Dyt_Api.Validators.Clients
{
    public class UpdateClientDtoValidator : AbstractValidator<UpdateClientDto>
    {
        private const string PhoneNumberPattern = @"^\+?[0-9\s().-]{7,20}$";

        public UpdateClientDtoValidator()
        {
            RuleFor(request => request.FirstName)
                .NotEmpty();

            RuleFor(request => request.LastName)
                .NotEmpty();

            RuleFor(request => request.BirthDate)
                .Must(ClientBirthDateRules.IsNotInFuture)
                .WithMessage(ClientBirthDateRules.FutureDateMessage)
                .Must(ClientBirthDateRules.IsWithinMaximumAge)
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
                .IsInEnum();

            RuleFor(request => request.ActivityLevel)
                .IsInEnum();

            RuleFor(request => request.Goal)
                .IsInEnum();
        }
    }
}
