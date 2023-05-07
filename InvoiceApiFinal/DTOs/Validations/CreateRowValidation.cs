using FluentValidation;
using InvoiceApiFinal.DTOs.RowInvoice;

namespace InvoiceApiFinal.DTOs.Validations
{
    public class CreateRowValidation : AbstractValidator<RowCreateRequest>
    {
        public CreateRowValidation()
        {
            RuleFor(x => x.Description).MinimumLength(6).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0).NotEmpty();
        }
    }
}
