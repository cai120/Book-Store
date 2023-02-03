using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            _unitOfWork= unitOfWork;
        }

        public IActionResult Index()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            TrolleyVM = new TrolleyVM
            {
                ListItems = _unitOfWork.Trolley.GetAll(a=>a.ApplicationUserId == claim.Value, includeProperties:"Product")
            };

            foreach(var item in TrolleyVM.ListItems)
            {
                item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);

                TrolleyVM.Total += (item.Price * item.Count);
			}

            return View(TrolleyVM);
        }
        
        public IActionResult Summary()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            TrolleyVM = new TrolleyVM
            {
                ListItems = _unitOfWork.Trolley.GetAll(a=>a.ApplicationUserId == claim.Value, includeProperties:"Product")
            };

            foreach(var item in TrolleyVM.ListItems)
            {
                item.Price = GetPriceBasedOnQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);

                TrolleyVM.Total += (item.Price * item.Count);
			}

            return View(TrolleyVM);
        }

        public IActionResult Plus(int id)
        {
            var trolley = _unitOfWork.Trolley.GetFirstOrDefault(a=>a.Id == id);
            _unitOfWork.Trolley.IncrementCount(trolley, 1);
            return RedirectToAction("Index");
        }
        
        public IActionResult Minus(int id)
        {
            var trolley = _unitOfWork.Trolley.GetFirstOrDefault(a=>a.Id == id);
            if (trolley.Count == 1)
                _unitOfWork.Trolley.Remove(trolley);
            else
			    _unitOfWork.Trolley.IncrementCount(trolley, -1);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var trolley = _unitOfWork.Trolley.GetFirstOrDefault(a=>a.Id==id);
            _unitOfWork.Trolley.Remove(trolley);
            return RedirectToAction("Index");
        }

        private double GetPriceBasedOnQuantity(int quantity, double price, double price50, double price100)
        {
            if(quantity <= 50)
            {
                return price;
            }
            else if(quantity <= 100)
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
