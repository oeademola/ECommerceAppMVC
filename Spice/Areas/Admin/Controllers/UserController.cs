using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Persistence;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetail.ManagerUser)]

    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;

        public UserController(IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await userRepository.GetApplicationUsersAsync(claim));
        }

        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
                return NotFound();

            var applicationUser = await userRepository.GetApplicationUserAsync(id);
            if (applicationUser == null)
                return NotFound();

            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);

            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
            
        }

        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
                return NotFound();

            var applicationUser = await userRepository.GetApplicationUserAsync(id);
            if (applicationUser == null)
                return NotFound();

            applicationUser.LockoutEnd = DateTime.Now;

            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}