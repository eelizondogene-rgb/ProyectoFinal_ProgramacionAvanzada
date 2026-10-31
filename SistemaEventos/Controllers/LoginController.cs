
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaEventos.Application.DTOs;
using SistemaEventos.Application.Interfaces;
using System.Security.Claims;

namespace SistemaEventos.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio;

        public LoginController(IUsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginDto login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var usuario = await _usuarioServicio
                .ValidarCredencialesAsync(login);

            if (usuario == null)
            {
                ModelState.AddModelError(
                    "", "Correo o contraseña incorrectos.");

                return View(login);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,
                    usuario.Id.ToString()),

                new Claim(ClaimTypes.Name,
                    usuario.Nombre + " " + usuario.Apellido),

                new Claim(ClaimTypes.Email,
                    usuario.Correo),

                new Claim(ClaimTypes.Role,
                    usuario.Rol)
            };

            var identidad = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identidad);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(
            RegistroUsuarioDto registro)
        {
            if (!ModelState.IsValid)
                return View(registro);

            var usuario = await _usuarioServicio
                .RegistrarAsync(registro);

            if (usuario == null)
            {
                ModelState.AddModelError(
                    "", "No se pudo registrar. Verifique los datos o el correo.");

                return View(registro);
            }

            TempData["Mensaje"] =
                "Registro exitoso. Ya puede iniciar sesión.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
