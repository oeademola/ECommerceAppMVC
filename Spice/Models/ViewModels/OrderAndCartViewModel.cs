﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class OrderAndCartViewModel
    {
        public List<ShoppingCart> CartCollection { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
