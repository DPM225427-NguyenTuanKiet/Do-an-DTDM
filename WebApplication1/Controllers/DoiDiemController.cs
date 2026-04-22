using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace CarShop.Controllers
{
    [Authorize]
    public class DoiDiemController : Controller
    {
        private readonly DiemTichLuyService _diemService;
        private readonly VoucherService _voucherService;
        private readonly QuaTangService _quaTangService;
        private readonly LichSuDoiDiemService _lichSuService;
        private readonly KhachHangService _khachHangService;

        public DoiDiemController(DiemTichLuyService diemService, VoucherService voucherService,
                                         QuaTangService quaTangService, LichSuDoiDiemService lichSuService,
                                         KhachHangService khachHangService)
        {
            _diemService = diemService;
            _voucherService = voucherService;
            _quaTangService = quaTangService;
            _lichSuService = lichSuService;
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
            if (idkh == null) return RedirectToAction("Index", "DoiDiem");

            var diem = await _diemService.GetByKhachHangIdAsync(idkh.Value);
            ViewBag.DiemHienTai = diem?.DIEMHIENTAI ?? 0;

            var vouchers = await _voucherService.GetAllAsync();
            var gifts = await _quaTangService.GetAllAsync();

            ViewBag.Vouchers = vouchers.Where(v => v.TRANGTHAI && v.SOLUONG > 0 && v.NGAYKETTHUC >= DateTime.Now).ToList();
            ViewBag.Gifts = gifts.Where(g => g.TRANGTHAI && g.SOLUONG > 0).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoiVoucher(int voucherId)
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return Json(new { success = false, message = "Vui lòng đăng nhập" });

            var voucher = await _voucherService.GetByIdAsync(voucherId);
            if (voucher == null || !voucher.TRANGTHAI || voucher.SOLUONG <= 0)
                return Json(new { success = false, message = "Voucher không khả dụng" });

            var diem = await _diemService.GetByKhachHangIdAsync(idkh.Value);
            if (diem == null || diem.DIEMHIENTAI < voucher.GIATRI)
                return Json(new { success = false, message = "Không đủ điểm" });

            // Trừ điểm
            diem.DIEMHIENTAI -= (int)voucher.GIATRI;
            diem.TONGDIEMDADUNG = (diem.TONGDIEMDADUNG ?? 0) + (int)voucher.GIATRI;
            diem.NGAYCAPNHAT = DateTime.Now;
            await _diemService.UpdateAsync(idkh.Value, diem);

            // Giảm số lượng voucher
            voucher.SOLUONG -= 1;
            await _voucherService.UpdateAsync(voucherId, voucher);

            // Ghi lịch sử
            var all = await _lichSuService.GetAllAsync();
            var newId = all.Any() ? all.Max(x => x.ID) + 1 : 1;
            var exchange = new LichSuDoiDiem
            {
                ID = newId,
                IDKH = idkh.Value,
                IDV = voucherId,
                SODIEM_DOI = (int)voucher.GIATRI,
                SOTIENGIAM = voucher.LOAI == "Tiền" ? voucher.GIATRI : null,
                NGAYDOI = DateTime.Now,
                TRANGTHAI = "Đã xử lý"
            };
            await _lichSuService.CreateAsync(exchange);

            return Json(new { success = true, message = "Đổi voucher thành công" });
        }

        [HttpPost]
        public async Task<IActionResult> DoiQuaTang(int giftId)
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return Json(new { success = false, message = "Vui lòng đăng nhập" });

            var gift = await _quaTangService.GetByIdAsync(giftId);
            if (gift == null || !gift.TRANGTHAI || gift.SOLUONG <= 0)
                return Json(new { success = false, message = "Quà tặng không khả dụng" });

            var diem = await _diemService.GetByKhachHangIdAsync(idkh.Value);
            if (diem == null || diem.DIEMHIENTAI < gift.DIEM_DOI)
                return Json(new { success = false, message = "Không đủ điểm" });

            // Trừ điểm
            diem.DIEMHIENTAI -= gift.DIEM_DOI;
            diem.TONGDIEMDADUNG = (diem.TONGDIEMDADUNG ?? 0) + gift.DIEM_DOI;
            diem.NGAYCAPNHAT = DateTime.Now;
            await _diemService.UpdateAsync(idkh.Value, diem);

            // Giảm số lượng quà
            gift.SOLUONG -= 1;
            await _quaTangService.UpdateAsync(giftId, gift);

            // Ghi lịch sử
            var all = await _lichSuService.GetAllAsync();
            var newId = all.Any() ? all.Max(x => x.ID) + 1 : 1;
            var exchange = new LichSuDoiDiem
            {
                ID = newId,
                IDKH = idkh.Value,
                IDQT = giftId,
                SODIEM_DOI = gift.DIEM_DOI,
                NGAYDOI = DateTime.Now,
                TRANGTHAI = "Đã xử lý"
            };
            await _lichSuService.CreateAsync(exchange);

            return Json(new { success = true, message = "Đổi quà tặng thành công" });
        }
    }
}