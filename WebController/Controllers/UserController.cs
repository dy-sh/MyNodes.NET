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

        private string NO_DB_ERROR = "This functionality is not available because program does not use a database.";

        [HttpGet]
        public async Task<IActionResult> Login(string ReturnUrl)
        {
            //auto-authorizing if DB disabled or AllowFullAccessWithoutAuthorization is true
            if (SystemController.webServerRules.AllowFullAccessWithoutAuthorization
                || !SystemController.dataBaseConfig.Enable)
            {
                await Authenticate("Guest");

                if (!String.IsNullOrEmpty(ReturnUrl))
                    return Redirect(ReturnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = ReturnUrl;

            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl)
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            if (ModelState.IsValid)
            {
                User user = db.GetUser(model.Name);
                if (user != null)
                {
                    if (model.Password == user.Password)
                    {
                        await Authenticate(model.Name);

                        if (!String.IsNullOrEmpty(ReturnUrl))
                            return Redirect(ReturnUrl);

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Incorrect login or password");
            }

            ViewBag.ReturnUrl = ReturnUrl;

            return View(model);
        }




        [HttpGet]
        public IActionResult Register()
        {
            if (!SystemController.webServerRules.AllowRegistrationOfNewUsers)
                return View("Error", "Registration of new users is prohibited. Please contact administrator.");

            if (db==null)
                return View("Error", NO_DB_ERROR);


            return View(new RegisterModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!SystemController.webServerRules.AllowRegistrationOfNewUsers)
                return View("Error", "Registration of new users is prohibited. Please contact administrator.");

            if (db == null)
                return View("Error", NO_DB_ERROR);

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
            var claims = new List<Claim> { new Claim("name", userName) };

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
            if (db == null)
                return View("Error", NO_DB_ERROR);

            List<User> users = db.GetAllUsers();
            return View(users);
        }

        public IActionResult Remove(int id)
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            User user = db.GetUser(id);

            if (user == null)
                return HttpBadRequest();

            db.RemoveUser(id);

            return RedirectToAction("List");
        }

        public IActionResult RemoveAllExceptActive()
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            string name = claims.FirstOrDefault(x => x.Type == "name").Value;

            User user = db.GetUser(name);

            if (user == null)
                return HttpBadRequest();

            List<User> users = db.GetAllUsers();
            users.Remove(user);

            db.RemoveUsers(users);

            return RedirectToAction("List");
        }


        [HttpGet]
        public IActionResult Add()
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            return View(new NewUserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(NewUserModel model)
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

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

                    return RedirectToAction("List");
                }

                ModelState.AddModelError("", "User already exists");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            User user = db.GetUser(id);
            if (user == null)
                return HttpBadRequest();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User model)
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            User user = db.GetUser(model.Name);
            if (user == null)
                return HttpBadRequest();

            user.Email = model.Email;
            db.UpdateUser(user);

            return RedirectToAction("List");
        }
    }
}
