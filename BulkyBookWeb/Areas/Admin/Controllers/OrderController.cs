using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
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

		#region API Calls
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			var orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(a=>a.OrderStatus == )
                    break;
				case "inprocess":
                    inprocess = "active text-white";
                    break;
                case "completed":
                    completed = "active text-white";
                    break;
                default:
                    all = "active text-white";
                    break;

            }

            return Json(new {data = orderHeaders});
		}
		#endregion
	}
}
