using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id = 0)
        {
            if (id == null || id == 0)
            {
                var company = new Company();
                return View(company);
            }
            else
            {
                var companyFromDb = _unitOfWork.Company.GetFirstOrDefault(a => a.Id == id);

                if (companyFromDb == null)
                    return NotFound();

                return View(companyFromDb);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Company company, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (company.Id != 0)
                {
                    _unitOfWork.Company.Modify(company);

                    TempData["Success"] = "Company Has Been Modified Successfully";

                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Company.Add(company);

                    TempData["Success"] = "Company Has Been Created Successfully";

                    return RedirectToAction("Index");
                }
            }
            return View(company);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return Json(new { success = false, message = "Id not available." });

            var companyFromDb = _unitOfWork.Company.GetFirstOrDefault(a => a.Id == id);

            if (companyFromDb == null)
                return Json(new { success = false, message = "Entity not found." });

            _unitOfWork.Company.Remove(companyFromDb);

            TempData["Success"] = "Company Has Been Deleted Successfully";

            return Json(new { success = true, message = "Company has been deleted." });
        }

        #endregion

    }
}
