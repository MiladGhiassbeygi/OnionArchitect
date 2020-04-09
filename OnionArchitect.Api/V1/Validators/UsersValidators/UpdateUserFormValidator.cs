using FluentValidation;
using OnionArchitect.Api.V1.Forms.Users;
using Framework.Core.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitect.Api.V1.Validators.UsersValidators
{
    public class UpdateUserFormValidator:AbstractValidator<UpdateUserForm>
    {
        public UpdateUserFormValidator()
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
