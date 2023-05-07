using FluentValidation;
using InvoiceApiFinal.DTOs.Customer;

namespace InvoiceApiFinal.DTOs.Validations
{
    public class UpdateCustomerValidation : AbstractValidator<CustomerEditRequest>
    {
        public UpdateCustomerValidation()
        {
            RuleFor(x => x.Name).MinimumLength(3);
            RuleFor(x => x.Email).Matches("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$");
            RuleFor(x => x.Address).MinimumLength(3);
            RuleFor(x => x.PhoneNumber).Matches("^(\\+994|0)?([ -])?(50|51|55|70|77|99)([ -])?(\\d{3})([ -])?(\\d{2})([ -])?(\\d{2})$");
        }
    }
}
