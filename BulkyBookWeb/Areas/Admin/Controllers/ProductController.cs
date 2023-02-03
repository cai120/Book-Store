using Microsoft.AspNetCore.Mvc;
using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var productList = _unitOfWork.Product.GetAll();
            return View(productList);
        }
        public async Task<IActionResult> Upsert(int? id = 0)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select( a=> new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString(),
            });
            IEnumerable<SelectListItem> CoverList = _unitOfWork.Cover.GetAll().Select( a=> new SelectListItem
            {
                Text = a.Name,
                Value = a.Id.ToString(),
            });

            if (id == null || id == 0)
            {
                var product = new Product();
                product.CategorySelectList = CategoryList;
                product.CoverSelectList = CoverList;
                return View(product);
            }
            else
            {
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(a => a.Id == id);

                if (productFromDb == null)
                    return NotFound();

                productFromDb.CategorySelectList = CategoryList;
                productFromDb.CoverSelectList = CoverList;

                return View(productFromDb);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Product product, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file!=null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"Images\Products");
                    var extention = Path.GetExtension(file.FileName);

                    if(!string.IsNullOrWhiteSpace(product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName+extention), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    product.ImageUrl=@"\Images\Products\"+fileName+extention;
                }

                if(product.Id != 0)
                {
                    _unitOfWork.Product.Modify(product);

                    TempData["Success"] = "Product Has Been Modified Successfully";

                    return RedirectToAction("Index");
                }
                else
                {
                    _unitOfWork.Product.Add(product); 

                    TempData["Success"] = "Product Has Been Created Successfully";

                    return RedirectToAction("Index");
                }
            }
            return View(product);
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category,Cover");
            return Json(new { data = productList });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return Json(new {success= false, message="Id not available."});

            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(a => a.Id == id);

            if (productFromDb == null)
                return Json(new {success= false, message="Entity not found."});

            _unitOfWork.Product.Remove(productFromDb);

            TempData["Success"] = "Product Has Been Deleted Successfully";

            return Json(new { success = true, message = "Product has been deleted." });
        }

        #endregion
    }
}
