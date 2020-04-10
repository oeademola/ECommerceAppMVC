using Spice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public interface ICouponRepository
    {
        void Add(Coupon coupon);
        Task<Coupon> GetCouponAsync(int? id);
        Task<IEnumerable<Coupon>> GetCouponsAsync();
        Task<IEnumerable<Coupon>> GetActiveCoupons();
        Task<Coupon> GetCouponByName(string couponCode);
        void Remove(Coupon coupon);
    }
}