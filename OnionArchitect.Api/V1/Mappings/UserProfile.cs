using AutoMapper;
using Infra.Authentication.Identity.Models;
using OnionArchitect.Api.V1.Dtos.Users;
using OnionArchitect.Api.V1.Forms.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitect.Api.V1.Mappings
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserForm, User>();
            CreateMap<UpdateUserForm, User>();
        }
    }
}
