using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DonHangController : Controller
    {
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;
        private readonly SanPhamService _sanPhamService;

        public DonHangController(DonHangService donHangService, KhachHangService khachHangService, SanPhamService sanPhamService)
        {
            _donHangService = donHangService;
            _khachHangService = khachHangService;
            _sanPhamService = sanPhamService;
        }

        // GET: /Admin/DonHang
        public async Task<IActionResult> Index(string search, string trangThai, int page = 1, int pageSize = 10)
        {
            var orders = await _donHangService.GetAllAsync();

            // Tìm kiếm theo IDDH hoặc IDKH
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    orders = orders.Where(x => x.IDDH == id || x.IDKH == id).ToList();
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThai) && trangThai != "all")
                orders = orders.Where(x => x.TRANGTHAI == trangThai).ToList();

            var totalCount = orders.Count;
            var items = orders.OrderByDescending(x => x.NGAYDAT).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(items);
        }

        // GET: /Admin/DonHang/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _donHangService.GetByIdAsync(id);
            if (order == null) return NotFound();

            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(order.IDKH);
            var productIds = order.CHITIET.Select(c => c.IDSP).Distinct();
            var products = await _sanPhamService.GetAllAsync();
            ViewBag.Products = products.Where(p => productIds.Contains(p.IDSP)).ToDictionary(p => p.IDSP);
            return View(order);
        }

        // POST: Cập nhật trạng thái đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _donHangService.UpdateStatusAsync(id, status);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/DonHang/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _donHangService.GetByIdAsync(id);
            if (order == null) return NotFound();
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(order.IDKH);
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(int id)
        {
            var order = await _donHangService.GetByIdAsync(id);
            if (order == null) return NotFound();

            // Cập nhật trạng thái duyệt
            order.DA_DUYET = true;
            order.TRANGTHAI = "Đã duyệt";
            await _donHangService.UpdateAsync(order.IDDH, order); // Sửa: truyền ID và entity

            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _donHangService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}