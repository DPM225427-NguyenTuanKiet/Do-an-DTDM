using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class GiaiThuocTinhController : Controller
    {
        private readonly GiaiThuocTinhService _service;
        private readonly SanPhamService _sanPhamService;
        private readonly ThuocTinhService _thuocTinhService;

        public GiaiThuocTinhController(GiaiThuocTinhService service, SanPhamService sanPhamService, ThuocTinhService thuocTinhService)
        {
            _service = service;
            _sanPhamService = sanPhamService;
            _thuocTinhService = thuocTinhService;
        }

        // GET: /Admin/GiaiThuocTinh
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDSP == id || x.IDTT == id).ToList();
                else
                    items = items.Where(x => x.GIATRI.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var totalCount = items.Count;
            var giaiThuocTinhs = items.OrderBy(x => x.IDSP).ThenBy(x => x.IDTT).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.ThuocTinhs = await _thuocTinhService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(giaiThuocTinhs);
        }

        // GET: /Admin/GiaiThuocTinh/Details/5? (dùng string id vì ObjectId)
        public async Task<IActionResult> Details(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP);
            ViewBag.ThuocTinh = await _thuocTinhService.GetByIdAsync(item.IDTT);
            return View(item);
        }

        // GET: /Admin/GiaiThuocTinh/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.ThuocTinhs = await _thuocTinhService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiaiThuocTinh model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp
                var existing = await _service.GetBySanPhamAndThuocTinhAsync(model.IDSP, model.IDTT);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Cặp sản phẩm - thuộc tính này đã tồn tại.");
                    ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
                    ViewBag.ThuocTinhs = await _thuocTinhService.GetAllAsync();
                    return View(model);
                }
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.ThuocTinhs = await _thuocTinhService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/GiaiThuocTinh/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.ThuocTinhs = await _thuocTinhService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, GiaiThuocTinh model)
        {
            if (id != model.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.ThuocTinhs = await _thuocTinhService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/GiaiThuocTinh/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP);
            ViewBag.ThuocTinh = await _thuocTinhService.GetByIdAsync(item.IDTT);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}