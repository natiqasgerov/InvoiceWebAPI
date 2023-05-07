using FluentValidation;
using InvoiceApiFinal.DTOs.Invoice;

namespace InvoiceApiFinal.DTOs.Validations
{
    public class UpdateInvoiceValidation : AbstractValidator<InvoiceEditRequest>
    {
        public UpdateInvoiceValidation()
        {
            RuleFor(x => x.Title).MinimumLength(6);
        }
    }
}
