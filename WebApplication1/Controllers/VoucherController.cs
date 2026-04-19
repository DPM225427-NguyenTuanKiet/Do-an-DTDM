using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Services;
using System.Security.Claims;

namespace CarShop.Controllers
{
    [Authorize]
    public class VoucherController : Controller
    {
        private readonly VoucherService _voucherService;
        private readonly LichSuDoiDiemService _lichSuDoiDiemService;
        private readonly KhachHangService _khachHangService;

        public VoucherController(VoucherService voucherService, LichSuDoiDiemService lichSuDoiDiemService, KhachHangService khachHangService)
        {
            _voucherService = voucherService;
            _lichSuDoiDiemService = lichSuDoiDiemService;
            _khachHangService = khachHangService;
        }

        private async Task<int> GetKhachHangId()
        {
            var userId = int.Parse(User.FindFirstValue("IDTK"));
            var kh = await _khachHangService.GetByTaiKhoanIdAsync(userId);
            return kh?.IDKH ?? 0;
        }

        public async Task<IActionResult> Index()
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == 0) return RedirectToAction("Profile", "Account");

            // Lấy các voucher đã đổi (từ lich su doi diem)
            var exchanges = await _lichSuDoiDiemService.GetByKhachHangIdAsync(khachHangId);
            var voucherIds = exchanges.Where(e => e.IDV.HasValue).Select(e => e.IDV.Value).Distinct();
            var vouchers = await _voucherService.GetAllAsync();
            var myVouchers = vouchers.Where(v => voucherIds.Contains(v.IDV)).ToList();

            return View(myVouchers);
        }
    }
}