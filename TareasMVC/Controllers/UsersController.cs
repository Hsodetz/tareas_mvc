using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Data;
using TareasMVC.Models;
using TareasMVC.Services;

namespace TareasMVC.Controllers
{
	public class UsersController: Controller
	{
        private readonly UserManager<IdentityUser> userManger;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext dbContext;

        public UsersController(UserManager<IdentityUser> userManger, SignInManager<IdentityUser> signInManager, ApplicationDbContext dbContext)
		{
            this.userManger = userManger;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var user = new IdentityUser() { Email = register.Email, UserName = register.Email };

            var res = await userManger.CreateAsync(user, password: register.Password);

            if (res.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in res.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return View(register);

        }

        [AllowAnonymous]
        public IActionResult Login(string message)
        {
            if (message is not null)
            {
                ViewData["message"] = message;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var res = await signInManager.PasswordSignInAsync(login.Email, login.Password, login.Remember, lockoutOnFailure: false);

            if (res.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contrasena incorrectas!");
                return View(login);
            }

        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public ChallengeResult ExternalLogin(string supplier, string returnUrl = null)
        {
            var urlRedirection = Url.Action("RegisterExternalUser", values: new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(supplier, urlRedirection);
            return new ChallengeResult(supplier, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterExternalUser(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var message = "";

            if (remoteError is not null)
            {
                message = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("login", routeValues: new { message });
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                message = "Error cargando la data del login externo!";
                return RedirectToAction("login", routeValues: new { message });
            }

            var resExternalLogin = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            // Ya la cuenta existe
            if (resExternalLogin.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // si no existe obtenemos el email
            string email = "";

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);

            }
            else
            {
                message = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("login", routeValues: new { message });
            }

            var user = new IdentityUser { Email = email, UserName = email };

            var resUserCreated = await userManger.CreateAsync(user);

            if (!resUserCreated.Succeeded)
            {
                message = resUserCreated.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { message });
            }

            var resAddLogin = await userManger.AddLoginAsync(user, info);

            if (resAddLogin.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);
                return LocalRedirect(returnUrl);

            }

            message = "Ha ocurrido un error!";
            return RedirectToAction("login", routeValues: new { message });

        }

        [HttpGet]
        [Authorize(Roles = Constants.RoleAdmin)] // Aqui le queremos decir que solo podran acceder a esta ruta los usuarios con rol admin
        public async Task<IActionResult> ListUser(string message = null)
        {
            // aqui hacemos un select para que traiga solo un campo, osea solo la columna email, y no todos los campos de usuarios
            var users = await dbContext.Users.Select(u => new UserViewModel
            {
                Email = u.Email
            }).ToListAsync();

            var model = new ListUserViewModel();
            model.Users = users;
            model.Message = message;

            return View(model);


        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleAdmin)] // Aqui le queremos decir que solo podran acceder a esta ruta los usuarios con rol admin
        public async Task<IActionResult> TodoAdmin(string email = null)
        {
            var user = await dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user is null)
            {
                return NotFound();
            }

            // asignamos al usuario el rol admin
            await userManger.AddToRoleAsync(user, Constants.RoleAdmin);

            return RedirectToAction("ListUser", routeValues: new { message = $"Rol asignado correctamente al usuario {user.UserName}" });
        }

        [HttpPost]
        [Authorize(Roles = Constants.RoleAdmin)] // Aqui le queremos decir que solo podran acceder a esta ruta los usuarios con rol admin
        public async Task<IActionResult> RemoveAdmin(string email = null)
        {
            var user = await dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user is null)
            {
                return NotFound();
            }

            // removemos al usuario el rol admin
            await userManger.RemoveFromRoleAsync(user, Constants.RoleAdmin);

            return RedirectToAction("ListUser", routeValues: new { message = $"Rol removido correctamente al usuario {user.UserName}" });
        }

    }


}

