using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator
{
    public class CreateAccountTransactionValidator : AbstractValidator<AccountTransactionRequest>
    {
        public CreateAccountTransactionValidator()
        {
            RuleFor(x => x.TransferType).NotEmpty().MaximumLength(10).WithMessage("TransferType cannot be empty");
            RuleFor(x => x.ReferenceNumber).NotEmpty().MaximumLength(50).WithMessage("ReferenceNumber cannot be empty or maximum length should be 50");
            RuleFor(x => x.Description).MaximumLength(300).WithMessage("Description cannot be longer than 300 characters");
        }
    }
}