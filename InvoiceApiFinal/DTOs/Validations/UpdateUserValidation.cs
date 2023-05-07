using FluentValidation;
using InvoiceApiFinal.DTOs.User;

namespace InvoiceApiFinal.DTOs.Validations
{
    public class UpdateUserValidation : AbstractValidator<UserEditRequest>
    {
        public UpdateUserValidation()
        {
            RuleFor(x => x.Name).MinimumLength(3);
            RuleFor(x => x.Email).Matches("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$");
            RuleFor(x => x.Address).MinimumLength(3);
            RuleFor(x => x.PhoneNumber).Matches("^(\\+994|0)?([ -])?(50|51|55|70|77|99)([ -])?(\\d{3})([ -])?(\\d{2})([ -])?(\\d{2})$");
        }
    }
}
