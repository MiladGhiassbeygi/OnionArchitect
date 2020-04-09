using FluentValidation;
using OnionArchitect.Api.V1.Forms.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitect.Api.V1.Validators.RolesValidators
{
    public class UpdateRoleFormValidator : AbstractValidator<UpdateRoleForm>
    {
        public UpdateRoleFormValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage("نام نقش نمی تواند بدون مقدار یا رشته خالی باشد ");
        }
    }
}
