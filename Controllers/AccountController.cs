using IdentityApp.Models;
using IdentityApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        private SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager,SignInManager<AppUser> signInManager)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _signInManager=signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if(user!=null)
                {
                    await _signInManager.SignOutAsync();

                    var result = await _signInManager.PasswordSignInAsync(user,model.Password,model.RememberMe,true);

                    if(result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user,null);

                        return RedirectToAction("Index","Home");
                    }
                    else if(result.IsLockedOut)
                    {
                        var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                        var timeleft=lockoutDate.Value - DateTime.UtcNow;

                        ModelState.AddModelError("", $"Your Account is locked for     {timeleft.Minutes} minutes.");
                    }
                    else
                    {
                        ModelState.AddModelError("","Your password is wrong");
                    }
                }
                else
                {
                    ModelState.AddModelError("","Not found an account with typed email adress");
                }
            }

            return View(model);
        }
    }
}