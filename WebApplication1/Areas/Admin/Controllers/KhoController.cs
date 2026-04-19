using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class KhoController : Controller
    {
        private readonly KhoService _service;

        public KhoController(KhoService service)
        {
            _service = service;
        }

        // GET: /Admin/Kho
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x => (x.TENKHO != null && x.TENKHO.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                         (x.DIACHI != null && x.DIACHI.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            var totalCount = items.Count;
            var khoList = items.OrderBy(x => x.IDKHO).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(khoList);
        }

        // GET: /Admin/Kho/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Admin/Kho/Create
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Kho model)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDKHO = all.Any() ? all.Max(x => x.IDKHO) + 1 : 1;
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/Kho/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Kho model)
        {
            if (id != model.IDKHO) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/Kho/Delete/5
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