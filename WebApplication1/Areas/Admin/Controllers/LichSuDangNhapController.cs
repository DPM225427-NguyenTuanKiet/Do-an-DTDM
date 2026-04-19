using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class LichSuDangNhapController : Controller
    {
        private readonly LichSuDangNhapService _service;
        private readonly TaiKhoanService _taiKhoanService;

        public LichSuDangNhapController(LichSuDangNhapService service, TaiKhoanService taiKhoanService)
        {
            _service = service;
            _taiKhoanService = taiKhoanService;
        }

        // GET: /Admin/LichSuDangNhap
        public async Task<IActionResult> Index(string search, int? trangThai, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDLS == id || x.IDTK == id).ToList();
                else
                    items = items.Where(x => (x.IP != null && x.IP.Contains(search)) ||
                                             (x.THIETBI != null && x.THIETBI.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            if (trangThai.HasValue)
                items = items.Where(x => x.TRANGTHAI == (trangThai.Value == 1)).ToList();

            var totalCount = items.Count;
            var logs = items.OrderByDescending(x => x.THOIGIAN).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(logs);
        }

        // GET: /Admin/LichSuDangNhap/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.TaiKhoan = await _taiKhoanService.GetByIdAsync(item.IDTK);
            return View(item);
        }

        // GET: /Admin/LichSuDangNhap/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.TaiKhoan = await _taiKhoanService.GetByIdAsync(item.IDTK);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa nhiều log
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(int[] ids)
        {
            if (ids != null && ids.Any())
            {
                foreach (var id in ids)
                    await _service.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa log cũ hơn ngày chỉ định
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOldLogs(DateTime beforeDate)
        {
            var allLogs = await _service.GetAllAsync();
            var oldLogs = allLogs.Where(x => x.THOIGIAN < beforeDate).ToList();
            foreach (var log in oldLogs)
                await _service.DeleteAsync(log.IDLS);
            return RedirectToAction(nameof(Index));
        }
    }
}