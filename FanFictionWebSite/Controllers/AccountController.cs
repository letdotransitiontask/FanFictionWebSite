using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FanFictionWebSite.Entities;
using FanFictionWebSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FanFictionWebSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IStringLocalizer<AccountController> localizer;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IStringLocalizer<AccountController> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.localizer = localizer;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["Title"] = localizer["LoginTitle"];
            return View(new LoginInputModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {

            if (ModelState.IsValid)
            {
                var result =
                    await signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Login error");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterInputModel model)
        {

            ViewData["Title"] = localizer.GetString("RegisterTitle");
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (userManager.Users.Count() == 1)
                    {
                        await userManager.AddToRoleAsync(user, "admin");
                    }
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        private Stack<User> GetUsersToModify(UsersListViewModel viewModel)
        {
            Stack<User> users = new Stack<User>();
            User[] usersArray = userManager.Users.ToArray();
            for (int i = 0; i < viewModel.UserFilters.Count; i++)
            {
                if (viewModel.UserFilters[i].IsSelected)
                {
                    users.Push(usersArray[i]);
                }
            }
            return users;
        }

        [Authorize(Roles = "admin")]
        public IActionResult Admin()
        {
            UsersListViewModel viewModel = new UsersListViewModel();
            viewModel.UserFilters = new List<UserFilter>();
            foreach (var u in userManager.Users)
            {
                viewModel.UserFilters.Add(new UserFilter { UserToFilter = u, IsSelected = false });
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ControlUser(UsersListViewModel viewModel, string block, string remove, string logout, string unblock, string makeAdmin, string makeUsualUser)
        {
            if (!string.IsNullOrEmpty(logout))
            {
                await signInManager.SignOutAsync();
            }
            else
            {
                Stack<User> usersToModify = GetUsersToModify(viewModel);
                Boolean me = false;
                while (usersToModify.Count != 0)
                {
                    var user = usersToModify.Pop();

                    if (!string.IsNullOrEmpty(remove))
                    {
                        if (user.UserName == User.Identity.Name) me = true;
                        user.LockoutEnd = DateTime.Now.AddYears(200);
                        await userManager.UpdateAsync(user);
                        await userManager.UpdateSecurityStampAsync(user);
                        var result = await userManager.DeleteAsync(user);
                    }
                    else if (!string.IsNullOrEmpty(block))
                    {
                        if (!user.Blocked)
                        {
                            if (user.UserName == User.Identity.Name) me = true;
                            user.Blocked = true;
                            user.LockoutEnd = DateTime.Now.AddYears(200);
                            await userManager.UpdateAsync(user);
                            await userManager.UpdateSecurityStampAsync(user);
                        }
                    }
                    else if (!string.IsNullOrEmpty(unblock))
                    {
                        if (user.Blocked)
                        {
                            user.Blocked = false;
                            user.LockoutEnd = DateTime.Now;
                            await userManager.UpdateAsync(user);
                            await userManager.UpdateSecurityStampAsync(user);
                        }
                    }
                    else if (!string.IsNullOrEmpty(makeAdmin))
                    {
                        if (!await userManager.IsInRoleAsync(user, "admin"))
                        {
                            await userManager.AddToRoleAsync(user, "admin");
                        }
                    }
                    else if (!string.IsNullOrEmpty(makeUsualUser))
                    {
                        if (await userManager.IsInRoleAsync(user, "admin"))
                        {
                            await userManager.RemoveFromRoleAsync(user, "admin");
                        }
                    }
                }
                if (me) await signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
