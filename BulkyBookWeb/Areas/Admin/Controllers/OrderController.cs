using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}
		
		public IActionResult Details(int id)
		{
			var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(a=>a.Id == id, includeProperties: "ApplicationUser");
			var orderDetail = _unitOfWork.OrderDetail.GetAll(a=>a.OrderId == id, includeProperties: "Product");

			var viewModel = new OrderVM();
			viewModel.OrderHeader = orderHeader;
			viewModel.OrderDetails = orderDetail;

			return View(viewModel);
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails(OrderVM viewModel)
		{
			var entity = _unitOfWork.OrderHeader.GetFirstOrDefault(a=>a.Id == viewModel.OrderHeader.Id);
			if (entity != null)
			{
				entity.Name = viewModel.OrderHeader.Name;
				entity.PhoneNumber = viewModel.OrderHeader.PhoneNumber;
				entity.Address = viewModel.OrderHeader.Address;
				entity.City = viewModel.OrderHeader.City;
				entity.County = viewModel.OrderHeader.County;
				entity.PostCode = viewModel.OrderHeader.PostCode;
				if(viewModel.OrderHeader.Carrier != null)
					entity.Carrier = viewModel.OrderHeader.Carrier;
				if(viewModel.OrderHeader.TrackingNumber != null)
					entity.TrackingNumber = viewModel.OrderHeader.TrackingNumber;
			}
			_unitOfWork.OrderHeader.Modify(entity);
			TempData["Success"] = "Order has been updated successfully.";
			return RedirectToAction("Index");
		}
		
		[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing(OrderVM viewModel)
		{
			_unitOfWork.OrderHeader.UpdateStatus(viewModel.OrderHeader.Id, BulkyBook.Models.Enum.OrderStatus.OrderPlaced);
			TempData["Success"] = "Order has been updated successfully.";
			return RedirectToAction("Index");
		}

		#region API Calls
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;
			if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
				orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
			}
			else
			{
				var claims = (ClaimsIdentity)User.Identity;
				var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
				orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
			}
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(a => a.OrderStatus == BulkyBook.Models.Enum.OrderStatus.OrderPlaced);
                    break;
				case "inprocess":
                    orderHeaders = orderHeaders.Where(a => a.OrderStatus == BulkyBook.Models.Enum.OrderStatus.AwaitingPayment);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(a => a.OrderStatus == BulkyBook.Models.Enum.OrderStatus.Shipped);
                    break;
                default:
                    break;

            }

            return Json(new {data = orderHeaders});
		}
		#endregion
	}
}
