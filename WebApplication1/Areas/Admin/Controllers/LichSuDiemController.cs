using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class LichSuDiemController : Controller
    {
        private readonly LichSuDiemService _lichSuDiemService;
        private readonly KhachHangService _khachHangService;
        private readonly DonHangService _donHangService;

        public LichSuDiemController(LichSuDiemService lichSuDiemService, KhachHangService khachHangService, DonHangService donHangService)
        {
            _lichSuDiemService = lichSuDiemService;
            _khachHangService = khachHangService;
            _donHangService = donHangService;
        }

        // GET: /Admin/LichSuDiem
        public async Task<IActionResult> Index(string search, string loai, int page = 1, int pageSize = 10)
        {
            var items = await _lichSuDiemService.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDKH == id || x.IDDH == id || x.ID == id).ToList();
                else
                    items = items.Where(x => x.LOAI.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                             (x.GHICHU != null && x.GHICHU.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            if (!string.IsNullOrEmpty(loai) && loai != "all")
                items = items.Where(x => x.LOAI == loai).ToList();

            var totalCount = items.Count;
            var lichSuDiems = items.OrderByDescending(x => x.NGAY).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Loai = loai;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(lichSuDiems);
        }

        // GET: /Admin/LichSuDiem/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _lichSuDiemService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(item.IDKH);
            ViewBag.DonHang = item.IDDH.HasValue ? await _donHangService.GetByIdAsync(item.IDDH.Value) : null;
            return View(item);
        }

        // GET: /Admin/LichSuDiem/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _lichSuDiemService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(item.IDKH);
            ViewBag.DonHang = item.IDDH.HasValue ? await _donHangService.GetByIdAsync(item.IDDH.Value) : null;
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _lichSuDiemService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}