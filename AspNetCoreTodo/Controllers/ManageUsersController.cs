using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTodo.Controllers;

[Authorize(Roles = Constants.AdministratorRole)]
public class ManageUsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public ManageUsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var admins = (await _userManager.GetUsersInRoleAsync(Constants.AdministratorRole)).ToArray();

        var everyone = await _userManager.Users.ToArrayAsync();
        var model = new ManageUsersViewModel
        {
            Administrators = admins,
            Everyone = everyone
        };

        return View(model);
    }
}