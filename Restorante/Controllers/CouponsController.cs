﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restorante.Data;
using Restorante.Models;
using Restorante.Utility;

namespace Restorante.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {

            return View(await _db.Coupons.ToListAsync());
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupons coupons)
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if(files[0] !=null && files[0].Length > 0)
                {
                    byte[] p1 = null;

                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupons.Picture = p1;
                }                
                _db.Coupons.Add(coupons);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }
  
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupons.SingleOrDefaultAsync(m => m.Id == id);

            if(coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupons coupons)
        {
            if(id != coupons.Id)
            {
                return NotFound();
            }

            var couponFromDB = await _db.Coupons.Where(c => c.Id == id).FirstOrDefaultAsync();

            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files[0] != null && files[0].Length > 0)
                {
                    byte[] p1 = null;

                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponFromDB.Picture = p1;
                }
                couponFromDB.MinimumAmount = coupons.MinimumAmount;
                couponFromDB.Name = coupons.Name;
                couponFromDB.Discount = coupons.Discount;
                couponFromDB.CouponType = coupons.CouponType;
                couponFromDB.isActive = coupons.isActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }//is Valid

            return View(coupons);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupons.SingleOrDefaultAsync(m => m.Id == id);

            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            var coupons = await _db.Coupons.SingleOrDefaultAsync(m => m.Id == id);
            _db.Coupons.Remove(coupons);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }//class
}