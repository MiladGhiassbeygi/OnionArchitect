using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Framework.Core.Extentions;
using Framework.Web.Api;
using Framework.Web.Extentions;
using Framework.Web.Models;
using Infra.Authentication.Identity.Models;
using Infra.Authentication.Identity.Services;
using Infra.Authentication.Jwt.Contracts;
using Infra.Authentication.Jwt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnionArchitect.Api.V1.Dtos.Accounts;
using OnionArchitect.Api.V1.Forms.Accounts;

namespace OnionArchitect.Api.V1.Controllers
{

    [Authorize]
    public class AccountController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;
        private readonly SignInManager<User> signInManager;
        private readonly ITokenFactory tokenFactory;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<User> userManager
            ,RoleManager<Role> roleManager
            ,SignInManager<User> signInManager,
            ITokenFactory tokenFactory,
            ITokenService tokenService
            ,IMapper mapper
            
        ) : base(null)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.tokenFactory = tokenFactory;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }


        [AllowAnonymous]
        [HttpPost("Token", Name = nameof(SignIn))]
        public virtual async Task<IActionResult> SignIn([FromBody]SignInForm signInForm,CancellationToken cancellationToken)
        {
            var userFromDatabase = await userManager.FindByEmailAsync(signInForm.Email);
            if (userFromDatabase == null)
                return BadRequest(new ResponseMessage { Title = $"خطا در احراز هویت", Descripton = $"ایمیل یا رمز عبور اشتباه است" });
            var isPasswordValid = await userManager.CheckPasswordAsync(userFromDatabase, signInForm.Password);
            if (!isPasswordValid)
                return BadRequest(new ResponseMessage { Title = $"خطا در احراز هویت", Descripton = $"ایمیل یا رمز عبور اشتباه است" });

            var result = await signInManager.ClaimsFactory.CreateAsync(userFromDatabase);
            var claimList = new List<Claim>(result.Claims);

            var generatedToken = tokenFactory.GenerateToken(claimList);

            var userToken = new UserToken()
            {
                UserId = userFromDatabase.Id,
                RefreshToken = generatedToken.RefreshToken,
                Expiration = generatedToken.AccessTokenExpirationTime
            };
            
            
            await tokenService.SaveRefreshTokenAsync(userToken, cancellationToken);
           
           


            return Created(string.Empty, generatedToken);
        }
      
        [AllowAnonymous]
        [HttpPost("RefreshToken", Name = nameof(RefreshToken))]
        public virtual async Task<IActionResult> RefreshToken([FromBody]RefreshTokenForm refreshTokenForm, CancellationToken cancellationToken)
        {
            
            var claimsPrincipal = tokenFactory.GetPrincipalFromExpiredToken(refreshTokenForm.AccessToken);
            var userId = claimsPrincipal.Identity.GetUserId<int>();
            var claimList = claimsPrincipal.Claims;
            await tokenService.DeleteExpiredRefreshTokensAsync(cancellationToken);
            var refreshToken = await tokenService.FindRefreshToken(userId, refreshTokenForm.RefreshToken,cancellationToken);

            if (refreshToken == null || refreshToken.Expiration < DateTime.Now)
                return Unauthorized(new ResponseMessage() { Title = "خطای احراز هویت", Descripton = "رفرش توکن منقضی یا نامعتبر است" });


            
            var generatedToken = tokenFactory.GenerateToken(claimList);
            refreshToken.RefreshToken = generatedToken.RefreshToken;
            refreshToken.Expiration = generatedToken.AccessTokenExpirationTime;
            tokenService.UpdateRefreshToken(refreshToken);

          


            return Created(string.Empty, generatedToken);
        }

        [HttpGet("Me",Name =nameof(GetCurrentUserAccount))]
        public virtual async Task<IActionResult> GetCurrentUserAccount()
        {
            var userId = User.Identity.GetUserId<int>();
            var userFromDatabase = await userManager.FindByIdAsync(userId.ToString());
            if (userFromDatabase == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $"متاسفانه نتوانستیم مشخصات مورد نظر را پیدا کنیم" });

            var userForReturn = mapper.Map<MeDto>(userFromDatabase);
           
            return Ok(userForReturn);
        }

        [HttpPut("Me/{id}", Name = nameof(UpdateCurrentUserAccount))]
        public virtual async Task<IActionResult> UpdateCurrentUserAccount([FromRoute]int id,[FromBody] UpdateAccountForm updateAcountForm)
        {
            var userId = User.Identity.GetUserId<int>();
            if (userId != id)
                return BadRequest(new ResponseMessage { Title = $"مقادیر ارسالی معتبر نیستند", Descripton = $"شناسه کاربر ارسالی با کاربر جاری مغایرت دارد" });
            var userForUpdate = await userManager.FindByIdAsync(userId.ToString());
            if (userForUpdate == null)
                return NotFound(new ResponseMessage { Title = $"خطای 404", Descripton = $"متاسفانه نتوانستیم مشخصات مورد نظر را پیدا کنیم" });
            if (string.IsNullOrEmpty(updateAcountForm.UserName.NullIfEmpty()) ||
              (string.IsNullOrEmpty(updateAcountForm.UserName.NullIfEmpty()) && (userForUpdate.Email != updateAcountForm.Email)))
                {
                updateAcountForm.UserName = updateAcountForm.Email;
            }
            mapper.Map(updateAcountForm, userForUpdate);

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

        [AllowAnonymous]
        [HttpPost(Name =nameof(SignUp))]
        public virtual async Task<IActionResult> SignUp([FromBody] SignUpForm signUpForm)
        {
            User userForCreate = mapper.Map<User>(signUpForm);
            userForCreate.UserName = userForCreate.Email;
            IdentityResult createdUserResult = userManager.CreateAsync(userForCreate, signUpForm.Password).Result;
            if (!createdUserResult.Succeeded)
            {
                foreach (var message in createdUserResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, message.Description);
                }
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.Title = "مقادیر ارسالی صحیح نیستند";
                responseMessage.Descripton = ModelState.GetSingleLineErrorMessages();
                return BadRequest(responseMessage);
            }

            var IdentityAssignRoleResult = userManager.AddToRoleAsync(userForCreate, "User").Result;

            if (!IdentityAssignRoleResult.Succeeded)
            {
                foreach (var message in IdentityAssignRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, message.Description);
                }
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.Title = "مقادیر ارسالی صحیح نیستند";
                responseMessage.Descripton = ModelState.GetSingleLineErrorMessages();
                return BadRequest(responseMessage);
            }
            MeDto createdUser = mapper.Map<MeDto>(userForCreate);
            return Created(Url.Link(nameof(createdUser), null), createdUser);
        }


    }
}