using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Services;
using System.Security.Claims;

namespace CarShop.Controllers
{
    [Authorize]
    public class DiemTichLuyController : Controller
    {
        private readonly DiemTichLuyService _diemService;
        private readonly KhachHangService _khachHangService;
        private readonly LichSuDiemService _lichSuDiemService;

        public DiemTichLuyController(DiemTichLuyService diemService, KhachHangService khachHangService, LichSuDiemService lichSuDiemService)
        {
            _diemService = diemService;
            _khachHangService = khachHangService;
            _lichSuDiemService = lichSuDiemService;
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

            var diem = await _diemService.GetByKhachHangIdAsync(khachHangId);
            if (diem == null)
            {
                diem = new Models.DiemTichLuy
                {
                    IDKH = khachHangId,
                    DIEMHIENTAI = 0,
                    TONGDIEMDADUNG = 0,
                    NGAYCAPNHAT = DateTime.Now
                };
                await _diemService.CreateAsync(diem);
            }

            var lichSu = await _lichSuDiemService.GetByKhachHangIdAsync(khachHangId);
            ViewBag.LichSu = lichSu.OrderByDescending(l => l.NGAY).Take(20).ToList();

            return View(diem);
        }
    }
}