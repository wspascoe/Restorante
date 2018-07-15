using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Restorante.Data;
using Restorante.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restorante.Controllers.API
{
    [Route("api/[controller]")]
    public class CouponApiController : Controller
    {
        private ApplicationDbContext _db;

        public CouponApiController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get(double OrderTotal, string CouponCode=null)
        {
            var rtn = "";
            if(CouponCode == null)
            {
                rtn = OrderTotal + ":E";
                return Ok(rtn);
            }

            var couponFromDB = _db.Coupons.Where(c => c.Name == CouponCode).FirstOrDefault();

            if(couponFromDB == null)
            {
                rtn = OrderTotal + ":E";
                return Ok(rtn);
            }
            if(couponFromDB.MinimumAmount > OrderTotal)
            {
                rtn = OrderTotal + ":E";
                return Ok(rtn);
            }

            if(Convert.ToInt32(couponFromDB.CouponType) == (int)Coupons.ECouponType.Dollar)
            {
                OrderTotal = OrderTotal - couponFromDB.Discount;

                rtn = OrderTotal + ":S";
                return Ok(rtn);
            }
            else
            {
                if (Convert.ToInt32(couponFromDB.CouponType) == (int)Coupons.ECouponType.Percent)
                {
                    OrderTotal = OrderTotal - (OrderTotal * couponFromDB.Discount / 100);

                    rtn = OrderTotal + ":S";
                    return Ok(rtn);
                }
            }
            return Ok();
        }

        
    }
}
