using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.DataBase.Entities;
using NewsAggregatorMvcApp.Models;
using Serilog;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace NewsAggregatorMvcApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;

        public AccountController(IUserService userService, 
            IMapper mapper, IRoleService roleService,
            IArticleService articleService)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _articleService = articleService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userRoleId = await _roleService.GetRoleIdByNameAsync("User");
                    var userDto = _mapper.Map<UserDto>(model);
                    var userWIthSameEmailExists = await _userService.IsUserExists(model.Email);
                    if (userDto != null 
                        && userRoleId != null
                        && !userWIthSameEmailExists
                        && model.Password.Equals(model.PasswordConfirmation))
                    {
                        userDto.RoleId = userRoleId.Value;
                        var result = await _userService.RegisterUser(userDto, model.Password);
                        if (result > 0)
                        {
                            await Authenticate(model.Email);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                return View(model);
            }

            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Register, ControllerName : Account");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult>  CheckEmail(string email)
        {
            var userWIthSameEmailExists = await _userService.IsUserExists(email);
            if (userWIthSameEmailExists)
            {
                return Ok(false);
            }
            return Ok(true);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var isPasswordCorrect = await _userService.CheckUserPassword(model.Email, model.Password);
                if (isPasswordCorrect)
                {
                    await Authenticate(model.Email);

                    if (model.Email == "admin@mail.com" || model.Email == "adminApi@mail.com")
                    {
                        model.IsAdmin = true;

                        return View("Admin_Logged_In");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : Login, ControllerName : Account");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(string email)
        {
            try
            {
                var userDto = await _userService.GetUserByEmailAsync(email);

                var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userDto.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userDto.RoleName)
            };

                var identity = new ClaimsIdentity(claims,
                    "ApplicationCookie",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType
                );


                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
            
            catch (Exception exception)
            {
                Log.Error(exception, "MethodName : Authenticate");
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]                            
        public async Task<IActionResult> AddNews()
        {
            await _articleService.AggregateArticlesFromExternalSourcesAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult IsLoggedIn()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest();
                }
                var user = _mapper.Map<UserDataModel>(await _userService.GetUserByEmailAsync(userEmail));

                return Ok(user);
            }

            catch (Exception exception)
            {
                Log.Error(exception, "ActionName : GetUserData, ControllerName : Account");
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
