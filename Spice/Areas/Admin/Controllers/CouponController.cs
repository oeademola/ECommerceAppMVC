using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Persistence;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetail.ManagerUser)]

    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ICouponRepository couponRepository;
        private readonly IUnitOfWork unitOfWork;

        public CouponController(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            this.couponRepository = couponRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            return View(await couponRepository.GetCouponsAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (!ModelState.IsValid)
                return View(coupon);

            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                byte[] picture = null;
                using (var filesStream = files[0].OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        filesStream.CopyTo(memoryStream);
                        picture = memoryStream.ToArray();
                    }
                }
                coupon.Picture = picture;
            }
            couponRepository.Add(coupon);
            await unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));

        }

        //GET -EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await couponRepository.GetCouponAsync(id);
            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupon)
        {
            if (!ModelState.IsValid)
                return View(coupon);

            var couponInDb = await couponRepository.GetCouponAsync(coupon.Id);
            if (couponInDb == null)
                return NotFound();

            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                byte[] picture = null;
                using (var filesStream = files[0].OpenReadStream())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        filesStream.CopyTo(memoryStream);
                        picture = memoryStream.ToArray();
                    }
                }
                couponInDb.Picture = picture;
            }

            couponInDb.Name = coupon.Name;
            couponInDb.CouponType = coupon.CouponType;
            couponInDb.Discount = coupon.Discount;
            couponInDb.MinimumAmount = coupon.MinimumAmount;
            couponInDb.IsActive = coupon.IsActive;

            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await couponRepository.GetCouponAsync(id);
            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        //GET -DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = await couponRepository.GetCouponAsync(id);
            if (coupon == null)
                return NotFound();

            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await couponRepository.GetCouponAsync(id);
            if (coupon == null)
                return NotFound();

            couponRepository.Remove(coupon);
            await unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}