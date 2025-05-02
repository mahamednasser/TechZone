using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;
using TechZone.DataAccess.Migrations;
using TechZone.DataAccess.Repository.IRepository;
using TechZone.Models.Models;
using TechZone.Models.ViewModels;
using TechZone.Utility;

namespace TechZoneWeb.Areas.Supervisor.Controllers
{
    [Area("Supervisor")]
    [Authorize]
    public class OrderManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }
        public IActionResult GetAll(string? status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.RoleAdmin) || User.IsInRole(SD.RoleEmployee)) 
            {
                orderHeaders  = _unitOfWork.OrderHeader.GetAll(IncludeProp: "ApplicationUser").ToList();

            }
            else 
            {
                ClaimsIdentity claimsIdentity= (ClaimsIdentity)User.Identity;
                var userid = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();

                orderHeaders= _unitOfWork.OrderHeader.GetAll(x=>x.ApplicationId == userid && x.OrderStatus!=SD.status_Pending, IncludeProp: "ApplicationUser").ToList();
            }
            
            

            if (!string.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "inprocess":
                        orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.status_proccessing);
                        break;
                    case "approved":
                        orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.status_Approved);
                        break;
                    case "completed":
                        orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.status_Shipped);
                        break;
                    default:
                        break;
                }
            }
            return View("GetAllOrder",orderHeaders);
        }

        public IActionResult Details(int id) 
        {
            orderViewModel orderFrmDb = new orderViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetById(x => x.Id == id, IncludeProp: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id ,IncludeProp:"Product")
            };
            return View("Details",orderFrmDb);
        }

        [Authorize(Roles =SD.RoleAdmin + "," + SD.RoleEmployee)]
        public IActionResult UpdateOrderDetails(orderViewModel orderRequest) 
        {

            OrderHeader orderFrmDb = _unitOfWork.OrderHeader.GetById(x => x.Id == orderRequest.OrderHeader.Id);
            orderFrmDb.Name=orderRequest.OrderHeader.Name;
            orderFrmDb.PhoneNumber=orderRequest.OrderHeader.PhoneNumber;
            orderFrmDb.StreetAddress=orderRequest.OrderHeader.StreetAddress;
            orderFrmDb.State=orderRequest.OrderHeader.State;
            orderFrmDb.City=orderRequest.OrderHeader.City;

            if (!string.IsNullOrEmpty(orderRequest.OrderHeader.Carrier))
            {
                orderFrmDb.Carrier=orderRequest.OrderHeader.Carrier;
            }

            if (!string.IsNullOrEmpty(orderRequest.OrderHeader.TrackingNumber))
            {
                orderFrmDb.TrackingNumber=orderRequest.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderFrmDb);
            _unitOfWork.save();
            TempData["success"] = "Order Details Updated Successfully";


                return RedirectToAction("Details", new {id=orderFrmDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]

        public IActionResult StartProcessing(shoppingCartViewModel shoppingCart)
        { 
            _unitOfWork.OrderHeader.UpdateStatus(shoppingCart.orderHeader.Id,SD.status_proccessing);
            _unitOfWork.save();
            TempData["success"] = "Status Updated Successfully";
            return RedirectToAction("Details", new {id=shoppingCart.orderHeader.Id});   
        }
        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]

        public IActionResult ShipOrder(shoppingCartViewModel shoppingCart)
        {
            if(shoppingCart.orderHeader.Carrier==null || shoppingCart.orderHeader.TrackingNumber == null)
            {
                if(shoppingCart.orderHeader.Carrier == null)
                {
                    TempData["error"] = "Please Enter The Carrier";
                }
                if(shoppingCart.orderHeader.TrackingNumber == null)
                {
                    TempData["error"] = "Please Enter The Tracking Number";

                }
                return RedirectToAction("Details", new { id = shoppingCart.orderHeader.Id });

            }
            else
            {
                    OrderHeader order = _unitOfWork.OrderHeader.GetById(x => x.Id == shoppingCart.orderHeader.Id);
                order.ShippingDate = DateTime.Now;
            
                order.TrackingNumber = shoppingCart.orderHeader.TrackingNumber;
                order.Carrier=shoppingCart.orderHeader.Carrier;
                order.OrderStatus = SD.status_Shipped;
                _unitOfWork.OrderHeader.Update(order);
                _unitOfWork.save();
                TempData["success"] = "Order Shipped Successfully";
                return RedirectToAction("Details", new {id=shoppingCart.orderHeader.Id});


            }

        }
        [HttpPost]
        [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
        public IActionResult cancelOrder(shoppingCartViewModel shoppingCart)
        {
            var orderHeader= _unitOfWork.OrderHeader.GetById(x=>x.Id==shoppingCart.orderHeader.Id);
            if (orderHeader.PaymentStatus == SD.Payment_status_Approved)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentintentId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.status_Cancelled, SD.status_Refunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id,SD.status_Cancelled,SD.status_Cancelled);
            }
            _unitOfWork.save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction("Details", new { id = shoppingCart.orderHeader.Id });

        }
    }
}
