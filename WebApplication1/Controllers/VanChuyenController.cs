using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic; // Thêm để dùng List

namespace CarShop.Controllers
{
    [Authorize]
    public class VanChuyenController : Controller
    {
        private readonly VanChuyenService _vanChuyenService;
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;

        public VanChuyenController(VanChuyenService vanChuyenService, DonHangService donHangService, KhachHangService khachHangService)
        {
            _vanChuyenService = vanChuyenService;
            _donHangService = donHangService;
            _khachHangService = khachHangService;
        }

        private async Task<int?> GetKhachHangId()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return null;
            var kh = await _khachHangService.GetByEmailAsync(userEmail);
            return kh?.IDKH;
        }

        // Danh sách vận chuyển
        public async Task<IActionResult> List()
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return RedirectToAction("List", "VanChuyen");

            var donHangs = await _donHangService.GetByKhachHangIdAsync(idkh.Value);
            var donHangIds = donHangs.Select(d => d.IDDH).ToList();
            // Sửa: gọi GetByIdsAsync thay vì GetByIdAsync
            var vanChuyens = await _vanChuyenService.GetByIdsAsync(donHangIds);
            return View(vanChuyens);
        }

        // Chi tiết vận chuyển của một đơn hàng
        public async Task<IActionResult> Details(int orderId)
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return RedirectToAction("Index", "VanChuyen");

            var donHang = await _donHangService.GetByIdAsync(orderId);
            if (donHang == null || donHang.IDKH != idkh) return NotFound();

            var vanChuyen = await _vanChuyenService.GetByDonHangIdAsync(orderId);
            if (vanChuyen == null) return NotFound();

            return View(vanChuyen);
        }
    }
}