using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Framework.Core.Helpers;
using Framework.Infra.Data;
using Framework.Web.Api;
using Framework.Web.Extentions;
using Framework.Web.Helpers;
using Framework.Web.Models;
using Framework.Core.Extentions;
using Infra.Authentication.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnionArchitect.Api.V1.Dtos.Users;
using OnionArchitect.Api.V1.Forms.Users;
using Microsoft.AspNetCore.Authorization;

namespace OnionArchitect.Api.V1.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly PagingMetadataHelper pagingMetadataHelper;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public UsersController(PagingMetadataHelper pagingMetadataHelper, UserManager<User> userManager,IMapper mapper)
            :base(pagingMetadataHelper)
        {
            this.pagingMetadataHelper = pagingMetadataHelper;
            this.userManager = userManager;
            this.mapper = mapper;
        }


        [HttpGet(Name = nameof(GetUsers))]
        public virtual IActionResult GetUsers([FromQuery]PaginationParameters paginationParameters)
        {

            var usersQuery = userManager.Users;

            PagedList<UserDto> usersForReturn =
                PageingHelper.CreatePagedList<User, UserDto>(usersQuery, paginationParameters.PageNumber, paginationParameters.PageSize, mapper);
            var paginationMetadata =
                 this.pagingMetadataHelper.GetPagingMetadata(usersForReturn, nameof(GetUsers), paginationParameters);

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            return Ok(usersForReturn);
        }

        [HttpGet("{id}", Name = nameof(GetUser))]
        public virtual IActionResult GetUser([FromRoute]int id)
        {
            var userFromDatabase = userManager.FindByIdAsync(id.ToString()).Result;
            if (userFromDatabase == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم کاربر با شناسه {id} را پیدا کنیم" });

            var userForReturn = mapper.Map<UserDto>(userFromDatabase);
            return Ok(userForReturn);
        }


        [HttpPost(Name = nameof(CreateUser))]
        public virtual IActionResult CreateUser([FromBody] CreateUserForm creatRoleForm)
        {
            User userForCreate = mapper.Map<User>(creatRoleForm);
            userForCreate.UserName = userForCreate.Email;
            IdentityResult createRoleResult = userManager.CreateAsync(userForCreate,creatRoleForm.Password).Result;
            if (!createRoleResult.Succeeded)
            {
                foreach (var message in createRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, message.Description);
                }
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.Title = "مقادیر ارسالی صحیح نیستند";
                responseMessage.Descripton = ModelState.GetSingleLineErrorMessages();
                return BadRequest(responseMessage);
            }

            UserDto createdRole = mapper.Map<UserDto>(userForCreate);
            return Created(Url.Link(nameof(GetUser), new { id = createdRole.Id }), createdRole);
        }


        [HttpPut("{id}", Name = nameof(UpdateUser))]
        public virtual IActionResult UpdateUser([FromRoute]int id, [FromBody]UpdateUserForm updateUserForm)
        {
            User userForUpdate = userManager.FindByIdAsync(id.ToString()).Result;
            if (userForUpdate == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم کاربر با شناسه {id} را پیدا کنیم" });
           if(string.IsNullOrEmpty(updateUserForm.UserName.NullIfEmpty())||
              (string.IsNullOrEmpty(updateUserForm.UserName.NullIfEmpty())&&(userForUpdate.Email!=updateUserForm.Email)))
                {
                updateUserForm.UserName = updateUserForm.Email;
            }
            mapper.Map(updateUserForm, userForUpdate);
            
            IdentityResult updateRoleResult = userManager.UpdateAsync(userForUpdate).Result;
            if (!updateRoleResult.Succeeded)
            {
                foreach (var message in updateRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, message.Description);
                }
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.Title = "مقادیر ارسالی صحیح نیستند";
                responseMessage.Descripton = ModelState.GetSingleLineErrorMessages();
                return BadRequest(responseMessage);
            }

            return NoContent();



        }


        [HttpDelete("{id}", Name = nameof(DeleteUser))]
        public virtual IActionResult DeleteUser(int id)
        {
            User userForDelete = userManager.FindByIdAsync(id.ToString()).Result;
            if (userForDelete == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم کاربر با شناسه {id} را پیدا کنیم" });
            IdentityResult deleteRoleResult = userManager.DeleteAsync(userForDelete).Result;
            if (!deleteRoleResult.Succeeded)
            {
                foreach (var message in deleteRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, message.Description);
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