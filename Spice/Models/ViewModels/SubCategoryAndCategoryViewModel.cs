﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public IEnumerable<Category> CategoryCollection { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategoryCollection { get; set; }   
        public string StatusMessage { get; set; }
    }
}
