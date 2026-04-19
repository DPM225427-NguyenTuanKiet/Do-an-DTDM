using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class KhuyenMaiController : Controller
    {
        private readonly KhuyenMaiService _service;

        public KhuyenMaiController(KhuyenMaiService service)
        {
            _service = service;
        }

        // GET: /Admin/KhuyenMai
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x => (x.TENKM != null && x.TENKM.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                         (x.PHANTRAMGIAM != null && x.PHANTRAMGIAM.ToString().Contains(search))).ToList();
            }
            var totalCount = items.Count;
            var khuyenMais = items.OrderByDescending(x => x.NGAYBATDAU).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(khuyenMais);
        }

        // GET: /Admin/KhuyenMai/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Admin/KhuyenMai/Create
        public IActionResult Create() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhuyenMai model)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDKM = all.Any() ? all.Max(x => x.IDKM) + 1 : 1;
                await _service.CreateAsync(model);
                // Chuyển sang trang gán sản phẩm cho khuyến mãi vừa tạo
                return RedirectToAction("Create", "SanPhamKhuyenMai", new { area = "Admin", kmId = model.IDKM });
            }
            return View(model);
        }

        // GET: /Admin/KhuyenMai/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhuyenMai model)
        {
            if (id != model.IDKM) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/KhuyenMai/Delete/5
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