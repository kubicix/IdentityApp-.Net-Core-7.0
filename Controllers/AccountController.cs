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

        private IEmailSender _emailSender;

        public AccountController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager,SignInManager<AppUser> signInManager,IEmailSender emailSender)
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _signInManager=signInManager;
            _emailSender=emailSender;
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
    if (ModelState.IsValid)
    {
        var user = new AppUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new { user.Id, token });

                await _emailSender.SendEmailAsync(user.Email, "Hesap Onayı", $"Lütfen email hesabınızı onaylamak için linke <a href='http://localhost:5157{url}'>tıklayınız.</a>");

                TempData["message"] = "Email hesabınızdaki onay mailini tıklayınız.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                TempData["message"] = TempData["EmailError"] ?? "E-posta gönderiminde bir sorun oluştu.";
                return RedirectToAction("Error"); // Bir hata sayfasına yönlendirebilirsiniz
            }
        }

        foreach (IdentityError err in result.Errors)
        {
            ModelState.AddModelError("", err.Description);
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

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

    }
}