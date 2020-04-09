using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Framework.Core.Helpers;
using Framework.Infra.Data;
using Framework.Web.Api;
using Framework.Web.Extentions;
using Framework.Web.Filters;
using Framework.Web.Helpers;
using Framework.Web.Models;
using Infra.Authentication.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnionArchitect.Api.V1.Dtos.Roles;
using OnionArchitect.Api.V1.Forms.UserRoles;

namespace OnionArchitect.Api.V1.Controllers
{

    [Route("api/Users/{userId}/Roles")]
    [Authorize(Roles = "Admin")]
    public class UserRolesController : BaseController
    {
        private readonly PagingMetadataHelper pagingMetadataHelper;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly IMapper mapper;

        public UserRolesController(PagingMetadataHelper pagingMetadataHelper,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IMapper mapper):base(pagingMetadataHelper)
        {
            this.pagingMetadataHelper = pagingMetadataHelper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        [LogRequest]
        [HttpGet(Name = nameof(GetUserRoles))]
        public virtual IActionResult GetUserRoles([FromRoute] int userId,[FromQuery]PaginationParameters paginationParameters)
        {

            var userFromDatabase = userManager.FindByIdAsync(userId.ToString()).Result;
            if(userFromDatabase==null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم کاربر با شناسه {userId} را پیدا کنیم" });

            var rolesQuery = userManager.GetRolesAsync(userFromDatabase).Result.AsQueryable();


            PagedList<string> rolesForReturn =
                PageingHelper.CreatePagedList(rolesQuery, paginationParameters.PageNumber, paginationParameters.PageSize);
            var paginationMetadata =
                 this.pagingMetadataHelper.GetPagingMetadata(rolesForReturn, nameof(GetUserRoles), paginationParameters);

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            return Ok(rolesForReturn);
        }


        [HttpGet("{roleName}", Name = nameof(GetUserRole))]
        public virtual IActionResult GetUserRole([FromRoute] int userId,[FromRoute]string roleName)
        {
            var userFromDatabase = userManager.FindByIdAsync(userId.ToString()).Result;
            if (userFromDatabase == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم کاربر با شناسه {userId} را پیدا کنیم" });

            var roleFromUserRoles = userManager.GetRolesAsync(userFromDatabase).Result.FirstOrDefault(r=>r.Equals(roleName));
            if(roleFromUserRoles==null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $"نقش {roleName} در لیست نقش های کاربر نبود" });

            var roleFromDatabase = roleManager.FindByNameAsync(roleFromUserRoles).Result;
            
            var roleForReturn = mapper.Map<RoleDto>(roleFromDatabase);
            return Ok(roleForReturn);
        }


        [HttpPost(Name = nameof(AssignRoleToUser))]
        public virtual IActionResult AssignRoleToUser([FromRoute] int userId,[FromBody] UserRoleAssignmentForm userRoleAssignmentForm)
        {
            var userFromDatabase = userManager.FindByIdAsync(userId.ToString()).Result;
            if (userFromDatabase == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم کاربر با شناسه {userId} را پیدا کنیم" });


            var roleFromDatabase = roleManager.FindByNameAsync(userRoleAssignmentForm.roleName).Result;
            if (roleFromDatabase == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم نقش با نام {userRoleAssignmentForm.roleName} را پیدا کنیم" });

            var assignmentResult = userManager.AddToRoleAsync(userFromDatabase, roleFromDatabase.Name).Result;
            if (!assignmentResult.Succeeded)
            {
                foreach (var message in assignmentResult.Errors)
                {
                    ModelState.AddModelError(nameof(userRoleAssignmentForm.roleName), message.Description);
                }
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.Title = "مقادیر ارسالی صحیح نیستند";
                responseMessage.Descripton = ModelState.GetSingleLineErrorMessages();
                return BadRequest(responseMessage);
            }

            var roleForReturn = mapper.Map<RoleDto>(roleFromDatabase);
            return Created(Url.Link(nameof(GetUserRole), new {userId=userId, roleName = userRoleAssignmentForm.roleName }), roleFromDatabase);
        }



        [HttpDelete("{roleName}", Name = nameof(DeleteUserRole))]
        public virtual IActionResult DeleteUserRole([FromRoute] int userId, [FromRoute]string roleName)
        {
            var userFromDatabase = userManager.FindByIdAsync(userId.ToString()).Result;
            if (userFromDatabase == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم کاربر با شناسه {userId} را پیدا کنیم" });

            var roleFromUserRoles = userManager.GetRolesAsync(userFromDatabase).Result.FirstOrDefault(r => r.Equals(roleName));
            if (roleFromUserRoles == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $"نقش {roleName} در لیست نقش های کاربر نبود" });


       
          
            IdentityResult deleteRoleResult = userManager.RemoveFromRoleAsync(userFromDatabase, roleFromUserRoles).Result;
            if (!deleteRoleResult.Succeeded)
            {
                foreach (var message in deleteRoleResult.Errors)
                {
                    ModelState.AddModelError(nameof(Role.Name), message.Description);
                }
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.Title = "مقادیر ارسالی صحیح نیستند";
                responseMessage.Descripton = ModelState.GetSingleLineErrorMessages();
                return BadRequest(responseMessage);
            }
            return NoContent();
        }

    }
}