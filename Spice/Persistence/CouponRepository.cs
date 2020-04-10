using Microsoft.EntityFrameworkCore;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext context;

        public CouponRepository(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<IEnumerable<Coupon>> GetCouponsAsync()
        {
            return await context.Coupons.ToListAsync();
        }

        public async Task<IEnumerable<Coupon>> GetActiveCoupons()
        {
            return await context.Coupons.Where(c => c.IsActive == true).ToListAsync();
        }

        public async Task<Coupon> GetCouponAsync(int? id)
        {
            return await context.Coupons.SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Coupon> GetCouponByName(string couponCode)
        {
            return await context.Coupons.Where(c => c.Name.ToLower() == couponCode.ToLower()).FirstOrDefaultAsync();
        }

        public void Add(Coupon coupon)
        {
            context.Coupons.Add(coupon);
        }

        public void Remove(Coupon coupon)
        {
            context.Remove(coupon);
        }
    }
}
