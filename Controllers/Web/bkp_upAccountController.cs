using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class bkp_upAccountController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public bkp_upAccountController(SignInManager<User> signInManager,
                             UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

}