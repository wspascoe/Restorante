using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var MenuItemFromDB = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m=> m.Id == id).FirstOrDefaultAsync();

            ShoppingCart CartObj = new ShoppingCart()
            {
                MenuItem = MenuItemFromDB,
                MenuItemId = id
            };

            return View(CartObj);

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart Cart)
        {
            Cart.Id = 0;

            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                Cart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDB = await _db.ShoppingCart.Where(c => c.ApplicationUserId == Cart.ApplicationUserId 
                            && c.MenuItemId == Cart.MenuItemId).FirstOrDefaultAsync();

                if(cartFromDB == null)
                {
                    _db.ShoppingCart.Add(Cart);
                }
                else
                {
                    cartFromDB.Count = cartFromDB.Count + Cart.Count;
                }

                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(c=>c.ApplicationUserId == Cart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("CartCount", count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var MenuItemFromDB = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == Cart.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart CartObj = new ShoppingCart()
                {
                    MenuItem = MenuItemFromDB,
                    MenuItemId = MenuItemFromDB.Id
                };

                return View(CartObj);
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
