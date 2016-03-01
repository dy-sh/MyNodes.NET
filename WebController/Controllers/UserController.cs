/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using MyNodes.Users;
using MyNodes.WebController.Code;
using MyNodes.WebController.ViewModels.User;

namespace MyNodes.WebController.Controllers
{


    public class UserController : Controller
    {
        private IUsersRepository db;

        public UserController()
        {
            db = SystemController.usersDb;
        }

        private string NO_DB_ERROR = "This functionality is not available because program does not use a database.";

        [HttpGet]
        public async Task<IActionResult> Login(string ReturnUrl)
        {
            //auto-authorizing if DB disabled or AllowFullAccessWithoutAuthorization is true
            if (SystemController.webServerRules.AllowFullAccessWithoutAuthorization
                || !SystemController.dataBaseConfig.Enable)
            {
                User user = new User
                {
                    Name = "Admin",
                    Password = "Admin"
                };

                user.SetClaims(Users.User.GetAllClaims());

                await Authenticate(user);

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
                        await Authenticate(user);

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

            if (db == null)
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
                    user = new User()
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                    };

                    db.AddUser(user);

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "User already exists");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("Name", user.Name));

            if (user.GetClaims() != null)
                foreach (var claim in user.GetClaims())
                {
                    claims.Add(new Claim(claim, ""));
                }

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return RedirectToAction("Login", "User");
        }



        public IActionResult AccessDenied()
        {
            return View("Error", "Access Denied");
        }




        [Authorize(UserClaims.UsersObserver)]

        public IActionResult List()
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            List<User> users = db.GetAllUsers();
            return View(users);
        }


        [Authorize(UserClaims.UsersEditor)]

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


        [Authorize(UserClaims.UsersEditor)]
        public IActionResult RemoveAllExceptActive()
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            string name = claims.FirstOrDefault(x => x.Type == "name").Value;

            List<User> users = db.GetAllUsers();
            User user = users.FirstOrDefault(x => x.Name == name);

            if (user == null)
                return HttpBadRequest();

            users.Remove(user);

            db.RemoveUsers(users);

            return RedirectToAction("List");
        }


        [Authorize(UserClaims.UsersEditor)]

        [HttpGet]
        public IActionResult Add()
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            return View(new NewUserModel());
        }

        [Authorize(UserClaims.UsersEditor)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(NewUserModel model)
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


        [Authorize(UserClaims.UsersEditor)]

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


        [Authorize(UserClaims.UsersEditor)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User model)
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



        [Authorize(UserClaims.UsersEditor)]

        [HttpGet]
        public IActionResult Permissions(int id)
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            User user = db.GetUser(id);
            if (user == null)
                return HttpBadRequest();

            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.Name;

            return View(user.GetUserPermissions());
        }


        [Authorize(UserClaims.UsersEditor)]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Permissions(UserPermissions model,int userId)
        {
            if (db == null)
                return View("Error", NO_DB_ERROR);

            User user = db.GetUser(userId);
            if (user == null)
                return HttpBadRequest();

            user.SetClaims(model);
            db.UpdateUser(user);

            return RedirectToAction("List");
        }
    }
}
