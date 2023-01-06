using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController  : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categoryList = _unitOfWork.Category.GetAll();
            return View(categoryList);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("CustomError", "The Name cannot match the Display Order");

            if(ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);

                TempData["Success"] = "Category Has Been Created Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null || id == 0)
                return NotFound();

            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(a=>a.Id == id);

            if(categoryFromDb == null)
                return NotFound();

            return View(categoryFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("CustomError", "The Name cannot match the Display Order");

            if(ModelState.IsValid)
            {
                _unitOfWork.Category.Modify(category);

                TempData["Success"] = "Category Has Been Modified Successfully";

                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var categoryFromDb = _unitOfWork.Category.GetFirstOrDefault(a=>a.Id == id);

            if (categoryFromDb == null)
                return NotFound();

            _unitOfWork.Category.Remove(categoryFromDb);

            TempData["Success"] = "Category Has Been Deleted Successfully";

            return RedirectToAction("Index");
        }
    }
}
