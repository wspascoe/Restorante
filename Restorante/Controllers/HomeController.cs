﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restorante.Data;
using Restorante.Models;
using Restorante.Models.HomeViewModel;

namespace Restorante.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(m=> m.Category).Include(m=>m.SubCategory).ToListAsync(),
                Category = _db.Category.OrderBy(c=>c.DisplayOrder),
                Coupons = _db.Coupons.Where(c=>c.isActive == true).ToList()
            };

            return View(IndexVM);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
