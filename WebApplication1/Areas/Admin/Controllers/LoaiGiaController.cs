using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class LoaiGiaController : Controller
    {
        private readonly LoaiGiaService _service;

        public LoaiGiaController(LoaiGiaService service)
        {
            _service = service;
        }

        // GET: /Admin/LoaiGia
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x => x.TENLOAI.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         x.DIEMTHUONG.ToString().Contains(search) ||
                                         x.GIAMGIA.ToString().Contains(search) ||
                                         x.DIEM_TOITHIEU.ToString().Contains(search)).ToList();
            }
            var totalCount = items.Count;
            var loaiGias = items.OrderBy(x => x.IDLG).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(loaiGias);
        }

        // GET: /Admin/LoaiGia/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Admin/LoaiGia/Create
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiGia model)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDLG = all.Any() ? all.Max(x => x.IDLG) + 1 : 1;
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/LoaiGia/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiGia model)
        {
            if (id != model.IDLG) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/LoaiGia/Delete/5
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
