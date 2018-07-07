using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restorante.Data;
using Restorante.Models;
using Restorante.Models.MenuItemViewModels;
using Restorante.Utility;

namespace Restorante.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IHostingEnvironment hostingEnviroment)
        {
            _db = db;
            _hostingEnvironment = hostingEnviroment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category.ToList(),
                MenuItem = new MenuItem()
            };

        }

        public async Task<IActionResult> Index()
        {
            var menuItems = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory);

            return View(await menuItems.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
          //  MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if(!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }
            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _db.MenuItem.Find(MenuItemVM.MenuItem.Id);

            if(files[0]!=null && files[0].Length > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public JsonResult GetSubCategory(int categoryId)
        {
            List<SubCategory> subCategoryList = new List<SubCategory>();

            subCategoryList = (from subCategory in _db.SubCategory
                               where subCategory.CategoryId == categoryId
                               select subCategory).ToList();

            return Json(new SelectList(subCategoryList, "Id", "Name"));
        }
    }
}