using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Restorante.Data;
using Restorante.Models;
using Restorante.Models.OrderDetailsViewModels;

namespace Restorante.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };
            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if(cart != null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var item in detailCart.listCart)
            {
                item.MenuItem = _db.MenuItem.FirstOrDefault(m=>m.Id == item.MenuItemId);
                detailCart.OrderHeader.OrderTotal += (item.MenuItem.Price * item.Count);
                
                if(item.MenuItem.Description.Length > 100)
                {
                    item.MenuItem.Description = item.MenuItem.Description.Substring(0, 99) + "...";
                }
            }

            detailCart.OrderHeader.PickUpTime = DateTime.Now;


            return View(detailCart);
        }
    }
}