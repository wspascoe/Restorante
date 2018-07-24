using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restorante.Data;
using Restorante.Models;
using Restorante.Models.OrderDetailsViewModels;
using Restorante.Utility;

namespace Restorante.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailViewModel OrderDetailVM = new OrderDetailViewModel()
            {
                OrderHeader = _db.OrderHeader.Where(o => o.Id == id && o.UserId == claim.Value).FirstOrDefault(),
                OrderDetail = _db.OrderDetails.Where(o=>o.OrderId == id).ToList()
            };

            return View(OrderDetailVM);
        }

        [Authorize]
        public IActionResult OrderHistory()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderDetailViewModel> orderDetailVM = new List<OrderDetailViewModel>();

            List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(u => u.UserId == claim.Value).OrderByDescending(u => u.OrderDate).ToList();

            foreach(OrderHeader item in OrderHeaderList)
            {
                OrderDetailViewModel individual = new OrderDetailViewModel
                {
                    OrderHeader = item,
                    OrderDetail = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                };
                orderDetailVM.Add(individual);
            }

            return View(orderDetailVM);
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult ManageOrder()
        {
            
            List<OrderDetailViewModel> orderDetailVM = new List<OrderDetailViewModel>();

            List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(o=>o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess)
                .OrderByDescending(u => u.PickUpTime).ToList();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailViewModel individual = new OrderDetailViewModel
                {
                    OrderHeader = item,
                    OrderDetail = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                };
                orderDetailVM.Add(individual);
            }

            return View(orderDetailVM);
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderPrepare(int orderid)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderid);
            orderHeader.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderCancel(int orderid)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderid);
            orderHeader.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderReady(int orderid)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderid);
            orderHeader.Status = SD.StatusReady;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");
        }

        public IActionResult OrderPickup(string searchEmail=null,string searchPhone=null, string searchOrder = null)
        {
            List<OrderDetailViewModel> orderDetailVM = new List<OrderDetailViewModel>();

            if (searchEmail != null || searchPhone != null || searchOrder != null)
            {
                var user = new ApplicationUser();
                List<OrderHeader> orderHeaderList = new List<OrderHeader>();

                if(searchOrder != null)
                {
                    orderHeaderList = _db.OrderHeader.Where(o => o.Id == Convert.ToInt32(searchOrder)).ToList();

                }
                else
                {
                    if(searchEmail != null)
                    {
                        user = _db.Users.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefault();

                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            user = _db.Users.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower())).FirstOrDefault();

                        }
                    }
                }

                if(user!=null || orderHeaderList.Count > 0)
                {
                    if (orderHeaderList.Count == 0)
                    {
                        orderHeaderList = _db.OrderHeader.Where(o => o.UserId == user.Id).OrderByDescending(o =>o.OrderDate).ToList();
                    }
                    foreach (OrderHeader item in orderHeaderList)
                    {
                        OrderDetailViewModel individual = new OrderDetailViewModel
                        {
                            OrderHeader = item,
                            OrderDetail = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                        };
                        orderDetailVM.Add(individual);
                    }
                }
            }
            else
            {

                List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(o => o.Status == SD.StatusReady)
                    .OrderByDescending(u => u.PickUpTime).ToList();

                foreach (OrderHeader item in OrderHeaderList)
                {
                    OrderDetailViewModel individual = new OrderDetailViewModel
                    {
                        OrderHeader = item,
                        OrderDetail = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                    };
                    orderDetailVM.Add(individual);
                }
            }
            return View(orderDetailVM);
        }

        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult OrderPickupDetails(int orderId)
        {
            OrderDetailViewModel OrderDetailsVM = new OrderDetailViewModel
            {
                OrderHeader = _db.OrderHeader.Where(o => o.Id == orderId).FirstOrDefault()
            };
            OrderDetailsVM.OrderHeader.ApplicationUser = _db.Users.Where(u => u.Id == OrderDetailsVM.OrderHeader.UserId).FirstOrDefault();
            OrderDetailsVM.OrderDetail = _db.OrderDetails.Where(o => o.OrderId == OrderDetailsVM.OrderHeader.Id).ToList();

            return View(OrderDetailsVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.AdminEndUser)]
        [ActionName("OrderPickupDetails")]
        public async Task<IActionResult> OrderPickupDetailsPost(int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();
            return RedirectToAction("OrderPickup", "Order");
        }

    }
}