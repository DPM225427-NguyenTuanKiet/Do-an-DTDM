using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ThongSoKyThuatController : Controller
    {
        private readonly ThongSoKyThuatService _service;
        private readonly SanPhamService _sanPhamService;

        public ThongSoKyThuatController(ThongSoKyThuatService service, SanPhamService sanPhamService)
        {
            _service = service;
            _sanPhamService = sanPhamService;
        }

        // GET: /Admin/ThongSoKyThuat
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDTS == id || x.IDSP == id).ToList();
                else
                    items = items.Where(x => (x.LOAI_DONG_CO != null && x.LOAI_DONG_CO.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (x.DUNG_TICH != null && x.DUNG_TICH.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (x.CONG_SUAT != null && x.CONG_SUAT.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (x.HOP_SO != null && x.HOP_SO.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (x.NHIEN_LIEU != null && x.NHIEN_LIEU.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            var totalCount = items.Count;
            var thongSos = items.OrderBy(x => x.IDTS).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(thongSos);
        }

        // GET: /Admin/ThongSoKyThuat/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP);
            return View(item);
        }

        // GET: /Admin/ThongSoKyThuat/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThongSoKyThuat model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem sản phẩm đã có thông số chưa
                if (model.IDSP > 0 && await _service.ExistsBySanPhamIdAsync(model.IDSP))
                {
                    ModelState.AddModelError("IDSP", "Sản phẩm này đã có thông số kỹ thuật. Vui lòng sửa thay vì tạo mới.");
                    ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
                    return View(model);
                }
                var all = await _service.GetAllAsync();
                model.IDTS = all.Any() ? all.Max(x => x.IDTS) + 1 : 1;
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/ThongSoKyThuat/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ThongSoKyThuat model)
        {
            if (id != model.IDTS) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/ThongSoKyThuat/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP);
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