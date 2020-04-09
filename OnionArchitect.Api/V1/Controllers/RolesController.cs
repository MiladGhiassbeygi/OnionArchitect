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
using Infra.Authentication.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnionArchitect.Api.V1.Dtos.Roles;
using OnionArchitect.Api.V1.Forms.Roles;

namespace OnionArchitect.Api.V1.Controllers
{

    [Authorize(Roles = "Admin")]
    public class RolesController : BaseController
    {
        private readonly RoleManager<Role> roleManager;
        private readonly IMapper mapper;

        public RolesController(PagingMetadataHelper pagingMetadataHelper,
            RoleManager<Role> roleManager, IMapper mapper)
            :base(pagingMetadataHelper)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
        }


        [HttpGet(Name = nameof(GetRoles))]
        public virtual IActionResult GetRoles([FromQuery]PaginationParameters paginationParameters)
        {
            
            var rolesQuery = roleManager.Roles;

            PagedList<RoleDto> rolesForReturn =
                PageingHelper.CreatePagedList<Role, RoleDto>(rolesQuery, paginationParameters.PageNumber, paginationParameters.PageSize, mapper);
            var paginationMetadata =
                 this.pagingMetadataHelper.GetPagingMetadata(rolesForReturn, nameof(GetRoles), paginationParameters);

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            return Ok(rolesForReturn);
        }

        [HttpGet("{id}", Name = nameof(GetRole))]
        public virtual IActionResult GetRole([FromRoute]int id)
        {
            var roleFromDatabase = roleManager.FindByIdAsync(id.ToString()).Result;
            if (roleFromDatabase == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم نقش با شناسه {id} را پیدا کنیم" });

            var roleForReturn = mapper.Map<RoleDto>(roleFromDatabase);
            return Ok(roleForReturn);
        }


        [HttpPost(Name = nameof(CreateRole))]
        public virtual IActionResult CreateRole([FromBody] CreateRoleForm creatRoleForm)
        {
            Role roleForCreate = mapper.Map<Role>(creatRoleForm);
            IdentityResult createRoleResult = roleManager.CreateAsync(roleForCreate).Result;
            if (!createRoleResult.Succeeded)
            {
                foreach (var message in createRoleResult.Errors)
                {
                    ModelState.AddModelError(nameof(creatRoleForm.Name), message.Description);
                }
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.Title = "مقادیر ارسالی صحیح نیستند";
                responseMessage.Descripton = ModelState.GetSingleLineErrorMessages();
                return BadRequest(responseMessage);
            }

            RoleDto createdRole = mapper.Map<RoleDto>(roleForCreate);
            return Created(Url.Link(nameof(GetRole), new { id = createdRole.Id }), createdRole);
        }


        [HttpPut("{id}",Name =nameof(UpdateRole))]
        public virtual IActionResult UpdateRole([FromRoute]int id,[FromBody]UpdateRoleForm updateRoleForm)
        {
            Role roleForUpdate = roleManager.FindByIdAsync(id.ToString()).Result;
            if(roleForUpdate == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم نقش با شناسه {id} را پیدا کنیم" });
             mapper.Map(updateRoleForm,roleForUpdate);
            IdentityResult updateRoleResult = roleManager.UpdateAsync(roleForUpdate).Result;
            if (!updateRoleResult.Succeeded)
            {
                foreach (var message in updateRoleResult.Errors)
                {
                    ModelState.AddModelError(nameof(updateRoleForm.Name), message.Description);
                }
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.Title = "مقادیر ارسالی صحیح نیستند";
                responseMessage.Descripton = ModelState.GetSingleLineErrorMessages();
                return BadRequest(responseMessage);
            }

            return NoContent();

           
            
        }


        [HttpDelete("{id}",Name =nameof(DeleteRole))]
        public virtual IActionResult DeleteRole(int id)
        {
            Role roleForDelete = roleManager.FindByIdAsync(id.ToString()).Result;
            if (roleForDelete == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $".متاسفانه نتوانستیم نقش با شناسه {id} را پیدا کنیم" });
            IdentityResult deleteRoleResult = roleManager.DeleteAsync(roleForDelete).Result;
            if(!deleteRoleResult.Succeeded)
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