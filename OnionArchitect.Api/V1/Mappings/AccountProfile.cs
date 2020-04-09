using AutoMapper;
using Infra.Authentication.Identity.Models;
using OnionArchitect.Api.V1.Forms.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitect.Api.V1.Mappings
{
    public class AccountProfile:Profile
    {
        public AccountProfile()
        {
            CreateMap<SignUpForm, User>();
            CreateMap<UpdateAccountForm, User>();
            
           
        }
    }
}
