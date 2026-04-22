using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarShop.Controllers
{
    [Authorize]
    public class LoaiKhachHangController : Controller
    {
        private readonly LoaiGiaService _loaiGiaService;
        private readonly DiemTichLuyService _diemService;
        private readonly KhachHangService _khachHangService;

        public LoaiKhachHangController(LoaiGiaService loaiGiaService, DiemTichLuyService diemService, KhachHangService khachHangService)
        {
            _loaiGiaService = loaiGiaService;
            _diemService = diemService;
            _khachHangService = khachHangService;
        }

        private async Task<int?> GetKhachHangId()
        {
            var userId = User.FindFirstValue("IDTK");
            if (string.IsNullOrEmpty(userId)) return null;
            var kh = await _khachHangService.GetByTaiKhoanIdAsync(int.Parse(userId));
            return kh?.IDKH;
        }

        public async Task<IActionResult> Index()
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return RedirectToAction("Index", "LoaiKhachHang");

            var diem = await _diemService.GetByKhachHangIdAsync(idkh.Value);
            var diemHienTai = diem?.DIEMHIENTAI ?? 0;

            // Tìm hạng phù hợp dựa trên điểm
            var hang = await _loaiGiaService.GetByDiemToiThieuAsync(diemHienTai);
            if (hang == null) hang = new LoaiGia { TENLOAI = "Thành viên", DIEMTHUONG = 0, GIAMGIA = 0 };

            ViewBag.Diem = diemHienTai;
            return View(hang);
        }
    }
}