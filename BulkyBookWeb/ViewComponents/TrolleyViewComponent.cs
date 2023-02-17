using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents
{
    public class TrolleyViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrolleyViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionTrolley) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionTrolley));
                }
                else
                {
                    HttpContext.Session.SetInt32(SD.SessionTrolley, _unitOfWork.Trolley.GetAll(a => a.ApplicationUserId == claim.Value).ToList().Count());

                    return View(HttpContext.Session.GetInt32(SD.SessionTrolley));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
