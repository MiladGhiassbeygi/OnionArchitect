using FluentValidation;
using OnionArchitect.Api.V1.Forms.UserRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitect.Api.V1.Validators.UserRoleValidators
{
    public class UserRoleAssignmentFormValidator: AbstractValidator<UserRoleAssignmentForm>
    {
        public UserRoleAssignmentFormValidator()
        {
            RuleFor(x => x.roleName)
               .NotNull()
               .NotEmpty()
               .WithMessage("نام نقش نمی تواند بدون مقدار یا رشته خالی باشد ");
        }
    }
}
