using FluentValidation;
using OnionArchitect.Api.V1.Forms.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitect.Api.V1.Validators.Account
{
    public class SignInFormValidator: AbstractValidator<SignInForm>
    {
        public SignInFormValidator()
        {
            RuleFor(x => x.Email)
               .NotNull()
               .NotEmpty()
               .WithMessage("نام نقش نمی تواند بدون مقدار یا رشته خالی باشد ")
               .EmailAddress()
               .WithMessage("لطفا یک ایمیل معتبر وارد کنید");

            RuleFor(x => x.Password)
               .NotNull()
               .NotEmpty()
               .WithMessage("رمز عبور نمی تواند بدون مقدار یا رشته خالی  باشد ");
        }
    }
}
