﻿using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public static class StaticDetail
    {
        public const string DefaultFoodImage = "default-image.png";

        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";
        public const string ssCartCount = "ssCartCount";
        public const string ssCouponCode = "ssCouponCode";


        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountPrice(Coupon couponFromDb, double OriginalOrderTotal)
        {
            if (couponFromDb == null)
                return OriginalOrderTotal;

            if (couponFromDb.MinimumAmount > OriginalOrderTotal)
                return OriginalOrderTotal;

            if(Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Dollar)
            {
                return Math.Round(OriginalOrderTotal - couponFromDb.Discount, 2);
            }

            if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent)
            {
                return Math.Round((OriginalOrderTotal * couponFromDb.Discount)/100, 2);
            }
            return OriginalOrderTotal;
        }
    }
}
