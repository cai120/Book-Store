using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BulkyBook.DataAccess;
using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoversController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoversController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Covers
        public async Task<IActionResult> Index()
        {
            var coverList = _unitOfWork.Cover.GetAll();
            return View(coverList);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cover cover)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Cover.Add(cover);

                TempData["Success"] = "Cover Has Been Created Successfully";
                return RedirectToAction("Index");
            }
            return View(cover);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var coverFromDb = _unitOfWork.Cover.GetFirstOrDefault(a => a.Id == id);

            if (coverFromDb == null)
                return NotFound();

            return View(coverFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Cover cover)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Cover.Modify(cover);

                TempData["Success"] = "Cover Has Been Modified Successfully";

                return RedirectToAction("Index");
            }
            return View(cover);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var coverFromDb = _unitOfWork.Cover.GetFirstOrDefault(a => a.Id == id);

            if (coverFromDb == null)
                return NotFound();

            _unitOfWork.Cover.Remove(coverFromDb);

            TempData["Success"] = "Cover Has Been Deleted Successfully";

            return RedirectToAction("Index");
        }
    }
}
