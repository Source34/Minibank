using FluentValidation;

namespace Minibank.Core.Domains.Transactions.Validators;

public class TransactionsValidator : AbstractValidator<Transaction>
{
    public const string IsValidTransactionModelRules = "IsValidTransactionModelRules";
    public TransactionsValidator()
    {
        RuleSet(IsValidTransactionModelRules, () =>
        {
            RuleFor(p => p.Amount)
                .GreaterThan(0)
                .WithMessage(TransactionsValidatorMessages.InvalidAmount);

            RuleFor(p => p.FromAccount)
                .NotNull()
                .WithMessage(TransactionsValidatorMessages.FromAccountError);

            RuleFor(p => p.ToAccount)
                .NotNull()
                .WithMessage(TransactionsValidatorMessages.ToAccountError);

            RuleFor(p => p.FromAccount.IsActive)
                .Equal(true)
                .WithMessage(TransactionsValidatorMessages.FromAccountNotActive);

            RuleFor(p => p.ToAccount.IsActive)
                .Equal(true)
                .WithMessage(TransactionsValidatorMessages.ToAccountNotActive);

            RuleFor(p => p.FromAccount.BankAccountId)
                .NotEqual(p => p.ToAccount.BankAccountId)
                .WithMessage(TransactionsValidatorMessages.EqualsBankAccountIds);

            RuleFor(p => p.FromAccount.Balance)
                .GreaterThanOrEqualTo(b => b.Amount)
                .WithMessage(TransactionsValidatorMessages.NotEnoughMoney);
        });
    }
}