using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class HinhAnhController : Controller
    {
        private readonly HinhAnhService _service;
        private readonly SanPhamService _sanPhamService;

        public HinhAnhController(HinhAnhService service, SanPhamService sanPhamService)
        {
            _service = service;
            _sanPhamService = sanPhamService;
        }

        // GET: /Admin/HinhAnh
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDHINHANH == id || x.IDSP == id).ToList();
                else
                    items = items.Where(x => x.DUONGDAN != null && x.DUONGDAN.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var totalCount = items.Count;
            var hinhAnhs = items.OrderBy(x => x.IDHINHANH).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(hinhAnhs);
        }

        // GET: /Admin/HinhAnh/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP ?? 0);
            return View(item);
        }

        // GET: /Admin/HinhAnh/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HinhAnh model)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDHINHANH = all.Any() ? all.Max(x => x.IDHINHANH) + 1 : 1;
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/HinhAnh/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HinhAnh model)
        {
            if (id != model.IDHINHANH) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/HinhAnh/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP ?? 0);
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