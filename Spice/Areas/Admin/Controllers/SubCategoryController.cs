using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Persistence;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetail.ManagerUser)]

    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryRepository subCategoryRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ISubCategoryRepository subCategoryRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            this.subCategoryRepository = subCategoryRepository;
            this.categoryRepository = categoryRepository;
            this.unitOfWork = unitOfWork;
        }
        
        //GET INDEX
        public async Task<IActionResult> Index()
        {
            return View(await subCategoryRepository.GetSubCategoriesAsync());
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            var viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryCollection = await categoryRepository.GetCategoriesAsync(),
                SubCategory = new SubCategory(),
                SubCategoryCollection = await subCategoryRepository.GetSubCategoryList()
            };

            return View(viewModel);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
               var isExist = await subCategoryRepository.GetCategoryByIdAndName(viewModel.SubCategory.CategoryId, viewModel.SubCategory.Name);
                if (isExist.Count() > 0)
                {
                    //Error Display Status Message 
                    StatusMessage = "Error: The Sub Category already exist under " + isExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    subCategoryRepository.Add(viewModel.SubCategory);
                    await unitOfWork.CompleteAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            var returnViewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryCollection = await categoryRepository.GetCategoriesAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryCollection = await subCategoryRepository.GetSubCategoryList(),
                StatusMessage = StatusMessage
            };

            return View(returnViewModel);
        }

        [ActionName("GetSubcategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            //subCategories = await (from subCategory in _context.SubCategories
            //                       where subCategory.CategoryId == id
            //                       select subCategory).ToListAsync();
            subCategories = await subCategoryRepository.GetSubCategoriesByCategoryId(id);
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await subCategoryRepository.GetSubCategoryAsync(id);
            if (subCategory == null)
                return NotFound();

            var viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryCollection = await categoryRepository.GetCategoriesAsync(),
                SubCategory = subCategory,
                SubCategoryCollection = await subCategoryRepository.GetSubCategoryList()
            };

            return View(viewModel);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var isExist = await subCategoryRepository.GetCategoryByIdAndName(viewModel.SubCategory.CategoryId, viewModel.SubCategory.Name);

                if (isExist.Count() > 0)
                {
                    //Error Display Status Message 
                    StatusMessage = "Error: The Sub Category already exist under " + isExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    var subCategoryInDb = await subCategoryRepository.GetSubCategoryAsync(viewModel.SubCategory.Id);
                    subCategoryInDb.Name = viewModel.SubCategory.Name;

                    await unitOfWork.CompleteAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            var returnViewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryCollection = await categoryRepository.GetCategoriesAsync(),
                SubCategory = viewModel.SubCategory,
                SubCategoryCollection = await subCategoryRepository.GetSubCategoryList(),
                StatusMessage = StatusMessage
            };

            return View(returnViewModel);
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await subCategoryRepository.GetSubCategoryAsync(id);
            if (subCategory == null)
                return NotFound();

            var viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryCollection = await categoryRepository.GetCategoriesAsync(),
                SubCategory = subCategory,
                SubCategoryCollection = await subCategoryRepository.GetSubCategoryList()
            };

            return View(viewModel);
        }

        //GET - DETAILS
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await subCategoryRepository.GetSubCategoryAsync(id);
            if (subCategory == null)
                return NotFound();

            var viewModel = new SubCategoryAndCategoryViewModel()
            {
                CategoryCollection = await categoryRepository.GetCategoriesAsync(),
                SubCategory = subCategory,
                SubCategoryCollection = await subCategoryRepository.GetSubCategoryList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var subCategory = await subCategoryRepository.GetSubCategoryAsync(id);
            if (subCategory == null)
                return NotFound();

            subCategoryRepository.Remove(subCategory);
            await unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}