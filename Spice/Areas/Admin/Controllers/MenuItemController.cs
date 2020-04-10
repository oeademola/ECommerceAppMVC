using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Persistence;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetail.ManagerUser)]

    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMenuItemRepository menuItemRepository;
        private readonly ISubCategoryRepository subCategoryRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(IWebHostEnvironment webHostEnvironment,
            IMenuItemRepository menuItemRepository,
            ISubCategoryRepository subCategoryRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _webHostEnvironment = webHostEnvironment;
            this.menuItemRepository = menuItemRepository;
            this.subCategoryRepository = subCategoryRepository;
            this.categoryRepository = categoryRepository;
            this.unitOfWork = unitOfWork;


            MenuItemVM = new MenuItemViewModel()
            {
                Categories = categoryRepository.GetCategories(),
                MenuItem = new Models.MenuItem()
            };
        }
        public async Task<IActionResult> Index()
        {
            var menuItems = await menuItemRepository.GetMenuItemsAsync();
            return View(menuItems);
        }

        //GET -CREATE
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
                return View(MenuItemVM);

            menuItemRepository.Add(MenuItemVM.MenuItem);
            await unitOfWork.CompleteAsync();

            //Process & save image section

            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await menuItemRepository.GetMenuItemAsync(MenuItemVM.MenuItem.Id, includeRelated: false);

            if (files.Count > 0)
            {
                // Image has been sucessfully uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                // No Image or file was uploaded, so use default
                var uploads = Path.Combine(webRootPath, @"images\" + StaticDetail.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET -EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem = await menuItemRepository.GetMenuItemAsync(id);
            if (MenuItemVM.MenuItem == null)
                return NotFound();

            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategories = await subCategoryRepository.GetSubCategoriesByCategoryId(MenuItemVM.MenuItem.CategoryId);
                return View(MenuItemVM);
            }

            //Process & save image section
            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await menuItemRepository.GetMenuItemAsync(MenuItemVM.MenuItem.Id, includeRelated: false);

            if (files.Count > 0)
            {
                // New Image has been sucessfully uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                // Delete the original file or image
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);

                // Upload new image
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;
            menuItemFromDb.Spiciness = MenuItemVM.MenuItem.Spiciness;

            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET -DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem = await menuItemRepository.GetMenuItemAsync(id);

            if (MenuItemVM.MenuItem == null)
                return NotFound();

            return View(MenuItemVM);
        }

        //GET -DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem = await menuItemRepository.GetMenuItemAsync(id);

            if (MenuItemVM.MenuItem == null)
                return NotFound();

            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var menuItemFromDb = await menuItemRepository.GetMenuItemAsync(id, includeRelated: false);
            if (menuItemFromDb == null)
                return NotFound();

            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            // Delete the original file or image
            var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            menuItemRepository.Remove(menuItemFromDb);
            await unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}