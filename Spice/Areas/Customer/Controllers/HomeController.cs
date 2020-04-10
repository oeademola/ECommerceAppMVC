using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Persistence;
using Spice.Utility;

namespace Spice.Controllers
{
    [Area ("Customer")]
    public class HomeController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly ICouponRepository couponRepository;
        private readonly IMenuItemRepository menuItemRepository;
        private readonly ICartRepository cartRepository;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ICategoryRepository categoryRepository,
            ICouponRepository couponRepository,
            IMenuItemRepository menuItemRepository,
            ICartRepository cartRepository,
            IUnitOfWork unitOfWork)
        {
            this.categoryRepository = categoryRepository;
            this.couponRepository = couponRepository;
            this.menuItemRepository = menuItemRepository;
            this.cartRepository = cartRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var indexViewModel = new IndexViewModel()
            {
                Categories = await categoryRepository.GetCategoriesAsync(),
                Coupons = await couponRepository.GetActiveCoupons(),
                MenuItems = await menuItemRepository.GetMenuItemsAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cart = await cartRepository.GetCartByUserId(claim);
                var cartCount = cart.Count();
                HttpContext.Session.SetInt32(StaticDetail.ssCartCount, cartCount);
            }

            return View(indexViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemInDb = await menuItemRepository.GetMenuItemAsync(id);

            var shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItemInDb,
                MenuItemId = menuItemInDb.Id
            };

            return View(shoppingCart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;

            if (!ModelState.IsValid)
            {
                var menuItemInDb = await menuItemRepository.GetMenuItemAsync(shoppingCart.Id);

                var cart = new ShoppingCart()
                {
                    MenuItem = menuItemInDb,
                    MenuItemId = menuItemInDb.Id
                };

                return View(cart);
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            var cartFromDb = await cartRepository.GetCartByMenuItemId(shoppingCart.ApplicationUserId, shoppingCart.MenuItemId);
            if (cartFromDb == null)
            {
                cartRepository.Add(shoppingCart);
            }
            else
            {
                cartFromDb.Count += shoppingCart.Count;
            }

            await unitOfWork.CompleteAsync();

            var sCart = await cartRepository.GetCartsAsync(shoppingCart.ApplicationUserId);
            var count = sCart.Count();
            HttpContext.Session.SetInt32(StaticDetail.ssCartCount, count);

            return RedirectToAction(nameof(Index));
        }
    }
}
