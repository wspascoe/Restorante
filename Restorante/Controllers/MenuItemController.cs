using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restorante.Data;
using Restorante.Models;
using Restorante.Models.MenuItemViewModels;

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
    }
}