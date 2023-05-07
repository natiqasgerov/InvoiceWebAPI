using FluentValidation;
using InvoiceApiFinal.DTOs.Customer;

namespace InvoiceApiFinal.DTOs.Validations
{
    public class CustomerValidation : AbstractValidator<CreateCustomerForm>
    {
        public CustomerValidation()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.Email).Matches("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$").NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(4);
            RuleFor(x => x.PhoneNumber).Matches("^(\\+994|0)?([ -])?(50|51|55|70|77|99)([ -])?(\\d{3})([ -])?(\\d{2})([ -])?(\\d{2})$").NotEmpty();
        }
    }
}
