using FluentValidation;
using InvoiceApiFinal.DTOs.Invoice;

namespace InvoiceApiFinal.DTOs.Validations
{
    public class CreateInvoiceValidation : AbstractValidator<InvoiceCreateRequest>
    {
        public CreateInvoiceValidation() {
            RuleFor(x => x.Title).MinimumLength(6).NotEmpty();
        }
    }
}
