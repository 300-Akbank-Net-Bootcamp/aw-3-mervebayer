using FluentValidation;
using Vb.Schema;

namespace Vb.Business.Validator
{
    public class CreateContactValidator: AbstractValidator<ContactRequest>
    {
        public CreateContactValidator()
        {               
            RuleFor(x => x.ContactType).NotEmpty().MaximumLength(10).WithMessage("Contact Type cannot be empty");
            RuleFor(x => x.Information).NotEmpty().MinimumLength(8).MaximumLength(100).WithMessage("Information cannot be empty");
            RuleFor(x => x.IsDefault).NotEmpty().WithName("Customer idefault information");
        }
    }
}