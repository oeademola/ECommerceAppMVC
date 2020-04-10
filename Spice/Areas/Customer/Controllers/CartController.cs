using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Persistence;
using Spice.Utility;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartRepository cartRepository;
        private readonly IMenuItemRepository menuItemRepository;
        private readonly ICouponRepository couponRepository;
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;

        [BindProperty]
        public OrderAndCartViewModel orderCart { get; set; }

        public CartController(ApplicationDbContext context,
            ICartRepository cartRepository,
            IMenuItemRepository menuItemRepository,
            ICouponRepository couponRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            this.cartRepository = cartRepository;
            this.menuItemRepository = menuItemRepository;
            this.couponRepository = couponRepository;
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            orderCart = new OrderAndCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };

            orderCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = await cartRepository.GetCartByUserId(claim);
            if (cart != null)
            {
                orderCart.CartCollection = cart.ToList();
            }

            foreach (var list in orderCart.CartCollection)
            {
                list.MenuItem = await menuItemRepository.GetMenuItemAsync(list.MenuItemId, includeRelated: false);

                if (list.MenuItem.Description == null)
                {
                    list.MenuItem.Description = "<p></p>";
                }
                orderCart.OrderHeader.OrderTotal =  Math.Round((orderCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count)),2);

                list.MenuItem.Description = StaticDetail.ConvertToRawHtml(list.MenuItem.Description);
                if (list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }

            orderCart.OrderHeader.OrderTotalOriginal = orderCart.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(StaticDetail.ssCouponCode) != null)
            {
                orderCart.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetail.ssCouponCode);
                var couponFromDb = await couponRepository.GetCouponByName(orderCart.OrderHeader.CouponCode);
                orderCart.OrderHeader.OrderTotal = StaticDetail.DiscountPrice(couponFromDb, orderCart.OrderHeader.OrderTotalOriginal);
            }
        

            return View(orderCart);
        }

        public async Task<IActionResult> Summary()
        {
            orderCart = new OrderAndCartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
            };

            orderCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var applicationUser = await userRepository.GetApplicationUserById(claim);

            var cart = await cartRepository.GetCartByUserId(claim); 
            if (cart != null)
            {
                orderCart.CartCollection = cart.ToList();
            }

            foreach (var list in orderCart.CartCollection)
            {
                list.MenuItem = await menuItemRepository.GetMenuItemAsync(list.MenuItemId, includeRelated: false);
                orderCart.OrderHeader.OrderTotal = Math.Round((orderCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count)), 2);
            }
            orderCart.OrderHeader.OrderTotalOriginal = orderCart.OrderHeader.OrderTotal;
            orderCart.OrderHeader.PickupName = applicationUser.Name;
            orderCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            orderCart.OrderHeader.PickupTime = DateTime.Now;


            if (HttpContext.Session.GetString(StaticDetail.ssCouponCode) != null)
            {
                orderCart.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetail.ssCouponCode);
                var couponFromDb = await couponRepository.GetCouponByName(orderCart.OrderHeader.CouponCode);
                orderCart.OrderHeader.OrderTotal = StaticDetail.DiscountPrice(couponFromDb, orderCart.OrderHeader.OrderTotalOriginal);
            }


            return View(orderCart);
        }

        public IActionResult AddCoupon()
        {
            if (orderCart.OrderHeader.CouponCode == null)
            {
                orderCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(StaticDetail.ssCouponCode, orderCart.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(StaticDetail.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            //TODO : Move Query to repository.
            var cart = await _context.ShoppingCarts.SingleOrDefaultAsync(m => m.Id == cartId);
            cart.Count += 1;

            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            //TODO : Move Query to repository.
            var cart = await _context.ShoppingCarts.SingleOrDefaultAsync(m => m.Id == cartId);
            if (cart.Count == 1)
            {
                cartRepository.Remove(cart);
                await unitOfWork.CompleteAsync();

                var count = await cartRepository.GetCartsAsync(cart.ApplicationUserId);
                var cnt = count.Count();
                HttpContext.Session.SetInt32(StaticDetail.ssCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await cartRepository.GetCartAsync(cartId);

            cartRepository.Remove(cart);
            await unitOfWork.CompleteAsync();

            var count = await cartRepository.GetCartsAsync(cart.ApplicationUserId);
            var cnt = count.Count();
            HttpContext.Session.SetInt32(StaticDetail.ssCartCount, cnt);

            return RedirectToAction(nameof(Index));
        }
    }
}