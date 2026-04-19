using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class BinhLuanController : Controller
    {
        private readonly BinhLuanService _binhLuanService;
        private readonly SanPhamService _sanPhamService;
        private readonly TaiKhoanService _taiKhoanService;

        public BinhLuanController(BinhLuanService binhLuanService, SanPhamService sanPhamService, TaiKhoanService taiKhoanService)
        {
            _binhLuanService = binhLuanService;
            _sanPhamService = sanPhamService;
            _taiKhoanService = taiKhoanService;
        }

        // GET: /Admin/BinhLuan
        public async Task<IActionResult> Index(string search, int? rating, int page = 1, int pageSize = 10)
        {
            var items = await _binhLuanService.GetAllAsync();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDSP == id || x.IDTK == id).ToList();
                else
                    items = items.Where(x => x.NOIDUNG != null && x.NOIDUNG.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Lọc theo số sao
            if (rating.HasValue && rating.Value > 0)
                items = items.Where(x => x.SOSAO == rating.Value).ToList();

            var totalCount = items.Count;
            var binhLuans = items.OrderByDescending(x => x.NGAYTAO).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();

            ViewBag.Search = search;
            ViewBag.Rating = rating;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(binhLuans);
        }

        // GET: /Admin/BinhLuan/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var binhLuan = await _binhLuanService.GetByIdAsync(id);
            if (binhLuan == null) return NotFound();

            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(binhLuan.IDSP);
            ViewBag.TaiKhoan = await _taiKhoanService.GetByIdAsync(binhLuan.IDTK);
            return View(binhLuan);
        }

        // GET: /Admin/BinhLuan/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var binhLuan = await _binhLuanService.GetByIdAsync(id);
            if (binhLuan == null) return NotFound();

            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(binhLuan.IDSP);
            ViewBag.TaiKhoan = await _taiKhoanService.GetByIdAsync(binhLuan.IDTK);
            return View(binhLuan);
        }

        // POST: /Admin/BinhLuan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _binhLuanService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa nhiều bình luận
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(int[] ids)
        {
            if (ids != null && ids.Any())
            {
                foreach (var id in ids)
                    await _binhLuanService.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}