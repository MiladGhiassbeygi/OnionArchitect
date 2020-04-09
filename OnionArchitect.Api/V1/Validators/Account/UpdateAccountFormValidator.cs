using FluentValidation;
using Framework.Core.Extentions;
using OnionArchitect.Api.V1.Forms.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitect.Api.V1.Validators.Account
{
    public class UpdateAccountFormValidator : AbstractValidator<UpdateAccountForm>
    {
        public UpdateAccountFormValidator()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage("نام نقش نمی تواند بدون مقدار یا رشته خالی باشد ")
                .EmailAddress()
                .WithMessage("لطفا یک ایمیل معتبر وارد کنید");

            RuleFor(x => x.PhoneNumber)
                 .Must(x => x.IsMobile())
                 .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                 .WithMessage("شماره تلفن همراه صحیح نمی باشد");
        }
    }
}
