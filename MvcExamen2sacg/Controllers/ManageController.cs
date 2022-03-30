using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcExamen2sacg.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcExamen2sacg.Controllers
{
    public class ManageController : Controller
    {
        private ServiceLogicAppUsuarios service;

        public ManageController(ServiceLogicAppUsuarios service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            string token = await this.service.GetTokenAsync(username, password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
            else
            {
                //SI EL USUARIO EXISTE, ALMACENAMOS EL TOKEN EN SESSION
                HttpContext.Session.SetString("TOKEN", token);

                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                /*
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, ));
                identity.AddClaim(new Claim(ClaimTypes.Name, username));
                */
                identity.AddClaim(new Claim("USERNAME", username));
                identity.AddClaim(new Claim("PASSWORD", password));
                identity.AddClaim(new Claim("TOKEN", token));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });

                return RedirectToAction("Index","Home");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("Index", "Home");
        }
    }
}
