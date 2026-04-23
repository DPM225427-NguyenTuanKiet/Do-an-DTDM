using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using CarShop.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

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
            // ✅ SỬ DỤNG ID TÀI KHOẢN ĐỂ ĐỒNG BỘ VỚI GIỎ HÀNG
            var userIdStr = User.FindFirstValue("IDTK");
            if (!string.IsNullOrEmpty(userIdStr))
            {
                var kh = await _khachHangService.GetByTaiKhoanIdAsync(int.Parse(userIdStr));
                if (kh != null) return kh.IDKH;
            }

            // Backup dự phòng (nếu IDTK bị lỗi thì dùng Email)
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return null;
            var khByEmail = await _khachHangService.GetByEmailAsync(userEmail);
            return khByEmail?.IDKH;
        }

        // Lịch sử đơn hàng
        public async Task<IActionResult> History()
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == null || khachHangId.Value == 0)
            {
                return View(new List<DonHang>());
            }

            var orders = await _donHangService.GetByKhachHangIdAsync(khachHangId.Value);

            // Sắp xếp đơn đặt mới nhất lên đầu tiên
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