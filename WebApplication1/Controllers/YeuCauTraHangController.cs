using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace CarShop.Controllers
{
    [Authorize]
    public class YeuCauTraHangController : Controller
    {
        private readonly YeuCauTraHangService _yeuCauService;
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;
        private readonly SanPhamService _sanPhamService;

        public YeuCauTraHangController(YeuCauTraHangService yeuCauService, DonHangService donHangService,
                                        KhachHangService khachHangService, SanPhamService sanPhamService)
        {
            _yeuCauService = yeuCauService;
            _donHangService = donHangService;
            _khachHangService = khachHangService;
            _sanPhamService = sanPhamService;
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
            var khachHangId = await GetKhachHangId();
            if (khachHangId == null) return RedirectToAction("Login", "Account");

            var requests = await _yeuCauService.GetByKhachHangIdAsync(khachHangId.Value);
            return View(requests);
        }

        public async Task<IActionResult> Create()
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == null) return RedirectToAction("Login", "Account");

            var orders = await _donHangService.GetByKhachHangIdAsync(khachHangId.Value);
            orders = orders.Where(o => o.TRANGTHAI == "Đã giao").ToList();
            ViewBag.Orders = orders;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int orderId, string reason, List<int> productIds, List<int> quantities)
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == null) return RedirectToAction("Login", "Account");

            var order = await _donHangService.GetByIdAsync(orderId);
            if (order == null || order.IDKH != khachHangId || order.TRANGTHAI != "Đã giao")
                return BadRequest("Đơn hàng không hợp lệ");

            var all = await _yeuCauService.GetAllAsync();
            var newId = all.Any() ? all.Max(r => r.IDYCTH) + 1 : 1;

            var request = new YeuCauTraHang
            {
                IDYCTH = newId,
                IDDH = orderId,
                IDKH = khachHangId.Value,
                NGAYYEUCAU = DateTime.Now,
                LYDO = reason,
                TRANGTHAI = "Đang xử lý",
                CHITIET = new List<ChiTietTraHang>()
            };

            for (int i = 0; i < productIds.Count; i++)
            {
                var detail = order.CHITIET.FirstOrDefault(c => c.IDSP == productIds[i]);
                if (detail != null && quantities[i] > 0 && quantities[i] <= detail.SOLUONG)
                {
                    request.CHITIET.Add(new ChiTietTraHang
                    {
                        IDSP = productIds[i],
                        SOLUONG = quantities[i],
                        DONGIA = detail.DONGIA,
                        TIENHOAN = detail.DONGIA * quantities[i]
                    });
                }
            }

            if (!request.CHITIET.Any())
                return BadRequest("Chưa chọn sản phẩm hợp lệ");

            await _yeuCauService.CreateAsync(request);
            TempData["Success"] = "Yêu cầu trả hàng đã được gửi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _yeuCauService.GetByIdAsync(id);
            if (request == null) return NotFound();

            var khachHangId = await GetKhachHangId();
            if (khachHangId == null || request.IDKH != khachHangId) return Forbid();

            ViewBag.Order = await _donHangService.GetByIdAsync(request.IDDH);
            var productIds = request.CHITIET.Select(c => c.IDSP).Distinct();
            var products = await _sanPhamService.GetAllAsync();
            ViewBag.Products = products.Where(p => productIds.Contains(p.IDSP)).ToDictionary(p => p.IDSP);

            return View(request);
        }
    }
}