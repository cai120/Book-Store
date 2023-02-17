using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,Cover");

            return View(products);
        }

        public IActionResult Details(int productId)
        {
            Trolley trolley = new Trolley
            {
                Product = _unitOfWork.Product.GetFirstOrDefault(a => a.Id == productId, includeProperties: "Category,Cover"),
                ProductId = productId,
                Count = 1
            };
            return View(trolley);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Trolley trolley)
        {
            var claims = (ClaimsIdentity)User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            var existingTrolleyItems = _unitOfWork.Trolley.GetFirstOrDefault(a => a.ApplicationUserId == claim.Value && a.ProductId == trolley.ProductId);

            if (existingTrolleyItems != null)
            {
                existingTrolleyItems.Count = (existingTrolleyItems.Count + trolley.Count);

                _unitOfWork.Trolley.Modify(existingTrolleyItems);
            }
            else
            {
                trolley.ApplicationUserId = claim.Value;
                _unitOfWork.Trolley.Add(trolley);
                HttpContext.Session.SetInt32(SD.SessionTrolley, _unitOfWork.Trolley.GetAll(a => a.ApplicationUserId == claim.Value).ToList().Count());

            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}