using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using MyNetSensors.Users;
using MyNetSensors.WebController.Code;
using MyNetSensors.WebController.ViewModels.User;

namespace MyNetSensors.WebController.Controllers
{
    public class UserController : Controller
    {
        private IUsersRepository db;

        public UserController()
        {
            db = SystemController.usersRepository;
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = db.GetUser(model.Name);
                if (user != null)
                {
                    if (model.Password == user.Password)
                    {
                        await Authenticate(model.Name);

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Incorrect login or password");
            }
            return View(model);
        }




        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = db.GetUser(model.Name);
                if (user == null)
                {
                    db.AddUser(new User
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password
                    });

                    await Authenticate(model.Name);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "User already exists");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>{new Claim("name", userName)};

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return RedirectToAction("Login", "User");
        }



        public IActionResult List()
        {
            List<User> users = db.GetAllUsers();
            return View(users);
        }

        public IActionResult Remove(int id)
        {
            User user = db.GetUser(id);

            if (user == null)
                return HttpBadRequest();

            db.RemoveUser(id);

            return RedirectToAction("List");
        }

        public IActionResult RemoveAllExceptActive()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            string name = claims.FirstOrDefault(x=>x.Type=="name").Value;

            User user = db.GetUser(name);

            if (user == null)
                return HttpBadRequest();

            List<User> users = db.GetAllUsers();
            users.Remove(user);

            db.RemoveUsers(users);

            return RedirectToAction("List");
        }
    }
}
