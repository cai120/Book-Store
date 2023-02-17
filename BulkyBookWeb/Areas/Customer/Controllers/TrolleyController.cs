using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.Enum;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class TrolleyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public TrolleyVM TrolleyVM { get; set; }
		public int OrderTotal { get; set; }

		public TrolleyController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claims = (ClaimsIdentity)User.Identity;
			var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

			TrolleyVM = new TrolleyVM
			{
				ListItems = _unitOfWork.Trolley.GetAll(a => a.ApplicationUserId == claim.Value, includeProperties: "Product"),
				OrderHeader = new()
			};

			foreach (var item in TrolleyVM.ListItems)
			{
				item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);

				TrolleyVM.OrderHeader.OrderTotal += (item.Price * item.Count);
			}

			return View(TrolleyVM);
		}

		public IActionResult Summary()
		{
			var claims = (ClaimsIdentity)User.Identity;
			var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

			TrolleyVM = new TrolleyVM
			{
				ListItems = _unitOfWork.Trolley.GetAll(a => a.ApplicationUserId == claim.Value, includeProperties: "Product"),
				OrderHeader = new()
			};

			TrolleyVM.OrderHeader.ApplicationUser = _unitOfWork.User.GetFirstOrDefault(a => a.Id == claim.Value);

			TrolleyVM.OrderHeader.Name = TrolleyVM.OrderHeader.ApplicationUser.Name;
			TrolleyVM.OrderHeader.PhoneNumber = TrolleyVM.OrderHeader.ApplicationUser.PhoneNumber;
			TrolleyVM.OrderHeader.Address = TrolleyVM.OrderHeader.ApplicationUser.StreetName;
			TrolleyVM.OrderHeader.City = TrolleyVM.OrderHeader.ApplicationUser.City;
			TrolleyVM.OrderHeader.County = TrolleyVM.OrderHeader.ApplicationUser.County;
			TrolleyVM.OrderHeader.PostCode = TrolleyVM.OrderHeader.ApplicationUser.PostCode;

			foreach (var item in TrolleyVM.ListItems)
			{
				item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);

				TrolleyVM.OrderHeader.OrderTotal += (item.Price * item.Count);
			}

			return View(TrolleyVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult SummaryPost(TrolleyVM TrolleyVM)
		{
			var claims = (ClaimsIdentity)User.Identity;
			var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

			TrolleyVM.ListItems = _unitOfWork.Trolley.GetAll(a => a.ApplicationUserId == claim.Value, includeProperties: "Product");

			TrolleyVM.OrderHeader.OrderDate = DateTime.Now;
			TrolleyVM.OrderHeader.ApplicationUserId = claim.Value;

			foreach (var item in TrolleyVM.ListItems)
			{
				item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);

				TrolleyVM.OrderHeader.OrderTotal += (item.Price * item.Count);
			}

			var user = _unitOfWork.User.GetFirstOrDefault(a => a.Id == claim.Value);

			if (!user.CompanyId.HasValue)
			{
				TrolleyVM.OrderHeader.PaymentStatus = PaymentStatus.AwaitingPayment;
				TrolleyVM.OrderHeader.OrderStatus = OrderStatus.AwaitingPayment;
			}
			else
			{
				TrolleyVM.OrderHeader.PaymentStatus = PaymentStatus.ApprovedForDelayedPayment;
				TrolleyVM.OrderHeader.OrderStatus = OrderStatus.OrderPlaced;
			}

			_unitOfWork.OrderHeader.Add(TrolleyVM.OrderHeader);

			foreach (var item in TrolleyVM.ListItems)
			{
				OrderDetail detail = new()
				{
					ProductId = item.ProductId,
					OrderId = TrolleyVM.OrderHeader.Id,
					Price = item.Price,
					Count = item.Count,
				};
				_unitOfWork.OrderDetail.Add(detail);
			}

			if (!user.CompanyId.HasValue)
			{
				var domain = "https://localhost:44304/";
				var options = new SessionCreateOptions
				{
					PaymentMethodTypes = new()
				{
					"card"
				},
					LineItems = new(),


					Mode = "payment",
					SuccessUrl = domain + $"customer/trolley/OrderConfirmed?id={TrolleyVM.OrderHeader.Id}",
					CancelUrl = domain + $"customer/trolley/index",
				};

				foreach (var item in TrolleyVM.ListItems)
				{
					options.LineItems.Add(
					  new SessionLineItemOptions
					  {
						  PriceData = new SessionLineItemPriceDataOptions
						  {
							  UnitAmount = (long)(item.Price * 100),
							  Currency = "gbp",
							  ProductData = new SessionLineItemPriceDataProductDataOptions
							  {
								  Name = item.Product.Name,
							  },
						  },
						  Quantity = item.Count,
					  });
				}

				var service = new SessionService();
				Session session = service.Create(options);
				_unitOfWork.OrderHeader.UpdateStripe(TrolleyVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}
			else
			{
				return RedirectToAction("OrderConfirmed", "Trolley", new {id=TrolleyVM.OrderHeader.Id});
			}

		}

		public IActionResult OrderConfirmed(int id)
		{
			HttpContext.Session.Clear();
			var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);
			if (orderHeader.PaymentStatus != PaymentStatus.ApprovedForDelayedPayment)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
					_unitOfWork.OrderHeader.UpdateStatus(id, OrderStatus.Paid, PaymentStatus.Paid);
			}
			var items = _unitOfWork.Trolley.GetAll(a=>a.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

			_unitOfWork.Trolley.RemoveRange(items);

			return View(id);
		}

		public IActionResult Plus(int id)
		{
			var trolley = _unitOfWork.Trolley.GetFirstOrDefault(a => a.Id == id);
			_unitOfWork.Trolley.IncrementCount(trolley, 1);
			return RedirectToAction("Index");
		}

		public IActionResult Minus(int id)
		{
			var trolley = _unitOfWork.Trolley.GetFirstOrDefault(a => a.Id == id);
			if (trolley.Count == 1)
				_unitOfWork.Trolley.Remove(trolley);
			else
				_unitOfWork.Trolley.IncrementCount(trolley, -1);

            HttpContext.Session.SetInt32(SD.SessionTrolley, _unitOfWork.Trolley.GetAll(a => a.ApplicationUserId == trolley.ApplicationUserId).ToList().Count());

            return RedirectToAction("Index");
		}

		public IActionResult Remove(int id)
		{
			var trolley = _unitOfWork.Trolley.GetFirstOrDefault(a => a.Id == id);
			_unitOfWork.Trolley.Remove(trolley);
            HttpContext.Session.SetInt32(SD.SessionTrolley, _unitOfWork.Trolley.GetAll(a => a.ApplicationUserId == trolley.ApplicationUserId).ToList().Count());
            return RedirectToAction("Index");
		}

		private double GetPriceBasedOnQuantity(int quantity, double price, double price50, double price100)
		{
			if (quantity <= 50)
			{
				return price;
			}
			else if (quantity <= 100)
			{
				return price50;
			}
			else
			{
				return price100;
			}
		}
	}
}
