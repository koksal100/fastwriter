using System.Security.Claims;
using fastwriter.Models;
using fastwriter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace fastwriter.Controllers;

[Route("Account")]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    private bool IsGoogleEnabled =>
        !string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientId"]) &&
        !string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientSecret"]);

    [HttpGet("Login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewBag.GoogleEnabled = IsGoogleEnabled;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewBag.GoogleEnabled = IsGoogleEnabled;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return RedirectToLocal(model.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Çok fazla hatalı denemeden dolayı hesabınız geçici olarak kilitlendi.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
        }

        return View(model);
    }

    [HttpGet("Register")]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewBag.GoogleEnabled = IsGoogleEnabled;
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewBag.GoogleEnabled = IsGoogleEnabled;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName.Trim()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToLocal(model.ReturnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, TranslateIdentityError(error));
        }

        return View(model);
    }

    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>Google girişini başlatır. Yalnızca ClientId/ClientSecret tanımlıysa çalışır.</summary>
    [HttpPost("ExternalLogin")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        if (!IsGoogleEnabled)
        {
            TempData["Error"] = "Google girişi yapılandırılmamış. appsettings.json içindeki Authentication:Google ayarlarını doldurun.";
            return RedirectToAction(nameof(Login));
        }

        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    [HttpGet("ExternalLoginCallback")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError is not null)
        {
            TempData["Error"] = $"Google girişi başarısız oldu: {remoteError}";
            return RedirectToAction(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            TempData["Error"] = "Google giriş bilgisi alınamadı. Lütfen tekrar deneyin.";
            return RedirectToAction(nameof(Login));
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            return RedirectToLocal(returnUrl);
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["Error"] = "Google hesabınızdan e-posta adresi alınamadı.";
            return RedirectToAction(nameof(Login));
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                TempData["Error"] = string.Join(" ", createResult.Errors.Select(TranslateIdentityError));
                return RedirectToAction(nameof(Login));
            }
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, info);

        if (!addLoginResult.Succeeded)
        {
            TempData["Error"] = string.Join(" ", addLoginResult.Errors.Select(TranslateIdentityError));
            return RedirectToAction(nameof(Login));
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToLocal(returnUrl);
    }

    [HttpGet("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    private static string TranslateIdentityError(IdentityError error)
    {
        return error.Code switch
        {
            "DuplicateUserName" or "DuplicateEmail" => "Bu e-posta adresiyle zaten bir hesap var.",
            "PasswordTooShort" => "Şifre en az 6 karakter olmalıdır.",
            "PasswordRequiresNonAlphanumeric" => "Şifre en az bir özel karakter içermelidir.",
            "PasswordRequiresDigit" => "Şifre en az bir rakam içermelidir.",
            "PasswordRequiresLower" => "Şifre en az bir küçük harf içermelidir.",
            "PasswordRequiresUpper" => "Şifre en az bir büyük harf içermelidir.",
            "InvalidEmail" or "InvalidUserName" => "Geçerli bir e-posta adresi girin.",
            _ => error.Description
        };
    }
}
