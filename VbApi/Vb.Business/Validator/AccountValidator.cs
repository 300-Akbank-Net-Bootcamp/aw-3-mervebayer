using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator;

public class CreateAccountValidator : AbstractValidator<AccountRequest>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("Account Number cannot be empty");
        RuleFor(x => x.CurrencyType).NotEmpty().MaximumLength(3).WithMessage("Currency Type cannot be empty");
        RuleFor(x => x.IBAN).NotEmpty().MaximumLength(34).WithName("IBAN");
        RuleFor(x => x.Name).MaximumLength(100);

        // RuleForEach(x => x.AccountTransactions).SetValidator(new CreateAccountTransactionValidator());
        // RuleForEach(x => x.EftTransactions).SetValidator(new CreateEftTransactionValidator());
    }
}