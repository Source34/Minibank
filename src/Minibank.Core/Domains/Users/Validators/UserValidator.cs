using FluentValidation;

namespace Minibank.Core.Domains.Users.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public static string IsValidLoginAndEmailRules = "IsValidLoginEmailRules";
        public UserValidator()
        {
            RuleSet(IsValidLoginAndEmailRules, () =>
            {
                RuleFor(p => p.Login)
                    .NotEmpty()
                    .WithMessage(UserValidatorMessages.LoginIsNullOrEmpty)
                    .NotNull()
                    .WithMessage(UserValidatorMessages.LoginIsNullOrEmpty);

                RuleFor(p => p.Login.Length)
                    .LessThanOrEqualTo(20)
                    .WithMessage(UserValidatorMessages.LoginIsTooLong);

                RuleFor(p => p.Email)
                    .NotEmpty()
                    .WithMessage(UserValidatorMessages.EmailIsNullOrEmpty)
                    .NotNull()
                    .WithMessage(UserValidatorMessages.EmailIsNullOrEmpty);

                RuleFor(p => p.Email.Length)
                    .LessThanOrEqualTo(20)
                    .WithMessage(UserValidatorMessages.EmailIsTooLong);
            });
        }
    }
}
