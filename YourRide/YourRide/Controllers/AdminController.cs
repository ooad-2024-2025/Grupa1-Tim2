using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourRide.Models;
using System;

namespace YourRide.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<Korisnik> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> AdminPocetna()
        {
            _logger.LogInformation("Administrator accessed User Management Dashboard.");

            var allRoles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();

            var usersByRole = new List<Tuple<string, List<Korisnik>>>();

            foreach (var role in allRoles)
            {
                var usersInThisRole = await _userManager.GetUsersInRoleAsync(role.Name);

                var korisnikUsersInRole = usersInThisRole.OfType<Korisnik>().ToList();

                if (korisnikUsersInRole.Any())
                {
                    usersByRole.Add(Tuple.Create(role.Name, korisnikUsersInRole.OrderBy(u => u.UserName).ToList()));
                }
            }


            return View(usersByRole);
        }
    }
}