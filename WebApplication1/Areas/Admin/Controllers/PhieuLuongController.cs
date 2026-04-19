using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class PhieuLuongController : Controller
    {
        private readonly PhieuLuongService _service;
        private readonly NhanVienService _nhanVienService;

        public PhieuLuongController(PhieuLuongService service, NhanVienService nhanVienService)
        {
            _service = service;
            _nhanVienService = nhanVienService;
        }

        // GET: /Admin/PhieuLuong
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDNV == id || x.IDPL == id).ToList();
                else
                    items = items.Where(x => (x.TRANG_THAI != null && x.TRANG_THAI.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (x.THANG.ToString().Contains(search)) ||
                                             (x.NAM.ToString().Contains(search))).ToList();
            }
            var totalCount = items.Count;
            var phieuLuongs = items.OrderByDescending(x => x.NAM).ThenByDescending(x => x.THANG).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(phieuLuongs);
        }

        // GET: /Admin/PhieuLuong/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhanVien = await _nhanVienService.GetByIdAsync(item.IDNV ?? 0);
            return View(item);
        }

        // GET: /Admin/PhieuLuong/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuLuong model)
        {
            if (ModelState.IsValid)
            {
                // Tự động tính tổng lĩnh
                model.TONG_LINH = (model.LUONG_CO_BAN ?? 0) + (model.HOA_HONG ?? 0) + (model.TIEN_THUONG_DIEM ?? 0);
                // Có thể cộng thêm lương overtime nếu cần (dựa vào SO_GIO_LAM)
                // model.TONG_LINH += (model.SO_GIO_LAM ?? 0) * 50000; // ví dụ

                var all = await _service.GetAllAsync();
                model.IDPL = all.Any() ? all.Max(x => x.IDPL) + 1 : 1;
                model.NGAY_THANH_TOAN = DateTime.Now;
                await _service.CreateAsync(model);
                // Cập nhật tổng lương của nhân viên
                await _nhanVienService.RecalculateTotalSalary(model.IDNV ?? 0);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/PhieuLuong/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhieuLuong model)
        {
            if (id != model.IDPL) return BadRequest();
            if (ModelState.IsValid)
            {
                model.TONG_LINH = (model.LUONG_CO_BAN ?? 0) + (model.HOA_HONG ?? 0) + (model.TIEN_THUONG_DIEM ?? 0);
                await _service.UpdateAsync(id, model);
                await _nhanVienService.RecalculateTotalSalary(model.IDNV ?? 0);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/PhieuLuong/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhanVien = await _nhanVienService.GetByIdAsync(item.IDNV ?? 0);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _service.GetByIdAsync(id);
            int? idNV = item?.IDNV;
            await _service.DeleteAsync(id);
            if (idNV.HasValue)
                await _nhanVienService.RecalculateTotalSalary(idNV.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}