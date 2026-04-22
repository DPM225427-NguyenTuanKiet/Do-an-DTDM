using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using CarShop.Extensions;
using System.Security.Claims;

namespace CarShop.Controllers
{
    [Authorize]
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

        private async Task<int?> GetKhachHangId()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return null;
            var kh = await _khachHangService.GetByEmailAsync(userEmail);
            return kh?.IDKH;
        }

        // Lịch sử đơn hàng
        public async Task<IActionResult> History()
        {
            var khachHangId = await GetKhachHangId();

            // SỬA LỖI: Nếu chưa có thông tin khách hàng, yêu cầu cập nhật Profile thay vì lặp lại History
            if (khachHangId == null || khachHangId.Value == 0)
                return RedirectToAction("History", "DonHang");

            var orders = await _donHangService.GetByKhachHangIdAsync(khachHangId.Value);

            // Sắp xếp đơn mới nhất lên đầu
            orders = orders.OrderByDescending(o => o.NGAYDAT).ToList();
            return View(orders);
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _donHangService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var khachHangId = await GetKhachHangId();
            if (khachHangId == null || order.IDKH != khachHangId.Value) return Forbid();

            var productIds = order.CHITIET.Select(c => c.IDSP).Distinct();
            var products = await _sanPhamService.GetAllAsync();
            ViewBag.Products = products.Where(p => productIds.Contains(p.IDSP)).ToDictionary(p => p.IDSP);

            return View(order);
        }

        // Trang báo đặt hàng thành công
        public IActionResult OrderSuccess(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
    }
}