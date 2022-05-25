using FluentValidation;
using Minibank.Core.Domains.Enums;

namespace Minibank.Core.Domains.BankAccounts.Validators;

public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public const string IsValidModelForCreatingRules = "IsValidModelForCreatingRules";
    public const string IsValidModelForClosingRules = "IsValidModelForClosingRules";
    public BankAccountValidator()
    {
        RuleSet(IsValidModelForCreatingRules, () =>
        {
            RuleFor(p => p)
                .Must(acc => acc.CurrencyCode is CurrencyCore.RUB or CurrencyCore.USD or CurrencyCore.EUR)
                .WithMessage(BankAccountValidatorMessages.InvalidCurrencyCode);
        });

        RuleSet(IsValidModelForClosingRules, () =>
        {
            RuleFor(p => p)
                .Must(acc => acc.Balance == 0)
                .WithMessage(BankAccountValidatorMessages.NotEmptyBalance)
                .Must(acc => !acc.IsActive)
                .WithMessage(BankAccountValidatorMessages.BankAccountAlreadyClosed);
        });
    }
}