using FluentValidation;
using InvoiceApiFinal.DTOs.User;

namespace InvoiceApiFinal.DTOs.Validations
{
    public class RegisterValidation : AbstractValidator<UserRegisterForm>
    {
        public RegisterValidation()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3);
            RuleFor(x => x.Email).Matches("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$").NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.Password).Matches("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$").NotEmpty();
            RuleFor(x => x.PhoneNumber).Matches("^(\\+994|0)?([ -])?(50|51|55|70|77|99)([ -])?(\\d{3})([ -])?(\\d{2})([ -])?(\\d{2})$").NotEmpty();
        }
    }
}
