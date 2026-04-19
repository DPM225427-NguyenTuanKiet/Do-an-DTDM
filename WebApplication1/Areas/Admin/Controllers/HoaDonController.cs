using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class HoaDonController : Controller
    {
        private readonly HoaDonService _hoaDonService;
        private readonly DonHangService _donHangService;
        private readonly NhanVienService _nhanVienService;

        public HoaDonController(HoaDonService hoaDonService, DonHangService donHangService, NhanVienService nhanVienService)
        {
            _hoaDonService = hoaDonService;
            _donHangService = donHangService;
            _nhanVienService = nhanVienService;
        }

        // GET: /Admin/HoaDon
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _hoaDonService.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDHD == id || x.IDDH == id || x.IDNV == id).ToList();
                else
                    items = items.Where(x => (x.HINHTHUCTHANHTOAN != null && x.HINHTHUCTHANHTOAN.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (x.TRANGTHAI != null && x.TRANGTHAI.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (x.TRANGTHAITHANHTOAN != null && x.TRANGTHAITHANHTOAN.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            var totalCount = items.Count;
            var hoaDons = items.OrderByDescending(x => x.NGAYLAP).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(hoaDons);
        }

        // GET: /Admin/HoaDon/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _hoaDonService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DonHang = await _donHangService.GetByIdAsync(item.IDDH ?? 0);
            ViewBag.NhanVien = await _nhanVienService.GetByIdAsync(item.IDNV ?? 0);
            return View(item);
        }

        // GET: /Admin/HoaDon/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HoaDon model)
        {
            if (ModelState.IsValid)
            {
                var all = await _hoaDonService.GetAllAsync();
                model.IDHD = all.Any() ? all.Max(x => x.IDHD) + 1 : 1;
                model.NGAYLAP = DateTime.Now;
                await _hoaDonService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/HoaDon/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _hoaDonService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HoaDon model)
        {
            if (id != model.IDHD) return BadRequest();
            if (ModelState.IsValid)
            {
                // Giữ nguyên ngày lập cũ nếu không muốn thay đổi
                var existing = await _hoaDonService.GetByIdAsync(id);
                if (existing != null) model.NGAYLAP = existing.NGAYLAP;
                await _hoaDonService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/HoaDon/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _hoaDonService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DonHang = await _donHangService.GetByIdAsync(item.IDDH ?? 0);
            ViewBag.NhanVien = await _nhanVienService.GetByIdAsync(item.IDNV ?? 0);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _hoaDonService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}