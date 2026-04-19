using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DanhMucController : Controller
    {
        private readonly DanhMucService _service;
        public DanhMucController(DanhMucService service) => _service = service;

        // GET: /Admin/DanhMuc
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
                items = items.Where(x => x.TENDANHMUC.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            ViewBag.Search = search;

            var totalCount = items.Count;
            var danhMucs = items.OrderBy(x => x.IDDM).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(danhMucs);
        }

        // GET: /Admin/DanhMuc/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Admin/DanhMuc/Create
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DanhMuc model)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDDM = all.Any() ? all.Max(x => x.IDDM) + 1 : 1;
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/DanhMuc/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DanhMuc model)
        {
            if (id != model.IDDM) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/DanhMuc/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}