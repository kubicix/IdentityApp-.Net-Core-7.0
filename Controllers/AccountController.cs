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

                    if(!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("","Conirm your account with email before signing in");
                        return View(model);
                    }

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

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new AppUser {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName=model.FullName
                };

                IdentityResult result = await _userManager.CreateAsync(user,model.Password);
                if(result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var url = Url.Action("ConfirmEmail","Account",new {user.Id,token});

                    // email

                    TempData["message"]="Check your email account for confirmation link";
                    return RedirectToAction("Login","Account");
                }
                foreach(IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string Id,string token)
        {
            if(Id == null || token == null)
            {
                TempData["message"]="Invalid token information";
                return RedirectToAction("Login","Account");
            }

            var user = await _userManager.FindByIdAsync(Id);

            if(user!=null)
            {
                var result = await _userManager.ConfirmEmailAsync(user,token);

                if(result.Succeeded)
                {
                    TempData["message"]="Your account is confirmed";
                    return View();
                }
            }
            TempData["message"]="We didn't find any user";
            return View();
        }

    }
}