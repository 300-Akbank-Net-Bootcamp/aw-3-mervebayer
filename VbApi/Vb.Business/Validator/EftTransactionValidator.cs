using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator
{
    public class CreateEftTransactionValidator : AbstractValidator<EftTransactionRequest>
    {
        public CreateEftTransactionValidator()
        {
            RuleFor(x => x.SenderAccount).NotEmpty().WithMessage("Sender Account cannot be empty!");
            RuleFor(x => x.SenderIban).NotEmpty().MaximumLength(50).WithMessage("Sender Iban cannot be empty!");
            RuleFor(x => x.SenderName).NotEmpty().MinimumLength(5).WithMessage("Sender Name cannot be empty!");
        }
    }
}