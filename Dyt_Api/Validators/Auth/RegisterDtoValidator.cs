using Dyt_Api.DTOs.Auth;
using FluentValidation;

namespace Dyt_Api.Validators.Auth
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(request => request.FirstName)
                .NotEmpty();

            RuleFor(request => request.LastName)
                .NotEmpty();

            RuleFor(request => request.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(request => request.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}
